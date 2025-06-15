using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : NetworkBehaviour
{
    [Header("Referências")]
    public ScoreManager scoreManager;
    public HockeyPuckController scriptToDisable; // Script a ser desabilitado
    public TMP_Text gameOverText, instructionsText, pauseText; // Texto de game over (deve estar desativado inicialmente)
    public Vector3 positionDisc;
    
    [Header("Configurações")]
    public int winningScore = 5;
    public string menuSceneName = "MenuScene";
    public string leftWinMessage = "Left Player Wins!";
    public string rightWinMessage = "Right Player Wins!";

    private bool gameOver = true, isPaused = false;
    
    void Update()
    {
        if(!IsServer) return;
        
        // Verifica se o jogo já acabou
        if (gameOver)
        {
            // Se qualquer input for detectado, carrega o menu
            if (Input.GetKeyDown(KeyCode.R))
            {
                scoreManager.scoreRight = 0;
                scoreManager.scoreLeft = 0;
                scoreManager.rightScoreFormat = "Right: {0}";
                scoreManager.scoreFormat = "Left: {0}";;
                scriptToDisable.transform.position = positionDisc;
                scriptToDisable.enabled = true;
                scriptToDisable.endPartida = false;
                scriptToDisable.StartDisc();
                
                //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
            else if (Input.GetKeyDown(KeyCode.M))
            {
                //NetworkManager.Singleton.
                //SceneManager.LoadScene(menuSceneName);
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
        scriptToDisable.endPartida = true;
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

    }
}