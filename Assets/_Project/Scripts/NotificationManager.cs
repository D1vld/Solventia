using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json;

public class NotificationManager : MonoBehaviour
{
    [Header("Configuración de Intervalos")]
    [SerializeField] private float checkInterval = 10f; // Intervalo en segundos para el polling (revisión periódica)

    [Header("Referencias de UI")]
    [SerializeField] private Transform content; // Transform del contenido en el ScrollView

    [Header("Prefab de Notificación")]
    [SerializeField] private GameObject notificationPrefab; // Prefab para la tarjeta de notificación

    [Header("Visibilizar de Notificaciones")]
    [SerializeField] private GameObject interfaz_principal; // Interfaz principal que quieres activar o desactivar
    [SerializeField] private GameObject interfaz_historial; // Interfaz de mensaje que quieres activar o desactivar

    [Header("Sonido de Notificación")]
    [SerializeField] private AudioSource notificationSound; // Sonido de notificación

    [Header("ID de la Notificación")]
    public int notificationID; // Mostrará el ID en el inspector

    private bool hasNewNotification = false; // Indica si hay una nueva notificación
    private bool notificationInstanciada = false; // Indica si una notificación ha sido instanciada
    private bool notificationClicked = false; // Indica si la notificación ha sido clicada
    private string notificationTitle; // Título de la notificación
    private string notificationDescription; // Descripción de la notificación
    private string notificationImageUrl; // URL de la imagen de la notificación
    private int notificationId; // ID de la notificación
    private string notificationUserType; // Tipo de usuario de la notificación (Agente o Usuario)
    private HashSet<int> shownNotificationIds = new HashSet<int>(); // Conjunto de IDs de notificaciones ya mostradas
    private List<GameObject> notificationInstances = new List<GameObject>(); // Lista para almacenar las instancias de notificaciones

    private bool interfazHistorialInstanciada = false; // Indica si la interfaz_historial ha sido instanciada

    void Start()
    {
        Debug.Log("START method called");
        StartCoroutine(CheckForNotificationsPeriodically());
        interfaz_principal.SetActive(false); // Asegurarse de que el canvas está inicialmente desactivado
        interfaz_historial.SetActive(false);
        // Asegurarse de que el sonido no se reproduzca al inicio
        if (notificationSound != null)
        {
            notificationSound.playOnAwake = false;
        }
    }

    // Coroutine para revisar periódicamente si hay nuevas notificaciones
    IEnumerator CheckForNotificationsPeriodically()
    {
        while (true)
        {
            yield return new WaitForSeconds(checkInterval); // Espera el intervalo especificado antes de revisar
            StartCoroutine(CheckForNewNotification()); // Inicia la revisión de nuevas notificaciones
        }
    }

    // Coroutine para revisar si hay nuevas notificaciones en el servidor
    IEnumerator CheckForNewNotification()
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get("https://triportation-backend.onrender.com/environments"))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log("Error checking for notifications: " + webRequest.error);
            }
            else
            {
                var jsonResponse = webRequest.downloadHandler.text;
                var environments = JsonConvert.DeserializeObject<List<EnvironmentData>>(jsonResponse);

                foreach (var environment in environments)
                {
                    if (environment.NotificationType != null && !shownNotificationIds.Contains(environment.id))
                    {
                        hasNewNotification = true;
                        notificationTitle = environment.name;
                        notificationDescription = environment.description;
                        notificationImageUrl = environment.img2D;
                        notificationId = environment.id;
                        notificationID = notificationId;  // Asigna el ID a la variable serializada para mostrarlo en el inspector
                        notificationUserType = environment.NotificationType;

                        shownNotificationIds.Add(notificationId); // Asegurarse de añadir el ID a la lista de mostrados
                        Debug.Log("Nueva notificación detectada: " + notificationTitle);

                        // Reproducir el sonido de notificación aquí
                        if (notificationSound != null && !notificationSound.isPlaying)
                        {
                            notificationSound.Play();
                            Debug.Log("Sonido de notificación reproducido.");
                        }

                        // Instanciar la notificación
                        AddNotificationItem(notificationTitle, notificationDescription, notificationImageUrl, notificationUserType);

                        // Mostrar el canvas emergente solo si la interfaz_historial no está activa y ha sido instanciada
                        if (!interfazHistorialInstanciada)
                        {
                            interfaz_principal.SetActive(true);
                        }

                        yield return new WaitForSeconds(1f); // Espera un momento antes de continuar para evitar solapamientos
                    }
                }
            }
        }
    }

    // Método que se llama cuando se instancia una nueva tarjeta de notificación
    public void InstanciaTarjeta()
    {
        if (hasNewNotification && !notificationInstanciada)
        {
            Debug.Log("Botón clicado");
            interfaz_principal.SetActive(false);
            interfaz_historial.SetActive(true);
            StartCoroutine(NotifyUser()); // Inicia la coroutine para notificar al usuario
            Debug.Log("NotifyUser coroutine iniciada");
        }
        else
        {
            Debug.Log("No hay nuevas notificaciones para mostrar o ya está instanciada.");
        }
    }

    // Coroutine para instanciar una notificación y mostrarla al usuario
    IEnumerator NotifyUser()
    {
        Debug.Log("Instanciando notificación");

        yield return null;
        notificationInstanciada = true;
        notificationClicked = true;

        Debug.Log("Notificación instanciada y lista para mostrar.");
    }

    // Método para agregar una nueva notificación a la UI
    void AddNotificationItem(string title, string description, string imageUrl, string userType)
    {
        Debug.Log("Instanciando prefab");

        var notificationInstance = Instantiate(notificationPrefab, content);
        if (notificationInstance == null)
        {
            Debug.LogError("Error al instanciar la notificación. notificationInstance es null.");
            return;
        }

        notificationInstances.Add(notificationInstance); // Añadir la instancia a la lista

        notificationInstance.SetActive(true); // Asegurarse de que el prefab esté activo
        Debug.Log("Notificación instanciada");

        // Obtener componentes necesarios
        var nombreText = notificationInstance.transform.Find("ID/Nombre")?.GetComponent<TextMeshProUGUI>();
        var estadoObject = notificationInstance.transform.Find("ID/Estado")?.GetComponent<Image>(); // Encontrar el objeto de estado
        var perfilImage = notificationInstance.transform.Find("ID/Perfil")?.GetComponent<Image>(); // Encontrar el componente de imagen de perfil

        RectTransform tarjetaRectTransform = notificationInstance.GetComponent<RectTransform>();
        RectTransform nombreRectTransform = nombreText.GetComponent<RectTransform>();
        RectTransform perfilRectTransform = perfilImage.GetComponent<RectTransform>();
        RectTransform estadoRectTransform = estadoObject.GetComponent<RectTransform>();

        if (nombreText != null)
        {
            if (userType == "usuario")
            {
                nombreText.text = "Usuario";
                perfilImage.sprite = Resources.Load<Sprite>("Recurso-13");

                nombreRectTransform.anchoredPosition = new Vector2(-492, nombreRectTransform.anchoredPosition.y);
                perfilRectTransform.anchoredPosition = new Vector2(-330, perfilRectTransform.anchoredPosition.y);
                estadoRectTransform.anchoredPosition = new Vector2(-370, estadoRectTransform.anchoredPosition.y);
            }
            else
            {
                nombreText.text = "Agente";
                perfilImage.sprite = Resources.Load<Sprite>("Willyrex");
            }
        }

        var titleText = notificationInstance.transform.Find("Bocadillo/Panel_titulo/Titulo")?.GetComponent<TextMeshProUGUI>();
        if (titleText != null)
        {
            titleText.text = title;
            Debug.Log("Título asignado: " + title);
        }

        var descriptionText = notificationInstance.transform.Find("Bocadillo/Contenido/Panel_descrip/Descripción")?.GetComponent<TextMeshProUGUI>();
        if (descriptionText != null)
        {
            descriptionText.text = description;
            Debug.Log("Descripción asignada: " + description);
        }

        var imageComponent = notificationInstance.transform.Find("Bocadillo/Contenido/Hotel_Img")?.GetComponent<Image>();
        if (imageComponent != null)
        {
            Debug.Log("Comenzando la descarga de la imagen");
            StartCoroutine(DownloadAndSetImage(imageUrl, imageComponent));
        }

        // Visibilizar el estado para la última notificación instanciada
        foreach (var instance in notificationInstances)
        {
            var instanceEstadoObject = instance.transform.Find("ID/Estado")?.GetComponent<Image>();
            if (instanceEstadoObject != null)
            {
                var color = instanceEstadoObject.color;
                color.a = 0f; // Opacidad baja por defecto para todas las instancias
                instanceEstadoObject.color = color;
            }
        }

        var lastEstadoObject = notificationInstances[notificationInstances.Count - 1].transform.Find("ID/Estado")?.GetComponent<Image>();
        if (lastEstadoObject != null)
        {
            var color = lastEstadoObject.color;
            color.a = 1.0f; // Opacidad alta para la última instancia
            lastEstadoObject.color = color;
            Debug.Log("Estado visibilizado para la última notificación.");
        }
    }

    // Coroutine para descargar y establecer la imagen de la notificación
    IEnumerator DownloadAndSetImage(string imageUrl, Image imageComponent)
    {
        Debug.Log("Descargando imagen desde URL: " + imageUrl);
        UnityWebRequest imgRequest = UnityWebRequestTexture.GetTexture(imageUrl);
        yield return imgRequest.SendWebRequest();

        if (imgRequest.result == UnityWebRequest.Result.Success)
        {
            Texture2D texture = DownloadHandlerTexture.GetContent(imgRequest);
            imageComponent.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            Debug.Log("Imagen descargada y asignada correctamente.");
        }
        else
        {
            Debug.LogError("Error downloading image: " + imgRequest.error);
        }
    }

    // Método para instanciar el prefab en la ventana
    public void InstanciaBuzonAgente()
    {
        GameObject buzonAgenteInstance = GameObject.Find("UI_FinalGeneral_BuzonAgente(Clone)");
        if (buzonAgenteInstance != null)
        {
            Transform content = buzonAgenteInstance.transform.Find("PanelInteractable/PanelCanvas/Canvas/Interfaz-Anclado/Scroll View/Viewport/Content");
            if (content != null)
            {
                foreach (var notificationInstance in notificationInstances)
                {
                    notificationInstance.transform.SetParent(content, false);
                }
                interfazHistorialInstanciada = true;
            }
            else
            {
                Debug.LogError("No se encontró el objeto Content en la instancia del buzón de agente.");
            }
        }
        else
        {
            Debug.LogError("No se encontró la instancia de UI_FinalGeneral_BuzonAgente.");
        }
    }
}

// Clase que representa los datos del entorno
public class EnvironmentData
{
    public int id;
    public string name;
    public string description;
    public string NotificationType; // Cambiar a string para representar "Agente" o "Usuario"
    public string img2D;
    public string created_at;
    public string updated_at;
    // public bool notificacion_usuario; // Campo preparado para futuras implementaciones
}
