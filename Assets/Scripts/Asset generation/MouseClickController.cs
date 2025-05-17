using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static Unity.Collections.AllocatorManager;

public class MouseClickController : MonoBehaviour
{
    public Vector3 clickPosition;

    [SerializeField] private Vector3 click; //we save the position
    [SerializeField] private UnityEvent<Vector3> OnClick = new UnityEvent<Vector3>(); // this is so other scripts can use the position



    private Ray lastMouseRay;
    private bool hasHit;

    void Update() {

        if (Input.GetMouseButtonDown(0))
        {
            //this creates the ray on the mouse position
            lastMouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);

            //this condition triggers if we click on something
            if (Physics.Raycast(lastMouseRay, out RaycastHit hitInfo))
            {

                click = hitInfo.point;
                hasHit = true;
                Debug.Log($"Click Position: {click}");
                OnClick.Invoke(click);
                
            }
        }

        //If the last condition is true then we draw the line
        if (hasHit)
        {
            Debug.DrawRay(lastMouseRay.origin, lastMouseRay.direction * 100f, Color.green);
        }
    } 

}
