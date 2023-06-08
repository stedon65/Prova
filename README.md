# Moto orbitale in Unity

### Introduzione
Uno dei principi più importanti del design orientato agli oggetti afferma che è meglio _favorire la composizione rispetto all’ereditarietà_. Questa affermazione ha come focus primario la manutenibilità del software e implicitamente si riferisce ad un altro importante principio che esorta a _programmare verso le interfacce e non verso l’implementazione_. Se l’obiettivo come sviluppatori è quello di raggiungere un alto disaccoppiamento ed un'alta riusabilità del codice l’applicazione di questi due principi ci conduce sulla retta via.

Unity architetturalmente abbraccia questa filosofia alla radice implementando una **architettura a componenti**.

Lo sviluppo orientato ai componenti si preoccupa di definire le interfacce a cui i componenti devono conformarsi in modo che i client degli oggetti debbano solo interessarsi di quali protocolli di comunicazione necessitano e non di come sono implementati internamente. L’obiettivo dello sviluppatore in Unity è quello di _scomporre il dominio del problema in componenti_ il più possibile autonomi per poi aggregare questi componenti in _contenitori_ rappresentanti le astrazioni importanti del dominio stesso. È importante soprattutto individuare _componenti comportamentali_ da implementare come classi riutilizzabili. In questo senso, pensare ad un design pattern come _Strategy_ può aiutare molto.

In Unity i componenti gestiti dal motore interno devono derivare da una classe base specifica chiamata **MonoBehaviour** e questo permette al motore di poter effettuare alcune _callback_ ridefinite in ogni componenete in alcuni momenti ben precisi. Naturalmente è possibile anche definire dei propri componenti come classi autonome che possono essere utilizzati tramite opportune interfacce come si vedrà nel codice della simulazione.

Tra i componenti nativi che interessano maggiormenete questa simulazione sicuramente è importante citare il **Rigidbody** direttamente coinvolto dal modulo di fisica interno e tra le callback **FixedUpdate** che permette di avere un _refresh rate_ della fisica costante disaccoppiato dal _frame rate_ grafico.

### Fisica

![](https://dl.dropboxusercontent.com/s/ziekoxun8flpd8o/diagram1.png?dl=1)

Questa semplice simulazione del moto orbitale dei pianeti viene eseguita usando solo il motore del modulo di fisica di Unity. Non viene eseguito nessun calcolo della traiettoria orbitale da programma ma la traiettoria è il risultato dell'applicazione di forze e velocità ai corpi rigidi. **Si considera solo il moto di rivoluzione e non quello di rotazione**.

Vengono usate solo due equazioni della fisica classica.

La prima è la **legge di gravitazione universale di Newton**:

### $$F_G=G \dfrac{m_1m_2}{d^2}$$

Questa legge afferma che la forza gravitazionale $F_G$ generata tra due corpi nello spazio è direttamente proporzionale al prodotto delle masse dei corpi e inversamente proporzionale al quadrato della distanza tra i centri dei corpi. $G$ è la costante di gravitazione universale e vale $6.674 \times10^{-11}Nm^2 kg^{-2}$ ma per esigenze di simulazione viene qui aumentata di alcuni ordini di grandezza, $m_1$ e $m_2$ sono le masse dei corpi e $d^2$ è la distanza al quadrato tra i corpi.

In Unity viene assegnata questa forza ai corpi rigidi e sarà poi compito del modulo di fisica calcolare l'accelerazione gravitazionale per ogni corpo come segue:

### $$F_G=G \dfrac{m_1m_2}{d^2}=m_1a_g$$

### $$a_g=G \dfrac{m_2}{d^2}= \dfrac{F_G}{m_1}$$

La seconda equazione permette di calcolare la **velocità orbitale istantanea** di un corpo durante la sua **orbita kepleriana ellittica**:

### $$v^2= \mu \left ( \dfrac{2}{r}- \dfrac{1}{a} \right )$$

Quindi, per determinare la velocità istantanea abbiamo:

### $$v= \sqrt { \mu \left ( \dfrac{2}{r}- \dfrac{1}{a} \right )}$$

In questa equazione $v$ è la velocità istantanea del corpo nella sua orbita, $\mu$ è la **costante di gravitazione planetaria** e quando un corpo è molto più grande dell'altro (es. Sole-Terra) $\mu =GM$,  $r$ è la distanza del corpo da uno dei fuochi dell'ellisse occupato (in questo esempio dal Sole) e $a$ è il semiasse maggiore dell'ellisse.

La prima legge di Keplero afferma che _ogni pianeta si muove su orbite ellittiche ed il sole occupa uno dei fuochi dell'ellisse_. Il punto sulla traiettoria di un pianeta più lontano dal sole si chiama **Afelio** mentre il più vicino si chiama **Perielio**. La media tra Afelio e Perielio definisce il semiasse maggiore dell'ellisse. Le orbite ellittiche dei pianeti hanno una eccentricità molto bassa, quello della terra è $0.017$ e quindi il semiasse maggiore dell'ellisse è quasi uguale al raggio di un cerchio con il Sole al centro. In questo senso le orbite ellittiche dei pianeti possono essere approssimate a orbite circolari. Anche la Luna si muove su un'orbita ellittica rispetto alla Terra con il punto più lontano che si chiama **Apogeo** mentre quello più vicino **Perigeo**.

In un'orbita circolare $a=r$ la velocità tangenziale alla traiettoria è costante in modulo mentre l'accelerazione gravitazionale varia continuamente in direzione ma non influenza la velocità del corpo essendo normale alla velocità (moto circolare uniforme). **È proprio una ben precisa velocità tangenziale a mantenere il corpo in un'orbita circolare**. L'equazione della velocità orbitale istantanea può quindi essere semplificata per orbite circolari, in cui il semiasse maggiore è uguale alla distanza tra i corpi, come segue:

### $$a=r \to \left( \dfrac{2}{r} - \dfrac{1}{r} \right) = \left( \dfrac{1}{r} \right) \to v= \sqrt{ \mu \left( \dfrac{1}{r} \right )}= \sqrt{ \mu \left( \dfrac{1}{a} \right) }$$

In un'orbita ellittica $a \neq r$ **l'accelerazione gravitazionale varia continuamente in modulo e direzione modificando la velocità tangenziale in modulo e direzione** (accelerazione del corpo in prossimità del Perielio).

Quindi, per poter simulare in grafica 3D il moto orbitale planetario usando il motore di fisica di Unity è necessario rispettare le seguenti condizioni:

- I rapporti tra le masse dei corpi.
- I rapporti tra le distanze dei corpi.

In questa simulazione, quindi, ipotizzo quanto segue:

- La costante $G$ è aumentata di alcuni ordini di grandezza.
- La massa della Luna vale $1.0$.
- La massa di Mercurio è $4.5$ volte la massa lunare.
- La massa di Venere è $66.2$ volte la massa lunare.
- La massa della Terra è $81.3$ volte la massa lunare.
- La massa del Sole è $27069000.0$ volte la massa lunare.
- Le distanze in metri sono moltiplicate per un fattore $10^{-7}$
- Il Sole ha $v_i = 0$ e non risente della forza gravitazionale dei pianeti.
- La Terra e la Luna hanno velocità iniziale $v_i$ rispettivamente all'Afelio e all'Apogeo considerando l'Apogeo lunare in coincidenza dell'Afelio terrestre.

### Codice
Per eseguire diversi tipi di simulazione ho creato una classe **SpatialEntity** che permetta di rappresentare, **a livello di dominio**, una generica entità dello spazio dotata di alcune proprietà fondamentali e che sia composta con una classe **GameObject** di Unity che rappresenta l'oggetto grafico 3D a sua volta composto con il componente necessario alla simulazione fisica ovvero la classe **Rigidbody**.

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
        mass = spatialEntityB.Mass;
        distance = spatialEntityA.CalculateDistanceTo(spatialEntityB);

        spatialEntityB.SetLookAtMe(spatialEntityA);
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

        Vector3 velocity = spatialEntityA.VectorRight * Mathf.Sqrt((G * mass) / semimajorAxis);

        spatialEntityA.Velocity += velocity;
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

        Vector3 velocity = spatialEntityA.VectorRight * Mathf.Sqrt((G * mass) * ((2 / distance) - (1 / semimajorAxis)));

        spatialEntityA.Velocity += velocity;
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
        earth.SetAsParentOf(camera2);
        camera2.transform.localRotation = Quaternion.Euler(90.0f, 90.0f, 0.0f);
        camera2.transform.localPosition = new Vector3(0.0f, 10.0f, 0.0f);
        camera2.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

        camera3 = GameObject.Find("Camera 3");
        directionalLight1 = GameObject.Find("Light 1");

        Material sunMaterial = Resources.Load<Material>("Materials/SunMaterial");
        sun.Material = sunMaterial;

        Material earthMaterial = Resources.Load<Material>("Materials/EarthMaterial");
        earth.Material = earthMaterial;

        Material moonMaterial = Resources.Load<Material>("Materials/MoonMaterial");
        moon.Material = moonMaterial;
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
        mercury.PositionRelativeTo(sun, 
                                  (new Vector3(mercurySunAphelionDistance, 0.0f, 0.0f) / 
                                   entityPositionFactor) * scalarFactor);

        venus.Name = "Venus";
        venus.Mass = venusMass;
        venus.Radius = venusRadius * entityRadiusFactor;
        venus.PositionRelativeTo(sun, 
                                (new Vector3(venusSunAphelionDistance, 0.0f, 0.0f) / 
                                 entityPositionFactor) * scalarFactor);

        earth.Name = "Earth";
        earth.Mass = earthMass;
        earth.Radius = earthRadius * entityRadiusFactor;
        earth.PositionRelativeTo(sun, 
                                (new Vector3(earthSunAphelionDistance, 0.0f, 0.0f) / 
                                 entityPositionFactor) * scalarFactor);

        moon.Name = "Moon";
        moon.Mass = moonMass;
        moon.Radius = moonRadius * entityRadiusFactor;
        moon.PositionRelativeTo(earth, 
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
        earth.SetLookAtMe(camera3);
        earth.SetLookAtMe(directionalLight1);

        if (enableTrail)
        {
            mercury.EnableTrail();
            venus.EnableTrail();
            earth.EnableTrail();
            moon.EnableTrail();
        }
        else
        {
            mercury.DisableTrail();
            venus.DisableTrail();
            earth.DisableTrail();
            moon.DisableTrail();
        }
    }

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
                if (!bodyA.Equals(bodyB) && !(bodyA.Name == "Sun"))
                {
                    bodyA.ApplyGravityForceRelativeTo(bodyB);

                    MoonEarthDistance = moon.CalculateDistanceTo(earth);
                    EarthSunDistance = earth.CalculateDistanceTo(sun);
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

[Video Simulazioni](https://github.com/stedon65/moto-orbitale-in-unity/blob/main/Simulazioni.md)


![](https://dl.dropboxusercontent.com/s/bu9hofko5hswmkk/Circular01.mp4?dl=0)

