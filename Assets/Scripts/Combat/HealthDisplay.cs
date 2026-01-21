using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class HealthDisplay : NetworkBehaviour
{
    [SerializeField] private Health health;
    [SerializeField] private Image healthBarImage;

    public override void OnNetworkSpawn()
    {
        // La UI solo tiene sentido en clientes.
        if (!IsClient) return;

        if (health == null || healthBarImage == null) return;

        health.currentHealth.OnValueChanged += HandleHealthChanged;

        // Inicializar manualmente con el valor actual.
        HandleHealthChanged(0, health.currentHealth.Value);
    }

    public override void OnNetworkDespawn()
    {
        if (!IsClient) return;

        if (health == null) return;

        health.currentHealth.OnValueChanged -= HandleHealthChanged;
    }

    private void HandleHealthChanged(int oldHealth, int newHealth)
    {
        if (healthBarImage == null) return;

        float max = health.MaxHealth <= 0 ? 1f : health.MaxHealth;
        healthBarImage.fillAmount = newHealth / max;
    }
}
