using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerAI : MonoBehaviour
{
    public Transform homePoint;
    public Transform opponentGoal;

    [Header("Chaser Selection")]
    public bool isChaser = false;

    [Header("Ranges")]
    public float controlRange = 1.5f;
    public float shootRange = 2f;
    public float chaseStopDistance = 0.15f;

    [Header("Decision Timing")]
    public float decisionCooldown = 0.4f;
    private float nextDecisionTime = 0f;

    private PlayerController pc;
    private BallController ball;
    private GameManager gm;

    void Awake()
    {
        pc = GetComponent<PlayerController>();
        ball = FindFirstObjectByType<BallController>();
        gm = FindFirstObjectByType<GameManager>();
    }

    void Update()
    {
        if (pc == null || ball == null || gm == null)
            return;

        if (pc.isHuman)
            return;

        if (!gm.CanAcceptInput())
        {
            pc.SetMoveInput(Vector2.zero);
            return;
        }

        // Chaser değilse pozisyon al / sabit kal
        if (!isChaser)
        {
            if (homePoint != null)
                MoveTowards(homePoint.position);
            else
                StopMoving();

            return;
        }

        // Chaser ise top bende değilken topa git
        if (!pc.HasBall())
        {
            ChaseBall();
            return;
        }

        // Top bende ise dur ve karar ver
        StopMoving();

        if (Time.time < nextDecisionTime)
            return;

        nextDecisionTime = Time.time + decisionCooldown;

        DecideAction();
    }

    void ChaseBall()
    {
        Vector2 toBall = (Vector2)ball.transform.position - (Vector2)transform.position;

        if (toBall.sqrMagnitude <= chaseStopDistance * chaseStopDistance)
        {
            StopMoving();
            return;
        }

        MoveTowards(ball.transform.position);
    }

    void DecideAction()
    {
        if (opponentGoal == null)
            return;

        Vector2 targetPoint;
        ActionType actionType;

        float distToGoal = Vector2.Distance(transform.position, opponentGoal.position);

        if (distToGoal <= shootRange)
        {
            actionType = ActionType.Shoot;
            targetPoint = opponentGoal.position;
        }
        else
        {
            actionType = ActionType.ShortPass;
            Vector2 forward = ((Vector2)opponentGoal.position - (Vector2)transform.position).normalized;
            targetPoint = (Vector2)transform.position + forward * 2.5f;
        }

        gm.RequestAction(actionType, pc, targetPoint);
    }

    void MoveTowards(Vector2 target)
    {
        Vector2 dir = target - (Vector2)transform.position;

        if (dir.sqrMagnitude < 0.0001f)
        {
            StopMoving();
            return;
        }

        pc.SetMoveInput(dir.normalized);
    }

    void StopMoving()
    {
        pc.SetMoveInput(Vector2.zero);
    }
}