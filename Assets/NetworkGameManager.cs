using System.Text;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkGameManager : MonoBehaviour
{
    [Header("Prefabs de Jogador")]
    public GameObject playerPrefabA;
    public GameObject playerPrefabB;

    public void Start()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        }
    }

    public void StartHosting()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            OnClientConnected(NetworkManager.Singleton.LocalClientId);
            StartGame();
        }
    }
    
    private void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        }
    }
    void OnClientConnected(ulong clientId)
    {
        // Pega o ConnectionData do cliente
        var connData = Encoding.ASCII.GetString(NetworkManager.Singleton.NetworkConfig.ConnectionData);
    
        GameObject playerToSpawn;
    
        if (connData == "A")
            playerToSpawn = Instantiate(playerPrefabA);
        else if (connData == "B")
            playerToSpawn = Instantiate(playerPrefabB);
        else
            playerToSpawn = Instantiate(playerPrefabA); // fallback
    
        var netObj = playerToSpawn.GetComponent<NetworkObject>();
        netObj.SpawnAsPlayerObject(clientId);
    }
    
    // Host chama isso para trocar de cena
    public void StartGame()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            NetworkManager.Singleton.SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
        }
    }
}