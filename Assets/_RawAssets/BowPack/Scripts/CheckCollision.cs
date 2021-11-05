using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckCollision : MonoBehaviour
{
    private GameObject WaterPlane;

    private void Awake()
    {
        WaterPlane = GameObject.Find("WaterPlane");
    }
    private bool LoopOnce = false;
    private void Update()
    {
        if(transform.position.y<= WaterPlane.transform.position.y)
        {
            if (LoopOnce == false)
            {
                GameObject SplashObj = Instantiate(Resources.Load("Splash 1")) as GameObject;
                SplashObj.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                SplashObj.transform.position = new Vector3(transform.position.x, WaterPlane.transform.position.y, transform.position.z) ;
                Destroy(SplashObj, 2f);
                LoopOnce = true;
                Destroy(this.gameObject,2.1f);
            }
        }
    }
}
