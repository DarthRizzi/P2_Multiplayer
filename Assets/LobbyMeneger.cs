using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyMeneger : MonoBehaviour
{
    #region variaves
    
    static public LobbyMeneger Instance;
    
    [Header("mostra Lobbys")]
    public GameObject mostraLobby;

    [Header("cria o perfil")]
    public GameObject crearOPerfil;
    public TMP_InputField nomeDoJogador;
    public TMP_Text MensagemDeErro;
    
    [Header("AntigaUi")]
    public GameObject antigaUi;

    [Space(10)]
    [Header("Lobby List")]
    [SerializeField] private Transform lobbyContectParent;
    [SerializeField] private GameObject lobbyIteamPrefab;
    
    [Space(10)]
    [Header("Joined Lobby")]
    [SerializeField] private Transform playerIteamPrefab;
    [SerializeField] private Transform playerListParent;
    [SerializeField] private TextMeshProUGUI joinedLobbyNameText;
    [SerializeField] private GameObject joinLobbyStartButton;
    [SerializeField] private GameObject joinLobbyObj;
    
    [Header("creat lobby")] 
    public int createlobbyMaxPlayerField = 2;
    public TMP_InputField createLobbyNameField;
    [SerializeField] private GameObject craftLobbyObj;
    
    string playerName;
    private Player playerData;
    private string joinedLobbyId;
    public Lobby lobby;
    #endregion

    #region funcao

    async void Awake()
    {
        nomeDoJogador.characterLimit = 15;
        createLobbyNameField.characterLimit = 15;
        
        Instance = this;

        await UnityServices.InitializeAsync();


        if (AuthenticationService.Instance.SessionTokenExists)
            AuthenticationService.Instance.SignOut();
     
        try
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
        catch
        {
            Debug.LogError("not singIn");
        }
        await UnityServices.InitializeAsync();
    }
    
    public void VerOnomeDoUsusario()
    {
        if (nomeDoJogador.text == "")
        {
            MensagemDeErro.text = "O campo não pode esta nulo ou vazio";
            return;
        }

        if (nomeDoJogador.text.ToCharArray().Any(caractere => !caractere.Equals(' ')))
        {
            MensagemDeErro.text = "";
            crearOPerfil.SetActive(false);
            mostraLobby.SetActive(true);
            showLobbies();
            playerName = nomeDoJogador.text;
            
            PlayerDataObject playerDataObjectName = new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, playerName);
            PlayerDataObject playerDataObjectTeam = new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, "A");
            PlayerDataObject playerDataObjectSave = new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, "0");

            playerData = new Player(id: AuthenticationService.Instance.PlayerId,data:
                new Dictionary<string, PlayerDataObject> 
                { 
                    {"Name", playerDataObjectName },
                    {"Team", playerDataObjectTeam },
                    {"SaveNeme", playerDataObjectSave}
                });
            return;
        }
        MensagemDeErro.text = "O campo não pode esta nulo ou vazio";
        
    }
    
    public void lobbyShow()
    {
        crearOPerfil.SetActive(true);
        antigaUi.SetActive(false);
    }


    #region lobby

    private async void showLobbies()
    {
        while (Application.isPlaying && mostraLobby.activeInHierarchy)
        {
            QueryLobbiesOptions queryLobbiesOptions = new QueryLobbiesOptions();
            queryLobbiesOptions.Filters = new List<QueryFilter>();

            QueryResponse querryResponse = await LobbyService.Instance.QueryLobbiesAsync(queryLobbiesOptions);

            foreach (Transform t in lobbyContectParent)
            {
                Destroy(t.gameObject);
            }

            foreach(Lobby lobby in querryResponse.Results)
            {
                GameObject newLobbyIteam = Instantiate(lobbyIteamPrefab, lobbyContectParent);
                newLobbyIteam.GetComponent<JoinLobbyButton>().lobbyId = lobby.Id;
                newLobbyIteam.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = lobby.Name;
                newLobbyIteam.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = lobby.Players.Count + "/" + lobby.MaxPlayers;
            
            }

            await Task.Delay(1000);
        }
    }
    
    public async void JoinLobby(string lobbyID)
    {
        try
        {
            await LobbyService.Instance.JoinLobbyByIdAsync(lobbyID, new JoinLobbyByIdOptions { Player = playerData });

            joinedLobbyId = lobbyID;
            UpdateLobbyInfo();
            joinLobbyObj.SetActive(true);
            mostraLobby.SetActive(false);

        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    private bool isJoin;
    private async void UpdateLobbyInfo()
    {
        while (Application.isPlaying)
        {

            if (string.IsNullOrEmpty(joinedLobbyId)) return;

            Lobby lobby = await LobbyService.Instance.GetLobbyAsync(joinedLobbyId);

            if (lobby == null) return;


            if (!isJoin && lobby.Data["JoinCode"].Value != string.Empty)
            {

                JoinAllocation joinAllocation =
                    await RelayService.Instance.JoinAllocationAsync(lobby.Data["JoinCode"].Value);

                NetworkManager.Singleton.GetComponent<UnityTransport>().SetClientRelayData(
                    joinAllocation.RelayServer.IpV4,
                    (ushort)joinAllocation.RelayServer.Port,
                    joinAllocation.AllocationIdBytes,
                    joinAllocation.Key,
                    joinAllocation.ConnectionData,
                    joinAllocation.HostConnectionData
                );

                NetworkManager.Singleton.StartClient();
                isJoin = true;
                return;
            }

            if (AuthenticationService.Instance.PlayerId == lobby.HostId)
            {
                joinLobbyStartButton.SetActive(true);
            }
            else
            {
                joinLobbyStartButton.SetActive(false);
            }

            joinedLobbyNameText.text = lobby.Name;

            foreach (Transform t in playerListParent)
            {
                Destroy(t.gameObject);
            }

            foreach (Player player in lobby.Players)
            {
                GameObject newPlayerIteam = Instantiate(playerIteamPrefab.gameObject, playerListParent);
                newPlayerIteam.transform.GetChild(0).GetComponent<TMP_Text>().text = player.Data["Name"].Value;
                newPlayerIteam.transform.GetChild(1).GetComponent<TMP_Text>().text = player.Data["Team"].Value;
                newPlayerIteam.transform.GetChild(2).GetComponent<TMP_Text>().text =
                    (lobby.HostId == player.Id) ? "owner" : "User";

            }

            await Task.Delay(3 * 1000);
        }
    }
    
    public async void CreateLobby()
    {
        Lobby createdLobby = null;

        CreateLobbyOptions options = new CreateLobbyOptions();
        options.IsPrivate = false;
        options.Player = playerData;

        DataObject dataObjectJoinCode = new DataObject(DataObject.VisibilityOptions.Public, string.Empty);

        DataObject dataObjectName = new DataObject(DataObject.VisibilityOptions.Public, string.Empty);
        options.Data = new Dictionary<string, DataObject>
        {
            {
                "JoinCode", dataObjectJoinCode
            }
        };

        try
        {
            createdLobby = await LobbyService.Instance.CreateLobbyAsync(createLobbyNameField.text, createlobbyMaxPlayerField, options);
            joinedLobbyId = createdLobby.Id;

            lobby = createdLobby;

            //await LobbyStart();

            UpdateLobbyInfo();

        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }

        lobbyHeartbeat(createdLobby);
        joinLobbyObj.SetActive(true);
        craftLobbyObj.SetActive(false);
        //comecar o lobby
        //await LobbyStart();

    }
    
    private async void lobbyHeartbeat(Lobby lobby)
    {
        while (true)
        {
            if (a) return;

            if (lobby == null) return;

            await LobbyService.Instance.SendHeartbeatPingAsync(lobby.Id);

            await Task.Delay(15 * 1000);

        }
    }
    
    bool a;
    public async void DestroyLobby()
    {
        a = true;
        try
        {
            AuthenticationService.Instance.ClearSessionToken();
        }
        catch
        {

        }
        await LobbyService.Instance.DeleteLobbyAsync(lobby.Id);
    }
    public async void LobbyStart()
    {
        Lobby lobby = await LobbyService.Instance.GetLobbyAsync(joinedLobbyId);

        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(lobby.MaxPlayers);

            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            isJoin = true;

            await LobbyService.Instance.UpdateLobbyAsync(joinedLobbyId, new UpdateLobbyOptions
            {
                Data = new Dictionary<string, DataObject>
                {
                    { "JoinCode", new DataObject(DataObject.VisibilityOptions.Public, joinCode) }
                }
            });

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetHostRelayData(
                allocation.RelayServer.IpV4,
                (ushort)allocation.RelayServer.Port, 
                allocation.AllocationIdBytes,
                allocation.Key,
                allocation.ConnectionData
            );

            NetworkManager.Singleton.StartHost();

        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
        }
        
    }
    
    #endregion
    #endregion
}
