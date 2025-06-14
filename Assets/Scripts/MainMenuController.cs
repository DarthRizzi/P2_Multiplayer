using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [Header("Botões")]
    public Button playButton;    // Botão para iniciar o jogo
    public Button closeButton;  // Botão para fechar o jogo

    void Start()
    {
        // Verifica se os botões estão atribuídos e adiciona os listeners
        if (playButton != null)
        {
            playButton.onClick.AddListener(StartGame);
        }
        else
        {
            Debug.LogError("PlayButton não atribuído no Inspector!");
        }

        if (closeButton != null)
        {
            closeButton.onClick.AddListener(QuitGame);
        }
        else
        {
            Debug.LogError("CloseButton não atribuído no Inspector!");
        }
    }

    // Método para iniciar o jogo
    public void StartGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    // Método para fechar o jogo
    public void QuitGame()
    {
        #if UNITY_EDITOR
            // Se estiver no editor, para de executar o play mode
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            // Se estiver na build compilada, fecha o aplicativo
            Application.Quit();
        #endif
    }
}