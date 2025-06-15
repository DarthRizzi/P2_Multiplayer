using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyMeneger : MonoBehaviour
{
    #region variaves
    [Header("mostra Lobbys")]
    public GameObject mostraLobby;

    [Header("cria o perfil")]
    public GameObject crearOPerfil;
    public TMP_InputField nomeDoJogador;
    public TMP_Text MensagemDeErro;
    
    [Header("AntigaUi")]
    public GameObject antigaUi;

    
    #endregion

    #region funcao

    void Awake()
    {
        nomeDoJogador.characterLimit = 15;
    }
    
    
    public void VerOnomeDoUsusario()
    {
        if (nomeDoJogador.text == "")
        {
            MensagemDeErro.text = "O campo não pode esta nulo ou vazio";
            return;
        }

        foreach (var caractere in nomeDoJogador.text.ToCharArray())
        {
            if (caractere.Equals(' ')) continue;
            //pode continuar
            MensagemDeErro.text = "";
            crearOPerfil.SetActive(false);
            mostraLobby.SetActive(true);
            return;
        }
        MensagemDeErro.text = "O campo não pode esta nulo ou vazio";
        
    }
    
    public void lobbyShow()
    {
        crearOPerfil.SetActive(true);
        antigaUi.SetActive(false);
    }

    #endregion
}
