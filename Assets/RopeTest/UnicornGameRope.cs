using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Obi;

public class UnicornGameRope : MonoBehaviour
{
    public static UnicornGameRope instance;

    public Material ropeMaterial;
    public Transform initialAnchorPoint;
    private GameObject endAnchorPoint;

    private bool ropeGenerated=false;

    //
    private ObiSolver solver; 
    private ObiRope rope;

    [Range(0.001f,1f)]
    public float ropeRadius;

    public Vector2 ropeMatTiling=new Vector2(1,20);

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

    void CreateRope(ObiSolver solver)
    {
        rope = CreateRope(initialAnchorPoint.position,endAnchorPoint.transform.position);
        initialAnchorPoint.gameObject.AddComponent<ObiCollider>();
        rope.transform.parent = solver.transform;
        PinRope(rope,initialAnchorPoint.gameObject.GetComponent<ObiCollider>(), endAnchorPoint.GetComponent<ObiCollider>(), new Vector3(0, 0.5f, 0), -new Vector3(0, 0.5f, 0));
    }


    private void PinRope(ObiRope rope, ObiCollider bodyA, ObiCollider bodyB, Vector3 offsetA, Vector3 offsetB)
    {
        // Pin both ends of the rope (this enables two-way interaction between character and rope):
        var pinConstraints = rope.GetConstraintsByType(Oni.ConstraintType.Pin) as ObiConstraints<ObiPinConstraintsBatch>;
        pinConstraints.Clear();
        var batch = new ObiPinConstraintsBatch();
        batch.AddConstraint(rope.solverIndices[0], bodyA, offsetA, Quaternion.identity, 0, 999, float.PositiveInfinity);
        batch.AddConstraint(rope.solverIndices[rope.activeParticleCount - 1], bodyB, offsetB, Quaternion.identity, 0, 999, float.PositiveInfinity);
        batch.activeConstraintCount =2;
        pinConstraints.AddBatch(batch);
    }

    // Creates a rope between two points in world space:
	private ObiRope CreateRope(Vector3 pointA, Vector3 pointB)
	{
		// Create a rope
        var ropeObject = new GameObject("solver", typeof(ObiRope), typeof(ObiRopeLineRenderer));
        var rope = ropeObject.GetComponent<ObiRope>();
		var ropeRenderer = ropeObject.GetComponent<ObiRopeLineRenderer>();
		rope.GetComponent<MeshRenderer>().material = ropeMaterial;
        rope.GetComponent<ObiPathSmoother>().decimation = 0.1f;
        ropeRenderer.uvScale = ropeMatTiling; //new Vector2(1, 5);

		// Setup a blueprint for the rope:
		var blueprint = ScriptableObject.CreateInstance<ObiRopeBlueprint>();
		blueprint.resolution = 0.15f;
        // blueprint.thickness = 0.05f;
        blueprint.thickness = ropeRadius;
        blueprint.pooledParticles = 0;

        //convert both points to the rope's local space:
        pointA = rope.transform.InverseTransformPoint(pointA);
		pointB = rope.transform.InverseTransformPoint(pointB);

		//Procedurally generate the rope path (a simple straight line):
		Vector3 direction = (pointB - pointA) * 0.25f;
		blueprint.path.Clear();
		blueprint.path.AddControlPoint(pointA, -direction, direction, Vector3.up, 0.1f, 0.1f, 1, 1, Color.white, "A");
		blueprint.path.AddControlPoint(pointB, -direction, direction, Vector3.up, 0.1f, 0.1f, 1, 1, Color.white, "B");
		blueprint.path.FlushEvents();

		rope.ropeBlueprint = blueprint;
		return rope;
	}

    

    public void UpdateRopeAnchor(GameObject newRopeAnchor)
    {
        if(!ropeGenerated)
        {
            newRopeAnchor.AddComponent<ObiCollider>();
            endAnchorPoint=newRopeAnchor;
            CreateRope(solver);
            ropeGenerated=true;
        }
        else
        {
            //anchor rope to new one
        }
    }

    public Transform arrowPrefab;

    void Update()
    {
        if(Input.GetMouseButtonDown(0)&&!ropeGenerated)
        {
            Transform newArrow=Instantiate(arrowPrefab,transform.position,Quaternion.identity);
            newArrow.gameObject.AddComponent<ObiCollider>();
            endAnchorPoint=newArrow.gameObject;
            CreateRope(solver);
            ropeGenerated=true;
        }
    }
}
