using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using System.Collections;

public class SprintController : MonoBehaviour
{
    [Header("HDRP Global Volume Settings")]
    public Volume globalVolume; // Reference to the global post-processing volume
    private LensDistortion lensDistortion;
    private ScreenSpaceLensFlare sslf;
    private MotionBlur motionBlur;

    // Intensity values for post-processing effects
    private float LD_Intensity = -0f;
    private float SSLF_Intensity = 0f;
    private float MB_Intensity = 0;

    public float lensDistortionIntensity = -0.2f;
    public float sslfIntensity = 1f;
    public float motionBlurIntensity = 20f;

    // Toggle states for post-processing effects
    public bool lensDistortionToggle = true;
    public bool sslfToggle = true;
    public bool motionBlurToggle = true;

    [Header("ShaderGraph Settings")]
    public Material shaderMaterial; // Reference to the shader material
    private float alpha = 0f;
    public float shaderAlpha = 0.4f;

    [Header("Animation Settings")]
    public float transitionSpeed = 1f; // Speed of effect transitions
    private bool isSprinting = false; // State check if sprinting
    private bool canSprint = true; // Controls cooldown period
    private int RunningState = 0; // 0 = idle, 1 = sprinting, 2 = stopping sprint

    private void Start()
    {
        // Retrieve post-processing effects from the global volume
        if (globalVolume != null && globalVolume.profile != null)
        {
            globalVolume.profile.TryGet(out lensDistortion);
            globalVolume.profile.TryGet(out sslf);
            globalVolume.profile.TryGet(out motionBlur);
        }
    }

    private void Update()
    {
        // Update HDRP Global Volume settings
        if (lensDistortion != null)
        {
            lensDistortion.active = lensDistortionToggle;
            lensDistortion.intensity.value = lensDistortionToggle ? LD_Intensity : 0;
        }

        if (sslf != null)
        {
            sslf.active = sslfToggle;
            sslf.intensity.value = sslfToggle ? SSLF_Intensity : 0;
        }

        if (motionBlur != null)
        {
            motionBlur.active = motionBlurToggle;
            motionBlur.intensity.value = motionBlurToggle ? MB_Intensity : 0;
        }

        // Update ShaderGraph property for transparency effect
        if (shaderMaterial != null)
        {
            shaderMaterial.SetFloat("_Alpha", alpha);
        }

        // Handle Sprint effect transitions
        if (isSprinting && RunningState == 1)
        {
            LD_Intensity = Mathf.Lerp(LD_Intensity, lensDistortionIntensity, Time.deltaTime * transitionSpeed);
            SSLF_Intensity = Mathf.Lerp(SSLF_Intensity, sslfIntensity, Time.deltaTime * transitionSpeed);
            alpha = Mathf.Lerp(alpha, shaderAlpha, Time.deltaTime * transitionSpeed);
        }
        else if (RunningState == 2)
        {
            LD_Intensity = Mathf.Lerp(LD_Intensity, 0, Time.deltaTime * transitionSpeed);
            SSLF_Intensity = Mathf.Lerp(SSLF_Intensity, 0, Time.deltaTime * transitionSpeed);
            alpha = Mathf.Lerp(alpha, 0, Time.deltaTime * transitionSpeed);
        }
    }

    public void SprintOn()
    {
        RunningState = 1; // Set running state to sprinting
        //if (canSprint)
        {
            // Activate all sprint effects
            lensDistortionToggle = true;
            sslfToggle = true;
            motionBlurToggle = true;
            isSprinting = true;
        }
    }

    public void SprintOff()
    {
        RunningState = 2; // Set running state to stopping sprint
        StartCoroutine(SprintOffRoutine()); // Start cooldown before disabling effects
    }

    private IEnumerator SprintOffRoutine()
    {
        isSprinting = false; // Mark sprint as inactive
        canSprint = false; // Prevent sprint reactivation immediately
        print("start of cooldown");
        yield return new WaitForSeconds(0.5f); // Cooldown period before sprint can be used again
        canSprint = true;
        print("end of cooldown");
        
        // Disable all sprint effects
        lensDistortionToggle = false;
        sslfToggle = false;
        motionBlurToggle = false;
        RunningState = 0; // Reset sprint state
        yield return null;
    }
}
