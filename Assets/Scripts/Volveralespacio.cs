using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(XRSimpleInteractable))]
public class VolverAlEspacio : MonoBehaviour
{
    [Header("Configuración de Escena")]
    public string nombreEscenaSistemaSolar = "SistemaSolar";
    public float tiempoRequerido = 2f; // Se activa al mantener presionado 2 segundos

    [Header("Opcional: barra de progreso")]
    public UnityEngine.UI.Image barraProgreso;

    private XRSimpleInteractable interactable;
    private float tiempoPresionado = 0f;
    private bool presionando = false;
    private bool yaViajando = false;

    void Awake()
    {
        interactable = GetComponent<XRSimpleInteractable>();
    }

    void OnEnable()
    {
        // Escucha los eventos del rayo láser VR del XRSimpleInteractable
        interactable.selectEntered.AddListener(OnEmpezarPresionar);
        interactable.selectExited.AddListener(OnDejarDePresionar);
    }

    void OnDisable()
    {
        interactable.selectEntered.RemoveListener(OnEmpezarPresionar);
        interactable.selectExited.RemoveListener(OnDejarDePresionar);
    }

    void OnEmpezarPresionar(SelectEnterEventArgs args)
    {
        if (yaViajando) return;
        presionando = true;
        tiempoPresionado = 0f;
        if (barraProgreso != null) barraProgreso.gameObject.SetActive(true);
    }

    void OnDejarDePresionar(SelectExitEventArgs args)
    {
        presionando = false;
        tiempoPresionado = 0f;
        if (barraProgreso != null) barraProgreso.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!presionando || yaViajando) return;

        tiempoPresionado += Time.deltaTime;

        if (barraProgreso != null)
            barraProgreso.fillAmount = tiempoPresionado / tiempoRequerido;

        // Si completa los 2 segundos presionando el gatillo...
        if (tiempoPresionado >= tiempoRequerido)
        {
            Volver();
        }
    }

    public void Volver()
    {
        if (yaViajando) return;
        yaViajando = true;

        if (barraProgreso != null) barraProgreso.gameObject.SetActive(false);

        // Desvinculamos el cohete del sistema VR justo un frame antes de viajar
        XRSimpleInteractable interactable = GetComponent<XRSimpleInteractable>();
        if (interactable != null && interactable.interactionManager != null)
        {
            interactable.interactionManager.UnregisterInteractable((IXRInteractable)interactable);
        }

        SceneManager.LoadScene(nombreEscenaSistemaSolar, LoadSceneMode.Single);
    }
}