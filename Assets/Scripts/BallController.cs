using UnityEngine;

public class BallController : MonoBehaviour
{
    public Rigidbody2D rb;

    public float rollingResistance = 1.2f;
    public float stopSpeed = 0.20f;


    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public float maxSpeed = 10f;

    void FixedUpdate()
    {
        if (rb.velocity.magnitude > maxSpeed)
            rb.velocity = rb.velocity.normalized * maxSpeed;

        // rolling resistance + stop threshold burada kalsýn
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerController player = collision.collider.GetComponent<PlayerController>();

        if (player != null)
        {
            GameManager gm = FindFirstObjectByType<GameManager>();
            if (gm != null)
            {
                gm.SetActivePlayer(player);
            }
        }
    }

}
