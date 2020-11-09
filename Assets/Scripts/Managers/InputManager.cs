using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    Vector3 clickPos;
    Vector3 worldPosition;

    private Joint jointSelected;
    private Board board;
    GameManager gameManager;

    private Vector2 firstTouch;
    public Vector2 lastTouch;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        board = FindObjectOfType<Board>();
        jointSelected = null;
    }

    // Update is called once per frame
    void Update()
    {
        if(gameManager.gameState == GameState.Running)
        {
            GetInput();
        }
    }

    //Available controls for both UnityEditor and Mobile Platforms
    private void GetInput()
    {

#if UNITY_EDITOR

        if (Input.GetKeyDown(KeyCode.D) && jointSelected != null)
        {
            StartCoroutine(board.RotateClockwiseAndClearRoutine());
        }
        else if (Input.GetKeyDown(KeyCode.A) && jointSelected != null)
        {
            StartCoroutine(board.RotateCounterAndClearRoutine());
        }

        if (Input.GetMouseButtonDown(0))
        {
            clickPos = Input.mousePosition;
            worldPosition = Camera.main.ScreenToWorldPoint(clickPos);
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

#elif UNITY_ANDROID
     if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                Vector3 touchPos = touch.position;
                touchPos.z = 10;
                firstTouch = Camera.main.ScreenToWorldPoint(touchPos);
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                Vector3 touchPos = touch.position;
                touchPos.z = 10;
                lastTouch = Camera.main.ScreenToWorldPoint(touchPos);
                float swipeDistance = Vector2.Distance(lastTouch, firstTouch);
                if (swipeDistance < 0.5f && IsWithinBounds(firstTouch))
                {
                    JointSelect();
                }
                else
                {
                    JointRotate();
                }
            }
        }
        
#endif

    }

    //Try to find the joint that's nearest to the player's touch 
    private void JointSelect()
    {
        try
        {
            jointSelected = FindObjectOfType<Board>().FindAndSelectNearestJoint(lastTouch);
        }
        catch (Exception e)
        {
            Debug.LogWarning("Could not find joint!");
        }
        
    }

    //According to the swipe position and direction, rotate the pivot point clockwise or counter-clockwise
    private void JointRotate()
    {
        Vector2 jointPos = jointSelected.transform.position;
        Vector2 firstTouchPos = firstTouch - jointPos;
        Vector2 lastTouchPos = lastTouch - jointPos;
        if(Vector2.SignedAngle(firstTouchPos, lastTouchPos) < 0f)
        {
            StartCoroutine(board.RotateClockwiseAndClearRoutine());
        }
        else
        {
            StartCoroutine(board.RotateCounterAndClearRoutine());
        }
    }

   
    //Checks if the recorded touch was within acceptable bounds with regards to the generated board
    private bool IsWithinBounds(Vector3 pos)
    {
        Vector2 worldBounds = board.GetBoardBounds();
        return (-1 < pos.x && pos.x < worldBounds.x * 1.5f && -1 < pos.y && pos.y < worldBounds.y * 1.5f);
    }

    
}
