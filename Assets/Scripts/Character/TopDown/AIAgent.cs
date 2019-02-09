using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAgent : AgentController
{
    private bool m_enabled;
    protected MovingAgent m_movingAgent;

    public LayerMask enemyHitLayerMask;
    public LayerMask floorHitLayerMask;

    // temp
    private MovingAgent player;
    private float moveCounter;
    private float shootingCounter =0;
    private Vector3 moveDirection;
    private int shotCount = 10;

    public AIAgent()
    {
        
       GameObject[] playerTaggedObjects = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject obj in playerTaggedObjects)
        {
            player = obj.GetComponent<MovingAgent>();

            if(player != null)
            {
                break;
            }
        }
    }

    #region Updates
    public void controllerUpdate()
    {
        if(!m_movingAgent.isEquiped())
        {
            m_movingAgent.togglepSecondaryWeapon();
        }
        m_movingAgent.AimWeapon();

        if (moveCounter > 1)
        {
            moveDirection = Random.insideUnitSphere;
            moveDirection.y = 0;
            moveCounter = 0;
        }

        Vector3 targetPostion = player.transform.position;
        targetPostion.y = 1.5f;
        m_movingAgent.setTargetPoint(targetPostion);

        if (shootingCounter > 1)
        {
            targetPostion = player.transform.position;
            targetPostion = new Vector3(targetPostion.x, 1.2f + targetPostion.y, targetPostion.z);
            m_movingAgent.setTargetPoint(targetPostion);

            m_movingAgent.FireWeapon();
            shootingCounter = 0;
            shotCount--;
            
            if(shotCount < 0)
            {
                shootingCounter = 10;
                shootingCounter = -3;
            }
        }

        shootingCounter += Time.deltaTime * 2;

        moveCounter += Time.deltaTime * 2;

       // m_movingAgent.moveCharacter(moveDirection.normalized);


    }
    #endregion

    #region getters and setters

    public void setEnabled(bool enabled)
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
                return new Vector3(position.x, 1.25f, position.z);
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
