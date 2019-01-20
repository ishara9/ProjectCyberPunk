using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class MobileInput : MonoBehaviour
{
    private Shooter m_shooter;
    private SwipeComponent m_swipeComponent;

    public void Awake()
    {
        m_shooter = this.GetComponent<Shooter>();
        m_swipeComponent = this.GetComponent<SwipeComponent>();
        m_swipeComponent.setGetTapObjectFunction(m_shooter.Shoot);
    }

    public void FixedUpdate()
    {
        m_shooter.ShooterLateUpdate();

        if(m_swipeComponent.SwipeUp)
        {
            m_shooter.JumpUp();
        }

        if(m_swipeComponent.SwipeLeft)
        {
            m_shooter.moveSide(Shooter.MOVINGSTATE.LEFT);
        }

        if(m_swipeComponent.SwipeRight)
        {
            m_shooter.moveSide(Shooter.MOVINGSTATE.RIGHT);
        }
    }

}
