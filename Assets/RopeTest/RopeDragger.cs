using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeDragger : MonoBehaviour
{
    private float rotationSpeed=100f;
    private float curAngle=0f;
  

    // Update is called once per frame
    void Update()
    {
        curAngle-=Time.deltaTime*rotationSpeed;
        transform.localRotation=Quaternion.Euler(0f,curAngle,0f);
    }
}
