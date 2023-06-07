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
        mass = spatialEntityB.RigidBody.mass;
        distance = Vector3.Distance(spatialEntityA.SpatialObject.transform.position, spatialEntityB.SpatialObject.transform.position);

        spatialEntityA.SpatialObject.transform.LookAt(spatialEntityB.SpatialObject.transform);
    }

}
