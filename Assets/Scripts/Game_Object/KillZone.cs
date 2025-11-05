using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class KillZone : MonoBehaviour
{
    [Header("Kill Zone Settings")]
    [Tooltip("Si vrai, le joueur est respawné via PlayerRespawnManager, sinon la scène est reload.")]
    [SerializeField] private bool useRespawnManager = true;

    [Tooltip("Afficher la zone dans l'éditeur ?")]
    [SerializeField] private bool showGizmo = true;

    [Tooltip("Couleur de la zone dans l'éditeur.")]
    [SerializeField] private Color gizmoColor = new Color(1f, 0f, 0f, 0.25f);

    [Header("Data")]
    [SerializeField] private ScoreDatas scoreManager;

    private void Reset()
    {
        var col = GetComponent<BoxCollider>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Vérifie si c’est un objet magnétique
        IMagnetic magnetic = other.GetComponentInParent<IMagnetic>();
        if (magnetic == null) return;

        Rigidbody rb = other.attachedRigidbody;
        if (rb == null) return;

        // Si c’est le player
        if (magnetic is PolarityManager)
        {
            if (useRespawnManager && PlayerRespawnManager.Instance != null)
            {
                PlayerRespawnManager.Instance.KillPlayer();
            }
            else
            {
                scoreManager.ResetScore();
                UnityEngine.SceneManagement.SceneManager.LoadScene(
                    UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex
                );
            }
        }
        else
        {
            // Si c’est un MagneticObject → reset position
            MagneticObjectReset reset = other.GetComponentInParent<MagneticObjectReset>();
            if (reset != null)
            {
                reset.ResetToOrigin();
            }
            else
            {
                // fallback si l’objet n’a pas de script de reset
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.transform.position = Vector3.zero;
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (!showGizmo) return;
        Gizmos.color = gizmoColor;
        BoxCollider col = GetComponent<BoxCollider>();
        Matrix4x4 matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
        Gizmos.matrix = matrix;
        Gizmos.DrawCube(col.center, col.size);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(col.center, col.size);
    }
}
