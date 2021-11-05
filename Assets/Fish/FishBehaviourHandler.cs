using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishBehaviourHandler : MonoBehaviour
{
    private FlockUnit fu;

    private Animation anim;

    void Start()
    {
        fu=GetComponent<FlockUnit>();
        anim=transform.GetChild(0).GetComponent<Animation>();
    }

    void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.tag=="Arrow")
        {
            col.gameObject.tag="Untagged";
            MakeFishSuffer();
            if(fu!=null)
            {
                fu.isAlive=false;
            }
            //
            if(DeadFishHandler.instance!=null)
            {
                DeadFishHandler.instance.HandleDeadFish(this.gameObject);
            }
        }
    }

    void MakeFishSuffer()
    {
       anim["swim"].speed = 4;
    }

    public void StopFishAnimation()
    {
        anim["swim"].speed = 0;
    }


}
