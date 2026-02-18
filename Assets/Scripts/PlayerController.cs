using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Vector2 moveInput;

    public BallController ball;
    public float shootPower = 6f;
    private Vector2 lastMoveDir = Vector2.right;

    public bool isActive = false;

    private GameManager gm;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        gm = FindFirstObjectByType<GameManager>();

        if (ball == null)
            ball = FindFirstObjectByType<BallController>();
    }
    

    void Update()
    {
        if (!isActive) return;
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        if (moveInput.sqrMagnitude > 0.01f)
            lastMoveDir = moveInput.normalized;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            TryShoot();
        }
    }

    void FixedUpdate()
    {
        rb.velocity = moveInput.normalized * moveSpeed;
    }

    void TryShoot()
    {
        if (ball == null || ball.rb == null) return;

        Rigidbody2D ballRb = ball.rb;

        float dist = Vector2.Distance(transform.position, ball.transform.position);
        if (dist >= 1.2f) return;

        Vector2 kickPos = (Vector2)transform.position + lastMoveDir * 0.6f;
        ballRb.position = kickPos;
        ballRb.velocity = Vector2.zero;
        ballRb.angularVelocity = 0f;
        ballRb.AddForce(lastMoveDir * shootPower, ForceMode2D.Impulse);
        
        

    }


    public void SetActive(bool active)
    {
        isActive = active;

        if (!active)
        {
            moveInput = Vector2.zero;
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        Debug.Log($"{name} active:{isActive} vel:{rb.velocity}");
    }




}


