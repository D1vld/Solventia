using UnityEngine;
using UnityEngine.UI;

public class ScrollbarController : MonoBehaviour
{
    // Referencia al componente Scrollbar
    public Scrollbar scrollbar;

    // Valores de incremento y decremento
    public float incrementValue = 0.01f;  // El valor por defecto es 0.01
    public float decrementValue = 0.01f;  // El valor por defecto es 0.01

    // Función para aumentar el valor
    [ContextMenu("Increase Value")]
    public void IncreaseValue()
    {
        scrollbar.value = Mathf.Clamp(scrollbar.value + incrementValue, 0f, 1f); // Asegura que el valor esté entre 0 y 1
    }

    // Función para disminuir el valor
    [ContextMenu("Decrease Value")]
    public void DecreaseValue()
    {
        scrollbar.value = Mathf.Clamp(scrollbar.value - decrementValue, 0f, 1f); // Asegura que el valor esté entre 0 y 1
    }
}
