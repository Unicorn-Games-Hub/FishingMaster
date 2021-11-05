using System.Collections;

using System.Collections.Generic;

using UnityEngine;

using System;

using UnityStandardAssets.Characters.FirstPerson;



public class PlayerSyncMecanim : MonoBehaviour {



	Rigidbody rigibody;

	WeaponsBaseMecanim curWeapon = null;



	Animator curAnim;

	public Animator Anim { get { return curAnim; } }



	void Awake ()

	{

		instance = this;

		curAnim = GetComponent <Animator> ();

		rigibody = GetComponentInParent <Rigidbody> ();

	}



	void Update () 

	{

		UpdateAnimatorParameters ();

	}



	void UpdateAnimatorParameters ()

	{

	/*	curAnim.SetBool ("isWalking", controller.IsWalking);

		curAnim.SetBool ("isRunning", controller.IsRunning);

		curAnim.SetBool ("isQuiet", controller.IsQuiet);

		curAnim.SetBool ("isJumping", controller.IsJumping);

    */

		if (curWeapon.WpType == WeaponType.Bludgeoning)

		{

			curAnim.SetBool ("isBlocking", curWeapon.IsBlocking);

			curAnim.SetBool ("blockHit", curWeapon.BlockHit);

			curAnim.SetBool ("isSpinning", curWeapon.IsSpinning);

		}

		else if (curWeapon.WpType == WeaponType.Spear)

		{

			curAnim.SetBool ("spearLow", curWeapon.IsSpearLow);

		}

		else

		{

			curAnim.SetBool ("firePressed", curWeapon.FirePressed);

			curAnim.SetBool ("isAimed", curWeapon.IsAimed);

		}



//		if (Input.GetKeyDown (KeyCode.R) && curAnim.GetCurrentAnimatorStateInfo (0).IsName ("Idle"))

//			curAnim.SetTrigger ("reload");



		if (isLoweringGun)

		{

			if (curAnim.GetCurrentAnimatorStateInfo (0).IsName ("Lower") &&

				(curAnim.GetCurrentAnimatorStateInfo (0).normalizedTime >= 0.9f))

			{

				// disable callback

				if (onFinishedDisablingAnimation != null)

					onFinishedDisablingAnimation ();

				isLoweringGun = false;

			}

		}



		if (isRaisingGun)

		{

			if (curAnim.GetCurrentAnimatorStateInfo (0).IsName ("Raise") &&

				(curAnim.GetCurrentAnimatorStateInfo (0).normalizedTime >= 0.9f))

			{

				// raise callback

				if (onFinishedRaisingAnimation != null)

					onFinishedRaisingAnimation ();

				isRaisingGun = false;

			}

		}

	}



	bool isLoweringGun;



	public void TriggerLowerGunAnimation ()

	{		

		curAnim.SetTrigger ("disable");

		isLoweringGun = true;

	}



	bool isRaisingGun;



	public void UpdateCurrentGun (WeaponsBaseMecanim _curWeapon)

	{

		if (!curAnim)

			return;

		

		this.curWeapon = _curWeapon;

		curAnim.runtimeAnimatorController = _curWeapon.properArmsAnimation as RuntimeAnimatorController;

		isRaisingGun = true;

	}



	public void SetReloadTrigger ()

	{

		curAnim.SetTrigger ("reload");

	}



	public void FireTriggered ()

	{

		curAnim.SetTrigger ("fireTrigger");

	}

		

	public void StrikeTriggered ()

	{

		curAnim.SetTrigger ("strikeTrigger");

	}



	public void StrikeMissTriggered ()

	{

		curAnim.SetTrigger ("strikeMissTrigger");

	}

		

	public void SwingLeftTriggered ()

	{

		curAnim.SetTrigger ("swingLeftTrigger");

	}



	public void SwingRightTriggered ()

	{

		curAnim.SetTrigger ("swingRightTrigger");

	}



	public void ThrowTriggered ()

	{

		curAnim.SetTrigger ("throwTrigger");

	}



//	public void SpinTriggered ()

//	{

//		curAnim.SetTrigger ("spinTrigger");

//	}



	/// <summary>

	/// Called by an event in the end of the Reload anim. - for bows and crossbows only

	/// </summary>

	public void OnFinishArrowLoadCallback (int value = 0)

	{

		if (value == 1)

			BroadcastMessage ("OnFinishArrowLoad");

	}



	/// <summary>

	/// Called by an event in the end of the Fire animation. - for bows and crossbows only

	/// </summary>

	public void OnArrowShotCallback (int value = 0)

	{

		if (value == 1)

			BroadcastMessage ("OnArrowShot");

	}



	public void OnStrikeCallback (int value = 0)

	{

		if (value == 1)

			BroadcastMessage ("StrikeCallback");

	}



	public void OnStrikeMissCallback (int value = 0)

	{

		if (value == 1)

			BroadcastMessage ("StrikeMissCallback");

	}



	public void OnSwingLeftCallback (int value = 0)

	{

		if (value == 1)

			BroadcastMessage ("SwingLeftCallback");

	}



	public void OnSwingRightCallback (int value = 0)

	{

		if (value == 1)

			BroadcastMessage ("SwingRightCallback");

	}



	public void OnThrowCallback (int value = 0)

	{

		if (value == 1)

			BroadcastMessage ("ThrowCallback");

	}



//	public void OnSpinCallback (int value = 0)

//	{

//		if (value == 1)

//			BroadcastMessage ("SpinCallback");

//	}

		

	FirstPersonController controller

	{

		get

		{

			return transform.root.GetComponent<FirstPersonController>();

		}

	}



	public static event Action onFinishedDisablingAnimation;

	public static event Action onFinishedRaisingAnimation;



	private static PlayerSyncMecanim instance = null;

	public static PlayerSyncMecanim Instance

	{

		get 

		{

			if (instance == null)

				instance = GameObject.FindObjectOfType <PlayerSyncMecanim> ();

			return instance;

		}

	}

}