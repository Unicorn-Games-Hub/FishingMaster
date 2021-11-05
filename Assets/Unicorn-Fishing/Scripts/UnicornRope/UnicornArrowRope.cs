using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Obi;

public class UnicornArrowRope : MonoBehaviour
{
    public static UnicornArrowRope instance;

    public Transform ropeInitialPoint;

    [Header("Obi Solver")]
    public ObiSolver solver;
    public float hookExtendRetractSpeed = 2;

    [Header("Rope Material")]
    public Material material;
    public ObiRopeSection section;

    [Header("Rope Radius")]
    public float ropeRadius=0.03f;
    private ObiRope rope;
    private ObiRopeBlueprint blueprint;
    private ObiRopeExtrudedRenderer ropeRenderer;
    private ObiRopeCursor cursor;

    private Transform target;

    //
    public bool canPullRope=false;

    public Transform player;

    public GameObject arrowRope;

    void Awake()
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

    void Start()
    {
        rope = gameObject.AddComponent<ObiRope>();
        ropeRenderer = gameObject.AddComponent<ObiRopeExtrudedRenderer>();
        ropeRenderer.section = section;
        ropeRenderer.uvScale = new Vector2(1, 5);
        ropeRenderer.normalizeV = false;
        ropeRenderer.uvAnchor = 1;
        rope.GetComponent<MeshRenderer>().material = material;
        rope.maxBending = 0.02f;

        blueprint = ScriptableObject.CreateInstance<ObiRopeBlueprint>();
        blueprint.resolution = 0.5f;
        blueprint.thickness = ropeRadius;

        // Add a cursor to be able to change rope length:
        cursor = rope.gameObject.AddComponent<ObiRopeCursor>();
        cursor.cursorMu = 0;
        cursor.direction = true;
    }

    void Update()
    {
        if(rope.isLoaded)
        {
            // if (Input.GetKey(KeyCode.Q))
            // {
            //     cursor.ChangeLength(rope.restLength - hookExtendRetractSpeed * Time.deltaTime);
            // }
            // if (Input.GetKey(KeyCode.E))
            // {
            //     cursor.ChangeLength(rope.restLength + hookExtendRetractSpeed * Time.deltaTime);
            // }

            if(canPullRope)
            {
                if(rope.restLength>=0.3f)
                {
                    cursor.ChangeLength(rope.restLength - hookExtendRetractSpeed * Time.deltaTime);
                }
                else
                {
                    DetatchRope();
                    if(UnicornBowController.instance!=null)
                    {
                        player.GetComponent<FirstPersonController>().enabled=false;
                        UnicornBowController.instance.StopPullingRope();
                        FishAndBucket.instance.MoveFishTowardsBucket(target);
                    }
                    canPullRope=false;
                }
            }
        }
    }

    #region Rope
    public void HandleRopeArrow(GameObject ropeTarget)
    {
        target=ropeTarget.transform;
        StartCoroutine(AttatchRopeToArrow(ropeInitialPoint.position,ropeTarget.transform.position));
        player.GetComponent<FirstPersonController>().enabled = false;
        StartCoroutine(DetatchArrowRope());
    }
    #endregion

    #region Attatch Rope
    public IEnumerator AttatchRopeToArrow(Vector3 pointA,Vector3 pointB)
    {   
        pointA=rope.transform.InverseTransformPoint(pointA);
        pointB=rope.transform.InverseTransformPoint(pointB);
        Vector3 direction = (pointB - pointA) * 0.25f;
         
        blueprint.path.Clear();
        blueprint.path.AddControlPoint(pointA, -direction.normalized, direction.normalized, Vector3.up, 0.1f, 0.1f, 1, 1, Color.white, "Hook start");
        blueprint.path.AddControlPoint(pointB, -direction.normalized, direction.normalized, Vector3.up, 0.1f, 0.1f, 1, 1, Color.white, "Hook end");
        blueprint.path.FlushEvents();

        yield return blueprint.Generate();
        
        rope.ropeBlueprint = blueprint;
        rope.GetComponent<MeshRenderer>().enabled = true;

        // Pin both ends of the rope (this enables two-way interaction between character and rope):
        var pinConstraints = rope.GetConstraintsByType(Oni.ConstraintType.Pin) as ObiConstraints<ObiPinConstraintsBatch>;
        pinConstraints.Clear();
        var batch = new ObiPinConstraintsBatch();
        batch.AddConstraint(rope.solverIndices[0], ropeInitialPoint.GetComponent<ObiColliderBase>(), transform.localPosition, Quaternion.identity, 0, 0, float.PositiveInfinity);
        batch.AddConstraint(rope.solverIndices[blueprint.activeParticleCount - 1], target.GetComponent<ObiColliderBase>(),
                                                          new Vector3(0f,0f,0f), Quaternion.identity, 0, 0, float.PositiveInfinity);
        batch.activeConstraintCount = 2;
        pinConstraints.AddBatch(batch);
        rope.SetConstraintsDirty(Oni.ConstraintType.Pin);
    }
    #endregion

    #region Detatch Rope
    public void DetatchRope()
    {
        rope.ropeBlueprint = null;
        rope.GetComponent<MeshRenderer>().enabled = false;
        player.GetComponent<FirstPersonController>().enabled = true;
    }

    IEnumerator DetatchArrowRope()
    {
        yield return new WaitForEndOfFrame();
        if(BowRopeHandler.instance!=null)
        {
            BowRopeHandler.instance.DetachHook();
        }
    }
    #endregion
}
