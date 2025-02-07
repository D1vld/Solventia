using UnityEngine;
using UnityEngine.Video;
using Oculus;            // Si necesitas referencias específicas del Oculus Integration
using Oculus.Platform;   // Dependiendo de cómo lo tengas configurado
using Oculus.Platform.Models;

public class QuestUserPresence : MonoBehaviour
{
    [Header("Video a Pausar/Reanudar")]
    public VideoPlayer videoPlayer;

    [Header("Permite reproducir el video al montar el visor")]
    public bool canPlayVideo = false;

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
        // Solo reproducimos si canPlayVideo es true.
        if (videoPlayer != null && canPlayVideo)
        {
            videoPlayer.Play();
            Debug.Log("Visor montado: Reanudando el video.");
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
        if (videoPlayer != null && videoPlayer.isPlaying)
        {
            // Pausamos el video cuando el usuario se quita el visor
            videoPlayer.Pause();
            Debug.Log("Visor desmontado: Video en pausa.");
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
