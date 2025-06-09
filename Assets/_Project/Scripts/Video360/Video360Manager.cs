using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class Video360Manager : MonoBehaviour
{
    /*================================================================*/
    #region  INSPECTOR FIELDS 

    [Header("Video Player 360")]
    [SerializeField] private VideoPlayer videoPlayer;

    [Header("Carpeta local de los vídeos 360")]
    [SerializeField] private string folderPath = "/storage/emulated/0/Movies/360Videos";

    [Header("Opciones de Vídeo")]
    [SerializeField] private bool playFirstVideo = true;

    [Header("Objetos a Activar / Desactivar al final")]
    [SerializeField] private GameObject objectToActivate;
    [SerializeField] private GameObject objectToDeactivate;

    [Header("Material del Skybox (vídeo 360)")]
    [SerializeField] private Material materialSkyboxVideo;

    /*——  NUEVO  ————————————————————————————————————————————————*/
    [Header("Fragmentos dentro del clip (seg.)")]
    [Tooltip("Rangos [inicio, fin) en segundos; ordénalos por «inicio»")]
    public List<VideoFragment> fragmentos = new List<VideoFragment>();
    /*================================================================*/
    #endregion

    #region  STRUCTS 
    [System.Serializable]
    public struct VideoFragment   // visible en el Inspector
    {
        public float inicio;     // inclusive
        public float fin;        // exclusivo
    }
    #endregion

    /*================================================================*/
    #region  PRIVATE FIELDS 
    private List<string> videoPaths = new List<string>();
    private int  currentVideoIndex = 0;  // clip dentro de la carpeta
    private int  fragmentIndex     = 0;  // fragmento lógico dentro del clip

    //  Cola de seeks (para evitar bloqueos por saltos muy seguidos)
    private bool    seekEnCurso   = false;
    private double? seekPendiente = null;
    #endregion

    /*================================================================*/
    #region  UNITY LIFECYCLE 
    private void Awake()
    {
        if (videoPlayer == null) videoPlayer = GetComponent<VideoPlayer>();
        videoPlayer.playOnAwake   = false;
        videoPlayer.seekCompleted += OnSeekCompleted;
    }

    private void Start()
    {
        if (materialSkyboxVideo != null) SetSkyboxTintColor("#808080");

        LoadAndSortVideos();

        if (videoPaths.Count >= 2)       currentVideoIndex = playFirstVideo ? 0 : 1;
        else if (videoPaths.Count == 1)  currentVideoIndex = 0;

        // Aseguramos fragmentos ordenados
        fragmentos = fragmentos.OrderBy(f => f.inicio).ToList();

        PrepareCurrentVideo();
    }

    private void Update()
    {
        if (videoPlayer == null || !videoPlayer.isPrepared || !videoPlayer.isPlaying ||
            fragmentos.Count == 0) return;

        double t = videoPlayer.time;

        // ¿En qué fragmento estamos?
        int idxActual = -1;
        for (int i = fragmentos.Count - 1; i >= 0; i--)
            if (t >= fragmentos[i].inicio) { idxActual = i; break; }

        // ¿Alcanzó el fin del fragmento?
        if (idxActual != -1 &&
            idxActual < fragmentos.Count - 1 &&
            t >= fragmentos[idxActual].fin - 0.05f)
        {
            SkipToTime(fragmentos[idxActual + 1].inicio);
            fragmentIndex = idxActual + 1;
        }
    }

    private void OnApplicationQuit() => StopVideo();
    #endregion

    /*================================================================*/
    #region CARGA Y PREPARACIÓN DE VÍDEOS 
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

        videoPaths = files
            .OrderBy(path => ExtractLeadingNumber(Path.GetFileNameWithoutExtension(path)))
            .ToList();
    }

    private int ExtractLeadingNumber(string name)
    {
        var parts = name.Split('_');
        return (parts.Length > 0 && int.TryParse(parts[0], out int n)) ? n : int.MaxValue;
    }

    private void PrepareCurrentVideo()
    {
        if (videoPaths.Count == 0 || videoPlayer == null) return;

        string url = videoPaths[currentVideoIndex];
        videoPlayer.url = url;

        videoPlayer.prepareCompleted -= OnVideoPrepared;
        videoPlayer.prepareCompleted += OnVideoPrepared;

        videoPlayer.loopPointReached -= OnVideoEndReached;
        videoPlayer.loopPointReached += OnVideoEndReached;

        videoPlayer.Prepare();
    }

    private void OnVideoPrepared(VideoPlayer vp)
    {
        fragmentIndex = 0;
        double inicio = (fragmentos.Count > 0) ? fragmentos[0].inicio : 0.0;
        vp.time = inicio;
        vp.Play();
    }

    private void OnVideoEndReached(VideoPlayer vp)
    {
        SetCameraToSolidColor("#000000");

        if (!playFirstVideo)
        {
            if (objectToActivate)   objectToActivate.SetActive(true);
            if (objectToDeactivate) objectToDeactivate.SetActive(false);
        }
    }
    #endregion

    /*================================================================*/
    #region  MÉTODOS DE REPRODUCCIÓN 
    public void PlayVideoByIndex(int index)
    {
        if (index < 0 || index >= videoPaths.Count) return;
        currentVideoIndex = index;
        PrepareCurrentVideo();
    }

    public void ResumeVideo()
    {
        if (videoPlayer && videoPlayer.isPrepared) videoPlayer.Play();
        RevertCameraToSkybox();
        SetSkyboxTintColor("#808080");
    }

    public void StopVideo()
    {
        if (videoPlayer) videoPlayer.Pause();
        // SetCameraToSolidColor("#000000");   // sólo pintamos al finalizar clip
    }
    #endregion

    /*================================================================*/
    #region SEEK ROBUSTO (cola de peticiones) 
    ///  Salta al instante indicado y reanuda la reproducción.
    public void SkipToTime(float segundos)
    {
        if (videoPlayer == null || !videoPlayer.isPrepared) return;

        segundos = Mathf.Clamp(segundos, 0f, (float)videoPlayer.length);

        // si ya estamos muy cerca, ignora
        if (Mathf.Abs((float)(videoPlayer.time - segundos)) < 0.2f) return;

        // si un seek está en progreso, encola el nuevo
        if (seekEnCurso)
        {
            seekPendiente = segundos;
            return;
        }

        RealizarSeek(segundos);
    }

    private void RealizarSeek(double s)
    {
        seekEnCurso   = true;
        seekPendiente = null;

        videoPlayer.time = s;
        videoPlayer.Play();
    }

    private void OnSeekCompleted(VideoPlayer vp)
    {
        seekEnCurso = false;

        // ¿hay petición pendiente?
        if (seekPendiente.HasValue)
            RealizarSeek(seekPendiente.Value);
    }
    #endregion

    /*================================================================*/
    #region  BOTONES UI 
    public void NextFragmentButton()
    {
        if (fragmentos.Count == 0 || videoPlayer == null || !videoPlayer.isPrepared)
            return;

        double t = videoPlayer.time;
        int idxSiguiente = fragmentos.FindIndex(f => f.inicio > t + 0.0001f);

        if (idxSiguiente != -1)
        {
            SkipToTime(fragmentos[idxSiguiente].inicio);
            fragmentIndex = idxSiguiente;
        }
        else
            Debug.Log("Ya estás en el último fragmento");
    }

    public void PreviousFragmentButton()
    {
        if (fragmentos.Count == 0 || videoPlayer == null || !videoPlayer.isPrepared)
            return;

        double t = videoPlayer.time;

        // 1) ¿En qué fragmento estoy?  (>= para incluir el inicio exacto)
        int idxActual = -1;
        for (int i = fragmentos.Count - 1; i >= 0; i--)
            if (t >= fragmentos[i].inicio) { idxActual = i; break; }

        // 2) El anterior es idxActual - 1
        int idxAnterior = idxActual - 1;

        if (idxAnterior >= 0)
        {
            SkipToTime(fragmentos[idxAnterior].inicio);
            fragmentIndex = idxAnterior;          // opcional: tracking interno
        }
        else
        {
            Debug.Log("Ya estás en el primer fragmento");
        }
    }
    #endregion

    /*================================================================*/
    #region  SKYBOX Y CÁMARA 
    private void SetSkyboxTintColor(string hex)
    {
        if (materialSkyboxVideo &&
            ColorUtility.TryParseHtmlString(hex, out Color c))
            materialSkyboxVideo.SetColor("_Tint", c);
    }

    private void SetCameraToSolidColor(string hex)
    {
        Camera cam = Camera.main; if (cam == null) return;
        cam.clearFlags = CameraClearFlags.SolidColor;
        if (ColorUtility.TryParseHtmlString(hex, out Color c))
        {
            c.a = 0f;
            cam.backgroundColor = c;
        }
    }

    private void RevertCameraToSkybox()
    {
        Camera cam = Camera.main; if (cam) cam.clearFlags = CameraClearFlags.Skybox;
    }
    #endregion

    /*================================================================*/
    #region  UTILIDADES EXTRA (Opcional) 
    public void StopVideoAfterSeconds(float seconds) =>
        StartCoroutine(StopAfterSecondsCoroutine(seconds));

    private IEnumerator StopAfterSecondsCoroutine(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        StopVideo();
    }

    public void SetVideoOption(bool option)
    {
        playFirstVideo = option;
        currentVideoIndex = playFirstVideo ? 0 : 1;
        PrepareCurrentVideo();
    }

    public void RestartLevel()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
    #endregion
}
