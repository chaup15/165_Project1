using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class objectSpawning : MonoBehaviour
{
    // Start is called before the first frame update

    public InputActionReference menuButton;
    public GameObject prefab;
    public Transform canvas;
    public float angBetweenpart = 10;
    public Transform hand;
    public Transform cam;
    public GameObject[] spawnablePrefabs;
    
    
    public UnityEvent<int> EventsSelected;

    private List<GameObject> menuItems = new List<GameObject>();
    private int curselectedpart = -1;
    private GameObject currentPreview;
    private int part;

    void Start(){
        part = spawnablePrefabs.Length;

        menuButton.action.started += ctx => Spawnbar();
        menuButton.action.performed += ctx => selectedpart();
        menuButton.action.canceled += ctx => HideAndTrigger();
        menuButton.action.Enable();

        canvas.gameObject.SetActive(false);
    }

    void OnDestroy(){
        menuButton.action.started -= ctx => Spawnbar();
        menuButton.action.performed -= ctx => selectedpart();
        menuButton.action.canceled -= ctx => HideAndTrigger(); 
        //menuButton.action.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        canvas.position = cam.position + cam.forward * 0.75f;
        canvas.rotation = cam.rotation;

        selectedpart();
    }

    public void HideAndTrigger(){
        Debug.Log("Menu Closed");
        EventsSelected.Invoke(curselectedpart);
        canvas.gameObject.SetActive(false);

        if(currentPreview) Destroy(currentPreview);

        Transform selected = menuItems[curselectedpart].transform;
        Vector3 spawnPos = canvas.position + Vector3.up * 0.1f;
        GameObject spawnedObj = Instantiate(spawnablePrefabs[curselectedpart],spawnPos,selected.rotation);

        spawnedObj.tag = "Interactable";
        spawnedObj.layer = LayerMask.NameToLayer("Interactable");

        MeshCollider collider = spawnedObj.AddComponent<MeshCollider>();
        collider.convex = true;

        Rigidbody rb = spawnedObj.GetComponent<Rigidbody>();
        if (rb == null){
            rb = spawnedObj.AddComponent<Rigidbody>();
        }
        rb.isKinematic = false;
    }

    public void selectedpart(){
        Debug.Log("Menu Opened");
        Vector3 centertoHand = hand.position - canvas.position;
        Vector3 centerToHandProjected = Vector3.ProjectOnPlane(centertoHand, canvas.forward);

        float ang = Vector3.SignedAngle(canvas.up, centerToHandProjected, -canvas.forward);
        if(ang < 0){
            ang += 360;
        }

        curselectedpart = (int) ang * part / 360;

        for(int i = 0; i < menuItems.Count; i++){
            if (i == curselectedpart){
                menuItems[i].GetComponent<Image>().color = Color.yellow;
                menuItems[i].transform.localScale = 1.1f * Vector3.one;
            } else {  
                menuItems[i].GetComponent<Image>().color = Color.white;
                menuItems[i].transform.localScale = 1.1f * Vector3.one;
            }
        }

        if(currentPreview){Destroy(currentPreview);}

        Transform target = menuItems[curselectedpart].transform;
        Vector3 prewPos = canvas.position;
        currentPreview = Instantiate(spawnablePrefabs[curselectedpart],prewPos,target.rotation);
    }

    public void Spawnbar(){
        Debug.Log("Menu Started");
        
        canvas.gameObject.SetActive(false);
        canvas.gameObject.SetActive(true);

        canvas.position = cam.position + cam.forward * 0.75f;
        canvas.rotation = cam.rotation;

        foreach (var i in menuItems){
           Destroy(i);
        }

        menuItems.Clear();

        for (int i = 0; i < part; i++){
            float ang = - i * 360 / part - angBetweenpart / 2;
            Vector3 rad_ang = new Vector3(0,0,ang);

            GameObject spawnedRad = Instantiate(prefab, canvas);
            spawnedRad.transform.position = canvas.position;
            spawnedRad.transform.localEulerAngles = rad_ang;

            spawnedRad.GetComponent<Image>().fillAmount =  (1 / (float) part) - (angBetweenpart / 360);

            menuItems.Add(spawnedRad);
        }
    }
}
