using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonClickHandler : MonoBehaviour
{
    private bool isSelected = false;

    // Arrays de GameObjects para activar y desactivar
    public GameObject[] objectsToActivate;
    public GameObject[] objectsToDeactivate;

    // Referencia a otros ButtonClickHandlers para resetear sus estados
    public ButtonClickHandler[] otherButtons;

    // Referencia al PassthroughOpacityModifier
    public bool usePassthrough = true; // Booleano para usar o no la opacidad del Passthrough
    [ConditionalHide("usePassthrough", true)]
    public PassthroughOpacityController passthroughOpacityController;

    // Referencia al ContentSwitch para alternar el contenido
    public bool useContentSwitch = true; // Booleano para usar o no el cambio de contenido
    [ConditionalHide("useContentSwitch", true)]
    public ContentSwitch contentSwitch;

    public float targetOpacitySelected = 0.8f; // Opacidad cuando está seleccionado
    public float targetOpacityDeselected = 0.3f; // Opacidad cuando está deseleccionado

    public void OnButtonClick()
    {
        // Primero, asegurarse de que todos los demás botones vuelvan a su estado normal
        ResetOtherButtons();

        // Ahora que todo está reseteado, proceder con la acción del botón actual
        if (isSelected)
        {
            // Acción cuando se deselecciona: Desactivar objetos a activar y activar objetos a desactivar
            foreach (GameObject obj in objectsToActivate)
            {
                if (obj != null)
                {
                    obj.SetActive(false);  // Desactiva los objetos del array de activación
                }
            }

            foreach (GameObject obj in objectsToDeactivate)
            {
                if (obj != null)
                {
                    obj.SetActive(true);  // Activa los objetos del array de desactivación
                }
            }

            // Cambiar la opacidad del Passthrough al valor deseleccionado solo si está activado el uso del passthrough
            if (usePassthrough && passthroughOpacityController != null)
            {
                passthroughOpacityController.ChangeOpacity(targetOpacityDeselected);
            }

            // Cambiar el contenido si está asignado y está activado el uso del ContentSwitch
            if (useContentSwitch && contentSwitch != null)
            {
                contentSwitch.SwitchContent(); // Alterna el contenido
            }

            Debug.Log("Deseleccionado: Objetos activados y opacidad modificada");
        }
        else
        {
            // Acción cuando se selecciona: Activar objetos a activar y desactivar objetos a desactivar
            foreach (GameObject obj in objectsToActivate)
            {
                if (obj != null)
                {
                    obj.SetActive(true);  // Activa los objetos del array de activación
                }
            }

            foreach (GameObject obj in objectsToDeactivate)
            {
                if (obj != null)
                {
                    obj.SetActive(false);  // Desactiva los objetos del array de desactivación
                }
            }

            // Cambiar la opacidad del Passthrough al valor seleccionado solo si está activado el uso del passthrough
            if (usePassthrough && passthroughOpacityController != null)
            {
                passthroughOpacityController.ChangeOpacity(targetOpacitySelected);
            }

            // Cambiar el contenido si está asignado y está activado el uso del ContentSwitch
            if (useContentSwitch && contentSwitch != null)
            {
                contentSwitch.SwitchContent(); // Alterna el contenido
            }

            Debug.Log("Seleccionado: Objetos activados y opacidad modificada");
        }

        // Alternar el estado para la próxima vez que se haga clic
        isSelected = !isSelected;
    }

    // Método para resetear otros botones usando su propio flip-flop
    private void ResetOtherButtons()
    {
        foreach (ButtonClickHandler button in otherButtons)
        {
            if (button != null && button != this)
            {
                button.EnsureNormalState();
            }
        }
    }

    // Método que asegura que este botón esté en su estado inicial (no seleccionado)
    public void EnsureNormalState()
    {
        if (isSelected)
        {
            // Si está seleccionado, activar el flip-flop para resetearlo
            OnButtonClick();
        }
    }
}
