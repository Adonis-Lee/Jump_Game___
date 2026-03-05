using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class PauseManager : MonoBehaviour
{
    public GameObject pausePanel;
    public TextMeshProUGUI countdownText;

    private bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    void TogglePause()
    {
        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            Time.timeScale = 0f;
            pausePanel.SetActive(true);
            isPaused = true;
        }
    }

    public void ResumeGame()
    {
        pausePanel.SetActive(false);
        StartCoroutine(Countdown());
    }

    IEnumerator Countdown()
    {
        countdownText.gameObject.SetActive(true);

        for (int i = 3; i > 0; i--)
        {
            countdownText.text = i.ToString();
            yield return new WaitForSecondsRealtime(1f);
        }

        countdownText.text = "GO!";
        yield return new WaitForSecondsRealtime(0.5f);

        countdownText.gameObject.SetActive(false);

        Time.timeScale = 1f;
        isPaused = false;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void Options()
    {

    }
}