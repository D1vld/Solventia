using UnityEngine;

public class DynamicSpacing : MonoBehaviour
{
    public RectTransform container;
    public float verticalSpacing = 10f; // Espaciado vertical entre prefabs

    void Start()
    {
        AdjustSpacing();
    }

    public void AdjustSpacing()
    {
        RectTransform[] prefabs = container.GetComponentsInChildren<RectTransform>();

        float currentYPosition = 0f;

        foreach (RectTransform prefab in prefabs)
        {
            if (prefab.gameObject == container.gameObject) continue; // Saltar el contenedor mismo

            // Ajustar posición del prefab
            prefab.anchoredPosition = new Vector2(prefab.anchoredPosition.x, currentYPosition);

            // Actualizar la posición para el próximo prefab
            currentYPosition -= (prefab.rect.height + verticalSpacing);
        }

        // Ajustar el tamaño del contenedor
        container.sizeDelta = new Vector2(container.sizeDelta.x, -currentYPosition);
    }

    // Método para ser llamado cuando se añaden nuevos prefabs dinámicamente
    public void AddPrefab(RectTransform prefab)
    {
        prefab.SetParent(container, false);
        AdjustSpacing();
    }
}
