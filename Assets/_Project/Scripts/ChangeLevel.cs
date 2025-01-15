using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeLevel : MonoBehaviour
{
    public string nombreDelNivel;

    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "_Agencia")
        {
            nombreDelNivel = "_PruebaPanel";
        }
        else
        {
            nombreDelNivel = "_Agencia";
        }
    }

    // Método para cambiar de nivel
    public void Cambiodenivel()
    {
        

        // Encuentra el GameObject con el tag "Fade"
        GameObject fadeObject = GameObject.FindGameObjectWithTag("Fade");
        if (fadeObject != null)
        {
            // Obtiene el componente Animator del GameObject encontrado
            Animator fadeAnimator = fadeObject.GetComponent<Animator>();
            if (fadeAnimator != null)
            {
                // Activa la condición trigger "fadeout"
                fadeAnimator.SetTrigger("FadeOut");
            }
            else
            {
                Debug.LogError("No se encontró el componente Animator en el GameObject con tag 'Fade'.");
            }
        }
        else
        {
            Debug.LogError("No se encontró un GameObject con el tag 'Fade'.");
        }

        //SceneManager.LoadScene(level);
    }
    public void nivel()
    {
        SceneManager.LoadScene(nombreDelNivel);
    }
}
