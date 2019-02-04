using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;

public class EquipmentSystem
{
    // Start is called before the first frame update
    #region protectedParameters
    protected Weapon m_currentWeapon;
    protected WeaponProp m_currentWeaponProp;
    protected GameObject m_target;
    protected Recoil m_recoil;
    protected string m_owner;
    protected AutoMovingAgent.CharacterMainStates m_currentState;
    protected AgentAnimationSystem m_animationSystem;
    #endregion

    public EquipmentSystem(Weapon weapon, WeaponProp prop, string owner, AutoMovingAgent.CharacterMainStates state,GameObject target,Recoil recoil,AgentAnimationSystem animSystem)
    {
        m_currentWeapon = weapon;
        m_currentWeaponProp = prop;
        m_owner = owner;
        m_currentState = state;
        m_target = target;
        m_currentWeapon.setGunTarget(target);
        m_currentWeapon.setAimed(false);
        m_currentWeapon.setOwner(m_owner);
        m_recoil = recoil;
        m_animationSystem = animSystem;
    }


    #region updates
    public void UpdateSystem(MovingAgent.CharacterMainStates state)
    {
        m_currentWeapon.updateWeapon();

        // On Character state change.
        switch (state)
        {
            case MovingAgent.CharacterMainStates.Aimed:

                // Set Gun Aimed;
                if (!m_currentState.Equals(MovingAgent.CharacterMainStates.Aimed))
                {
                    aimCurrentEquipment(true);
                }

                break;
            case MovingAgent.CharacterMainStates.Armed_not_Aimed:

                // Set Gun Aimed;
                if (!m_currentState.Equals(MovingAgent.CharacterMainStates.Armed_not_Aimed))
                {
                    
                    aimCurrentEquipment(false);
                }
                break;
            case MovingAgent.CharacterMainStates.Idle:
                if (!m_currentState.Equals(MovingAgent.CharacterMainStates.Idle))
                {
                    aimCurrentEquipment(false);
                }
                break;
        }


        m_currentState = state;
    }
    #endregion

    #region animationEvents
    // Equip Animation event.
    public void Equip()
    {
        m_currentWeapon.gameObject.SetActive(true);
        m_currentWeaponProp.setVisible(false);
        m_currentWeapon.setGunTarget(m_target);
    }

    // UnEquip Animation event.
    public void UnEquip()
    {
        m_currentWeapon.gameObject.SetActive(false);
        m_currentWeaponProp.setVisible(true);
    }
    #endregion

    #region commands
    public void FireCurrentWeapon()
    {
        if (m_currentWeapon)
        {
            m_currentWeapon.FireProjectile();
            m_recoil.Fire(2);
        }
    }

    public void DropCurrentWeapon()
    {
        m_currentWeapon.dropWeapon();
    }


    public MovingAgent.CharacterMainStates equipCurrentEquipment()
    {
        m_animationSystem.equipEquipment();
        return MovingAgent.CharacterMainStates.Armed_not_Aimed;
    }

    public MovingAgent.CharacterMainStates toggleEquipCurrentEquipment()
    {
        return m_animationSystem.toggleEquip();
    }

    public MovingAgent.CharacterMainStates unEquipCurrentEquipment()
    {
        m_animationSystem.unEquipEquipment();
        return MovingAgent.CharacterMainStates.Idle;
    }

    public void aimCurrentEquipment(bool aimed)
    {
        m_animationSystem.aimEquipment(aimed);
        getCurrentWeapon().setAimed(aimed);
    }

    public bool isProperlyAimed()
    {
        return m_animationSystem.isProperlyAimed();
    }

    #endregion

    #region GettersAndSetters definition

    public void setCurrentWeapon(Weapon currentWeapon)
    {
        this.m_currentWeapon = currentWeapon;
        m_currentWeapon.setGunTarget(m_target);
        m_currentWeapon.setOwner(m_owner);
    }

    public void setCurretnWeaponProp(WeaponProp weaponProp)
    {
        this.m_currentWeaponProp = weaponProp;
    }

    public void setOwner(string owner)
    {
        m_owner = owner;
    }

    public Weapon getCurrentWeapon()
    {
        return m_currentWeapon;
    }

    public WeaponProp getCurrentWeaponProp()
    {
        return m_currentWeaponProp;
    }

    public void setWeaponTarget(GameObject target)
    {
        m_currentWeapon.setGunTarget(target);
    }

    public GameObject getTarget()
    {
        return m_target;
    }

    public bool isEquiped()
    {
        return m_animationSystem.isEquiped();
    }
    #endregion
}
