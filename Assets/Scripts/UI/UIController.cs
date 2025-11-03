using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text starText;
    [SerializeField] private ScoreDatas scoreDatas;

    private void OnEnable()
    {
        GameEventManager.OnScoreChanged += UpdateScore;
        GameEventManager.OnStarCollected += UpdateStars;
        UpdateScore(scoreDatas.CurrentScore);
        UpdateStars(scoreDatas.StarCount);
    }

    private void OnDisable()
    {
        GameEventManager.OnScoreChanged -= UpdateScore;
        GameEventManager.OnStarCollected -= UpdateStars;
    }

    private void UpdateScore(int value)
    {
        if (scoreText != null)
            scoreText.text = $"SCORE : {value}";
    }

    private void UpdateStars(int count)
    {
        if (starText != null)
            starText.text = $"STARS : {count}";
    }
}
