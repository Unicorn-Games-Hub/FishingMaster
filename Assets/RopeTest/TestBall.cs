using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Obi;

public class TestBall : MonoBehaviour
{
    private bool isMovable=true;
    public float speed=5f;

    void Start()
    {
        StartCoroutine(StopTheMovement());
    }

    void Update()
    {
        if(isMovable)
        {
            transform.Translate(Vector3.forward*Time.deltaTime*speed);
        }
    }

    private Rigidbody rb;

    IEnumerator StopTheMovement()
    {
        yield return new WaitForSeconds(2f);
        isMovable=false;
        //lets add rigidbody
        rb=this.gameObject.AddComponent<Rigidbody>();
        this.gameObject.AddComponent<ObiRigidbody>();
    }
}
