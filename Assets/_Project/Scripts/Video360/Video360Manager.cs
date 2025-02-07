using UnityEngine;
using UnityEngine.Video;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class Video360Manager : MonoBehaviour
{
    [Header("Video Player 360")]
    [SerializeField] private VideoPlayer videoPlayer;    

    [Header("Lista de Videos")]
    [SerializeField] private List<VideoClip> videoClips = new List<VideoClip>();

    [Header("GameObjects generales")]
    [SerializeField] private GameObject gameObjectToActivate;
    [SerializeField] private GameObject gameObjectToDesactivate;

    [Header("Paneles Interactables (Duplicados)")]
    [SerializeField] private GameObject panelInteractableES; // Panel en Español
    [SerializeField] private GameObject panelInteractableEN; // Panel en Inglés

    [Header("Menús Bajo (Duplicados)")]
    [SerializeField] private GameObject menuBajoES; // Menú en Español
    [SerializeField] private GameObject menuBajoEN; // Menú en Inglés

    [Header("Material del Skybox (Video 360)")]
    [SerializeField] private Material materialSkyboxVideo;

    [Header("Text Mesh Pro")]
    [SerializeField] private TextMeshProUGUI textMeshPro;

    [Header("Idioma")]
    [Tooltip("true = Español, false = Inglés")]
    [SerializeField] private bool isSpanish = true;

    private int currentVideoIndex = 0;

    private void Start()
    {
        if (videoPlayer == null)
            videoPlayer = GetComponent<VideoPlayer>();

        if (videoClips.Count > 0 && videoPlayer != null)
        {
            // Reproducimos el primer video al iniciar
            currentVideoIndex = 0;
            videoPlayer.clip = videoClips[currentVideoIndex];
            // videoPlayer.Play();

            // Asegurarnos de poner inicialmente el color que desees (por ejemplo #808080)
            if (materialSkyboxVideo != null)
            {
                SetSkyboxTintColor("#808080");
            }
        }
        else
        {
            Debug.LogWarning("No hay VideoPlayer o la lista de videos está vacía.");
        }
    }

    private void OnVideoEnd(VideoPlayer vp)
    {
        if (gameObjectToActivate != null)
            gameObjectToActivate.SetActive(true);

        if (gameObjectToDesactivate != null)
            gameObjectToDesactivate.SetActive(false);

        // Si deseas que, al terminar el video, inicie el siguiente automáticamente, descomenta:
        // NextVideo();
    }

    private void NextVideo()
    {
        currentVideoIndex++;
        if (currentVideoIndex >= videoClips.Count)
        {
            currentVideoIndex = 0;
        }
        videoPlayer.clip = videoClips[currentVideoIndex];
        videoPlayer.Play();
    }

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

    /// <summary>
    /// Reanudar la reproducción del video y regresar el tint a un gris #808080.
    /// Mostrando el menú de acuerdo al idioma seleccionado.
    /// </summary>
    public void ResumeVideo()
    {
        if (videoPlayer != null)
        {
            videoPlayer.Play();
        }
        
        // Desactivamos ambos paneles para asegurarnos de que no se vean.
        panelInteractableES.SetActive(false);
        panelInteractableEN.SetActive(false);

        // Activamos el menú correspondiente al idioma seleccionado.
        if (isSpanish)
        {
            menuBajoES.SetActive(true);
            menuBajoEN.SetActive(false);
        }
        else
        {
            menuBajoES.SetActive(false);
            menuBajoEN.SetActive(true);
        }

        // Cambiamos el tint del Skybox de regreso a #808080
        SetSkyboxTintColor("#808080");
    }

    /// <summary>
    /// Detener el video y poner la vista en negro.
    /// Muestra el panel interactable según el idioma.
    /// </summary>
    public void StopVideo()
    {
        if (videoPlayer != null)
        {
            videoPlayer.Pause();
        }

        textMeshPro.gameObject.SetActive(false);

        // Desactivamos ambos menús para asegurarnos de que no se vean.
        menuBajoES.SetActive(false);
        menuBajoEN.SetActive(false);

        // Activamos el panel correspondiente al idioma seleccionado.
        if (isSpanish)
        {
            panelInteractableES.SetActive(true);
            panelInteractableEN.SetActive(false);
        }
        else
        {
            panelInteractableES.SetActive(false);
            panelInteractableEN.SetActive(true);
        }

        // Cambiamos el tint del Skybox a negro
        SetSkyboxTintColor("#000000");
    }

    /// <summary>
    /// Cambiar el tint color del material del Skybox a partir de un valor Hex (#RRGGBB).
    /// </summary>
    private void SetSkyboxTintColor(string hexColor)
    {
        if (materialSkyboxVideo == null) return;

        Color color;
        if (ColorUtility.TryParseHtmlString(hexColor, out color))
        {
            // Asegúrate de usar el nombre de propiedad correcto según tu Shader.
            // Con un Skybox/Panoramic o Skybox/6 Sided suele ser "_Tint" o "_TintColor"
            materialSkyboxVideo.SetColor("_Tint", color);
        }
        else
        {
            Debug.LogWarning("No se ha podido parsear el color con el código: " + hexColor);
        }
    }

    public void VideoSkipTime(int time)
    {
        videoPlayer.time = time;
    }

    public void StopVideoAfterSeconds(float seconds)
    {
        StartCoroutine(StopAfterSecondsCoroutine(seconds));
    }

    private IEnumerator StopAfterSecondsCoroutine(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        // Detenemos el video
        StopVideo();

        Debug.Log("Se ha detenido el video tras " + seconds + " segundos, mostrando fondo negro.");
    }

    /// <summary>
    /// Muestra en pantalla el texto que se pase por parámetro.
    /// </summary>
    /// <param name="texto">Texto que deseas mostrar en el TextMeshPro.</param>
    public void MostrarTextoEnTMP(string texto)
    {
        if (textMeshPro == null)
        {
            Debug.LogWarning("TextMeshPro no está asignado en el inspector.");
            return;
        }

        textMeshPro.text = texto;

        // Si está desactivado, lo activamos para que sea visible
        if (!textMeshPro.gameObject.activeSelf)
        {
            textMeshPro.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Togglea el idioma entre Español e Inglés.
    /// </summary>
    public void ToggleLanguage()
    {
        // Cambia de true a false, o viceversa
        isSpanish = !isSpanish;
        
        // Puedes incluir aquí cualquier lógica adicional que desees
        // cuando se cambie el idioma automáticamente.
        Debug.Log("Idioma cambiado. Ahora isSpanish = " + isSpanish);
    }

    /// <summary>
    /// Establece directamente el valor de isSpanish.
    /// </summary>
    public void SetLanguage(bool value)
    {
        isSpanish = value;
        Debug.Log("Idioma establecido directamente a isSpanish = " + value);
    }
}
