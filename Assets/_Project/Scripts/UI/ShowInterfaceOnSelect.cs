using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowInterfaceOnSelects : MonoBehaviour
{
    public GameObject interfaceToShow; // La interfaz que quieres mostrar

    public void ShowInterface()
    {
        if (interfaceToShow != null)
        {
            interfaceToShow.SetActive(true);
        }
    }
}
