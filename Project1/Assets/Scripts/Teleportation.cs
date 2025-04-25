using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

public class Teleportation : MonoBehaviour
{
    [SerializeField] private Transform controller;
    [SerializeField] private Transform xrRig;
    [SerializeField] private InputActionReference buttonA;
    private LineRenderer line;
    private Vector3 hitPoint;
    private bool isValidTarget = false;

    private void Start()
    {
        buttonA.action.performed += OnTeleport;
    }

    private void OnDestroy()
    {
        buttonA.action.performed -= OnTeleport;
    }

    private void Update()
    {
        Ray ray = new Ray(controller.position, controller.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if(hit.transform.CompareTag("Floor"))
            {
                isValidTarget = true;
                hitPoint = hit.point;
            }
            else
            {
                isValidTarget = false;
            }
        }
    }

    private void OnTeleport(InputAction.CallbackContext ctx)
    {
        if (isValidTarget)
        {
            Vector3 targetPos = new Vector3(hitPoint.x, xrRig.position.y, hitPoint.z);
            xrRig.position = targetPos;
        }
    }
} 