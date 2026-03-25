using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class FriendlyAI : MonoBehaviour
{
    public enum State { Positioning, ChaseBall, DribbleToGoal, Pass, Shoot }
    public State state = State.Positioning;

    [Header("Chaser Selection")]
    public bool isChaser = false;

    [Header("Refs")]
    public Transform homePoint;
    public Transform opponentGoal;
    public Transform myGoal;

    [Header("Ranges")]
    public float shootRangeToGoal = 2.2f;
    public float controlBallRange = 1.6f;
    public float arriveStop = 0.25f;

    [Header("Decision")]
    public float pressureRadius = 1.8f;

    [Header("After Kick")]
    public float returnHomeTime = 1.0f;
    private float returnHomeUntil = 0f;
    public bool CanBeChaser => Time.time >= returnHomeUntil;

    private PlayerController pc;
    private BallController ball;
    private GameManager gm;

    private float nextActionTime = 0f;
    public float actionCooldown = 0.25f;

    public float shootCooldown = 0.35f;
    private float nextShootTime = 0f;

    void Awake()
    {
        pc = GetComponent<PlayerController>();
        ball = FindFirstObjectByType<BallController>();
        gm = FindFirstObjectByType<GameManager>();
    }

    void Update()
    {
        if (ball == null || gm == null || pc == null) return;

        if (gm.state != GameState.Playing)
        {
            pc.SetMoveInput(Vector2.zero);
            return;
        }

        // Ýnsan kontrolündeyse AI karýþmasýn
        if (pc.isHuman)
        {
            pc.SetMoveInput(Vector2.zero);
            return;
        }

        // Þut/pas sonrasý home'a dön
        if (Time.time < returnHomeUntil)
        {
            state = State.Positioning;
            DoPositioning();
            return;
        }

        Vector2 myPos = transform.position;
        Vector2 ballPos = ball.transform.position;

        if (!isChaser)
        {
            state = State.Positioning;
            DoPositioning();
            return;
        }

        // Top bu oyuncuda deðilse kovala
        if (!pc.HasBall())
        {
            float distToBall = Vector2.Distance(myPos, ballPos);

            if (distToBall > controlBallRange)
            {
                state = State.ChaseBall;
                MoveTowards(ballPos, arriveStop);
                return;
            }
            Debug.Log($"{name} TOP BENDE");
            MoveTowards(ballPos, arriveStop);
            return;
        }

        // Top bu oyuncudaysa karar ver
        pc.SetMoveInput(Vector2.zero);

        if (opponentGoal != null)
        {
            float distToGoal = Vector2.Distance(ballPos, opponentGoal.position);

            if (distToGoal <= shootRangeToGoal)
            {
                state = State.Shoot;
                TryShootAtGoal();
                return;
            }

            bool underPressure = IsUnderPressure();
            if (underPressure)
            {
                Transform mate = FindBestTeammate();
                if (mate != null)
                {
                    state = State.Pass;
                    TryPassTo(mate.position);
                    return;
                }
            }

            state = State.DribbleToGoal;
            DribbleTowards(opponentGoal.position);
            return;
        }

        state = State.DribbleToGoal;
        DribbleTowards(ballPos);
    }

    void DoPositioning()
    {
        if (homePoint != null) MoveTowards(homePoint.position, arriveStop);
        else pc.SetMoveInput(Vector2.zero);
    }

    void TryShootAtGoal()
    {
        pc.SetMoveInput(Vector2.zero);

        if (Time.time < nextActionTime) return;
        nextActionTime = Time.time + actionCooldown;

        if (Time.time < nextShootTime) return;
        nextShootTime = Time.time + shootCooldown;

        if (opponentGoal == null) return;

        gm.RequestAction(ActionType.Shoot, pc, opponentGoal.position);

        returnHomeUntil = Time.time + returnHomeTime;
        isChaser = false;
    }

    void TryPassTo(Vector2 targetPos)
    {
        if (Time.time < nextActionTime)
        {
            pc.SetMoveInput(Vector2.zero);
            return;
        }

        nextActionTime = Time.time + actionCooldown;

        pc.SetMoveInput(Vector2.zero);
        gm.RequestAction(ActionType.ShortPass, pc, targetPos);

        returnHomeUntil = Time.time + returnHomeTime;
        isChaser = false;
    }

    void DribbleTowards(Vector2 target)
    {
        Vector2 to = target - (Vector2)transform.position;

        if (to.sqrMagnitude < 0.01f)
        {
            pc.SetMoveInput(Vector2.zero);
            return;
        }

        pc.SetMoveInput(to.normalized);
    }

    bool IsUnderPressure()
    {
        var enemies = FindObjectsOfType<EnemyAI>();
        foreach (var e in enemies)
        {
            if (e == null) continue;

            float d = Vector2.Distance(e.transform.position, transform.position);
            if (d <= pressureRadius) return true;
        }
        return false;
    }

    Transform FindBestTeammate()
    {
        FriendlyAI[] mates = FindObjectsOfType<FriendlyAI>();
        Transform best = null;
        float bestScore = Mathf.Infinity;

        foreach (var m in mates)
        {
            if (m == null || m == this) continue;

            float d = Vector2.Distance(m.transform.position, transform.position);

            float goalBonus = 0f;
            if (opponentGoal != null)
                goalBonus = Vector2.Distance(m.transform.position, opponentGoal.position) * 0.2f;

            float score = d + goalBonus;
            if (score < bestScore)
            {
                bestScore = score;
                best = m.transform;
            }
        }

        return best;
    }

    void MoveTowards(Vector2 target, float stopDist)
    {
        Vector2 to = target - (Vector2)transform.position;

        if (to.magnitude <= stopDist)
        {
            pc.SetMoveInput(Vector2.zero);
            return;
        }

        pc.SetMoveInput(to.normalized);
    }
}