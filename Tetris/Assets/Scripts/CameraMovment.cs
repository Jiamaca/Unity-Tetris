using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovment : MonoBehaviour
{

    public float horizontalIntencity = 5;
    public float verticalIntencity = 5;

    // Update is called once per frame
    void Update()
    {
        float x = -InputHandler.inputAxis.y;
        float y = InputHandler.inputAxis.x;

        transform.rotation = Quaternion.Euler(x * horizontalIntencity, y * verticalIntencity, 0).normalized;
    }
}
