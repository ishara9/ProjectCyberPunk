using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponProp : MonoBehaviour
{
    public bool PropEnabled;

    public void setVisible(bool state)
    {
        this.gameObject.SetActive(state);
    }
}
