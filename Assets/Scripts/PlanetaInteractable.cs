using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.SceneManagement;
using TMPro;

[RequireComponent(typeof(XRSimpleInteractable))]
public class PlanetaInteractable : MonoBehaviour
{
    [Header("Configuración de selección")]
    public float tiempoRequerido = 3f;

    [Header("Configuración del Portal VR")]
    [Tooltip("Nombre EXACTO de la escena a cargar para este planeta en Build Settings")]
    public string nombreEscenaDestino;

    [Header("UI Compartida (arrastrá el mismo panel para todos los planetas)")]
    public GameObject panelInformativo;
    public TextMeshProUGUI textoTitulo;
    public TextMeshProUGUI textoInfo;

    [Header("Opcional: barra de progreso")]
    public UnityEngine.UI.Image barraProgreso;

    private XRSimpleInteractable interactable;
    private float tiempoPresionado = 0f;
    private bool presionando = false;
    private bool yaMostrado = false;
    private Transform camaraPrincipal;

    void Awake()
    {
        interactable = GetComponent<XRSimpleInteractable>();
    }

    void Start()
    {
        BuscarCamara();
    }

    // Método de soporte para re-vincular los ojos del jugador al cambiar de escena
    void BuscarCamara()
    {
        if (Camera.main != null)
        {
            camaraPrincipal = Camera.main.transform;
        }
    }

    void OnEnable()
    {
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
        if (yaMostrado && !string.IsNullOrEmpty(nombreEscenaDestino))
        {
            ViajarAEscene();
            return;
        }

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
        // Si la cámara es null o se destruyó en el cambio de escena, la volvemos a buscar en caliente
        if (camaraPrincipal == null)
        {
            BuscarCamara();
        }

        // Rotación estable del cartel hacia la cámara del usuario (Billboard inmersivo)
        if (panelInformativo != null && panelInformativo.activeSelf && camaraPrincipal != null)
        {
            panelInformativo.transform.LookAt(camaraPrincipal.position);
            panelInformativo.transform.Rotate(0, 180, 0);
        }

        if (!presionando || yaMostrado) return;

        tiempoPresionado += Time.deltaTime;

        if (barraProgreso != null)
            barraProgreso.fillAmount = tiempoPresionado / tiempoRequerido;

        if (tiempoPresionado >= tiempoRequerido)
        {
            yaMostrado = true;
            if (barraProgreso != null) barraProgreso.gameObject.SetActive(false);
            MostrarInfo();
        }
    }

    void MostrarInfo()
    {
        if (panelInformativo == null) return;

        // Si la cámara se desvinculó justo al activarse, la recuperamos antes de calcular la posición
        if (camaraPrincipal == null) BuscarCamara();

        Vector3 direccionHaciaCamara = camaraPrincipal != null ? (camaraPrincipal.position - transform.position).normalized : Vector3.back;
        Vector3 posicionCartel = transform.position + (Vector3.up * 3.0f) + (direccionHaciaCamara * 2.0f);

        panelInformativo.transform.position = posicionCartel;
        panelInformativo.SetActive(true);

        textoTitulo.text = ObtenerTitulo(gameObject.name);
        textoInfo.text = ObtenerDescripcion(gameObject.name);
    }

    void ViajarAEscene()
    {
        if (panelInformativo != null) panelInformativo.SetActive(false);

        if (interactable != null && interactable.interactionManager != null)
        {
            interactable.interactionManager.UnregisterInteractable((IXRInteractable)interactable);
        }

        SceneManager.LoadScene(nombreEscenaDestino, LoadSceneMode.Single);
    }

    string ObtenerTitulo(string nombreObjeto)
    {
        switch (nombreObjeto.ToLower())
        {
            case "sun": return "EL SOL";
            case "erath": return "LA TIERRA";
            case "moon": return "LA LUNA";
            case "mercury": return "MERCURIO";
            case "venus": return "VENUS";
            case "mars": return "MARTE";
            case "jupiter": return "JÚPITER";
            case "saturn": return "SATURNO";
            case "uranus": return "URANO";
            case "neptune": return "NEPTUNO";
            case "pluto": return "PLUTÓN";
            default: return nombreObjeto.ToUpper();
        }
    }

    string ObtenerDescripcion(string nombreObjeto)
    {
        switch (nombreObjeto.ToLower())
        {
            case "sun": return "Tipo: Estrella Enana Amarilla.\nEs el corazón de nuestro sistema y contiene el 99.8% de toda su masa.";
            case "erath": return "Tipo: Planeta Terrestre.\nNuestro hogar. Es el único mundo conocido en el universo que alberga agua líquida y vida.";
            case "moon": return "Tipo: Satélite Natural.\nAfecta las mareas de la Tierra y es el único cuerpo celeste que el ser humano ha pisado.";
            case "mercury": return "Tipo: Planeta Terrestre.\nEl más cercano al Sol. No tiene atmósfera, por lo que sus noches son gélidas y sus días abrasadores.";
            case "venus": return "Tipo: Planeta Terrestre.\nTiene un tamaño similar a la Tierra, pero su atmósfera atrapa tanto calor que es el planeta más caliente.";
            case "mars": return "Tipo: Planeta Terrestre.\nEl planeta rojo. Cubierto de óxido de hierro, alberga el volcán más grande del sistema solar.";
            case "jupiter": return "Tipo: Gigante Gaseoso.\nEl coloso del sistema, tan grande que podría contener a todos los demás planetas juntos.";
            case "saturn": return "Tipo: Gigante Gaseoso.\nFamoso por su espectacular sistema de anillos compuestos por miles de millones de pedazos de hielo.";
            case "uranus": return "Tipo: Gigante Helado.\nTiene la atmósfera más fría del sistema solar y rota completamente inclinado de lado.";
            case "neptune": return "Tipo: Gigante Helado.\nEl mundo más lejano de nuestro Sol. Es un lugar oscuro, frío y azotado por vientos supersónicos.";
            case "pluto": return "Tipo: Planeta Enano.\nUn mundo helado en los confines del sistema que posee un característico glaciar en forma de corazón.";
            default: return "Cuerpo celeste del Sistema Solar interactivo.";
        }
    }
}