using UnityEngine;

public class EndLevelTrigger : MonoBehaviour
{
    [SerializeField] private GameObject endLevelUI;
    private bool levelEnded = false;

    private void OnTriggerEnter(Collider other)
    {
        if (levelEnded) return;

        // Vérifie si l'objet a un PolarityManager (ton player)
        var player = other.GetComponentInParent<PolarityManager>();
        if (player != null)
        {
            levelEnded = true;;
            endLevelUI.SetActive(true);
            Time.timeScale = 0f;
        }
    }
}