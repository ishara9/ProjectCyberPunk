using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;
public class Shooter : MonoBehaviour {

	// Shooting Action Parameters
	[Header("Shooting Parameters")]
	public Transform hoverboard;
	public Vector3 offset;
	private Animator m_animator;
	private Transform chest;
    private Gun m_gun;
	private AimIK m_aimIk;
    private RagdollController m_ragdoll;
    private Recoil m_recoil;


	// HoverBoard Motion Paramters
	public enum MOVINGSTATE {LEFT,RIGHT,IDLE}
	public enum JUMPINGSTATE{JUMP,IDLE}
    public enum SHOOTINGSTATE {IDLE, AIM,FIRE,RECOVER}

	private JUMPINGSTATE currentJumpingState = JUMPINGSTATE.IDLE;
	private MOVINGSTATE currentMovingState = MOVINGSTATE.IDLE;
    private SHOOTINGSTATE currentShootingStage = SHOOTINGSTATE.IDLE;

    private bool playerActive = true;
	private float SideMovedDistance =0;
	private float turnAngle;
	public float hoverboardCurrnetHoverDistance;
	private float hoverboardAvarageHeight;
	private float currentJumpHeight;
	private float jumpCurvePosition;
	private float hoberboardLocalPos;
	private float hoverboardActionPos;
	private float randomActionFactor1;
	private float randomActionFactor2;
	private float originalHoverMovingRate;
    private bool m_gunAimDone = false;

	[Header("HoverBoard Motion")]
	public float maxSideMoveDistance = 2;
	public float movingSPeed =5f;
	public float turningSPeed = 5f;
	public float hoverboardHoverMovingRate = 200f;
	public float hoverboardHoverDistance = 0.15f;
	public float jumpMaxHeight;
	public float jumpSpeed;
	// Hoberboard turning angle when moving to side.
	public float maxTurnAngle;
	public Transform leftFoot;
	public Transform rightFoot;
	public AnimationCurve jumpCurve;
	public AnimationCurve hoverboardActionCurve;

	void Awake () 
	{
		m_animator = GetComponent<Animator>();
        m_gun = this.GetComponentInChildren<Gun>();
		m_aimIk = this.GetComponent<AimIK>();
        m_ragdoll = this.GetComponent<RagdollController>();
        m_recoil = this.GetComponent<Recoil>();
	}

    void Start()
    {
        chest = m_animator.GetBoneTransform(HumanBodyBones.Chest);
        hoverboardAvarageHeight = this.transform.position.y;
        hoberboardLocalPos = hoverboard.localPosition.y;
        originalHoverMovingRate = hoverboardHoverMovingRate;
		m_aimIk.solver.IKPositionWeight =0;
        m_ragdoll.setRagdollState(false);
        m_gun.setNotifyRecoilStart(gunRecoilAction);
    }

	// Main Update function of the shooter
	public void ShooterLateUpdate () 
	{
        if(playerActive)
        {
            //aimAtTarget();
            updatePlayerMovment();

            /*
             * Update Shoting Action
             * Update AimIK weight
             * Update Gun 
             */
            UpdateShootingAction();
        }


        //if (Input.GetKeyDown(KeyCode.LeftArrow))
        //{
        //    moveSide(MOVINGSTATE.LEFT);
        //}
        //else if (Input.GetKeyDown(KeyCode.RightArrow))
        //{
        //    moveSide(MOVINGSTATE.RIGHT);
        //}

        //if (Input.GetKeyDown(KeyCode.Space) && currentJumpingState.Equals(JUMPINGSTATE.IDLE))
        //{
        //    currentJumpingState = JUMPINGSTATE.JUMP;

        //    Set Random factors for randomize hoverboard jump action animation.
        //   randomActionFactor1 = Random.Range(2, 4);
        //    randomActionFactor2 = Random.Range(-2, 2);
        //}
    }

    // Update IK for foot
	void OnAnimatorIK()
    {
		if(m_animator && leftFoot !=null && rightFoot !=null && playerActive)
		{
			m_animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, playerActive ? 1 : 0);
			m_animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, playerActive ? 1 : 0);
			m_animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, playerActive ? 1 : 0);
			m_animator.SetIKRotationWeight(AvatarIKGoal.RightFoot,1);

			m_animator.SetIKPosition(AvatarIKGoal.LeftFoot,leftFoot.position);
			m_animator.SetIKPosition(AvatarIKGoal.RightFoot,rightFoot.position);

			m_animator.SetIKRotation(AvatarIKGoal.LeftFoot,leftFoot.rotation);
			m_animator.SetIKRotation(AvatarIKGoal.RightFoot,rightFoot.rotation);
		}
	}


	// ---- Non-inherited functions
    public void JumpUp()
    {
        currentJumpingState = JUMPINGSTATE.JUMP;

        // Set Random factors for randomize hoverboard jump action animation.
        randomActionFactor1 = Random.Range(2, 4);
        randomActionFactor2 = Random.Range(-2, 2);
    }

    /**
     * Start Shooting gun
     * 
     */
    public void Shoot(Transform target)
    {
        // Start Shooting Animation.
        m_animator.SetBool("shoot",true);
        m_animator.SetBool("idle", false);
        m_aimIk.solver.target = target;

        // Notify Start Aiming.
        currentShootingStage = SHOOTINGSTATE.AIM;
        m_gunAimDone = false;
    }

    public void OnShootAnimEvent()
    {
        //m_gun.FireGun(target);
    }

    /*
    * Set Player side movment
    */
	public void moveSide(MOVINGSTATE movingState)
	{
		
		if( movingState.Equals(MOVINGSTATE.LEFT) && currentMovingState != MOVINGSTATE.LEFT)
		{
			currentMovingState = MOVINGSTATE.LEFT;
		}else if(movingState.Equals(MOVINGSTATE.RIGHT) && currentMovingState != MOVINGSTATE.RIGHT)
		{
			currentMovingState = MOVINGSTATE.RIGHT;
		}

        randomActionFactor1 = Random.Range(2, 4);
        randomActionFactor2 = Random.Range(-2, 2);
    }

    public void setPlayerActiveState(bool state)
    {
        playerActive = state;
    }

    /*
    * Aim at target.
    * - No longer using this
    */
	//private void aimAtTarget()
	//{
	//	chest.LookAt(target);
	//	chest.rotation = chest.rotation * Quaternion.Euler(offset);
	//}


	/*
	 * Controls side motion of hoverboard
	 */
	private void updateMoveSIde()
	{
		//DebugText.DebuggerText.setDebugText(turnAngle.ToString());
		//Debug.Log(turnAngle + "and" + currentMovingState);

		switch (currentMovingState)
		{
			case MOVINGSTATE.IDLE:
				 turnAngle = Mathf.Lerp(turnAngle,0, movingSPeed * Time.deltaTime);
			break;
			case MOVINGSTATE.LEFT:
				SideMovedDistance += Time.deltaTime*movingSPeed;
				this.transform.Translate(this.transform.InverseTransformDirection(Vector3.left)*Time.deltaTime*movingSPeed);
				if(SideMovedDistance > maxSideMoveDistance)
				{
					SideMovedDistance =0;
					currentMovingState = MOVINGSTATE.IDLE;
					return;
				}

				if(SideMovedDistance < maxSideMoveDistance/2.5f)
				{
					turnAngle = Mathf.Lerp(turnAngle,maxTurnAngle, movingSPeed * Time.deltaTime);
				}
				else
				{
					turnAngle = Mathf.Lerp(turnAngle,0,movingSPeed*Time.deltaTime);
				}

			break;
			case MOVINGSTATE.RIGHT:
				SideMovedDistance += Time.deltaTime*movingSPeed;
				this.transform.Translate(this.transform.InverseTransformDirection(Vector3.right)*Time.deltaTime*movingSPeed);
				if(SideMovedDistance > maxSideMoveDistance)
				{
					SideMovedDistance =0;
					currentMovingState = MOVINGSTATE.IDLE;
					return;
				}

				if(SideMovedDistance < maxSideMoveDistance/2.5f)
				{
					turnAngle = Mathf.Lerp(turnAngle,-maxTurnAngle,movingSPeed * Time.deltaTime);
				}
				else
				{
					turnAngle = Mathf.Lerp(turnAngle,0, movingSPeed * Time.deltaTime);
				}

			break;
		};
	}

	/*
	* Hovering motion and jumping motion of hoverboard;
	*/
	private void updateMoveUp()
	{
		// Control Jumping
		switch(currentJumpingState)
		{
			case JUMPINGSTATE.IDLE:
				//currentJumpHeight = Mathf.Lerp(currentJumpHeight,0,Time.deltaTime*jumpSpeed/2);
				currentJumpHeight =0;
				hoverboardHoverMovingRate = Mathf.Lerp(hoverboardHoverMovingRate,originalHoverMovingRate,Time.deltaTime);

				// Only hover motion when idle
				hoverboardCurrnetHoverDistance += Time.deltaTime*hoverboardHoverMovingRate;
			break;
			case JUMPINGSTATE.JUMP:
				hoverboardHoverMovingRate = Mathf.Lerp(hoverboardHoverMovingRate,originalHoverMovingRate*5,Time.deltaTime*5);
				jumpCurvePosition += Time.deltaTime*1;
				currentJumpHeight = jumpCurve.Evaluate(jumpCurvePosition)*2;
				hoverboardActionPos = hoverboardActionCurve.Evaluate(jumpCurvePosition)*20;
				hoverboard.transform.localPosition = new Vector3(hoverboard.transform.localPosition.x,hoberboardLocalPos + currentJumpHeight/8,hoverboard.transform.localPosition.z);
				if(jumpCurvePosition >= 1)
				{
					jumpCurvePosition = 0;
					currentJumpHeight = 0;
					currentJumpingState = JUMPINGSTATE.IDLE;
				}
			break;
		}
	}


    private void updatePlayerMovment()
    {
        /*
        * Hoverboard rotation = rotation when side moving + roation when jumping. Need turnAngle calculated from moveSIde function.
        * Need hoverboardActionPos calculated from moveUp function
        */
        hoverboard.transform.rotation = Quaternion.Euler(new Vector3(hoverboardActionPos * -randomActionFactor1/2, hoverboardActionPos * randomActionFactor2 / 2, turnAngle));

        // Character Rotation when side moving.
        this.transform.rotation = Quaternion.Euler(new Vector3(0, 0, turnAngle / 5));

        // Hovering motion of hoverboard;
        float Hoverposition = Mathf.Sin(hoverboardCurrnetHoverDistance / 180 * Mathf.PI);

        // All up and down motion of hover board. Jumping + hovering
        this.transform.position = new Vector3(this.transform.position.x, hoverboardAvarageHeight + currentJumpHeight + Hoverposition * hoverboardHoverDistance, this.transform.position.z);

        // Does Side Motion Calculations and produce result for turnAngle.
        updateMoveSIde();

        // Does Up Down Motion calculates and produe resutls currentJumpHeight,Hoverposition and hoverboardActionPos.
        updateMoveUp();
    }

    private void UpdateShootingAction()
    {

        switch (currentShootingStage)
        {
            case SHOOTINGSTATE.IDLE:
                break;
            case SHOOTINGSTATE.AIM:
                m_aimIk.solver.IKPositionWeight = Mathf.Lerp(m_aimIk.solver.IKPositionWeight, 1, Time.deltaTime * 10);

                if (m_aimIk.solver.IKPositionWeight > 0.99f)
                {
                    m_gun.pulTrigger();
                    currentShootingStage = SHOOTINGSTATE.FIRE;
                }
                break;
            case SHOOTINGSTATE.FIRE:

                // Wait until gun firing over.
                if(!m_gun.isShoting())
                {
                    currentShootingStage = SHOOTINGSTATE.RECOVER;

                    // Set animation back to idle.
                    m_animator.SetBool("idle", true);
                    m_animator.SetBool("shoot", false);
                }
                break;
            case SHOOTINGSTATE.RECOVER:
                m_aimIk.solver.IKPositionWeight = Mathf.Lerp(m_aimIk.solver.IKPositionWeight, 0, Time.deltaTime * 10);

                if(m_aimIk.solver.IKPositionWeight < 0.05f)
                {
                    currentShootingStage = SHOOTINGSTATE.IDLE;
                }

                break;
        }

        //if (m_gun.isShoting())
        //{
        //    m_aimIk.solver.IKPositionWeight = Mathf.Lerp(m_aimIk.solver.IKPositionWeight, 1, Time.deltaTime * 10);

        //    // Pull the trigger when aim is ready.
        //    if(!m_gunAimDone && m_aimIk.solver.IKPositionWeight > 0.99f)
        //    {
        //        m_gun.pulTrigger();
        //    }
        //}
        //else
        //{
        //    m_aimIk.solver.IKPositionWeight = Mathf.Lerp(m_aimIk.solver.IKPositionWeight, 0, Time.deltaTime * 10);
        //    m_animator.SetBool("idle", true);
        //    m_animator.SetBool("shoot", false);
        //}

        if (m_gun != null)
        {
            m_gun.UpdateGun();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
       if(other.tag == "Vehicles")
        {
            //m_ragdoll.setRagdollState(true);
            //m_ragdoll.AddImpulseToRagdoll(RagdollPart.TYPE.Chest, -Vector3.forward * 1000);
            //this.setPlayerActiveState(false);
        }
    }

    private void gunRecoilAction()
    {
        m_recoil.Fire(10);
    }
}
