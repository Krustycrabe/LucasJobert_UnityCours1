using UnityEngine;

public class Player_Collect : MonoBehaviour
{
    [SerializeField] private ScoreDatas _scoreData;

    public void UpdateScore(int value)
    {
        _scoreData.ScoreValue = Mathf.Clamp(_scoreData.ScoreValue+ value,0,_scoreData.ScoreValue+ value);
    }
}
