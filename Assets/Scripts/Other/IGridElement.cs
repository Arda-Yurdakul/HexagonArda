using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Simple Interface for grid structure essentials
public interface IGridElement              
{
    int xIndex { get; set; }
    int yIndex { get; set; }

    void Init(int x, int y, Board board);
}
