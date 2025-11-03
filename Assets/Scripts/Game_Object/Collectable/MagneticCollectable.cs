using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Collectable : MonoBehaviour
{
    [SerializeField] private int scoreValue = 10;
    [SerializeField] private ScoreDatas scoreDatas;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PolarityManager player))
        {
            scoreDatas.AddScore(scoreValue);
            Destroy(gameObject);
        }
    }
}
