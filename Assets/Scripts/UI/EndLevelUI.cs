using UnityEngine;
using UnityEngine.SceneManagement;

public class EndLevelUI : MonoBehaviour
{
    public void RestartLevel()
    {
        Debug.Log("🔁 Redémarrage du niveau...");
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void NextLevel()
    {
        Debug.Log("➡️ Niveau suivant...");
        Time.timeScale = 1f;

        int nextIndex = SceneManager.GetActiveScene().buildIndex + 1;

        // Vérifie qu’un niveau suivant existe
        if (nextIndex < SceneManager.sceneCountInBuildSettings)
            SceneManager.LoadScene(nextIndex);
        else
            Debug.Log("🏁 Dernier niveau atteint !");
    }

    public void QuitToMenu()
    {
        Debug.Log("⬅️ Retour au menu principal...");
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu"); // Optionnel, si tu veux un menu plus tard
    }
}