using System;
using CustomUtilities;
using UnityEngine;

public class ShotZoneHighlighter : MonoBehaviour {
    [SerializeField] private Transform zoneTransform;
    [SerializeField] private Renderer zoneRenderer;
    [SerializeField] private Color anyShotColour;
    [SerializeField] private Color flatShotColour;
    [SerializeField] private Color topSpinShotColour;
    [SerializeField] private Color sliceShotColour;
    [SerializeField] private Color dropOrLobShotColour;
    [SerializeField] private Color volleyShotColour;

    public event Action<TennisBall> OnBallInShotZone;

    private void OnTriggerEnter(Collider other) {
        if (!other.TryGetComponent(out TennisBall ball)) return;

        OnBallInShotZone?.Invoke(ball);
    }

    private void ColourZone(Color colourValue, bool animated = false, float animationDuration = 0f) {
        if (animated) {
            Color startColour = zoneRenderer.material.color;

            this.DoRoutine(animationDuration, null, t => {
                zoneRenderer.material.color = Lerp.Value(startColour, colourValue, t, Easing.SmoothStep.Smoother);
            });
        } else {
            zoneRenderer.material.color = colourValue;
        }
    }

    public void ColourZone(ShotType shotType, bool animated = false, float animationDuration = 0f) {
        Color colourValue = shotType switch {
            ShotType.Any => anyShotColour,
            ShotType.Flat => flatShotColour,
            ShotType.TopSpin => topSpinShotColour,
            ShotType.Slice => sliceShotColour,
            ShotType.DropOrLob => dropOrLobShotColour,
            ShotType.Volley => volleyShotColour,
            _ => Color.white
        };

        ColourZone(colourValue, animated, animationDuration);
    }

    private void ScaleAndPositionZone(Vector3 targetScale, Vector3 targetPosition, bool animated = false, float animationDuration = 0f) {
        if (animated) {
            Vector3 startingScale = zoneTransform.localScale;
            Vector3 startingPosition = zoneTransform.position;

            this.DoRoutine(animationDuration, null, t => {
                zoneTransform.localScale = Lerp.Value(startingScale, targetScale, t, Easing.SmoothStep.Smoother);
                zoneTransform.localPosition = Lerp.Value(startingPosition, targetPosition, t, Easing.SmoothStep.Smoother);
            });
        } else {
            zoneTransform.localScale = targetScale;
            zoneTransform.localPosition = targetPosition;
        }
    }

    public void ScaleAndPositionZone(CourtZone courtZone, bool animated = false, float animationDuration = 0f) {
        Vector3 targetScale = courtZone.MaxBounds - courtZone.MinBounds;
        Vector3 targetPosition = (courtZone.MaxBounds + courtZone.MinBounds) / 2f;

        ScaleAndPositionZone(targetScale, targetPosition, animated, animationDuration);
    }
}