using System.Numerics;

namespace GameEngine;

//character contorller
//https://www.youtube.com/watch?v=YR6Q7dUz2uk&t

//university paper
//https://research.ncl.ac.uk/game/mastersdegree/gametechnologies/physicstutorials/1raycasting/Physics%20-%20Raycasting.pdf

//2d box collide
//https://www.youtube.com/watch?v=eo_hrg6kVA8
public static class Physics
{
    public static bool Raycast(Ray ray, out RaycastHit hit)
    {
        return true;
    }

    public static bool Raycast(Ray ray, CollisionBox collisionBox)
    {
        return false;

        //
    }

    public static bool RayPlaneIntersection(Ray ray, Plane plane)
    {
        float denom = Vector3.Dot(plane.Normal, ray.Direction);
        if (MathF.Abs(denom) > 0.0001f)
        {
            float t = Vector3.Dot(plane.GetPointOnPlane() - ray.Position, plane.Normal) / denom;
            if (t >= 0)
                return true;
        }

        return false;
    }
}

public struct Ray
{
    public Vector3 Position;
    public Vector3 Direction;
}

public struct RaycastHit
{

}