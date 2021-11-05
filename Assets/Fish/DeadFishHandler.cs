using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadFishHandler : MonoBehaviour
{
    public static DeadFishHandler instance;

    [Header("Refrence To Recently Killed Fish")]
    public GameObject deadFish;
    public Transform fishContainer;

    void Start()
    {
        if(instance!=null)
        {
            return;
        }
        else
        {
            instance=this;
        }
    }


    private bool moveFishUp=false;
    private bool moveFishTowardsBasket=false;

    private Vector3 targetPos;

    private Vector3 fishDisplayPos;
    private float upPos=15f;


    void FixedUpdate()
    {
        if(deadFish!=null)
        {
            if(moveFishUp)
            {
                deadFish.transform.position=Vector3.MoveTowards(deadFish.transform.position,fishDisplayPos,Time.deltaTime*20f);
                if(Vector3.Distance(deadFish.transform.position,fishDisplayPos)<=0.1f)
                {
                    StartCoroutine(TimeToPutFishIntoBasket());
                    moveFishUp=false;
                }
            }

            if(moveFishTowardsBasket)
            {
                deadFish.transform.position=Vector3.MoveTowards(deadFish.transform.position,targetPos,Time.deltaTime*20f);
                if(Vector3.Distance(deadFish.transform.position,targetPos)<=0.1f)
                {
                    StartCoroutine(DropTheFish());
                    moveFishTowardsBasket=false;
                }
            }
        }
    }

    IEnumerator TimeToPutFishIntoBasket()
    {
        yield return new WaitForSeconds(0.5f);
        targetPos=fishContainer.position+new Vector3(0f,4f,0f);
        moveFishTowardsBasket=true;
    }

    IEnumerator DropTheFish()
    {
        yield return new WaitForSeconds(0.1f);
        deadFish.GetComponent<Rigidbody>().useGravity=true;
        deadFish.GetComponent<FishBehaviourHandler>().StopFishAnimation();
        deadFish.GetComponent<Rigidbody>().velocity=Vector3.zero;
        deadFish.transform.SetParent(fishContainer);
        deadFish.GetComponent<BoxCollider>().isTrigger=false;
        deadFish.transform.localPosition=new Vector3(0f,deadFish.transform.localPosition.y,0f);
        yield return new WaitForSeconds(1f);
        GameManager.instance.ResetThinsForNextHunt();
    }



    public void HandleDeadFish(GameObject fish)
    {
        deadFish=fish;
        fishDisplayPos=deadFish.transform.position+new Vector3(0f,upPos,0f);
        deadFish.transform.rotation=Quaternion.Euler(0f,0f,0f);
        deadFish.GetComponent<Rigidbody>().velocity=Vector3.zero;
        deadFish.GetComponent<Rigidbody>().useGravity=false;
        GameManager.instance.HuntFish(deadFish);
        moveFishUp=true;
    }

    
}
