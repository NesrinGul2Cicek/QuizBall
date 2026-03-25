using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class EnemyAI : MonoBehaviour
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
    public float chaseEnter = 1.9f;
    public float chaseExit = 1.3f;
    public float arriveStop = 0.25f;
    public float pressureRadius = 1.8f;

    [Header("After Action")]
    public float returnHomeTime = 1.0f;
    private float returnHomeUntil = 0f;

    [Header("Decision")]
    public float actionCooldown = 0.25f;
    private float nextActionTime = 0f;

    public float shootCooldown = 0.35f;
    private float nextShootTime = 0f;

    public bool CanBeChaser => Time.time >= returnHomeUntil;

    private PlayerController pc;
    private BallController ball;
    private GameManager gm;

    void Awake()
    {
        pc = GetComponent<PlayerController>();
        ball = FindFirstObjectByType<BallController>();
        gm = FindFirstObjectByType<GameManager>();
        Debug.Log($"{name} | homePoint={(homePoint ? homePoint.name : "NULL")} | myGoal={(myGoal ? myGoal.name : "NULL")} | opponentGoal={(opponentGoal ? opponentGoal.name : "NULL")}");
        
    }

    void Update()
    {
        if (pc == null || ball == null || gm == null) return;

        if (gm.state != GameState.Playing)
        {
            pc.SetMoveInput(Vector2.zero);
            return;
        }

        if (Time.time < returnHomeUntil)
        {
            state = State.Positioning;
            DoPositioning();
            return;
        }

        if (!isChaser)
        {
            state = State.Positioning;
            DoPositioning();
            return;
        }

        Vector2 myPos = transform.position;
        Vector2 ballPos = ball.transform.position;

        // Top bu oyuncuda değilse kovala
        if (!pc.HasBall())
        {
            float distToBall = Vector2.Distance(myPos, ballPos);

            if (distToBall > chaseEnter)
            {
                state = State.ChaseBall;
                MoveTowards(ballPos, arriveStop);
                return;
            }

            if (distToBall >= chaseExit)
            {
                state = State.ChaseBall;
                MoveTowards(ballPos, arriveStop);
                return;
            }

            MoveTowards(ballPos, arriveStop);
            return;
        }

        // Top bu oyuncudaysa hareketi kesip karar ver
        pc.SetMoveInput(Vector2.zero);

        if (Time.time < nextActionTime) return;
        nextActionTime = Time.time + actionCooldown;

        DecideAction(ballPos);
    }

    void DecideAction(Vector2 ballPos)
    {
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
            DribbleTowardsGoal();
            return;
        }

        state = State.DribbleToGoal;
        if (myGoal != null)
        {
            Vector2 dir = ((Vector2)transform.position - (Vector2)myGoal.position).normalized;
            if (dir.sqrMagnitude < 0.001f) dir = Vector2.right;
            DribbleTowards((Vector2)transform.position + dir * 4f);
        }
        else
        {
            pc.SetMoveInput(Vector2.zero);
        }
    }

    void DoPositioning()
    {
        if (homePoint != null) MoveTowards(homePoint.position, arriveStop);
        else pc.SetMoveInput(Vector2.zero);
    }

    void TryShootAtGoal()
    {
        if (opponentGoal == null) return;

        pc.SetMoveInput(Vector2.zero);

        if (Time.time < nextShootTime) return;
        nextShootTime = Time.time + shootCooldown;

        gm.RequestAction(ActionType.Shoot, pc, opponentGoal.position);

        returnHomeUntil = Time.time + returnHomeTime;
        isChaser = false;
    }

    void TryPassTo(Vector2 targetPos)
    {
        pc.SetMoveInput(Vector2.zero);

        gm.RequestAction(ActionType.ShortPass, pc, targetPos);

        returnHomeUntil = Time.time + returnHomeTime;
        isChaser = false;
    }

    void DribbleTowardsGoal()
    {
        if (opponentGoal == null)
        {
            pc.SetMoveInput(Vector2.zero);
            return;
        }

        DribbleTowards(opponentGoal.position);
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
        var humans = FindObjectsOfType<PlayerController>();

        foreach (var h in humans)
        {
            if (h == null) continue;
            if (!h.isHuman) continue;

            float d = Vector2.Distance(h.transform.position, transform.position);
            if (d <= pressureRadius) return true;
        }

        return false;
    }

    Transform FindBestTeammate()
    {
        EnemyAI[] mates = FindObjectsOfType<EnemyAI>();
        Transform best = null;
        float bestScore = Mathf.Infinity;

        foreach (var m in mates)
        {
            if (m == null) continue;
            if (m == this) continue;

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