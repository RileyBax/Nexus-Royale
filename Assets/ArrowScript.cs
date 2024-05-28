using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowScript : MonoBehaviour
{

    public Vector2 zone;
    public Vector2 temp;
    public Vector3 playerPos;

    // Update is called once per frame
    void Update()
    {
        temp = Vector2.zero;

        float angle = -(float)Math.Atan2(zone.y - playerPos.y, zone.x - playerPos.x) + 1.55f;

        temp.x = (float)(playerPos.x + Math.Sin(angle) * 5);
        temp.y = (float)(playerPos.y + Math.Cos(angle) * 5);

        this.transform.position = temp;
        this.transform.right = new Vector2(zone.x - playerPos.x, zone.y - playerPos.y);

    }
}
