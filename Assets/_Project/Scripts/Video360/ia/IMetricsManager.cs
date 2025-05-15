using UnityEngine;

/// <summary>
/// API pública para el sistema de métricas.
/// </summary>
public interface IMetricsManager
{
    void Initialize();

    void LogUsageEvent(string eventName, string objectId, string duration);

    void PauseVideo();
    void ResumeVideo();

    void DumpSessionToFile();
}