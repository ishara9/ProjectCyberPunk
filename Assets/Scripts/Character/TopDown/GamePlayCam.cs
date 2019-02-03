using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayCam : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject target;

    private Vector3 offset;

    void Start()
    {
        offset = target.transform.position - this.transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        this.transform.position = Vector3.Lerp(this.transform.position,  target.transform.position - offset,Time.deltaTime*6);
    }
}
