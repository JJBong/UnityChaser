using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallSight : MonoBehaviour
{
    public float ViewAngle;
    public float ViewDistance;

    public LayerMask TargetMask;
    public LayerMask ObstacleMask;

    public static ArrayList visible_targets;
    public static ArrayList visible_obstacles;

    public static float _angle;
    public static float _distance;
    public static LayerMask _target_mask;
    public static LayerMask _obstacle_mask;

    private void Awake()
    {
        _angle = ViewAngle;
        _distance = ViewDistance;
        _target_mask = TargetMask;
        _obstacle_mask = ObstacleMask;
    }

    void Update()
    {
        DrawView();
        FindVisibleTargets();
        GameObject.Find("MiniMap").GetComponent<TileGenerator>().DrawMiniMapTiles();
    }

    public Vector3 DirFromAngle(Transform _transform, float angleInDegrees)
    {
        angleInDegrees += _transform.transform.eulerAngles.y;
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    public void DrawView()
    {
        for (int i = 0; i < BallAgent.agents.Length; i++) {
            Vector3 leftBoundary = DirFromAngle(BallAgent.agents[i].transform, - ViewAngle / 2);
            Vector3 centerBoundary = DirFromAngle(BallAgent.agents[i].transform, 0);
            Vector3 rightBoundary = DirFromAngle(BallAgent.agents[i].transform, ViewAngle / 2);
            Debug.DrawLine(BallAgent.agents[i].transform.position, BallAgent.agents[i].transform.position + leftBoundary * ViewDistance, Color.blue);
            Debug.DrawLine(BallAgent.agents[i].transform.position, BallAgent.agents[i].transform.position + centerBoundary * ViewDistance, Color.green);
            Debug.DrawLine(BallAgent.agents[i].transform.position, BallAgent.agents[i].transform.position + rightBoundary * ViewDistance, Color.blue);
        }
    }

    public void FindVisibleTargets()
    {
        visible_targets = new ArrayList();
        visible_obstacles = new ArrayList();
        for (int i = 0; i < BallAgent.agents.Length; i++)
        {
            Collider[] targets = Physics.OverlapSphere(BallAgent.agents[i].transform.position, ViewDistance, TargetMask);
            Collider[] obstacles = Physics.OverlapSphere(BallAgent.agents[i].transform.position, ViewDistance, ObstacleMask);
            
            for (int j = 0; j < targets.Length; j++)
            {
                Transform target = targets[j].transform;
                Vector3 dirToTarget = (target.position - BallAgent.agents[i].transform.position).normalized;

                if (Vector3.Dot(BallAgent.agents[i].transform.forward, dirToTarget) > Mathf.Cos((ViewAngle / 2) * Mathf.Deg2Rad))
                {
                    float distToTarget = Vector3.Distance(BallAgent.agents[i].transform.position, target.position);

                    if (!Physics.Raycast(BallAgent.agents[i].transform.position, dirToTarget, distToTarget, ObstacleMask))
                    {
                        if (!visible_targets.Contains(target))
                        {
                            visible_targets.Add(target);
                        }
                        Debug.DrawLine(BallAgent.agents[i].transform.position, target.position, Color.red);
                    }
                }
            }

            for (int j = 0; j < obstacles.Length; j++)
            {
                Transform obstacle = obstacles[j].transform;
                Vector3 dirToObstacle = (obstacle.position - BallAgent.agents[i].transform.position).normalized;

                if (Vector3.Dot(BallAgent.agents[i].transform.forward, dirToObstacle) > Mathf.Cos((ViewAngle / 2) * Mathf.Deg2Rad))
                {
                    float distToObstacle = Vector3.Distance(BallAgent.agents[i].transform.position, obstacle.position);

                    if (!Physics.Raycast(BallAgent.agents[i].transform.position, dirToObstacle, distToObstacle, TargetMask))
                    {
                        if (!visible_obstacles.Contains(obstacle))
                        {
                            visible_obstacles.Add(obstacle);
                        }
                        Debug.DrawLine(BallAgent.agents[i].transform.position, obstacle.position, Color.red);
                    }
                }
            }
        }
    }

    public static ArrayList Visible_Targets(Transform origin)
    {
        
        Collider[] targets = Physics.OverlapSphere(origin.transform.position, _distance, _target_mask);
        ArrayList _visible_targets = new ArrayList();
        for (int i = 0; i < targets.Length; i++) {
            Transform target = targets[i].transform;
            Vector3 dirToTarget = (target.position - origin.transform.position).normalized;
            if (Vector3.Dot(origin.transform.forward, dirToTarget) > Mathf.Cos((_angle / 2) * Mathf.Deg2Rad)) {
                _visible_targets.Add(target);
            }
        }
        return _visible_targets;
    }

    public static ArrayList Visible_Obstacles(Transform origin)
    {
        Collider[] obstacles = Physics.OverlapSphere(origin.transform.position, _distance, _obstacle_mask);
        ArrayList _visible_obstacles = new ArrayList();
        for (int i = 0; i < obstacles.Length; i++)
        {
            Transform obstacle = obstacles[i].transform;
            Vector3 dirToTarget = (obstacle.position - origin.transform.position).normalized;
            if (Vector3.Dot(origin.transform.forward, dirToTarget) > Mathf.Cos((_angle / 2) * Mathf.Deg2Rad))
            {
                _visible_obstacles.Add(obstacle);
            }
        }
        return _visible_obstacles;
    }
}
