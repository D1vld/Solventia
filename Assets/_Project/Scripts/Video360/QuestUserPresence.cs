using UnityEngine;
using UnityEngine.Video;
using Oculus;                     // Requiere Oculus Integration
using Oculus.Platform;            // Según tu configuración
using Oculus.Platform.Models;
using System.Globalization;
using System;

/// <summary>
/// Pausa/reanuda el vídeo según la presencia del usuario
/// y activa un GameObject si el visor estuvo quitado
/// más de "delayToActivate" segundos.
/// </summary>
public class QuestUserPresence : MonoBehaviour
{
    /* ------------- Configuración en el Inspector ------------- */

    [Header("Video a Pausar/Reanudar")]
    public VideoPlayer videoPlayer;

    [Header("Permite reproducir el vídeo al usar el visor")]
    public bool canPlayVideo = false;

    [Header("Managers externos")]
    [SerializeField] private MetricsManager  metricsManager;
    [SerializeField] private Video360Manager video360Manager;

    [Header("Activar GO tras quitar el visor > delay")]
    [Tooltip("GameObject que se activará al volver a ponerse el visor")]
    [SerializeField] private GameObject goToActivate;

    [Tooltip("Segundos que el visor debe estar quitado antes de activar el GO")]
    [SerializeField] [Min(0f)] private float delayToActivate = 5f;

    /* ------------- Variables internas ------------- */

    /// <summary>Momento en que se desmontó el visor; -1 = sin desmontar.</summary>
    private float unmountedTime = -1f;

    /* ------------- Ciclo de vida ------------- */

    private void OnEnable()
    {
        OVRManager.HMDMounted   += OnHMDMounted;
        OVRManager.HMDUnmounted += OnHMDUnmounted;

        if (videoPlayer != null)
            videoPlayer.loopPointReached += OnVideoFinished;
    }

    private void OnDisable()
    {
        OVRManager.HMDMounted   -= OnHMDMounted;
        OVRManager.HMDUnmounted -= OnHMDUnmounted;

        if (videoPlayer != null)
            videoPlayer.loopPointReached -= OnVideoFinished;
    }

    /* ------------- API pública ------------- */

    /// <summary>Permite cambiar la bandera desde UI u otros scripts.</summary>
    public void SetCanPlayVideo(bool value)
    {
        canPlayVideo = value;
        Debug.Log("canPlayVideo ahora es: " + canPlayVideo);
    }

    /* ------------- Callbacks de presencia ------------- */

    /// <summary>Llamado automáticamente cuando se coloca de nuevo el visor.</summary>
    private void OnHMDMounted()
    {
        /* --- 1. Activar el GameObject si procede --- */
        if (unmountedTime >= 0f &&
            Time.time - unmountedTime > delayToActivate &&
            goToActivate != null)
        {
            goToActivate.SetActive(true);
            Debug.Log($"GameObject '{goToActivate.name}' activado tras {Time.time - unmountedTime:F1}s sin visor.");
        }

        unmountedTime = -1f; // reset

        /* --- 2. Reanudar el vídeo y registrar métricas --- */
        string vidResumeName = video360Manager.GetCurrentVideoName();
        float  currentTime   = video360Manager.GetCurrentPlaybackTime();

        float truncatedTime = (float)(Math.Truncate(currentTime * 100.0) / 100.0);
        string timeVideo    = truncatedTime.ToString("F2", CultureInfo.InvariantCulture);

        if (videoPlayer != null && canPlayVideo)
        {
            videoPlayer.Play();
            Debug.Log("Visor montado: Reanudando el vídeo.");

            metricsManager.LogUsageEvent("SessionStarted", "Resume", "0");
            metricsManager.LogUsageEvent("VideoStart",   "Resume_" + vidResumeName, timeVideo);
        }
    }

    /// <summary>Llamado automáticamente cuando se quita el visor.</summary>
    private void OnHMDUnmounted()
    {
        unmountedTime = Time.time; // guardamos el momento exacto

        string vidPauseName = video360Manager.GetCurrentVideoName();
        float  currentTime  = video360Manager.GetCurrentPlaybackTime();

        float truncatedTime = (float)(Math.Truncate(currentTime * 100.0) / 100.0);
        string timeVideo    = truncatedTime.ToString("F2", CultureInfo.InvariantCulture);

        if (videoPlayer != null && videoPlayer.isPlaying)
        {
            videoPlayer.Pause();
            Debug.Log("Visor desmontado: Vídeo en pausa.");

            metricsManager.LogUsageEvent("VideoEnd",    "Pause_" + vidPauseName, timeVideo);
            metricsManager.LogUsageEvent("SessionEnded","Pause", "0");
        }
    }

    /* ------------- Callback fin de vídeo ------------- */

    private void OnVideoFinished(VideoPlayer vp)
    {
        canPlayVideo = false;
        Debug.Log("Vídeo finalizado. canPlayVideo ahora es false.");
    }
}
