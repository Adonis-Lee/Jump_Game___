using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverUI : MonoBehaviour
{
    [Header("Skor Yazıları")]
    public TextMeshProUGUI currentScoreText;
    public TextMeshProUGUI highScoreText;
    public TextMeshProUGUI congratsText;

    void OnEnable()
    {
        bool isNewRecord = ScoreManager.Instance.TrySaveHighScore();
        currentScoreText.text = "Your Score: " + ScoreManager.Instance.CurrentScore;
        highScoreText.text = "High Score: " + ScoreManager.Instance.HighScore;
        if (congratsText != null)
            congratsText.gameObject.SetActive(isNewRecord);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}