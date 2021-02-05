using System;
using CustomUtilities;
using UnityEngine;
using UnityEngine.UI;

public class ShotSessionProgressDisplay : MonoBehaviour {
    [SerializeField] private ShotSessionEventRelay shotSessionEventRelay;
    [SerializeField] private GameObject panel;
    [SerializeField] private Image fillImage;

    [SerializeField] private float fillDuration;

    private float currentPercentage = 0;
    
    // for UI normalizing
    private const float MinRange = 0.2f;
    private const float MaxRange = 0.8f;
    private const float MinValue = 0f;
    private const float MaxValue = 1f;

    private void OnEnable() {
        shotSessionEventRelay.OnProgressChanged += ChangeProgress;
    }

    private void OnDisable() {
        shotSessionEventRelay.OnProgressChanged -= ChangeProgress;
    }

    private void Start() {
        SetProgress(0, false);
    }

    private void SetProgress(float percentage, bool animated) {
        float normalizedValue = NormalizeInRange(MinRange, MaxRange, MinValue, MaxValue, percentage);
        
        // Debug.Log($"Passed -> {percentage}");
        // Debug.Log($"Normalized -> {normalizedValue}");
        
        if (animated) {
            float startValue = currentPercentage;
            float targetValue = normalizedValue;

            this.DoRoutine(fillDuration, null, t => {
                fillImage.fillAmount = Lerp.Value(startValue, targetValue, t, Easing.SmoothStep.Smoother);
            }, () => {
                fillImage.fillAmount = targetValue;
            });
        } else {
            fillImage.fillAmount = normalizedValue;
        }

        currentPercentage = normalizedValue;
    }

    private void ChangeProgress(float percentage) {
        SetProgress(percentage, true);
    }

    private float NormalizeInRange(float minRange, float maxRange, float minValue, float maxValue, float value) {
        return (maxRange - minRange) * ((value - minValue) / (maxValue - minValue)) + minRange;
    }
}