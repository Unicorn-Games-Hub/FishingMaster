using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

[RequireComponent(typeof(UnityEngine.AudioSource))]
public abstract class WeaponsBaseLegacy : MonoBehaviour
{
	#region Fields

	public WeaponAnimations weaponAnimations;

	protected Animation[] myAnims;

	protected bool canFire = true;

	public AudioClip TakeSound;
	public AudioClip FireSound;

	[HideInInspector] public bool isAimed;
	protected bool canAim;

	private Quaternion defaultCamRot;

	protected bool isReloading = false; 
	protected bool disableWeapon = false;

	protected AudioSource Source;

	[HideInInspector]
	public bool blockHit = false;

	#endregion


	protected virtual void Awake()
	{
		Source = GetComponent<AudioSource>();
		myAnims = GetComponentsInChildren <Animation> ();
		disableWeapon = false;
	}

	protected virtual void Start()
	{
		defaultCamRot = Camera.main.transform.localRotation;
	}

	protected virtual void OnEnable()
	{
		Source.clip = TakeSound;
		Source.Play();
		canFire = true;
		canAim = true;
		disableWeapon = false;
		playerSync.UpdateCurrentWeapon (GetComponent <WeaponsBaseLegacy> ());
		playerSync.WpState = WeaponState.Raising;
	}

	protected virtual void OnDisable ()
	{
		disableWeapon = false;
	}

	void Update()
	{
		InputUpdate();
		Aim();
		SyncState();
	}

	protected abstract void InputUpdate ();

	protected abstract void SyncState ();

	protected virtual void Aim () {}

	protected void PlayWeaponAnimation (string name, WrapMode wp = WrapMode.Default)
	{
		//		myAnims.wrapMode = wp;
		//
		//		if (wp == WrapMode.Loop)
		//			myAnims.CrossFade (name);
		//		else
		//			myAnims.Play (name);

		foreach (Animation a in myAnims)
		{
			a.wrapMode = wp;

			if (a.wrapMode == WrapMode.Loop)
				a.CrossFade (name);
			else
				a.Play (name);
		}
	}

	protected IEnumerator WaitAnimationFinish (AnimationClip clip)
	{
		float reloadTime =0f; //clip.length;
		yield return new WaitForSeconds (reloadTime);
	}
		
	public virtual void DisableWeapon ()
	{
		canFire = false;
		disableWeapon = true;
		StopAllCoroutines();
	}

	public virtual IEnumerator ReloadNormal () 
	{ 
		yield return null; 
	}

	protected FirstPersonController controller
	{
		get
		{
			return transform.root.GetComponent<FirstPersonController>();
		}
	}

	protected PlayerSyncLegacy playerSync
	{
		get
		{
			return PlayerSyncLegacy.Instance;
		}
	}

	protected PlayerManager playerManager 
	{ 
		get 
		{ 
			return PlayerManager.Instance;
		} 
	}

	public bool CanFire
	{
		get
		{
			if (canFire && !isReloading)
				//			if (canFire && !isReloading && !controller.IsRunning && isLoaded)
				return true;
			else
				return false;
		}
	}

	public bool CanAim
	{
		get
		{
			if (canAim)
				return true;
			else
				return false;
		}
	}

	public bool IsReloading
	{
		get 
		{
			return isReloading;
		}
	}

}	// class