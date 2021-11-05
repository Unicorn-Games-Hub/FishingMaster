using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAtDisplay : MonoBehaviour {

	public int weaponID;

	public int rotationSpeed;

	public MeshRenderer label;
	public Material redFontMaterial;
	public Material blackFontMaterial;

	void FixedUpdate ()
	{
		transform.Rotate (Vector3.up * Time.fixedDeltaTime * rotationSpeed);
	}

	void OnTriggerEnter ()
	{
		label.material = redFontMaterial;
	}

	void OnTriggerExit ()
	{
		label.material = blackFontMaterial;
	}
}
