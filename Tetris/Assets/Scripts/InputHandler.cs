using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputHandler : MonoBehaviour
{
    public bool touchEnabled = true;

    public static Vector2 inputAxis;
    public static int rotateDirection = 0;

    private static bool pressedRotateButon;


    // Update is called once per frame
    void Update()
    {
        pressedRotateButon = false;
        HandleInputAxis();
    }

    private void HandleInputAxis()
    {
        if (pressedRotateButon || !touchEnabled || IsPointerOverUIElement()) return;

        inputAxis.x = Input.GetAxisRaw("Horizontal");
        inputAxis.y = Input.GetAxisRaw("Vertical");

        if (Input.GetMouseButton(0))
        {
            Vector2 touchPosition = Input.mousePosition;

            if (touchPosition.y < Screen.height / 8)
            {
                inputAxis = Vector2.down;
            }
            else
            {
                if (touchPosition.x > Screen.width / 2)
                {
                    inputAxis.x = 1;
                }

                if (touchPosition.x < Screen.width / 2)
                {
                    inputAxis.x = -1;
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            inputAxis = Vector2.zero;
        }
    }

    public static void OnPressedRotateButton(int direction)
    {
        pressedRotateButon = true;
        rotateDirection = direction;
        inputAxis = Vector3.zero;
    }

    ///Returns 'true' if we touched or hovering on Unity UI element.
    public static bool IsPointerOverUIElement()
    {
        return IsPointerOverUIElement(GetEventSystemRaycastResults());
    }
    ///Returns 'true' if we touched or hovering on Unity UI element.
    public static bool IsPointerOverUIElement(List<RaycastResult> eventSystemRaysastResults)
    {
        for (int index = 0; index < eventSystemRaysastResults.Count; index++)
        {
            RaycastResult curRaysastResult = eventSystemRaysastResults[index];
            if (curRaysastResult.gameObject.layer == LayerMask.NameToLayer("UI"))
                return true;
        }
        return false;
    }
    ///Gets all event systen raycast results of current mouse or touch position.
    static List<RaycastResult> GetEventSystemRaycastResults()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> raysastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raysastResults);
        return raysastResults;
    }

}
