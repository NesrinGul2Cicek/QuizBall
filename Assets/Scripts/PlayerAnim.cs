using UnityEngine;

public class PlayerAnim : MonoBehaviour
{
    Animator anim;
    SpriteRenderer sr;

    void Awake()
    {
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        Vector2 move = new Vector2(x, y).normalized;

        // Hareket var mý?
        bool moving = move.sqrMagnitude > 0.001f;
        anim.SetBool("Moving", moving);

        // Yön parametreleri (Blend Tree / state seçiminde kullanýlýr)
        anim.SetFloat("MoveX", move.x);
        anim.SetFloat("MoveY", move.y);

        // Sola gidiyorsa sprite'ý çevir (saða gidiyorsa normale dön)
        if (move.x < -0.01f) sr.flipX = true;
        else if (move.x > 0.01f) sr.flipX = false;
    }
}
