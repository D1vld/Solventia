using UnityEngine;

/// <summary>
/// Interfaz para la reproducción de video 360.
/// </summary>
public interface IVideoPlaybackService
{
    /// <summary>
    /// Inicializa el componente de video.
    /// </summary>
    void Initialize();

    /// <summary>
    /// Devuelve si el video está en reproducción.
    /// </summary>
    bool IsVideoPlaying();

    /// <summary>
    /// Nombre del video actual en reproducción.
    /// </summary>
    string GetCurrentVideoName();

    /// <summary>
    /// Tiempo actual de reproducción en segundos.
    /// </summary>
    float GetCurrentPlaybackTime();

    /// <summary>
    /// Inicia la reproducción del video actual.
    /// </summary>
    void PlayCurrentVideo();

    /// <summary>
    /// Reproduce el video en curso y actualiza el Skybox si aplica.
    /// </summary>
    void Play();

    /// <summary>
    /// Pausa el video.
    /// </summary>
    void Pause();
}