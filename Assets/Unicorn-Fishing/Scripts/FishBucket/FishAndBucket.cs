using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishAndBucket : MonoBehaviour
{
    public static FishAndBucket instance;

    //refrence to the fish that need to be put on bucket
    public Transform fish=null;
    private Rigidbody fishRb;

    public Transform fishBucket=null;
    public Transform fishContainer;
    private Vector3 fishYpos=new Vector3(0f,3f,0f);
    public float fishMoveSpeed=10f;

    private bool animateBucket=true;
    private float bucketAngle=-90f;

    private bool timeToMoveFish=false;
    private Vector3 newFishPos=Vector3.zero;


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


    void FixedUpdate()
    {
        if(animateBucket)
        {
            bucketAngle+=Time.deltaTime*15f;
            if(bucketAngle>=30f)
            {
                animateBucket=false;
            }
            fishBucket.transform.GetChild(0).transform.localRotation=Quaternion.Euler(bucketAngle,0f,0f);
        }

        if(timeToMoveFish)
        {
            fish.transform.position=Vector3.MoveTowards(fish.transform.position,newFishPos,Time.deltaTime*fishMoveSpeed);
            if(Vector3.Distance(fish.transform.position,newFishPos)<=0.1f)
            {
                StartCoroutine(DropFishIntoBucket());
                timeToMoveFish=false;
            }
        }
    }


    public void MoveFishTowardsBucket(Transform targetFish)
    {
        fish=targetFish;
        //fish.GetComponent<FishController>().enabled=false;
        fishRb=fish.GetComponent<Rigidbody>();
        fishRb.velocity=Vector3.zero;
        fish.rotation=Quaternion.Euler(0f,90f,0f);
        fishRb.isKinematic=true;
        if(GameManager.instance!=null)
        {
            GameManager.instance.HuntFish(fish.gameObject);
        }
        newFishPos=fishContainer.transform.position+fishYpos;
        timeToMoveFish=true;

        //lets disable all the arrows
        for(int i=0;i<fish.GetComponent<FishController>().attatchedArrows.Count;i++)
        {
            fish.GetComponent<FishController>().attatchedArrows[i].SetActive(false);
        }
    }

    IEnumerator DropFishIntoBucket()
    {
        //fish.transform.SetParent(fishContainer.transform);
        //fish.transform.localPosition=Vector3.zero;
        fish.GetComponent<BoxCollider>().isTrigger=false;
        fish.gameObject.tag="Untagged";
        fish.GetComponent<FishController>().MakeFishSuffer();
        fishRb.mass=1f;
        yield return new WaitForSeconds(3f);
        fish.GetComponent<FishController>().StopFishAnimation();
        fishRb.isKinematic=false;
        fishRb.useGravity=true;
        GameManager.instance.ResetThinsForNextHunt();
    }
}
