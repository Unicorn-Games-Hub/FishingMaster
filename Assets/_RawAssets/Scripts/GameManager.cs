using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public int FishesToHunt = 5;
    private int fishInt = 0;
    string s;
    public Text FishText;

    public GameObject player;


    public enum startCam
    {
        underWater,
        overWater
    }

    public startCam tutorialCam;
 

    public enum ShootingType
    {
        Normal,
        Rope
    }

    public ShootingType shootMechanics;

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
        FishText.text = FishDisplay();
        player.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(false);
        GameObject.Find("MiddleTarget").GetComponent<Image>().enabled = false;
        
    }

    private string FishDisplay()
    {       
        s = "Fish ( " + fishInt + " / " + FishesToHunt + " ) ";

        return s;
    }

    public static GameObject Fish;

    public void HuntFish(GameObject HuntedFish)
    {
        FirstPersonController.canRotate = false;
        Fish = HuntedFish;
        fishInt++;
        FishText.text = FishDisplay();
        player.GetComponent<FirstPersonController>().enabled = false;
        //player.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(false);
        // Invoke("HuntAgain", 2.5f);
        //Invoke("HuntAgain", 10f);
        //GameObject.Find("MiddleTarget").GetComponent<Image>().enabled = false;
        //GameObject.Find("LongBow_LOD1").GetComponent<Renderer>().enabled = false;
       // GameObject.Find("FPArms_Male").GetComponent<Renderer>().enabled = false;        
    }

    public void ResetThinsForNextHunt()
    {
        Invoke("HuntAgain", 2f);
    }

    private void HuntAgain()
    {
        // GameObject.Find("MiddleTarget").GetComponent<Image>().enabled = true;
        // GameObject.Find("LongBow_LOD1").GetComponent<Renderer>().enabled = true;
        // GameObject.Find("FPArms_Male").GetComponent<Renderer>().enabled = true;
        FirstPersonController.canRotate = true;
        Fish = null;
        player.GetComponent<FirstPersonController>().enabled = true;
        //player.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(true);
    }

    private bool IsTutorial = true;

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.T))
        {
            if (IsTutorial == true)
            {
                IsTutorial = false;
            }
        }
    }

    private void FixedUpdate()
    {
        if (IsTutorial == false)
        {
            if (player.transform.GetChild(0).transform.localPosition != new Vector3(0, 0, 0))
            {
                player.transform.GetChild(0).transform.localPosition =
                    Vector3.MoveTowards(player.transform.GetChild(0).transform.localPosition,
                                                                                            new Vector3(0, 0f, 0), 0.5f);
            }
            else
            {
                player.transform.GetChild(0).GetChild(0).localPosition=new Vector3(0f,1f,0f);
                if (player.GetComponent<FirstPersonController>().enabled == false)
                {
                    if (aBool == false)
                    {
                        player.GetComponent<FirstPersonController>().enabled = true;
                        player.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(true);
                        GameObject.Find("MiddleTarget").GetComponent<Image>().enabled = true;
                        aBool = true;
                    }
                }
            }

            if (Fish != null)
            {
                Camera.main.transform.LookAt(Fish.transform);
                Camera.main.GetComponent<Camera>().fieldOfView =
                    Mathf.MoveTowards(Camera.main.GetComponent<Camera>().fieldOfView, 60, 1.5f);
            }
        }
        else
        {
            if(tutorialCam==startCam.underWater)
            {
                player.transform.GetChild(0).transform.Rotate(new Vector3(0, 1, 0) * 0.1f);
            }
            else
            {
                player.transform.GetChild(0).transform.Rotate(new Vector3(0, 1, 0) * 0.1f);
            }
            
        }
    }

    
    private bool aBool = false;
}
