using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;

public class EquipmentSystem : MonoBehaviour
{
    // Start is called before the first frame update
    protected Weapon m_currentWeapon;
    protected WeaponProp m_currentWeaponProp;
    protected GameObject target;
    protected Recoil m_recoil;


    void Awake()
    {
        // Intialize Weapon
        m_currentWeapon = GetComponentInChildren<Weapon>();
        m_currentWeaponProp = GetComponentInChildren<WeaponProp>();
        m_recoil = this.GetComponent<Recoil>();
    }

    // Update is called once per frame
    void Start()
    {
        m_currentWeapon.setGunTarget(target);
        m_currentWeapon.setOwner(this.name);
    }

    public void setCurrentWeapon(Weapon currentWeapon)
    {
        this.m_currentWeapon = currentWeapon;
        m_currentWeapon.setGunTarget(target);
        m_currentWeapon.setOwner(this.transform.name);
    }

    public void setCurretnWeaponProp(WeaponProp weaponProp)
    {
        this.m_currentWeaponProp = weaponProp;
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

    public void UpdateSystem()
    {
        m_currentWeapon.updateWeapon();
    }

    public void DropCurrentWeapon()
    {
        m_currentWeapon.dropWeapon();
    }

    public void Equip()
    {
        m_currentWeapon.gameObject.SetActive(true);
        m_currentWeaponProp.setVisible(false);
    }

    public void UnEquip()
    {
        m_currentWeapon.gameObject.SetActive(false);
        m_currentWeaponProp.setVisible(true);
    }

    public void OnCharacterStateChanged(MovingAgent.CharacterMainStates state)
    {
        switch (state)
        {
            case MovingAgent.CharacterMainStates.Aimed:
                getCurrentWeapon().setAimed(true);
                break;
            case MovingAgent.CharacterMainStates.Armed_not_Aimed:
                getCurrentWeapon().setAimed(false);
                break;
            case MovingAgent.CharacterMainStates.Idle:
                break;
        }

    }

    public void FireCurrentWeapon()
    {
        if(m_currentWeapon)
        {
            m_currentWeapon.FireProjectile();
            m_recoil.Fire(2);
        }
    }
}
