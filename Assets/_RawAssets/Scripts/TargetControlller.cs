using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TargetControlller : MonoBehaviour
{
    public Color GreenClr;
    private Color NativeColor;

    private void Start()
    {
        NativeColor = GetComponent<Image>().color;
    }

    void Update()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(transform.position);
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider != null)
            {
                if (hit.collider.gameObject.tag == "Generic")
                {
                    GetComponent<Image>().color = GreenClr;
                }
            }

            SetWhite(hit);
        }
    }

    void SetWhite(RaycastHit hit)
    {
        if(hit.collider.tag != "Generic")
        {
            GetComponent<Image>().color = NativeColor;
        }
    }
}
