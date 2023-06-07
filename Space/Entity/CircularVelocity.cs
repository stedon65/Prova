using UnityEngine;


public class CircularVelocity : VelocityParameters, IStartingVelocity
{
    public CircularVelocity()
    {
    }

    public void ApplyStartingVelocity(SpatialEntity spatialEntityA, SpatialEntity spatialEntityB, float semimajorAxis, float G)
    {
        SetVelocityParameters(spatialEntityA, spatialEntityB);

        Vector3 velocity = spatialEntityA.VectorRight * Mathf.Sqrt((G * mass) / semimajorAxis);

        spatialEntityA.Velocity += velocity;
    }
}


