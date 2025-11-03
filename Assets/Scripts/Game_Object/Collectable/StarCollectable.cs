using UnityEngine;

[RequireComponent(typeof(Collider))]
public class StarCollectable : MonoBehaviour
{
    [SerializeField] private int scoreBonus = 100;
    [SerializeField] private ScoreDatas scoreDatas;

    private bool collected = false;

    private void OnTriggerEnter(Collider other)
    {
        if (collected) return;
        if (!other.TryGetComponent(out PolarityManager player)) return;

        collected = true;
        scoreDatas.AddScore(scoreBonus);
        scoreDatas.AddStar();

        // Optionnel : feedback visuel
        // TODO: ParticleSystem / son / fondu
        Destroy(gameObject);
    }
}
