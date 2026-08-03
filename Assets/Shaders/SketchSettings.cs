using UnityEngine;
using UnityEngine.Rendering;

[System.Serializable, VolumeComponentMenu("ARC-TYPE/Sketch"), DisplayInfo(name = "Sketch")]
public class SketchSettings : VolumeComponent, IPostProcessComponent
{
    [Tooltip("Texture to use for the sketch pattern.")]
    public TextureParameter sketchTexture = new(null);

    [Tooltip("Color used to tint the sketch texture.")]
    public ColorParameter sketchColor = new(Color.black);

    [Tooltip("How much the sketch texture should be tiled in each direction.")]
    public Vector2Parameter sketchTiling = new(Vector2.one);

    [Tooltip("First value = shadow value where sketches start.\n +" +
        "Second value = shadow value where sketches are at full opacity.")]
    public Vector2Parameter sketchThresholds = new(new Vector2(0.0f, 0.1f));

    [Tooltip("Controls whether to sample the sketch texture twice.")]
    public BoolParameter crossHatching = new(false);

    [Tooltip("How strongly the shadow map is blurred. Higher values mean the sketches extend " +
        "further outside the shadowed regions.")]
    public ClampedIntParameter blurAmount = new(3, 3, 500);

    [Tooltip("Higher values will skip pixels during blur passes. Increase for better performance.")]
    public ClampedIntParameter blurStepSize = new(1, 1, 16);

    [Tooltip("Sensitivity of the function which prevents sketches appearing improperly on " +
        "some objects.")]
    public ClampedFloatParameter extendedDepthSensitivity = new(0.002f, 0.0001f, 0.01f);

    public bool IsActive()
    {
        return sketchTexture.value != null && active;
    }

    public bool IsTileCompatible()
    {
        return false;
    }
}