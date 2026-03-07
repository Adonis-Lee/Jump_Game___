using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Paneller")]
    [Tooltip("Options butonuna basılınca açılacak ses ayarları paneli")]
    public GameObject optionsPanel;

    void Start()
    {
        // Oyun başladığında panel kapalı olsun
        if (optionsPanel != null)
            optionsPanel.SetActive(false);
    }

    public void Play()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    /// <summary>Options butonuna tıklanınca çağrılır.</summary>
    public void OpenOptions()
    {
        if (optionsPanel != null)
            optionsPanel.SetActive(true);
    }

    /// <summary>Geri butonuna tıklanınca çağrılır.</summary>
    public void CloseOptions()
    {
        if (optionsPanel != null)
            optionsPanel.SetActive(false);
    }

    public void Quit()
    {
        Application.Quit();
        Debug.Log("Player has been quit");
    }
}

