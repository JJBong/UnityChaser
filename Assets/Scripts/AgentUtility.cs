using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Timers;
using UnityEngine.SceneManagement;

public class AgentUtility : MonoBehaviour {

    NavMeshAgent[] agents = new NavMeshAgent[BallAgent.agentNum];

    private float[][] targetToDistances;

    void Start() {
        for (int i = 0; i < BallAgent.agentNum; i++) {
            agents[i] = BallAgent.agents[i].GetComponent<NavMeshAgent>();
        }
        StartCoroutine(PathFind(.01f));
    }

    private void Update() {
        for (int i = 0; i < BallAgent.agentNum; i++)
        {
            for (int j = 0; j < BallAgent.targetNum; j++)
            {
                if (Mathf.Min(targetToDistances[i]) == targetToDistances[i][j] && BallAgent.targets[j] != null)
                {

                    agents[i].SetDestination(BallAgent.targets[j].position);
                }

                else if (Mathf.Min(targetToDistances[i]) == targetToDistances[i][j] && BallAgent.targets[j] == null)
                {
                    targetToDistances[i][j] = 100000000f;
                    agents[i].SetDestination(BallAgent.agents[i].position);
                }
            }
        }
    }
        
    IEnumerator PathFind(float delayTime) {
        while(true) {

            targetToDistances = new float[BallAgent.agentNum][];

            for (int i = 0; i < BallAgent.agentNum; i++) {
                targetToDistances[i] = new float[BallAgent.targetNum];
                for (int j = 0; j < BallAgent.targetNum; j++) {
                    if (BallAgent.targets[j] != null) {
                        targetToDistances[i][j] = Mathf.Sqrt(
                            Mathf.Pow((BallAgent.targets[j].position.x - agents[i].transform.position.x), 2)
                            + Mathf.Pow((BallAgent.targets[j].position.z - agents[i].transform.position.z), 2)
                        );
                    }
                    else {
                        targetToDistances[i][j] = 100000000f;
                    }
                }
            }

            yield return new WaitForSeconds(delayTime);
        }
    }
}

