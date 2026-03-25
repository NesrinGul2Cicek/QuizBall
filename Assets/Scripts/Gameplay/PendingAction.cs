using UnityEngine;

[System.Serializable]
public class PendingAction
{
    public ActionType actionType;
    public PlayerController actor;
    public Vector2 targetPoint;
    public PlayerController targetPlayer;

    public PendingAction(ActionType actionType, PlayerController actor, Vector2 targetPoint, PlayerController targetPlayer = null)
    {
        this.actionType = actionType;
        this.actor = actor;
        this.targetPoint = targetPoint;
        this.targetPlayer = targetPlayer;
    }
}