using UnityEngine;


public class EllipticalVelocity : VelocityParameters, IStartingVelocity
{
    public EllipticalVelocity()
    {
    }

    public void ApplyStartingVelocity(SpatialEntity spatialEntityA, SpatialEntity spatialEntityB, float semimajorAxis, float G)
    {
        SetVelocityParameters(spatialEntityA, spatialEntityB);

        Vector3 velocity = spatialEntityA.VectorRight * Mathf.Sqrt((G * mass) * ((2 / distance) - (1 / semimajorAxis)));

        spatialEntityA.Velocity += velocity;
    }
}


