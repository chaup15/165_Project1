using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

public class SelectionManipulation : MonoBehaviour
{   
    [SerializeField]    
    XRBaseController leftController;

    [SerializeField]    
    XRBaseController rightController;
    [SerializeField]private Material highlightMaterial;

    private Material leftOriginalMaterial;
    private Material rightOriginalMaterial;
    private Transform leftSelection;
    private Transform rightSelection;
    private GameObject[] objects;
    private Transform target;
    private Vector3 initialScale;
    private Vector3 initialVector;
    private Quaternion initialRotation;
    private float initialDist;

    private bool leftGripPressed;
    private bool rightGripPressed;
    private bool leftTriggerPressed;
    private bool rightTriggerPressed;
    
    private bool isScaling = false;
    private bool isRotating = false;
    private string selectableTag = "Interactable";
    private float rayDistance = 20f;
    private float maxDistance = 0.5f;
    private float movementSpeed = 5f;
    private float rotateSpeed = 10f;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ScaleObject();
        RotateObject();
        HighlightSelected();
    }

    void ScaleObject()
    {   
        leftGripPressed = GetGripStatus(leftController);
        rightGripPressed = GetGripStatus(rightController);

        RaycastHit leftRay, rightRay;
        bool leftHit = Physics.Raycast(leftController.transform.position, leftController.transform.forward, out leftRay, rayDistance);
        bool rightHit = Physics.Raycast(rightController.transform.position, rightController.transform.forward, out rightRay, rayDistance);

        if(leftGripPressed && rightGripPressed && leftHit && rightHit)
        {
            var leftObject = leftRay.collider.gameObject;
            var rightObject = rightRay.collider.gameObject;
            if(leftObject == rightObject && leftRay.transform.CompareTag(selectableTag))
            {
                if(!isScaling)
                {
                    target = leftRay.transform;
                    initialScale = target.localScale;
                    initialDist = Vector3.Distance(leftRay.point, rightRay.point);
                    isScaling = true;
                }
                else
                {
                    float currDist = Vector3.Distance(leftRay.point, rightRay.point);
                    float scaleFactor = currDist / initialDist;
                    target.localScale = initialScale * scaleFactor;
                }
                
            }
            else
            {
                isScaling = false;
            }   
        }
        else
        {
            isScaling = false;
        }
        
            
    }    

    void RotateObject()
    {
        leftGripPressed = GetGripStatus(leftController);
        rightGripPressed = GetGripStatus(rightController);
        leftTriggerPressed = GetTriggerStatus(leftController);
        rightTriggerPressed = GetTriggerStatus(rightController);

        RaycastHit leftRay, rightRay;
        bool leftHit = Physics.Raycast(leftController.transform.position, leftController.transform.forward, out leftRay, rayDistance);
        bool rightHit = Physics.Raycast(rightController.transform.position, rightController.transform.forward, out rightRay, rayDistance);

        //left hand grab and rotate
        if(leftGripPressed && rightTriggerPressed && leftHit)
        {
            if(leftRay.transform.CompareTag(selectableTag))
            {
                Rotate(leftRay.transform, rightRay, leftRay);
            }
            else
            {
                isRotating = false;
            }
        }
        //right hand grab and rotate
        else if(rightGripPressed && leftTriggerPressed && rightHit)
        {
            if(rightRay.transform.CompareTag(selectableTag))
            {
                Rotate(rightRay.transform, rightRay, leftRay);
            }
            else
            {
                isRotating = false;
            }
        }
        else
        {
            isRotating = false;
        }
    }

    void Rotate(Transform hitObject, RaycastHit right, RaycastHit left)
    {
        if(!isRotating)
        {
            target = hitObject;
            initialVector = right.point - left.point;
            initialRotation = target.rotation;
            isRotating = true;
        }
        else
        {
            Vector3 currVector = right.point - left.point;
            if(initialVector != Vector3.zero && currVector != Vector3.zero)
            {
                Quaternion currRotDelta = Quaternion.FromToRotation(initialVector, currVector);
                Quaternion targetRotation = currRotDelta * initialRotation;
                target.rotation = Quaternion.Slerp(target.rotation, targetRotation, Time.deltaTime * rotateSpeed);
            }
        }
    }

    void HighlightSelected()
    {
        if(leftSelection != null){
            var renderer = leftSelection.GetComponent<Renderer>();
            renderer.material = leftOriginalMaterial;
            leftSelection = null;
        }
        if(rightSelection != null){
            var renderer = rightSelection.GetComponent<Renderer>();
            renderer.material = rightOriginalMaterial;
            rightSelection = null;
        }

        RaycastHit leftRay, rightRay;
        bool leftHit = Physics.Raycast(leftController.transform.position, leftController.transform.forward, out leftRay, rayDistance, LayerMask.GetMask("Interactable"));
        bool rightHit = Physics.Raycast(rightController.transform.position, rightController.transform.forward, out rightRay, rayDistance, LayerMask.GetMask("Interactable"));

        if (leftHit)
        {
            var hitObject = leftRay.transform;
            if(hitObject.CompareTag(selectableTag))
            {
                leftSelection = hitObject;
                var renderer = hitObject.GetComponent<Renderer>();
                if(renderer != null)
                {
                    leftOriginalMaterial = renderer.material;
                    renderer.material = highlightMaterial;
                }

                MoveObject(leftRay, leftController);
            }
        }
        else if(rightHit)
        {
            var hitObject = rightRay.transform;
            if(hitObject.CompareTag(selectableTag))
            {    
                rightSelection = hitObject;
                var renderer = hitObject.GetComponent<Renderer>();
                if(renderer != null)
                {
                    rightOriginalMaterial = renderer.material;
                    renderer.material = highlightMaterial;
                }

                MoveObject(rightRay, rightController);
            }
        }
    }

    void MoveObject(RaycastHit hit, XRBaseController controller)
    {
        if (GetGripStatus(controller))
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
        }
    }

    bool GetGripStatus(XRBaseController controller)
    {
        if(controller != null)
        {
            return controller.selectInteractionState.active;
        }
        return false;
    }
    bool GetTriggerStatus(XRBaseController controller)
    {
        if(controller != null)
        {
            return controller.uiPressInteractionState.active;
        }
        return false;
    }
}
