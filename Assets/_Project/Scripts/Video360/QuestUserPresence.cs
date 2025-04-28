using UnityEngine;
using UnityEngine.Video;
using Oculus;            // Si necesitas referencias específicas del Oculus Integration
using Oculus.Platform;   // Dependiendo de cómo lo tengas configurado
using Oculus.Platform.Models;
using System.Globalization;
using System;


public class QuestUserPresence : MonoBehaviour
{
    [Header("Video a Pausar/Reanudar")]
    public VideoPlayer videoPlayer;

    [Header("Permite reproducir el video al usar el visor")]
    public bool canPlayVideo = false;

    [SerializeField] private MetricsManager metricsManager;
    [SerializeField] private Video360Manager video360Manager;

    void OnEnable()
    {
        // Suscribirse a los eventos de Oculus
        OVRManager.HMDMounted += OnHMDMounted;
        OVRManager.HMDUnmounted += OnHMDUnmounted;

        // Suscribirse al evento de finalización del video (si no está en loop)
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached += OnVideoFinished;
        }
    }

    void OnDisable()
    {
        // Desuscribirse de los eventos de Oculus
        OVRManager.HMDMounted -= OnHMDMounted;
        OVRManager.HMDUnmounted -= OnHMDUnmounted;

        // Desuscribirse del evento de finalización del video
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached -= OnVideoFinished;
        }
    }

    /// <summary>
    /// Función para establecer el valor de canPlayVideo desde otros scripts o UI.
    /// </summary>
    /// <param name="value">true para permitir reproducción automática, false para desactivarla</param>
    public void SetCanPlayVideo(bool value)
    {
        canPlayVideo = value;
        Debug.Log("canPlayVideo ahora es: " + canPlayVideo);
    }

    /// <summary>
    /// Se llama automáticamente cuando el visor se detecta montado en la cabeza (HMDMounted).
    /// </summary>
    private void OnHMDMounted()
{
    string vidResumeName = video360Manager.GetCurrentVideoName();
    float currentTime  = video360Manager.GetCurrentPlaybackTime();

    // Usamos System.Math.Truncate y luego casteamos a float
    float truncatedTime = (float)(Math.Truncate(currentTime * 100.0) / 100.0);

    string timeVideo = truncatedTime.ToString("F2", CultureInfo.InvariantCulture);

    if (videoPlayer != null && canPlayVideo)
    {
        videoPlayer.Play();
        Debug.Log("Visor montado: Reanudando el video.");

        metricsManager.LogUsageEvent("SessionStarted", "Resume", "0");
        metricsManager.LogUsageEvent("VideoStart", "Resume_" + vidResumeName, timeVideo);
    }
    else
    {
        Debug.Log("Visor montado, pero canPlayVideo = false. No se reproduce el video.");
    }
}
    /// <summary>
    /// Se llama automáticamente cuando el visor se detecta desmontado de la cabeza (HMDUnmounted).
    /// </summary>
    private void OnHMDUnmounted()
    {
        string vidPauseName = video360Manager.GetCurrentVideoName();
        float currentTime  = video360Manager.GetCurrentPlaybackTime();

        // Usamos System.Math.Truncate y luego casteamos a float
        float truncatedTime = (float)(Math.Truncate(currentTime * 100.0) / 100.0);

        string timeVideo = truncatedTime.ToString("F2", CultureInfo.InvariantCulture);

        if (videoPlayer != null && videoPlayer.isPlaying)
        {
            // Pausamos el video cuando el usuario se quita el visor
            videoPlayer.Pause();
            Debug.Log("Visor desmontado: Video en pausa.");

            metricsManager.LogUsageEvent("VideoEnd", "Pause_"+vidPauseName, timeVideo);
            metricsManager.LogUsageEvent("SessionEnded", "Pause", "0");

        }
    }

    /// <summary>
    /// Se llama automáticamente cuando el VideoPlayer llega al final (loopPointReached).
    /// </summary>
    private void OnVideoFinished(VideoPlayer vp)
    {
        // Cuando termina el video, ponemos el bool en false
        canPlayVideo = false;
        Debug.Log("Video finalizado. canPlayVideo ahora es false.");
    }
}
