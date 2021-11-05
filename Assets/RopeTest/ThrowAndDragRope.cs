using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Obi;

public class ThrowAndDragRope : MonoBehaviour
{
    public ObiSolver solver;
    public ObiCollider character;
    public float hookExtendRetractSpeed = 2;

    public Material material;
    public ObiRopeSection section;

    private ObiRope rope;
    private ObiRopeBlueprint blueprint;
    private ObiRopeExtrudedRenderer ropeRenderer;

    private ObiRopeCursor cursor;


    public Transform initialPoint;
    public Transform target;

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
        blueprint.thickness = 0.06f;

        // Add a cursor to be able to change rope length:
        cursor = rope.gameObject.AddComponent<ObiRopeCursor>();
        cursor.cursorMu = 0;
        cursor.direction = true;
    }

    private HingeJoint fishHinge;
    public void UpdateRopeEndAnchor(GameObject Fish,GameObject Arrow)
    {
        // rope.ropeBlueprint = null;
        Fish.GetComponent<FishController>().enabled=false;
        Arrow.GetComponent<Missile>().enabled=false;

       
        // if(Arrow.GetComponent<Rigidbody>()==null)
        // {
        //     Arrow.AddComponent<Rigidbody>();
        //     Arrow.AddComponent<ObiRigidbody>();
        // }

        // if(Fish.GetComponent<HingeJoint>()==null)
        // {
        //     fishHinge=Fish.AddComponent<HingeJoint>();
        //     fishHinge.connectedBody=Arrow.GetComponent<Rigidbody>();
        // }
        target=Fish.transform;
        
        LaunchHook(Fish);

    }

    public Transform bulletPrefab;

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            //Transform newTarget=Instantiate(bulletPrefab,initialPoint.position,Quaternion.identity);
            LaunchHook(target.gameObject);
        }

        if (rope.isLoaded)
        {
            if (Input.GetKey(KeyCode.Q))
            {
                cursor.ChangeLength(rope.restLength - hookExtendRetractSpeed * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.E))
            {
                cursor.ChangeLength(rope.restLength + hookExtendRetractSpeed * Time.deltaTime);
            }
        }
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
        // batch.AddConstraint(rope.solverIndices[0], initialPoint.GetComponent<ObiColliderBase>(), transform.localPosition, Quaternion.identity, 0, 0, float.PositiveInfinity);
        // batch.AddConstraint(rope.solverIndices[blueprint.activeParticleCount - 1], target.GetComponent<ObiColliderBase>(),
        //                                                   new Vector3(0f,0f,0f), Quaternion.identity, 0, 0, float.PositiveInfinity);

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
}
