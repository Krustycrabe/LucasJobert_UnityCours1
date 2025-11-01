using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerRespawnManager : MonoBehaviour
{
    public static PlayerRespawnManager Instance { get; private set; }

    [SerializeField] private Transform respawnPoint;
    private PolarityManager player;
    private Rigidbody playerRb;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        player = GetComponentInParent<PolarityManager>();
        if (player != null)
            playerRb = player.GetComponent<Rigidbody>();

        if (respawnPoint == null && player != null)
            respawnPoint = player.transform;
    }

    public void KillPlayer()
    {
        Debug.Log("☠️ Player mort → rechargement de la scène...");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private System.Collections.IEnumerator RespawnRoutine()
    {
        // ⚡ Optionnel : désactive brièvement le player pour effet de mort
        player.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.3f); // petit délai (invisible si tu veux 0f)

        RespawnPlayer();
    }

    private void RespawnPlayer()
    {
        if (player == null) return;

        // Réinitialise la physique
        player.transform.position = respawnPoint.position;
        player.transform.rotation = respawnPoint.rotation;

        if (playerRb != null)
        {
            playerRb.linearVelocity = Vector3.zero;
            playerRb.angularVelocity = Vector3.zero;
        }

        player.gameObject.SetActive(true);
        Debug.Log("↩️ Player respawné !");
    }

    public void SetRespawnPoint(Transform newPoint)
    {
        respawnPoint = newPoint;
        Debug.Log("📍 Nouveau point de respawn défini !");
    }
}
