using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Captura y gestiona el buffer de UsageEvent.
/// </summary>
public class MetricsManager : MonoBehaviour, IMetricsManager
{
    /* -------------------- Configurable en el Inspector -------------------- */

    [Tooltip("Intervalo (seg.) para volcar el buffer a disco (sin usar actualmente).")]
    [SerializeField] private float flushInterval = 10f;

    [Tooltip("Eventos máximos en buffer antes de forzar volcado (sin usar actualmente).")]
    [SerializeField] private int maxBufferSize  = 10;

    [Tooltip("Componente que implementa IVideoPlaybackService.")]
    [SerializeField] private MonoBehaviour videoPlaybackServiceComponent;

    /* -------------------- Datos internos -------------------- */

    private readonly List<UsageEvent> eventBuffer = new List<UsageEvent>();

    private string sessionId       = null;
    private string currentFilename = null;

    private IVideoPlaybackService _videoService;
    private bool _skipFirstResumeEvent = true;

    private string deviceID;

    /* --------------------------------------------------------------------- */
    /*  INICIALIZACIÓN                                                        */
    /* --------------------------------------------------------------------- */

    private void Awake()
    {
        // Auto-init para garantizar que sessionId existe
        EnsureInitialized();
    }

    /// <summary>
    /// Inicializa nombres de archivo, sessionId y referencias.
    /// Llamar solo una vez por sesión.
    /// </summary>
    public void Initialize()
    {
        const string Key = "DeviceShortID";
        const int Digits = 6;
        if (!PlayerPrefs.HasKey(Key))
        {
            int max = (int)Mathf.Pow(10, Digits);
            int val = UnityEngine.Random.Range(0, max);
            string id = val.ToString().PadLeft(Digits, '0');
            PlayerPrefs.SetString(Key, id);
            PlayerPrefs.Save();
        }
        deviceID = PlayerPrefs.GetString(Key);

        sessionId       = MetricsHelper.GenerateSessionID();
        currentFilename = MetricsHelper.GetCurrentDateFilename();
        _videoService   = videoPlaybackServiceComponent as IVideoPlaybackService;

        if (_videoService == null)
            Debug.LogError("[Metrics] videoPlaybackServiceComponent no implementa IVideoPlaybackService.");

        Debug.Log($"[Metrics] Inicializado con sessionId={sessionId}");
    }

    /// <summary>
    /// Garantiza que Initialize() se ha ejecutado.
    /// </summary>
    private void EnsureInitialized()
    {
        if (string.IsNullOrEmpty(sessionId))
            Initialize();
    }

    /* --------------------------------------------------------------------- */
    /*  REGISTRO DE EVENTOS                                                   */
    /* --------------------------------------------------------------------- */

    public void LogUsageEvent(string eventName, string objectId, string duration)
    {
        EnsureInitialized(); // ← nuevo

        UsageEvent newEvent = new UsageEvent(sessionId, eventName, objectId, duration);
        eventBuffer.Add(newEvent);
    }

    /* --------------------------------------------------------------------- */
    /*  PAUSA / RESUME                                                        */
    /* --------------------------------------------------------------------- */

    public void PauseVideo()
    {
        EnsureInitialized();

        if (_videoService == null) return;

        Debug.Log("Usuario ausente: pausando vídeo.");
        _videoService.Pause();

        float   rawTime      = _videoService.GetCurrentPlaybackTime();
        string  playbackTime = rawTime.ToString("F2");
        string  videoName    = _videoService.GetCurrentVideoName();

        LogUsageEvent("VideoEnd",  "End_"  + videoName, playbackTime);
        LogUsageEvent("SessionEnd","End",  "0");
    }

    public void ResumeVideo()
    {
        EnsureInitialized();

        if (_skipFirstResumeEvent) { _skipFirstResumeEvent = false; return; }
        if (_videoService == null) return;

        Debug.Log("Usuario presente: reanudando vídeo.");
        _videoService.Play();

        float   rawTime      = _videoService.GetCurrentPlaybackTime();
        string  playbackTime = rawTime.ToString("F2");
        string  videoName    = _videoService.GetCurrentVideoName();

        LogUsageEvent("SessionStart","Start","0");
        LogUsageEvent("VideoStart",  "Start_" + videoName, playbackTime);
    }

    /* --------------------------------------------------------------------- */
    /*  VOLCADO SESIÓN COMPLETA                                               */
    /* --------------------------------------------------------------------- */

    public void DumpSessionToFile()
    {
        EnsureInitialized();
        string filePath = Path.Combine(Application.persistentDataPath, currentFilename);

        // 1. Prepara el payload actual
        SessionPayload payload = new SessionPayload
        {
            sessionId = sessionId,
            deviceID = deviceID, // ← esta línea añade el ID del dispositivo
            events = new List<UsageEvent>(eventBuffer)
        };

        // 2. Crea la lista de sesiones (lee el fichero si existe)
        SessionPayloadList sessionList = new SessionPayloadList();

        if (File.Exists(filePath))
        {
            string fileContent = File.ReadAllText(filePath).Trim();
            if (!string.IsNullOrEmpty(fileContent))
            {
                try
                {
                    // JsonUtility no soporta arrays root, así que lo "envolvemos"
                    sessionList = JsonUtility.FromJson<SessionPayloadList>("{\"sessions\":" + fileContent + "}");
                }
                catch (System.Exception ex)
                {
                    Debug.LogWarning("[MetricsManager] Error al deserializar métricas existentes, se sobrescribirá: " + ex.Message);
                    sessionList = new SessionPayloadList();
                }
            }
        }

        // 3. Añade la nueva sesión a la lista
        sessionList.sessions.Add(payload);

        // 4. Serializa todo y elimina el wrapper para dejar solo el array
        string jsonWithWrapper = JsonUtility.ToJson(sessionList, true);
        int startIdx = jsonWithWrapper.IndexOf('[');
        int endIdx = jsonWithWrapper.LastIndexOf(']');
        string finalJson = jsonWithWrapper.Substring(startIdx, endIdx - startIdx + 1);

        File.WriteAllText(filePath, finalJson);

        // 5. Limpia el buffer
        eventBuffer.Clear();
    }


    /* -------------------  (Flush periódico deshabilitado) ------------------ */
}