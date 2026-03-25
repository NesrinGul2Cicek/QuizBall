using UnityEngine;

public class HomeTeamManager : MonoBehaviour
{
    public PlayerAI[] teammates;
    private BallController ball;

    [Header("Stability")]
    public float updateInterval = 0.15f;
    public float switchMargin = 0.25f;

    [Header("Lock")]
    public float holdTime = 0.6f;

    private float chaserLockedUntil = 0f;
    private float nextUpdateTime = 0f;
    private PlayerAI currentChaser;

    private const float tieEpsilon = 0.0005f;

    void Start()
    {
        if (teammates == null || teammates.Length == 0)
            teammates = FindObjectsOfType<PlayerAI>();

        ball = FindFirstObjectByType<BallController>();
    }

    void Update()
    {
        if (Time.time < nextUpdateTime) return;
        nextUpdateTime = Time.time + updateInterval;

        if (ball == null || teammates == null || teammates.Length == 0) return;

        Vector2 ballPos = ball.transform.position;

        // Eðer mevcut chaser artýk uygun deðilse býrak
        if (currentChaser != null && !CanBeChaser(currentChaser))
            currentChaser = null;

        // Lock aktifse seçim deðiþtirme, sadece uygula
        if (Time.time < chaserLockedUntil)
        {
            ApplyChaser();
            return;
        }

        PlayerAI best = null;
        float bestDist = float.PositiveInfinity;

        foreach (var ai in teammates)
        {
            if (ai == null) continue;
            if (!CanBeChaser(ai)) continue;

            float d = Vector2.Distance(ai.transform.position, ballPos);

            if (d < bestDist - tieEpsilon)
            {
                bestDist = d;
                best = ai;
            }
            else if (Mathf.Abs(d - bestDist) <= tieEpsilon && best != null)
            {
                if (ai.GetInstanceID() < best.GetInstanceID())
                    best = ai;
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

    bool CanBeChaser(PlayerAI ai)
    {
        if (ai == null) return false;

        PlayerController pc = ai.GetComponent<PlayerController>();
        if (pc == null) return false;

        // Ýnsan kontrolündeki oyuncuyu chaser yapma
        if (pc.isHuman) return false;

        return true;
    }

    void ApplyChaser()
    {
        foreach (var ai in teammates)
        {
            if (ai == null) continue;
            ai.isChaser = (currentChaser != null && ai == currentChaser);
        }
    }
}