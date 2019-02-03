using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;

public class MovingAgent : MonoBehaviour
{
    #region parameters
    // Public - Editor Values
    public enum CharacterMainStates { Aimed, Armed_not_Aimed, Idle }
    public LayerMask floorHitLayerMask;
    public LayerMask enemyHitLayerMask;
    public bool isPlayer = false;

    // Attribute - Systems
    protected EquipmentSystem m_equipmentSystem;
    protected AgentAnimationSystem m_animationSystem;
    protected AgentMovmentSystem m_movmentSystem;
    protected DamageSystem m_damageSystem;
    protected AgentController m_agentController;
    private Vector3 m_movmentVector;
   
    // Attributes
    protected CharacterMainStates m_characterState = CharacterMainStates.Idle;
    protected GameObject m_target;
    bool m_characterEnabled = true;
    #endregion

    #region initalize
    public virtual void Awake ()
    {
        m_target = new GameObject();
        m_movmentVector = new Vector3(0, 0, 0);

        // Create Animation system.
        AimIK m_aimIK = this.GetComponent<AimIK>();
        m_aimIK.solver.target = m_target.transform;
        m_animationSystem = new AgentAnimationSystem(this.GetComponent<Animator>(), this.GetComponent<AimIK>(), 10);

        // Create equipment system.
        Weapon m_currentWeapon = this.GetComponentInChildren<Weapon>();
        WeaponProp m_currentWeaponProp = this.GetComponentInChildren<WeaponProp>();
        m_equipmentSystem = new EquipmentSystem(m_currentWeapon, m_currentWeaponProp, this.transform.name, m_characterState, m_target,GetComponent<Recoil>(),m_animationSystem);

        // Create movment system.
        m_movmentSystem = new AgentMovmentSystem(this.transform,m_characterState,m_target,m_animationSystem);

        m_damageSystem = new DamageSystem(2, this.GetComponent<RagdollUtility>(), this.GetComponentInChildren<HitReaction>());
        
        if(isPlayer)
        {
            m_agentController = new PlayerAgent(enemyHitLayerMask,floorHitLayerMask);
            m_agentController.setMovableAgent(this);
        }
        else
        {
            m_agentController = new AIAgent();
            m_agentController.setMovableAgent(this);
        }
    }
    #endregion

    #region updates
    // Update is called once per frame
    void FixedUpdate()
    {
        if (m_characterEnabled)
        {
            // Update Systems.
            m_animationSystem.UpdateAnimationState(m_characterState);
            m_movmentSystem.UpdateMovmentSystem(m_characterState, m_movmentVector);
            m_equipmentSystem.UpdateSystem(m_characterState);
        }       
    }

    private void Update()
    {
        if(m_characterEnabled && m_agentController != null)
        {
            m_agentController.controllerUpdate();
        }
    }
    #endregion

    #region Need_to_refactor
    // Getters
    public DamageSystem getDamageSystem()
    {
        return m_damageSystem;
    }

    /*
     * Start Shooting.
     */
    public virtual void updateShooting()
    {
        if(Input.GetMouseButtonDown(0) && Input.GetMouseButton(1))
        {
           if(m_equipmentSystem.isProperlyAimed())
            {
                m_equipmentSystem.FireCurrentWeapon();
            }
        }
    }

    public virtual void FireWeapon()
    {
        if (m_equipmentSystem.isProperlyAimed() && m_characterState.Equals(CharacterMainStates.Aimed))
        {
            m_equipmentSystem.FireCurrentWeapon();
        }
    }

    public virtual void AimWeapon()
    {
        if (m_characterState.Equals(CharacterMainStates.Armed_not_Aimed))
        {
            m_characterState = CharacterMainStates.Aimed;
        }
    }

    public virtual void StopAiming()
    {
        if (m_characterState.Equals(CharacterMainStates.Aimed))
        {
             m_characterState = CharacterMainStates.Armed_not_Aimed;
        }
    }

    public virtual void moveCharacter(Vector3 movmentDirection)
    {
        m_movmentVector = movmentDirection;
    }




    public void DestroyCharacter()
    {
        m_equipmentSystem.DropCurrentWeapon();
        m_characterEnabled = false;
        m_damageSystem.destroyCharacter();
        m_animationSystem.disableAnimationSystem();
    }


    public bool isEquiped()
    {
        return m_animationSystem.isEquiped() && (m_characterState.Equals(CharacterMainStates.Armed_not_Aimed) || m_characterState.Equals(CharacterMainStates.Aimed));
    }

    public void Equip()
    {
        m_equipmentSystem.Equip();
    }

    public void UnEquip()
    {
        m_equipmentSystem.UnEquip();
    }
    #endregion

    #region depriciated

    //// Get Position of the target
    //public virtual Vector3 getTargetPoint()
    //{
    //    Vector3 mouse = Input.mousePosition;
    //    Ray castPoint = Camera.main.ScreenPointToRay(mouse);
    //    RaycastHit hit;

    //    if (Physics.Raycast(castPoint, out hit, Mathf.Infinity, enemyHitLayerMask))
    //    {
    //        return setTargetHeight(hit.point, hit.transform.tag);
    //    }

    //    if (Physics.Raycast(castPoint, out hit, Mathf.Infinity, floorHitLayerMask))
    //    {
    //        return setTargetHeight(hit.point, hit.transform.tag);
    //    }
    //    return Vector3.zero;
    //}

    // Set target Height depending on the target type.
    //private Vector3 setTargetHeight(Vector3 position ,string tag)
    //{
    //    switch (tag)
    //    {
    //        case "Floor":
    //            return new Vector3(position.x, 1.25f, position.z);
    //        case "Enemy":
    //            return position;
    //    }
    //    return Vector3.zero; 
    //}

    /*
 * Set Character State.
 */
    //public virtual void setCharacterState()
    //{
    //    if(m_characterState.Equals(CharacterMainStates.Idle))
    //    {
    //        if (Input.GetMouseButton(1))
    //        {

    //            if(!m_characterState.Equals(CharacterMainStates.Aimed))
    //            {
    //                m_characterState = CharacterMainStates.Aimed;
    //            }
    //        }
    //        else
    //        {

    //            if (!m_characterState.Equals(CharacterMainStates.Armed_not_Aimed))
    //            {
    //                m_characterState = CharacterMainStates.Armed_not_Aimed;
    //            }
    //        }
    //    }


    //    if(Input.GetKeyDown(KeyCode.E))
    //    {
    //        m_characterState = m_equipmentSystem.toggleEquipCurrentEquipment();

    //    }

    //    if (Input.GetKeyDown(KeyCode.C))
    //    {
    //        m_animationSystem.toggleCrouched();
    //    }
    //}

    /*
 * Set Character movment.
 */
    //public virtual Vector3 getMovmentInput()
    //{
    //    return new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
    //}

    //private void updateAgent()
    //{
    //    switch (m_characterState)
    //    {
    //        case CharacterMainStates.Aimed:
    //            //Gun aim
    //            m_target.transform.position = getTargetPoint();

    //            //Turn player
    //            float angle = Vector3.Angle(getTargetDirection(), this.transform.forward);

    //            if (getMovmentInput().magnitude < 0.1)
    //            {
    //                if (Mathf.Abs(angle) > 90)
    //                {
    //                    this.transform.LookAt(getTurnPoint(), Vector3.up);
    //                }

    //            }
    //            else
    //            {
    //                this.transform.LookAt(getTurnPoint(), Vector3.up);
    //            }

    //            // Move Character
    //            Vector3 moveDiection = getMovmentInput();
    //            moveDiection = this.transform.InverseTransformDirection(moveDiection);
    //            m_animationSystem.setMovment(-moveDiection.x, moveDiection.z);
    //            Vector3 translateDirection = new Vector3(moveDiection.z, 0, -moveDiection.x);
    //            this.transform.Translate(translateDirection.normalized / 15);

    //            break;

    //        case CharacterMainStates.Armed_not_Aimed:
    //        case CharacterMainStates.Idle:
    //            //Gun Aim
    //            // m_aimIK.solver.IKPositionWeight = Mathf.Lerp(m_aimIK.solver.IKPositionWeight, 0, Time.deltaTime * 10);

    //            //Move character and turn
    //            if (getMovmentInput().magnitude > 0)
    //            {
    //                Vector3 moveDirection = new Vector3(getMovmentInput().z, 0, -getMovmentInput().x);
    //                this.transform.rotation = Quaternion.Lerp(this.transform.rotation, Quaternion.LookRotation(moveDirection, Vector3.up), 5f * Time.deltaTime);
    //            }

    //            m_animationSystem.setMovment(getMovmentInput().magnitude, 0);

    //            float divider = 1;
    //            if (m_characterState.Equals(CharacterMainStates.Idle))
    //            {
    //                divider = 20;
    //            }
    //            else
    //            {
    //                divider = 15;
    //            }

    //            this.transform.Translate(Vector3.forward * getMovmentInput().magnitude / divider);
    //            break;
    //    }
    //}

    /*
    * Current Target direction from the player
    */
    //public virtual Vector3 getTargetDirection()
    //{
    //    return (getTargetPoint() - this.transform.position).normalized;
    //}

    /*
        * Get Target Position with y value as characters feet height.
        */
    //public virtual Vector3 getTurnPoint()
    //{
    //    Vector3 position = m_target.transform.position;
    //    position.y = this.transform.position.y;
    //    return position;
    //}

    /*
        * Get Target Postion with y value as characters weapon level height. 
        */
    //public virtual Vector3 getLookPoint()
    //{
    //    Vector3 position = m_target.transform.position;
    //    position.y = this.transform.position.y + 1.25f;
    //    return position;
    //}
    #endregion

    //public EquipmentSystem getEquipmentSystem()
    //{
    //    return m_equipmentSystem;
    //}

    public AgentAnimationSystem getAnimationSystem()
    {
        return m_animationSystem;
    }

    public void toggleCurrentWeapon()
    {
        m_characterState = m_equipmentSystem.toggleEquipCurrentEquipment();
    }

    public void setTargetPoint(Vector3 position)
    {
        m_target.transform.position = position;
    }

    //public CharacterMainStates getCurrentState()
    //{
    //    return m_characterState;
    //}

    //public void setCharacterState(CharacterMainStates state)
    //{
    //    m_characterState = state;
    //}
}
