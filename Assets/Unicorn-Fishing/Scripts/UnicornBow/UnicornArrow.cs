using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnicornArrow : MonoBehaviour
{
    public float arrowSpeed=20f;
    private Vector3 velocity=Vector3.zero;
    private Vector3 arrowNewPos=Vector3.zero;
    private bool move=false;

    public bool fishCatched=false;
    private float distanceTravelled=0f;

    //
    private Rigidbody arrowRb;

    public void HandleArrowInfo(Vector3 direction)
    {
        arrowNewPos=transform.position;
        velocity=arrowSpeed*direction;
        move=true;
    }

    void Update()
    {
        if(move)
        {
            arrowNewPos+=velocity*Time.deltaTime;
            transform.position=arrowNewPos;
            distanceTravelled+=Time.deltaTime;
            if(distanceTravelled>=2f&&!fishCatched)
            {
                AddComponentsToArrow();
                move=false;
            }
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.tag=="Generic")
        {
            fishCatched=true;
            move=false;
        }
    }

    void AddComponentsToArrow()
    {
        GetComponent<BoxCollider>().isTrigger=false;
        // arrowRb=transform.GetChild(0).GetChild(0).gameObject.AddComponent<Rigidbody>();
        // arrowRb.useGravity=false;
        // arrowRb.velocity=Vector3.zero;
    }
}
