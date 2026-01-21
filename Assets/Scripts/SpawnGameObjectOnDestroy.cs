using UnityEngine;

public class SpawnGameObjectOnDestroy : MonoBehaviour
{
    [Header("Prefab a instanciar al destruirse este objeto")]
    [SerializeField] private GameObject prefabToSpawn;

    private void OnDestroy()
    {
        // Si no hay prefab asignado, no hacemos nada
        if (prefabToSpawn == null)
            return;

        // Temporal: solo la instancia "ClientProjectile" genera FX
        if (!name.Contains("ClientProjectile")) return;

        Instantiate(prefabToSpawn, transform.position, transform.rotation);
    }
}