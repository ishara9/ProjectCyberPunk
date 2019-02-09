using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum WEAPONTYPE { primary,secondary};

    public GameObject targetPoint;
    public GameObject target;
    public GameObject projectile;
    public LayerMask hitLayerMask;
    public WEAPONTYPE m_weaponType;

    public bool isAimed = false;
    private LineRenderer m_line;
    private Rigidbody m_rigidbody;
    private BoxCollider m_collider;
    private string ownerName;
    private bool enableLine;
    private Vector3 gunFireingPoint;
    


    public void Awake()
    {
        m_line = this.GetComponent<LineRenderer>();
        m_rigidbody = this.GetComponent<Rigidbody>();
        m_collider = this.GetComponent<BoxCollider>();
    }
    public void setGunTarget(GameObject target)
    {
        this.target = target;
    }

    public void setAimed(bool aimed)
    {
        isAimed = aimed;
        if(enableLine)
        {
            m_line.enabled = aimed;
        }

    }

    public void updateWeapon()
    {
        if (isAimed && enableLine)
        {
            Vector3 direction = target.transform.position - targetPoint.transform.position;
            m_line.SetPosition(0, targetPoint.transform.position);


            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            // Raycast to find a ragdoll collider
            RaycastHit hit = new RaycastHit();

            if (Physics.Raycast(targetPoint.transform.position, direction.normalized, out hit, 1000, hitLayerMask))
            {
                m_line.SetPosition(1, hit.point);
            }
            else
            {
                m_line.SetPosition(1, targetPoint.transform.position + direction * 50);
            }

            Debug.DrawRay(targetPoint.transform.position, direction.normalized, Color.red);
        }

        gunFireingPoint = targetPoint.transform.position - targetPoint.transform.forward * 0.1f;
    }

    public void dropWeapon()
    {
        this.transform.parent = null;
        m_rigidbody.isKinematic = false;
        m_rigidbody.useGravity = true;
        m_collider.isTrigger = false;
    }

    public void FireProjectile()
    {
        GameObject Tempprojectile = GameObject.Instantiate(projectile, gunFireingPoint, this.transform.rotation);
        Tempprojectile.transform.forward =( target.transform.position - targetPoint.transform.position).normalized;
        Tempprojectile.GetComponent<ProjectileBasic>().speed = 1f;
        Tempprojectile.GetComponent<ProjectileBasic>().setShooterName(ownerName);
    }

    public void setOwner(string owner)
    {
        ownerName = owner;
    }

    public void SetGunTargetLineStatus(bool status)
    {
        enableLine = status;
    }

    public WEAPONTYPE getWeaponType()
    {
        return m_weaponType;
    }


}
