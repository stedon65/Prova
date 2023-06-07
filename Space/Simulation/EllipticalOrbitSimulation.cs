using UnityEngine;


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
        float moonSunSemimajorAxis = (earthSunAverageDistance + moonEarthApogeeDistance) / 
                                      entityPositionFactor;
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


