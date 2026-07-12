using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Poné este script en el mismo objeto del planeta (o en un objeto invisible
/// más grande alrededor de él) que ya tiene su SphereCollider.
/// Cuando el jugador mira el planeta y presiona el botón de selección
/// (o se acerca demasiado), carga la escena de ese planeta.
/// </summary>
public class PortalAPlaneta : MonoBehaviour
{
    [Tooltip("Nombre EXACTO de la escena a cargar (debe estar en Build Settings)")]
    public string nombreEscenaDestino;

    [Tooltip("Distancia a la que se activa automáticamente el viaje (opcional)")]
    public float distanciaViaje = 3f;

    [Tooltip("Si está en true, hace falta presionar un botón. Si es false, se activa solo por cercanía.")]
    public bool requierePresionarBoton = true;

    private Transform jugador;
    private bool yaViajando = false;

    void Start()
    {
        if (Camera.main != null)
            jugador = Camera.main.transform;
    }

    void Update()
    {
        if (yaViajando || jugador == null) return;

        float distancia = Vector3.Distance(jugador.position, transform.position);

        if (distancia <= distanciaViaje)
        {
            if (!requierePresionarBoton)
            {
                Viajar();
            }
            else if (Input.GetKeyDown(KeyCode.E)) // en el Device Simulator, tecla E de prueba
            {
                Viajar();
            }
        }
    }

    public void Viajar()
    {
        if (string.IsNullOrEmpty(nombreEscenaDestino) || yaViajando) return;

        yaViajando = true;
        SceneManager.LoadScene(nombreEscenaDestino, LoadSceneMode.Single);
    }
}