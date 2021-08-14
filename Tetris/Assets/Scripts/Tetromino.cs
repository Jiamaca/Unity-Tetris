using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tetromino : MonoBehaviour 
{
    public float tickTime = 1.0f;

    private List<GameObject> blocks;

    private IEnumerator coroutine;

    public void Start()
    {
        coroutine = GravityRoutine(tickTime);
        StartCoroutine(coroutine);
    }

    public void Update()
    {

        Rotate();

        Move();

        SnapToGrid();
    }

    private IEnumerator GravityRoutine(float _tickTime)
    {
        while (true)
        {
            yield return new WaitForSeconds(_tickTime);

            Vector3 newPosition = transform.position + Vector3.down;

            if (CanMove(newPosition))
            {
                transform.position = newPosition;
            }

        }
    }

    private void Move()
    {
        Vector3 newPosition = transform.position;

        if (Input.GetKeyDown(KeyCode.A))
        {
            newPosition += Vector3.left;
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            newPosition += Vector3.right;
        }

        if (CanMove(newPosition))
        {
            transform.position = newPosition;
        }
    }

    private bool CanMove(Vector3 newPosition)
    {
        foreach (Transform child in transform)
        {
            Vector3 newBlockPosition = child.localPosition + newPosition;

            int newPositionX = Mathf.RoundToInt(newBlockPosition.x);
            int newPositionY = Mathf.RoundToInt(newBlockPosition.y);

            if (newPositionX > 9 || newPositionX < 0 || newPositionY < 0)
            {
                return false;
            }
        }

        return true;
    }

    private void Rotate()
    {
        Quaternion newRotation = transform.rotation;

        if (Input.GetKeyDown(KeyCode.Q))
        {
            newRotation = Quaternion.Euler(0, 0, 90);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            newRotation = Quaternion.Euler(0, 0, -90);
        }


        if (CanRotate(newRotation))
        {
            foreach (Transform child in transform)
            {
                Vector3 newBlockPosition = newRotation * child.localPosition;
                child.transform.localPosition = newBlockPosition;
            }
        }
    }

    private bool CanRotate(Quaternion newRotation)
    {
        foreach (Transform child in transform)
        {
            Vector3 newBlockPosition = (newRotation * child.localPosition) + transform.position;

            int newPositionX = Mathf.RoundToInt(newBlockPosition.x);
            int newPositionY = Mathf.RoundToInt(newBlockPosition.y);

            if (newPositionX > 9 || newPositionX < 0 || newPositionY < 0)
            {
                return false;
            }
        }

        return true;
    }

    void SnapToGrid()
    {
        int newPositionX = Mathf.RoundToInt(transform.position.x);
        int newPositionY = Mathf.RoundToInt(transform.position.y);

        transform.position = new Vector3(newPositionX, newPositionY, 0);
    }
}
