using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeController : MonoBehaviour
{
    public GameObject Target;

    public Vector3 Offset;

    private bool showHealthBar=true;

    void Start()
    {
        TotalHits = Target.GetComponent<FishController>().HitInt;
        transform.localPosition=new Vector3(0f,1.18f,0f);
        if(GameManager.instance.shootMechanics==GameManager.ShootingType.Rope)
        {
            showHealthBar=false;
            transform.GetChild(0).gameObject.SetActive(false);
        }
    }
    private int TotalHits = 0;

    /*
    private void Update()
    {
        if (Target == null)
        {
            Destroy(this.gameObject);
        }
    }

    void FixedUpdate()
    {
        if (Target != null)
        {
            transform.position = Target.transform.position + Offset;
            transform.LookAt((Camera.main.gameObject.transform));

            float ScaleFloat;

            ScaleFloat = (float)Target.GetComponent<FishController>().HitInt / (float)TotalHits;

            transform.GetChild(0).transform.GetChild(0).transform.localScale =
                new Vector3(transform.GetChild(0).transform.GetChild(0).transform.localScale.x, transform.GetChild(0).transform.GetChild(0).transform.localScale.y,
                                            Mathf.MoveTowards(transform.GetChild(0).transform.GetChild(0).transform.localScale.z, ScaleFloat, 0.05f));
        }
    }
    */

    void FixedUpdate()
    {
        if(showHealthBar)
        {
            if (Target != null)
            {
                //transform.position = transform.localPosition + Offset;
                transform.LookAt((Camera.main.gameObject.transform));

                float ScaleFloat;

                ScaleFloat = (float)Target.GetComponent<FishController>().HitInt / (float)TotalHits;

                transform.GetChild(0).transform.GetChild(0).transform.localScale =
                    new Vector3(transform.GetChild(0).transform.GetChild(0).transform.localScale.x, transform.GetChild(0).transform.GetChild(0).transform.localScale.y,
                                                Mathf.MoveTowards(transform.GetChild(0).transform.GetChild(0).transform.localScale.z, ScaleFloat, 0.05f));
            } 
        }
    }
}
