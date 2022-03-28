using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomColider : MonoBehaviour
{
    [SerializeField] private  Bounds characterBounds;

    [SerializeField] [Range(0.1f, 0.3f)] private float rayBuffer = 0.1f;

  private RayRange raysUp, raysRight, raysDown,  raysLeft;  
  private void Update() {
      ProcessCollisions();
  }

    private void ProcessCollisions()
    {
        CalculateRayRange();
    }

    private void CalculateRayRange()
    {
         var b = new Bounds(transform.position, characterBounds.size);

        raysDown = new RayRange(b.min.x + rayBuffer, b.min.y, b.max.x - rayBuffer, b.min.y, Vector2.down);
        raysUp = new RayRange(b.min.x + rayBuffer, b.max.y, b.max.x - rayBuffer, b.max.y, Vector2.up);
        raysLeft = new RayRange(b.min.x, b.min.y + rayBuffer, b.min.x, b.max.y - rayBuffer, Vector2.left);
        raysRight = new RayRange(b.max.x, b.min.y + rayBuffer, b.max.x, b.max.y - rayBuffer, Vector2.right);
    }

    //rivate IEnumerable<Vector2> Evaluate

    private struct RayRange {

      public readonly Vector2 Start, End, Dir;
      public RayRange(float x1, float y1, float x2, float y2, Vector2 dir)
      {
          Start = new Vector2(x1, y1);
          End = new Vector2(y1, x1);
          Dir = dir;
      }
  }
}
