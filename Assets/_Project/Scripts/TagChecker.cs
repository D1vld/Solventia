using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class TagChecker : MonoBehaviour
{
    // Objeto que será activado/desactivado
    public GameObject[] objectsToActivate;
    public GameObject[] objectsToDesactivate;
    public GameObject objectToAnimation;
    public GameObject UIMe;


    public void CheckAndActivateObjects()
    {
        List<GameObject> tag180Objects = new List<GameObject>();
        List<GameObject> tag360Objects = new List<GameObject>();
        List<GameObject> animationObjects = new List<GameObject>();
        List<GameObject> UIObjects = new List<GameObject>();
        List<GameObject> tresDObjects = new List<GameObject>();
        List<GameObject> BigStoneObjects = new List<GameObject>();

        // Encuentra todos los objetos en la escena
        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();

        // Clasifica los objetos según su tag y añade a las listas
        foreach (GameObject obj in allObjects)
        {
            if (obj.CompareTag("180"))
            {
                tag180Objects.Add(obj);
            }
            else if (obj.CompareTag("360"))
            {
                tag360Objects.Add(obj);
            }
            else if (obj.CompareTag("animacion"))
            {
                animationObjects.Add(obj);
            }
            else if (obj.CompareTag("UI"))
            {
                UIObjects.Add(obj);
            }
            else if (obj.CompareTag("Chat"))
            {
                UIObjects.Add(obj);
            }
            else if (obj.CompareTag("Next"))
            {
                BigStoneObjects.Add(obj);
            }
            else if (obj.CompareTag("3D"))
            {
                tresDObjects.Add(obj);
            }
        }
        // Activa el gesto de la mano
        foreach (GameObject obj in BigStoneObjects)
        {
            obj.SetActive(true);
        }

        // Desactiva todos los objetos con el tag "360"
        foreach (GameObject obj in tag360Objects)
        {
            obj.SetActive(false);
        }
        // Desactiva todos los objetos con el tag "UI"
        foreach (GameObject obj in UIObjects)
        {
            obj.SetActive(false);
        }
        // Activa todos los objetos con el tag "180"
        foreach (GameObject obj in tag180Objects)
        {
            obj.SetActive(true);
        }

        // Ejecuta la función SetPanoramico en todos los objetos con el tag "animacion"
        foreach (GameObject obj in animationObjects)
        {
            boolAnimation boolAnim = obj.GetComponent<boolAnimation>();
            if (boolAnim != null)
            {
                boolAnim.SetPanoramico(true);
            }
        }
        // Desactiva todos los objetos con el tag "3D"
        foreach (GameObject obj in tresDObjects)
        {
            obj.SetActive(false);
        }
        foreach (GameObject obj in objectsToActivate)
        {
            if (obj != null)
            {
                obj.SetActive(true);
            }
        }

        foreach (GameObject obj in objectsToDesactivate)
        {
            if (obj != null)
            {
                obj.SetActive(false);
            }
        }

        if (UIMe != null)
        {
            UIMe.SetActive(true);
        }

        if (objectToAnimation != null)
        {
            objectToAnimation.GetComponent<boolAnimation>().SetPanoramico(false);
        }


    }

    // Función para desactivar el objeto especificado
    public void DeactivateObject()
    {

        List<GameObject> tag180Objects = new List<GameObject>();
        List<GameObject> tag360Objects = new List<GameObject>();
        List<GameObject> animationObjects = new List<GameObject>();
        List<GameObject> UIObjects = new List<GameObject>();
        List<GameObject> tresDObjects = new List<GameObject>();
        List<GameObject> BigStoneObjects = new List<GameObject>();

        // Encuentra todos los objetos en la escena
        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();

        // Clasifica los objetos según su tag y añade a las listas
        foreach (GameObject obj in allObjects)
        {
            if (obj.CompareTag("180"))
            {
                tag180Objects.Add(obj);
            }
            else if (obj.CompareTag("360"))
            {
                tag360Objects.Add(obj);
            }
            else if (obj.CompareTag("animacion"))
            {
                animationObjects.Add(obj);
            }
            else if (obj.CompareTag("UI"))
            {
                UIObjects.Add(obj);
            }
            else if (obj.CompareTag("Chat"))
            {
                UIObjects.Add(obj);
            }
            else if (obj.CompareTag("Next"))
            {
                BigStoneObjects.Add(obj);
            }
            else if (obj.CompareTag("3D"))
            {
                tresDObjects.Add(obj);
            }
        }

        // Desactiva todos los objetos con el tag "UI"
        foreach (GameObject obj in UIObjects)
        {
            obj.SetActive(true);
        }
        // Desactiva todos los objetos de la mano
        foreach (GameObject obj in BigStoneObjects)
        {
            obj.SetActive(false);
        }
        // Activa todos los objetos con el tag "3D"
        foreach (GameObject obj in tresDObjects)
        {
            obj.SetActive(false);
        }
        // Desactiva los objetos a desactivar
        foreach (GameObject obj in objectsToActivate)
        {
            if (obj != null)
            {
                obj.SetActive(false);
            }
        }
        // activa los objetos a desactivar
        foreach (GameObject obj in objectsToDesactivate)
        {
            if (obj != null)
            {
                obj.SetActive(true);
            }
        }

        if (objectToAnimation != null)
        {
            objectToAnimation.GetComponent<boolAnimation>().SetPanoramico(true);
        }

        if (objectToAnimation != null)
        {
            UIMe.SetActive(true);
        }
    }
}
