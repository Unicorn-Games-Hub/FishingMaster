using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour {

	public List <WeaponId> allWeapons = new List <WeaponId> ();
	public WeaponId equippedWeapon;
	public int equippedWeaponIndex;
	[SerializeField] int startingGunIndex = 0;

	public ArmsManager[] allArms;

	public Transform playerBase;

	public GameObject weaponsContainer;

	bool isSwitching = false;
	int nearWeapon = -1;
	int nextWeapon = -1;

	void Awake ()
	{
		instance = this;
	}

	void Start ()
	{
		foreach (WeaponId gun in allWeapons) 
		{
			gun.gameObject.SetActive (false);
		}

		TakeWeapon (startingGunIndex);
		//		initialPos = playerBase.localPosition;
		//		target = initialPos;
	}

	void OnEnable ()
	{
		PlayerSyncLegacy.onFinishedDisablingAnimation 	+= OnDisableWeaponCallback;
		PlayerSyncLegacy.onFinishedRaisingAnimation 	+= OnRaiseWeaponCallback;
		//
		PlayerSyncMecanim.onFinishedDisablingAnimation 	+= OnDisableWeaponCallback;
		PlayerSyncMecanim.onFinishedRaisingAnimation 	+= OnRaiseWeaponCallback;
	}

	void OnDisable ()
	{
		PlayerSyncLegacy.onFinishedDisablingAnimation 	-= OnDisableWeaponCallback;
		PlayerSyncLegacy.onFinishedRaisingAnimation 	-= OnRaiseWeaponCallback;
		//
		PlayerSyncMecanim.onFinishedDisablingAnimation 	-= OnDisableWeaponCallback;
		PlayerSyncMecanim.onFinishedRaisingAnimation 	-= OnRaiseWeaponCallback;
	}

	void Update ()
	{	
		if (Input.GetKeyDown(KeyCode.E) && CanSwitch)
		{
			isSwitching = true;
			nextWeapon = nearWeapon;
			BroadcastMessage ("DisableWeapon");
		}

		if (Input.GetKeyDown (KeyCode.V)) {
			ChangeSkin ();
		}

		if (Input.GetKeyDown ("1"))
			ChangeArms (0);

		if (Input.GetKeyDown ("2"))
			ChangeArms (1);

		if (Input.GetKeyDown ("3"))
			ChangeArms (2);

		if (Input.GetKeyDown ("4"))
			ChangeArms (3);

		if (Input.GetKeyDown ("5"))
			ChangeArms (4);

		if (Input.GetKeyDown ("6"))
			ChangeArms (5);
	}

	int curArms = 0;

	void ChangeArms (int value)
	{
		if (value == curArms)
			return;

		allArms [curArms].gameObject.SetActive (false);

		allArms [value].gameObject.SetActive (true);
		weaponsContainer.transform.SetParent (allArms [value].gameObject.GetComponent <ArmsManager> ().weaponMountPoint.transform);
		weaponsContainer.transform.localPosition = Vector3.zero;
		weaponsContainer.transform.localRotation = new Quaternion (0, 0, 0, 0);

		curArms = value;

		//		curSkin = 0;

		onUpdateArmsPrefab ();
	}

	//	bool adjustSightOn = false;
	//	Vector3 target;
	//	Vector3 initialPos;

	//	void OnAdjustSightView (float amount)
	//	{
	//		if (amount != 0)
	//			target = new Vector3 (playerBase.localPosition.x, initialPos.y - amount, playerBase.localPosition.z);
	//		else
	//			target = new Vector3 (playerBase.localPosition.x, initialPos.y, playerBase.localPosition.z);	
	//	}

	//	public void UpdateAimedPosition (bool isAimed)
	//	{
	//		if (isAimed)
	//			playerBase.localPosition = Vector3.Slerp (playerBase.localPosition, target, Time.deltaTime * 10f);
	//		else
	//			playerBase.localPosition = initialPos;
	//	}

	void ChangeSkin ()
	{
		allArms[curArms].ChangeSkin ();
	}

	void OnTriggerEnter (Collider hit)
	{
		if (hit.transform.tag == "WeaponAtDisplay") {
			nearWeapon = hit.GetComponent <WeaponAtDisplay> ().weaponID;
		}
	}

	void OnTriggerExit (Collider hit)
	{
		if (hit.transform.tag == "WeaponAtDisplay") {
			nearWeapon = -1;
		}
	}

	void OnDisableWeaponCallback ()
	{
		equippedWeapon.gameObject.SetActive (false);

		int index = allWeapons.FindIndex (x => x.weaponID == nextWeapon);

		nextWeapon = -1;

		if (index >= 0)
			TakeWeapon (index);
		else
			Debug.LogWarning ("Could not find weapon ID on the list!");
	}

	/// <summary>
	/// Called at the end of raise animation. Make sure the animation transition in the Animator Controller windowgoes all the way to the end fo raise animation so it does not miss this.
	/// </summary>
	void OnRaiseWeaponCallback ()
	{
		nextWeapon = -1;
		isSwitching = false;
	}

	void TakeWeapon (int index)
	{
		equippedWeaponIndex = index;
		equippedWeapon = allWeapons [index];
		equippedWeapon.gameObject.SetActive (true);
		//		playerSync.WpState = WeaponState.Raising;
	}

	public static event System.Action onUpdateArmsPrefab;

	static PlayerManager instance = null;
	public static PlayerManager Instance
	{
		get 
		{
			if (instance == null)
				instance = GameObject.FindObjectOfType <PlayerManager> ();
			return instance;
		}
	}

	bool CanSwitch 
	{
		get { return ((!isSwitching && nearWeapon != -1 && nearWeapon != equippedWeapon.weaponID) ? true : false); }
	}

	public bool IsSwitching 
	{ 
		get { return isSwitching; } 
	}

} // class