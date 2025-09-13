using UnityEngine;
using System.Collections.Generic;
using System.Collections;


/// VisualEffectsManager provides atmospheric visual enhancements for the enhanced Akash 2D walkway demo.
/// Features background layers, parallax scrolling, dynamic lighting, and particle effects for professional presentation.

public class VisualEffectsManager : MonoBehaviour
{
    [Header("Background Layers")]
    [SerializeField] private bool enableParallax = true;
    [SerializeField] private ParallaxLayer[] parallaxLayers;
    [SerializeField] private Color skyColor = new Color(0.2f, 0.3f, 0.6f, 1f);
    [SerializeField] private Color groundColor = new Color(0.1f, 0.2f, 0.1f, 1f);

    [Header("Atmospheric Lighting")]
    [SerializeField] private bool enableDynamicLighting = true;
    [SerializeField] private Color ambientLightColor = new Color(0.4f, 0.4f, 0.6f, 1f);
    [SerializeField] private Light directionalLight;
    [SerializeField] private float lightIntensity = 1f;
    [SerializeField] private AnimationCurve lightVariationCurve = AnimationCurve.EaseInOut(0, 0.8f, 1, 1.2f);

    [Header("Particle Effects")]
    [SerializeField] private bool enableParticleEffects = true;
    [SerializeField] private GameObject particleSystemPrefab;
    [SerializeField] private int maxParticleSystems = 3;
    [SerializeField] private ParticleEffectType[] effectTypes;

    [Header("Environmental Effects")]
    [SerializeField] private bool enableEnvironmentalEffects = true;
    [SerializeField] private float windSpeed = 1f;
    [SerializeField] private Vector2 windDirection = Vector2.right;
    [SerializeField] private AnimationCurve windVariation = AnimationCurve.EaseInOut(0, 0.5f, 1, 1.5f);

    [Header("Performance Settings")]
    [SerializeField] private bool enableCulling = true;
    [SerializeField] private float cullingDistance = 20f;
    [SerializeField] private int maxVisibleEffects = 10;

    // System components
    private Camera mainCamera;
    private CameraBehaviorSystem cameraSystem;
    private WalkwaySystem walkwaySystem;

    // Background management
    private List<GameObject> backgroundObjects = new List<GameObject>();
    private Vector3 lastCameraPosition;

    // Lighting management
    private List<Light> dynamicLights = new List<Light>();
    private float lightAnimationTime;

    // Particle management
    private List<ParticleSystem> activeParticleSystems = new List<ParticleSystem>();
    private Queue<ParticleSystem> particlePool = new Queue<ParticleSystem>();

    // State tracking
    private bool isInitialized = false;
    private Bounds visibleBounds;

    private void Awake()
    {
        // Find system components
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            mainCamera = FindObjectOfType<Camera>();
        }

        cameraSystem = FindObjectOfType<CameraBehaviorSystem>();
        walkwaySystem = FindObjectOfType<WalkwaySystem>();
    }

    private void Start()
    {
        InitializeVisualEffects();
        isInitialized = true;

        Debug.Log("VisualEffectsManager: Initialized with atmospheric effects enabled");
    }

    private void Update()
    {
        if (!isInitialized) return;

        UpdateParallaxLayers();
        UpdateDynamicLighting();
        UpdateParticleEffects();
        UpdateCulling();
    }


    /// Initializes all visual effect systems

    private void InitializeVisualEffects()
    {
        SetupBackgroundLayers();
        SetupLighting();
        SetupParticleEffects();
        CreateEnvironmentalEffects();

        if (mainCamera != null)
        {
            lastCameraPosition = mainCamera.transform.position;
        }
    }


    /// Sets up parallax background layers

    private void SetupBackgroundLayers()
    {
        if (!enableParallax) return;

        // Create default parallax layers if none are specified
        if (parallaxLayers == null || parallaxLayers.Length == 0)
        {
            CreateDefaultParallaxLayers();
        }

        // Initialize each layer
        for (int i = 0; i < parallaxLayers.Length; i++)
        {
            CreateParallaxLayer(parallaxLayers[i], i);
        }
    }


    /// Creates default parallax layers

    private void CreateDefaultParallaxLayers()
    {
        parallaxLayers = new ParallaxLayer[]
        {
            new ParallaxLayer
            {
                name = "Sky",
                parallaxSpeed = 0f,
                color = skyColor,
                sortingOrder = -100,
                scale = Vector2.one * 2f
            },
            new ParallaxLayer
            {
                name = "Mountains",
                parallaxSpeed = 0.2f,
                color = new Color(0.3f, 0.4f, 0.6f, 0.8f),
                sortingOrder = -80,
                scale = Vector2.one * 1.5f
            },
            new ParallaxLayer
            {
                name = "Hills",
                parallaxSpeed = 0.5f,
                color = new Color(0.2f, 0.4f, 0.3f, 0.6f),
                sortingOrder = -60,
                scale = Vector2.one * 1.2f
            },
            new ParallaxLayer
            {
                name = "Ground",
                parallaxSpeed = 1f,
                color = groundColor,
                sortingOrder = -20,
                scale = Vector2.one
            }
        };
    }


    /// Creates a single parallax layer

    private void CreateParallaxLayer(ParallaxLayer layer, int index)
    {
        GameObject layerObject = new GameObject($"ParallaxLayer_{layer.name}");
        layerObject.transform.parent = transform;

        // Create sprite renderer
        SpriteRenderer renderer = layerObject.AddComponent<SpriteRenderer>();
        renderer.sprite = CreateLayerSprite(layer);
        renderer.color = layer.color;
        renderer.sortingOrder = layer.sortingOrder;

        // Scale the layer
        layerObject.transform.localScale = new Vector3(layer.scale.x, layer.scale.y, 1f);

        // Position behind camera
        Vector3 position = Vector3.zero;
        if (mainCamera != null)
        {
            position = mainCamera.transform.position;
            position.z = 10f + index; // Ensure proper depth sorting
        }
        layerObject.transform.position = position;

        // Add parallax behavior component
        ParallaxBehavior parallax = layerObject.AddComponent<ParallaxBehavior>();
        parallax.Initialize(layer.parallaxSpeed, mainCamera);

        backgroundObjects.Add(layerObject);
    }


    /// Creates a sprite for a parallax layer

    private Sprite CreateLayerSprite(ParallaxLayer layer)
    {
        int width = 512;
        int height = 256;
        Texture2D texture = new Texture2D(width, height);

        Color[] pixels = new Color[width * height];

        // Create layer-specific patterns
        switch (layer.name.ToLower())
        {
            case "sky":
                CreateSkyPattern(pixels, width, height);
                break;
            case "mountains":
                CreateMountainPattern(pixels, width, height);
                break;
            case "hills":
                CreateHillPattern(pixels, width, height);
                break;
            case "ground":
                CreateGroundPattern(pixels, width, height);
                break;
            default:
                CreateGenericPattern(pixels, width, height);
                break;
        }

        texture.SetPixels(pixels);
        texture.Apply();

        return Sprite.Create(texture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f), 64f);
    }


    /// Creates sky pattern

    private void CreateSkyPattern(Color[] pixels, int width, int height)
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float gradient = (float)y / height;
                Color color = Color.Lerp(skyColor * 0.8f, skyColor * 1.2f, gradient);
                pixels[y * width + x] = color;
            }
        }
    }


    /// Creates mountain silhouette pattern

    private void CreateMountainPattern(Color[] pixels, int width, int height)
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float mountainHeight = Mathf.PerlinNoise(x * 0.01f, 0) * height * 0.7f + height * 0.2f;

                if (y < mountainHeight)
                {
                    pixels[y * width + x] = Color.white;
                }
                else
                {
                    pixels[y * width + x] = Color.clear;
                }
            }
        }
    }


    /// Creates hill pattern

    private void CreateHillPattern(Color[] pixels, int width, int height)
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float hillHeight = Mathf.PerlinNoise(x * 0.02f, 0) * height * 0.5f + height * 0.1f;

                if (y < hillHeight)
                {
                    pixels[y * width + x] = Color.white;
                }
                else
                {
                    pixels[y * width + x] = Color.clear;
                }
            }
        }
    }


    /// Creates ground pattern

    private void CreateGroundPattern(Color[] pixels, int width, int height)
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (y < height * 0.3f)
                {
                    float noise = Mathf.PerlinNoise(x * 0.1f, y * 0.1f);
                    Color color = Color.Lerp(groundColor * 0.8f, groundColor * 1.2f, noise);
                    pixels[y * width + x] = color;
                }
                else
                {
                    pixels[y * width + x] = Color.clear;
                }
            }
        }
    }


    /// Creates generic pattern

    private void CreateGenericPattern(Color[] pixels, int width, int height)
    {
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = Color.white;
        }
    }


    /// Sets up dynamic lighting system

    private void SetupLighting()
    {
        if (!enableDynamicLighting) return;

        // Set ambient light
        RenderSettings.ambientLight = ambientLightColor;

        // Create or setup directional light
        if (directionalLight == null)
        {
            GameObject lightObject = new GameObject("DirectionalLight");
            lightObject.transform.parent = transform;

            // Use regular Light component for compatibility
            directionalLight = lightObject.AddComponent<Light>();
            directionalLight.type = LightType.Directional;
            directionalLight.intensity = lightIntensity;
            directionalLight.color = Color.white;
            directionalLight.shadows = LightShadows.None;

            // Position and rotate the light for 2D effect
            lightObject.transform.rotation = Quaternion.Euler(50f, -30f, 0f);

            // Add to dynamic lights list
            dynamicLights.Add(directionalLight);
        }
    }


    /// Sets up particle effect systems

    private void SetupParticleEffects()
    {
        if (!enableParticleEffects) return;

        // Create particle pool
        for (int i = 0; i < maxParticleSystems; i++)
        {
            GameObject particleObject = new GameObject($"ParticleEffect_{i}");
            particleObject.transform.parent = transform;

            ParticleSystem particles = particleObject.AddComponent<ParticleSystem>();
            ConfigureParticleSystem(particles, ParticleEffectType.Ambient);

            particleObject.SetActive(false);
            particlePool.Enqueue(particles);
        }
    }


    /// Configures a particle system based on effect type

    private void ConfigureParticleSystem(ParticleSystem particles, ParticleEffectType effectType)
    {
        var main = particles.main;
        var emission = particles.emission;
        var shape = particles.shape;
        var velocityOverLifetime = particles.velocityOverLifetime;

        switch (effectType)
        {
            case ParticleEffectType.Ambient:
                main.startLifetime = 10f;
                main.startSpeed = 0.5f;
                main.startSize = 0.1f;
                main.startColor = new Color(1f, 1f, 1f, 0.3f);
                main.maxParticles = 50;

                emission.rateOverTime = 5f;

                shape.enabled = true;
                shape.shapeType = ParticleSystemShapeType.Box;
                shape.scale = new Vector3(20f, 10f, 1f);

                velocityOverLifetime.enabled = true;
                velocityOverLifetime.space = ParticleSystemSimulationSpace.Local;
                velocityOverLifetime.x = new ParticleSystem.MinMaxCurve(-0.5f, 0.5f);
                velocityOverLifetime.y = new ParticleSystem.MinMaxCurve(-0.2f, 0.2f);
                break;

            case ParticleEffectType.Wind:
                main.startLifetime = 5f;
                main.startSpeed = 2f;
                main.startSize = 0.05f;
                main.startColor = new Color(0.8f, 0.9f, 1f, 0.2f);
                main.maxParticles = 30;

                emission.rateOverTime = 6f;

                velocityOverLifetime.enabled = true;
                velocityOverLifetime.space = ParticleSystemSimulationSpace.Local;
                velocityOverLifetime.x = new ParticleSystem.MinMaxCurve(windSpeed * windDirection.x);
                velocityOverLifetime.y = new ParticleSystem.MinMaxCurve(windSpeed * windDirection.y);
                break;

            case ParticleEffectType.Sparkle:
                main.startLifetime = 3f;
                main.startSpeed = 1f;
                main.startSize = 0.2f;
                main.startColor = new Color(1f, 1f, 0.5f, 0.8f);
                main.maxParticles = 20;

                emission.rateOverTime = 2f;
                break;
        }
    }


    /// Creates environmental effects

    private void CreateEnvironmentalEffects()
    {
        if (!enableEnvironmentalEffects) return;

        // Create ambient particle effects
        SpawnParticleEffect(ParticleEffectType.Ambient, Vector3.zero);
        SpawnParticleEffect(ParticleEffectType.Wind, Vector3.zero);
    }


    /// Updates parallax layer positions

    private void UpdateParallaxLayers()
    {
        if (!enableParallax || mainCamera == null) return;

        Vector3 cameraMovement = mainCamera.transform.position - lastCameraPosition;
        lastCameraPosition = mainCamera.transform.position;

        // Update each parallax behavior component
        ParallaxBehavior[] parallaxBehaviors = FindObjectsOfType<ParallaxBehavior>();
        foreach (ParallaxBehavior behavior in parallaxBehaviors)
        {
            behavior.UpdateParallax();
        }
    }


    /// Updates dynamic lighting effects

    private void UpdateDynamicLighting()
    {
        if (!enableDynamicLighting) return;

        lightAnimationTime += Time.deltaTime * 0.5f;
        float lightVariation = lightVariationCurve.Evaluate(Mathf.PingPong(lightAnimationTime, 1f));

        // Update all dynamic lights
        foreach (Light light in dynamicLights)
        {
            if (light != null)
            {
                light.intensity = lightIntensity * lightVariation;
            }
        }

        // Update directional light if available
        if (directionalLight != null)
        {
            directionalLight.intensity = lightIntensity * lightVariation;
        }
    }


    /// Updates particle effects

    private void UpdateParticleEffects()
    {
        if (!enableParticleEffects || mainCamera == null) return;

        // Update particle positions relative to camera
        foreach (ParticleSystem particles in activeParticleSystems)
        {
            if (particles != null && particles.gameObject.activeInHierarchy)
            {
                // Keep ambient particles around camera
                Vector3 cameraPos = mainCamera.transform.position;
                cameraPos.z = particles.transform.position.z;
                particles.transform.position = cameraPos;
            }
        }
    }


    /// Updates culling for performance

    private void UpdateCulling()
    {
        if (!enableCulling || mainCamera == null) return;

        visibleBounds = new Bounds(mainCamera.transform.position, Vector3.one * cullingDistance);

        // Cull background objects that are too far
        foreach (GameObject bgObject in backgroundObjects)
        {
            if (bgObject != null)
            {
                bool shouldBeVisible = visibleBounds.Contains(bgObject.transform.position);
                if (bgObject.activeInHierarchy != shouldBeVisible)
                {
                    bgObject.SetActive(shouldBeVisible);
                }
            }
        }
    }


    /// Spawns a particle effect at the specified position

    public ParticleSystem SpawnParticleEffect(ParticleEffectType effectType, Vector3 position)
    {
        if (particlePool.Count == 0) return null;

        ParticleSystem particles = particlePool.Dequeue();
        particles.transform.position = position;
        particles.gameObject.SetActive(true);

        ConfigureParticleSystem(particles, effectType);
        particles.Play();

        activeParticleSystems.Add(particles);

        // Auto-return to pool after lifetime
        StartCoroutine(ReturnParticleToPool(particles, particles.main.startLifetime.constant + 1f));

        return particles;
    }


    /// Returns a particle system to the pool after delay

    private IEnumerator ReturnParticleToPool(ParticleSystem particles, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (particles != null)
        {
            particles.Stop();
            particles.gameObject.SetActive(false);
            activeParticleSystems.Remove(particles);
            particlePool.Enqueue(particles);
        }
    }


    /// Triggers a special environmental effect

    public void TriggerSpecialEffect(Vector3 position, ParticleEffectType effectType = ParticleEffectType.Sparkle)
    {
        SpawnParticleEffect(effectType, position);
    }


    /// Sets the wind parameters

    public void SetWind(Vector2 direction, float speed)
    {
        windDirection = direction.normalized;
        windSpeed = speed;

        // Update existing wind particles
        foreach (ParticleSystem particles in activeParticleSystems)
        {
            if (particles != null && particles.name.Contains("Wind"))
            {
                var velocityOverLifetime = particles.velocityOverLifetime;
                velocityOverLifetime.space = ParticleSystemSimulationSpace.Local;
                velocityOverLifetime.x = new ParticleSystem.MinMaxCurve(windSpeed * windDirection.x);
                velocityOverLifetime.y = new ParticleSystem.MinMaxCurve(windSpeed * windDirection.y);
            }
        }
    }


    /// Gets the current visual bounds

    public Bounds GetVisualBounds()
    {
        return visibleBounds;
    }

    private void OnDrawGizmos()
    {
        // Draw visual bounds
        if (enableCulling)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(visibleBounds.center, visibleBounds.size);
        }

        // Draw wind direction
        if (enableEnvironmentalEffects)
        {
            Gizmos.color = Color.cyan;
            Vector3 windVector = new Vector3(windDirection.x, windDirection.y, 0) * windSpeed;
            Gizmos.DrawRay(transform.position, windVector);
        }
    }
}


/// Data structure for parallax layer configuration

[System.Serializable]
public struct ParallaxLayer
{
    public string name;
    public float parallaxSpeed;
    public Color color;
    public int sortingOrder;
    public Vector2 scale;
}


/// Types of particle effects available

public enum ParticleEffectType
{
    Ambient,
    Wind,
    Sparkle,
    Explosion,
    Trail
}


/// Component that handles parallax scrolling behavior

public class ParallaxBehavior : MonoBehaviour
{
    private float parallaxSpeed;
    private Camera targetCamera;
    private Vector3 lastCameraPosition;

    public void Initialize(float speed, Camera camera)
    {
        parallaxSpeed = speed;
        targetCamera = camera;

        if (targetCamera != null)
        {
            lastCameraPosition = targetCamera.transform.position;
        }
    }

    public void UpdateParallax()
    {
        if (targetCamera == null) return;

        Vector3 cameraMovement = targetCamera.transform.position - lastCameraPosition;

        // Apply parallax movement
        Vector3 parallaxMovement = cameraMovement * parallaxSpeed;
        transform.position += new Vector3(parallaxMovement.x, parallaxMovement.y, 0);

        lastCameraPosition = targetCamera.transform.position;
    }
}