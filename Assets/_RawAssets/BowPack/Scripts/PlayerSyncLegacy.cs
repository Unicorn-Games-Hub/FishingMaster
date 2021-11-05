using System.Collections;

using System.Collections.Generic;

using UnityEngine;

using System;



public enum WeaponState 

{ 

	AimedIdle,

	AimedWalking,

	AimedFiring,

	PrepareAiming,	// state for more complex bow transition where archer pulls the string and take aim

	Idle,

	IdleUnloaded,	// for crossbows, after shooting	

	IdleLow,		// for spear

	Walking,

	Running, 

	Firing, 

	Reloading,

	Lowering,

	Raising,

	Swinging,		// for maces, clubs

	Spinning,		// for flail

	Attacking,

	Blocking,		// for all bludgeoning weapons

	AttackMiss		// for flail

}



public class PlayerSyncLegacy : MonoBehaviour {



	Animation myAnim = null;

	WeaponsBaseLegacy curWeapon = null;

	bool isSwitching = false;



	void Awake ()

	{

		instance = this;

		myAnim = GetComponent <Animation> ();

		UpdateAnimation ();

	}



	void Start ()

	{

		UpdateAnimation ();

	}



	void OnEnable ()

	{

		PlayerManager.onUpdateArmsPrefab += UpdateAnimation;

	}



	void OnDisable ()

	{

		PlayerManager.onUpdateArmsPrefab -= UpdateAnimation;

	}



	public void UpdateCurrentWeapon (WeaponsBaseLegacy _curWeapon)

	{

		this.curWeapon = _curWeapon;

	}



	public void UpdateCurrentAnimation (Animation curAnim)

	{

		this.myAnim = curAnim;

	}



	void UpdateAnimation ()

	{

		if (!curWeapon || !myAnim)

			return;

		

		switch (wpState) 

		{

			case WeaponState.Idle:

				myAnim.CrossFade (curWeapon.weaponAnimations.Arms_Idle.name);

				break;

			case WeaponState.Walking:

				if (curWeapon.weaponAnimations.Arms_Walk)

					myAnim.CrossFade (curWeapon.weaponAnimations.Arms_Walk.name);

				break;

			case WeaponState.Running:

				if (curWeapon.weaponAnimations.Arms_Sprint)
                    myAnim.CrossFade(curWeapon.weaponAnimations.Arms_Idle.name);
                //myAnim.CrossFade (curWeapon.weaponAnimations.Arms_Sprint.name);

                break;

			case WeaponState.Firing:

				myAnim.Play (curWeapon.weaponAnimations.Arms_Fire.name);

				break;

			case WeaponState.Reloading:

				if (curWeapon.weaponAnimations.Arms_Reload) {

					myAnim.Play (curWeapon.weaponAnimations.Arms_Reload.name);					

					curWeapon.StartCoroutine (curWeapon.ReloadNormal ());

				}

				break;

			case WeaponState.Lowering:

				StartCoroutine (PlayDisableClip (curWeapon.weaponAnimations.Arms_Lower));

				break;

			case WeaponState.Raising:

				StartCoroutine (PlayRaiseClip (curWeapon.weaponAnimations.Arms_Raise));

				break;

			case WeaponState.PrepareAiming:	

				myAnim.Play (curWeapon.weaponAnimations.Arms_IdleToADS.name);				

				break;

			case WeaponState.AimedIdle:

				if (curWeapon.weaponAnimations.ADS_Idle)

					myAnim.CrossFade (curWeapon.weaponAnimations.ADS_Idle.name, 0.2f);

				break;

			case WeaponState.AimedWalking:

				if (curWeapon.weaponAnimations.ADS_Walk)

					myAnim.CrossFade (curWeapon.weaponAnimations.ADS_Walk.name, 0.2f);

				break;

			case WeaponState.AimedFiring:

				if (curWeapon.weaponAnimations.ADS_Fire)

					myAnim.Play (curWeapon.weaponAnimations.ADS_Fire.name);

				break;

			case WeaponState.Attacking:

				myAnim.Play (curWeapon.weaponAnimations.Arms_Attack.name);

				break;

			case WeaponState.AttackMiss:

				myAnim.Play (curWeapon.weaponAnimations.Arms_Miss.name);

				break;

			case WeaponState.Swinging:

				myAnim.Play (curWeapon.weaponAnimations.Arms_Swing.name);

				break;

			case WeaponState.Spinning:

				myAnim.CrossFade (curWeapon.weaponAnimations.Arms_Spin.name);

				break;

			case WeaponState.Blocking:

				StartCoroutine (Block ());

				break;

			case WeaponState.IdleLow:

				myAnim.CrossFade (curWeapon.weaponAnimations.Arms_IdleLow.name);

				break;

			default :

				break;

		}

	}



	IEnumerator Block ()

	{

		while (wpState == WeaponState.Blocking) 

		{

			if (curWeapon.blockHit) 

			{

				myAnim.Play (curWeapon.weaponAnimations.Arms_BlockHit.name);

//				myAnim.CrossFade (curWeapon.weaponAnimations.Arms_BlockHit.name);

				yield return StartCoroutine (WaitAnimationFinish (curWeapon.weaponAnimations.Arms_BlockHit));

				curWeapon.blockHit = false;

			} 

			else

			{

				myAnim.CrossFade (curWeapon.weaponAnimations.Arms_BlockIdle.name);

			}

			yield return null;

		}

		yield return null;

	}



	IEnumerator PlayDisableClip (AnimationClip clip)

	{

		if (isSwitching) yield break;

		isSwitching = true;

		myAnim.Play (clip.name);

		yield return StartCoroutine (WaitAnimationFinish (clip));

		isSwitching = false;

		if (onFinishedDisablingAnimation != null)

			onFinishedDisablingAnimation ();

	}



	IEnumerator PlayRaiseClip (AnimationClip clip)

	{

		myAnim.Play (clip.name);



		if (curWeapon.weaponAnimations.Weapon_Raise) 

		{

			if (onStartRaisingAnimation != null)

			{

				onStartRaisingAnimation (curWeapon.weaponAnimations.Weapon_Raise.name);

			}

		}

		yield return StartCoroutine (WaitAnimationFinish (clip));

		if (onFinishedRaisingAnimation != null)

			onFinishedRaisingAnimation ();

	}

		

	IEnumerator WaitAnimationFinish (AnimationClip clip)

	{

		float reloadTime = clip.length;

		yield return new WaitForSeconds (reloadTime);

	}



	public static event Action onFinishedDisablingAnimation;

	public static event Action onFinishedRaisingAnimation;

	public static event Action <string> onStartRaisingAnimation;



	private static PlayerSyncLegacy instance = null;

	public static PlayerSyncLegacy Instance

	{

		get 

		{

			if (instance == null)

				instance = GameObject.FindObjectOfType <PlayerSyncLegacy> ();

			return instance;

		}

	}



	private WeaponState wpState;

	public WeaponState WpState 

	{ 

		get 

		{ 

			return wpState; 

		} 

		set 

		{ 

			wpState = value; 

			UpdateAnimation ();

		} 

	}



	PlayerManager pManager 

	{ 

		get 

		{ 

			return PlayerManager.Instance;

		} 

	}

		

}

