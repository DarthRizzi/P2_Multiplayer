using System.Collections.Generic;
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

    private Dictionary<ulong, string> clientTeams = new Dictionary<ulong, string>();

    public void Start()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        }
    }

    public void StartHosting()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
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
        
        if (!NetworkManager.Singleton.IsServer)
            return;

        string team = clientTeams.ContainsKey(clientId) ? clientTeams[clientId] : "A";

        GameObject playerToSpawn = team switch
        {
            "A" => Instantiate(playerPrefabA),
            "B" => Instantiate(playerPrefabB),
            _ => Instantiate(playerPrefabA)
        };

        playerToSpawn.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
    }
    
    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        response.Approved = true;
        response.CreatePlayerObject = false; // A gente instancia manualmente

        string team = Encoding.ASCII.GetString(request.Payload);
        Debug.Log($"Client {request.ClientNetworkId} pediu conexão com team: {team}");

        clientTeams[request.ClientNetworkId] = team;
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