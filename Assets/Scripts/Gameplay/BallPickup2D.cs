using UnityEngine;



[RequireComponent(typeof(BallController))]
[RequireComponent(typeof(Collider2D))]
public class BallPickup2D : MonoBehaviour
{
    private BallController ball;

    [Header("Pickup Settings")]
    public float maxPickupBallSpeed = 2.5f;
    public bool requireLooseBall = true;

    void Awake()
    {
        ball = GetComponent<BallController>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (ball == null) return;

        PlayerController pc = other.GetComponent<PlayerController>();
        if (pc == null) return;

        // Top zaten bu oyuncudaysa tekrar verme
        if (ball.CurrentOwner == pc) return;

        // Top çok hýzlý gidiyorsa anýnda sahiplenilmesin
        if (ball.rb != null && ball.rb.velocity.magnitude > maxPickupBallSpeed) return;

        // Ýstersen sadece boþtaki top alýnsýn
        if (requireLooseBall && ball.CurrentOwner != null) return;

        ball.SetOwner(pc);
        Debug.Log($"BALL OWNER -> {pc.name}");
    }
}