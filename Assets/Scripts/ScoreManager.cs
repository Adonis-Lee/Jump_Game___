using UnityEngine;
using TMPro; // TextMeshPro kullanıyorsan bunu ekle

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    public Transform player;       // Oyuncu referansı
    public TextMeshProUGUI scoreText; // Ekrandaki yazı referansı
    public float multiplier = 10f; // Yükseklik çarpanı (isteğine göre 10 yaptık)

    private float highestY = float.MinValue;   // Ulaşılan en yüksek nokta

    public int CurrentScore { get; private set; }
    public int HighScore { get; private set; }

    void Awake()
    {
        // Bu kod satırını skoru sıfırlamak için ekledim isteyen bi kez açıp tekrar yoruma alsın
        //PlayerPrefs.DeleteAll();
        
        Instance = this;
        HighScore = PlayerPrefs.GetInt("HighScore", 0);
        
        
    }

    void Update()
    {
        if (player == null) return;

        // Eğer oyuncunun şu anki yüksekliği, ulaşılan en yüksekten fazlaysa skoru güncelle
        // Bu sayede aşağı düşerken skor azalmaz.
        if (player.position.y > highestY)
        {
            highestY = player.position.y;
            CurrentScore = Mathf.FloorToInt(highestY * multiplier);
            UpdateScoreUI();
        }
    }

    void UpdateScoreUI()
    {
        // Skoru tam sayıya yuvarla ve ekrana yazdır
        int currentScore = Mathf.FloorToInt(highestY * multiplier);
        scoreText.text = "Skor: " + currentScore.ToString();
    }

    public bool TrySaveHighScore()
    {
        if (CurrentScore > HighScore)
        {
            HighScore = CurrentScore;
            PlayerPrefs.SetInt("HighScore", HighScore);
            PlayerPrefs.Save();
            return true; // Yeni rekor!
        }
        return false;
    }
}