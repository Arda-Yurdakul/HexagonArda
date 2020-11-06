using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    Vector3 mousePos;
    Vector3 worldPosition;

    private Joint jointSelected;
    private Board board;
        
    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
        jointSelected = null;
    }

    // Update is called once per frame
    void Update()
    {
        JointSelection();
        RotateSelectedJoint();
        CollapseAll();
    }

    private void CollapseAll()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            board.CollapseAllColumns();
        }
    }

    private void RotateSelectedJoint()
    {
        if(Input.GetKeyDown(KeyCode.D) && jointSelected != null)
        {
            StartCoroutine(board.RotateClockwiseAndClearRoutine());
        }
        else if(Input.GetKeyDown(KeyCode.A) && jointSelected != null)
        {
            StartCoroutine(board.RotateCounterAndClearRoutine());
        }
    }

    private void JointSelection()
    {
        if (Input.GetMouseButtonDown(0))
        {
            mousePos = Input.mousePosition;
            worldPosition = Camera.main.ScreenToWorldPoint(mousePos);
            if (IsWithinBounds(worldPosition))
            {
                try
                {
                    jointSelected = FindObjectOfType<Board>().FindAndSelectNearestJoint(worldPosition);
                }
                catch (Exception e)
                {
                    Debug.LogWarning("Could not find joint!");
                }
            }    
        }
    }


    private bool IsWithinBounds(Vector3 pos)
    {
        Vector2 worldBounds = board.GetBoardBounds();
        return (-1 < pos.x && pos.x < worldBounds.x * 1.5f && -1 < pos.y && pos.y < worldBounds.y * 1.5f);
    }

    
}
