using UnityEngine;
using UnityEngine.Video;

public class VideoControl : MonoBehaviour
{
    public VideoPlayer videoPlayer; // Asocia tu VideoPlayer desde el Inspector
    public KeyCode skipKey = KeyCode.P; // Tecla para saltar al tiempo especificado
    public KeyCode speedUpKey = KeyCode.S; // Tecla para acelerar la reproducción (por ejemplo, tecla 'S')
    public float playbackSpeed = 1.4f; // Velocidad de reproducción, 1.4x

    void Update()
    {
        // Verifica si la tecla P está presionada para saltar al tiempo 21 segundos
        if (Input.GetKeyDown(skipKey))
        {
            videoPlayer.time = 40f;
        }

        // Verifica si la tecla S está presionada para cambiar la velocidad de reproducción
        if (Input.GetKeyDown(speedUpKey))
        {
            videoPlayer.playbackSpeed = playbackSpeed; // Establece la velocidad del video a 1.4x
        }
    }
}
