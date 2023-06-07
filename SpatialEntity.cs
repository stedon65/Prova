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

    public GameObject SpatialObject
    {
        get
        {
            return spatialObject;
        }
    }

    public Rigidbody RigidBody
    {
        get
        {
            return rigidBody;
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

    public Vector3 Radius
    {
        get
        {
            return spatialObject.transform.localScale;
        }
        set
        {
            spatialObject.transform.localScale = value * 2;
        }
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

    public void PositionRelativeTo(Transform objectTransform, Vector3 localPosition)
    {
        Vector3 actualLocalScale = objectTransform.localScale;
        Vector3 unitScale = new Vector3(1.0f, 1.0f, 1.0f);
        objectTransform.localScale = unitScale;

        spatialObject.transform.SetParent(objectTransform);
        spatialObject.transform.localPosition = localPosition;
        spatialObject.transform.SetParent(null);

        objectTransform.localScale = actualLocalScale;
    }

}


