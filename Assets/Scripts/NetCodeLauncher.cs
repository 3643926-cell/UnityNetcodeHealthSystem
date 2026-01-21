using System.Linq;
using UnityEngine;
using Unity.Netcode;

public class NetCodeLauncher : MonoBehaviour
{
    public NetworkObject playerOnePrefab;
    public NetworkObject playerTwoPrefab;
    public Transform spawnPointOne;
    public Transform spawnPointTwo;

    void OnEnable()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
    }

    void OnDisable()
    {
        if (NetworkManager.Singleton != null)
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
    }

    private void OnClientConnected(ulong clientId)
    {
        // Solo el servidor/host spawnea jugadores
        if (!NetworkManager.Singleton.IsServer) return;

        // Número de clientes conectados en este instante
        int numClients = NetworkManager.Singleton.ConnectedClientsIds.Count;

        NetworkObject prefabToSpawn;
        Transform spawnPoint;

        // Primer cliente -> Player 1, segundo -> Player 2
        if (numClients == 1)
        {
            prefabToSpawn = playerOnePrefab;
            spawnPoint = spawnPointOne;
        }
        else
        {
            prefabToSpawn = playerTwoPrefab;
            spawnPoint = spawnPointTwo;
        }

        // Instanciamos y lo marcamos como PlayerObject de ese clientId
        NetworkObject playerInstance = Instantiate(prefabToSpawn, spawnPoint.position, spawnPoint.rotation);
        playerInstance.SpawnAsPlayerObject(clientId);
    }
}
