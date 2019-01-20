using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;

public class MovableCharacter : MonoBehaviour {

    // Use this for initialization
    Animator m_animator;
    TargetFinder m_targetFinder;
    private AimIK m_aimIk;
    public GameObject targetObject;
    public LayerMask hitLayers;

    private bool m_shooting = false;

    void Start ()
    {
        m_animator = this.GetComponent<Animator>();
        m_aimIk = this.GetComponent<AimIK>();
        m_targetFinder = this.GetComponentInChildren<TargetFinder>();
        //targetObject = new GameObject();
        m_aimIk.solver.target = targetObject.transform;
    }
	
	// Update is called once per frame
	void FixedUpdate ()
    {



        targetObject.transform.position = getTargetPoint() ;
        trunPlayer();
        moveCharacterMobile(false);

        // Mobile
        //checkShooting();
        //aimAtTArget();
    }

    Vector3 getTargetPoint()
    {
        //return Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));
        Vector3 mouse = Input.mousePosition;
        Ray castPoint = Camera.main.ScreenPointToRay(mouse);
        RaycastHit hit;
        if (Physics.Raycast(castPoint, out hit, Mathf.Infinity, hitLayers))
        {
            return hit.point;
        }
        return Vector3.zero;
    }

    void checkShooting()
    {
       if( SimpleInput.GetButton("Jump") || Input.GetKey(KeyCode.Space))
        {
            m_shooting = true;
            m_animator.SetBool("shooting", true);
        }
       else
        {
            m_shooting = false;
            m_animator.SetBool("shooting", false);
        }
    }

    void moveCharacterMobile(bool mobile)
    {
        //float Horizontal = Input.GetAxis("Horizontal");
        //float Vertical = Input.GetAxis("Vertical");
        
        float Horizontal = SimpleInput.GetAxis("Horizontal");
        float Vertical = SimpleInput.GetAxis("Vertical");

        Vector3 moveDirection = new Vector3(Horizontal, 0, Vertical);

        if(mobile)
        {
            moveDirection = this.transform.InverseTransformDirection(moveDirection);
        }


        if (m_shooting)
        {
            m_animator.SetFloat("forward", moveDirection.x);
            m_animator.SetFloat("side", -moveDirection.z);
        }
        else
        {
            m_animator.SetFloat("forward",  Mathf.Abs( Vertical )+ Mathf.Abs(Horizontal));
            this.transform.LookAt(new Vector3(-Vertical, 0, Horizontal) + this.transform.position, Vector3.up);
        }
    }

    void trunPlayer()
    {
        Vector3 aimPoint = getTargetPoint();
        aimPoint.y = 0;
        Vector3 direction = aimPoint - this.transform.position;
        float angle = Vector3.Angle(this.transform.forward, direction);

        if(Mathf.Abs(angle) >30)
        {
            //this.transform.LookAt(aimPoint, Vector3.up);
            this.transform.rotation = Quaternion.Lerp(this.transform.rotation,Quaternion.LookRotation(aimPoint,Vector3.up), 5f*Time.deltaTime);
            m_animator.SetFloat("turn", angle*2);
        }
        else
        {
            m_animator.SetFloat("turn", 0, 1, Time.deltaTime);
        }
    }

    void aimAtTArget()
    {
       Vector3 target =  m_targetFinder.getCurrentTarget(this.transform.position);
       // targetObject.transform.position = this.transform.position + this.transform.forward * 5;

        if (target != Vector3.zero && m_shooting)
        {
            targetObject.transform.position = Vector3.Lerp(targetObject.transform.position,target,Time.deltaTime*20);
        }
        else
        {
            targetObject.transform.position = Vector3.Lerp(targetObject.transform.position, this.transform.position + this.transform.forward * 50, Time.deltaTime * 3); ;
        }
    }
}
