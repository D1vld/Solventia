using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class HorizontalObjectLayout : MonoBehaviour
{
    public Transform[] objectsToArrange; // Los objetos que quieres organizar
    public float spacing = 0.5f; // Espaciado adicional entre objetos
    public Vector3 startPosition = Vector3.zero; // Posición inicial del primer objeto

    void OnValidate()
    {
        ArrangeObjects();
    }

    public void ArrangeObjects()
    {
        // Posición inicial para el primer objeto
        Vector3 nextPosition = startPosition;

        foreach (Transform obj in objectsToArrange)
        {
            if (obj != null)
            {
                // Coloca el objeto en la posición actual
                obj.position = nextPosition;

                // Obtén el tamaño del objeto usando el MeshRenderer o Collider si está disponible
                Renderer renderer = obj.GetComponent<Renderer>();
                Collider collider = obj.GetComponent<Collider>();

                // Calcula el ancho del objeto
                float objectWidth = 0;

                if (renderer != null)
                {
                    objectWidth = renderer.bounds.size.x;
                }
                else if (collider != null)
                {
                    objectWidth = collider.bounds.size.x;
                }

                // Mueve la posición siguiente hacia la derecha según el tamaño del objeto más el espaciado
                nextPosition += new Vector3(objectWidth + spacing, 0, 0);
            }
        }
    }
}

