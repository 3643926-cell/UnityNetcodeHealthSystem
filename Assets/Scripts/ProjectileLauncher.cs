using UnityEngine;
using Unity.Netcode;

public class ProjectileLauncher : NetworkBehaviour
{
    [SerializeField] private GameObject serverProjectilePrefab;
    [SerializeField] private GameObject clientProjectilePrefab;

    [SerializeField] private Transform projectileSpawnPoint;

    [SerializeField] private InputReader inputReader;

    [Header("Settings")] [SerializeField] private float projectileSpeed;

    [SerializeField] private GameObject muzzleFlash;

    [SerializeField] private Collider2D playerCollider;

    [SerializeField] private float fireRate = 1f; 
    // Disparos por segundo
    private float previousFireTime = 0f;

    [SerializeField] private float muzzleFlashDuration = 0.075f; 
    private float muzzleFlashTimer = 0f;


    public override void OnNetworkSpawn() 
    {
        if (!IsOwner) return; 
        inputReader.PrimaryFireEvent += HandlePrimaryFire;
    }

    public override void OnNetworkDespawn() 
    {
        if (IsOwner) 
        {
            inputReader.PrimaryFireEvent -= HandlePrimaryFire;
        }
    }

    private bool shouldFire = false; 
    private void HandlePrimaryFire(bool shouldFire) 
    { 
        this.shouldFire = shouldFire; 
    }

    private void Update()
    {
        if (muzzleFlashTimer > 0)
        {
            muzzleFlashTimer -= Time.deltaTime;
            if (muzzleFlashTimer <= 0)
            {
                muzzleFlash.SetActive(false);
            }
        }

        if (!IsOwner) return; 
        if (!shouldFire) return;

        if (Time.time < previousFireTime + (1f / fireRate))
        {
            return; 
            // Aún no ha pasado el tiempo suficiente para disparar
        } 
        // Si pasa la validación:
        previousFireTime = Time.time;  

        // 1. Crear proyectil local
        SpawnDummyProjectile(projectileSpawnPoint.position, projectileSpawnPoint.up);      
        
        // 2. Avisar al servidor
        PrimaryFireServerRpc(projectileSpawnPoint.position, projectileSpawnPoint.up);
    }

    private void SpawnDummyProjectile(Vector3 spawnPos, Vector3 direction) 
    { 
        GameObject projectileInstance = Instantiate(clientProjectilePrefab, spawnPos, Quaternion.identity);
        Collider2D projectileCollider = projectileInstance.GetComponent<Collider2D>(); 
        Physics2D.IgnoreCollision(playerCollider, projectileCollider);
        projectileInstance.transform.up = direction;
        if (projectileInstance.TryGetComponent<Rigidbody2D>(out var rb))
        { 
            // Importante en 2D usar "transform.up" en lugar de "transform.forward"
            rb.linearVelocity = rb.transform.up * projectileSpeed; 
        }
        muzzleFlash.SetActive(true); 
        muzzleFlashTimer = muzzleFlashDuration;
    }

    [ServerRpc]
    private void PrimaryFireServerRpc(Vector3 spawnPos, Vector3 direction)
    {     // Instanciar el proyectil real
          GameObject projectileInstance = Instantiate(serverProjectilePrefab, spawnPos, Quaternion.identity);     
          projectileInstance.transform.up = direction;      
          // Notificar a todos los clientes
          SpawnDummyProjectileClientRpc(spawnPos, direction); 
    }

    [ClientRpc]
    private void SpawnDummyProjectileClientRpc(Vector3 spawnPos, Vector3 direction)
    {
        if (IsOwner) return; 
        
        // Evita crear doble proyectil en quien disparo
        SpawnDummyProjectile(spawnPos, direction); }
    }


