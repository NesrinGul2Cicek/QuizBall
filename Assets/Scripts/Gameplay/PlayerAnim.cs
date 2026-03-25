/*using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerAnim : MonoBehaviour
{
    Animator anim;
    Rigidbody2D rb;
    SpriteRenderer sr;

    [Header("Tuning")]
    public float deadzone = 0.05f;     // çok küçük titremeleri yok say
    public bool fourWayOnly = true;

    // Son yönü korumak için
    Vector2 lastDir = Vector2.down;

    void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        Vector2 v = rb.velocity;

        // Deadzone
        if (v.magnitude < deadzone) v = Vector2.zero;

        Vector2 dir = v;

        if (fourWayOnly && dir != Vector2.zero)
        {
            float ax = Mathf.Abs(dir.x);
            float ay = Mathf.Abs(dir.y);

            if (ax > ay) dir = new Vector2(Mathf.Sign(dir.x), 0f);
            else dir = new Vector2(0f, Mathf.Sign(dir.y));
        }
        else if (dir != Vector2.zero)
        {
            dir.Normalize();
        }

        bool moving = dir != Vector2.zero;
        anim.SetBool("Moving", moving);

        if (moving)
        {
            lastDir = dir;
            anim.SetFloat("MoveX", dir.x);
            anim.SetFloat("MoveY", dir.y);
        }
        else
        {
            // Idle yönü koru
            anim.SetFloat("MoveX", lastDir.x);
            anim.SetFloat("MoveY", lastDir.y);
        }

        // Flip (sað/sol için tek sprite setin varsa)
        if (sr != null)
        {
            if (lastDir.x < 0f) sr.flipX = true;
            else if (lastDir.x > 0f) sr.flipX = false;
        }
    }
}*/

using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerAnim : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rb;
    private SpriteRenderer sr;

    [Header("Tuning")]
    public float deadzone = 0.08f;
    public bool fourWayOnly = true;

    [Header("Anti-Jitter")]
    public float directionChangeThreshold = 0.15f;

    private Vector2 lastDir = Vector2.down;
    private Vector2 cachedVelocity;

    void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // Velocity'yi tek sefer alýp cache'liyoruz
        cachedVelocity = rb.velocity;
    }

    void LateUpdate()
    {
        Vector2 v = cachedVelocity;

        // Çok küçük fizik kalýntýlarýný yok say
        if (v.magnitude < deadzone)
            v = Vector2.zero;

        Vector2 dir = v;

        if (dir != Vector2.zero)
            dir.Normalize();

        if (fourWayOnly && dir != Vector2.zero)
        {
            float ax = Mathf.Abs(dir.x);
            float ay = Mathf.Abs(dir.y);

            // Eksenler birbirine çok yakýnsa eski yönü koru
            if (Mathf.Abs(ax - ay) < directionChangeThreshold)
            {
                dir = lastDir;
            }
            else if (ax > ay)
            {
                dir = new Vector2(Mathf.Sign(dir.x), 0f);
            }
            else
            {
                dir = new Vector2(0f, Mathf.Sign(dir.y));
            }
        }

        bool moving = v != Vector2.zero;
        anim.SetBool("Moving", moving);

        if (moving)
        {
            lastDir = dir;
        }

        anim.SetFloat("MoveX", lastDir.x);
        anim.SetFloat("MoveY", lastDir.y);

        if (sr != null)
        {
            if (lastDir.x < 0f) sr.flipX = true;
            else if (lastDir.x > 0f) sr.flipX = false;
        }
    }
}