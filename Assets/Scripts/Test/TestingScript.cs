using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingScript : MonoBehaviour {

    // Use this for initialization
    RagdollController ragdollController;
    Shooter shooter;
	void Start ()
    {
        ragdollController = this.GetComponent<RagdollController>();
        shooter = this.GetComponent<Shooter>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            /*
            ragdollController.setRagdollState(true);
            ragdollController.AddImpulseToRagdoll(RagdollPart.TYPE.Chest, -Vector3.forward * 1000);
            shooter.setPlayerActiveState(false);
            */
        }
	}
}
