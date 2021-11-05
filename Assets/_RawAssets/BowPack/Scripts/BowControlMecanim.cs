using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

public class BowControlMecanim : WeaponsBaseMecanim {

	#region Fields

	public GameObject arrow = null;
	public Transform mountPoint = null;    
	public GameObject attachedArrow = null;

	// basic stats
	public int range = 300; 
	public float damage = 20.0f;
	public float maxPenetration = 3.0f;
	public float fireRate = 0.5f; 
	public int impactForce = 50;   
	public float arrowSpeed = 60.0f;  

	[HideInInspector] public float baseSpread = 1.0f;  
	float maxSpread = 4.0f; 
	public float defaultSpread = 1f;
	public float defaultMaxSpread = 4f;

	protected bool canAim;
	public bool useSmooth = true;
	[Range (1f, 10f)]
	public float aimSmooth;
	[Range(0, 179)]
	public float aimFov = 35;
	float defaultFov;
	float currentFov;

	Quaternion camPosition;
	Quaternion defaultCamRot;

//	bool firePressed = false;
	bool isLoaded = false;

	#endregion

	void Start ()
	{
		defaultCamRot = Camera.main.transform.localRotation;
		defaultFov = Camera.main.fieldOfView;	
	}

	protected override void OnEnable ()
	{
		base.OnEnable ();
		canAim = true;
		wpType = WeaponType.Bow;
	}

	protected override void OnDisable ()
	{
		base.OnDisable ();
		canAim = false;
	}

    protected override void InputUpdate()
    {
        if (GameManager.Fish == null)
        {
            if (Input.GetMouseButton(1) && CanAim)
            {
                if (!isAimed)
                    isAimed = true;

                if (!isFiring)
                    EmptySlot(false);

                if (Input.GetMouseButtonDown(0) && CanFire && isLoaded)
                {
                    isFiring = true;
                    Fire();
                }
            }
            else
            {
                isAimed = false;
                isLoaded = false;
            }
        }
    }

	/// <summary>
	/// Called by Player Sync, in the end of the IdleToADS animation. 
	/// </summary>
	public void OnFinishArrowLoad ()
	{
		isLoaded = true;
	}

	/// <summary>
	/// Called by Player Sync, in the end of the Fire animation. 
	/// </summary>
	public void OnArrowShot ()
	{
		isFiring = false;
	}

	protected override void UpdateAnimatorParameters ()
	{			
		foreach (Animator a in myAnims)
		{
			a.SetBool ("firePressed", isFiring);
			a.SetBool ("isAimed", isAimed);
		}

		if (Input.GetKeyDown (KeyCode.R) && playerSync.Anim.GetCurrentAnimatorStateInfo (0).IsName ("Idle"))
		{
			foreach (Animator a in myAnims)
			{
				a.SetTrigger ("reload");
			}
		}
	}

	void Fire()
	{
		StartCoroutine (FireOneShot());
		SetFireTrigger ();
		EmptySlot (true);
		isLoaded = false;
		Source.clip = FireSound;
		Source.spread = Random.Range (1.0f, 1.5f);
		Source.pitch  = Random.Range (1.0f, 1.05f);
		Source.Play();
//		PlayWeaponAnimation (weaponAnimations.Weapon_ADS_Fire.name, WrapMode.Once);
//		yield return StartCoroutine (WaitAnimationFinish (weaponAnimations.Weapon_ADS_Fire));
//		firePressed = false;
		//		yield return null;
	}

	void SetFireTrigger ()
	{
		//			SendMessageUpwards ("FireTriggered");
		playerSync.FireTriggered ();

		foreach (Animator a in myAnims)
		{
			a.SetTrigger ("fireTrigger");
		}
	}

	IEnumerator FireOneShot()
	{
		Vector3 position = mountPoint.position;

		// set the gun's info into an array to send to the bullet
		MissileInfo info = new MissileInfo();
		info.damage = damage;
		info.impactForce = impactForce;
		info.maxPenetration = maxPenetration;
		info.maxspread = maxSpread;
		info.speed = arrowSpeed;
		info.position = this.transform.root.position;
		info.lifeTime = range;

		Quaternion q = Quaternion.Euler (new Vector3 (0, transform.eulerAngles.y - 90f, -transform.eulerAngles.x));

		GameObject newArrow = Instantiate (arrow, position, q) as GameObject;
		newArrow.GetComponent<Missile>().SetUp(info);
		//		newArrow.transform.RotateAround (newArrow.transform.position, newArrow.transform.right, 90f);
		newArrow.transform.RotateAround (newArrow.transform.position, newArrow.transform.forward, 90f);
		Source.clip = FireSound;
		Source.spread = Random.Range (1.0f, 1.5f);
		Source.Play();

		yield return null;
	}

	void EmptySlot (bool value)
	{
		if (value) 
			attachedArrow.SetActive (false);
		else
			attachedArrow.SetActive (true);
	}

	protected override void Aim ()
	{
		if (isAimed)
		{
			currentFov = aimFov;
			baseSpread = defaultSpread / 2f; 
			maxSpread = defaultMaxSpread / 2f;
//			SendMessageUpwards ("UpdateAimedPosition", true);
//			BroadcastMessage ("UpdateSightStatus", true, SendMessageOptions.DontRequireReceiver);
		}
		else
		{      
			currentFov = defaultFov;
			baseSpread = defaultSpread;
			maxSpread = defaultMaxSpread;
//			SendMessageUpwards ("UpdateAimedPosition", false);
//			BroadcastMessage ("UpdateSightStatus", false, SendMessageOptions.DontRequireReceiver);
		}
		Camera.main.fieldOfView = useSmooth ? 
			Mathf.Lerp(Camera.main.fieldOfView, currentFov, Time.deltaTime * (aimSmooth * 3)) : //apply fog distance
			Mathf.Lerp(Camera.main.fieldOfView, currentFov, Time.deltaTime * aimSmooth);
	}

	public bool CanAim
	{
		get
		{
			
				return false;
		}
	}
}
