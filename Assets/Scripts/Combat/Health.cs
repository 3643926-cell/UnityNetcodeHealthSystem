using System;
using Unity.Netcode;
using UnityEngine;

public class Health : NetworkBehaviour
{
    // Solo el servidor escribe; todos leen.
    public NetworkVariable<int> currentHealth = new NetworkVariable<int>(
        0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    // Visible en Inspector, get público y set privado.
    [field: SerializeField]
    public int MaxHealth { get; private set; } = 100;

    private bool isDead = false;

    public event Action<Health> OnDie;

    public override void OnNetworkSpawn()
    {
        // Solo el servidor inicializa la salud sincronizada.
        if (!IsServer) return;

        currentHealth.Value = MaxHealth;
        isDead = false;
    }

    public void TakeDamage(int damageValue)
    {
        // Por diseño, el servidor es quien debe aplicar daño.
        if (!IsServer) return;

        ModifyHealth(-damageValue);
    }

    public void RestoreHealth(int healValue)
    {
        if (!IsServer) return;

        ModifyHealth(healValue);
    }

    private void ModifyHealth(int value)
    {
        if (isDead) return;

        int newHealth = currentHealth.Value + value;
        currentHealth.Value = Mathf.Clamp(newHealth, 0, MaxHealth);

        if (currentHealth.Value == 0)
        {
            isDead = true;
            OnDie?.Invoke(this);
        }
    }
}
