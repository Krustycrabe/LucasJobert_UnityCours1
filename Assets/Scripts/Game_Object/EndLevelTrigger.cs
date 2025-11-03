using UnityEngine;

public class EndLevelTrigger : MonoBehaviour
{
    [SerializeField] private GameObject endLevelUI;
    private bool levelEnded = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<PolarityManager>() != null)
        {
            GameEventManager.RaiseLevelCompleted();
        }
    }
}