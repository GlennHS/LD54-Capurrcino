using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseHandler : MonoBehaviour
{
    public static MouseHandler instance;

    // Start is called before the first frame update
    void Awake()
    {
        if (instance != null)
            Destroy(this);
        else
            instance = this;

        // Variable initialisation
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.gameOver) return;
        if (Input.GetMouseButtonDown(0))
        {
            // Debug.Log($"Board coord clicked: {ClickToBoardPos(Input.mousePosition)[0]} / {ClickToBoardPos(Input.mousePosition)[1]}");
            int[] boardPosClicked = ClickToBoardPos(Input.mousePosition);

            //if (BoardManager.instance.CheckTileEmpty(boardPosClicked[0], boardPosClicked[1]))
            BoardManager.instance.PlaceNextCatomino(boardPosClicked);
        }
    }

    public int[] ClickToBoardPos(Vector3 pos)
    {
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(pos);

        int xPos = Mathf.FloorToInt((worldPos - BoardManager.instance.boardBottomLeft).x + BoardManager.instance.gridSize / 2);
        int yPos = Mathf.FloorToInt((worldPos - BoardManager.instance.boardBottomLeft).y + BoardManager.instance.gridSize / 2);

        return new int[] { xPos, yPos };
    }
}
