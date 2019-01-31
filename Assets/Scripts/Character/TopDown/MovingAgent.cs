using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;


[RequireComponent(typeof(EquipmentSystem))]
public class MovingAgent : MonoBehaviour
{
    public LayerMask floorHitLayerMask;
    public LayerMask enemyHitLayerMask;

    // Use this for initialization
    Animator m_anim;
    protected AimIK m_aimIK;
   //rotected Recoil m_recoil;
    private HitReaction m_hitReaction;
    public GameObject targetObject;
    //protected Weapon m_weapon;
    protected RagdollUtility m_ragdoll;
    //protected WeaponProp m_weaponProp;
    protected bool crouched = false;
    protected EquipmentSystem m_equipmentSystem;

    public enum CharacterMainStates { Aimed,Armed_not_Aimed,Idle}

    protected CharacterMainStates m_characterState = CharacterMainStates.Idle;

    // Parameters - temp
    float m_health = 5;
    bool characterEnabled = true;

    public virtual void Awake ()
    {
        m_anim = this.GetComponent<Animator>();
        m_aimIK = this.GetComponent<AimIK>();
        //recoil = this.GetComponent<Recoil>();
        m_hitReaction = this.GetComponentInChildren<HitReaction>();
        m_ragdoll = this.GetComponent<RagdollUtility>();
       // m_weaponProp = this.GetComponentInChildren<WeaponProp>();
       // m_weapon = GetComponentInChildren<Weapon>();
        m_equipmentSystem = this.GetComponent<EquipmentSystem>();
    }

    public virtual void Start()
    {
        if (targetObject == null)
        {
            targetObject = new GameObject();
        }

        m_aimIK.solver.target = targetObject.transform;

        // Set Wepon target
        m_equipmentSystem.setWeaponTarget(targetObject);
    }
	
	// Update is called once per frame
	void FixedUpdate()
    {
        if (characterEnabled)
        {
            updateShooting();
            updateMovment();
            m_equipmentSystem.UpdateSystem();
        }       
    }

    private void Update()
    {
        if(characterEnabled)
        {
            setCharacterState();
        }
    }

    private void updateMovment()
    {
        switch (m_characterState)
        {
            case CharacterMainStates.Aimed:
                //Gun aim
                targetObject.transform.position = getTargetPoint();
                m_aimIK.solver.IKPositionWeight = Mathf.Lerp(m_aimIK.solver.IKPositionWeight, 1, Time.deltaTime *2.5f);

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

                //Swtich between aim and idle
                m_anim.SetBool("aimed", true);

                // Move Character
                Vector3 moveDiection = getMovmentInput();
                moveDiection = this.transform.InverseTransformDirection(moveDiection);
                m_anim.SetFloat("forward", -moveDiection.x);
                m_anim.SetFloat("side", moveDiection.z);
                Vector3 translateDirection = new Vector3(moveDiection.z, 0, -moveDiection.x);
                this.transform.Translate(translateDirection.normalized / 15);

                break;
            case CharacterMainStates.Armed_not_Aimed:
            case CharacterMainStates.Idle:
                //Gun Aim
                m_aimIK.solver.IKPositionWeight = Mathf.Lerp(m_aimIK.solver.IKPositionWeight, 0, Time.deltaTime * 10);

                //Move character and turn
                if (getMovmentInput().magnitude > 0)
                {
                    Vector3 moveDirection = new Vector3(getMovmentInput().z, 0, -getMovmentInput().x);
                    this.transform.rotation = Quaternion.Lerp(this.transform.rotation, Quaternion.LookRotation(moveDirection, Vector3.up), 5f * Time.deltaTime);
                }

                m_anim.SetFloat("forward", getMovmentInput().magnitude);

                float divider = 1;
                if(m_characterState.Equals(CharacterMainStates.Idle))
                {
                    divider = 20;
                }
                else
                {
                    divider = 15;
                }

                this.transform.Translate(Vector3.forward*getMovmentInput().magnitude/ divider);
                

                //Swtich between aim and idle
                m_anim.SetBool("aimed", false);
                break;
        }
    }

    // Getters
    public HitReaction GetHitReaction()
    {
        return m_hitReaction;
    }

    // Get Position of the target
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

    // Set target Height depending on the target type.
    private Vector3 setTargetHeight(Vector3 position ,string tag)
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
           if( m_aimIK.solver.IKPositionWeight >0.3)
            {
                m_equipmentSystem.FireCurrentWeapon();
            }
        }
    }

    /*
     * Set Character State.
     */
    public virtual void setCharacterState()
    {
        if(!m_characterState.Equals(CharacterMainStates.Idle))
        {
            if (Input.GetMouseButton(1))
            {
                
                if(!m_characterState.Equals(CharacterMainStates.Aimed))
                {
                    m_equipmentSystem.OnCharacterStateChanged(CharacterMainStates.Aimed);
                    m_characterState = CharacterMainStates.Aimed;
                }
            }
            else
            {
                
                if (!m_characterState.Equals(CharacterMainStates.Armed_not_Aimed))
                {
                    m_equipmentSystem.OnCharacterStateChanged(CharacterMainStates.Armed_not_Aimed);
                    m_characterState = CharacterMainStates.Armed_not_Aimed;
                }
            }
        }


        if(Input.GetKeyDown(KeyCode.E))
        {
            bool state = m_anim.GetBool("equip");
            state = !state;
            m_anim.SetBool("equip", state);

            if(state)
            {
                m_characterState = CharacterMainStates.Armed_not_Aimed;
            }
            else
            {
                m_characterState = CharacterMainStates.Idle;
            }

        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            bool crouched = m_anim.GetBool("crouched");
            crouched = !crouched;
            m_anim.SetBool("crouched", crouched);
        }
    }

    /*
     * Set Character movment.
     */
    public virtual Vector3 getMovmentInput()
    {
        return new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
    }

    // Temp Code 

    public virtual float getHealth()
    {
        return m_health;
    }

    public virtual void setHealth(float health)
    {
        this.m_health = health;
    }

    public virtual void enableRagdoll()
    {
        m_ragdoll.EnableRagdoll();
        m_aimIK.enabled = false;
        characterEnabled = false;
        m_equipmentSystem.DropCurrentWeapon();
    }

    //public virtual void Equip()
    //{
    //    m_weapon.gameObject.SetActive(true);
    //    m_weaponProp.setVisible(false);
    //}

    //public virtual void UnEquip()
    //{
    //    m_weapon.gameObject.SetActive(false);
    //    m_weaponProp.setVisible(true);
    //}
    
    public virtual void ToggleEquip()
    {
        bool state = m_anim.GetBool("equip");
        state = !state;
        m_anim.SetBool("equip", state);

        if (state)
        {
            m_characterState = CharacterMainStates.Armed_not_Aimed;
        }
        else
        {
            m_characterState = CharacterMainStates.Idle;
        }
    }

    public bool isEquiped()
    {
        return m_anim.GetBool("equip"); 
    }
}
