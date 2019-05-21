using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetUtility : MonoBehaviour {
    public static int eliminatedNum = 0;
    private bool isCollision = false;
    void OnCollisionEnter(Collision other) {
        if (other.transform.tag == "Player" && !isCollision) {
            isCollision = true;
            TargetUtility.eliminatedNum++;

            GameObject.Destroy(gameObject);
        }
    }
}
