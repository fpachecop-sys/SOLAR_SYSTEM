using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Poné este script en un objeto llamado "Narrador" DENTRO de cada escena
/// de planeta (no es persistente, es distinto en cada escena). Reproduce
/// un audio de voz automáticamente unos segundos después de cargar, y
/// opcionalmente muestra subtítulos en pantalla mientras habla.
/// </summary>
public class NarradorPlaneta : MonoBehaviour
{
    [Header("Audio de la narración (el mp3 que descargues)")]
    public AudioClip clipNarracion;
    [Range(0f, 1f)] public float volumen = 1f;

    [Header("Tiempo de espera antes de hablar (para que el jugador ya haya aparecido)")]
    public float retrasoInicial = 1.5f;

    [Header("Subtítulos (opcional, dejalo vacío si no querés)")]
    public TextMeshProUGUI textoSubtitulo;
    [TextArea(2, 5)]
    public string textoCompleto;

    [Header("Reproducir solo una vez POR PARTIDA (no se repite al volver a esta escena)")]
    public bool soloUnaVez = true;

    // Memoria estática: sobrevive a la recarga de escena (LoadScene la destruye
    // y recrea el objeto, pero esta lista static NO se reinicia con eso).
    // Se guarda por nombre de escena, así cada planeta se recuerda por separado.
    private static HashSet<string> escenasYaNarradas = new HashSet<string>();

    private AudioSource fuente;
    private bool yaSeReprodujo = false;

    void Awake()
    {
        fuente = gameObject.AddComponent<AudioSource>();
        fuente.playOnAwake = false;
        fuente.loop = false;

        // Si esta escena ya fue narrada antes en esta misma partida, lo marcamos
        // de entrada para que ReproducirNarracion() no vuelva a sonar.
        string escenaActual = SceneManager.GetActiveScene().name;
        if (soloUnaVez && escenasYaNarradas.Contains(escenaActual))
        {
            yaSeReprodujo = true;
        }
    }

    void Start()
    {
        StartCoroutine(ReproducirConRetraso());
    }

    IEnumerator ReproducirConRetraso()
    {
        yield return new WaitForSeconds(retrasoInicial);
        ReproducirNarracion();
    }

    public void ReproducirNarracion()
    {
        if (soloUnaVez && yaSeReprodujo) return;
        if (clipNarracion == null) return;

        yaSeReprodujo = true;
        escenasYaNarradas.Add(SceneManager.GetActiveScene().name);
        fuente.clip = clipNarracion;
        fuente.volume = volumen;
        fuente.Play();

        if (textoSubtitulo != null)
        {
            textoSubtitulo.text = textoCompleto;
            textoSubtitulo.gameObject.SetActive(true);
            StartCoroutine(OcultarSubtituloAlTerminar(clipNarracion.length));
        }
    }

    IEnumerator OcultarSubtituloAlTerminar(float duracion)
    {
        yield return new WaitForSeconds(duracion);
        if (textoSubtitulo != null)
            textoSubtitulo.gameObject.SetActive(false);
    }
}