using UnityEngine;
using System.Collections;

public class MatchManager : MonoBehaviour
{
    public BallController ball;

    public Transform restartCenter;
    public Transform restartTop;
    public Transform restartBottom;
    public Transform restartLeft;
    public Transform restartRight;

    public float pauseSeconds = 0.6f;
    bool busy;

    void Awake()
    {
        if (ball == null) ball = FindFirstObjectByType<BallController>();
       
        Debug.Log("Ball found: " + (ball != null ? ball.name : "NULL"));
    }

    public void OnBallOut(OutSide side)
    {
        if (busy) return;
        StartCoroutine(HandleOut(side));
    }

    IEnumerator HandleOut(OutSide side)
    {
        busy = true;
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(pauseSeconds);

        Time.timeScale = 1f;
        ball.ResetBall(GetRestart(side));
        busy = false;
    }

    Vector2 GetRestart(OutSide side)
    {
        return side switch
        {
            OutSide.Top => restartTop.position,
            OutSide.Bottom => restartBottom.position,
            OutSide.Left => restartLeft.position,
            OutSide.Right => restartRight.position,
            _ => restartCenter.position,
        };
    }

    public void OnGoalScored(bool isRightGoal)
    {
        if (busy) return;
        StartCoroutine(HandleGoal());
    }

    IEnumerator HandleGoal()
    {
        busy = true;
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(pauseSeconds);

        Time.timeScale = 1f;
        ball.ResetBall(restartCenter.position);
        busy = false;
    }
}