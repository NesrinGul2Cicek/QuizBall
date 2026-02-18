using UnityEngine;

public class GameManager : MonoBehaviour
{
    private PlayerController[] players;
    public BallController ball;

    public CameraFollow cam;


    void Start()
    {
        players = FindObjectsOfType<PlayerController>();
        if (cam == null)
            cam = FindFirstObjectByType<CameraFollow>();

    }

    /*public void SwitchToNearestPlayer()
    {
        PlayerController nearest = null;
        float minDist = Mathf.Infinity;

        foreach (PlayerController p in players)
        {
            float dist = Vector2.Distance(p.transform.position, ball.transform.position);

            if (dist < minDist)
            {
                minDist = dist;
                nearest = p;
            }
        }

        foreach (PlayerController p in players)
        {
            p.SetActive(false);
        }

        if (nearest != null)
        {
            nearest.SetActive(true);

            if (cam != null)
                cam.target = nearest.transform;
        }


    }*/

    public void SwitchToNearestPlayerExcluding(PlayerController exclude)
    {
        if (ball == null) return;

        PlayerController nearest = null;
        float minDist = Mathf.Infinity;

        foreach (PlayerController p in players)
        {
            if (p == null || p == exclude) continue;

            float dist = Vector2.Distance(p.transform.position, ball.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = p;
            }
        }

        foreach (PlayerController p in players)
            if (p != null) p.SetActive(false);

        if (nearest != null)
        {
            nearest.SetActive(true);
            if (cam != null) cam.target = nearest.transform;
        }

        Debug.Log($"Switch -> {nearest.name}");
    }

    public void SetActivePlayer(PlayerController newPlayer)
    {
        if (newPlayer == null) return;

        foreach (PlayerController p in players)
            if (p != null) p.SetActive(false);

        newPlayer.SetActive(true);

        if (cam != null)
            cam.target = newPlayer.transform;
    }

}
