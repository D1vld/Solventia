using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialImageSelector : MonoBehaviour
{
    // Referencia al material que se actualizará
    public Material targetMaterial;

    // Variable pública para almacenar la textura inicial desde otro script
    public Texture2D initialTexture;

    // Referencia al script que contiene el array de imágenes
    public SpawnPrefabsWithEvents spawnPrefabsWithEvents;

    void Start()
    {
        // Aplicar la textura inicial al iniciar el script si está asignada
        if (targetMaterial != null && initialTexture != null)
        {
            targetMaterial.SetTexture("_MainTex", initialTexture);
        }
        else
        {
            Debug.LogError("Target material or initial texture is null.");
        }

        // Asignar la primera imagen del array de imágenes (posición 0)
        SetInitialImageFromArray(0);
    }

    // Método que será llamado al presionar un botón
    public void OnButtonSelect(Texture2D selectedTexture)
    {
        if (targetMaterial != null && selectedTexture != null)
        {
            targetMaterial.SetTexture("_MainTex", selectedTexture);
        }
        else
        {
            Debug.LogError("Target material or selected texture is null.");
        }
    }

    // Método para asignar la primera imagen del array al material
    void SetInitialImageFromArray(int index)
    {
        if (true)
    {   
        Sprite sprite = spawnPrefabsWithEvents.imagenes[index];  // Asegúrate que Imagenes está con la "I" mayúscula si es necesario.
        if (sprite != null)
        {
            Texture2D texture = SpriteToTexture2D(sprite);
            OnButtonSelect(texture);
        }
        else
        {
            Debug.LogError("Sprite at index " + index + " is null.");
        }
    }
    else
    {
        Debug.LogError("spawnPrefabsWithEvents or Imagenes array is null/empty.");
    }
    }

    // Método para convertir un Sprite en una Texture2D
    Texture2D SpriteToTexture2D(Sprite sprite)
    {
        if (sprite.rect.width != sprite.texture.width)
        {
            // Crear una nueva Texture2D si el sprite no cubre toda la textura
            Texture2D newText = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
            Color[] newColors = sprite.texture.GetPixels((int)sprite.textureRect.x,
                                                         (int)sprite.textureRect.y,
                                                         (int)sprite.textureRect.width,
                                                         (int)sprite.textureRect.height);
            newText.SetPixels(newColors);
            newText.Apply();
            return newText;
        }
        else
        {
            // Si el sprite ocupa toda la textura, usar la textura original
            return sprite.texture;
        }
    }
}
