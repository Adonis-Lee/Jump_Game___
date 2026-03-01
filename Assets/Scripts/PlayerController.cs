using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Hareket Ayarları")]
    public float moveSpeed = 8f;
    public float jumpForce = 12f;

    [Header("Hareket Yumuşatma")]
    public float acceleration = 15f;
    public float deceleration = 20f;
    public float airControl = 0.6f;

    [Header("Duvar Zıplama Yeteneği (Skill)")]
    public float wallJumpCooldown = 5f;
    public float wallJumpPushForce = 5f;
    public float wallJumpTime = 0.2f;
    private float nextWallJumpTime = 0f;
    private bool isWallJumping;

    [Header("Zemin Kontrolü (Ayaklar)")]
    public Transform groundCheckLeft;
    public Transform groundCheckRight;

    [Header("Duvar Kontrolü (Gövde/Omuzlar)")]
    public Transform wallCheckLeft;
    public Transform wallCheckRight;

    public float checkRadius = 0.2f;
    public LayerMask groundLayer;
    public LayerMask wallLayer;

    private Rigidbody2D rb;
    private float horizontalInput;
    private bool isGrounded;
    private bool isTouchingWall;
    private bool isTouchingLeftWall;
    private bool isTouchingRightWall;

    [Header("UI")]
    public GameObject gameOverPanel;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");

        // ZIPLAMA TUŞU (W)
        if (Input.GetKeyDown(KeyCode.W))
        {
            // 1. Durum: Yerdeysek Normal Zıpla
            if (isGrounded)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            }
            // 2. Durum: Havada, Duvara Değiyorsak ve Süre Dolduysa (Skill)
            else if (isTouchingWall && Time.time >= nextWallJumpTime)
            {
                PerformWallJump();
            }
        }

        // ÖLÜM KONTROLÜ
        Camera cam = Camera.main;
        float camBottomY = cam.transform.position.y - cam.orthographicSize;

        if (transform.position.y < camBottomY)
        {
            Die();
        }
    }

    void Die()
    {
        gameOverPanel.SetActive(true);
    }

    void PerformWallJump()
    {
        bool leftSensor = Physics2D.OverlapCircle(wallCheckLeft.position, checkRadius, wallLayer);
        bool rightSensor = Physics2D.OverlapCircle(wallCheckRight.position, checkRadius, wallLayer);

        float pushX = 0f;
        if (leftSensor)
        {
            pushX = transform.position.x > wallCheckLeft.position.x ? wallJumpPushForce : -wallJumpPushForce;
            Debug.Log($"[WallJump] Sol sensör duvar algıladı, {(pushX > 0 ? "sağa" : "sola")} itiliyor. (pushX={pushX})");
        }
        else if (rightSensor)
        {
            pushX = transform.position.x > wallCheckRight.position.x ? wallJumpPushForce : -wallJumpPushForce;
            Debug.Log($"[WallJump] Sağ sensör duvar algıladı, {(pushX > 0 ? "sağa" : "sola")} itiliyor. (pushX={pushX})");
        }

        Vector2 appliedForce = new Vector2(pushX, jumpForce * 2f);
        rb.linearVelocity = appliedForce;

        isWallJumping = true;
        Invoke(nameof(EndWallJump), wallJumpTime);

        nextWallJumpTime = Time.time + wallJumpCooldown;

        Debug.Log($"[WallJump] Uygulanan vektör: {appliedForce} | isWallJumping=AKTİF ({wallJumpTime}s)");
    }

    void EndWallJump()
    {
        isWallJumping = false;
        Debug.Log("[WallJump] isWallJumping=KAPALI - Yatay kontrol serbest.");
    }

    void FixedUpdate()
    {
        // --- ZEMİN KONTROLÜ ---
        // Yukarı çıkarken (zıplarken) zemin kontrolünü iptal et (Bug önleyici)
        if (rb.linearVelocity.y > 0.1f)
        {
            isGrounded = false;
        }
        else
        {
            bool leftStep = Physics2D.OverlapCircle(groundCheckLeft.position, checkRadius, groundLayer);
            bool rightStep = Physics2D.OverlapCircle(groundCheckRight.position, checkRadius, groundLayer);
            isGrounded = leftStep || rightStep;
        }

        // --- DUVAR KONTROLÜ ---
        isTouchingLeftWall = Physics2D.OverlapCircle(wallCheckLeft.position, checkRadius, wallLayer);
        isTouchingRightWall = Physics2D.OverlapCircle(wallCheckRight.position, checkRadius, wallLayer);
        isTouchingWall = isTouchingLeftWall || isTouchingRightWall;

        // --- HAREKET --- (duvardan sekiyorsak input yok sayılır)
        if (!isWallJumping)
        {
            float targetSpeed = horizontalInput * moveSpeed;
            float currentSpeed = rb.linearVelocity.x;
            float speedDiff = targetSpeed - currentSpeed;

            // Havada mı yerde mi? (hava kontrolü daha az)
            float controlMultiplier = isGrounded ? 1f : airControl;

            // Hızlanıyor muyuz yoksa fren mi yapıyoruz?
            float rate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : deceleration;

            float force = speedDiff * rate * controlMultiplier;
            rb.AddForce(new Vector2(force, 0), ForceMode2D.Force);

            // Sprite çevirme
            if (horizontalInput > 0)
                transform.localScale = new Vector3(1, 1, 1);
            else if (horizontalInput < 0)
                transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    void OnDrawGizmos()
    {
        // 1. AYAKLARI GÖRSELLEŞTİR (Zemin Kontrolü)
        if (groundCheckLeft != null && groundCheckRight != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheckLeft.position, checkRadius);
            Gizmos.DrawWireSphere(groundCheckRight.position, checkRadius);
        }

        // 2. OMUZLARI GÖRSELLEŞTİR (Duvar Kontrolü)
        if (wallCheckLeft != null && wallCheckRight != null)
        {
            Gizmos.color = isTouchingWall ? Color.blue : Color.yellow;
            Gizmos.DrawWireSphere(wallCheckLeft.position, checkRadius);
            Gizmos.DrawWireSphere(wallCheckRight.position, checkRadius);
        }
    }
}