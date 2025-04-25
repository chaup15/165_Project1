using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

public class HighlightSelection : MonoBehaviour
{
    [SerializeField]private Material highlightMaterial;
    [SerializeField] XRBaseController controller;
    private Material originalMaterial;
    private string selectableTag = "Interactable";
    private Transform _selection;
    private bool isGripPressed = false;
    private float rayDistance = 7f;
    private float maxDistance = 0.5f;
    private float movementSpeed = 5f;


    // Update is called once per frame
    void Update()
    {
        HighlightSelected();
    }

    void HighlightSelected()
    {
        if(_selection != null){
            var renderer = _selection.GetComponent<Renderer>();
            renderer.material = originalMaterial;
            _selection = null;
        }

        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, rayDistance))
        {
            var hitObject = hit.transform;
            if(hitObject.CompareTag(selectableTag))
            {
                _selection = hitObject;
                var renderer = hitObject.GetComponent<Renderer>();
                if(renderer != null)
                {
                    originalMaterial = renderer.material;
                    renderer.material = highlightMaterial;
                }

                MoveObject(hit);
            }
        }
    }

    void MoveObject(RaycastHit hit)
    {
        if(controller != null)
        {
            isGripPressed = controller.selectInteractionState.active;
        }
        if (isGripPressed)
        {
            float distance = Vector3.Distance(controller.transform.position, hit.point);
            Vector3 direction = (controller.transform.position - hit.point).normalized;
            Rigidbody rb = hit.collider.GetComponent<Rigidbody>();
            
            if (distance > maxDistance)
            {
                hit.collider.transform.position += direction * movementSpeed * Time.deltaTime;

                if (rb != null)
                {
                    rb.velocity = Vector3.zero;  
                    rb.angularVelocity = Vector3.zero;
                }
            }
            else
            {
                hit.collider.transform.position = controller.transform.position - direction * maxDistance;

                if (rb != null)
                {
                    rb.velocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                }
            }

            // Debug.Log(_selection);
            // Debug.Log(distance);
        }
    }
}
