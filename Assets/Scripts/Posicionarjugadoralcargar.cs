using System.Collections;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

public class PosicionarJugadorAlCargar : MonoBehaviour
{
    private CharacterController controllerJugador;
    private XROrigin origin;

    void Awake()
    {
        controllerJugador = GetComponentInChildren<CharacterController>();
        origin = GetComponentInChildren<XROrigin>();
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnEscenaCargada;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnEscenaCargada;
    }

    void OnEscenaCargada(Scene escena, LoadSceneMode modo)
    {
        if (origin != null) origin.gameObject.SetActive(false);
        if (controllerJugador != null) controllerJugador.enabled = false;

        StartCoroutine(SincronizarJugadorVR(escena));
    }

    IEnumerator SincronizarJugadorVR(Scene escena)
    {
        XRInteractionManager nuevoManager = Object.FindFirstObjectByType<XRInteractionManager>();
        GameObject spawn = GameObject.FindGameObjectWithTag("PlayerSpawn");

        if (spawn != null && origin != null)
        {
            transform.position = spawn.transform.position;
            transform.rotation = spawn.transform.rotation;

            origin.MoveCameraToWorldLocation(spawn.transform.position);
            Physics.SyncTransforms();
        }

        // Espera de seguridad de dos frames para la jerarquía persistente
        yield return null;
        yield return null;

        if (origin != null && origin.Camera != null)
        {
            origin.Camera.tag = "MainCamera";
            if (!origin.Camera.enabled) origin.Camera.enabled = true;
        }

        // 1. REPARAR EL EVENT SYSTEM EN CALIENTE
        InputSystemUIInputModule moduloUI = Object.FindFirstObjectByType<InputSystemUIInputModule>();
        if (moduloUI != null && origin != null)
        {
            moduloUI.xrTrackingOrigin = origin.transform;
        }

        // 2. REINICIAR ASSETS DE ACCIONES DESDE LA API GLOBAL
        InputActionManager inputManager = GetComponent<InputActionManager>();
        if (inputManager != null && inputManager.actionAssets != null)
        {
            foreach (var asset in inputManager.actionAssets)
            {
                if (asset != null)
                {
                    asset.Disable();
                    asset.Enable();
                }
            }
        }

        // 3. RESTAURAR EL MOVIMIENTO CONTINUO
        ActionBasedContinuousMoveProvider moveProvider = GetComponentInChildren<ActionBasedContinuousMoveProvider>(true);
        LocomotionSystem locomotion = GetComponentInChildren<LocomotionSystem>(true);

        if (moveProvider != null && origin != null)
        {
            if (origin.Camera != null) moveProvider.forwardSource = origin.Camera.transform;
            if (locomotion != null) moveProvider.system = locomotion;

            moveProvider.enabled = false;
            moveProvider.enabled = true;
        }

        // 4. RE-ENLAZAR EL INTERACTION MANAGER A LOS LÁSERES
        XRRayInteractor[] rayosInteractors = GetComponentsInChildren<XRRayInteractor>(true);
        foreach (var rayo in rayosInteractors)
        {
            if (rayo != null)
            {
                if (nuevoManager != null) rayo.interactionManager = nuevoManager;
                rayo.attachTransform = rayo.transform;
            }
        }

        // 5. RE-ENFOCAR EL MOTOR DE ENTRADAS GLOBALES
        InputSystem.Update();

        // 6. ENCENDER EL JUGADOR COMPLETAMENTE OPERATIVO
        if (origin != null) origin.gameObject.SetActive(true);
        if (controllerJugador != null)
        {
            controllerJugador.enabled = true;
            controllerJugador.Move(Vector3.zero);
        }

        // 7. REFRESCAR EL SIMULADOR DE TECLADO
        var simulador = GetComponentInChildren<UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation.XRDeviceSimulator>(true);
        if (simulador != null)
        {
            simulador.enabled = false;
            simulador.enabled = true;
        }

        Debug.Log($"[XRI Prefab Fixed] Regreso exitoso a {escena.name}. Input Action Asset re-habilitado correctamente.");
    }
}