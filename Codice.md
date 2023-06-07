# Moto orbitale in Unity

### Codice
Per eseguire diversi tipi di simulazione ho creato una classe **SpatialEntity** che permetta di rappresentare, a livello di dominio, una generica entità dello spazio dotata di alcune proprietà fondamentali e che sia composta con una classe **GameObject** di Unity che rappresenta l'oggetto grafico 3D a sua volta composto con il componente necessario alla simulazione fisica ovvero la classe **Rigidbody**.

```cs
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

Il metodo **ApplyGravityForceRelativeTo** permette al client dell'oggetto di aggiungere una forza al corpo rigido **rigidBody** dell'oggetto grafico. La forza viene calcolata tramite l'equazione di gravitazione universale di Newton.

Il metodo **ApplyStartingVelocityRelativeTo** permette al client dell'oggetto di aggiungere una velocità iniziale al corpo rigido **rigidBody** dell'oggetto grafico. Al suo interno viene chiamata la **ApplyStartingVelocity** su un oggetto iniettato nel costruttore sull'interfaccia **IStartingVelocity**. L'interfaccia permette di usare **strategie diverse** per calcolare la velocità iniziale. **Sebbene la stessa formula possa essere usata sia per la velocità orbitale circolare che ellittica (quando il semiasse maggiore e la distanza sono uguali) ho preferito usare due formule diverse di cui una semplificata**.

```cs
public interface IStartingVelocity
{
    public void ApplyStartingVelocity(SpatialEntity spatialEntityA, SpatialEntity spatialEntityB, float semimajorAxis, float G);
}

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

public class CircularVelocity : VelocityParameters, IStartingVelocity
{
    public CircularVelocity()
    {
    }

    public void ApplyStartingVelocity(SpatialEntity spatialEntityA, SpatialEntity spatialEntityB, float semimajorAxis, float G)
    {
        SetVelocityParameters(spatialEntityA, spatialEntityB);

        Vector3 velocity = spatialEntityA.SpatialObject.transform.right * Mathf.Sqrt((G * mass) / semimajorAxis);

        spatialEntityA.RigidBody.velocity += velocity;
    }
}

public class EllipticalVelocity : VelocityParameters, IStartingVelocity
{
    public EllipticalVelocity()
    {
    }

    public void ApplyStartingVelocity(SpatialEntity spatialEntityA, SpatialEntity spatialEntityB, float semimajorAxis, float G)
    {
        SetVelocityParameters(spatialEntityA, spatialEntityB);

        Vector3 velocity = spatialEntityA.SpatialObject.transform.right * Mathf.Sqrt((G * mass) * ((2 / distance) - (1 / semimajorAxis)));

        spatialEntityA.RigidBody.velocity += velocity;
    }
}
```

Infine, ho sviluppato due classi client per due simulazioni diverse: **CircularOrbitSimulation** e **EllipticalOrbitSimulation**.

Nella seconda, che illustro qui brevemente, ho aggiunto Mercurio e Venere oltre a Terra, Luna e Sole.

```cs
public class EllipticalOrbitSimulation : MonoBehaviour
{
    private SpatialEntity sun = null;
    private SpatialEntity mercury = null;
    private SpatialEntity venus = null;
    private SpatialEntity earth = null;
    private SpatialEntity moon = null;
    private SpatialEntity[] bodies = null;

    public float entityRadiusFactor = 1.0f;
    public float entityPositionFactor = 1.0f;
    public float scalarFactor = 1.0f;

    public Vector3 EarthForce = Vector3.zero;
    public Vector3 MoonForce = Vector3.zero;

    public Vector3 EarthAcceleration = Vector3.zero;
    public Vector3 MoonAcceleration = Vector3.zero;

    public Vector3 EarthVelocity = Vector3.zero;
    public Vector3 MoonVelocity = Vector3.zero;

    public float MoonEarthDistance = 0.0f;
    public float EarthSunDistance = 0.0f;

    GameObject camera3 = null;
    GameObject directionalLight1 = null;
    public bool enableTrail = true;


    // Start is called before the first frame update
    void Start()
    {
        InitDomain();

        GameObject camera2 = GameObject.Find("Camera 2");
        camera2.transform.SetParent(earth.SpatialObject.transform);
        camera2.transform.localRotation = Quaternion.Euler(90.0f, 90.0f, 0.0f);
        camera2.transform.localPosition = new Vector3(0.0f, 10.0f, 0.0f);
        camera2.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

        camera3 = GameObject.Find("Camera 3");
        directionalLight1 = GameObject.Find("Light 1");

        Material sunMaterial = Resources.Load<Material>("Materials/SunMaterial");
        sun.SpatialObject.GetComponent<MeshRenderer>().material = sunMaterial;

        Material earthMaterial = Resources.Load<Material>("Materials/EarthMaterial");
        earth.SpatialObject.GetComponent<MeshRenderer>().material = earthMaterial;

        Material moonMaterial = Resources.Load<Material>("Materials/MoonMaterial");
        moon.SpatialObject.GetComponent<MeshRenderer>().material = moonMaterial;
    }

    private void InitDomain()
    {
        EllipticalVelocity ellipticalVelocity = new EllipticalVelocity();

        sun = new SpatialEntity(ellipticalVelocity, null);
        mercury = new SpatialEntity(ellipticalVelocity, null);
        venus = new SpatialEntity(ellipticalVelocity, null);
        earth = new SpatialEntity(ellipticalVelocity, SetEarthValues);
        moon = new SpatialEntity(ellipticalVelocity, SetMoonValues);

        bodies = new SpatialEntity[5];
        bodies[0] = sun;
        bodies[1] = mercury;
        bodies[2] = venus;
        bodies[3] = earth;
        bodies[4] = moon;

        Vector3 moonRadius = new Vector3(1.0f, 1.0f, 1.0f);
        Vector3 mercuryRadius = moonRadius * 1.4f;
        Vector3 venusRadius = moonRadius * 3.4f;
        Vector3 earthRadius = moonRadius * 3.6f;
        Vector3 sunRadius = moonRadius * 392.0f;

        float moonMass = 1.0f;
        float mercuryMass = moonMass * 4.5f;
        float venusMass = moonMass * 66.2f;
        float earthMass = moonMass * 81.3f;
        float sunMass = moonMass * (2.7069f * Mathf.Pow(10, 7));

        // reduced of a 10^-7 factor for simulation purpose
        float moonEarthAverageDistance = 3.844f * Mathf.Pow(10, 1);
        float mercurySunAverageDistance = 5.791f * Mathf.Pow(10, 3);
        float venusSunAverageDistance = 1.08209f * Mathf.Pow(10, 4);
        float earthSunAverageDistance = 1.4960f * Mathf.Pow(10, 4);

        // reduced of a 10^-7 factor for simulation purpose
        float moonEarthApogeeDistance = 4.055f * Mathf.Pow(10, 1);
        float mercurySunAphelionDistance = 6.9817f * Mathf.Pow(10, 3);
        float venusSunAphelionDistance = 1.08941f * Mathf.Pow(10, 4);
        float earthSunAphelionDistance = 1.5209f * Mathf.Pow(10, 4);

        sun.Name = "Sun";
        sun.Mass = sunMass;
        sun.Radius = sunRadius * entityRadiusFactor;

        mercury.Name = "Mercury";
        mercury.Mass = mercuryMass;
        mercury.Radius = mercuryRadius * entityRadiusFactor;
        mercury.PositionRelativeTo(sun.SpatialObject.transform, 
                                  (new Vector3(mercurySunAphelionDistance, 0.0f, 0.0f) / 
                                   entityPositionFactor) * scalarFactor);

        venus.Name = "Venus";
        venus.Mass = venusMass;
        venus.Radius = venusRadius * entityRadiusFactor;
        venus.PositionRelativeTo(sun.SpatialObject.transform, 
                                (new Vector3(venusSunAphelionDistance, 0.0f, 0.0f) / 
                                 entityPositionFactor) * scalarFactor);

        earth.Name = "Earth";
        earth.Mass = earthMass;
        earth.Radius = earthRadius * entityRadiusFactor;
        earth.PositionRelativeTo(sun.SpatialObject.transform, 
                                (new Vector3(earthSunAphelionDistance, 0.0f, 0.0f) / 
                                 entityPositionFactor) * scalarFactor);

        moon.Name = "Moon";
        moon.Mass = moonMass;
        moon.Radius = moonRadius * entityRadiusFactor;
        moon.PositionRelativeTo(earth.SpatialObject.transform, 
                               (new Vector3(moonEarthApogeeDistance, 0.0f, 0.0f) / 
                                entityPositionFactor) * scalarFactor);

        float moonEarthSemimajorAxis = moonEarthAverageDistance / entityPositionFactor;
        float moonSunSemimajorAxis = (earthSunAverageDistance + moonEarthApogeeDistance) / entityPositionFactor;
        float mercurySunSemimajorAxis = mercurySunAverageDistance / entityPositionFactor;
        float venusSunSemimajorAxis = venusSunAverageDistance / entityPositionFactor;
        float earthSunSemimajorAxis = earthSunAverageDistance / entityPositionFactor;

        moon.ApplyStartingVelocityRelativeTo(earth, moonEarthSemimajorAxis);
        moon.ApplyStartingVelocityRelativeTo(sun, moonSunSemimajorAxis);
        mercury.ApplyStartingVelocityRelativeTo(sun, mercurySunSemimajorAxis);
        venus.ApplyStartingVelocityRelativeTo(sun, venusSunSemimajorAxis);
        earth.ApplyStartingVelocityRelativeTo(sun, earthSunSemimajorAxis);
    }

    // Update is called once per frame
    void Update()
    {
        camera3.transform.LookAt(earth.SpatialObject.transform);
        directionalLight1.transform.LookAt(earth.SpatialObject.transform);

        if (enableTrail)
        {
            mercury.SpatialObject.GetComponent<TrailRenderer>().enabled = true;
            venus.SpatialObject.GetComponent<TrailRenderer>().enabled = true;
            earth.SpatialObject.GetComponent<TrailRenderer>().enabled = true;
            moon.SpatialObject.GetComponent<TrailRenderer>().enabled = true;
        }
        else
        {
            mercury.SpatialObject.GetComponent<TrailRenderer>().enabled = false;
            venus.SpatialObject.GetComponent<TrailRenderer>().enabled = false;
            earth.SpatialObject.GetComponent<TrailRenderer>().enabled = false;
            moon.SpatialObject.GetComponent<TrailRenderer>().enabled = false;

            mercury.SpatialObject.GetComponent<TrailRenderer>().Clear();
            venus.SpatialObject.GetComponent<TrailRenderer>().Clear();
            earth.SpatialObject.GetComponent<TrailRenderer>().Clear();
            moon.SpatialObject.GetComponent<TrailRenderer>().Clear();
        }
    }

    // FixedUpdate is called every 0.02s by physics engine
    void FixedUpdate()
    {
        EarthForce = Vector3.zero;
        MoonForce = Vector3.zero;

        EarthAcceleration = Vector3.zero;
        MoonAcceleration = Vector3.zero;

        foreach (SpatialEntity bodyA in bodies)
        {
            foreach (SpatialEntity bodyB in bodies)
            {
                if (!bodyA.Equals(bodyB) && !(bodyA.SpatialObject.name == "Sun"))
                {
                    bodyA.ApplyGravityForceRelativeTo(bodyB);

                    MoonEarthDistance = Vector3.Distance(moon.SpatialObject.transform.position, 
                                                         earth.SpatialObject.transform.position);
                    EarthSunDistance = Vector3.Distance(earth.SpatialObject.transform.position, 
                                                        sun.SpatialObject.transform.position);
                }
            }
        }
    }

    public void SetEarthValues(Vector3 force, Vector3 acceleration, Vector3 velocity)
    {
        EarthForce += force;
        EarthAcceleration += acceleration;
        EarthVelocity = velocity;
    }

    public void SetMoonValues(Vector3 force, Vector3 acceleration, Vector3 velocity)
    {
        MoonForce += force;
        MoonAcceleration += acceleration;
        MoonVelocity = velocity;
    }
}
```

Nella callback **Start** di Unity viene chiamato il metodo **InitDomain** per l'nizializzazione degli oggetti. Qui si crea l'oggetto di tipo **EllipticalVelocity** per la velocità iniziale e gli oggetti di tipo **SpatialEntity** per i Pianeti ed il Sole impostando i parametri numerici per la simulazione.

Nella callback **FixedUpdate** chiamata da Unity ogni **50 volte al secondo** viene chiamata la **ApplyGravityForceRelativeTo** per ogni coppia di corpi celesti e quindi calcolata la forza gravitazionale di un oggetto e applicata al suo Rigidbody interno gestito dal motore di fisica di Unity.
