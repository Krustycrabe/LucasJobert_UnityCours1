using UnityEngine;

[RequireComponent(typeof(Transform))]
public class MagneticAnchorMover : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private Transform pointA;  // Point de départ
    [SerializeField] private Transform pointB;  // Point d'arrivée
    [SerializeField] private float speed = 2f;  // Vitesse du déplacement
    [SerializeField] private bool loop = true;  // Repart après arrivée
    [SerializeField] private bool usePingPong = true; // Va-et-vient

    [Header("Optional")]
    [SerializeField] private bool visualizePath = true; // Pour voir dans l'éditeur

    private Vector3 startPos;
    private Vector3 endPos;
    private float journeyLength;
    private float t;
    private bool goingForward = true;

    private void Start()
    {
        // Si les points ne sont pas assignés, crée automatiquement un déplacement vertical
        if (pointA == null)
        {
            pointA = new GameObject(name + "_PointA").transform;
            pointA.position = transform.position;
        }

        if (pointB == null)
        {
            pointB = new GameObject(name + "_PointB").transform;
            pointB.position = transform.position + Vector3.down * 3f; // Mouvement vertical par défaut
        }

        startPos = pointA.position;
        endPos = pointB.position;
        journeyLength = Vector3.Distance(startPos, endPos);
    }

    private void Update()
    {
        if (journeyLength <= 0.01f) return;

        // Avance le "t" selon la direction
        float step = (speed / journeyLength) * Time.deltaTime;
        t += goingForward ? step : -step;

        // Boucle ou ping-pong
        if (usePingPong)
        {
            if (t > 1f)
            {
                t = 1f;
                goingForward = false;
            }
            else if (t < 0f)
            {
                t = 0f;
                goingForward = true;
            }
        }
        else if (loop && t > 1f)
        {
            t = 0f;
        }

        // Interpolation
        transform.position = Vector3.Lerp(startPos, endPos, Mathf.SmoothStep(0f, 1f, t));
    }

    private void OnDrawGizmos()
    {
        if (!visualizePath) return;

        Gizmos.color = Color.red;
        if (pointA != null && pointB != null)
        {
            Gizmos.DrawLine(pointA.position, pointB.position);
            Gizmos.DrawSphere(pointA.position, 0.1f);
            Gizmos.DrawSphere(pointB.position, 0.1f);
        }
    }
}
