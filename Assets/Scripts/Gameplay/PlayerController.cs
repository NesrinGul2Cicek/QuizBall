using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Control")]
    public bool isHuman = true;

    [Header("Movement")]
    public float moveSpeed = 4.5f;

    [Header("Kick Settings")]
    public float shortPassForce = 6f;
    public float longPassForce = 10f;
    public float shootForce = 13f;
    public float failForceMultiplier = 0.45f;

    private BallController ball;
    private Camera mainCam;
    private Rigidbody2D rb;

    private Vector2 moveInput;

    public bool isSelected = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        ball = FindFirstObjectByType<BallController>();
        mainCam = Camera.main;
    }

    void Update()
    {
        if (GameManager.Instance == null) return;

        HandleMovementInput();

        if (!isHuman) return;
        if (!isSelected) return;
        if (!GameManager.Instance.CanAcceptInput()) return;
        if (!HasBall()) return;

        if (Input.GetKeyDown(KeyCode.J))
            TryShortPass();

        if (Input.GetKeyDown(KeyCode.K))
            TryLongPass();

        if (Input.GetKeyDown(KeyCode.L))
            TryShoot();
    }

    void FixedUpdate()
    {
        if (rb == null) return;

        if (!GameManager.Instance || !GameManager.Instance.CanAcceptInput())
        {
            rb.velocity = Vector2.zero;
            return;
        }

        if (moveInput.sqrMagnitude < 0.0001f)
            rb.velocity = Vector2.zero;
        else
            rb.velocity = moveInput.normalized * moveSpeed;
    }

    void HandleMovementInput()
    {
        if (!isHuman) return;

        // Seçili değilse hareket almasın
        if (!isSelected)
        {
            moveInput = Vector2.zero;
            return;
        }

        float x = 0f;
        float y = 0f;

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) x = -1f;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) x = 1f;
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) y = -1f;
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) y = 1f;

        moveInput = new Vector2(x, y).normalized;
    }

    public void SetMoveInput(Vector2 input)
    {
        if (isHuman) return;
        moveInput = Vector2.ClampMagnitude(input, 1f);
    }

    public Vector2 GetMoveInput()
    {
        return moveInput;
    }

    public void TryShortPass()
    {
        Vector2 target = GetAimPoint();
        GameManager.Instance.RequestAction(ActionType.ShortPass, this, target);
    }

    public void TryLongPass()
    {
        Vector2 target = GetAimPoint();
        GameManager.Instance.RequestAction(ActionType.LongPass, this, target);
    }

    public void TryShoot()
    {
        Vector2 target = GetAimPoint();
        GameManager.Instance.RequestAction(ActionType.Shoot, this, target);
    }

    public void ExecuteShortPass(Vector2 targetPoint)
    {
        KickTowards(targetPoint, shortPassForce);
    }

    public void ExecuteLongPass(Vector2 targetPoint)
    {
        KickTowards(targetPoint, longPassForce);
    }

    public void ExecuteShoot(Vector2 targetPoint)
    {
        KickTowards(targetPoint, shootForce);
    }

    public void ExecuteFailedPass(Vector2 targetPoint)
    {
        Vector2 wrongTarget = AddRandomError(targetPoint, 2f);
        KickTowards(wrongTarget, shortPassForce * failForceMultiplier);
    }

    public void ExecuteFailedLongPass(Vector2 targetPoint)
    {
        Vector2 wrongTarget = AddRandomError(targetPoint, 3f);
        KickTowards(wrongTarget, longPassForce * failForceMultiplier);
    }

    public void ExecuteFailedShoot(Vector2 targetPoint)
    {
        Vector2 wrongTarget = AddRandomError(targetPoint, 2.5f);
        KickTowards(wrongTarget, shootForce * failForceMultiplier);
    }

    private void KickTowards(Vector2 targetPoint, float force)
    {
        if (ball == null) return;
        if (!HasBall()) return;

        Vector2 dir = (targetPoint - (Vector2)transform.position).normalized;

        if (dir.sqrMagnitude < 0.001f)
            dir = Vector2.right;

        rb.velocity = Vector2.zero;
        ball.Kick(dir * force);
    }

    private Vector2 AddRandomError(Vector2 originalTarget, float radius)
    {
        Vector2 randomOffset = Random.insideUnitCircle * radius;
        return originalTarget + randomOffset;
    }

    private Vector2 GetAimPoint()
    {
        if (mainCam == null)
            mainCam = Camera.main;

        if (mainCam == null)
            return (Vector2)transform.position + Vector2.right;

        Vector3 mouseWorld = mainCam.ScreenToWorldPoint(Input.mousePosition);
        return new Vector2(mouseWorld.x, mouseWorld.y);
    }

    public bool HasBall()
    {
        return ball != null && ball.CurrentOwner == this;
    }
}