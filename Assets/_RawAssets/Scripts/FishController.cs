using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishController : MonoBehaviour
{
    public float moveSpeed = 2;
    public float turnSpeed = 45;
    public Transform xPos = null;
    public Transform zPos = null;
    private Vector3 targetPos = Vector3.zero;

    private float tempSpeed = 1;

    private bool CanMove = true;

    private Animation anim;

    public enum FishType
    {
        Fastest,
        Lazy
    }
    public FishType fishBehaviour;


    private bool isDead=false;
    

    private Rigidbody fishRb;

    private void Awake()
    {
        HitInt = Random.Range(4, 6);
        fishRb=GetComponent<Rigidbody>();
    }
    private void Start()
    {    
        targetPos = new Vector3(Random.Range(xPos.position.x, zPos.position.x), transform.position.y, Random.Range(xPos.position.z, zPos.position.z));
        //Invoke("RandomizePosition", Random.Range(5f, 10f));
        RandomizePosition();
        anim = transform.GetChild(0).GetComponent<Animation>();
        
       
    }

    private bool CanScare = true;

    public void ScareIt()
    {
        if (CanScare == true)
        {
            CanScare = false;
            anim["swim"].speed = 4;
            prevSpeed = tempSpeed;
            if(fishBehaviour==FishType.Lazy)
            {
                tempSpeed = 4;
            }
            else
            {
                tempSpeed = 8;
            }
           
            CancelInvoke("BackToNormal");
            Invoke("BackToNormal", 4f);
        }
    }

    private float prevSpeed = 0;
    public float yPos = 2.5f;
    private void BackToNormal()
    {
        CanScare = true;
        anim["swim"].speed = 1;
        tempSpeed = prevSpeed;
    }

    private void FixedUpdate()
    {
        if(!isDead)
        {
            if (CanMove == true)
            {
                MoveGameObject();

                if (Vector3.Distance(transform.position , targetPos)<2)
                {               
                    RandomizePosition();
                }
            }
            else
            {
                transform.localPosition= new Vector3(transform.localPosition.x,
                    Mathf.MoveTowards(transform.localPosition.y,yPos,0.05f), transform.localPosition.z);

                transform.localScale = Vector3.MoveTowards(transform.localScale, new Vector3(0.7f, 0.7f, 0.7f), 0.05f);
                // GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;            
            }
        }
    }
    private void RandomizePosition()
    {        
        tempSpeed = Random.Range(moveSpeed- 2f, moveSpeed+2f);
        targetPos = new Vector3(Random.Range(xPos.position.x, zPos.position.x), transform.position.y, Random.Range(xPos.position.z, zPos.position.z));
        
        CancelInvoke("RandomizePosition");
        Invoke("RandomizePosition", Random.Range(5f, 10f));
    }

    private void MoveGameObject()
    { 
        Vector3 angle = targetPos - transform.position;
        if (angle.y != 0) angle.y = 0;
        if (targetPos.y != transform.position.y) targetPos.y = transform.position.y;
        if (transform.position != Vector3.MoveTowards(transform.position, targetPos, tempSpeed * Time.deltaTime))
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, tempSpeed * Time.deltaTime);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(angle), turnSpeed * Time.deltaTime);
        }
    }

    public int HitInt = 0;

    public bool IsSeaFish = false;

    private bool fishCatched=false;

    public List<GameObject> attatchedArrows=new List<GameObject>();
   

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag== "Arrow")
        {
            Debug.Log("Fish Caught!!");

            other.gameObject.tag="Untagged";
            attatchedArrows.Add(other.gameObject);

            if (IsSeaFish)
            {
                attatchedArrows.Add(other.gameObject);
                if(GameManager.instance.shootMechanics==GameManager.ShootingType.Rope)
                {
                    fishRb.angularVelocity=Vector3.zero;
                    fishRb.velocity=Vector3.zero;
                    
                    HitInt=0;
                    if(!fishCatched)
                    {
                        isDead=true;

                        StartCoroutine(UnicornBowController.instance.WaitBeforePullingRope());

                        other.gameObject.GetComponent<UnicornArrow>().fishCatched=true;
                        // if(DraggableRopeHandler.instance!=null)
                        // {
                        //     DraggableRopeHandler.instance.UpdateRopeEndAnchor(this.gameObject,other.gameObject);
                        // }

                        if(UnicornArrowRope.instance!=null)
                        {
                            UnicornArrowRope.instance.HandleRopeArrow(this.gameObject);
                        }
                        anim["swim"].speed = 3;
                        fishCatched=false;
                    }

                    if(transform.childCount>=1)
                    {
                        Debug.Log("Has Child!");
                        //transform.GetChild(1).gameObject.SetActive(false);
                    }
                   
                }
                else
                {
                    HitInt--;
                    if (HitInt == 0&&!isDead)
                    {
                        CanMove = false;
                        if(DraggableRopeHandler.instance!=null)
                        {
                            DraggableRopeHandler.instance.UpdateRopeEndAnchor(this.gameObject,other.gameObject);
                        }
                        //Destroy(GetComponent<BoxCollider>()); 
                        //FindObjectOfType<GameManager>().HuntFish(this.gameObject);
                        anim["swim"].speed = 3;
                        //Destroy(this.gameObject, 2.5f);
                        transform.GetChild(1).gameObject.SetActive(false);
                        isDead=true;
                    }
                    //Destroy(other.gameObject.GetComponent<Missile>(), 0.005f);
                    //Destroy(other.gameObject.GetComponent<Collider>());
                    //Destroy(other.gameObject.transform.GetChild(0).gameObject);
                }
                other.gameObject.GetComponent<Collider>().enabled=false;
                //other.gameObject.GetComponent<Missile>().enabled=false;
                other.gameObject.transform.SetParent(transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform);
            }
            else
            {
                CanMove = false;
                Destroy(GetComponent<BoxCollider>());
                Destroy(other.gameObject.GetComponent<Missile>(), 0.005f);
                Destroy(other.gameObject.GetComponent<Collider>());
                Destroy(other.gameObject.transform.GetChild(0).gameObject);
                other.gameObject.transform.SetParent(transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform);

                FindObjectOfType<GameManager>().HuntFish(this.gameObject);
                anim["swim"].speed = 3;
                Destroy(this.gameObject, 2.5f);
            }
            ScareIt();
        }
    }

    public void MakeFishSuffer()
    {
        anim["swim"].speed = 5;
    }

    public void StopFishAnimation()
    {
        transform.GetChild(0).GetComponent<Animation>().enabled=false;
    }
}
