using UnityEngine;
public class DestroySelfOnContact : MonoBehaviour 
{ 
    private void OnTriggerEnter2D(Collider2D other) 
    { 
        // Tan pronto colisione con cualquier cosa, se destruye 
        Destroy(gameObject);
    } 
} 