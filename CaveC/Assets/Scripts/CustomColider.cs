using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CustomColider : MonoBehaviour
{

    [SerializeField] [Range(0.05f, 0.3f)] private float rayBuffer = 0.1f;
    [SerializeField] private int detectorCount;
    [SerializeField] private float detectionRayLength;
    [SerializeField] private bool gizmos = true;
    [SerializeField] CapsuleCollider2D capsuleCollider;
    [SerializeField] float rayBoundsScale = 0.85f;
   
    private Bounds b;
    private RayRange raysUp, raysRight, raysDown,  raysLeft;


    private void Update() {
      ProcessCollisions();
  }

    private void ProcessCollisions()
    {
        CalculateRayRange();
    }

    private bool RunDetection(RayRange range, LayerMask layer)
    {
        return EvaluateRayPositions(range).Any(point => Physics2D.Raycast(point, range.Dir, detectionRayLength, layer))
        || Physics2D.OverlapArea(b.min, b.max, layer);
    }

    private List<RayRange> CalculateRayRange()
    {
         b = new Bounds(new Vector2(transform.position.x, transform.position.y) + capsuleCollider.offset, capsuleCollider.size * rayBoundsScale);

        raysDown = new RayRange(b.min.x + rayBuffer, b.min.y, b.max.x - rayBuffer, b.min.y, Vector2.down);
        raysUp = new RayRange(b.min.x + rayBuffer, b.max.y, b.max.x - rayBuffer, b.max.y, Vector2.up);
        raysLeft = new RayRange(b.min.x, b.min.y + rayBuffer, b.min.x, b.max.y - rayBuffer, Vector2.left);
        raysRight = new RayRange(b.max.x, b.min.y + rayBuffer, b.max.x, b.max.y - rayBuffer, Vector2.right);

        return new List<RayRange>{raysDown,raysUp,raysLeft,raysRight};
    }

    private IEnumerable<Vector2> EvaluateRayPositions(RayRange range) {
        for(var i = 0; i < detectorCount; i++) {
            var t = (float)i / (detectorCount -1);
            yield return Vector2.Lerp(range.Start, range.End, t);
        }
    }

    public bool isTouching(LayerMask layerMask) {
        foreach(RayRange range in CalculateRayRange())
        {
            if(RunDetection(range, layerMask)) return true;
        }
        return false;
    }

    public bool isTouching(LayerMask layerMask, Vector2 dir) {
        foreach(RayRange range in CalculateRayRange())
        {
            if(range.Dir == dir) return RunDetection(range, layerMask);
        }
        return false;
    }

    private void OnDrawGizmosSelected() {
        if (!gizmos) return;

        // Rays
        if (!Application.isPlaying) {
            CalculateRayRange();
            Gizmos.color = Color.blue;
            foreach (var range in new List<RayRange> {raysUp, raysDown, raysLeft, raysRight}) {
                foreach (var point in EvaluateRayPositions(range)) {
                    Gizmos.DrawRay(point, range.Dir * detectionRayLength);
                }
            }
        }

        if (!Application.isPlaying) return;
    }

    private struct RayRange {

      public readonly Vector2 Start, End, Dir;
      public RayRange(float x1, float y1, float x2, float y2, Vector2 dir)
      {
          Start = new Vector2(x1, y1);
          End = new Vector2(x2, y2);
          Dir = dir;
      }
  }
}
