using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;

public class EquipmentSystem
{
    #region protectedParameters
    protected Weapon m_currentWeapon;

    protected Weapon m_rifle;
    protected Weapon m_pistol;
    protected WeaponProp m_rifleProp;
    protected WeaponProp m_pistolProp;
    protected GameObject m_target;
    protected Recoil m_recoil;
    protected string m_owner;
    protected AutoMovingAgent.CharacterMainStates m_currentState;
    protected AgentAnimationSystem m_animationSystem;

    private bool m_inEquipingAction = false;
    #endregion

    public EquipmentSystem(Weapon[] weapons, WeaponProp[] props, string owner, AutoMovingAgent.CharacterMainStates state,GameObject target,Recoil recoil,AgentAnimationSystem animSystem)
    {
        m_owner = owner;
        m_currentState = state;
        m_target = target;
        m_recoil = recoil;
        m_animationSystem = animSystem;
        getAllWeapons(weapons, props);
    }


    #region updates
    public void UpdateSystem(MovingAgent.CharacterMainStates state)
    {
        if(m_currentWeapon !=null)
        {
            m_currentWeapon.updateWeapon();
        }


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
        Weapon.WEAPONTYPE type = m_currentWeapon.getWeaponType();
        m_inEquipingAction = false;

        switch (type)
        {
            case Weapon.WEAPONTYPE.primary:
                // Select rifle as currentWeapon
                m_rifleProp.setVisible(false);
                m_currentWeapon = m_rifle;
                break;

            case Weapon.WEAPONTYPE.secondary:
                // Select pistol as currentWeapon
                m_pistolProp.setVisible(false);
                m_currentWeapon = m_pistol;
                break;
        }

        // Set Current Weapon Properties.
        m_currentWeapon.gameObject.SetActive(true);
        m_currentWeapon.setGunTarget(m_target);
    }

    // UnEquip Animation event.
    public void UnEquip()
    {
        Weapon.WEAPONTYPE type = m_currentWeapon.getWeaponType();
        m_currentWeapon.gameObject.SetActive(false);
        m_currentWeapon = null;
        m_inEquipingAction = false;

        switch (type)
        {
            case Weapon.WEAPONTYPE.primary:
                m_rifleProp.setVisible(true);
                break;

            case Weapon.WEAPONTYPE.secondary:
                m_pistolProp.setVisible(true);
                break;
        }
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
       return m_animationSystem.equipCurrentEquipment();
    }

    public MovingAgent.CharacterMainStates unEquipCurrentEquipment()
    {
       return m_animationSystem.unEquipEquipment();
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
        this.m_pistolProp = weaponProp;
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
        return m_pistolProp;
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

    public MovingAgent.CharacterMainStates togglePrimary()
    {
        if (!m_inEquipingAction)
        {
            m_animationSystem.setCurretnWeapon(1);

            if (m_currentWeapon != null)
            {
                if (m_currentWeapon.getWeaponType().Equals(Weapon.WEAPONTYPE.primary))
                {
                    m_inEquipingAction = true;
                    return m_animationSystem.unEquipEquipment();
                }
                else
                {
                    // Fast toggle
                    m_currentWeapon = m_rifle;
                    m_pistol.gameObject.SetActive(false);
                    m_rifle.gameObject.SetActive(true);
                    m_rifleProp.setVisible(false);
                    m_pistolProp.setVisible(true);
                    return m_animationSystem.equipCurrentEquipment();
                }
            }
            else
            {
                m_inEquipingAction = true;
                m_currentWeapon = m_rifle;
                return m_animationSystem.equipCurrentEquipment();
            }
        }
        else
        {
            return m_currentState;
        }

    }

    public MovingAgent.CharacterMainStates toggleSecondary()
    {
        if(!m_inEquipingAction)
        {
            m_animationSystem.setCurretnWeapon(0);

            if (m_currentWeapon != null)
            {
                if (m_currentWeapon.getWeaponType().Equals(Weapon.WEAPONTYPE.secondary))
                {
                    m_inEquipingAction = true;
                    return m_animationSystem.unEquipEquipment();
                }
                else
                {
                    // Fast toggle
                    m_currentWeapon = m_pistol;
                    m_rifle.gameObject.SetActive(false);
                    m_rifleProp.setVisible(true);
                    m_pistolProp.setVisible(false);
                    m_pistol.gameObject.SetActive(true);
                    return m_animationSystem.equipCurrentEquipment();
                }
            }
            else
            {
                m_inEquipingAction = true;
                m_currentWeapon = m_pistol;
                return m_animationSystem.equipCurrentEquipment();
            }
        }
        else
        {
            return m_currentState;
        }



    }

    private void getAllWeapons(Weapon[] weapons, WeaponProp[] props)
    {
        foreach (Weapon wep in weapons)
        {
            wep.setOwner(m_owner);
            wep.setAimed(false);
            wep.setGunTarget(m_target);
            wep.gameObject.SetActive(false);

            Weapon.WEAPONTYPE type = wep.getWeaponType();

            switch (type)
            {
                case Weapon.WEAPONTYPE.primary:
                    m_rifle = wep;
                    break;

                case Weapon.WEAPONTYPE.secondary:
                    m_pistol = wep;
                    break;
            }
        }

        foreach (WeaponProp prop in props)
        {
            Weapon.WEAPONTYPE type = prop.getPropType();

            switch (type)
            {
                case Weapon.WEAPONTYPE.primary:
                    m_rifleProp = prop;
                    break;

                case Weapon.WEAPONTYPE.secondary:
                    m_pistolProp = prop;
                    break;
            }
        }
    }
    #endregion
}
