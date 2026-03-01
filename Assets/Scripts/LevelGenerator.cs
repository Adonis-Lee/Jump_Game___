using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [Header("Gerekli Referanslar")]
    public GameObject platformPrefab;
    public GameObject wallPrefab;
    public Transform player;

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

    [Header("Duvar Ayarları")]
    public float wallLeftX = -5f;
    public float wallRightX = 5f;
    public float wallSegmentHeight = 25f;
    public float wallThickness = 1f;
    public float wallSpawnBuffer = 5f;

    private Vector3 nextSpawnPosition;
    private List<GameObject> activePlatforms = new List<GameObject>();
    private float nextWallY = -2f;
    private List<GameObject> activeWalls = new List<GameObject>();
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

        if (wallPrefab != null) SpawnWallSegment();
    }

    void Update()
    {
        if (Vector2.Distance(player.position, nextSpawnPosition) < spawnDistance)
        {
            SpawnPlatform();
        }

        if (wallPrefab != null && player.position.y + wallSpawnBuffer > nextWallY)
        {
            SpawnWallSegment();
        }

        RemoveOldPlatforms();
        RemoveOldWalls();
    }

    void SpawnPlatform()
    {
        // 1. Rastgele X konumu
        float randomX = Random.Range(minX, maxX);
        Vector3 spawnPos = new Vector3(randomX, nextSpawnPosition.y, 0);

        // 2. Oluştur
        GameObject newPlatform = Instantiate(platformPrefab, spawnPos, Quaternion.identity);

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

        Vector3 leftPos = new Vector3(wallLeftX, centerY, 0);
        GameObject leftWall = Instantiate(wallPrefab, leftPos, Quaternion.identity);
        leftWall.transform.localScale = new Vector3(wallThickness, wallSegmentHeight, 1f);
        ApplyFrictionlessMaterial(leftWall);
        activeWalls.Add(leftWall);

        Vector3 rightPos = new Vector3(wallRightX, centerY, 0);
        GameObject rightWall = Instantiate(wallPrefab, rightPos, Quaternion.identity);
        rightWall.transform.localScale = new Vector3(wallThickness, wallSegmentHeight, 1f);
        ApplyFrictionlessMaterial(rightWall);
        activeWalls.Add(rightWall);

        nextWallY += wallSegmentHeight;
    }

    void ApplyFrictionlessMaterial(GameObject wall)
    {
        var col = wall.GetComponent<BoxCollider2D>();
        if (col != null) col.sharedMaterial = frictionlessMaterial;
    }

    void RemoveOldWalls()
    {
        float removeThreshold = wallSegmentHeight + 10f;
        for (int i = activeWalls.Count - 1; i >= 0; i--)
        {
            if (player.position.y - activeWalls[i].transform.position.y > removeThreshold)
            {
                Destroy(activeWalls[i]);
                activeWalls.RemoveAt(i);
            }
        }
    }
}