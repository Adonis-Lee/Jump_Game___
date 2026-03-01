using UnityEngine;
using TMPro; // TextMeshPro kullanıyorsan bunu ekle

public class ScoreManager : MonoBehaviour
{
    public Transform player;       // Oyuncu referansı
    public TextMeshProUGUI scoreText; // Ekrandaki yazı referansı
    public float multiplier = 10f; // Yükseklik çarpanı (isteğine göre 10 yaptık)

    private float highestY = 0f;   // Ulaşılan en yüksek nokta

    void Update()
    {
        if (player == null) return;

        // Eğer oyuncunun şu anki yüksekliği, ulaşılan en yüksekten fazlaysa skoru güncelle
        // Bu sayede aşağı düşerken skor azalmaz.
        if (player.position.y > highestY)
        {
            highestY = player.position.y;
            UpdateScoreUI();
        }
    }

    void UpdateScoreUI()
    {
        // Skoru tam sayıya yuvarla ve ekrana yazdır
        int currentScore = Mathf.FloorToInt(highestY * multiplier);
        scoreText.text = "Skor: " + currentScore.ToString();
    }
}