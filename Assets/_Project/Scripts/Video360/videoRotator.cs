using System.Collections;
using UnityEngine;
using UnityEngine.Video;

public class VideoRotator : MonoBehaviour
{
    // Objeto VideoPlayer en el que se reproducirán los videos
    public VideoPlayer targetVideoPlayer;

    // Referencia al script MediaStorage
    public MediaStorage mediaStorage;

    // Tiempo entre cada cambio de video en segundos
    private float changeInterval = 5f;

    private int currentVideoIndex = 0;
    private VideoClip[] videos;
    private Coroutine videoRotationCoroutine;
    private bool isPaused = false;

    private void Awake()
    {
        // Buscar y asignar el VideoPlayer por tag
        /*GameObject videoPlayerObject = GameObject.FindWithTag("VideoPlayer");
        if (videoPlayerObject != null)
        {
            targetVideoPlayer = videoPlayerObject.GetComponent<VideoPlayer>();
        }
        else
        {
            Debug.LogWarning("No se encontró un objeto con el tag 'VideoPlayer'.");
        }

        // Buscar y asignar el MediaStorage por tag
        GameObject mediaStorageObject = GameObject.FindWithTag("mediaStorage");
        if (mediaStorageObject != null)
        {
            mediaStorage = mediaStorageObject.GetComponent<MediaStorage>();
        }
        else
        {
            Debug.LogWarning("No se encontró un objeto con el tag 'mediaStorage'.");
        }*/
    }

    private void Start()
    {
        if (mediaStorage == null || targetVideoPlayer == null)
        {
            Debug.LogWarning("MediaStorage o targetVideoPlayer no están asignados correctamente.");
            return;
        }

        // Obtiene todos los videos del MediaStorage
        videos = mediaStorage.GetAllVideos().ToArray();
        if (videos.Length == 0)
        {
            Debug.LogWarning("No hay videos en MediaStorage.");
            return;
        }

        // Iniciar la rotación automática de videos
        videoRotationCoroutine = StartCoroutine(RotateVideos());
    }

    private IEnumerator RotateVideos()
    {
        while (true)
        {
            ShowNextVideo();
            yield return new WaitForSeconds(changeInterval);
        }
    }

    public void ShowNextVideo()
    {
        currentVideoIndex = (currentVideoIndex + 1) % videos.Length;
        targetVideoPlayer.clip = videos[currentVideoIndex];
        targetVideoPlayer.Play();
    }

    public void ShowPreviousVideo()
    {
        currentVideoIndex = (currentVideoIndex - 1 + videos.Length) % videos.Length;
        targetVideoPlayer.clip = videos[currentVideoIndex];
        targetVideoPlayer.Play();
    }

    public void ShowVideoByIndex(int index)
    {
        if (index < 0 || index >= videos.Length)
        {
            Debug.LogWarning($"Índice {index} fuera de rango. Debe estar entre 0 y {videos.Length - 1}.");
            return;
        }

        currentVideoIndex = index;
        targetVideoPlayer.clip = videos[currentVideoIndex];
        targetVideoPlayer.Play();
    }
}
