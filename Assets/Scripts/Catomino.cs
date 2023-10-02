using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Catomino : MonoBehaviour
{
    [SerializeField]
    public List<ListWrapper> relativeBlockPositions;

    [SerializeField]
    public Vector2 placementOffset = Vector2.zero;

    public int[] myBoardLocation;

    public void Start()
    {
        //Invoke("GiveLocation", 0.5f);
    }

    public void GiveLocation()
    {
        Debug.Log($"Meow! My position is ({string.Join(",", myBoardLocation)})");
    }

    public List<int[]> GetOverlappingSquares()
    {
        List<List<int>> rbps = new List<List<int>>();

        foreach(ListWrapper l in relativeBlockPositions)
        {
            rbps.Add(l.myList);
        }

        List<int[]> overlaps = new List<int[]>();
        foreach(List<int> rbp in rbps)
        {
            int xPos = myBoardLocation[0] + rbp[0];
            int yPos = myBoardLocation[1] + rbp[1];

            overlaps.Add(new int[] { xPos, yPos });
        }

        overlaps.Add(myBoardLocation);

        Debug.Log($"Cat located at ({myBoardLocation[0]},{myBoardLocation[1]}) has the following squares it sits on: {string.Join(" || ", overlaps.Select(a=>string.Join(",",a)))}");

        //foreach (int[] o in overlaps)
        //{
        //    Debug.Log($"({o[0]},{o[1]})");
        //}

        return overlaps;
    }

    public List<int[]> GetOverlappingSquares(int[] proposedLocation)
    {
        List<List<int>> rbps = new List<List<int>>();

        foreach (ListWrapper l in relativeBlockPositions)
        {
            rbps.Add(l.myList);
        }

        List<int[]> overlaps = new List<int[]>();
        foreach (List<int> rbp in rbps)
        {
            int xPos = proposedLocation[0] + rbp[0];
            int yPos = proposedLocation[1] + rbp[1];

            overlaps.Add(new int[] { xPos, yPos });
        }

        overlaps.Add(proposedLocation);

        Debug.Log($"Cat located at ({proposedLocation[0]},{proposedLocation[1]}) has the following squares it sits on: {string.Join(" || ", overlaps.Select(a => string.Join(",", a)))}");

        //foreach (int[] o in overlaps)
        //{
        //    Debug.Log($"({o[0]},{o[1]})");
        //}

        return overlaps;
    }
}
