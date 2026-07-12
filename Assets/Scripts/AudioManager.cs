using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Poné este script en un objeto único llamado "AudioManager" dentro de tu
/// escena "SistemaSolar" (la primera que carga el juego). Se vuelve persistente
/// automáticamente y cambia la música de fondo con un fundido suave (crossfade)
/// cada vez que entrás a una escena nueva (planeta, sistema solar, etc).
/// </summary>
public class AudioManager : MonoBehaviour
{
    [System.Serializable]
    public class MusicaPorEscena
    {
        public string nombreEscena; // debe coincidir EXACTO con el nombre de la escena
        public AudioClip clip;
        [Range(0f, 1f)] public float volumen = 0.5f;
    }

    public static AudioManager Instancia;

    [Header("Asigná una música para cada escena")]
    public List<MusicaPorEscena> listaMusica;

    [Header("Configuración")]
    public float duracionFundido = 1.5f;

    private AudioSource fuenteA;
    private AudioSource fuenteB;
    private bool usandoFuenteA = true;

    void Awake()
    {
        if (Instancia != null && Instancia != this)
        {
            Destroy(gameObject);
            return;
        }

        Instancia = this;
        DontDestroyOnLoad(gameObject);

        // Creamos dos AudioSource para poder cruzar el volumen entre pistas
        fuenteA = gameObject.AddComponent<AudioSource>();
        fuenteB = gameObject.AddComponent<AudioSource>();
        fuenteA.loop = true;
        fuenteB.loop = true;
        fuenteA.playOnAwake = false;
        fuenteB.playOnAwake = false;
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnEscenaCargada;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnEscenaCargada;
    }

    void Start()
    {
        // Reproduce la música de la primera escena al iniciar el juego
        CambiarMusica(SceneManager.GetActiveScene().name);
    }

    void OnEscenaCargada(Scene escena, LoadSceneMode modo)
    {
        CambiarMusica(escena.name);
    }

    void CambiarMusica(string nombreEscena)
    {
        MusicaPorEscena datos = listaMusica.Find(m => m.nombreEscena == nombreEscena);

        if (datos == null || datos.clip == null)
        {
            Debug.LogWarning("AudioManager: no hay música asignada para la escena " + nombreEscena);
            return;
        }

        AudioSource fuenteNueva = usandoFuenteA ? fuenteB : fuenteA;
        AudioSource fuenteVieja = usandoFuenteA ? fuenteA : fuenteB;
        usandoFuenteA = !usandoFuenteA;

        fuenteNueva.clip = datos.clip;
        fuenteNueva.volume = 0f;
        fuenteNueva.Play();

        StopAllCoroutines();
        StartCoroutine(Fundido(fuenteVieja, fuenteNueva, datos.volumen));
    }

    IEnumerator Fundido(AudioSource vieja, AudioSource nueva, float volumenObjetivo)
    {
        float tiempo = 0f;
        float volumenInicialVieja = vieja.volume;

        while (tiempo < duracionFundido)
        {
            tiempo += Time.deltaTime;
            float t = tiempo / duracionFundido;

            vieja.volume = Mathf.Lerp(volumenInicialVieja, 0f, t);
            nueva.volume = Mathf.Lerp(0f, volumenObjetivo, t);

            yield return null;
        }

        vieja.Stop();
        vieja.volume = 0f;
        nueva.volume = volumenObjetivo;
    }
}