using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Paneller")]
    [Tooltip("Options butonuna basılınca açılacak ses ayarları paneli")]
    public GameObject optionsPanel;

    [Tooltip("Characters butonuna basılınca açılacak karakter seçim paneli")]
    public GameObject charactersPanel;

    void Start()
    {
        // Oyun başladığında paneller kapalı olsun
        if (optionsPanel != null)
            optionsPanel.SetActive(false);
        if (charactersPanel != null)
            charactersPanel.SetActive(false);
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

    /// <summary>Characters butonuna tıklanınca çağrılır.</summary>
    public void OpenCharacters()
    {
        if (charactersPanel != null)
            charactersPanel.SetActive(true);
    }

    /// <summary>Karakter panelini kapatır.</summary>
    public void CloseCharacters()
    {
        if (charactersPanel != null)
            charactersPanel.SetActive(false);
    }

    public void Quit()
    {
        Application.Quit();
        Debug.Log("Player has been quit");
    }
}

