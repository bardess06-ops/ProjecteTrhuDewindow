using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class Player : MonoBehaviour
{
    // [Variables existents de moviment...]
    public float speed = 5.0f;
    private Rigidbody2D rb2D;
    private float move;
    public float jumpForce = 4.0f;
    private bool isGrounded;
    public Transform groundCheck;
    public float groundRadious = 0.1f;
    public LayerMask groundLayer;
    private Animator animator;
    public TextMeshProUGUI WinText;

    // [Variables per plataformes]
    private Collider2D playerCollider;
    private GameObject currentPlatform;
    private float ignorePlatformTime = 0f;

    // [Variables per empemta]
    private bool isBeingPushedToPosition = false;
    private Vector2 pushTargetPosition;
    private float pushPositionTime;
    private float pushPositionTimer = 0f;
    private Vector2 pushStartPosition;

    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerCollider = GetComponent<Collider2D>();

        if (WinText != null)
            WinText.gameObject.SetActive(false);
    }

    void Update()
    {
        // Moviment horitzontal
        move = Input.GetAxisRaw("Horizontal");
        rb2D.linearVelocity = new Vector2(move * speed, rb2D.linearVelocity.y);

        if (move != 0)
        {
            transform.localScale = new Vector3(Mathf.Sign(move), 1, 1);
        }

        if (move != 0)
        {
            animator.SetBool("isRunning", true);
        }

        else
        {
            animator.SetBool("isRunning", false);
        }

        // Salt - PERMET TRAVESSAR PLATAFORMES CAP AMUNT
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb2D.linearVelocity = new Vector2(rb2D.linearVelocity.x, jumpForce);

            // Ignorar TOTES les plataformes quan saltem (per pujar)
            IgnoreAllPlatforms(true);
            ignorePlatformTime = 0.4f;
        }

        // Baixar de plataforma - NOMÉS la plataforma actual
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            if (isGrounded && currentPlatform != null && currentPlatform.CompareTag("Platform"))
            {
                // Ignorar NOMÉS la plataforma on estem actualment
                Physics2D.IgnoreCollision(playerCollider, currentPlatform.GetComponent<Collider2D>(), true);
                ignorePlatformTime = 0.3f;

                // Programar per reactivar aquesta plataforma específica
                Invoke(() => ReactivateSpecificPlatform(currentPlatform), 0.5f);
            }
        }

        // Gestionar el temps d'ignorar plataformes (per salt)
        if (ignorePlatformTime > 0)
        {
            ignorePlatformTime -= Time.deltaTime;
            if (ignorePlatformTime <= 0 && !Input.GetKey(KeyCode.DownArrow) && !Input.GetKey(KeyCode.S))
            {
                // Reactivar totes les plataformes (només per salt)
                IgnoreAllPlatforms(false);
            }
        }

        animator.SetFloat("Speed", Mathf.Abs(move));
        animator.SetFloat("VerticalVelocity", rb2D.linearVelocity.y);
        animator.SetBool("IsGrounded", isGrounded);

        // Gestió de l'empenta a posició
        if (isBeingPushedToPosition)
        {
            pushPositionTimer += Time.deltaTime;

            // Interpolació suau cap a la posició objectiu
            float progress = pushPositionTimer / pushPositionTime;
            Vector2 newPosition = Vector2.Lerp(pushStartPosition, pushTargetPosition, progress);

            rb2D.linearVelocity = Vector2.zero; // Aturar qualsevol moviment
            transform.position = newPosition; // Moure directament

            if (progress >= 1f)
            {
                isBeingPushedToPosition = false;
            }
            return; // Sortir del Update

        }
    }

    // Mètode per empenta a posició específica
    public void ApplyPushToPosition(Vector2 targetPosition, float duration)
    {
        isBeingPushedToPosition = true;
        pushTargetPosition = targetPosition;
        pushPositionTime = duration;
        pushPositionTimer = 0f;
        pushStartPosition = transform.position;

        Debug.Log($"Empenta de {transform.position.x} a {targetPosition.x}");
    }

    private void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadious, groundLayer);
    }

    // Ignorar o activar TOTES les plataformes (per saltar)
    private void IgnoreAllPlatforms(bool ignore)
    {
        GameObject[] platforms = GameObject.FindGameObjectsWithTag("Platform");
        foreach (GameObject platform in platforms)
        {
            Collider2D platformCollider = platform.GetComponent<Collider2D>();
            if (platformCollider != null)
            {
                Physics2D.IgnoreCollision(playerCollider, platformCollider, ignore);
            }
        }
    }

    // Reactivar una plataforma específica
    private void ReactivateSpecificPlatform(GameObject platform)
    {
        if (platform != null)
        {
            Collider2D platformCollider = platform.GetComponent<Collider2D>();
            if (platformCollider != null)
            {
                Physics2D.IgnoreCollision(playerCollider, platformCollider, false);
            }
        }
    }

    // Detectar la plataforma actual on estem
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
            // Comprovar si estem SOBRE la plataforma
            if (IsOnTopOfPlatform(collision))
            {
                currentPlatform = collision.gameObject;
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
            if (IsOnTopOfPlatform(collision))
            {
                currentPlatform = collision.gameObject;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject == currentPlatform)
        {
            currentPlatform = null;
        }
    }

    private bool IsOnTopOfPlatform(Collision2D collision)
    {
        float playerBottom = playerCollider.bounds.min.y;
        float platformTop = collision.collider.bounds.max.y;
        return Mathf.Abs(playerBottom - platformTop) < 0.2f;
    }

    public void Death()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        SceneManager.LoadScene("DeathScene");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("DeathZone"))
        {
            Death();
        }

        if (collision.CompareTag("Finish"))
        {
            if (WinText != null)
            {
                WinText.gameObject.SetActive(true);
            }
        }
    }

    // DEBUG
    private void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundRadious);
        }

        if (currentPlatform != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(currentPlatform.transform.position,
                               currentPlatform.GetComponent<Collider2D>().bounds.size);
        }
    }

    // Mètode auxiliar per Invoke amb paràmetres
    private void Invoke(System.Action action, float time)
    {
        StartCoroutine(InvokeRoutine(action, time));
    }

    private System.Collections.IEnumerator InvokeRoutine(System.Action action, float time)
    {
        yield return new WaitForSeconds(time);
        action?.Invoke();
    }
}