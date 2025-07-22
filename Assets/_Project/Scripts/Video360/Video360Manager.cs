using UnityEngine;
using UnityEngine.Video;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using System.IO;
using System.Linq;
using System;
using UnityEngine.SceneManagement;

public enum Language
{
    Spanish = 0,
    English = 1,
    French  = 2,
    German  = 3
}

public class Video360Manager : MonoBehaviour, IVideoPlaybackService
{
    #region Inspector Fields 

    [Header("Video Player 360")]
    [SerializeField] public VideoPlayer videoPlayer;

    [Header("Carpeta local de los vídeos 360")]
    [SerializeField] private string folderPath = "/storage/emulated/0/Movies/360Videos";

    [Header("Opciones de vídeo")]
    [SerializeField] private bool playFirstVideo = true;

    /*========= UI por idioma =========*/
    [Header("GameObjects a Activar/Desactivar por Idioma")]
    [SerializeField] private GameObject gameObjectToActivateES;
    [SerializeField] private GameObject gameObjectToActivateEN;
    [SerializeField] private GameObject gameObjectToActivateFR;
    [SerializeField] private GameObject gameObjectToActivateDE;

    [SerializeField] private GameObject gameObjectToDesactivateES;
    [SerializeField] private GameObject gameObjectToDesactivateEN;
    [SerializeField] private GameObject gameObjectToDesactivateFR;
    [SerializeField] private GameObject gameObjectToDesactivateDE;

    [Header("Paneles Interactables (Duplicados)")]
    [SerializeField] private GameObject panelInteractableES;
    [SerializeField] private GameObject panelInteractableEN;
    [SerializeField] private GameObject panelInteractableFR;
    [SerializeField] private GameObject panelInteractableDE;

    [Header("Menús Bajo (Duplicados)")]
    [SerializeField] private GameObject menuBajoES;
    [SerializeField] private GameObject menuBajoEN;
    [SerializeField] private GameObject menuBajoFR;
    [SerializeField] private GameObject menuBajoDE;

    [Header("Material Skybox (Vídeo 360)")]
    [SerializeField] private Material materialSkyboxVideo;

    [Header("Text Mesh Pro")]
    [SerializeField] private TextMeshProUGUI textMeshPro;

    [Header("Idioma inicial")]
    [SerializeField] private Language currentLanguage = Language.Spanish;

    #endregion

    #region Datos de tiempos y nombres 

    /*――― Solventia ―――*/
    private readonly float[] tiempos = {
        1f, 6.7f, 78f, 116f, 140.5f, 148.5f, 168f, 183f, 190f, 195.8f,
        202f, 208f, 217f, 231f, 241f, 253f, 261.3f, 268f, 276.5f, 284f,
        291.5f, 304f, 310f, 317.5f, 326.5f, 340f, 345f, 354.7f
    };

    private readonly string[] nombresES = {
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
        "Basket 3x3",
        "Campo de Fútbol",
        "Vestuarios de Piscina",
        "Vestuarios de Salas",
        "Zona Coworking",
        "Zona Cumpleaños Sparty",
        "Cafetería",
        "Tienda Nutrición",
        "Clínica Fisioterapia",
        "Obras de Arte (Artista Daniel Rodríguez Báez)"
    };

    private readonly string[] nombresEN = {
        "Semi‑Olympic Pool & Hydrothermal Pool",
        "Fitness & Weight Training Room",
        "Calisthenics & Box Area",
        "OCR Circuit",
        "Padel Court",
        "Fight Club (Boxing & Dojo)",
        "CROSSFIT / HYROX",
        "Pilates Equipment Studio",
        "Pole Dance Studio",
        "Mind‑Body Activities Studio",
        "Indoor Group Exercise Studio",
        "Outdoor Group Exercise Area",
        "Spinning & Elliptical Fit Studio",
        "HIITBOX Studio",
        "Personal Training Studio",
        "Tennis: Clay Court",
        "Tennis: Fast Court",
        "Kids’ Playroom",
        "3 × 3 Basketball Court",
        "Football Pitch",
        "Pool Changing Rooms",
        "Studios’ Changing Rooms",
        "Coworking Area",
        "“Sparty” Birthday Party Zone",
        "Cafeteria",
        "Nutrition Shop",
        "Physiotherapy Clinic",
        "Artworks by Daniel Rodríguez Báez"
    };

    private readonly string[] nombresFR = {
        "Piscine semi‑olympique et piscine d’hydrothérapie",
        "Salle de fitness & musculation",
        "Zone Calisthenics & Box",
        "Parcours OCR",
        "Terrain de padel",
        "Fight Club (Boxe & Dojo)",
        "CROSSFIT / HYROX",
        "Studio Pilates Reformer",
        "Studio Pole Dance",
        "Studio Activités Corps‑Esprit",
        "Studio d’exercices collectifs Indoor",
        "Espace d’activités de groupe Outdoor",
        "Studio Spinning & Elliptical Fit",
        "Studio HIITBOX",
        "Studio d’entraînement personnel",
        "Tennis : Court en terre battue",
        "Tennis : Court rapide",
        "Salle de jeux pour enfants",
        "Terrain de basket 3×3",
        "Terrain de football",
        "Vestiaires piscine",
        "Vestiaires des studios",
        "Espace Coworking",
        "Zone anniversaires « Sparty »",
        "Cafétéria",
        "Boutique de nutrition",
        "Clinique de physiothérapie",
        "Œuvres de Daniel Rodríguez Báez"
    };

    private readonly string[] nombresDE = {
        "Halbolympisches Becken & hydrothermales Becken",
        "Fitness‑ & Krafttrainingsraum",
        "Calisthenics‑ & Box‑Bereich",
        "OCR‑Parcours",
        "Padelplatz",
        "Fight Club (Boxen & Dojo)",
        "CROSSFIT / HYROX",
        "Pilates‑Gerätestudio",
        "Pole‑Dance‑Studio",
        "Mind‑Body‑Aktivitäten‑Studio",
        "Indoor‑Gruppenübungsraum",
        "Outdoor‑Gruppenbereich",
        "Spinning‑ & Elliptical‑Fit‑Studio",
        "HIITBOX‑Studio",
        "Personal‑Training‑Studio",
        "Tennis: Sandplatz",
        "Tennis: Hartplatz",
        "Kinderspielzimmer",
        "3×3‑Basketballplatz",
        "Fußballfeld",
        "Schwimmbad‑Umkleiden",
        "Studio‑Umkleiden",
        "Coworking‑Bereich",
        "Geburtstagszone „Sparty“",
        "Cafeteria",
        "Nahrungsergänzungsshop",
        "Physiotherapie‑Klinik",
        "Kunstwerke von Daniel Rodríguez Báez"
    };

    /*――― Cedaga ―――*/
    private readonly float[] tiemposCedaga = {
        0.8f, 18, 38, 75, 96, 119, 140, 156, 181, 202, 230
    };

    private readonly string[] nombresESCedaga = {
        "Piscina Semiolímpica",
        "Piscina de Enseñanza y Aquagym",
        "Sala Fitness/Musculación",
        "Zona Boxing",
        "Sala Pilates Máquina",
        "Sala Actividades Dirigidas Indoor",
        "Sala Spinning",
        "Solarium Exterior",
        "Ludoteca",
        "Vestuarios Abonados",
        "Vestuarios Familias"
    };

    private readonly string[] nombresENCedaga = {
        "Semi‑Olympic Pool",
        "Instructional & Aquagym Pool",
        "Fitness & Weight Training Room",
        "Boxing Zone",
        "Pilates Reformer Studio",
        "Group Training Room (Indoor Activities)",
        "Spinning Studio",
        "Outdoor Solarium",
        "Kids’ Playroom",
        "Member Changing Rooms",
        "Family Changing Rooms"
    };

    private readonly string[] nombresFRCedaga = {
        "Piscine semi‑olympique",
        "Bassin d’instruction & Aquagym",
        "Salle de fitness & musculation",
        "Zone de boxe",
        "Studio Pilates Reformer",
        "Salle d’entraînement de groupe (intérieur)",
        "Studio Spinning",
        "Solarium extérieur",
        "Salle de jeux pour enfants",
        "Vestiaires membres",
        "Vestiaires familles"
    };

    private readonly string[] nombresDECedaga = {
        "Halbolympisches Becken",
        "Lehr‑ & Aquagym‑Becken",
        "Fitness‑ & Krafttrainingsraum",
        "Boxbereich",
        "Pilates‑Reformer‑Studio",
        "Gruppentrainingsraum (Indoor)",
        "Spinning‑Studio",
        "Außen‑Solarium",
        "Kinderspielzimmer",
        "Umkleiden Mitglieder",
        "Umkleiden Familien"
    };

    #endregion

    #region Métricas y reproducción 

    private Coroutine cambioTextoCoroutine;
    private int currentVideoIndex = 0;
    private readonly List<string> videoPaths = new();

    [SerializeField] private MetricsManager metricMgr;
    [SerializeField] private SyncManager   syncManager;
    private float sessionStartTime;
    private bool  environmentActive = false;

    #endregion

    #region Unity Lifecycle 

    void Awake()
    {
        if (videoPlayer == null)
            videoPlayer = GetComponent<VideoPlayer>();

        videoPlayer.playOnAwake = false;
    }

    void Start()
    {
        if (materialSkyboxVideo != null)
            SetSkyboxTintColor("#808080");

        LoadAndSortVideos();

        currentVideoIndex = videoPaths.Count switch
        {
            >= 2 => playFirstVideo ? 0 : 1,
            1    => 0,
            _    => currentVideoIndex
        };

        metricMgr      = FindObjectOfType<MetricsManager>();
        sessionStartTime = Time.time;
        if (metricMgr != null)
            metricMgr.LogUsageEvent("SessionStart", "Start", "0");

        syncManager = FindObjectOfType<SyncManager>();
        syncManager.StartSync();
        //PrepareCurrentVideo();  // ← se llama cuando corresponda
    }

    private void OnApplicationQuit()
    {
        RecordQuitEvent();
    }

    private void RecordQuitEvent()
    {
        if (metricMgr == null) return;

        string vidQuitName = GetCurrentVideoName();
        string timeVideo   = GetCurrentPlaybackTime().ToString();
        metricMgr.LogUsageEvent("VideoEnd", "Quit_" + vidQuitName, timeVideo);
        metricMgr.LogUsageEvent("SessionEnd", "Quit", "0");
    }

    #endregion

    #region Gestión de vídeos (carpeta) 

    private void LoadAndSortVideos()
    {
        if (!Directory.Exists(folderPath))
        {
            Debug.LogWarning($"La carpeta no existe: {folderPath}");
            return;
        }

        string[] files = Directory.GetFiles(folderPath, "*.mp4");
        if (files.Length == 0)
        {
            Debug.LogError($"No se encontraron .mp4 en la carpeta: {folderPath}");
            return;
        }

        videoPaths.AddRange(
            files.OrderBy(p => ExtractLeadingNumber(Path.GetFileNameWithoutExtension(p)))
        );
        Debug.Log($"Archivos encontrados: {files.Length}");
    }

    private static int ExtractLeadingNumber(string fileName)
    {
        var parts = fileName.Split('_');
        return parts.Length > 0 && int.TryParse(parts[0], out int n) ? n : int.MaxValue;
    }

    private void PrepareCurrentVideo()
    {
        if (videoPaths.Count == 0 || videoPlayer == null) return;

        string videoUrl = videoPaths[currentVideoIndex];
        if (string.IsNullOrEmpty(videoUrl))
        {
            Debug.LogError("El path del vídeo es NULL o vacío.");
            return;
        }

        videoPlayer.prepareCompleted -= OnVideoPrepared;
        videoPlayer.prepareCompleted += OnVideoPrepared;
        videoPlayer.loopPointReached -= OnFolderVideoEnd;
        videoPlayer.loopPointReached += OnFolderVideoEnd;

        videoPlayer.url = videoUrl;
        videoPlayer.Prepare();
        Debug.Log("Preparando vídeo: " + Path.GetFileName(videoUrl));
        StartCoroutine(CheckAndRetryVideoStartCoroutine());
    }

    private void OnVideoPrepared(VideoPlayer vp)
    {
        vp.Play();
        Debug.Log("Vídeo preparado y reproduciendo: " + Path.GetFileName(vp.url));

        if (metricMgr != null)
        {
            metricMgr.LogUsageEvent("EntornoCargado", Path.GetFileName(vp.url), "0");
            environmentActive = true;
        }
    }

    private void OnFolderVideoEnd(VideoPlayer _)
    {
        StopVideo();
        if (playFirstVideo) return;   // salta si era el vídeo de introducción

        var (activate, deactivate) = GetActivationPair();
        if (activate   != null) activate.SetActive(true);
        if (deactivate != null) deactivate.SetActive(false);
    }

    private IEnumerator CheckAndRetryVideoStartCoroutine()
    {
        yield return new WaitForSeconds(2f);
        if (!videoPlayer.isPlaying)
        {
            Debug.LogWarning("El vídeo no comenzó tras 2 s. Reintentando…");
            PrepareCurrentVideo();
        }
    }

    #endregion

    #region Reproducción
    public string GetCurrentVideoName() =>
        videoPlayer != null && !string.IsNullOrEmpty(videoPlayer.url)
            ? Path.GetFileName(videoPlayer.url)
            : null;

    public float GetCurrentPlaybackTime() =>
        videoPlayer != null ? (float)videoPlayer.time : 0f;

    private void NextVideo()
    {
        currentVideoIndex = (currentVideoIndex + 1) % videoPaths.Count;
        PrepareCurrentVideo();
    }

    public void PlayVideoByIndex(int index)
    {
        if (metricMgr != null)
            metricMgr.LogUsageEvent("UIClicked", "PlayVideoByIndex", "0");

        if (index < 0 || index >= videoPaths.Count)
        {
            Debug.LogWarning("Índice fuera de rango.");
            return;
        }

        currentVideoIndex = index;
        PrepareCurrentVideo();
    }

    #endregion

    #region UI y control 

    public void ResumeVideo()
    {
        if (metricMgr != null)
        {
            metricMgr.LogUsageEvent("StartVideo",
                "StartVideo_" + GetCurrentVideoName(),
                GetCurrentPlaybackTime().ToString("F2"));
        }

        if (videoPlayer != null && videoPlayer.isPrepared) videoPlayer.Play();
        RevertCameraToSkybox();

        // Oculta paneles interactables
        SetPanelsInteractable(false);

        // Gestión de menús bajo
        if (!playFirstVideo)
            SetMenusBajo(false);
        else
            SetMenuByLanguage(true);

        SetSkyboxTintColor("#808080");
    }

    public void StopVideo()
    {
        if (metricMgr != null && environmentActive)
        {
            string videoName = GetCurrentVideoName();
            string playback  = GetCurrentPlaybackTime().ToString("F2");

            metricMgr.LogUsageEvent("EntornoDescargado", videoName, playback);
            metricMgr.LogUsageEvent("VideoEnd", "End_" + videoName, playback);

            float tol = 0.1f;
            if (videoPlayer.length > 0 && videoPlayer.length - videoPlayer.time <= tol)
                metricMgr.LogUsageEvent("EnviromentComplete", videoName, playback);
            else
                metricMgr.LogUsageEvent("EnviromentIncomplete", videoName, playback);

            environmentActive = false;
        }

        videoPlayer?.Pause();
        textMeshPro?.gameObject.SetActive(false);
        SetMenusBajo(false);
        SetPanelsInteractable(true);
        SetCameraToSolidColor("#000000");
    }

    public void ClickUIVolver() => metricMgr.LogUsageEvent("UIClicked", "Volver", "0");
    public void ClickUISalir()  => metricMgr.LogUsageEvent("UIClicked", "Salir",  "0");

    public void VideoSkipTime(float t) => videoPlayer.time = t;
    public void StopVideoAfterSeconds(float s) => StartCoroutine(StopAfterSecondsCoroutine(s));
    private IEnumerator StopAfterSecondsCoroutine(float s) { yield return new WaitForSeconds(s); StopVideo(); }

    public void ToggleLanguage()
    {
        currentLanguage = (Language)(((int)currentLanguage + 1) % 4);
        metricMgr?.LogUsageEvent("UIClicked", "SelectLanguage_" + currentLanguage, "0");
    }

    /// <summary>Asigna idioma por índice (0‑3) desde UI.</summary>
    public void SetLanguage(int langIndex)
    {
        if (langIndex is < 0 or > 3) return;
        currentLanguage = (Language)langIndex;
        metricMgr?.LogUsageEvent("UIClicked", "SelectLanguage_" + currentLanguage, "0");
    }

    /// <summary>Método legacy (mantiene compatibilidad). true=español, false=inglés.</summary>
    public void SetLanguage(bool value) => SetLanguage(value ? 0 : 1);

    public void SetVideoOption(bool option)
    {
        playFirstVideo  = option;
        currentVideoIndex = playFirstVideo ? 0 : 1;

        metricMgr?.LogUsageEvent("UIClicked",
            $"SelectVideo_{GetCurrentVideoName()}", "0");

        PrepareCurrentVideo();
    }

    public void RestartLevel()
    {
        if (metricMgr != null)
        {
            metricMgr.LogUsageEvent("SessionEnd", "Restart",
                (Time.time - sessionStartTime).ToString());
            metricMgr.DumpSessionToFile();
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    #endregion

    #region Skybox y cámara 

    private void SetSkyboxTintColor(string hex)
    {
        if (materialSkyboxVideo == null) return;
        if (ColorUtility.TryParseHtmlString(hex, out Color c))
            materialSkyboxVideo.SetColor("_Tint", c);
    }

    public void SetCameraToSolidColor(string hex)
    {
        Camera cam = Camera.main; if (cam == null) return;
        cam.clearFlags = CameraClearFlags.SolidColor;
        if (ColorUtility.TryParseHtmlString(hex, out Color c))
        {
            c.a = 0f;
            cam.backgroundColor = c;
        }
    }

    public void RevertCameraToSkybox()
    {
        Camera cam = Camera.main;
        if (cam != null) cam.clearFlags = CameraClearFlags.Skybox;
    }

    #endregion

    #region Subtítulos 

    public void MostrarTextoEnTMP(string texto)
    {
        if (textMeshPro == null) return;
        textMeshPro.text = texto;
        if (!textMeshPro.gameObject.activeSelf)
            textMeshPro.gameObject.SetActive(true);
    }

    public void IniciarCambioTexto()
    {
        if (cambioTextoCoroutine != null) StopCoroutine(cambioTextoCoroutine);

        textMeshPro.text = "";
        textMeshPro.gameObject.SetActive(true);

        string[] nombres = GetNamesArray_Solventia();
        cambioTextoCoroutine = StartCoroutine(
            CambiarTextoCoroutine(tiempos, nombres));
    }

    private IEnumerator CambiarTextoCoroutine(float[] t, string[] nombres)
    {
        for (int i = 0; i < t.Length; i++)
        {
            while (videoPlayer.time < t[i]) yield return null;
            MostrarTextoEnTMP(nombres[i]);

            metricMgr?.LogUsageEvent("ObjectiveCompleted", nombres[i], "0");
        }
    }

    public void IniciarCambioTextoCedaga()
    {
        if (cambioTextoCoroutine != null) StopCoroutine(cambioTextoCoroutine);

        textMeshPro.text = "";
        textMeshPro.gameObject.SetActive(true);

        string[] nombres = GetNamesArray_Cedaga();
        cambioTextoCoroutine = StartCoroutine(
            CambiarTextoCoroutine(tiemposCedaga, nombres));
    }

    private string[] GetNamesArray_Solventia() => currentLanguage switch
    {
        Language.Spanish => nombresES,
        Language.English => nombresEN,
        Language.French  => nombresFR,
        Language.German  => nombresDE,
        _ => nombresES
    };

    private string[] GetNamesArray_Cedaga() => currentLanguage switch
    {
        Language.Spanish => nombresESCedaga,
        Language.English => nombresENCedaga,
        Language.French  => nombresFRCedaga,
        Language.German  => nombresDECedaga,
        _ => nombresESCedaga
    };

    #endregion

    #region Helpers de UI 

    private (GameObject activate, GameObject deactivate) GetActivationPair() => currentLanguage switch
    {
        Language.Spanish => (gameObjectToActivateES, gameObjectToDesactivateES),
        Language.English => (gameObjectToActivateEN, gameObjectToDesactivateEN),
        Language.French  => (gameObjectToActivateFR, gameObjectToDesactivateFR),
        Language.German  => (gameObjectToActivateDE, gameObjectToDesactivateDE),
        _ => (null, null)
    };

    private (GameObject panelES, GameObject panelEN, GameObject panelFR, GameObject panelDE) GetPanels()
        => (panelInteractableES, panelInteractableEN, panelInteractableFR, panelInteractableDE);

    private (GameObject es, GameObject en, GameObject fr, GameObject de) GetMenus()
        => (menuBajoES, menuBajoEN, menuBajoFR, menuBajoDE);

    private void SetPanelsInteractable(bool state)
    {
        var (es, en, fr, de) = GetPanels();
        es?.SetActive(state && currentLanguage == Language.Spanish);
        en?.SetActive(state && currentLanguage == Language.English);
        fr?.SetActive(state && currentLanguage == Language.French);
        de?.SetActive(state && currentLanguage == Language.German);
    }

    private void SetMenusBajo(bool state)
    {
        var (es, en, fr, de) = GetMenus();
        es?.SetActive(state && currentLanguage == Language.Spanish);
        en?.SetActive(state && currentLanguage == Language.English);
        fr?.SetActive(state && currentLanguage == Language.French);
        de?.SetActive(state && currentLanguage == Language.German);
    }

    private void SetMenuByLanguage(bool active) => SetMenusBajo(active);

    #endregion

    #region Interface IVideoPlaybackService 

    public void Initialize()               => throw new NotImplementedException();
    public bool  IsVideoPlaying()          => throw new NotImplementedException();
    public void  PlayCurrentVideo()        => throw new NotImplementedException();
    public void  Play()                    => throw new NotImplementedException();
    public void  Pause()                   => throw new NotImplementedException();
    #endregion
}
