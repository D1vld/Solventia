using UnityEngine;
using UnityEngine.UI;

public class ImageSelector : MonoBehaviour
{
    // Referencia a la imagen principal que se actualizará
    public Image mainImage;

    // Método que será llamado al presionar un botón
    public void OnButtonSelect(Sprite selectedSprite)
    {
        if (mainImage != null && selectedSprite != null)
        {
            mainImage.sprite = selectedSprite;
        }
        else
        {
            Debug.LogError("Main image or selected sprite is null.");
        }
    }

    // Método para configurar la imagen predeterminada desde otra clase
    public void SetDefaultImage(Sprite defaultSprite)
    {
        if (mainImage != null && defaultSprite != null)
        {
            mainImage.sprite = defaultSprite; // Configurar la imagen predeterminada
        }
        else
        {
            Debug.LogError("Main image or default sprite is null.");
        }
    }
}
