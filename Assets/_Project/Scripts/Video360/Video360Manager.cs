using UnityEngine;
using UnityEngine.Video;
using System.Collections.Generic;

public class Video360Manager : MonoBehaviour
{
    [Header("Video Player 360")]
    [SerializeField] private VideoPlayer videoPlayer;    

    [Header("Lista de Videos")]
    [SerializeField] private List<VideoClip> videoClips = new List<VideoClip>();

    private int currentVideoIndex = 0;

    private void Start()
    {
        // Si no se asignó en el inspector, intentamos buscar un VideoPlayer
        if (videoPlayer == null)
        {
            videoPlayer = GetComponent<VideoPlayer>();
        }

        // Comprobamos que haya clips en la lista
        if (videoClips.Count > 0 && videoPlayer != null)
        {
            // Suscribimos al evento que se dispara cuando el video termina
            videoPlayer.loopPointReached += OnVideoEnd;

            // Reproducimos el primer video al iniciar
            currentVideoIndex = 0;
            videoPlayer.clip = videoClips[currentVideoIndex];
            videoPlayer.Play();
        }
        else
        {
            Debug.LogWarning("No hay VideoPlayer o lista de videos vacía.");
        }
    }

    /// <summary>
    /// Método que se llama automáticamente cuando un video se termina (loopPointReached).
    /// </summary>
    private void OnVideoEnd(VideoPlayer vp)
    {
        // Saltamos al siguiente video cuando termina el actual
        NextVideo();
    }

    /// <summary>
    /// Reproduce el siguiente video de la lista, y si llegamos al final, reinicia desde el principio.
    /// </summary>
    private void NextVideo()
    {
        currentVideoIndex++;
        if (currentVideoIndex >= videoClips.Count)
        {
            currentVideoIndex = 0; // Repite desde el primero
        }

        videoPlayer.clip = videoClips[currentVideoIndex];
        videoPlayer.Play();
    }

    /// <summary>
    /// Función pública para reproducir el video del índice especificado.
    /// El primer video corresponde al índice 0, el segundo al 1, etc.
    /// </summary>
    public void PlayVideoByIndex(int index)
    {
        if (index < 0 || index >= videoClips.Count)
        {
            Debug.LogWarning("Índice fuera de rango. No se reproducirá ningún video.");
            return;
        }

        currentVideoIndex = index;
        videoPlayer.clip = videoClips[currentVideoIndex];
        videoPlayer.Play();
    }
    public void VideoSkipTime(int time )
    {
        videoPlayer.time = time;
    }
}
