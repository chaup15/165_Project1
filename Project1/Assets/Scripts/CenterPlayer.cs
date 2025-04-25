using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class CenterPlayer : MonoBehaviour
{
    [SerializeField]private Transform target;
    [SerializeField]private Transform xrRig;

    // Start is called before the first frame update
    void Start()
    {
        Vector3 center = target.localPosition;
        xrRig.localPosition = new Vector3(center.x, xrRig.localPosition.y, center.z + 2f);
        
        xrRig.Rotate(0f,180f,0f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
