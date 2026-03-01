using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Hedef")]
    public Transform player;

    [Header("Kamera Ayarları")]
    public float followSpeed = 5f;        // Oyuncuyu takip hızı
    public float autoRiseSpeed = 1f;      // Otomatik yukarı çıkma hızı
    public float playerYOffset = 2f;      // Oyuncunun ekranda kaç birim üstte olacağı

    private float highestY;               // Kameranın inebileceği en düşük nokta

    void Start()
    {
        highestY = transform.position.y;
    }

    void LateUpdate()
    {
        float targetY = transform.position.y;

        // 1) Otomatik yukarı çıkma
        targetY += autoRiseSpeed * Time.deltaTime;

        // 2) Oyuncu kameranın üstüne çıkıyorsa onu takip et
        float playerTargetY = player.position.y + playerYOffset;
        if (playerTargetY > targetY)
        {
            targetY = Mathf.Lerp(transform.position.y, playerTargetY, followSpeed * Time.deltaTime);
        }

        // 3) En yüksek noktayı güncelle (kamera hiç aşağı inmeyecek)
        highestY = Mathf.Max(highestY, targetY);
        targetY = highestY;

        // 4) Pozisyonu uygula (X ve Z sabit kalsın)
        transform.position = new Vector3(
            transform.position.x,
            targetY,
            transform.position.z
        );
    }
}