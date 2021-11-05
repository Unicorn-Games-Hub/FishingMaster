using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewRopeGenerator : MonoBehaviour
{
    public Transform bulletPrefab;
    public Transform spawnPoint;

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Instantiate(bulletPrefab,spawnPoint.position,Quaternion.identity);
        }
    }
}
