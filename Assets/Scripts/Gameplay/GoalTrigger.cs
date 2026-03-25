using UnityEngine;

public class GoalTrigger : MonoBehaviour
{
    public bool isRightGoal;

    void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.CompareTag("Ball")) return;

        var mm = FindFirstObjectByType<MatchManager>();
        if (mm != null) mm.OnGoalScored(isRightGoal);
    }
}