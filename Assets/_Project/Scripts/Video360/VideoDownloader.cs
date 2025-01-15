using UnityEngine;
using UnityEngine.Video;
using UnityEngine.Networking;
using System.Collections;
using System.IO;
using TMPro;

public class VideoDownloader : MonoBehaviour
{
    public TextMeshProUGUI downloadText; // Asigna este en el inspector
    public VideoPlayer videoPlayer; // Asigna el VideoPlayer en el inspector
    // Lista de URLs de los videos que quieres descargar
    public string[] videoUrls = new string[]
    {
        "https://example.com/video1.mp4",
        "https://example.com/video2.mp4",
        "https://example.com/video3.mp4"
    };

    private string[] filePaths; // Lista para almacenar las rutas de los videos descargados
    private int currentVideoIndex = 0; // Índice para llevar el seguimiento del video que se está reproduciendo

    private void Start()
    {
        filePaths = new string[videoUrls.Length];
        videoPlayer.loopPointReached += OnVideoEnd; // Registramos el evento cuando el video termina
        StartCoroutine(DownloadAndPlayVideos());
    }

    // Corutina para descargar y reproducir videos uno por uno
    private IEnumerator DownloadAndPlayVideos()
    {
        // Mientras haya videos por descargar y reproducir
        while (currentVideoIndex < videoUrls.Length)
        {
            // Verificamos si el video ya está descargado
            string filePath = Path.Combine(Application.persistentDataPath, Path.GetFileName(videoUrls[currentVideoIndex]));
            if (File.Exists(filePath))
            {
                // Si el archivo ya existe, solo guardamos la ruta y pasamos al siguiente video
                filePaths[currentVideoIndex] = filePath;
                Debug.Log($"El video {Path.GetFileName(videoUrls[currentVideoIndex])} ya está descargado.");
            }
            else
            {
                // Si no está descargado, lo descargamos
                yield return StartCoroutine(DownloadVideo(videoUrls[currentVideoIndex], currentVideoIndex));
            }

            // Reproducimos el video una vez descargado
            StreamVideo();

            // Lanzamos una corutina para seguir descargando el siguiente video mientras se reproduce el actual
            if (currentVideoIndex + 1 < videoUrls.Length)
            {
                StartCoroutine(DownloadVideo(videoUrls[currentVideoIndex + 1], currentVideoIndex + 1));
            }

            // Esperamos hasta que termine el video antes de proceder al siguiente
            yield return new WaitUntil(() => !videoPlayer.isPlaying);
        }

        // Si ya no hay más videos para reproducir, mostramos un mensaje
        downloadText.gameObject.SetActive(true);
        downloadText.text = "Todos los videos se han reproducido.";
        Debug.Log("Todos los videos se han reproducido.");
    }

    // Corutina para descargar un solo video con progreso
    private IEnumerator DownloadVideo(string url, int index)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            // Deshabilitamos la verificación SSL (solo para pruebas)
            webRequest.certificateHandler = new BypassCertificateHandler();

            // Enviar la solicitud de descarga
            webRequest.SendWebRequest();

            // Mientras la solicitud esté activa, se mostrará el progreso
            while (!webRequest.isDone)
            {
                // Calculamos el progreso de la descarga
                float progress = webRequest.downloadProgress;

                // Mostrar el progreso en la UI
                downloadText.text = $"Descargando: {progress * 100:0.0}%";

                // Esperamos un frame antes de continuar el ciclo
                yield return null;
            }

            // Verificamos si la descarga fue exitosa
            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error al descargar el video: " + webRequest.error);
            }
            else
            {
                // Obtener el archivo descargado como bytes
                byte[] videoData = webRequest.downloadHandler.data;

                // Guardar el video en un archivo en el sistema local
                string filePath = Path.Combine(Application.persistentDataPath, Path.GetFileName(url));
                File.WriteAllBytes(filePath, videoData);
                filePaths[index] = filePath; // Guardamos la ruta del archivo descargado
                Debug.Log($"Video guardado en: {filePath}");
            }
        }
    }

    // Función para comenzar a transmitir el video
    private void StreamVideo()
    {
        // Verificamos si hay videos descargados y comenzamos con el primero
        if (currentVideoIndex < filePaths.Length && !string.IsNullOrEmpty(filePaths[currentVideoIndex]))
        {
            // Ocultar el TextMeshPro mientras se reproduce el video
            downloadText.gameObject.SetActive(false);

            // Reproducir el video
            videoPlayer.source = VideoSource.Url;
            videoPlayer.url = "file://" + filePaths[currentVideoIndex];
            videoPlayer.Play();
        }
        else
        {
            Debug.LogError("No hay videos descargados para reproducir.");
        }
    }

    // Método que se llama cuando un video ha terminado de reproducirse
    private void OnVideoEnd(VideoPlayer vp)
    {
        // Incrementamos el índice para descargar y reproducir el siguiente video
        currentVideoIndex++;

        // Verificamos si hay más videos para reproducir
        if (currentVideoIndex < filePaths.Length)
        {
            // Reproducir el siguiente video
            StreamVideo();
        }
        else
        {
            // Si ya no hay más videos, mostrar el TextMeshPro y dar por finalizado el proceso
            downloadText.gameObject.SetActive(true);
            downloadText.text = "Todos los videos se han reproducido.";
            Debug.Log("Todos los videos se han reproducido.");
        }
    }

    // Clase para omitir la validación de certificados SSL (solo para pruebas)
    public class BypassCertificateHandler : CertificateHandler
    {
        protected override bool ValidateCertificate(byte[] certificateData)
        {
            // Siempre devuelve true para omitir la validación del certificado
            return true;
        }
    }
}
