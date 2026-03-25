using UnityEngine;
using UnityEngine.UI;

public class OutOfBoundsTrigger : MonoBehaviour
{
    public OutSide side;   // Inspector’dan hangi taraf olduðunu seç

    private MatchManager matchManager;

    void Start()
    {
        matchManager = FindFirstObjectByType<MatchManager>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        BallController ball = other.GetComponent<BallController>();

        if (ball != null)
        {
            matchManager.OnBallOut(side);
        }
    }
}