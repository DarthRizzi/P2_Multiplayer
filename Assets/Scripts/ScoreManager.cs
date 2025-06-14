using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [Header("Configuração de Pontuação")]
    public int scoreLeft = 0;
    public int scoreRight = 0;

    [Header("Elementos de Texto")]
    [SerializeField] private TMP_Text leftScoreText;
    [SerializeField] private TMP_Text rightScoreText;

    [Header("Formatação")]
    public string scoreFormat = "Left: {0}";
    public string rightScoreFormat = "Right: {0}";

    void Start()
    {
        UpdateScoreTexts();
    }

    // Atualiza ambos os textos de pontuação
    private void UpdateScoreTexts()
    {
        if (leftScoreText != null)
            leftScoreText.text = string.Format(scoreFormat, scoreLeft);
        
        if (rightScoreText != null)
            rightScoreText.text = string.Format(rightScoreFormat, scoreRight);
    }

    // Métodos públicos para modificar as pontuações
    public void AddLeftScore(int amount)
    {
        scoreLeft += amount;
        UpdateScoreTexts();
    }

    public void AddRightScore(int amount)
    {
        scoreRight += amount;
        UpdateScoreTexts();
    }

    public void ResetScores()
    {
        scoreLeft = 0;
        scoreRight = 0;
        UpdateScoreTexts();
    }

    // Método para definir os textos manualmente se não estiverem atribuídos no Inspector
    public void SetTextReferences(TMP_Text leftText, TMP_Text rightText)
    {
        leftScoreText = leftText;
        rightScoreText = rightText;
        UpdateScoreTexts();
    }
}