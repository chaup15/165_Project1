using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gazeHighlighter : MonoBehaviour
{

    public float gazeDistance = 100f;
    public Color highlightColor = Color.red;

    private GameObject currentTarget;
    private Color originalColor;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Update()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, gazeDistance))
        {
            GameObject hitObject = hit.collider.gameObject;

            if (hitObject.CompareTag("Interactable"))
            {
                if (hitObject != currentTarget)
                {
                    ResetPreviousTarget();

                    currentTarget = hitObject;
                    Renderer rend = currentTarget.GetComponent<Renderer>();
                    if (rend != null)
                    {
                        originalColor = rend.material.color;
                        rend.material.color = highlightColor;
                    }
                }
            }
        }
        else
        {
            ResetPreviousTarget();
        }
    }

    void ResetPreviousTarget()
    {
        if (currentTarget != null)
        {
            Renderer rend = currentTarget.GetComponent<Renderer>();
            if (rend != null)
            {
                rend.material.color = originalColor;
            }
            currentTarget = null;
        }
    }
}
