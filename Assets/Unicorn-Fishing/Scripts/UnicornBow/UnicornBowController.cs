using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnicornBowController : MonoBehaviour
{
   public static UnicornBowController instance;

   [Header("Bow Arrow")]
   public GameObject arrowPrefab;

   [Header("Arrow Spawn Position")]
   public Transform arrowSpawnPos;

   [Header("Arrow Attatched To Bow")]
   public GameObject attatchedArrow;

   [Header("Camera")]
   public Camera gameCam;

   [Header("Player Hand")]
   public GameObject playerHand;
   private Animation handAnim;
   public AnimationClip[] handAnimations;

   [Header("Player Bow")]
   public GameObject playerBow;
   private Animation bowAnim;
   public AnimationClip[] bowAnimations;

   private bool canShoot=false;

   public Transform character;

   public bool fishCatched=false;

   
   void Awake()
   {
      if(instance!=null)
      {
         return;
      }
      else
      {
         instance=this;
      }
   }

   void Start()
   {
      //initially bow and hand must be at idle position
      handAnim=playerHand.GetComponent<Animation>();
      bowAnim=playerBow.GetComponent<Animation>();

      PlayIdleAnimations();
   }

   void Update()
   {  
      if(Input.GetKeyDown(KeyCode.V)&&!canShoot)
      {
         TimeToHideArrowRope();
         PlayAimAnimations();
         //gameCam.fieldOfView=60;
         //zoom the screen and adjust bow pos
         newTargetPos=new Vector3(-0.05f,0f,-0.3f);
         canShoot=true;
      }
      else if(Input.GetMouseButtonDown(0)&&canShoot)
      {
         HandleArrowShooting();
         canShoot=false;
      }
      else if(canShoot&&Input.GetKeyDown(KeyCode.V))
      {
         PlayIdleAnimations();
         canShoot=false;
      }

      character.transform.localPosition=Vector3.Lerp(character.transform.localPosition,newTargetPos,Time.deltaTime*3f);
   }

   private Vector3 newTargetPos=Vector3.zero;

   void HandleArrowShooting()
   {
      attatchedArrow.SetActive(false);
      Quaternion q = Quaternion.Euler (new Vector3 (0, transform.eulerAngles.y - 90f, -transform.eulerAngles.x));
      GameObject newArrow=Instantiate(arrowPrefab,arrowSpawnPos.position,q);
      newArrow.GetComponent<UnicornArrow>().HandleArrowInfo(transform.forward);
      PlayShootToIdleAnimations();

      if(BowRopeHandler.instance!=null)
      {
         BowRopeHandler.instance.LaunchHook(newArrow.transform.GetChild(0).GetChild(0).gameObject);
      }
   }

   public IEnumerator WaitBeforePullingRope()
   {
      yield return new WaitForSeconds(1f);
      PlayRopePullAnimations();
   }

   void ReloadArrow()
   {   
      attatchedArrow.SetActive(true);
   }

   #region Animation
   void PlayIdleAnimations()
   {
      attatchedArrow.transform.localPosition=new Vector3(0.02f,0f,0f);
      if(!attatchedArrow.activeInHierarchy)
      {
         attatchedArrow.SetActive(true);
      }
      handAnim.clip=handAnimations[0];
      bowAnim.clip=bowAnimations[0];
      handAnim.Play();
      bowAnim.Play();
      newTargetPos=new Vector3(0f,0f,0.15f);
   }

   void PlayAimAnimations()
   {
      attatchedArrow.transform.localPosition=new Vector3(0.068f,0f,0f);
      handAnim.clip=handAnimations[1];
      bowAnim.clip=bowAnimations[1];
      handAnim.Play();
      bowAnim.Play();
      attatchedArrow.SetActive(true);
   }

   void PlayShootToIdleAnimations()
   {
      handAnim.clip=handAnimations[2];
      bowAnim.clip=bowAnimations[2];
      handAnim.Play();
      bowAnim.Play();
      StartCoroutine(HandleIdlePos());
   }

   IEnumerator HandleIdlePos()
   {
      yield return new WaitForSeconds(0.6f);
      newTargetPos=new Vector3(0f,0f,0.15f);
   }

   void PlayRopePullAnimations()
   {
      if(UnicornArrowRope.instance!=null)
      {
         UnicornArrowRope.instance.canPullRope=true;
      }
      handAnim.clip=handAnimations[3];
      bowAnim.clip=bowAnimations[3];
      handAnim.Play();
      bowAnim.Play();
     newTargetPos=new Vector3(-0.25f,0f,-0.05f);
   }

   public void StopPullingRope()
   {
      PlayIdleAnimations();
   }

   void TimeToHideArrowRope()
   {
      if(UnicornArrowRope.instance!=null)
      {
         UnicornArrowRope.instance.DetatchRope();
      }

      if(BowRopeHandler.instance!=null)
      {
         BowRopeHandler.instance.DetachHook();
      }
      PlayIdleAnimations();
   }
   #endregion
}
