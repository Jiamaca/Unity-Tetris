using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tetromino : MonoBehaviour 
{
    private List<GameObject> blocks;

    private IEnumerator coroutine;

    public void Start()
    {

    }

    public void Update()
    {

        Vector3 newPosition = transform.position;

        if (Input.GetKeyDown(KeyCode.A))
        {
            newPosition += Vector3.left;
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            newPosition -= Vector3.left;
        }

        if (CanMove(newPosition))
        {
            transform.position = newPosition;
        }

        SnapToGrid();
    }

    private bool CanMove(Vector3 newPosition)
    {
        foreach (Transform child in transform)
        {
            Vector3 newBlockPosition = child.localPosition + newPosition;

            if (newBlockPosition.x > 10 || newBlockPosition.x < 0)
            {
                return false;
            }
        }

        return true;
    }

    void SnapToGrid()
    {
        transform.position = new Vector3((int)transform.position.x, (int)transform.position.y, 0);
    }
}
