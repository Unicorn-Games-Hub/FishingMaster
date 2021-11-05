using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmsManager : MonoBehaviour {
	public GameObject weaponMountPoint;
	public Material[] allSkins;

	void Start ()
	{
		if (PlayerSyncLegacy.Instance)
			PlayerSyncLegacy.Instance.UpdateCurrentAnimation (this.gameObject.GetComponent <Animation> ());
	}

	void OnEnable ()
	{
		if (PlayerSyncLegacy.Instance)
			PlayerSyncLegacy.Instance.UpdateCurrentAnimation (this.gameObject.GetComponent <Animation> ());
	}

	int curSkin = 0;

	public void ChangeSkin ()
	{
		curSkin++;
		if (curSkin >= allSkins.Length)
			curSkin = 0;		

		GetComponentInChildren <SkinnedMeshRenderer>().material = allSkins[curSkin];
	}
}

