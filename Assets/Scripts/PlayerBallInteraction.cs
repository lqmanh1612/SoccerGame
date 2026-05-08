using UnityEngine;
using System.Collections.Generic;

public class PlayerBallInteraction : MonoBehaviour
{
    [Header("Settings")]
    public float interactionDistance = 2f;
    public string ballNamePrefix = "Soccer Ball";
    
    private List<Rigidbody> balls = new List<Rigidbody>();
    public Rigidbody NearestBall { get; private set; }
    public List<Rigidbody> AllBalls => balls;

    void Start()
    {
        InitializeBalls();
    }

    public void InitializeBalls()
    {
        balls.Clear();
        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (obj.hideFlags != HideFlags.None || obj.scene.name == null) continue;
            if (obj.name.StartsWith(ballNamePrefix))
            {
                Rigidbody rb = obj.GetComponent<Rigidbody>();
                if (rb == null) rb = obj.AddComponent<Rigidbody>();
                balls.Add(rb);
            }
        }
    }

    void Update()
    {
        UpdateNearestBall();
    }

    void UpdateNearestBall()
    {
        NearestBall = null;
        float minDistance = float.MaxValue;

        foreach (Rigidbody ball in balls)
        {
            if (ball == null) continue;
            float distance = Vector3.Distance(transform.position, ball.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                NearestBall = ball;
            }
        }

        // Only keep it if it's within interaction distance
        if (NearestBall != null && Vector3.Distance(transform.position, NearestBall.position) > interactionDistance)
        {
            NearestBall = null;
        }
    }
}
