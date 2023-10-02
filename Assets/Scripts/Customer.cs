using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Content;
using UnityEngine;

public class Customer : MonoBehaviour
{
    public int turnsUntilLeaves;

    [SerializeField]
    public List<Sprite> patienceSprites = new List<Sprite>();

    public int[] myBoardLocation;

    // Start is called before the first frame update
    void Start()
    {
        turnsUntilLeaves = GameManager.instance.turnsCustomerWillWait;
    }

    public void LowerPatience()
    {
        Debug.Log("Fired func...");
        turnsUntilLeaves--;
        gameObject.GetComponent<SpriteRenderer>().sprite = patienceSprites[turnsUntilLeaves];
        if( turnsUntilLeaves == 0 )
        {
            GameManager.instance.GameOver();
        }
    }
}
