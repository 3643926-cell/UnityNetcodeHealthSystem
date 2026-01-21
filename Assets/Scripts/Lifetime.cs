using UnityEngine; 

public class Lifetime : MonoBehaviour 
{
    [SerializeField] private float lifetime = 1f; // Valor por defecto de 1 segundo
    
    private void Start() 
    {
        // Podemos usar directamente Destroy con el parámetro de tiempo
        Destroy(gameObject, lifetime);
    } 
}  