using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using MLAgents;
using System;

[RequireComponent(typeof(NavMeshAgent))]
public class BallAgent : Agent {
    
    public Transform pivotTransform;
    public Transform agent;
    public Transform target;
    public Transform _camera;

    public static Transform[] agents;
    public static Transform[] targets;
    public Camera[] cameras;

    private Rigidbody[] agentRigidbodys;
    private Rigidbody[] targetRigidbodys;

    public static int agentNum = 3;
    public static int targetNum = 10;
    public float moveForce = 10f;
    private int priorEliminatedNum = 0;

    private bool agentResetIsComplete = false;
    
    private void Awake() {
        agents = new Transform[agentNum];
        targets = new Transform[targetNum];
        agentRigidbodys = new Rigidbody[agentNum];
        targetRigidbodys = new Rigidbody[targetNum];
        cameras = new Camera[agentNum];
    }

    IEnumerator WaitForMapRendering()
    {
        
        yield return new WaitUntil(() => MapGenerator.mapCreated);
    }

    IEnumerator WaitForMiniMapRendering()
    {
        yield return new WaitUntil(() => TileGenerator.tileMap_isCreated && TileGenerator.tileMapInfo != null);
    }

    IEnumerator AgentResetComplited()
    {
        yield return new WaitUntil(() => agentResetIsComplete);
    }
    private void Update()
    {
        
    }
    void ResetTarget() {
        StartCoroutine(WaitForMapRendering());

        for (int i = 0; i < targetNum; i++) {
            int randomX = UnityEngine.Random.Range(MapGenerator.currentMapSize.x / 2, MapGenerator.currentMapSize.x);
            int randomZ = UnityEngine.Random.Range(MapGenerator.currentMapSize.y / 2, MapGenerator.currentMapSize.y);
            while (MapGenerator.obstacleMap[randomX, randomZ])
            {
                randomX = UnityEngine.Random.Range(MapGenerator.currentMapSize.x / 2, MapGenerator.currentMapSize.x);
                randomZ = UnityEngine.Random.Range(MapGenerator.currentMapSize.y / 2, MapGenerator.currentMapSize.y);
            }
            Vector3 randomPos = MapGenerator.tilePositions[randomX, randomZ];
            bool isPlaced = false;
            for (int j = 0; j < i; j++)
            {
                if (targets[j].transform.position == randomPos + pivotTransform.position)
                {
                    isPlaced = true;
                    i--;
                    break;
                }
            }
            if (!isPlaced)
            {
                if (targets[i] != null)
                {
                    targets[i].transform.position = randomPos + pivotTransform.position;
                }
                else
                {
                    Transform Target = Instantiate(target, randomPos + pivotTransform.position, Quaternion.identity) as Transform;
                    targets[i] = Target;
                    targetRigidbodys[i] = Target.GetComponent<Rigidbody>();
                }
                targetRigidbodys[i].velocity = Vector3.zero;
            }
        }
    }

    public override void AgentReset() {
        StartCoroutine(WaitForMapRendering());
        
        for (int i = 0; i <  agentNum; i++) {
            int randomX = UnityEngine.Random.Range(0, MapGenerator.currentMapSize.x / 2);
            int randomZ = UnityEngine.Random.Range(0, MapGenerator.currentMapSize.y / 2);
            while (MapGenerator.obstacleMap[randomX, randomZ])
            {
                randomX = UnityEngine.Random.Range(0, MapGenerator.currentMapSize.x / 2);
                randomZ = UnityEngine.Random.Range(0, MapGenerator.currentMapSize.y / 2);
            }
            Vector3 randomPos = MapGenerator.tilePositions[randomX, randomZ];
            bool isPlaced = false;
            for (int j = 0; j < i; j++)
            {
                if (agents[j].transform.position == randomPos + pivotTransform.position)
                {
                    isPlaced = true;
                    i--;
                    break;
                }
            }
            if (!isPlaced)
            {
                if (agents[i] != null)
                {
                    agents[i].transform.position = randomPos + pivotTransform.position;
                }
                else
                {
                    Transform Agent = Instantiate(agent, randomPos + pivotTransform.position, Quaternion.identity) as Transform;
                    agents[i] = Agent;
                    agentRigidbodys[i] = Agent.GetComponent<Rigidbody>();
                }
                agentRigidbodys[i].velocity = Vector3.zero;
            }
            
        }
        
        TargetUtility.eliminatedNum = 0;
        priorEliminatedNum = 0;

        ResetTarget();
        agentResetIsComplete = true;
    }

    static float[] ConvertByteArrayToFloat(byte[] bytes)
    {
        float[] floats = new float[bytes.Length / 4];

        for (int i = 0; i < bytes.Length / 4; i++)
            floats[i] = BitConverter.ToSingle(bytes, i * 4);

        return floats;
    }

    public override void CollectObservations() {
        StartCoroutine(AgentResetComplited());
        StartCoroutine(WaitForMiniMapRendering());
        
        
        
        for (int i = 0; i < agents.Length; i++) {

            ArrayList visible_targets = new ArrayList();
            ArrayList visible_obstacles = new ArrayList();
            Vector3[] visible_targets_position = new Vector3[targetNum];
            Vector3[] visible_obstacles_position = new Vector3[MapGenerator.obstacleCount];

            Vector3 zero_pad = new Vector3(0f, 0f, 0f);

            visible_targets = BallSight.Visible_Targets(agents[i]);

            int target_idx = 0;
            foreach (Transform target in visible_targets)
            {
                if (target != null) {
                    Vector3 dirToTar = (target.position - agents[i].transform.position).normalized;
                    visible_targets_position[target_idx] = dirToTar;
                    target_idx++;
                }
            }
            for (int obs_idx = 0; obs_idx < target_idx; obs_idx++) {
                AddVectorObs(visible_targets_position[obs_idx]);
            }
            for (int obs_idx = target_idx; obs_idx < targetNum; obs_idx++) {
                AddVectorObs(zero_pad);
            }


            visible_obstacles = BallSight.Visible_Obstacles(agents[i]);

            int obstacle_idx = 0;
            foreach (Transform obstacle in visible_obstacles)
            {
                if (target != null)
                {
                    Vector3 dirToTar = (obstacle.position - agents[i].transform.position).normalized;
                    visible_obstacles_position[obstacle_idx] = dirToTar;
                    obstacle_idx++;
                }
            }
            for (int obs_idx = 0; obs_idx < obstacle_idx; obs_idx++)
            {
                AddVectorObs(visible_targets_position[obs_idx]);
            }
            for (int obs_idx = obstacle_idx; obs_idx < MapGenerator.obstacleCount; obs_idx++)
            {
                AddVectorObs(zero_pad);
            }
            
        }

        //if (TileGenerator.tileMapInfo != null) {
        //for (int i = 0; i < agents.Length; i++) {
        //    for (int x = 0; x < MapGenerator.currentMapSize.x; x++)
        //    {
        //        float[] row = new float[MapGenerator.currentMapSize.y];
        //        for (int y = 0; y < MapGenerator.currentMapSize.y; y++)
        //        {
        //            row[y] = TileGenerator.tileMapInfo[i, x, y];
        //        }
        //        AddVectorObs(row);
        //    }
        //    //texture[i] = rt[i].toTexture2D();
        //    //AddVectorObs(ConvertByteArrayToFloat(texture[i].GetRawTextureData()));
        //}
        
        
        //}
        //else {
        //   for (int i = 0; i < agents.Length; i++)
        //    {
        //        for (int x = 0; x < MapGenerator.currentMapSize.x; x++)
        //        {
        //            float[] row = new float[MapGenerator.currentMapSize.y];
        //            for (int y = 0; y < MapGenerator.currentMapSize.y; y++)
        //            {
        //                row[y] = -1f;
        //            }
        //            AddVectorObs(row);
        //        }
        //    }
        //}
    }

    public override void AgentAction(float[] vectorAction, string textAction) {

        StartCoroutine(AgentResetComplited());
        StartCoroutine(WaitForMiniMapRendering());

        float[] agentInput = new float[agentNum * 2];

        int vectoraction_index = 0;

        for (int i = 0; i < agentNum * 2; i++) {
            if (agentRigidbodys[(int)(i / 2)] != null) {
                agentInput[i] = vectorAction[i];
                if (i % 2 == 1) {
                    agentRigidbodys[(int)(i / 2)].AddForce(agentInput[i - 1] * moveForce, 0f, agentInput[i] * moveForce);
                }
                vectoraction_index = i + 1;
            }
        }

        for (int i = 0; i < targetNum * 2; i++) {
            if (targetRigidbodys[(int)(i / 2)] != null) {
                if (targetRigidbodys[(int)(i / 2)] != null) {
                    float randomX = UnityEngine.Random.Range(-1f, 1f);
                    float randomZ = UnityEngine.Random.Range(-1f, 1f);
                    targetRigidbodys[(int)(i / 2)].AddForce(randomX * moveForce, 0f, randomZ * moveForce);
                }
            }
        }

        if (priorEliminatedNum != TargetUtility.eliminatedNum) {
            AddReward((float)(TargetUtility.eliminatedNum - priorEliminatedNum));
            priorEliminatedNum = TargetUtility.eliminatedNum;
        }

        bool isEpisodeEnd = true;
        for (int i = 0; i < targetNum; i++) {
            if (targets[i] != null) {
                isEpisodeEnd = false;
                break;
            }
        }

        if (isEpisodeEnd) {
            Done();

            for (int i = 0; i < agentNum; i++) {
                if (agents[i] == true) {
                    Destroy((agents[i] as Transform).gameObject);
                }
            }
            for (int i = 0; i < targetNum; i++) {
                if (targets[i] == true) {
                    Destroy((targets[i] as Transform).gameObject);
                }
            }
            agents = new Transform[agentNum];
            targets = new Transform[targetNum];
            agentRigidbodys = new Rigidbody[agentNum];
            targetRigidbodys = new Rigidbody[targetNum];

            AgentReset();
        }
    }
}
