using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarUnitInterfaceLines : MonoBehaviour
{
    /*
     For ease, put this in seperate class
     */

    private const float LineZaxis = 1.0f;

    public LineRenderer movementLine;

    private Vector3[] pointsArray;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void AddMovementLine(Vector2[] points)
    {
        /* Draw a Line with the "movement" effect
         To signal that a ship will be moving next turn.
         */
        
        if (points.Length == 0)
        {
            // Clear instead.
            ClearMovementLine();
            return;
        }

        movementLine.positionCount = points.Length +1;
        //movementLine always start from the origin of the ship
        movementLine.SetPosition(0,transform.position);
        for (var idx = 0; idx < points.Length; idx++)
        {
            var point2D = points[idx];
            movementLine.SetPosition(idx + 1, new Vector3(point2D.x, point2D.y, LineZaxis));
        }
        // movementLine.SetPositions(new []{
        //     new Vector3(1, 2, 3)
        // });
        
    }

    public void ClearMovementLine()
    {
        movementLine.positionCount = 0;
    }

    public void ClearAllLines()
    {
        ClearMovementLine();
    }
}
