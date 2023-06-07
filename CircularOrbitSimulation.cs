using UnityEngine;


public class CircularOrbitSimulation : MonoBehaviour
{
    private SpatialEntity sun = null;
    private SpatialEntity earth = null;
    private SpatialEntity moon = null;
    private SpatialEntity[] bodies = null;

    public float entityRadiusFactor = 1.0f;
    public float entityPositionFactor = 1.0f;

    public Vector3 EarthForce = Vector3.zero;
    public Vector3 MoonForce = Vector3.zero;

    public Vector3 EarthAcceleration = Vector3.zero;
    public Vector3 MoonAcceleration = Vector3.zero;

    public Vector3 EarthVelocity = Vector3.zero;
    public Vector3 MoonVelocity = Vector3.zero;

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
        CircularVelocity circularVelocity = new CircularVelocity();

        sun = new SpatialEntity(circularVelocity, null);
        earth = new SpatialEntity(circularVelocity, SetEarthValues);
        moon = new SpatialEntity(circularVelocity, SetMoonValues);

        bodies = new SpatialEntity[3];
        bodies[0] = sun;
        bodies[1] = earth;
        bodies[2] = moon;

        Vector3 moonRadius = new Vector3(1.0f, 1.0f, 1.0f);
        Vector3 earthRadius = moonRadius * 3.6f;
        Vector3 sunRadius = moonRadius * 392.0f;

        float moonMass = 1.0f;
        float earthMass = moonMass * 81.3f;
        float sunMass = moonMass * (2.7069f * Mathf.Pow(10, 7));

        // reduced of a 10^-7 factor for simulation purpose
        float earthSunAverageDistance = 1.4960f * Mathf.Pow(10, 4);
        float moonEarthAverageDistance = 3.844f * Mathf.Pow(10, 1);

        sun.Name = "Sun";
        sun.Mass = sunMass;
        sun.Radius = sunRadius * entityRadiusFactor;

        earth.Name = "Earth";
        earth.Mass = earthMass;
        earth.Radius = earthRadius * entityRadiusFactor;
        earth.PositionRelativeTo(sun.SpatialObject.transform, (new Vector3(earthSunAverageDistance, 0.0f, 0.0f) / entityPositionFactor));

        moon.Name = "Moon";
        moon.Mass = moonMass;
        moon.Radius = moonRadius * entityRadiusFactor;
        moon.PositionRelativeTo(earth.SpatialObject.transform, (new Vector3(moonEarthAverageDistance, 0.0f, 0.0f) / entityPositionFactor));

        float moonEarthSemimajorAxis = moonEarthAverageDistance / entityPositionFactor;
        float moonSunSemimajorAxis = (earthSunAverageDistance + moonEarthAverageDistance) / entityPositionFactor;
        float earthSunSemimajorAxis = earthSunAverageDistance / entityPositionFactor;

        moon.ApplyStartingVelocityRelativeTo(earth, moonEarthSemimajorAxis);
        moon.ApplyStartingVelocityRelativeTo(sun, moonSunSemimajorAxis);
        earth.ApplyStartingVelocityRelativeTo(sun, earthSunSemimajorAxis);

    }

    // Update is called once per frame
    void Update()
    {
        camera3.transform.LookAt(earth.SpatialObject.transform);
        directionalLight1.transform.LookAt(earth.SpatialObject.transform);

        if (enableTrail)
        {
            earth.SpatialObject.GetComponent<TrailRenderer>().enabled = true;
            moon.SpatialObject.GetComponent<TrailRenderer>().enabled = true;
        }
        else
        {
            earth.SpatialObject.GetComponent<TrailRenderer>().enabled = false;
            moon.SpatialObject.GetComponent<TrailRenderer>().enabled = false;
            earth.SpatialObject.GetComponent<TrailRenderer>().Clear();
            moon.SpatialObject.GetComponent<TrailRenderer>().Clear();
        }

    }

    void FixedUpdate()
    {
        EarthForce = Vector3.zero;
        MoonForce = Vector3.zero;

        EarthAcceleration = Vector3.zero;
        MoonAcceleration = Vector3.zero;

        foreach (SpatialEntity celestialBodyA in bodies)
        {
            foreach (SpatialEntity celestialBodyB in bodies)
            {
                if (!celestialBodyA.Equals(celestialBodyB) && !(celestialBodyA.SpatialObject.name == "Sun"))
                {
                    celestialBodyA.ApplyGravityForceRelativeTo(celestialBodyB);
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
