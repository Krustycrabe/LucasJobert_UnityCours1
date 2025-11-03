using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerRespawnManager : MonoBehaviour
{
    public static PlayerRespawnManager Instance { get; private set; }

    public enum DeathBehaviour { ReloadScene, RespawnAtPoint }

    [Header("Death Behaviour")]
    [SerializeField] private DeathBehaviour defaultBehaviour = DeathBehaviour.ReloadScene; // ← compat laser

    [Header("Respawn")]
    [SerializeField] private Transform respawnPoint;

    private PolarityManager player;
    private Rigidbody playerRb;

    // --- Singleton ---
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
        // Trouve le player de façon robuste
        if (player == null) player = GetComponentInParent<PolarityManager>();
        if (player == null) player = GetComponentInParent<PolarityManager>(true);

        if (player != null) playerRb = player.GetComponent<Rigidbody>();

        // Si aucun respawnPoint assigné, mémorise la position initiale du joueur
        if (respawnPoint == null && player != null)
        {
            GameObject pivot = new GameObject("RespawnPoint_Auto");
            pivot.transform.SetPositionAndRotation(player.transform.position, player.transform.rotation);
            respawnPoint = pivot.transform;
        }
    }

    // ------------------- API PUBLIQUE -------------------

    /// <summary>
    /// Rétro-compat : par défaut RELANCE la scène (pour le laser).
    /// </summary>
    public void KillPlayer()
    {
        if (defaultBehaviour == DeathBehaviour.ReloadScene)
            KillPlayerReload();
        else
            KillAndRespawn();
    }

    /// <summary>
    /// Appel statique safe, utilisable depuis n'importe où (laser, pièges, etc.).
    /// </summary>
    public static void KillPlayerStatic()
    {
        if (Instance != null) Instance.KillPlayer();
        else
        {
            // Fallback ultra-sûr si le manager n'est pas dans la scène
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    /// <summary>
    /// Mode "hard restart" : recharge la scène (Pour laser / mort instant).
    /// </summary>
    public void KillPlayerReload()
    {
        // Toujours remettre l'écoulement du temps avant un reload
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// Mode "respawn doux" : désactive/replace le joueur sans recharger la scène.
    /// </summary>
    public void KillAndRespawn()
    {
        if (!isActiveAndEnabled)
        {
            // Si le manager est disabled, on fait un reload pour éviter un blocage
            KillPlayerReload();
            return;
        }
        StartCoroutine(RespawnRoutine());
    }

    public void SetRespawnPoint(Transform newPoint)
    {
        respawnPoint = newPoint;
    }

    // ------------------- INTERNE -------------------

    private System.Collections.IEnumerator RespawnRoutine()
    {
        if (player != null) player.gameObject.SetActive(false);
        // Realtime : fonctionne même si le jeu est en pause (Time.timeScale = 0)
        yield return new WaitForSecondsRealtime(0.3f);
        RespawnPlayer();
    }

    private void RespawnPlayer()
    {
        if (player == null || respawnPoint == null) { KillPlayerReload(); return; }

        player.transform.SetPositionAndRotation(respawnPoint.position, respawnPoint.rotation);

        if (playerRb != null)
        {
            playerRb.linearVelocity = Vector3.zero;        // reset vitesses
            playerRb.angularVelocity = Vector3.zero;
        }

        player.gameObject.SetActive(true);
    }
}
