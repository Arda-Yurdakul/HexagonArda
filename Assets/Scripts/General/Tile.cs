using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[SelectionBase]
public class Tile : MonoBehaviour
{
    public int xIndex { get; set; }
    public int yIndex { get; set; }
    private Board m_board;

    public void Init(int x, int y, Board board)
    {
        xIndex = x;
        yIndex = y;
        m_board = board;
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseDown()
    {
        print("Tile: " + xIndex + "," + yIndex);
    }
}
