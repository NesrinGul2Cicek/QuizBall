using UnityEngine;
using System.Collections;

public class BallController : MonoBehaviour
{
    public Rigidbody2D rb;

    [Header("Possession Follow")]
    public bool followOwner = true;
    public Vector2 holdOffset = new Vector2(0.45f, 0f);

    [Header("Arc Visual (Child)")]
    public Transform visual;
    public float passArcHeight = 0.25f;
    public float shootArcHeight = 0.35f;
    public float arcDurationMin = 0.20f;
    public float arcDurationMax = 0.70f;

    private Coroutine arcCo;

    public PlayerController CurrentOwner { get; private set; }

    void Awake()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (CurrentOwner == null && rb.simulated && rb.velocity.sqrMagnitude < 0.02f)
            StopArc();
    }

    void LateUpdate()
    {
        if (!followOwner || CurrentOwner == null)
            return;

        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;

        Vector2 dir = Vector2.right;

        if (CurrentOwner.GetMoveInput().sqrMagnitude > 0.001f)
            dir = CurrentOwner.GetMoveInput().normalized;

        transform.position = (Vector2)CurrentOwner.transform.position + dir * holdOffset.magnitude;
    }

    public void SetOwner(PlayerController player)
    {
        CurrentOwner = player;

        if (CurrentOwner != null)
        {
            StopArc();
            followOwner = true;
            rb.simulated = false;
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;

            Vector2 dir = Vector2.right;
            if (CurrentOwner.GetMoveInput().sqrMagnitude > 0.001f)
                dir = CurrentOwner.GetMoveInput().normalized;

            transform.position = (Vector2)CurrentOwner.transform.position + dir * holdOffset.magnitude;
        }
        else
        {
            followOwner = false;
            rb.simulated = true;
        }
    }

    public void ReleaseFromOwner()
    {
        CurrentOwner = null;
        followOwner = false;
        rb.simulated = true;
    }

    public void ReleaseBall()
    {
        ReleaseFromOwner();
    }

    public void Kick(Vector2 velocity)
    {
        ReleaseFromOwner();
        rb.velocity = velocity;
    }

    public void PassTo(Transform target)
    {
        if (target == null) return;

        ReleaseFromOwner();

        Vector2 dir = ((Vector2)target.position - rb.position).normalized;
        rb.velocity = dir * 8f;

        StartArc(passArcHeight, target.position);
    }

    public void Shoot(Vector2 targetPos)
    {
        ReleaseFromOwner();

        Vector2 dir = (targetPos - rb.position).normalized;
        rb.velocity = dir * 12f;

        StartArc(shootArcHeight, targetPos);
    }

    void StartArc(float height, Vector2 targetPos)
    {
        if (visual == null) return;

        float dist = Vector2.Distance(rb.position, targetPos);
        float t01 = Mathf.Clamp01(dist / 6f);
        float duration = Mathf.Lerp(arcDurationMin, arcDurationMax, t01);

        StopArc();
        arcCo = StartCoroutine(ArcRoutine(height, duration));
    }

    IEnumerator ArcRoutine(float height, float duration)
    {
        float t = 0f;
        Vector3 baseLocal = visual.localPosition;

        while (t < duration)
        {
            t += Time.deltaTime;
            float x = Mathf.Clamp01(t / duration);
            float h = 4f * height * x * (1f - x);

            Vector3 p = baseLocal;
            p.y = baseLocal.y + h;
            visual.localPosition = p;

            yield return null;
        }

        Vector3 end = baseLocal;
        end.y = baseLocal.y;
        visual.localPosition = end;
        arcCo = null;
    }

    void StopArc()
    {
        if (arcCo != null)
        {
            StopCoroutine(arcCo);
            arcCo = null;
        }

        if (visual != null)
        {
            Vector3 p = visual.localPosition;
            p.y = 0f;
            visual.localPosition = p;
        }
    }

    public void ResetBall(Vector2 position)
    {
        CurrentOwner = null;
        followOwner = false;

        rb.simulated = false;
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;

        transform.position = position;
        StopArc();

        rb.simulated = true;
    }

    public void ResetBall(Transform restartPoint)
    {
        if (restartPoint == null) return;
        ResetBall((Vector2)restartPoint.position);
    }

    public Transform GetOwner()
    {
        return CurrentOwner != null ? CurrentOwner.transform : null;
    }

    public bool IsOwnedBy(PlayerController player)
    {
        return CurrentOwner == player;
    }

    public bool IsOwnedBy(Transform t)
    {
        return CurrentOwner != null && CurrentOwner.transform == t;
    }
}