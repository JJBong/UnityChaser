using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextLocator : MonoBehaviour
{
    public GameObject Canvas;
    public Text countText;
    public Text miniMapText;

    float timer;
    private const float fps = .5f;
   
    private void DrawMiniMapText()
    {
        miniMapText.text = "RedBall's MiniMap";
        if (MapGenerator.currentMapSize.x <= 22)
        {
            miniMapText.fontSize = 11;
        }
        else
        {
            miniMapText.fontSize = (int)(MapGenerator.currentMapSize.x / 2 - 1);
        }
        float x_pos = MapGenerator.currentMapSize.x * 5 / 3;
        float z_pos = MapGenerator.currentMapSize.y / 4;
        miniMapText.transform.position = new Vector3(x_pos, 0, z_pos);
        miniMapText.color = Color.magenta;
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer > fps && MapGenerator.mapCreated) {
            DrawMiniMapText();
            countText.text = "BlueBall Survivors: " + (BallAgent.targetNum - TargetUtility.eliminatedNum).ToString("G");
            if (MapGenerator.currentMapSize.x <= 24) {
                countText.fontSize = 12;
            }
            else {
                countText.fontSize = (int)(MapGenerator.currentMapSize.x / 2);
            }
            
            float x_pos = MapGenerator.currentMapSize.x * 5 / 3;
            float z_pos = MapGenerator.currentMapSize.y / 2;
            countText.transform.position = new Vector3(x_pos, 0, z_pos);
            countText.color = Color.white;
            timer = 0f;
        }
    }
}
