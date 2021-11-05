using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Obi;

public class UnicornGameRope3 : MonoBehaviour
{
    public static UnicornGameRope3 instance;

    public bool isRopeGenerated=false;

    public Transform initialAnchorPoint;
    private Transform endAnchorPoint;

    //public Transform targetPoint;


    //rope main
    private ObiSolver solver; 
    private ObiRope rope;

    //
    public Material ropeMaterial;
    //
    private ObiRopeBlueprint blueprint;
    private ObiRopeExtrudedRenderer ropeRenderer;
    //
    public ObiRopeSection section;
    //
    private ObiRopeCursor cursor;
    //
    [Range(0.001f,1f)]
    public float ropeRadius=0.01f;

    public float hookExtendRetractSpeed = 2;

    void Awake()
    {
        //create an object containing both the solver and the updater:
        GameObject solverObject = new GameObject("solver", typeof(ObiSolver), typeof(ObiFixedUpdater));
		solver = solverObject.GetComponent<ObiSolver>();
		ObiFixedUpdater updater = solverObject.GetComponent<ObiFixedUpdater>();
        updater.substeps = 2;

        //adjust solver settings:
        solver.particleCollisionConstraintParameters.enabled = false;
        solver.distanceConstraintParameters.iterations = 8;
        solver.pinConstraintParameters.iterations = 4;
        solver.parameters.sleepThreshold = 0.001f;
        solver.PushSolverParameters();

        //add the solver to the updater:
		updater.solvers.Add(solver);

        CreateNewRope();
    }

    void CreateNewRope()
    {
        // Create both the rope and the solver:	
        rope = gameObject.AddComponent<ObiRope>();
        ropeRenderer = gameObject.AddComponent<ObiRopeExtrudedRenderer>();
        ropeRenderer.section = section;
        ropeRenderer.uvScale = new Vector2(1, 20);
        ropeRenderer.normalizeV = false;
        ropeRenderer.uvAnchor = 1;
        rope.GetComponent<MeshRenderer>().material = ropeMaterial;

        // Setup a blueprint for the rope:
        blueprint = ScriptableObject.CreateInstance<ObiRopeBlueprint>();
        blueprint.resolution = 0.5f;

        blueprint.thickness = ropeRadius;

        // Tweak rope parameters:
        rope.maxBending = 0.02f;
        rope.transform.parent = solver.transform;

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
    }

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.P)&&rope!=null)
        {
            DetatchRope();
        }
        if(!isRopeDetatched)
        {
            PullTheFish();
        }
    }

    public void UpdateRopeAnchor(GameObject arrow)
    {
        if(!isRopeGenerated)
        {
            arrow.AddComponent<ObiCollider>();
            //endAnchorPoint=targetPoint;
            endAnchorPoint=arrow.transform;
            StartCoroutine(AttachRope(initialAnchorPoint.position,endAnchorPoint.transform.position,initialAnchorPoint.gameObject.GetComponent<ObiCollider>(),endAnchorPoint.GetComponent<ObiCollider>()));
            isRopeGenerated=true;
        }
    }

    #region Rope
    void CreateRope(ObiSolver solver)
    {
        rope = CreateRope(initialAnchorPoint.position,endAnchorPoint.transform.position);
        rope.transform.parent = solver.transform;
        PinRope(rope,initialAnchorPoint.gameObject.GetComponent<ObiCollider>(), endAnchorPoint.GetComponent<ObiCollider>(), new Vector3(0, 0f, 0), -new Vector3(0, 0f, 0));
    }

    private void PinRope(ObiRope rope, ObiCollider bodyA, ObiCollider bodyB, Vector3 offsetA, Vector3 offsetB)
    {
        // Pin both ends of the rope (this enables two-way interaction between character and rope):
        var pinConstraints = rope.GetConstraintsByType(Oni.ConstraintType.Pin) as ObiConstraints<ObiPinConstraintsBatch>;
        pinConstraints.Clear();
        var batch = new ObiPinConstraintsBatch();
        batch.AddConstraint(rope.solverIndices[0], bodyA, offsetA, Quaternion.identity, 0, 0f, float.PositiveInfinity);
        batch.AddConstraint(rope.solverIndices[rope.activeParticleCount - 1], bodyB, offsetB, Quaternion.identity, 0, 0f, float.PositiveInfinity);
        batch.activeConstraintCount =2;
        pinConstraints.AddBatch(batch);
    }

	private ObiRope CreateRope(Vector3 pointA, Vector3 pointB)
	{
		// Create a rope
        var ropeObject = new GameObject("solver", typeof(ObiRope), typeof(ObiRopeLineRenderer));
        var rope = ropeObject.GetComponent<ObiRope>();
		var ropeRenderer = ropeObject.GetComponent<ObiRopeLineRenderer>();
		rope.GetComponent<MeshRenderer>().material = ropeMaterial;
        rope.GetComponent<ObiPathSmoother>().decimation = 0.1f;
        
        ropeRenderer.uvScale = new Vector2(1, 20);

		// Setup a blueprint for the rope:
		var blueprint = ScriptableObject.CreateInstance<ObiRopeBlueprint>();
		blueprint.resolution = 0.15f;
        blueprint.thickness = 0.02f;
        //blueprint.thickness = ropeRadius;
        blueprint.pooledParticles = 0;

        //convert both points to the rope's local space:
        pointA = rope.transform.InverseTransformPoint(pointA);
		pointB = rope.transform.InverseTransformPoint(pointB);

		//Procedurally generate the rope path (a simple straight line):
		Vector3 direction = (pointB - pointA) * 0.25f;
		blueprint.path.Clear();
		blueprint.path.AddControlPoint(pointA, -direction.normalized, direction.normalized, Vector3.up, 0.1f, 0.1f, 1, 1, Color.white, "A");
		blueprint.path.AddControlPoint(pointB, -direction.normalized, direction.normalized, Vector3.up, 0.1f, 0.1f, 1, 1, Color.white, "B");
		blueprint.path.FlushEvents();

		rope.ropeBlueprint = blueprint;
		return rope;
	}
    #endregion

    #region Attaching Rope
    private IEnumerator AttachRope(Vector3 pointA,Vector3 pointB,ObiCollider bodyA,ObiCollider bodyB)
    {
        //convert both points to the rope's local space:
        pointA = rope.transform.InverseTransformPoint(pointA);
		pointB = rope.transform.InverseTransformPoint(pointB);

		//Procedurally generate the rope path (a simple straight line):
		Vector3 direction = (pointB - pointA) * 0.25f;
        blueprint.path.Clear();
		blueprint.path.AddControlPoint(pointA, -direction.normalized, direction.normalized, Vector3.up, 0.1f, 0.1f, 1, 1, Color.white, "A");
		blueprint.path.AddControlPoint(pointB, -direction.normalized, direction.normalized, Vector3.up, 0.1f, 0.1f, 1, 1, Color.white, "B");
		blueprint.path.FlushEvents();

        // Generate the particle representation of the rope (wait until it has finished):
        yield return blueprint.Generate();

        // Set the blueprint (this adds particles/constraints to the solver and starts simulating them).
        rope.ropeBlueprint = blueprint;
        rope.GetComponent<MeshRenderer>().enabled = true;

        var pinConstraints = rope.GetConstraintsByType(Oni.ConstraintType.Pin) as ObiConstraints<ObiPinConstraintsBatch>;
        pinConstraints.Clear();

        var batch = new ObiPinConstraintsBatch();
        batch.AddConstraint(rope.solverIndices[0], bodyA, new Vector3(0f,0f,0f), Quaternion.identity, 0, 0f, float.PositiveInfinity);
        batch.AddConstraint(rope.solverIndices[rope.activeParticleCount - 1], bodyB, new Vector3(0f,0f,0f), Quaternion.identity, 0, 0f, float.PositiveInfinity);
        batch.activeConstraintCount =2;
        pinConstraints.AddBatch(batch);
        rope.SetConstraintsDirty(Oni.ConstraintType.Pin);
        isRopeDetatched=false;
    }
    #endregion


    #region Deattaching Rope
    private bool isRopeDetatched=false;

    public void DetatchRope()
    {
        if(!isRopeDetatched)
        {
            rope.ropeBlueprint = null;
            rope.GetComponent<MeshRenderer>().enabled = false;
            isRopeGenerated=false;
            isRopeDetatched=true;
        }
    }
    #endregion

    #region Disable Fish Movent
    private HingeJoint fishHinge;
    public void DisableFishMovement(GameObject g,GameObject arrow)
    {
        if(arrow.GetComponent<Rigidbody>()==null)
        {
            arrow.AddComponent<Rigidbody>();
            arrow.AddComponent<ObiRigidbody>();
        }
        g.GetComponent<FishController>().enabled=false;
        //g.GetComponent<Rigidbody>().isKinematic=true;
        if(g.GetComponent<HingeJoint>()==null)
        {
            fishHinge=g.AddComponent<HingeJoint>();
            fishHinge.connectedBody=arrow.GetComponent<Rigidbody>();
            g.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
        }
    }
    #endregion


    #region Pulling the fish
    void PullTheFish()
    {
        if (rope.isLoaded)
        {
            if(Input.GetKeyDown(KeyCode.Z))
            {

            }
            // if (Input.GetKey(KeyCode.W))
            // {
            //     cursor.ChangeLength(rope.restLength - hookExtendRetractSpeed * Time.deltaTime);
            // }
            // if (Input.GetKey(KeyCode.S))
            // {
            //     cursor.ChangeLength(rope.restLength + hookExtendRetractSpeed * Time.deltaTime);
            // }
        }
    }
    #endregion
}
