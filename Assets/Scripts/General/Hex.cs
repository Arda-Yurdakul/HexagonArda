using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hex : MonoBehaviour
{
    public int xIndex { get; set; }
    public int yIndex { get; set; }

    public HexType hexType;
    public enum HexType
    {
        Red,
        Green,
        Yellow,
        Blue,
        Purple,
        Orange
    }

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
}
