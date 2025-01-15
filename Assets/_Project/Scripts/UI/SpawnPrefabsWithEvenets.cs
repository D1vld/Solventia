using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class SpawnPrefabsWithEvents : MonoBehaviour
{
    public GameObject buttonPrefab;
    public Transform parentTransform;
    public List<string> textos;
    public List<Sprite> imagenes;
    public List<Material> materiales;  // Nueva lista para manejar materiales
    public MaterialImageSelector materialImageSelector;
    public ImageSelector spriteImageSelector;  // Referencia para el script de imágenes
    public TextMeshProUGUI mainPlace;
    public TextMeshProUGUI hotelName;
    public string newHotelName;

    void Start()
    {
        if (hotelName != null && !string.IsNullOrEmpty(newHotelName))
        {
            hotelName.text = newHotelName;
        }

        // Asignar la primera imagen de la lista como predeterminada al ImageSelector
        if (imagenes.Count > 0 && spriteImageSelector != null)
        {
            spriteImageSelector.SetDefaultImage(imagenes[0]);  // Usar la primera imagen como predeterminada
        }

        // Verificar que las referencias están asignadas
        if (buttonPrefab != null && parentTransform != null && (materialImageSelector != null || spriteImageSelector != null) && mainPlace != null)
        {
            // Verificar que haya imágenes o materiales
            if (imagenes.Count > 0 || materiales.Count > 0)
            {
                for (int i = 0; i < Mathf.Max(textos.Count, imagenes.Count, materiales.Count); i++)
                {
                    GameObject newButton = Instantiate(buttonPrefab, parentTransform);
                    TextMeshProUGUI buttonText = newButton.GetComponentInChildren<TextMeshProUGUI>();
                    if (buttonText != null && i < textos.Count)
                    {
                        buttonText.text = textos[i];
                    }

                    // Asignar sprite o material en el botón
                    Image buttonImage = newButton.GetComponentInChildren<Image>();
                    if (buttonImage != null)
                    {
                        if (i < imagenes.Count && imagenes[i] != null)
                        {
                            buttonImage.sprite = imagenes[i];
                        }
                        else if (i < materiales.Count && materiales[i] != null)
                        {
                            // Opción para asignar material en lugar de imagen
                            buttonImage.material = materiales[i];
                        }
                    }

                    // Asignar evento OnClick
                    Button buttonComponent = newButton.GetComponent<Button>();
                    if (buttonComponent != null)
                    {
                        int index = i;
                        buttonComponent.onClick.AddListener(() => 
                        {
                            if (index < imagenes.Count && imagenes[index] != null)
                            {
                                // Usar el ImageSelector para manejar sprites
                                spriteImageSelector?.OnButtonSelect(imagenes[index]);
                            }
                            else if (index < materiales.Count && materiales[index] != null)
                            {
                                // Usar el MaterialImageSelector para manejar materiales
                                materialImageSelector?.OnButtonSelect(materiales[index].mainTexture as Texture2D);
                            }
                            mainPlace.text = textos[index];
                        });
                    }
                    else
                    {
                        Debug.LogError("Button component or image not found in the instantiated prefab.");
                    }
                }
            }
            else
            {
                Debug.LogError("No hay imágenes o materiales en las listas.");
            }
        }
        else
        {
            Debug.LogError("ButtonPrefab, ParentTransform, ImageSelector o MainText no están asignados en el inspector.");
        }
    }
}
