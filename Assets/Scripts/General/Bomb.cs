using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

//Bombs inherit from the Hex class.
public class Bomb : Hex
{
    private int timer;
    [SerializeField] private List<Sprite> timerSprites;
    SpriteRenderer spriteRenderer;
    GameManager gameManager;
    Board board;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        board = FindObjectOfType<Board>();
        spriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
        timer = 5;
        spriteRenderer.sprite = timerSprites[timer - 1];
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Ticks down timer. This is called after every valid move
    public void DecreaseTimer()
    {
        timer--;
        if (timer == 0)
        {
            KingdomCome();
        }
        else
        {
            spriteRenderer.sprite = timerSprites[timer - 1];
        }
    }

    //Tells GameManager to end the game
    public void KingdomCome()
    {
        gameManager.EndGame();
    }
}
