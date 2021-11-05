using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Obi;

public class PullableRope : MonoBehaviour
{
    private ObiRope rope;
    [Range(0.01f,1f)]
    public float ropeRadius;
    [Header("Rope Material")]
    public Material ropeMaterial;

    public ObiRopeBlueprint blueprint;
    private ObiRopeExtrudedRenderer ropeRenderer;

    //
    public ObiRopeSection section;



    //
    private ObiRopeCursor cursor;

    [Header("Rope Pull Speed")]
    public float hookExtendRetractSpeed=2f;

   
    void Start()
    {
        rope=GetComponent<ObiRope>();

        //
        ropeRenderer=GetComponent<ObiRopeExtrudedRenderer>();
        ropeRenderer.section = section;
        ropeRenderer.uvScale = new Vector2(1, 5);
        ropeRenderer.normalizeV = false;
        ropeRenderer.uvAnchor = 1;
        //
        rope.GetComponent<MeshRenderer>().material = ropeMaterial;
        
        //Tweak rope parameters:
        rope.maxBending = 0.02f;

        //bluePrints
        blueprint.resolution = 0.5f;
        blueprint.thickness = ropeRadius;

        //cursor for changing rope length
        cursor = rope.gameObject.AddComponent<ObiRopeCursor>();
        cursor.cursorMu = 0;
        cursor.direction = true;
    }

    void Update()
    {
        if(rope.isLoaded)
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

    #region Attatching Rope

    #endregion
}
