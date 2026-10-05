using UnityEngine;
using UnityEngine.Rendering;

//TODO: Выкинуть эту херь и написать нормальный шейдер
public class UnderwaterVisual : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private ParticleSystem underwaterParticles;

    [Header("Settings")]
    [SerializeField] private float waterSurfaceY;
    [SerializeField] private float fadeSpeed = 4f;

    [Header("Fog")]
    [SerializeField] private Color underwaterFogColor = new Color(0.0f, 0.3f, 0.55f);
    [SerializeField] private float underwaterFogDensity = 0.15f;

    [Header("Camera Background")]
    [SerializeField] private Color underwaterBackgroundColor = new Color(0, 0.3f, 0.55f);

    private Volume underwaterVolume;

    private Color defaultFogColor;
    private float defaultFogDensity;
    private FogMode defaultFogMode;
    private bool defaultFogEnabled;

    private CameraClearFlags defaultClearFlags;
    private Color defaultBackgroundColor;

    private bool wasUnderwater;

    private void Start()
    {
        if (playerCamera == null) playerCamera = GetComponent<Camera>();

        underwaterVolume = FindAnyObjectByType<Volume>();
        if (underwaterVolume != null)
            underwaterVolume.weight = 0f;

        defaultFogColor = RenderSettings.fogColor;
        defaultFogDensity = RenderSettings.fogDensity;
        defaultFogMode = RenderSettings.fogMode;
        defaultFogEnabled = RenderSettings.fog;

        defaultClearFlags = playerCamera.clearFlags;
        defaultBackgroundColor = playerCamera.backgroundColor;

        if (underwaterParticles != null)
            underwaterParticles.Stop();
    }

    private void Update()
    {
        if (playerCamera == null) return;

        var underwater = playerCamera.transform.position.y < waterSurfaceY;

        if (underwaterVolume != null)
        {
            float targetWeight = underwater ? 1f : 0f;
            underwaterVolume.weight = Mathf.MoveTowards(
                underwaterVolume.weight, targetWeight, fadeSpeed * Time.deltaTime);
        }

        if (underwater)
        {
            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.ExponentialSquared;
            RenderSettings.fogColor = underwaterFogColor;
            RenderSettings.fogDensity = underwaterFogDensity;

            playerCamera.clearFlags = CameraClearFlags.SolidColor;
            playerCamera.backgroundColor = underwaterBackgroundColor;
        }
        else
        {
            RenderSettings.fog = defaultFogEnabled;
            RenderSettings.fogMode = defaultFogMode;
            RenderSettings.fogColor = defaultFogColor;
            RenderSettings.fogDensity = defaultFogDensity;

            playerCamera.clearFlags = defaultClearFlags;
            playerCamera.backgroundColor = defaultBackgroundColor;
        }

        if (underwater != wasUnderwater && underwaterParticles != null)
        {
            if (underwater) underwaterParticles.Play();
            else underwaterParticles.Stop();
        }
        wasUnderwater = underwater;
    }
}