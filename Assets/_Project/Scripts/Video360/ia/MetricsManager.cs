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

        LogUsageEvent("VideoEnd",  "Pause_"  + videoName, playbackTime);
        LogUsageEvent("SessionEnd","Pause",  "0");
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

        LogUsageEvent("SessionStart","Resume","0");
        LogUsageEvent("VideoStart",  "Resume_" + videoName, playbackTime);
    }

    /* --------------------------------------------------------------------- */
    /*  VOLCADO SESIÓN COMPLETA                                               */
    /* --------------------------------------------------------------------- */

    public void DumpSessionToFile()
    {
        EnsureInitialized();

        string filePath = Path.Combine(Application.persistentDataPath, currentFilename);

        SessionPayload payload = new SessionPayload
        {
            sessionId = sessionId,
            events    = eventBuffer
        };

        string json = JsonUtility.ToJson(payload, true);
        File.AppendAllText(filePath, json);

        eventBuffer.Clear();
        Debug.Log($"[Metrics] Sesión volcada a {filePath}");
    }

    /* -------------------  (Flush periódico deshabilitado) ------------------ */
}