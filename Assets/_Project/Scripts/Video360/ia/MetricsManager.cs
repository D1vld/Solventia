using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// MetricsManager: Clase para capturar y gestionar el buffer de UsageEvent.
/// </summary>
public class MetricsManager : MonoBehaviour
{
    private List<UsageEvent> eventBuffer = new List<UsageEvent>();
    private string sessionId;
    private string currentFilename;

    [Tooltip("Intervalo (en segundos) para volcar el buffer a disco.")]
    [SerializeField] private float flushInterval = 10f;

    [Tooltip("Cantidad m�xima de eventos en el buffer antes de forzar un volcado a disco.")]
    [SerializeField] private int maxBufferSize = 10;

    [Tooltip("Servicio de reproducci�n de video.")]
    //[SerializeField] private MonoBehaviour videoPlaybackServiceComponent;
    [SerializeField] private Video360Manager _videoService;

    private bool _skipFirstResumeEvent = true;
    private bool sessionEnded = false;
    void Awake()
    {
        Initialize();
    }
    public void Initialize()
    {
        sessionId = MetricsHelper.GenerateSessionID();
        currentFilename = MetricsHelper.GetCurrentDateFilename();
        //_videoService = videoPlaybackServiceComponent as IVideoPlaybackService;
        StartCoroutine(FlushBufferPeriodically());
    }

    public void LogUsageEvent(string eventName, string objectId, string duration)
    {
        UsageEvent newEvent = new UsageEvent(sessionId, eventName, objectId, duration);
        eventBuffer.Add(newEvent);

        if (eventBuffer.Count >= maxBufferSize)
        {
            StartCoroutine(FlushBuffer());
        }
    }

    public void PauseVideo()
    {
        Debug.Log("Usuario ausente: pausando v�deo.");
        _videoService.videoPlayer.Pause();

        float rawTime = _videoService.GetCurrentPlaybackTime();
        string videoName = _videoService.GetCurrentVideoName();
        string playbackTime = rawTime.ToString("F2");
        //LogUsageEvent("VideoEnd", "Pause", playbackTime);
    }

    public void ResumeVideo()
    {
        if (_skipFirstResumeEvent)
        {
            _skipFirstResumeEvent = false;
            return;
        }

        Debug.Log("Usuario presente: reanudando v�deo.");
        _videoService.videoPlayer.Play();

        float rawTime = _videoService.GetCurrentPlaybackTime();
        string videoName = _videoService.GetCurrentVideoName();
        string playbackTime = rawTime.ToString("F2");
        LogUsageEvent("VideoStart", "Pause", playbackTime);
    }

    private IEnumerator FlushBufferPeriodically()
    {
        while (true)
        {
            yield return new WaitForSeconds(flushInterval);

            if (eventBuffer.Count > 0)
            {
                yield return StartCoroutine(FlushBuffer());
            }
        }
    }

    private IEnumerator FlushBuffer()
    {
        List<UsageEvent> eventsToFlush = new List<UsageEvent>(eventBuffer);
        eventBuffer.Clear();

        string filePath = Path.Combine(Application.persistentDataPath, currentFilename);

        try
        {
            using (StreamWriter sw = new StreamWriter(filePath, true))
            {
                foreach (UsageEvent usageEvent in eventsToFlush)
                {
                    string jsonStr = JsonUtility.ToJson(usageEvent);
                    sw.WriteLine(jsonStr);
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error al volcar eventos a disco: " + e);
            eventBuffer.InsertRange(0, eventsToFlush);
        }

        yield return null;
    }
}
