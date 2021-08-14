using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tetromino : MonoBehaviour 
{
    public float fallTime = 0.8f;

    private float previousTime = 0;
    private bool freezed = false;

    public static int tileRows = 21;
    public static int tileCollumns = 10;

    private static Transform[,] tilesMatrix = new Transform[tileCollumns, tileRows];

    public void Update()
    {
        if (freezed) return;

        Rotate();

        Move();

        if (Time.time - previousTime > (Input.GetKey(KeyCode.S) ? fallTime / 10 : fallTime))
        {
            Vector3 newPosition = transform.position + Vector3.down;

            if (CanMove(newPosition))
            {
                transform.position = newPosition;
            } else
            {
                freezed = true;

                AddChildsToMatrix();

                CheckLine();

                GameManager.Main.SpawnTetromino();
            }

            previousTime = Time.time;
        }

        SnapToGrid();
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

            if (tilesMatrix[newPositionX, newPositionY] != null)
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

            if (tilesMatrix[newPositionX, newPositionY] != null)
            {
                return false;
            }
        }

        return true;
    }

    private void AddChildsToMatrix()
    {
        foreach (Transform child in transform)
        {
            int positionX = Mathf.RoundToInt(child.position.x);
            int positionY = Mathf.RoundToInt(child.position.y);

            tilesMatrix[positionX, positionY] = child;
        }
    }

    private void CheckLine()
    {
        for (int i = 0; i < tileRows; i++)
        {
            bool isComplete = true;
            for (int j = 0; j < tileCollumns; j++)
            {
                if (tilesMatrix[j, i] == null)
                {
                    isComplete = false;
                    break;
                }
            }
            if (isComplete)
            {
                ClearLine(i);
                RowDown(i);
            }
        }
    }

    private void ClearLine(int line)
    {
        for (int col = 0; col < tileCollumns; col++)
        {
            GameObject go = tilesMatrix[col, line].gameObject;
            Destroy(go);
        }
    }

    private void RowDown(int line)
    {
        for (int i = line + 1; i < tileRows - 1; i++)
        {
            for (int j = 0; j < tileCollumns; j++)
            {
                Debug.Log("i = " + i + " j = " + j);

                Transform t = tilesMatrix[j, i];
                if (t != null)
                {
                    tilesMatrix[j, i+1] = t;
                    tilesMatrix[j, i] = null;
                    tilesMatrix[j, i+1].transform.position += Vector3.down;
                }
            }
        }
    }

    void SnapToGrid()
    {
        int newPositionX = Mathf.RoundToInt(transform.position.x);
        int newPositionY = Mathf.RoundToInt(transform.position.y);

        transform.position = new Vector3(newPositionX, newPositionY, 0);
    }
}
