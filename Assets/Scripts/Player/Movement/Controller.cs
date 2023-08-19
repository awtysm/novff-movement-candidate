using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent (typeof(BoxCollider2D))]
public class Controller : MonoBehaviour
{
    
    struct RaycastOrigins
    {
        public Vector2 TLeft, TRight;
        public Vector2 BLeft, BRight;
    }
    public struct CollisionInfo
    {
        public bool above, below, left, right;
        public void reset()
        {
            above = below = false;
            left = right = false; 
        }
    }

    BoxCollider2D col;
    RaycastOrigins raycastOrigins;
    public CollisionInfo collisions;
    [SerializeField] LayerMask collisionMask;
    const float RaycastInset = 0.08f;
    [SerializeField][Range(2, 10)] int horRayCount = 5, verRayCount = 5;
    private float horRaySpacing, verRaySpacing;

    void Awake()
    {
        Application.targetFrameRate = 300;

        col = GetComponent<BoxCollider2D>();
        CalcRaySpacing();
    }
    
    void UpdateRaycastOrigins()
    {
        Bounds bounds = col.bounds;
        bounds.Expand(RaycastInset * -2);
        raycastOrigins.BLeft = new Vector2(bounds.min.x, bounds.min.y);
        raycastOrigins.BRight = new Vector2(bounds.max.x, bounds.min.y);
        raycastOrigins.TLeft = new Vector2(bounds.min.x, bounds.max.y);
        raycastOrigins.TRight = new Vector2(bounds.max.x, bounds.max.y);
    }
    void CalcRaySpacing()
    {
        Bounds bounds = col.bounds;
        bounds.Expand(RaycastInset * -2);
        horRayCount = Mathf.Clamp(horRayCount, 2, 10);
        verRayCount = Mathf.Clamp(verRayCount, 2, 10);

        horRaySpacing = bounds.size.y / (horRayCount - 1);
        verRaySpacing = bounds.size.x / (verRayCount - 1);
    }

    public void Move(Vector3 velocity)
    {
        UpdateRaycastOrigins();
        collisions.reset();
        if(velocity.x != 0) 
            HorizontalCollisions(ref velocity);
        if(velocity.y != 0)
            VerticalCollisions(ref velocity);
        

        transform.Translate(velocity);
    }

    void VerticalCollisions(ref Vector3 velocity)
    {
        float dirY = Mathf.Sign(velocity.y);
        float rayLength = Mathf.Abs(velocity.y) + RaycastInset;

        for(int i = 0; i < verRayCount; i++)
        {
            Vector2 rayOrigin = (dirY == -1) ? raycastOrigins.BLeft : raycastOrigins.TLeft;
            rayOrigin += Vector2.right * (verRaySpacing * i + velocity.x);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * dirY, rayLength, collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.up * dirY * rayLength,Color.green);

            if(hit)
            {
                velocity.y = (hit.distance - RaycastInset) * dirY;
                rayLength = hit.distance;

                collisions.below = dirY == -1; 
                collisions.above = dirY == 1; 
            }
        }
    }
    void HorizontalCollisions(ref Vector3 velocity)
    {
        float dirX = Mathf.Sign(velocity.x);
        float rayLength = Mathf.Abs(velocity.x)+ RaycastInset;

        for(int i = 0; i < horRayCount; i++)
        {
            Vector2 rayOrigin = (dirX == -1) ? raycastOrigins.BLeft : raycastOrigins.BRight;
            rayOrigin += Vector2.up * (horRaySpacing * i);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * dirX, rayLength, collisionMask);
            
            Debug.DrawRay(rayOrigin, Vector2.right * dirX * rayLength,Color.green);
            if(hit)
            {
                velocity.x = (hit.distance - RaycastInset) * dirX;
                rayLength = hit.distance;
                
                collisions.left = dirX == -1; 
                collisions.right = dirX == 1; 
            }
        }
    }

}
