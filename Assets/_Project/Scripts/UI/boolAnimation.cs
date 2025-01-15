using UnityEngine;

public class boolAnimation : MonoBehaviour
{ 
    private Animator animator;

    void Start()
    {
        // Obtén el componente Animator del objeto
        animator = GetComponent<Animator>();
        
        // Verifica si el Animator existe en el objeto
        if (animator == null)
        {
            Debug.LogError("No se encontró el componente Animator en este objeto.");
        }
    }

    // Función para establecer el valor del parámetro "panoramico"
    public void SetPanoramico(bool value)
    {
        if (animator != null)
        {
            animator.SetBool("panoramico", value);
        }
    }
    void Update()
    {
        // Verifica si se ha presionado la tecla "P"
        if (Input.GetKeyDown(KeyCode.P))
        {
            // Alterna el valor del parámetro "panoramico"
            bool currentValue = animator.GetBool("panoramico");
            SetPanoramico(!currentValue);
        }
    }
}
