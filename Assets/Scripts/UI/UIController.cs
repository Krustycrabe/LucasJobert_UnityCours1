using TMPro;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField] private TMP_Text _scoreText;

    private void Start()
    {
        UpdateScore(0);
    }
    
      
    

    public void UpdateScore(int newScore)
    {
        _scoreText.text = "Score :" + newScore.ToString();
    }
      

}
