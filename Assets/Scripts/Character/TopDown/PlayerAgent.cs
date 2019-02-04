using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAgent :AgentController
{
    private bool m_enabled;
    protected MovingAgent m_movingAgent;

    public LayerMask enemyHitLayerMask;
    public LayerMask floorHitLayerMask;

    public PlayerAgent(LayerMask enemyHitLayerMask, LayerMask floorHitLayerMask)
    {
        this.enemyHitLayerMask = enemyHitLayerMask;
        this.floorHitLayerMask = floorHitLayerMask;
    }

    #region Updates
    public void controllerUpdate()
    {
        // Setting Character Aiming.
        if (Input.GetMouseButton(1))
        {
            m_movingAgent.AimWeapon();
        }
        else
        {
            m_movingAgent.StopAiming();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            m_movingAgent.toggleCurrentWeapon();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            m_movingAgent.getAnimationSystem().toggleCrouched();
        }

        m_movingAgent.moveCharacter(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")));

        UpdateShooting();

        UpdateTargetPoint();
    }

    private void UpdateShooting()
    {
        if (Input.GetMouseButtonDown(0) && Input.GetMouseButton(1))
        {
            m_movingAgent.FireWeapon();
        }
    }

    private void UpdateTargetPoint()
    {
        Vector3 mouse = Input.mousePosition;
        Ray castPoint = Camera.main.ScreenPointToRay(mouse);
        RaycastHit hit;
        Vector3 targetPosition = Vector3.zero;

        if (Physics.Raycast(castPoint, out hit, Mathf.Infinity, floorHitLayerMask))
        {
            // targetPosition = setTargetHeight(hit.point, hit.transform.tag);
            targetPosition = hit.point;
        }

        if (Physics.Raycast(castPoint, out hit, Mathf.Infinity, enemyHitLayerMask))
        {
            //targetPosition = setTargetHeight(hit.point, hit.transform.tag);
            targetPosition = hit.point;
        }

        m_movingAgent.setTargetPoint(targetPosition);
    }
    #endregion

    #region getters and setters

    public void setEnabled (bool enabled)
    {
        m_enabled = enabled;
    }

    public bool getEnabled()
    {
        return m_enabled;
    }

    // Set target Height depending on the target type.
    private Vector3 setTargetHeight(Vector3 position, string tag)
    {
        switch (tag)
        {
            case "Floor":
                return new Vector3(position.x, position.y, position.z);
            case "Enemy":
                return position;
        }
        return Vector3.zero;
    }

    public void setMovableAgent(MovingAgent agent)
    {
        m_movingAgent = agent;
    }
    #endregion
}
