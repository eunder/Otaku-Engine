using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class VertexDistanceComparer : IComparer<SaveAndLoadLevel.Corner>
{
    private Vector3 targetPos;

    public VertexDistanceComparer(Vector3 targetPos)
    {
        this.targetPos = targetPos;
    }

    public int Compare(SaveAndLoadLevel.Corner a, SaveAndLoadLevel.Corner b)
    {
        float distanceA = Vector3.Distance(a.corner_Pos, targetPos);
        float distanceB = Vector3.Distance(b.corner_Pos, targetPos);

        // Compare the distances and return the result.
        // For sorting in ascending order (closest first):
         return distanceA.CompareTo(distanceB);
        // For sorting in descending order (farthest first):
        //return distanceB.CompareTo(distanceA);
    }
}