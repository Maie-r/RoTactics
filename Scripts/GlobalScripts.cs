using System;
using Godot;

public static class GlobalScripts
{

}

public static class TempTimer
{
    public static Timer Get(float WaitTime)
    {
        if (WaitTime < 0) WaitTime += 0.001f;
        Timer tt = new Timer()
        {
            Autostart = true,
            OneShot = true,
            WaitTime = WaitTime
        };
        tt.Timeout += tt.QueueFree;
        return tt;
    }
}

public static class CollisionHelper
{
    public static KinematicCollision2D CustomMoveAndCollide(
        PhysicsBody2D mover,
        Vector2 motion,
        Func<CollisionObject2D, bool> filter = null,
        int maxResults = 8)
    {
        var spaceState = mover.GetWorld2D().DirectSpaceState;

        var shapeOwner = mover.ShapeOwnerGetOwner(mover.ShapeFindOwner(0));
        var shape = mover.ShapeOwnerGetShape(mover.ShapeFindOwner(0), 0);

        var query = new PhysicsShapeQueryParameters2D
        {
            Shape = shape,
            Transform = mover.GlobalTransform.Translated(motion),
            CollisionMask = mover.CollisionMask
            //CollisionLayer = mover.CollisionLayer
        };

        var results = spaceState.IntersectShape(query, maxResults);

        foreach (var result in results)
        {
            var collider = (CollisionObject2D)result["collider"];

            // Apply custom filter (skip if says "false")
            if (filter != null && !filter(collider))
                continue;

            // Construct a fake KinematicCollision2D-like response
            var collision = new KinematicCollision2D();
            /*collision.Collider = collider;
            collision.Position = (Vector2)result["position"];
            collision.Normal = (Vector2)result["normal"];
            collision.Travel = motion;//*/
            return collision;
        }

        // No collision
        mover.GlobalPosition += motion;
        return null;
    }
}