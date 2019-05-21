using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPosition : MonoBehaviour
{
    public int agent_index = 0;

    private Vector3 rotate_dir;

    public Vector3 DirFromAngle(Transform _transform, float angleInDegrees)
    {
        angleInDegrees += _transform.transform.eulerAngles.y;
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    void Update()
    {
        transform.position = BallAgent.agents[agent_index].transform.position;

        transform.transform.eulerAngles = new Vector3(
            transform.transform.eulerAngles.x,
            BallAgent.agents[agent_index].transform.transform.eulerAngles.y,
            transform.transform.eulerAngles.z
        );
    }
}
