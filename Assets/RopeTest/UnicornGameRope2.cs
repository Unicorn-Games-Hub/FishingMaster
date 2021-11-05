using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Obi;

public class UnicornGameRope2 : MonoBehaviour
{
    //obi rope solver
    public ObiSolver solver; 

    public ObiCollider character;
    public float hookExtendRetractSpeed = 2;
    public Material material;
    public ObiRopeSection section;

    [Range(0.001f,1f)]
    public float ropeRadius=0.05f;
    private ObiRope rope;
    private ObiRopeBlueprint blueprint;
    private ObiRopeExtrudedRenderer ropeRenderer;

    private ObiRopeCursor cursor;

    private RaycastHit hookAttachment;

    public Transform initialAttachPoint;
    public Transform finalAttachPoint;
    public ObiCollider target;


    void Awake()
    {
        // Create both the rope and the solver:	
        rope = gameObject.AddComponent<ObiRope>();
        ropeRenderer = gameObject.AddComponent<ObiRopeExtrudedRenderer>();
        ropeRenderer.section = section;
        ropeRenderer.uvScale = new Vector2(1, 50);
        ropeRenderer.normalizeV = false;
        ropeRenderer.uvAnchor = 1;
        rope.GetComponent<MeshRenderer>().material = material;

        // Setup a blueprint for the rope:
        blueprint = ScriptableObject.CreateInstance<ObiRopeBlueprint>();
        blueprint.resolution = 0.5f;
        blueprint.thickness = ropeRadius;

        // Tweak rope parameters:
        rope.maxBending = 0.02f;

        // Add a cursor to be able to change rope length:
        // cursor = rope.gameObject.AddComponent<ObiRopeCursor>();
        // cursor.cursorMu = 0;
        // cursor.direction = true;
    }

    private void OnDestroy()
    {
        //DestroyImmediate(blueprint);
    }

    private void LaunchHook()
    {
        StartCoroutine(AttachHook(initialAttachPoint.position,finalAttachPoint.position));
    }

    private IEnumerator AttachHook(Vector3 pointA,Vector3 pointB)
    {
        yield return 0;
       
        //convert both points to the rope's local space:
        // pointA = rope.transform.InverseTransformPoint(pointA);
		// pointB = rope.transform.InverseTransformPoint(pointB);

        //Procedurally generate the rope path (a simple straight line):
        blueprint.path.Clear();
        Vector3 direction = (pointB - pointA) ;

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
        batch.AddConstraint(rope.solverIndices[0], character,transform.localPosition, Quaternion.identity, 0, 0, float.PositiveInfinity);
        batch.AddConstraint(rope.solverIndices[rope.activeParticleCount - 1], target, finalAttachPoint.position, Quaternion.identity, 0, 0, float.PositiveInfinity);
      
      
        batch.activeConstraintCount = 2;
        pinConstraints.AddBatch(batch);
        //rope.SetConstraintsDirty(Oni.ConstraintType.Pin);
    }

    private void DetachHook()
    {
        // Set the rope blueprint to null (automatically removes the previous blueprint from the solver, if any).
        rope.ropeBlueprint = null;
        rope.GetComponent<MeshRenderer>().enabled = false;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!rope.isLoaded)
                LaunchHook();
            else
                DetachHook();
        }

        // if (rope.isLoaded)
        // {
        //     if (Input.GetKey(KeyCode.W))
        //     {
        //         cursor.ChangeLength(rope.restLength - hookExtendRetractSpeed * Time.deltaTime);
        //     }
        //     if (Input.GetKey(KeyCode.S))
        //     {
        //         cursor.ChangeLength(rope.restLength + hookExtendRetractSpeed * Time.deltaTime);
        //     }
        // }
    }
}
