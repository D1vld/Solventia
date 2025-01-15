using UnityEngine;

public class PlaySoundOnClick : MonoBehaviour
{
    public AudioSource audioSource; // Referencia al AudioSource donde se reproducirá el sonido
    public AudioClip clickSound;    // El clip de sonido que quieres reproducir

    // Asegúrate de que este método sea público para que Unity lo detecte en el Inspector
    public void PlaySound()
    {
        // Verificamos que el AudioSource y el sonido estén asignados
        if (audioSource != null && clickSound != null)
        {
            audioSource.PlayOneShot(clickSound); // Reproduce el sonido
        }
        else
        {
            Debug.LogWarning("AudioSource o ClickSound no están asignados.");
        }
    }
}
