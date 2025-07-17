using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Transform player;

    void Start()
    {
        
    }

    void LateUpdate()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        Vector3 temp = transform.position;
        temp.x = player.position.x;
        temp.y = player.position.y;

        transform.position = temp;
    }
}
