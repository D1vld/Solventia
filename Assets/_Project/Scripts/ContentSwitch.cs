using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContentSwitch : MonoBehaviour
{
    // Variables para los contenidos
    public GameObject Content;    
    public GameObject Content1;

    // Función para hacer el switch entre Content y Content1
    public void SwitchContent()
    {
        if (Content.activeSelf)
        {
            // Si Content está activo, lo desactiva y activa Content1
            Content.SetActive(false);
            Content1.SetActive(true);
        }
        else
        {
            // Si Content1 está activo, lo desactiva y activa Content
            Content1.SetActive(false);
            Content.SetActive(true);
        }
    }
}
