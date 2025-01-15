using UnityEngine;
using UnityEngine.UI;

public class MaterialCube : MonoBehaviour
{
    // Arrays para los sprites, cubemaps y textos
    public Sprite[] spriteArray;
    public Cubemap[] cubemapArray;
    public string[] textArray;

    // GameObject cuyo material se va a modificar
    public GameObject targetObject;

    // Contenedor de botones que se asignará desde el inspector
    public Transform buttonContainer;

    // Prefab de botón que se asignará desde el inspector
    public Button buttonPrefab;

    private void Start()
    {
        // Validar que los arrays tengan la misma longitud
        if (spriteArray.Length != cubemapArray.Length || spriteArray.Length != textArray.Length)
        {
            Debug.LogError("Los arrays de sprites, cubemaps y textos deben tener la misma longitud.");
            return;
        }

        // Crear un botón por cada sprite en el array
        for (int i = 0; i < spriteArray.Length; i++)
        {
            // Instanciar un nuevo botón a partir del prefab y colocarlo en el contenedor
            Button newButton = Instantiate(buttonPrefab, buttonContainer);

            // Asignar la imagen del sprite al botón
            Image buttonImage = newButton.GetComponentInChildren<Image>();
            if (buttonImage != null)
            {
                buttonImage.sprite = spriteArray[i];
            }

            // Asignar el texto correspondiente al botón
            Text buttonText = newButton.GetComponentInChildren<Text>();
            if (buttonText != null)
            {
                buttonText.text = textArray[i];
            }

            // Añadir evento al botón para que cambie la textura del material cuando sea presionado
            int index = i;  // Capturar el índice para utilizarlo en el closure
            newButton.onClick.AddListener(() => OnButtonClicked(index));
        }
    }

    // Método que se ejecuta cuando un botón es presionado
    private void OnButtonClicked(int index)
    {
        if (targetObject != null)
        {
            // Obtener el material del objeto
            Material objectMaterial = targetObject.GetComponent<Renderer>().material;

            if (objectMaterial != null)
            {
                // Cambiar la textura principal del material por el cubemap correspondiente
                objectMaterial.SetTexture("_MainTex", cubemapArray[index]);
            }
        }
        else
        {
            Debug.LogError("El objeto objetivo no está asignado.");
        }
    }
}
