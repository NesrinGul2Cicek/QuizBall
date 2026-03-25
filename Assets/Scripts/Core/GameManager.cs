using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameState state = GameState.Playing;

    [Header("References")]
    public BallController ball;
    public QuestionManager questionManager;

    private PendingAction pendingAction;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public bool CanAcceptInput()
    {
        return state == GameState.Playing;
    }

    public void RequestAction(ActionType actionType, PlayerController actor, Vector2 targetPoint, PlayerController targetPlayer = null)
    {
        if (state != GameState.Playing) return;
        if (actor == null) return;

        pendingAction = new PendingAction(actionType, actor, targetPoint, targetPlayer);
        state = GameState.QuestionPopup;

        if (questionManager != null)
            questionManager.ShowQuestion(actionType);
        else
            OnQuestionAnswered(true);
    }

    public void OnQuestionAnswered(bool isCorrect)
    {
        if (pendingAction == null)
        {
            state = GameState.Playing;
            return;
        }

        state = GameState.ResolvingAction;

        if (isCorrect)
            ResolveSuccess(pendingAction);
        else
            ResolveFail(pendingAction);

        pendingAction = null;
        state = GameState.Playing;
    }

    void ResolveSuccess(PendingAction action)
    {
        if (action == null || action.actor == null) return;

        switch (action.actionType)
        {
            case ActionType.ShortPass:
                action.actor.ExecuteShortPass(action.targetPoint);
                break;

            case ActionType.LongPass:
                action.actor.ExecuteLongPass(action.targetPoint);
                break;

            case ActionType.Shoot:
                action.actor.ExecuteShoot(action.targetPoint);
                break;
        }
    }

    void ResolveFail(PendingAction action)
    {
        if (action == null || action.actor == null) return;

        switch (action.actionType)
        {
            case ActionType.ShortPass:
                action.actor.ExecuteFailedPass(action.targetPoint);
                break;

            case ActionType.LongPass:
                action.actor.ExecuteFailedLongPass(action.targetPoint);
                break;

            case ActionType.Shoot:
                action.actor.ExecuteFailedShoot(action.targetPoint);
                break;
        }
    }

    public void CancelPendingAction()
    {
        pendingAction = null;
        state = GameState.Playing;
    }
}