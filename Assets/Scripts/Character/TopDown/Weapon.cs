using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public GameObject targetPoint;
    public GameObject target;

    public bool isAimed;
    private LineRenderer m_line;
    private Rigidbody m_rigidbody;
    private BoxCollider m_collider;

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
        m_line.enabled = aimed;
    }

    public void updateWeapon()
    {
        if(isAimed)
        {
            m_line.SetPosition(0, targetPoint.transform.position);
            m_line.SetPosition(1, target.transform.position);
        }
    }

    public void disarmWeapon()
    {
        this.transform.parent = null;
        m_rigidbody.isKinematic = false;
        m_rigidbody.useGravity = true;
        m_collider.isTrigger = false;
    }

}
