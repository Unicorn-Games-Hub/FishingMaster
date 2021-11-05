using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayManager : MonoBehaviour
{
    public enum fishType
    {
        SchoolOfFish,
        NormalFish,
        Sharks
    }

    public fishType gameFish;

    public Transform schoolFishAssets;
    public Transform normalFishAssets;
    public Transform sharkAssets;


    void Start()
    {
        if(gameFish==fishType.SchoolOfFish)
        {
           schoolFishAssets.gameObject.SetActive(true);
           normalFishAssets.gameObject.SetActive(false);
           sharkAssets.gameObject.SetActive(false);
        }
        else if(gameFish==fishType.NormalFish)
        {
           schoolFishAssets.gameObject.SetActive(false);
           normalFishAssets.gameObject.SetActive(true);
           sharkAssets.gameObject.SetActive(false);
        }
        else if(gameFish==fishType.Sharks)
        {
            schoolFishAssets.gameObject.SetActive(false);
            normalFishAssets.gameObject.SetActive(false);
            sharkAssets.gameObject.SetActive(true);
        }
    }
}
