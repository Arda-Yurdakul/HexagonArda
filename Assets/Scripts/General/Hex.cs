using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class for the game pieces(Hexes).
[RequireComponent(typeof(SpriteRenderer))]
public class Hex : MonoBehaviour, IGridElement
{
    public int xIndex { get; set; }
    public int yIndex { get; set; }

    public HexType hexType;                  //HexTypes. Default number in the Editor is 5
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

    public void Init(int x, int y, Board board)         //Called from the Board class to keep pieces updated of their own properties
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

    public void MoveHex(float destX, float destY, float moveTime)
    {
        StartCoroutine(MoveRoutine(new Vector3(destX, destY, 0), moveTime));
    }


    //Lerp Coroutine for moving objects. Uses an Ease-Out style of interpolation
    public IEnumerator MoveRoutine(Vector3 target, float moveTime)
    {
        Vector3 startPosition = transform.position;

        bool reachedDestination = false;
        float elapsedTime = 0f;

        while (!reachedDestination)
        {
            if (Vector3.Distance(transform.position, target) < 0.01f)
            {
                reachedDestination = true;
                m_board.PlaceHex(this, target);
                break;
            }


            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp(elapsedTime / moveTime, 0f, 1f);
            t = Mathf.Sin(t * Mathf.PI * 0.5f);

            transform.position = Vector3.Lerp(startPosition, target, t);
            yield return null;

        }
    }
}
