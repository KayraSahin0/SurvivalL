using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallsScript : MonoBehaviour
{
    public GameObject upWall;
    public GameObject rightWall;
    public GameObject downWall;
    public GameObject leftWall;

    private float upMove;
    private float rightMove;
    private float downMove;
    private float leftMove;

    private void Update()
    {
        upMove = (float)upWall.transform.position.y;
        rightMove = (float)upWall.transform.position.x;
        downMove = (float)upWall.transform.position.y;
        leftMove = (float)upWall.transform.position.x;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        upMove = -0.1f;
        rightMove = -0.1f;
        downMove = 0.1f;
        leftMove = 0.1f;
    }

    
}
