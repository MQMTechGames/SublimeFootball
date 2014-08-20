using UnityEngine;
using System.Collections.Generic;

public class Match : MonoBehaviour
{
    [SerializeField]
    List<Ball> balls = new List<Ball>();

    [SerializeField]
    BaseSquad _playerSquad;
    public BaseSquad PlayerSquad { get { return _playerSquad; }}

    [SerializeField]
    BaseSquad _enemySquad;
    public BaseSquad EnemySquad { get { return _enemySquad; }}

    public void Awake()
    {
        //Physics.gravity = new Vector3(0f, -128f, 0f);
        Physics.gravity = new Vector3(0f, -20f, 0f);
    }

    public bool RemoveBall(Ball ball)
    {
        return balls.Remove(ball);
    }

    public Ball FindClosestBallToPosition(Vector3 position)
    {
        if(balls.Count == 1)
        {
            return balls[0];
        }

        float closestDist = float.MaxValue;
        Ball closestBall = null;

        foreach (Ball ball in balls)
        {
            float sqrDist = (position - ball.transform.position).sqrMagnitude;

            if(sqrDist < closestDist)
            {
                closestDist = sqrDist;
                closestBall = ball;
            }
        }

        return closestBall;
    }
}
