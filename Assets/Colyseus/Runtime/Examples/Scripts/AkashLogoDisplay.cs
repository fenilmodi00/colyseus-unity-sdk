using UnityEngine;

/// <summary>
/// Manages the Akash logo display in the center of the demo scene.
/// Serves as the visual target for player movement and showcases Akash branding.
/// </summary>
public class AkashLogoDisplay : MonoBehaviour
{
    [Header("Logo Configuration")]
    [SerializeField] private SpriteRenderer logoRenderer;
    [SerializeField] private Vector2 logoPosition = Vector2.zero;
    [SerializeField] private Vector2 logoScale = Vector2.one * 2f;
    
    [Header("Visual Effects")]
    [SerializeField] private bool enablePulsing = true;
    [SerializeField] private float pulseSpeed = 2f;
    [SerializeField] private float pulseAmount = 0.1f;
    
    private Vector2 originalScale;
    private float pulseTimer;

    private void Awake()
    {
        SetupLogoRenderer();
        originalScale = logoScale;
    }

    private void Start()
    {
        PositionLogo();
    }

    private void Update()
    {
        if (enablePulsing && logoRenderer != null)
        {
            ApplyPulseEffect();
        }
    }

    /// <summary>
    /// Sets up the logo sprite renderer component
    /// </summary>
    private void SetupLogoRenderer()
    {
        if (logoRenderer == null)
        {
            logoRenderer = GetComponent<SpriteRenderer>();
            
            if (logoRenderer == null)
            {
                logoRenderer = gameObject.AddComponent<SpriteRenderer>();
            }
        }

        // Create a simple colored square as placeholder for Akash logo
        if (logoRenderer.sprite == null)
        {
            CreatePlaceholderSprite();
        }

        // Set logo appearance
        logoRenderer.color = GetAkashBrandColor();
        logoRenderer.sortingOrder = 1; // Ensure logo appears above other elements
    }

    /// <summary>
    /// Creates a simple placeholder sprite when no logo sprite is available
    /// </summary>
    private void CreatePlaceholderSprite()
    {
        // Create a simple square texture as placeholder
        Texture2D texture = new Texture2D(100, 100);
        Color[] pixels = new Color[100 * 100];
        
        // Fill with Akash brand color
        Color akashColor = GetAkashBrandColor();
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = akashColor;
        }
        
        texture.SetPixels(pixels);
        texture.Apply();
        
        // Create sprite from texture
        Sprite logoSprite = Sprite.Create(texture, new Rect(0, 0, 100, 100), new Vector2(0.5f, 0.5f));
        logoRenderer.sprite = logoSprite;
    }

    /// <summary>
    /// Positions the logo at the center of the scene
    /// </summary>
    private void PositionLogo()
    {
        transform.position = new Vector3(logoPosition.x, logoPosition.y, 0f);
        transform.localScale = new Vector3(logoScale.x, logoScale.y, 1f);
    }

    /// <summary>
    /// Applies a subtle pulsing effect to the logo
    /// </summary>
    private void ApplyPulseEffect()
    {
        pulseTimer += Time.deltaTime * pulseSpeed;
        float scaleFactor = 1f + Mathf.Sin(pulseTimer) * pulseAmount;
        
        Vector2 currentScale = originalScale * scaleFactor;
        transform.localScale = new Vector3(currentScale.x, currentScale.y, 1f);
    }

    /// <summary>
    /// Returns the Akash brand color
    /// </summary>
    /// <returns>Akash brand color (red/orange theme)</returns>
    private Color GetAkashBrandColor()
    {
        // Akash brand color - using a red/orange theme
        return new Color(1f, 0.2f, 0.2f, 1f); // Bright red
    }

    /// <summary>
    /// Sets a custom logo sprite (to be used when actual Akash logo is available)
    /// </summary>
    /// <param name="newLogoSprite">The new logo sprite to display</param>
    public void SetLogoSprite(Sprite newLogoSprite)
    {
        if (logoRenderer != null && newLogoSprite != null)
        {
            logoRenderer.sprite = newLogoSprite;
        }
    }

    /// <summary>
    /// Gets the world position of the logo (useful for player targeting)
    /// </summary>
    /// <returns>World position of the logo</returns>
    public Vector2 GetLogoPosition()
    {
        return transform.position;
    }

    /// <summary>
    /// Enables or disables the pulsing effect
    /// </summary>
    /// <param name="enabled">Whether to enable pulsing</param>
    public void SetPulsingEnabled(bool enabled)
    {
        enablePulsing = enabled;
        
        if (!enabled)
        {
            // Reset to original scale
            transform.localScale = new Vector3(originalScale.x, originalScale.y, 1f);
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Draw logo position in scene view for easy positioning
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(new Vector3(logoPosition.x, logoPosition.y, 0f), 1f);
    }
}