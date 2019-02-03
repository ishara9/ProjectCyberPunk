﻿using UnityEngine;
using RootMotion.FinalIK;

public class ProjectileBasic : MonoBehaviour
{
    // Start is called before the first frame update
    public float speed = 0;
    private float moveDirection;
    private Transform target;
    public float DistanceTravelled = 0;
    private string shooterName ="test";
    private bool hit = false;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        this.transform.Translate(Vector3.forward * speed);
        DistanceTravelled += Time.deltaTime * speed;

        if (DistanceTravelled > 1)
        {
            Debug.Log("destroy");
            Destroy(this.gameObject);
        }
    }

    public void setTarget(Transform target)
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        MovingAgent movingAgnet = other.transform.GetComponentInParent<MovingAgent>();
        
        if (movingAgnet != null  && !hit)
        {
            if( !shooterName.Equals(movingAgnet.name))
            {
                hit = true;
                movingAgnet.getDamageSystem().reactOnHit(other, (this.transform.forward) * 5f, other.transform.position);

                DamageSystem damageSystem = movingAgnet.getDamageSystem();
                damageSystem.DamageByAmount(1);

                speed = 0;
                Destroy(this.gameObject);

                if (!damageSystem.IsFunctional())
                {
                    movingAgnet.DestroyCharacter();
                    Rigidbody rb = other.transform.GetComponent<Rigidbody>();

                    if (rb != null)
                    { 
                        rb.isKinematic = false;
                        rb.AddForce((this.transform.forward) * 200, ForceMode.Impulse);
                    }
                }
            }

        }

    }
    public void setShooterName(string name)
    {
        this.shooterName = name;
    }
}
