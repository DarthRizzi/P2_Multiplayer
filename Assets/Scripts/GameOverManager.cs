using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    [Header("Referências")]
    public ScoreManager scoreManager;
    public MonoBehaviour scriptToDisable; // Script a ser desabilitado
    public TMP_Text gameOverText, instructionsText, pauseText; // Texto de game over (deve estar desativado inicialmente)

    [Header("Configurações")]
    public int winningScore = 5;
    public string menuSceneName = "MenuScene";
    public string leftWinMessage = "Left Player Wins!";
    public string rightWinMessage = "Right Player Wins!";

    private bool gameOver = false, isPaused = false;

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.P))
        {
            TogglePause();
        }

        // Verifica se o jogo já acabou
        if (gameOver)
        {
            // Se qualquer input for detectado, carrega o menu
            if (Input.GetKeyDown(KeyCode.R))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
            else if (Input.GetKeyDown(KeyCode.M))
            {
                SceneManager.LoadScene(menuSceneName);
            }
        }

        // Verifica as condições de vitória
        if (scoreManager.scoreLeft >= winningScore)
        {
            EndGame(leftWinMessage);
        }
        else if (scoreManager.scoreRight >= winningScore)
        {
            EndGame(rightWinMessage);
        }
    }

    void EndGame(string message)
    {
        gameOver = true;

        // Desabilita o script especificado
        if (scriptToDisable != null)
        {
            scriptToDisable.enabled = false;
        }

        // Ativa e configura o texto de game over
        if (gameOverText != null)
        {
            gameOverText.gameObject.SetActive(true);
            instructionsText.gameObject.SetActive(true);
            gameOverText.text = message;
        }

        // Opcional: Pausa o jogo
        Time.timeScale = 0f;
    }

    void OnDestroy()
    {
        // Garante que o timescale volte ao normal quando o objeto for destruído
        Time.timeScale = 1f;
    }
    
     void TogglePause()
    {
        isPaused = !isPaused;

        // Ativa/desativa o texto de pausa
        if (pauseText != null)
        {
            pauseText.gameObject.SetActive(isPaused);
        }

        // Pausa/despausa o jogo
        Time.timeScale = isPaused ? 0f : 1f;
    }
}