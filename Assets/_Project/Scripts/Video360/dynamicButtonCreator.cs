using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DynamicButtonCreator : MonoBehaviour
{
    [SerializeField] private MediaStorage mediaStorage;       // Referencia al MediaStorage
    [SerializeField] private GameObject buttonPrefab;         // Prefab del toggle (que también contiene los elementos de mensaje)
    [SerializeField] private Transform buttonParent;          // Contenedor para los toggles

    private List<MediaStorage.NamedImage> images;
    private List<UnityEngine.Video.VideoClip> videos;

    private void Start()
    {
        // Verificar referencias iniciales
        if (mediaStorage == null || buttonPrefab == null || buttonParent == null)
        {
            Debug.LogError("Asegúrate de asignar MediaStorage, el prefab del toggle y el contenedor de toggles.");
            return;
        }

        // Obtener todas las imágenes y videos del MediaStorage
        images = mediaStorage.GetAllImages();
        videos = mediaStorage.GetAllVideos();

        if (images == null || images.Count == 0)
        {
            Debug.LogWarning("No hay imágenes en el MediaStorage.");
            return;
        }

        // Crear un botón por cada imagen
        for (int i = 0; i < images.Count; i++)
        {
            CreateToggle(images[i], i);
        }
    }

    private void CreateToggle(MediaStorage.NamedImage namedImage, int index)
    {
        // Instanciar el prefab del toggle
        GameObject newToggle = Instantiate(buttonPrefab, buttonParent);

        // Acceder al Image en el tercer GameObject desde el final
        Transform hotelImgTransform = newToggle.transform.Find("Bocadillo/Contenido/Hotel_Img");
        Image toggleImage = hotelImgTransform != null ? hotelImgTransform.GetComponent<Image>() : null;
        if (toggleImage != null)
        {
            toggleImage.sprite = namedImage.image; // Asigna la imagen del MediaStorage
        }
        else
        {
            Debug.LogError("No se encontró 'Hotel_Img' o no tiene un componente Image en el prefab.");
        }

        // Acceder al TextMeshPro en el último GameObject
        Transform descripcionTransform = newToggle.transform.Find("Bocadillo/Contenido/Panel_descrip/Descripcion");
        TextMeshProUGUI toggleText = descripcionTransform != null ? descripcionTransform.GetComponent<TextMeshProUGUI>() : null;
        if (toggleText != null)
        {
            toggleText.text = namedImage.name; // Asigna el nombre de la imagen como texto
        }
        else
        {
            Debug.LogError("No se encontró 'Descripcion' o no tiene un componente TextMeshProUGUI en el prefab.");
        }

        // Acceder al TextMeshPro del tiempo dentro del prefab
        Transform tiempoTransform = newToggle.transform.Find("Bocadillo/Contenido/Panel_descrip/Tiempo");
        TextMeshProUGUI tiempoTextPrefab = tiempoTransform != null ? tiempoTransform.GetComponent<TextMeshProUGUI>() : null;
        if (tiempoTextPrefab != null)
        {
            // Mostrar la duración del video correspondiente si existe
            UpdateVideoTime(tiempoTextPrefab, index);
        }
        else
        {
            Debug.LogError("No se encontró 'Tiempo' o no tiene un componente TextMeshProUGUI en el prefab.");
        }
    }

    private void UpdateVideoTime(TextMeshProUGUI tiempoTextPrefab, int index)
    {
        if (videos != null && index < videos.Count && videos[index] != null)
        {
            // Obtener la duración del video
            double duration = videos[index].length;

            // Convertir la duración a formato minutos:segundos
            int minutes = Mathf.FloorToInt((float)(duration / 60));
            int seconds = Mathf.FloorToInt((float)(duration % 60));

            // Mostrar la duración en el TextMeshPro del botón
            tiempoTextPrefab.text = $"Duración: {minutes:D2}:{seconds:D2}";
        }
        else
        {
            // Si no hay un video asociado, dejar el texto vacío
            tiempoTextPrefab.text = "Sin video asociado";
        }
    }
}
