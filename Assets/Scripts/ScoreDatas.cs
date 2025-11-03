using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "ScoreDatas", menuName = "Game/Score Datas", order = 0)]
public class ScoreDatas : ScriptableObject
{
    [SerializeField] private int currentScore = 0;
    [SerializeField] private int starCount = 0;

    public int CurrentScore => currentScore;
    public int StarCount => starCount;

#if UNITY_EDITOR
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void ResetOnPlay()
    {
        // Recherche automatique de l’asset ScoreManager dans le projet
        var scoreManagers = Resources.FindObjectsOfTypeAll<ScoreDatas>();
        foreach (var sm in scoreManagers)
        {
            sm.ResetScore();
        }
    }
#endif


    public void ResetScore()
    {
        currentScore = 0;
        starCount = 0;
        GameEventManager.RaiseScoreChanged(currentScore);
    }

    public void AddScore(int amount)
    {
        currentScore += amount;
        GameEventManager.RaiseScoreChanged(currentScore);
    }

    public void AddStar()
    {
        starCount++;
        GameEventManager.RaiseStarCollected(starCount);
    }
}
