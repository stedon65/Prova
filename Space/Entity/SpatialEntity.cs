using UnityEngine;


public class SpatialEntity
{
    private GameObject spatialObject = null;
    private Rigidbody rigidBody = null;    
    private IStartingVelocity startingVelocity = null;

    public delegate void SetValues(Vector3 force, Vector3 acceleration, Vector3 velocity);
    private SetValues delegateSetValues;

    private readonly float G = 1000.0f;


    public SpatialEntity(IStartingVelocity startingVelocity, SetValues delegateSetValues)
    {
        CreateObject();
        AddTrail();

        this.startingVelocity = startingVelocity;
        this.delegateSetValues = delegateSetValues;
    }

    private void CreateObject()
    {
        GameObject primitive = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        Rigidbody primitiveRB = primitive.AddComponent<Rigidbody>();
        primitiveRB.useGravity = false;
        primitiveRB.freezeRotation = true;
        primitiveRB.velocity = Vector3.zero;

        spatialObject = GameObject.Instantiate(primitive);
        rigidBody = spatialObject.GetComponent<Rigidbody>();

        GameObject.Destroy(primitive);
    }

    private void AddTrail()
    {
        TrailRenderer trailRenderer = spatialObject.AddComponent<TrailRenderer>();

        trailRenderer.time = 20.0f;
        trailRenderer.startWidth = 12.0f;
        trailRenderer.endWidth = 0.1f;
        trailRenderer.material = new Material(Shader.Find("Sprites/Default"));

        float alpha = 1.0f;
        Gradient gradient = new Gradient();
        gradient.SetKeys(new GradientColorKey[] { new GradientColorKey(Color.red, 0.0f), new GradientColorKey(Color.green, 1.0f) },
                         new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0.0f), new GradientAlphaKey(alpha, 1.0f) });
        trailRenderer.colorGradient = gradient;
    }

    private Vector3 LocalScale
    {
        get
        {
            return spatialObject.transform.localScale;
        }
        set
        {
            spatialObject.transform.localScale = value;
        }
    }

    private Transform Transform
    {
        get
        {
            // is readonly
            return spatialObject.transform;
        }
    }

    public Vector3 VectorRight
    {
        get
        {
            return spatialObject.transform.right;
        }
    }

    public string Name
    {
        get
        {
            return spatialObject.name;
        }
        set
        {
            spatialObject.name = value;
        }
    }

    public float Mass
    {
        get
        {
            return spatialObject.GetComponent<Rigidbody>().mass;
        }
        set
        {
            spatialObject.GetComponent<Rigidbody>().mass = value;
        }
    }

    public Vector3 Velocity
    {
        get
        {
            return spatialObject.GetComponent<Rigidbody>().velocity;
        }
        set
        {
            spatialObject.GetComponent<Rigidbody>().velocity = value;
        }
    }

    public Vector3 Radius
    {
        get
        {
            return spatialObject.transform.localScale / 2;
        }
        set
        {
            spatialObject.transform.localScale = value * 2;
        }
    }

    public Material Material
    {
        get
        {
            return spatialObject.GetComponent<MeshRenderer>().material;
        }
        set
        {
            spatialObject.GetComponent<MeshRenderer>().material = value;
        }
    }

    public void SetLookAtMe(SpatialEntity theEntity)
    {
        theEntity.Transform.LookAt(spatialObject.transform);
    }

    public void SetLookAtMe(GameObject theObject)
    {
        theObject.transform.LookAt(spatialObject.transform);
    }

    public void EnableTrail()
    {
        spatialObject.GetComponent<TrailRenderer>().enabled = true;
    }

    public void DisableTrail()
    {
        spatialObject.GetComponent<TrailRenderer>().enabled = false;
        spatialObject.GetComponent<TrailRenderer>().Clear();
    }

    public void SetAsParentOf(GameObject objectChild)
    {
        objectChild.transform.SetParent(spatialObject.transform);
    }

    public float CalculateDistanceTo(SpatialEntity entity)
    {
        return Vector3.Distance(spatialObject.transform.position, entity.Transform.position);
    }

    public void ApplyStartingVelocityRelativeTo(SpatialEntity orbited, float semimajorAxis)
    {
        startingVelocity.ApplyStartingVelocity(this, orbited, semimajorAxis, G);
    }

    public void ApplyGravityForceRelativeTo(SpatialEntity entity)
    {
        float mass = rigidBody.mass;
        float orbitedMass = entity.spatialObject.GetComponent<Rigidbody>().mass;

        Vector3 forceDirection = entity.spatialObject.transform.position - spatialObject.transform.position;
        
        float distanceSquared = Mathf.Pow(forceDirection.magnitude, 2);
        Vector3 normalizedDirection = forceDirection.normalized;
        float forceMagnitude = G * ((mass * orbitedMass) / distanceSquared);

        Vector3 force = normalizedDirection * forceMagnitude;
        Vector3 acceleration = normalizedDirection * ((G * orbitedMass) / distanceSquared);

        rigidBody.AddForce(force);

        if (delegateSetValues != null)
        {
            delegateSetValues(force, acceleration, rigidBody.velocity);
        }
    }

    public void PositionRelativeTo(SpatialEntity entity, Vector3 localPosition)
    {      
        Vector3 actualLocalScale = entity.LocalScale;
        Vector3 unitScale = new Vector3(1.0f, 1.0f, 1.0f);
        entity.LocalScale = unitScale;

        spatialObject.transform.SetParent(entity.Transform);
        spatialObject.transform.localPosition = localPosition;
        spatialObject.transform.SetParent(null);

        entity.LocalScale = actualLocalScale;
    }
}


