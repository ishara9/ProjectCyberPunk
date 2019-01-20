﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;

public class MovingAgent : MonoBehaviour
{
    public LayerMask floorHitLayerMask;
    public LayerMask enemyHitLayerMask;

    // Use this for initialization
    Animator m_anim;
    protected AimIK m_aimIK;
    protected Recoil m_recoil;
    private HitReaction m_hitReaction;
    public GameObject targetObject;
    protected Weapon m_weapon;

    public enum CharacterMainStates { Aim,Idle}

    protected CharacterMainStates m_characterState;

	void Start ()
    {
        m_anim = this.GetComponent<Animator>();
        m_aimIK = this.GetComponent<AimIK>();
        m_recoil = this.GetComponent<Recoil>();
        m_hitReaction = this.GetComponentInChildren<HitReaction>();

        if(targetObject == null)
        {
            targetObject = new GameObject();
        }
        m_weapon = GetComponentInChildren<Weapon>();
        m_aimIK.solver.target = targetObject.transform;
        m_weapon.setGunTarget(targetObject);
    }
	
	// Update is called once per frame
	void FixedUpdate()
    {
        setCharacterState(); //mouse right clicked or not , isAimed
        updateShooting();
        updateMovment();
        m_weapon.updateWeapon();
    }

    private void updateMovment()
    {
        switch (m_characterState)
        {
            case CharacterMainStates.Aim:
                //Gun aim
                targetObject.transform.position = getTargetPoint();
                m_aimIK.solver.IKPositionWeight = Mathf.Lerp(m_aimIK.solver.IKPositionWeight, 1, Time.deltaTime * 4);

                //Turn player
                float angle = Vector3.Angle(getTargetDirection(), this.transform.forward);

                if (getMovmentInput().magnitude < 0.1)
                {
                    if (Mathf.Abs(angle) > 90 )
                    {
                        this.transform.LookAt(getTurnPoint(), Vector3.up);
                    }

                }
                else
                {
                    this.transform.LookAt(getTurnPoint(), Vector3.up);
                }

                //Swtich between aim and idle state
                m_anim.SetBool("aimed", true);

                // Move Character
                Vector3 moveDiection = getMovmentInput();
                moveDiection = this.transform.InverseTransformDirection(moveDiection); //convert character move direction to world direction
                m_anim.SetFloat("forward", -moveDiection.x);
                m_anim.SetFloat("side", moveDiection.z);

                break;
            case CharacterMainStates.Idle:
                //Gun Aim
                m_aimIK.solver.IKPositionWeight = Mathf.Lerp(m_aimIK.solver.IKPositionWeight, 0, Time.deltaTime * 10);

                //Move character and turn
                if(getMovmentInput().magnitude >0)
                {
                    Vector3 moveDirection = new Vector3(getMovmentInput().z, 0, -getMovmentInput().x);
                    this.transform.rotation = Quaternion.Lerp(this.transform.rotation, Quaternion.LookRotation(moveDirection, Vector3.up), 5f * Time.deltaTime);
                    m_anim.SetFloat("forward", getMovmentInput().magnitude);
                }

                //Swtich between aim and idle
                m_anim.SetBool("aimed", false);
                break;
        }
    }

    // Getters
    HitReaction GetHitReaction()
    {
        return m_hitReaction;
    }


    public virtual Vector3 getTargetPoint()
    {
        Vector3 mouse = Input.mousePosition;
        Ray castPoint = Camera.main.ScreenPointToRay(mouse);
        RaycastHit hit;

        if (Physics.Raycast(castPoint, out hit, Mathf.Infinity, enemyHitLayerMask))
        {
            return setTargetHeight(hit.point, hit.transform.tag);
        }

        if (Physics.Raycast(castPoint, out hit, Mathf.Infinity, floorHitLayerMask))
        {
            return setTargetHeight(hit.point, hit.transform.tag);
        }
        return Vector3.zero;
    }

    public Vector3 setTargetHeight(Vector3 position ,string tag)
    {
        switch (tag)
        {
            case "Floor":
                return new Vector3(position.x, 1.25f, position.z);
            case "Enemy":
                return position;
        }
        return Vector3.zero; 
    }

    /*
     * Current Target direction from the player
     */
    public virtual Vector3 getTargetDirection()
    {
        return (getTargetPoint() - this.transform.position).normalized;
    }

    /*
     * Get Target Position with y value as characters feet height.
     */
    public virtual Vector3 getTurnPoint()
    {
        Vector3 position = targetObject.transform.position;
        position.y = this.transform.position.y;
        return position;
    }

    /*
     * Get Target Postion with y value as characters weapon level height. 
     */
    public virtual Vector3 getLookPoint()
    {
        Vector3 position = targetObject.transform.position;
        position.y = this.transform.position.y + 1.25f;
        return position;
    }

    /*
     * Start Shooting.
     */
    public virtual void updateShooting()
    {
        if(Input.GetMouseButtonDown(0) && Input.GetMouseButton(1))
        {
			//solver represent hand position, fire only when hand comes to aim. (0 -idle position, 1- fully targeted)
           if( m_aimIK.solver.IKPositionWeight >0.9)
            {
                m_recoil.Fire(2); //character str value + recoil value

                if (Input.GetMouseButton(0))
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                    // Raycast to find a ragdoll collider
                    RaycastHit hit = new RaycastHit();
                    if (Physics.Raycast(ray, out hit, 100f,enemyHitLayerMask))
                    {
                      MovingAgent movingAgnet= hit.transform.GetComponentInParent<MovingAgent>();

                        if(movingAgnet != null)
                        {
                            movingAgnet.GetHitReaction().Hit(hit.collider, (hit.transform.position - this.transform.position) * 0.6f, hit.point);
                        }
                        // Use the HitReaction

                    }
                }
            }
        }
    }

    /*
     * Set Character State.
     */
    public virtual void setCharacterState()
    {
        if (Input.GetMouseButton(1))
        {
            m_characterState = CharacterMainStates.Aim;
            m_weapon.setAimed(true);
        }
        else
        {
            m_characterState = CharacterMainStates.Idle;
            m_weapon.setAimed(false);
        }
    }

    /*
     * Set Character movment.
     */
    public virtual Vector3 getMovmentInput()
    {
        return new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
    }
}
