using UnityEngine;
using UnityEngine.Video;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using System.IO;
using System.Linq;
using System;

public class Video360Manager : MonoBehaviour
{
    #region Inspector Fields

    [Header("Video Player 360")]
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

    // Array de Solventia (28 elementos)
    private float[] tiempos = new float[]
    {
        1f,   // piscina
        6.7f, // musculo
        78f,  // calistenia
        116f, // ocr
        140.5f, // padel
        148.5f, // dojo
        168f, // crossfit
        183f, // pilate
        190f, // pole
        195.8f, // cuerpo
        202f, // indoor
        208f, // outdoor
        217f, // spinnig
        231f, // hiitarea
        241f, // personal
        253f, // tierra
        261.3f, // rapida
        268f, // ludoteca
        276.5f, // baloncesto
        284f, // futboll
        291.5f, // Vpis
        304f, // Vsala
        310f, // coworking
        317.5f, // Sparty
        326.5f, // Cafe
        340f, // suplementos
        345f, // fisio
        354.7f, // arte
    };
    private string[] nombresEN = new string[]
    {
        "Semi-Olympic Pool & Hydrothermal Pool",
        "Fitness & Weight Training Room",
        "Calisthenics & Box Area",
        "OCR Circuit",
        "Padel Court",
        "Fight Club (Boxing & Dojo)",
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

    // Array de cedaga (10 elementos)
    private float[] tiemposcedaga = new float[]
    {
        0.8f,     // Piscina
        20f,      // Aquagym
        33f,      // Musculo
        70.5f,    // Boxeo
        84.3f,    // Pilate
        100.3f,   // Indoor
        114f,     // Solarium
        133f,    // Ludoteca
        156f,     // Abonados
        172f,    // Familia        
    };
    private string[] nombresENcedaga = new string[]
    {
        "Semi-Olympic Pool",
        "Instructional & Aquagym Pool",
        "Fitness & Weight Training Room",
        "Fight Club (Boxing)",
        "Pilates Reformer Studio",
        "Group Training Room (Indoor Activities)",
        "Spinning Studio",
        "Outdoor Solarium",
        "Kids’ Playroom",
        "Member Changing Rooms",
        "Family Changing Rooms",
    };
    private string[] nombresEScedaga = new string[]
    {
        "Piscina Semiolímpica",
        "Piscina de enseñanza y Aquagym",
        "Sala Fitness/Musculación",
        "Fight Club (Boxing)",
        "Sala Pilates Máquina",
        "Sala Actividades Dirigidas Indoor",
        "Sala Spinning",
        "Ludoteca",
        "Solarium",
        "Vestuarios Abonados",
        "Vestuarios de Familias",
    };

    private Coroutine cambioTextoCoroutine;

    #endregion

    #region Private Fields

    private int currentVideoIndex = 0;
    private List<string> videoPaths = new List<string>();

    // CAMPOS PARA MÉTRICAS
    private MetricsManager metricMgr;
    private float sessionStartTime;
    private float environmentStartTime;
    private bool environmentActive = false;

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

        // INTEGRACIÓN DE MÉTRICAS: Obtener MetricsManager y registrar SessionStarted
        metricMgr = FindObjectOfType<MetricsManager>();
        sessionStartTime = Time.time;
        if (metricMgr != null)
        {
            metricMgr.LogUsageEvent("SessionStarted", "Video360Manager");
        }

        //PrepareCurrentVideo();
    }

    // Se registra SessionEnded al cerrar la aplicación
    private void OnApplicationQuit()
    {
        if (metricMgr != null)
        {
            float sessionDuration = Time.time - sessionStartTime;
            metricMgr.LogUsageEvent("SessionEnded", "Video360Manager", sessionDuration);
        }
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

        // MÉTRICA: Entorno Cargado
        if (metricMgr != null)
        {
            string videoName = Path.GetFileName(vp.url);
            environmentStartTime = Time.time;
            environmentActive = true;
            metricMgr.LogUsageEvent("EntornoCargado", videoName);
        }
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
        // MÉTRICA: UIElementClicked para cambiar de video por índice
        if (metricMgr != null)
        {
            metricMgr.LogUsageEvent("UIElementClicked", "PlayVideoByIndex");
        }

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
        // MÉTRICA: UIElementClicked para reanudar video
        if (metricMgr != null)
        {
            metricMgr.LogUsageEvent("UIElementClicked", "ResumeVideo");
        }

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
        // Si se está reproduciendo un entorno, registrar la salida y los eventos de completado/incompleto.
        if (metricMgr != null && environmentActive)
        {
            float duration = Time.time - environmentStartTime;
            string videoName = Path.GetFileName(videoPlayer.url);
            // MÉTRICA: Entorno Descargado/Saliente con la duración transcurrida.
            metricMgr.LogUsageEvent("EntornoDescargado", videoName, duration);

            // Se define una tolerancia para considerar el video finalizado.
            float tolerance = 1.0f;
            if (videoPlayer.length > 0 && (videoPlayer.length - videoPlayer.time <= tolerance))
            {
                metricMgr.LogUsageEvent("EnviromentCompleted", videoName, duration);
            }
            else
            {
                float progress = videoPlayer.length > 0 ? (float)(videoPlayer.time / videoPlayer.length * 100.0) : 0f;
                metricMgr.LogUsageEvent("EnviromentIncompleted", videoName, progress);
            }
            environmentActive = false;
        }

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

    public void VideoSkipTime(float time)
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

    public void ToggleLanguage()
    {
        // MÉTRICA: UIElementClicked para cambiar idioma
        if (metricMgr != null)
        {
            metricMgr.LogUsageEvent("UIElementClicked", "ToggleLanguage");
        }
        isSpanish = !isSpanish;
    }

    public void SetLanguage(bool value)
    {
        isSpanish = value;
    }

    public void SetVideoOption(bool option)
    {
        // MÉTRICA: UIElementClicked para cambiar opción de video
        if (metricMgr != null)
        {
            metricMgr.LogUsageEvent("UIElementClicked", "SetVideoOption");
        }
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

    #region Métodos Subtitulos

    public void MostrarTextoEnTMP(string texto)
    {
        if (textMeshPro == null)
            return;

        textMeshPro.text = texto;
        if (!textMeshPro.gameObject.activeSelf)
            textMeshPro.gameObject.SetActive(true);
    }

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
        int i = 0;
        // Mientras queden eventos de cambio
        while (i < tiempos.Length)
        {
            // Espera hasta que el tiempo actual del video sea mayor o igual al tiempo objetivo
            while (videoPlayer.time < tiempos[i])
            {
                yield return null; // Espera al siguiente frame
            }

            // Una vez alcanzado el tiempo, cambia el texto
            MostrarTextoEnTMP(nombres[i]);
            // MÉTRICA: Se registra el objetivo completado (zona visitada)
            if (metricMgr != null)
            {
                metricMgr.LogUsageEvent("ObjectiveCompleted", nombres[i]);
            }
            i++; // Avanza al siguiente evento
        }
    }

    public void IniciarCambioTextoCedaga()
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
        string[] nombresSeleccionados = isSpanish ? nombresEScedaga : nombresENcedaga;

        // Iniciamos la coroutine que se encargará de actualizar el texto
        cambioTextoCoroutine = StartCoroutine(CambiarTextoCoroutineCedaga(tiemposcedaga, nombresSeleccionados));
    }

    private IEnumerator CambiarTextoCoroutineCedaga(float[] tiempos, string[] nombres)
    {
        int i = 0;
        // Mientras queden eventos de cambio
        while (i < tiempos.Length)
        {
            // Espera hasta que el tiempo actual del video sea mayor o igual al tiempo objetivo
            while (videoPlayer.time < tiempos[i])
            {
                yield return null; // Espera al siguiente frame
            }

            // Una vez alcanzado el tiempo, cambia el texto
            MostrarTextoEnTMP(nombres[i]);
            // MÉTRICA: Se registra el objetivo completado (zona visitada)
            if (metricMgr != null)
            {
                metricMgr.LogUsageEvent("ObjectiveCompleted", nombres[i]);
            }
            i++; // Avanza al siguiente evento
        }
    }
}
#endregion