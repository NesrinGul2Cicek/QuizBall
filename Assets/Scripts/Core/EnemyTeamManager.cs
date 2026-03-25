using UnityEngine;

public class EnemyTeamManager : MonoBehaviour
{
    public EnemyAI[] enemies;
    private BallController ball;

    [Header("Stability")]
    public float updateInterval = 0.15f;
    public float switchMargin = 0.25f;

    [Header("Lock")]
    public float holdTime = 0.6f;

    private float chaserLockedUntil = 0f;
    private float nextUpdateTime = 0f;
    private EnemyAI currentChaser;

    private const float tieEpsilon = 0.0005f;

    void Start()
    {
        if (enemies == null || enemies.Length == 0)
            enemies = FindObjectsOfType<EnemyAI>();

        ball = FindFirstObjectByType<BallController>();
    }

    void Update()
    {
        if (Time.time < nextUpdateTime) return;
        nextUpdateTime = Time.time + updateInterval;

        if (ball == null || enemies == null || enemies.Length == 0) return;

        Vector2 ballPos = ball.transform.position;

        if (currentChaser != null && !currentChaser.CanBeChaser)
            currentChaser = null;

        if (Time.time < chaserLockedUntil)
        {
            ApplyChaser();
            return;
        }

        EnemyAI best = null;
        float bestDist = float.PositiveInfinity;

        foreach (var e in enemies)
        {
            if (e == null) continue;
            if (!e.CanBeChaser) continue;

            float d = Vector2.Distance(e.transform.position, ballPos);

            if (d < bestDist - tieEpsilon)
            {
                bestDist = d;
                best = e;
            }
            else if (Mathf.Abs(d - bestDist) <= tieEpsilon && best != null)
            {
                if (e.GetInstanceID() < best.GetInstanceID())
                    best = e;
            }
        }

        if (best == null)
        {
            currentChaser = null;
            ApplyChaser();
            return;
        }

        float currentDist = currentChaser != null
            ? Vector2.Distance(currentChaser.transform.position, ballPos)
            : float.PositiveInfinity;

        bool shouldSwitch =
            currentChaser == null ||
            (best != currentChaser && bestDist < currentDist - switchMargin);

        if (shouldSwitch)
        {
            currentChaser = best;
            chaserLockedUntil = Time.time + holdTime;
        }

        ApplyChaser();
    }

    void ApplyChaser()
    {
        foreach (var e in enemies)
        {
            if (e == null) continue;
            e.isChaser = (currentChaser != null && e == currentChaser);
        }
    }
}