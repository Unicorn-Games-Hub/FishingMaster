using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public enum WeaponType {
	Bow,
	Crossbow,
	Bludgeoning,
	Spear
} 

public abstract class WeaponsBaseMecanim : MonoBehaviour {

	#region Fields

	protected WeaponType wpType;
	public WeaponType WpType { get { return wpType; } }

	public RuntimeAnimatorController properArmsAnimation;

	public AudioClip TakeSound;
	public AudioClip FireSound;
		
	protected AudioSource Source;
	protected Animator[] myAnims = null;

	bool disableWeapon = false;
	protected bool isReloading;
	
	float nextFireTime = 0.0f; 

	#endregion
	
	protected virtual void Awake()
	{
		Source = GetComponent<AudioSource>();
		disableWeapon = false;
	
		if (GetComponentInChildren<Animator>())	{
			myAnims = GetComponentsInChildren <Animator> ();
		}
	}

	protected virtual void OnEnable()
	{
		Source.clip = TakeSound;
		Source.Play();
		canFire = true;
		disableWeapon = false;
		isReloading = false;
		playerSync.UpdateCurrentGun (GetComponent <WeaponsBaseMecanim> ());
	}

	protected virtual void OnDisable ()
	{
		disableWeapon = false;
	}

	protected virtual void Update()
	{
		InputUpdate();
		Aim();
		UpdateAnimatorParameters ();
	}

	protected abstract void InputUpdate ();

//	protected abstract void SyncState ();

	protected virtual void UpdateAnimatorParameters () {}

	protected virtual void Aim() {}

	protected IEnumerator WaitAnimationFinish (AnimationClip clip)
	{
		float reloadTime = clip.length;
		yield return new WaitForSeconds (reloadTime);
	}
		
	public virtual void DisableWeapon()
	{
//		canAim = false;
		canFire = false;
		disableWeapon = true;
		StopAllCoroutines();
		playerSync.TriggerLowerGunAnimation ();
		//		SendMessageUpwards ("TriggerLowerGunAnimation");
	}

	protected bool isFiring = false;  
	public bool IsFiring { get { return isFiring; } }

	protected bool firePressed = false;  
	public bool FirePressed { get { return firePressed; } }

	protected bool isAimed;
	public bool IsAimed { get { return isAimed; } } 

	protected bool canFire = true;
	public bool CanFire { get {	return canFire; } }

	protected bool isStriking = false;  
	public virtual bool IsStriking { get { return isStriking; } }

	protected bool isStrikingMiss = false;  
	public virtual bool IsStrikingMiss { get { return isStrikingMiss; } }

	protected bool isSwingingRight = false;  
	public bool IsSwingingRight { get { return isSwingingRight; } } 

	protected bool isSwingingLeft = false;  
	public bool IsSwingingLeft { get { return isSwingingLeft; } }

	protected bool isSpinning = false;  
	public bool IsSpinning { get { return isSpinning; } }

	protected bool isBlocking = false;  
	public virtual bool IsBlocking { get { return isBlocking; } }

	protected bool blockHit = false;
	public bool BlockHit { get { return blockHit; } }

	protected bool isThrowing = false;  
	public virtual bool IsThrowing { get { return isThrowing; } }

	protected bool isSpearLow = false;  
	public virtual bool IsSpearLow { get { return isSpearLow; } }

	protected FirstPersonController controller
	{
		get
		{
			return transform.root.GetComponent<FirstPersonController>();
		}
	}
	
	protected PlayerSyncMecanim playerSync
	{
		get
		{
			return PlayerSyncMecanim.Instance;
		}
	}
	
	protected PlayerManager playerManager 
	{ 
		get 
		{ 
			return PlayerManager.Instance;
		} 
	}
		
}	// class