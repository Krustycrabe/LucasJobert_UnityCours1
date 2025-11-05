using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Unity.VisualScripting;

public class EndLevelUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text starText;

    [Header("Data")]
    [SerializeField] private ScoreDatas scoreManager;

    // optionnel : si tu veux masquer/afficher le curseur
    [SerializeField] private bool manageCursor = true;

    private bool isOpen = false;
    private PolarityManager cachedPlayer;

    private void Awake()
    {
        if (panel) panel.SetActive(false);
        cachedPlayer = GetComponent<PolarityManager>();
    }

    private void OnEnable()
    {
        GameEventManager.OnLevelCompleted += ShowMenu;
    }

    private void OnDisable()
    {
        GameEventManager.OnLevelCompleted -= ShowMenu;
        // sécurité : si jamais on disable alors que c’est ouvert, on remet le temps
        if (isOpen) ResumeTime();
    }

    // ---------- Affichage du menu de fin ----------
    private void ShowMenu()
    {
        if (isOpen) return;
        isOpen = true;

        if (panel) panel.SetActive(true);

        // freeze gameplay
        PauseTime();
        DisablePlayerControl();

        // update UI
        if (scoreText) scoreText.text = $"SCORE FINAL : {scoreManager.CurrentScore}";
        if (starText) starText.text = $"Stars : {scoreManager.StarCount}";
    }

    // ---------- Boutons ----------
    public void RestartLevel()
    {
        Time.timeScale = 1f;
        panel?.SetActive(false);
        scoreManager.ResetScore();

        if (PlayerRespawnManager.Instance != null)
            PlayerRespawnManager.Instance.KillPlayerReload(); // ← reload scène
        else
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }


    public void NextLevel()
    {
        ResumeTime();

        if (SceneManager.GetActiveScene().buildIndex != 0)
            scoreManager.ResetScore();

        int next = SceneManager.GetActiveScene().buildIndex + 1;
        if (next < SceneManager.sceneCountInBuildSettings)
            SceneManager.LoadScene(next);
        else
            Debug.Log("Dernier niveau atteint !");
    }

    public void QuitGame()
    {
        ResumeTime();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // ---------- Helpers ----------
    private void PauseTime()
    {
        Time.timeScale = 0f;
        if (manageCursor)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    private void ResumeTime()
    {
        Time.timeScale = 1f;
        if (manageCursor)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    private void DisablePlayerControl()
    {
        if (!cachedPlayer) cachedPlayer = GetComponent<PolarityManager>();
        if (cachedPlayer) cachedPlayer.enabled = false; // simple et efficace
    }
}
