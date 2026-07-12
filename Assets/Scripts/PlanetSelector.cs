using UnityEngine;
using TMPro;

public class PlanetSelector : MonoBehaviour
{
    public float distanciaActivacion = 45f;

    [Header("UI Componentes")]
    public GameObject panelInformativo;
    public TextMeshProUGUI textoTitulo;
    public TextMeshProUGUI textoInfo;

    [Header("VR - Origen del rayo")]
    [Tooltip("Arrastra aquí la cámara del XR Origin (Camera Offset > Main Camera). Si lo dejás vacío, usa este mismo transform.")]
    public Transform origenMirada;

    private GameObject planetaActualMirado;

    void Start()
    {
        if (panelInformativo != null) panelInformativo.SetActive(false);

        // Si no asignaste manualmente el origen del rayo, intenta usar la cámara principal
        if (origenMirada == null && Camera.main != null)
        {
            origenMirada = Camera.main.transform;
        }
    }

    void Update()
    {
        if (origenMirada == null) return;

        Ray ray = new Ray(origenMirada.position, origenMirada.forward);
        RaycastHit hit;

        // 1. Detección por el rayo de la mirada (centro de la cámara VR)
        if (Physics.Raycast(ray, out hit, distanciaActivacion))
        {
            GameObject objetoChocado = hit.collider.gameObject;

            if (EsPlanetaValido(objetoChocado.name))
            {
                if (objetoChocado != planetaActualMirado)
                {
                    planetaActualMirado = objetoChocado;

                    Vector3 puntoMirado = hit.point;
                    Vector3 posicionCartel = puntoMirado + (origenMirada.up * 2.5f) - (origenMirada.forward * 2f);

                    MostrarInfo(planetaActualMirado.name, posicionCartel);
                }
            }
        }
        else
        {
            // 2. Margen de lectura estable
            if (planetaActualMirado != null)
            {
                float distanciaAlPlaneta = Vector3.Distance(origenMirada.position, planetaActualMirado.transform.position);

                if (distanciaAlPlaneta > distanciaActivacion + 10f)
                {
                    ApagarPanel();
                }
            }
        }

        // El cartel siempre mira hacia el jugador (útil sobre todo en VR, con 2 ojos)
        if (panelInformativo != null && panelInformativo.activeSelf)
        {
            panelInformativo.transform.LookAt(origenMirada.position);
            panelInformativo.transform.Rotate(0, 180, 0);
        }
    }

    void ApagarPanel()
    {
        if (panelInformativo != null && panelInformativo.activeSelf)
        {
            panelInformativo.SetActive(false);
            planetaActualMirado = null;
        }
    }

    bool EsPlanetaValido(string nombre)
    {
        string n = nombre.ToLower();
        return (n == "sun" || n == "erath" || n == "moon" || n == "mercury" || n == "venus" ||
                n == "mars" || n == "jupiter" || n == "saturn" || n == "uranus" || n == "neptune" || n == "pluto");
    }

    void MostrarInfo(string nombreObjeto, Vector3 posicionFija)
    {
        if (panelInformativo == null) return;

        string tituloFormateado = "";
        string descripcion = "";

        switch (nombreObjeto.ToLower())
        {
            case "sun":
                tituloFormateado = "EL SOL";
                descripcion = "Tipo: Estrella Enana Amarilla.\nEs el corazón de nuestro sistema y contiene el 99.8% de toda su masa.";
                break;
            case "erath":
                tituloFormateado = "LA TIERRA";
                descripcion = "Tipo: Planeta Terrestre.\nNuestro hogar. Es el único mundo conocido en el universo que alberga agua líquida y vida.";
                break;
            case "moon":
                tituloFormateado = "LA LUNA";
                descripcion = "Tipo: Satélite Natural.\nAfecta las mareas de la Tierra y es el único cuerpo celeste que el ser humano ha pisado.";
                break;
            case "mercury":
                tituloFormateado = "MERCURIO";
                descripcion = "Tipo: Planeta Terrestre.\nEl más cercano al Sol. No tiene atmósfera, por lo que sus noches son gélidas y sus días abrasadores.";
                break;
            case "venus":
                tituloFormateado = "VENUS";
                descripcion = "Tipo: Planeta Terrestre.\nTiene un tamaño similar a la Tierra, pero su atmósfera atrapa tanto calor que es el planeta más caliente.";
                break;
            case "mars":
                tituloFormateado = "MARTE";
                descripcion = "Tipo: Planeta Terrestre.\nEl planeta rojo. Cubierto de óxido de hierro, alberga el volcán más grande del sistema solar.";
                break;
            case "jupiter":
                tituloFormateado = "JÚPITER";
                descripcion = "Tipo: Gigante Gaseoso.\nEl coloso del sistema, tan grande que podría contener a todos los demás planetas juntos.";
                break;
            case "saturn":
                tituloFormateado = "SATURNO";
                descripcion = "Tipo: Gigante Gaseoso.\nFamoso por su espectacular sistema de anillos compuestos por miles de millones de pedazos de hielo.";
                break;
            case "uranus":
                tituloFormateado = "URANO";
                descripcion = "Tipo: Gigante Helado.\nTiene la atmósfera más fría del sistema solar y rota completamente inclinado de lado.";
                break;
            case "neptune":
                tituloFormateado = "NEPTUNO";
                descripcion = "Tipo: Gigante Helado.\nEl mundo más lejano de nuestro Sol. Es un lugar oscuro, frío y azotado por vientos supersónicos.";
                break;
            case "pluto":
                tituloFormateado = "PLUTÓN";
                descripcion = "Tipo: Planeta Enano.\nUn mundo helado en los confines del sistema que posee un característico glaciar en forma de corazón.";
                break;
        }

        panelInformativo.transform.position = posicionFija;
        panelInformativo.SetActive(true);
        textoTitulo.text = tituloFormateado;
        textoInfo.text = descripcion;
    }
}