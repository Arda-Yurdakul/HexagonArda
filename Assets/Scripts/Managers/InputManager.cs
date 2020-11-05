using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    Vector3 mousePos;
    Vector3 worldPosition;

    private Joint jointSelected;
        
    // Start is called before the first frame update
    void Start()
    {
        jointSelected = null;
    }

    // Update is called once per frame
    void Update()
    {
        JointSelection();
    }

    private void JointSelection()
    {
        if (Input.GetMouseButtonDown(0))
        {
            mousePos = Input.mousePosition;
            worldPosition = Camera.main.ScreenToWorldPoint(mousePos);
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
