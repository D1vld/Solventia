using UnityEngine;
using UnityEngine.Video;
using Oculus;                     // Requiere Oculus Integration
using Oculus.Platform;            // Según tu configuración
using Oculus.Platform.Models;
using System.Globalization;
using System;

public class QuestUserPresence : MonoBehaviour
{
    /* ------------- Configuración en el Inspector ------------- */
    
    [Header("Video a Pausar/Reanudar")]
    public VideoPlayer videoPlayer;

    [Header("Permite reproducir el vídeo al usar el visor")]
    public bool canPlayVideo = false;

    [Header("Managers externos")]
    [SerializeField] private MetricsManager metricsManager;
    [SerializeField] private Video360Manager video360Manager;

    /* ------------- Variables internas ------------- */

    /// <summary>Momento en que se desmontó el visor; -1 = sin desmontar.</summary>
    private float unmountedTime = -1f;

    /* ------------- Referencia al GameObject para activar ------------- */
    
    [Header("GameObject a Activar")]
    public GameObject objectToActivate;  // Se asignará desde el Inspector

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
        // Lógica para cuando se coloca el visor.
    }

    /// <summary>Llamado automáticamente cuando se quita el visor.</summary>
    private void OnHMDUnmounted()
    {
        // Añadimos más depuración para asegurar que esta función se llama correctamente.
        Debug.Log("Visor desmontado: OnHMDUnmounted llamado.");
        
        // Aquí activamos el GameObject asignado desde el Inspector
        if (objectToActivate != null)
        {
            objectToActivate.SetActive(true);  // Activar el GameObject
            Debug.Log("GameObject activado.");
        }
        else
        {
            Debug.LogWarning("No se ha asignado ningún GameObject en el Inspector.");
        }

        unmountedTime = Time.time; // guardamos el momento exacto

        string vidPauseName = video360Manager.GetCurrentVideoName();
        float  currentTime  = video360Manager.GetCurrentPlaybackTime();

        float truncatedTime = (float)(Math.Truncate(currentTime * 100.0) / 100.0);
        string timeVideo    = truncatedTime.ToString("F2", CultureInfo.InvariantCulture);

        if (videoPlayer != null && videoPlayer.isPlaying)
        {
            videoPlayer.Pause();
            Debug.Log("Visor desmontado: Vídeo en pausa.");

            metricsManager.LogUsageEvent("VideoEnd",    "End_" + vidPauseName, timeVideo);
            metricsManager.LogUsageEvent("SessionEnd", "End", "0");
        }
    }

    /* ------------- Callback fin de vídeo ------------- */

    private void OnVideoFinished(VideoPlayer vp)
    {
        canPlayVideo = false;
        Debug.Log("Vídeo finalizado. canPlayVideo ahora es false.");
    }
}
