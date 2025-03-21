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
    [SerializeField] private string folderPath = "/storage/emulated/0/Movies/360Videos";

    [Header("Opciones de Video")]
    [SerializeField] private bool playFirstVideo = true;

    [Header("GameObjects a Activar/Desactivar por Idioma")]
    [SerializeField] private GameObject gameObjectToActivateES;
    [SerializeField] private GameObject gameObjectToActivateEN;
    [SerializeField] private GameObject gameObjectToDesactivateES;
    [SerializeField] private GameObject gameObjectToDesactivateEN;


    [Header("Paneles Interactables (Duplicados)")]
    [SerializeField] private GameObject panelInteractableES;
    [SerializeField] private GameObject panelInteractableEN;

    [Header("Menús Bajo (Duplicados)")]
    [SerializeField] private GameObject menuBajoES;
    [SerializeField] private GameObject menuBajoEN;

    [Header("Material del Skybox (Video 360)")]
    [SerializeField] private Material materialSkyboxVideo;

    [Header("Text Mesh Pro")]
    [SerializeField] private TextMeshProUGUI textMeshPro;

    [Header("Idioma")]
    [SerializeField] private bool isSpanish = true;

    // Array de tiempos (en segundos) para cada estancia (28 elementos)
    private float[] tiempos = new float[]
    {
        0.06f, 1.18f, 1.55f, 2.20f, 2.27f, 2.48f, 3.03f, 3.10f, 3.15f, 3.21f,
        3.27f, 3.37f, 3.51f, 4.00f, 4.13f, 4.20f, 4.27f, 4.36f, 4.43f, 4.50f,
        5.04f, 5.10f, 5.17f, 5.26f, 5.40f, 5.44f, 5.53f, 6.20f
    };

    // Array de nombres en ingles (28 elementos)
    private string[] nombresEN = new string[]
    {
        "Semi-Olympic Pool & Hydrothermal Pool",
        "Fitness & Weight Training Room",
        "Calisthenics & Box Area",
        "OCR Circuit",
        "Padel Court",
        "Fight Club",
        "CROSSFIT / HYROX",
        "Pilates Equipment Studio",
        "Pole Dance Studio",
        "Mind-Body Activities Studio",
        "Indoor Group Exercise Studio",
        "Outdoor Group Exercise Area",
        "Spinning & Elliptical Fit Studio",
        "Hittbox Studio",
        "Personal Training Studio",
        "Tennis: Clay Court",
        "Tennis:Fast Court",
        "Kids’ Playroom",
        "3x3 Basketball Court",
        "Football Pitch",
        "Pool Changing Rooms",
        "Studios’ Changing Rooms",
        "Coworking Area",
        "“Sparty” Birthday Party Zone",
        "Cafeteria",
        "Nutrition Shop",
        "Physiotherapy Clinic",
        "Artworks by Daniel Rodríguez Báez"
    };

    // Array de nombres en español (28 elementos)
    private string[] nombresES = new string[]
    {
        "Piscina Semiolímpica y Piscina Hidrotermal",
        "Sala Fitness/Musculación",
        "Calistenia y Box Area",
        "Circuito OCR",
        "Pista de Pádel",
        "Fight Club (Boxing y Dojo)",
        "CrossFit / Hyrox",
        "Sala Pilates Máquina",
        "Sala Pole Dance",
        "Sala Actividades Cuerpo/Mente",
        "Sala Actividades Dirigidas Indoor",
        "Sala Actividades Dirigidas Outdoor",
        "Sala Spinning y Elíptica Fit",
        "Sala HITTBOX",
        "Sala Entrenamiento Personal",
        "Tenis: Tierra Batida",
        "Tenis: Pista Rápida",
        "Ludoteca",
        "Basket 3x3",
        "Campo de Fútbol",
        "Vestuarios de Piscina",
        "Vestuarios de Salas",
        "Zona Coworking",
        "Zona Cumpleaños Sparty",
        "Cafetería",
        "Tienda Nutrición",
        "Clínica Fisioterapia",
        "Obras de Arte (Artista Daniel Rodríguez Báez)"
    };

    private Coroutine cambioTextoCoroutine;

    #endregion

    #region Private Fields

    private int currentVideoIndex = 0;
    private List<string> videoPaths = new List<string>();

    #endregion

    #region Unity Lifecycle

    private void Start()
    {
        if (videoPlayer == null)
            videoPlayer = GetComponent<VideoPlayer>();

        videoPlayer.playOnAwake = false;

        if (materialSkyboxVideo != null)
            SetSkyboxTintColor("#808080");

        LoadAndSortVideos();

        if (videoPaths.Count >= 2)
            currentVideoIndex = playFirstVideo ? 0 : 1;
        else if (videoPaths.Count == 1)
            currentVideoIndex = 0;

        //PrepareCurrentVideo();
    }

    #endregion

    #region Gestión de Videos (Modo Carpeta)

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

    private int ExtractLeadingNumber(string fileName)
    {
        var parts = fileName.Split('_');
        if (parts.Length > 0 && int.TryParse(parts[0], out int number))
            return number;
        return int.MaxValue;
    }

    private void PrepareCurrentVideo()
    {
        if (videoPaths.Count == 0 || videoPlayer == null)
            return;

        string videoUrl = videoPaths[currentVideoIndex];

        if (string.IsNullOrEmpty(videoUrl))
        {
            Debug.LogError("El path del video es NULL o vacío.");
            return;
        }

        videoPlayer.prepareCompleted -= OnVideoPrepared;
        videoPlayer.prepareCompleted += OnVideoPrepared;

        videoPlayer.loopPointReached -= OnFolderVideoEnd;
        videoPlayer.loopPointReached += OnFolderVideoEnd;

        videoPlayer.url = videoUrl;
        videoPlayer.Prepare();

        Debug.Log("Preparando video: " + Path.GetFileName(videoUrl));

        StartCoroutine(CheckAndRetryVideoStartCoroutine());
    }

    private void OnVideoPrepared(VideoPlayer vp)
    {
        vp.Play();
        Debug.Log("Video preparado y reproduciendo: " + Path.GetFileName(vp.url));
    }

private void OnFolderVideoEnd(VideoPlayer vp)
{
    StopVideo();

    if (!playFirstVideo)
    {
        if (isSpanish)
        {
            if (gameObjectToActivateES != null) gameObjectToActivateES.SetActive(true);
            if (gameObjectToDesactivateES != null) gameObjectToDesactivateES.SetActive(false);
        }
        else
        {
            if (gameObjectToActivateEN != null) gameObjectToActivateEN.SetActive(true);
            if (gameObjectToDesactivateEN != null) gameObjectToDesactivateEN.SetActive(false);
        }
    }
}



    private IEnumerator CheckAndRetryVideoStartCoroutine()
    {
        yield return new WaitForSeconds(2f);

        if (!videoPlayer.isPlaying)
        {
            Debug.LogWarning("El video no comenzó tras 2 segundos. Reintentando...");
            PrepareCurrentVideo();
        }
    }

    #endregion

    #region Métodos de Reproducción

    private void NextVideo()
    {
        currentVideoIndex = (currentVideoIndex + 1) % videoPaths.Count;
        PrepareCurrentVideo();
    }

    public void PlayVideoByIndex(int index)
    {
        if (index < 0 || index >= videoPaths.Count)
        {
            Debug.LogWarning("Índice fuera de rango.");
            return;
        }

        currentVideoIndex = index;
        PrepareCurrentVideo();
    }

    #endregion

    #region Métodos de Control y UI

    public void ResumeVideo()
    {
        if (videoPlayer != null && videoPlayer.isPrepared)
            videoPlayer.Play();

        RevertCameraToSkybox();

        if (panelInteractableES != null) panelInteractableES.SetActive(false);
        if (panelInteractableEN != null) panelInteractableEN.SetActive(false);

        // Si playFirstVideo es falso, se desactivan ambos menús bajos
        if (!playFirstVideo)
        {
            if (menuBajoES != null) menuBajoES.SetActive(false);
            if (menuBajoEN != null) menuBajoEN.SetActive(false);
        }
        else // si playFirstVideo es true, activa según idioma
        {
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
        }

        SetSkyboxTintColor("#808080");
    }


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

        SetCameraToSolidColor("#000000");        
    }

    public void VideoSkipTime(int time)
    {
        if (videoPlayer != null)
            videoPlayer.time = time;
    }

    public void StopVideoAfterSeconds(float seconds)
    {
        StartCoroutine(StopAfterSecondsCoroutine(seconds));
    }

    private IEnumerator StopAfterSecondsCoroutine(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        StopVideo();
        Debug.Log("Video detenido tras " + seconds + " segundos.");
    }

    public void MostrarTextoEnTMP(string texto)
    {
        if (textMeshPro == null)
            return;

        textMeshPro.text = texto;
        if (!textMeshPro.gameObject.activeSelf)
            textMeshPro.gameObject.SetActive(true);
    }


    public void ToggleLanguage()
    {
        isSpanish = !isSpanish;
    }

    public void SetLanguage(bool value)
    {
        isSpanish = value;
    }

    public void SetVideoOption(bool option)
    {
        playFirstVideo = option;
        currentVideoIndex = playFirstVideo ? 0 : 1;
        PrepareCurrentVideo();
    }

    #endregion

    #region Métodos Skybox y Cámara

    private void SetSkyboxTintColor(string hexColor)
    {
        if (materialSkyboxVideo == null) return;
        if (ColorUtility.TryParseHtmlString(hexColor, out Color color))
            materialSkyboxVideo.SetColor("_Tint", color);
    }

    public void SetCameraToSolidColor(string hexColor)
    {
        Camera mainCamera = Camera.main;
        if (mainCamera == null) return;

        mainCamera.clearFlags = CameraClearFlags.SolidColor;
        if (ColorUtility.TryParseHtmlString(hexColor, out Color color))
        {
            color.a = 0f;
            mainCamera.backgroundColor = color;
        }
    }

    public void RevertCameraToSkybox()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
            mainCamera.clearFlags = CameraClearFlags.Skybox;
    }

    #endregion


    /////// IGG
    ///
    public void IniciarCambioTexto()
    {
        // Si ya hay una coroutine en ejecución, se detiene primero
        if (cambioTextoCoroutine != null)
        {
            StopCoroutine(cambioTextoCoroutine);
            cambioTextoCoroutine = null;
        }

        // Reiniciamos el texto
        textMeshPro.text = "";
        textMeshPro.gameObject.SetActive(true);

        // Seleccionamos el array de nombres según el idioma
        string[] nombresSeleccionados = isSpanish ? nombresES : nombresEN;

        // Iniciamos la coroutine que se encargará de actualizar el texto
        cambioTextoCoroutine = StartCoroutine(CambiarTextoCoroutine(tiempos, nombresSeleccionados));
    }

    private IEnumerator CambiarTextoCoroutine(float[] tiempos, string[] nombres)
    {
        float tiempoAnterior = 0f;

        for (int i = 0; i < tiempos.Length; i++)
        {
            // Calculamos el tiempo a esperar respecto al cambio anterior.
            float esperar = tiempos[i] - tiempoAnterior;
            if (esperar > 0)
                yield return new WaitForSeconds(esperar);

            // Actualizamos el texto utilizando el método existente.
            MostrarTextoEnTMP(nombres[i]);

            // Actualizamos el tiempoAnterior para el siguiente ciclo.
            tiempoAnterior = tiempos[i];
        }
    }
}
