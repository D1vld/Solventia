using UnityEngine;
using UnityEngine.Video;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using System.IO;
using System.Linq;
using UnityEngine.SceneManagement;

public class Video360Manager : MonoBehaviour, IVideoPlaybackService
{
    #region Inspector Fields

    [Header("Video Player 360")]
    [SerializeField] public VideoPlayer videoPlayer;

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
        1f, 6.7f, 78f, 116f, 140.5f, 148.5f, 168f, 183f, 190f, 195.8f, 202f, 208f, 217f,
        231f, 241f, 253f, 261.3f, 268f, 276.5f, 284f, 291.5f, 304f, 310f, 317.5f, 326.5f,
        340f, 345f, 354.7f,
    };

    private string[] nombresEN = new string[]
    {
        "Semi-Olympic Pool & Hydrothermal Pool", "Fitness & Weight Training Room", "Calisthenics & Box Area", "OCR Circuit",
        "Padel Court", "Fight Club (Boxing & Dojo)", "CROSSFIT / HYROX", "Pilates Equipment Studio", "Pole Dance Studio",
        "Mind-Body Activities Studio", "Indoor Group Exercise Studio", "Outdoor Group Exercise Area", "Spinning & Elliptical Fit Studio",
        "Hittbox Studio", "Personal Training Studio", "Tennis: Clay Court", "Tennis:Fast Court", "Kids’ Playroom",
        "3x3 Basketball Court", "Football Pitch", "Pool Changing Rooms", "Studios’ Changing Rooms", "Coworking Area",
        "“Sparty” Birthday Party Zone", "Cafeteria", "Nutrition Shop", "Physiotherapy Clinic", "Artworks by Daniel Rodríguez Báez"
    };

    private string[] nombresES = new string[]
    {
        "Piscina Semiolímpica y Piscina Hidrotermal", "Sala Fitness/Musculación", "Calistenia y Box Area", "Circuito OCR",
        "Pista de Pádel", "Fight Club (Boxing y Dojo)", "CrossFit / Hyrox", "Sala Pilates Máquina", "Sala Pole Dance",
        "Sala Actividades Cuerpo/Mente", "Sala Actividades Dirigidas Indoor", "Sala Actividades Dirigidas Outdoor",
        "Sala Spinning y Elíptica Fit", "Sala HITTBOX", "Sala Entrenamiento Personal", "Tenis: Tierra Batida",
        "Tenis: Pista Rápida", "Ludoteca", "Basket 3x3", "Campo de Fútbol", "Vestuarios de Piscina", "Vestuarios de Salas",
        "Zona Coworking", "Zona Cumpleaños Sparty", "Cafetería", "Tienda Nutrición", "Clínica Fisioterapia",
        "Obras de Arte (Artista Daniel Rodríguez Báez)"
    };

    // Array de cedaga (10 elementos)
    private float[] tiemposcedaga = new float[]
    {
        0.8f, 20f, 33f, 70.5f, 84.3f, 100.3f, 114f, 133f, 156f, 172f,
    };

    private string[] nombresENcedaga = new string[]
    {
        "Semi-Olympic Pool", "Instructional & Aquagym Pool", "Fitness & Weight Training Room", "Fight Club (Boxing)",
        "Pilates Reformer Studio", "Group Training Room (Indoor Activities)", "Spinning Studio", "Outdoor Solarium",
        "Kids’ Playroom", "Member Changing Rooms", "Family Changing Rooms",
    };

    private string[] nombresEScedaga = new string[]
    {
        "Piscina Semiolímpica", "Piscina de enseñanza y Aquagym", "Sala Fitness/Musculación", "Fight Club (Boxing)",
        "Sala Pilates Máquina", "Sala Actividades Dirigidas Indoor", "Sala Spinning", "Ludoteca", "Solarium",
        "Vestuarios Abonados", "Vestuarios de Familias",
    };

    private Coroutine cambioTextoCoroutine;

    #endregion

    #region Private Fields

    private int currentVideoIndex = 0;
    private List<string> videoPaths = new List<string>();

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        if (videoPlayer == null)
            videoPlayer = GetComponent<VideoPlayer>();

        videoPlayer.playOnAwake = false;
    }

    private void Start()
    {
        if (materialSkyboxVideo != null)
            SetSkyboxTintColor("#808080");

        LoadAndSortVideos();

        if (videoPaths.Count >= 2)
            currentVideoIndex = playFirstVideo ? 0 : 1;
        else if (videoPaths.Count == 1)
            currentVideoIndex = 0;

        PrepareCurrentVideo();
    }

    private void OnApplicationQuit()
    {
        StopVideo();
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

        videoPaths = files.OrderBy(path => ExtractLeadingNumber(Path.GetFileNameWithoutExtension(path))).ToList();

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

    public string GetCurrentVideoName()
    {
        if (videoPlayer != null && !string.IsNullOrEmpty(videoPlayer.url))
            return Path.GetFileName(videoPlayer.url);
        Debug.Log("No se está reproduciendo ningún video");
        return null;
    }

    public float GetCurrentPlaybackTime()
    {
        return videoPlayer != null ? (float)videoPlayer.time : 0f;
    }

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

        if (!playFirstVideo)
        {
            if (menuBajoES != null) menuBajoES.SetActive(false);
            if (menuBajoEN != null) menuBajoEN.SetActive(false);
        }
        else
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

    public void ClickUIVolver() { }
    public void ClickUISalir() { }

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

    public void RestartLevel()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentScene);
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
        if (cambioTextoCoroutine != null)
        {
            StopCoroutine(cambioTextoCoroutine);
            cambioTextoCoroutine = null;
        }

        textMeshPro.text = "";
        textMeshPro.gameObject.SetActive(true);

        string[] nombresSeleccionados = isSpanish ? nombresES : nombresEN;

        cambioTextoCoroutine = StartCoroutine(CambiarTextoCoroutine(tiempos, nombresSeleccionados));
    }

    private IEnumerator CambiarTextoCoroutine(float[] tiempos, string[] nombres)
    {
        int i = 0;
        while (i < tiempos.Length)
        {
            while (videoPlayer.time < tiempos[i])
            {
                yield return null;
            }

            MostrarTextoEnTMP(nombres[i]);
            i++;
        }
    }

    public void IniciarCambioTextoCedaga()
    {
        if (cambioTextoCoroutine != null)
        {
            StopCoroutine(cambioTextoCoroutine);
            cambioTextoCoroutine = null;
        }

        textMeshPro.text = "";
        textMeshPro.gameObject.SetActive(true);

        string[] nombresSeleccionados = isSpanish ? nombresEScedaga : nombresENcedaga;

        cambioTextoCoroutine = StartCoroutine(CambiarTextoCoroutineCedaga(tiemposcedaga, nombresSeleccionados));
    }

    private IEnumerator CambiarTextoCoroutineCedaga(float[] tiempos, string[] nombres)
    {
        int i = 0;
        while (i < tiempos.Length)
        {
            while (videoPlayer.time < tiempos[i])
            {
                yield return null;
            }

            MostrarTextoEnTMP(nombres[i]);
            i++;
        }
    }

    #endregion

    #region Interface IVideoPlaybackService (Métodos por implementar)

    public void Initialize() { throw new System.NotImplementedException(); }
    public bool IsVideoPlaying() { throw new System.NotImplementedException(); }
    public void PlayCurrentVideo() { throw new System.NotImplementedException(); }
    public void Play() { throw new System.NotImplementedException(); }
    public void Pause() { throw new System.NotImplementedException(); }

    #endregion
}
