using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallScript : MonoBehaviour
{
    private Rigidbody2D rb;
    //private CircleCollider2D cc;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.AddForce(new Vector2(9.8f * 25f, 9.8f * 25f));
    }

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    cc.radius = -0.1f;
    //}


}
