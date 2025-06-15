using UnityEngine;
using UnityEngine.EventSystems;

public class JoinLobbyButton : MonoBehaviour ,IPointerClickHandler
{
    
    /// <summary>
    /// Stores the ID of the lobby to be joined.
    /// </summary>
    public string lobbyId;

    /// <summary>
    /// Called when the join lobby button is pressed.
    /// Initiates the process of joining the specified lobby.
    /// </summary>
    public void OnJoinLobbyButtonPressed()
    {
        LobbyMeneger.Instance.JoinLobby(lobbyId);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnJoinLobbyButtonPressed();
    }
}
