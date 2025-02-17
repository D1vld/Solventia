using UnityEngine;
using UnityEngine.Video;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using System.IO;
using System.Linq;

public class Video360Manager : MonoBehaviour
{
    #region Inspector Fields

    [Header("Video Player 360")]
    [SerializeField] private VideoPlayer videoPlayer;

    [Header("Carpeta local de los videos 360")]
    [Tooltip("Ruta de la carpeta local donde se encuentran los videos 360. Ejemplo en Android: /storage/emulated/0/Movies/360Videos")]
    [SerializeField] private string folderPath = "/storage/emulated/0/Movies/360Videos";    // storage/emulated/0/Movies/360Videos Gafas 
                                                                                            // C:\Users\crist\OneDrive\Escritorio\pruena\video Local

    [Header("GameObjects generales")]
    [SerializeField] private GameObject gameObjectToActivate;
    [SerializeField] private GameObject gameObjectToDesactivate;

    [Header("Paneles Interactables (Duplicados)")]
    [SerializeField] private GameObject panelInteractableES; // Panel en Español
    [SerializeField] private GameObject panelInteractableEN; // Panel en Inglés

    [Header("Menús Bajo (Duplicados)")]
    [SerializeField] private GameObject menuBajoES; // Menú en Español
    [SerializeField] private GameObject menuBajoEN; // Menú en Inglés

    [Header("Material del Skybox (Video 360)")]
    [SerializeField] private Material materialSkyboxVideo;

    [Header("Text Mesh Pro")]
    [SerializeField] private TextMeshProUGUI textMeshPro;

    [Header("Idioma")]
    [Tooltip("true = Español, false = Inglés")]
    [SerializeField] private bool isSpanish = true;

    #endregion

    #region Private Fields

    // Índice del video actual.
    private int currentVideoIndex = 0;

    // Lista de rutas para los videos de la carpeta.
    private List<string> videoPaths = new List<string>();

    #endregion

    #region Unity Lifecycle

    private void Start()
    {
        // Si no se asignó el VideoPlayer en el inspector, se busca en el GameObject.
        if (videoPlayer == null)
            videoPlayer = GetComponent<VideoPlayer>();

        // Configuramos el tint inicial del Skybox.
        if (materialSkyboxVideo != null)
            SetSkyboxTintColor("#808080");

        // Cargamos y reproducimos el primer video de la carpeta.
        LoadAndSortVideos();
        if (videoPaths.Count > 0)
        {
            currentVideoIndex = 0;
            PlayCurrentVideo();
        }
        else
        {
            Debug.LogWarning("No se encontraron archivos .mp4 en la carpeta especificada.");
        }
    }

    #endregion

    #region Gestión de Videos (Modo Carpeta)

    /// <summary>
    /// Carga y ordena los videos encontrados en la carpeta especificada.
    /// </summary>
    private void LoadAndSortVideos()
    {
        if (!Directory.Exists(folderPath))
        {
            Debug.LogWarning("La carpeta no existe: " + folderPath);
            return;
        }

        string[] files = Directory.GetFiles(folderPath, "*.mp4");

        if (files.Length == 0)
        {
            Debug.LogError("No se encontraron archivos .mp4 en la carpeta: " + folderPath);
            return;
        }

        videoPaths = files
            .OrderBy(path => ExtractLeadingNumber(Path.GetFileNameWithoutExtension(path)))
            .ToList();

        Debug.Log("Archivos encontrados: " + files.Length);
    }

    /// <summary>
    /// Extrae el número inicial del nombre del archivo para ordenar.
    /// Ejemplo: "1_VideoEntrada" devolverá 1.
    /// </summary>
    /// <param name="fileName">Nombre del archivo sin extensión.</param>
    /// <returns>Número extraído o int.MaxValue si no se encuentra.</returns>
    private int ExtractLeadingNumber(string fileName)
    {
        var parts = fileName.Split('_');
        if (parts.Length > 0 && int.TryParse(parts[0], out int number))
            return number;
        return int.MaxValue;
    }

    /// <summary>
    /// Reproduce el video actual en modo carpeta.
    /// </summary>
    private void PlayCurrentVideo()
    {
        if (videoPaths.Count == 0 || videoPlayer == null)
            return;

        string videoUrl = videoPaths[currentVideoIndex];
        if (string.IsNullOrEmpty(videoUrl))
        {
            Debug.LogError("El path del video es NULL o vacío.");
            return;
        }

        // Evitamos múltiples suscripciones eliminando primero el posible handler anterior.
        videoPlayer.loopPointReached -= OnFolderVideoEnd;
        videoPlayer.loopPointReached += OnFolderVideoEnd;

        videoPlayer.url = videoUrl;
        videoPlayer.Play();

        Debug.Log("Reproduciendo video: " + Path.GetFileName(videoUrl));
    }

    /// <summary>
    /// Evento que se dispara cuando finaliza la reproducción de un video en modo carpeta.
    /// </summary>
    /// <param name="vp">Referencia al VideoPlayer.</param>
    private void OnFolderVideoEnd(VideoPlayer vp)
    {
        NextVideo();
    }

    #endregion

    #region Métodos de Reproducción

    /// <summary>
    /// Reproduce el siguiente video.
    /// </summary>
    private void NextVideo()
    {
        currentVideoIndex++;
        if (currentVideoIndex >= videoPaths.Count)
            currentVideoIndex = 0;
        PlayCurrentVideo();
    }

    /// <summary>
    /// Reproduce el video según el índice especificado.
    /// </summary>
    /// <param name="index">Índice del video a reproducir.</param>
    public void PlayVideoByIndex(int index)
    {
        if (index < 0 || index >= videoPaths.Count)
        {
            Debug.LogWarning("Índice fuera de rango para videos en carpeta.");
            return;
        }

        currentVideoIndex = index;
        PlayCurrentVideo();
    }

    #endregion

    #region Métodos de Control y UI

    /// <summary>
    /// Reanuda la reproducción del video y restablece el tint a gris (#808080).
    /// También muestra el menú correspondiente al idioma seleccionado.
    /// </summary>
    public void ResumeVideo()
    {
        if (videoPlayer != null)
            videoPlayer.Play();

        if (panelInteractableES != null) panelInteractableES.SetActive(false);
        if (panelInteractableEN != null) panelInteractableEN.SetActive(false);

        if (isSpanish)
        {
            if (menuBajoES != null) menuBajoES.SetActive(true);
            if (menuBajoEN != null) menuBajoEN.SetActive(false);
        }
        else
        {
            if (menuBajoES != null) menuBajoES.SetActive(false);
            if (menuBajoEN != null) menuBajoEN.SetActive(true);
        }

        SetSkyboxTintColor("#808080");
    }

    /// <summary>
    /// Detiene la reproducción del video, pone la vista en negro y muestra el panel interactable según el idioma.
    /// </summary>
    public void StopVideo()
    {
        if (videoPlayer != null)
            videoPlayer.Pause();

        if (textMeshPro != null)
            textMeshPro.gameObject.SetActive(false);

        if (menuBajoES != null) menuBajoES.SetActive(false);
        if (menuBajoEN != null) menuBajoEN.SetActive(false);

        if (isSpanish)
        {
            if (panelInteractableES != null) panelInteractableES.SetActive(true);
            if (panelInteractableEN != null) panelInteractableEN.SetActive(false);
        }
        else
        {
            if (panelInteractableES != null) panelInteractableES.SetActive(false);
            if (panelInteractableEN != null) panelInteractableEN.SetActive(true);
        }

        SetSkyboxTintColor("#000000");
    }

    /// <summary>
    /// Cambia el tint color del material del Skybox a partir de un valor Hex (#RRGGBB).
    /// Asegúrate de usar el nombre de propiedad correcto según tu Shader (por ejemplo, "_Tint").
    /// </summary>
    /// <param name="hexColor">Color en formato hexadecimal.</param>
    private void SetSkyboxTintColor(string hexColor)
    {
        if (materialSkyboxVideo == null)
            return;

        if (ColorUtility.TryParseHtmlString(hexColor, out Color color))
            materialSkyboxVideo.SetColor("_Tint", color);
        else
            Debug.LogWarning("No se ha podido parsear el color con el código: " + hexColor);
    }

    /// <summary>
    /// Salta a un tiempo específico del video.
    /// </summary>
    /// <param name="time">Tiempo en segundos.</param>
    public void VideoSkipTime(int time)
    {
        if (videoPlayer != null)
            videoPlayer.time = time;
    }

    /// <summary>
    /// Detiene el video después de una cantidad de segundos especificada.
    /// </summary>
    /// <param name="seconds">Segundos a esperar antes de detener el video.</param>
    public void StopVideoAfterSeconds(float seconds)
    {
        StartCoroutine(StopAfterSecondsCoroutine(seconds));
    }

    private IEnumerator StopAfterSecondsCoroutine(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        StopVideo();
        Debug.Log("Se ha detenido el video tras " + seconds + " segundos, mostrando fondo negro.");
    }

    /// <summary>
    /// Muestra en pantalla el texto que se pase por parámetro en el TextMeshPro.
    /// </summary>
    /// <param name="texto">Texto a mostrar.</param>
    public void MostrarTextoEnTMP(string texto)
    {
        if (textMeshPro == null)
        {
            Debug.LogWarning("TextMeshPro no está asignado en el inspector.");
            return;
        }

        textMeshPro.text = texto;
        if (!textMeshPro.gameObject.activeSelf)
            textMeshPro.gameObject.SetActive(true);
    }

    /// <summary>
    /// Alterna el idioma entre Español e Inglés.
    /// </summary>
    public void ToggleLanguage()
    {
        isSpanish = !isSpanish;
        Debug.Log("Idioma cambiado. Ahora isSpanish = " + isSpanish);
    }

    /// <summary>
    /// Establece directamente el idioma.
    /// </summary>
    /// <param name="value">true para Español, false para Inglés.</param>
    public void SetLanguage(bool value)
    {
        isSpanish = value;
        Debug.Log("Idioma establecido directamente a isSpanish = " + value);
    }

    #endregion
}
