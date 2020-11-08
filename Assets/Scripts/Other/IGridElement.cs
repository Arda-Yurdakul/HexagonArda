using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGridElement
{
    int xIndex { get; set; }
    int yIndex { get; set; }

    void Init(int x, int y, Board board);
}
