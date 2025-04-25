using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

public class Indicator : MonoBehaviour
{
    // [SerializeField] Transform controller;
    // private float maxDistance = 10f;

    private LineRenderer line;
    private Vector3 hitPoint;

    private void Start()
    {
        line = GetComponent<LineRenderer>();
        if (line == null)
        {
            line = gameObject.AddComponent<LineRenderer>();
        }

        line.startWidth = 0.002f;
        line.endWidth = 0.002f;
        line.positionCount = 2;
        line.material = new Material(Shader.Find("Sprites/Default"));
    }

    private void Update()
    {
        line.enabled = true;
        line.SetPosition(0, transform.position);
        line.SetPosition(1, transform.position + transform.forward);

        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 10f))
        {
            if(hit.transform.CompareTag("Floor"))
            { 
                line.startColor = Color.green;
                line.endColor = Color.green;
            }
            else if(hit.transform.CompareTag("Interactable"))
            {
                line.startColor = Color.red;
                line.endColor = Color.red;
            }
            else
            {
                line.startColor = Color.white;
                line.endColor = Color.white;
            }
        }
        else
        {
            line.startColor = Color.white;
            line.endColor = Color.white;
        }
    }
}
