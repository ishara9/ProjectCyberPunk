using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public GameObject targetPoint;
    public GameObject target;

    public bool isAimed;
    private LineRenderer m_line;

    public void Awake()
    {
        m_line = this.GetComponent<LineRenderer>();
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

}
