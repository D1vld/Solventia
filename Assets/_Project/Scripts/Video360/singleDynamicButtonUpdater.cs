using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SingleDynamicButtonUpdater : MonoBehaviour
{
    [SerializeField] private MediaStorage mediaStorage;    // Referencia al MediaStorage
    [SerializeField] private GameObject buttonPrefab;      // Prefab del botón único
    [SerializeField] private Transform buttonParent;       // Contenedor para el botón
    [SerializeField] private string projectedImageName;    // Nombre de la imagen proyectada (para buscar en MediaStorage)

    private GameObject singleButton;
    private Image buttonImage;
    private TextMeshProUGUI buttonText;

    private void Start()
    {
        if (mediaStorage == null || buttonPrefab == null || buttonParent == null)
        {
            Debug.LogWarning("Asegúrate de asignar MediaStorage, el prefab del botón y el contenedor en el inspector.");
            return;
        }

        // Crear el único botón
        CreateSingleButton();

        // Actualizar la imagen proyectada inicialmente
        UpdateButtonContent();
    }

    private void CreateSingleButton()
{
    // Instanciar el prefab del botón
    singleButton = Instantiate(buttonPrefab, buttonParent);

    // Buscar los componentes dentro del prefab del botón
    Transform imageTransform = singleButton.transform.Find("Bocadillo/Contenido/Hotel_Img");
    if (imageTransform != null)
    {
        buttonImage = imageTransform.GetComponent<Image>();
        if (buttonImage == null)
        {
            Debug.LogError("El objeto 'Hotel_Img' existe, pero no tiene un componente Image.");
        }
    }
    else
    {
        Debug.LogError("No se encontró 'Hotel_Img' en el prefab del botón.");
    }

    Transform textTransform = singleButton.transform.Find("Bocadillo/Contenido/Panel_descrip/Descripcion");
    if (textTransform != null)
    {
        buttonText = textTransform.GetComponent<TextMeshProUGUI>();
        if (buttonText == null)
        {
            Debug.LogError("El objeto 'Descripcion' existe, pero no tiene un componente TextMeshProUGUI.");
        }
    }
    else
    {
        Debug.LogError("No se encontró 'Descripcion' en el prefab del botón.");
    }

    /*// Configurar el evento del botón para cambiar la imagen proyectada
    Button buttonComponent = singleButton.GetComponent<Button>();
    if (buttonComponent != null)
    {
        buttonComponent.onClick.AddListener(() => UpdateButtonContent());
    }
    else
    {
        Debug.LogError("El prefab del botón no tiene un componente Button.");
    }*/
}

private void UpdateButtonContent()
{
    // Obtener todas las imágenes del MediaStorage
    var allImages = mediaStorage.GetAllImages();

    // Buscar la imagen proyectada en MediaStorage
    MediaStorage.NamedImage projectedImage = allImages.Find(image => image.name == projectedImageName);

    // Si no se encuentra la imagen proyectada, usar la primera imagen como predeterminada
    if (projectedImage == null && allImages.Count > 0)
    {
        projectedImage = allImages[0];
        Debug.LogWarning($"No se encontró la imagen proyectada con nombre '{projectedImageName}'. Usando la primera imagen por defecto: {projectedImage.name}");
    }

    // Actualizar el sprite y el texto solo si se encontró una imagen válida
    if (projectedImage != null)
    {
        if (buttonImage != null)
        {
            buttonImage.sprite = projectedImage.image;
            Debug.Log($"Imagen actualizada a {projectedImage.image.name}");
        }

        if (buttonText != null)
        {
            buttonText.text = projectedImage.name;
            Debug.Log($"Texto actualizado a {projectedImage.name}");
        }
    }
    else
    {
        Debug.LogWarning("No hay imágenes disponibles en MediaStorage para usar como predeterminadas.");
    }
}



    // Método para cambiar dinámicamente el nombre de la imagen proyectada
    public void SetProjectedImageName(string newImageName)
    {
        projectedImageName = newImageName;
        UpdateButtonContent();
    }
}
