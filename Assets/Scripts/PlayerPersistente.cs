using UnityEngine;

/// <summary>
/// Poné este script en el objeto raíz del jugador (el que contiene el XR Origin
/// y el XR Device Simulator). Hace que sobreviva al cambio de escena, así el
/// tracking VR y los scripts como PlanetSelector no se pierden al viajar
/// del Sistema Solar a un planeta y viceversa.
/// </summary>
public class PlayerPersistente : MonoBehaviour
{
    private static PlayerPersistente instancia;

    void Awake()
    {
        // Si ya existe un jugador persistente (por ejemplo volviste a cargar
        // la escena del Sistema Solar), destruimos este duplicado.
        if (instancia != null && instancia != this)
        {
            Destroy(gameObject);
            return;
        }

        instancia = this;
        DontDestroyOnLoad(gameObject);
    }
}