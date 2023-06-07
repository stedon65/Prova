using UnityEngine;


public abstract class VelocityParameters
{
    protected float mass = 0.0f;
    protected float distance = 0.0f;

    public VelocityParameters()
    {
    }

    protected virtual void SetVelocityParameters(SpatialEntity spatialEntityA, SpatialEntity spatialEntityB)
    {
        mass = spatialEntityB.Mass;
        distance = spatialEntityA.CalculateDistanceTo(spatialEntityB);

        spatialEntityB.SetLookAtMe(spatialEntityA);
    }
}
