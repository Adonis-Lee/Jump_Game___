using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [Header("Gerekli Referanslar")]
    [Tooltip("6 basamak görseli - spawn sırasında rastgele biri seçilir")]
    public GameObject[] platformPrefabs;
    public Transform player;

    [Header("Duvar Prefabları (Sol/Sağ farklı görünüm)")]
    public GameObject wallLeftPrefab;
    public GameObject wallRightPrefab;

    [Header("Background")]
    public GameObject backgroundPrefab;

    [Header("Spawn Ayarları")]
    public int initialSpawnCount = 10;
    public float distanceBetween = 3.0f;
    public float spawnDistance = 20f;

    [Header("Rastgelelik Ayarları (Genişlik & Yükseklik)")]
    public float minX = -2.5f;
    public float maxX = 2.5f;

    // Genişlik (Sağ-Sol uzunluk)
    public float minWidth = 0.5f;
    public float maxWidth = 3.0f;

    // Yükseklik (Kalınlık) - İsteğin üzerine eklendi
    public float minHeight = 0.2f;
    public float maxHeight = 1.0f;

    [Header("Duvar Ayarları (Scale bizim ayar)")]
    [Tooltip("Sol duvar merkezinin X pozisyonu.")]
    public float wallLeftX = -5f;
    [Tooltip("Sağ duvar merkezinin X pozisyonu.")]
    public float wallRightX = 5f;
    [Tooltip("Duvar segmenti scale X (eni/kalınlık).")]
    public float wallSegmentWidth = 1f;
    [Tooltip("Duvar segmenti scale Y (boyu).")]
    public float wallSegmentHeight = 25f;
    [Tooltip("İki duvar cismi arasındaki fark: 0=bitişik, + = boşluk, - = örtüşme.")]
    public float wallSpacingBetweenSegments = 0f;
    public float wallSpawnBuffer = 5f;

    [Header("Background Ayarları (Scale bizim ayar)")]
    [Tooltip("Arka plan segmenti scale X (eni).")]
    public float backgroundSegmentWidth = 4.5f;
    [Tooltip("Arka plan segmenti scale Y (boyu).")]
    public float backgroundSegmentHeight = 21.4f;
    [Tooltip("İki arka plan cismi arasındaki fark: 0=bitişik, + = arada boşluk, - = örtüşme.")]
    public float backgroundSpacingBetweenSegments = 0f;
    [Tooltip("Arka plan merkezinin X pozisyonu.")]
    public float backgroundCenterX = 0f;
    public float backgroundSpawnBuffer = 5f;

    private Vector3 nextSpawnPosition;
    private List<GameObject> activePlatforms = new List<GameObject>();
    private float nextWallY = -2f;
    private List<GameObject> activeWalls = new List<GameObject>();
    private float nextBackgroundY = -2f;
    private List<GameObject> activeBackgrounds = new List<GameObject>();
    private static PhysicsMaterial2D frictionlessMaterial;

    void Awake()
    {
        if (frictionlessMaterial == null)
            frictionlessMaterial = new PhysicsMaterial2D("WallFrictionless") { friction = 0f, bounciness = 0f };
    }

    void Start()
    {
        nextSpawnPosition = new Vector3(0, -2f, 0);

        for (int i = 0; i < initialSpawnCount; i++)
        {
            SpawnPlatform();
        }

        if (wallLeftPrefab != null || wallRightPrefab != null) SpawnWallSegment();
        if (backgroundPrefab != null) SpawnBackgroundSegment();
    }

    void Update()
    {
        if (Vector2.Distance(player.position, nextSpawnPosition) < spawnDistance)
        {
            SpawnPlatform();
        }

        if ((wallLeftPrefab != null || wallRightPrefab != null) && player.position.y + wallSpawnBuffer > nextWallY)
            SpawnWallSegment();

        if (backgroundPrefab != null && player.position.y + backgroundSpawnBuffer > nextBackgroundY)
            SpawnBackgroundSegment();

        RemoveOldPlatforms();
        RemoveOldWalls();
        RemoveOldBackgrounds();
    }

    void SpawnPlatform()
    {
        if (platformPrefabs == null || platformPrefabs.Length == 0) return;

        float randomX = Random.Range(minX, maxX);
        Vector3 spawnPos = new Vector3(randomX, nextSpawnPosition.y, 0);

        GameObject prefab = platformPrefabs[Random.Range(0, platformPrefabs.Length)];
        if (prefab == null) return;
        GameObject newPlatform = Instantiate(prefab, spawnPos, Quaternion.identity);

        // 3. Rastgele Genişlik (X) VE Rastgele Yükseklik (Y) ayarla
        float randomWidth = Random.Range(minWidth, maxWidth);
        float randomHeight = Random.Range(minHeight, maxHeight); // Yeni eklenen kısım

        // Z ekseni 2D oyunda genelde 1 kalır, X ve Y değişti
        newPlatform.transform.localScale = new Vector3(randomWidth, randomHeight, 1f);

        // 4. Listeye ekle
        activePlatforms.Add(newPlatform);

        // 5. Sonraki pozisyonu güncelle
        nextSpawnPosition.y += distanceBetween;
    }

    void RemoveOldPlatforms()
    {
        if (activePlatforms.Count > 0)
        {
            GameObject oldestPlatform = activePlatforms[0];

            if (player.position.y - oldestPlatform.transform.position.y > 15f)
            {
                Destroy(oldestPlatform);
                activePlatforms.RemoveAt(0);
            }
        }
    }

    void SpawnWallSegment()
    {
        float centerY = nextWallY + wallSegmentHeight * 0.5f;
        Vector3 scale = new Vector3(wallSegmentWidth, wallSegmentHeight, 1f);

        if (wallLeftPrefab != null)
        {
            GameObject leftWall = Instantiate(wallLeftPrefab, new Vector3(wallLeftX, centerY, 0f), Quaternion.identity);
            leftWall.transform.localScale = scale;
            ApplyFrictionlessMaterial(leftWall);
            activeWalls.Add(leftWall);
        }

        if (wallRightPrefab != null)
        {
            GameObject rightWall = Instantiate(wallRightPrefab, new Vector3(wallRightX, centerY, 0f), Quaternion.identity);
            rightWall.transform.localScale = scale;
            ApplyFrictionlessMaterial(rightWall);
            activeWalls.Add(rightWall);
        }

        nextWallY += wallSegmentHeight + wallSpacingBetweenSegments;
    }

    void ApplyFrictionlessMaterial(GameObject wall)
    {
        var col = wall.GetComponent<BoxCollider2D>();
        if (col != null) col.sharedMaterial = frictionlessMaterial;
    }

    void RemoveOldWalls()
    {
        float removeThreshold = Mathf.Abs(wallSegmentHeight + wallSpacingBetweenSegments) + 10f;
        for (int i = activeWalls.Count - 1; i >= 0; i--)
        {
            if (player.position.y - activeWalls[i].transform.position.y > removeThreshold)
            {
                Destroy(activeWalls[i]);
                activeWalls.RemoveAt(i);
            }
        }
    }

    void SpawnBackgroundSegment()
    {
        float centerY = nextBackgroundY + backgroundSegmentHeight * 0.5f;
        Vector3 pos = new Vector3(backgroundCenterX, centerY, 0f);
        GameObject segment = Instantiate(backgroundPrefab, pos, Quaternion.identity);
        segment.transform.localScale = new Vector3(backgroundSegmentWidth, backgroundSegmentHeight, 1f);
        activeBackgrounds.Add(segment);
        nextBackgroundY += backgroundSegmentHeight + backgroundSpacingBetweenSegments;
    }

    void RemoveOldBackgrounds()
    {
        float removeThreshold = Mathf.Abs(backgroundSegmentHeight + backgroundSpacingBetweenSegments) + 10f;
        for (int i = activeBackgrounds.Count - 1; i >= 0; i--)
        {
            if (player.position.y - activeBackgrounds[i].transform.position.y > removeThreshold)
            {
                Destroy(activeBackgrounds[i]);
                activeBackgrounds.RemoveAt(i);
            }
        }
    }
}