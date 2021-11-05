using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Obi;

public class DraggableRopeHandler : MonoBehaviour
{
    public static DraggableRopeHandler instance;

    public ObiSolver solver;
    public ObiCollider character;
    public float hookExtendRetractSpeed = 2;

    public Material material;
    public ObiRopeSection section;

    public float ropeRadius=0.03f;

    private ObiRope rope;
    private ObiRopeBlueprint blueprint;
    private ObiRopeExtrudedRenderer ropeRenderer;

    private ObiRopeCursor cursor;


    public Transform initialPoint;
    private Transform target;

    public Transform arrowPrefab;


    public GameObject bowRope;

    [Header("Arm Mesh")]
    public GameObject fishingArm;
    public GameObject fishingBow;
    public GameObject pullingHand;

    //
    public Transform player;

    void Awake()
    {
        // Create both the rope and the solver:	
        rope = gameObject.AddComponent<ObiRope>();
        ropeRenderer = gameObject.AddComponent<ObiRopeExtrudedRenderer>();
        ropeRenderer.section = section;
        ropeRenderer.uvScale = new Vector2(1, 5);
        ropeRenderer.normalizeV = false;
        ropeRenderer.uvAnchor = 1;
        rope.GetComponent<MeshRenderer>().material = material;

        // Setup a blueprint for the rope:
        blueprint = ScriptableObject.CreateInstance<ObiRopeBlueprint>();
        blueprint.resolution = 0.5f;

        // Tweak rope parameters:
        rope.maxBending = 0.02f;

        //
        blueprint.thickness = ropeRadius;

        // Add a cursor to be able to change rope length:
        cursor = rope.gameObject.AddComponent<ObiRopeCursor>();
        cursor.cursorMu = 0;
        cursor.direction = true;
    }

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

        if(pullingHand!=null)
        {
            pullingHand.SetActive(false);
        }
    }

    private HingeJoint fishHinge;
    private Rigidbody arrowRb;
    public void UpdateRopeEndAnchor(GameObject Fish,GameObject Arrow)
    {  
        currentFish=Fish;
        if(Fish.GetComponent<ObiRigidbody>()==null)
        {
            Fish.AddComponent<ObiRigidbody>();
            Fish.AddComponent<ObiCollider>();
        }

        if(GameManager.instance.shootMechanics==GameManager.ShootingType.Rope)
        {
            StartCoroutine(DetatchRope());
            Fish.GetComponent<FishController>().enabled=false;
            Fish.GetComponent<Rigidbody>().mass=5f;
            Fish.GetComponent<Rigidbody>().velocity=Vector3.zero;
            //Arrow.GetComponent<Missile>().enabled=false;
            LaunchHook(Fish);
            ShowFishPullingFromSide();
            DisableArrows();
            fishShown=false;
        }
        else
        {
            HandleFishDeath();
        }
    }

    #region pulling the fish
    private bool canPullRope=false;
    IEnumerator PullTheRope()
    {
        yield return new WaitForSeconds(2f);
        if(pullingHand.transform.GetChild(0).GetComponent<Animator>()!=null)
        {
            pullingHand.transform.GetChild(0).GetComponent<Animator>().SetBool("pull",true);
        }
        canPullRope=true;
    }
    #endregion

    IEnumerator DetatchRope()
    {
        yield return new WaitForEndOfFrame();
        if(BowRopeHandler.instance!=null)
        {
            BowRopeHandler.instance.DetachHook();
        }
    }

    void Update()
    {
        if (rope.isLoaded)
        {
            // if(rope.restLength<=1f)
            // {
            //     ShowFishDeathAnimation();
            // }
           
            if (Input.GetKey(KeyCode.Q))
            {
                cursor.ChangeLength(rope.restLength - hookExtendRetractSpeed * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.E))
            {
                cursor.ChangeLength(rope.restLength + hookExtendRetractSpeed * Time.deltaTime);
            }

            if(canPullRope)
            {
                if(rope.restLength>=0.1f)
                {
                    cursor.ChangeLength(rope.restLength - hookExtendRetractSpeed * Time.deltaTime);
                }
                else
                {
                    pullingHand.SetActive(false);
                    //DetachHook();
                    StopFishPulling();
                    canPullRope=false;
                }
            }
        }
    }

    private bool moveFishUpward=false;
    private Vector3 upwardTargetPos;

    void FixedUpdate()
    {
        if(moveFishUpward)
        {
            currentFish.transform.position=Vector3.MoveTowards(currentFish.transform.position,upwardTargetPos,Time.deltaTime*10f);
            if(Vector3.Distance(currentFish.transform.position,upwardTargetPos)<=0.1f)
            {
                StartCoroutine(TimeToMoveFishTowardsBasket());
                moveFishUpward=false;
            }
        }

        if(moveFishToBasket)
        {
            currentFish.transform.position=Vector3.MoveTowards(currentFish.transform.position,fishTargetPos,Time.deltaTime*10f);
            if(Vector3.Distance(currentFish.transform.position,fishTargetPos)<0.1f)
            {
                StartCoroutine(DropTheFish());
                moveFishToBasket=false;
            }
        }
    }

    //put fish on basket
    public Transform fishContainer;
    private GameObject currentFish=null;
    private bool fishShown=false;

    void HandleFishDeath()
    {
        if(GameManager.instance.shootMechanics==GameManager.ShootingType.Rope)
        {
            StartCoroutine(PutFishOnBasket());
        }
        else
        {
            StartCoroutine(ShowFish());
        }
        DisableArrows();
    }

    void DisableArrows()
    {
         if(currentFish.GetComponent<FishController>().attatchedArrows.Count>0)
        {
            for(int i=0;i<currentFish.GetComponent<FishController>().attatchedArrows.Count;i++)
            {
                currentFish.GetComponent<FishController>().attatchedArrows[i].SetActive(false);
            }
        }
    }


    //move FishTowards basket;

    public void ShowFishDeathAnimation()
    {
        if(!fishShown)
        {
            DetachHook();
            StartCoroutine(PutFishOnBasket());
            fishShown=true;
        }
    }

    //
    private bool moveFishToBasket=false;
    private Vector3 fishTargetPos;

    IEnumerator ShowFish()
    {
        yield return new WaitForSeconds(1f);
        //currentFish.transform.localScale=new Vector3(0.4f,0.4f,0.4f);
        upwardTargetPos=currentFish.transform.position+new Vector3(0f,4f,0f);
        //currentFish.transform.position=new Vector3(currentFish.transform.position.x,currentFish.transform.position.y+4f,currentFish.transform.position.z);
        currentFish.GetComponent<FishController>().MakeFishSuffer();
        currentFish.GetComponent<Rigidbody>().velocity=Vector3.zero;
        currentFish.transform.rotation=Quaternion.Euler(0f,0f,0f);
        currentFish.GetComponent<Rigidbody>().isKinematic=true;
        GameManager.instance.HuntFish(currentFish);
        fishTargetPos=fishContainer.position+new Vector3(0f,4f,0f);
        moveFishUpward=true;
    }

    IEnumerator TimeToMoveFishTowardsBasket()
    {
        yield return new WaitForSeconds(2f);
        currentFish.GetComponent<FishController>().StopFishAnimation();
        moveFishToBasket=true;
    }

    IEnumerator DropTheFish()
    {
        currentFish.GetComponent<Rigidbody>().velocity=Vector3.zero;
        currentFish.transform.SetParent(fishContainer);
        currentFish.transform.localPosition=new Vector3(0f,currentFish.transform.localPosition.y,0f);
        currentFish.GetComponent<Rigidbody>().isKinematic=false;
        currentFish.GetComponent<Rigidbody>().useGravity=true;
        currentFish.GetComponent<BoxCollider>().isTrigger=false;
        yield return new WaitForSeconds(2f);
        GameManager.instance.ResetThinsForNextHunt();
    }


    IEnumerator PutFishOnBasket()
    {
        yield return new WaitForSeconds(1f);
        currentFish.transform.localScale=new Vector3(0.4f,0.4f,0.4f);
        //currentFish.GetComponent<Rigidbody>().velocity=Vector3.zero;
        currentFish.GetComponent<Rigidbody>().isKinematic=true;

        currentFish.transform.SetParent(fishContainer);
        currentFish.transform.localPosition=new Vector3(0f,3f,0f);
        
        currentFish.transform.rotation=Quaternion.Euler(0f,0f,0f);
        GameManager.instance.HuntFish(currentFish);
        currentFish.GetComponent<FishController>().MakeFishSuffer();
        yield return new WaitForSeconds(3f);
        currentFish.GetComponent<FishController>().StopFishAnimation();
        currentFish.GetComponent<Rigidbody>().isKinematic=false;
        yield return new WaitForSeconds(1f);
        GameManager.instance.ResetThinsForNextHunt();
    }


    public void LaunchHook(GameObject tempTarget)
    {
        target=tempTarget.transform;
        StartCoroutine(AttachHook(initialPoint.position,target.position));
    }

    private bool hookDetatched=false;

    public void DetachHook()
    {
        if(!hookDetatched)
        {
            rope.ropeBlueprint = null;
            rope.GetComponent<MeshRenderer>().enabled = false;
            hookDetatched=true;
        }
    }

    #region  Attach Hook
    private IEnumerator AttachHook(Vector3 pointA,Vector3 pointB)
    {
        yield return 0;
        pointA=rope.transform.InverseTransformPoint(pointA);
        pointB=rope.transform.InverseTransformPoint(pointB);
        Vector3 direction = (pointB - pointA) * 0.25f;

        // Procedurally generate the rope path (a simple straight line):
        blueprint.path.Clear();
        blueprint.path.AddControlPoint(pointA, -direction.normalized, direction.normalized, Vector3.up, 0.1f, 0.1f, 1, 1, Color.white, "Hook start");
        blueprint.path.AddControlPoint(pointB, -direction.normalized, direction.normalized, Vector3.up, 0.1f, 0.1f, 1, 1, Color.white, "Hook end");
        blueprint.path.FlushEvents();

        // Generate the particle representation of the rope (wait until it has finished):
        yield return blueprint.Generate();
        
        // Set the blueprint (this adds particles/constraints to the solver and starts simulating them).
        rope.ropeBlueprint = blueprint;
        rope.GetComponent<MeshRenderer>().enabled = true;

        // Pin both ends of the rope (this enables two-way interaction between character and rope):
        var pinConstraints = rope.GetConstraintsByType(Oni.ConstraintType.Pin) as ObiConstraints<ObiPinConstraintsBatch>;
        pinConstraints.Clear();
        var batch = new ObiPinConstraintsBatch();
        batch.AddConstraint(rope.solverIndices[0], initialPoint.GetComponent<ObiColliderBase>(), transform.localPosition, Quaternion.identity, 0, 0, float.PositiveInfinity);
        batch.AddConstraint(rope.solverIndices[blueprint.activeParticleCount - 1], target.GetComponent<ObiColliderBase>(),
                                                          new Vector3(0f,0f,0f), Quaternion.identity, 0, 0, float.PositiveInfinity);
        batch.activeConstraintCount = 2;
        pinConstraints.AddBatch(batch);

        rope.SetConstraintsDirty(Oni.ConstraintType.Pin);

        //
        hookDetatched=false;
    }
    #endregion

    #region  Generating new arrow rigidbody
    public void GenerateArrowRigidbody(GameObject oldArrow)
    {
        Transform arrow=Instantiate(arrowPrefab,oldArrow.transform.position,oldArrow.transform.rotation);
        oldArrow.SetActive(false);
        arrow.GetComponent<Rigidbody>().useGravity=false;
        LaunchHook(arrow.gameObject);
    }
    #endregion


    #region  Showing camera from Side
    public GameObject fishPullCam;
    public GameObject playerCam;
    void ShowFishPullingFromSide()
    {
        fishingArm.SetActive(false);
        fishingBow.SetActive(false);
        pullingHand.SetActive(true);
        player.GetComponent<FirstPersonController>().enabled = false;
        playerCam.GetComponent<Camera>().enabled=false;
        fishPullCam.SetActive(true);
        StartCoroutine(PullTheRope());
    }

    void StopFishPulling()
    {
        DetachHook();
        currentFish.GetComponent<Rigidbody>().velocity=Vector3.zero;
    }
    #endregion
}
