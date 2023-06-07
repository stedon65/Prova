# Moto orbitale in Unity

### Codice
Per eseguire diversi tipi di simulazione ho creato una classe **SpatialEntity** che permetta di rappresentare, a livello di dominio, una generica entità dello spazio dotata di alcune proprietà fondamentali e che sia composta con una classe **GameObject** di Unity che rappresenta l'oggetto grafico 3D a sua volta composto con il componente necessario alla simulazione fisica ovvero la classe **Rigidbody**.

```cs
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
```

Nel costruttore della classe viene chiamato il metodo **CreateObject** per creare l'istanza del **GameObject** partendo dalla primitiva **Sphere** a cui viene aggiunto il componente **Rigidbody**.

La classe **Rigidbody** consente a Unity di controllare direttamente il moto dell'oggetto grafico tramite il motore di fisica built-in. Per queste simulazioni, tuttavia, la proprietà **useGravity** viene disabilitata in quanto la forza gravitazionale viene calcolata direttamente dalla classe **SpatialEntity**.

Successivamente viene chiamato il metodo **AddTrail** che aggiunge il componente **TrailRenderer** per disegnare la traccia durante il moto orbitale dei pianeti.

Il metodo **PositionRelativeTo** permette al client dell'oggetto di posizionare l'oggetto grafico **spatialObject** rispetto ad un altro oggetto di riferimento _parent_ tramite il suo componente **Transform objectTransform**. L'oggetto **spatialObject**, quindi, avrà il proprio sistema di riferimento posizionato in coordinate locali rispetto al sistema di riferimento dell'oggetto **objectTranform**. In questo modo, ad esempio, sarà facile posizionare la Luna conoscendo la distanza dalla Terra tramite un vettore **localPosition**.











