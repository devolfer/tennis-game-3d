using System;
using System.Collections;
using CustomUtilities;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class AnimationOperation {
    public GameObject targetObject;
    public UIAnimationType type;
    public EaseType easing;
    public float delay;
    public float duration;

    public bool activate;
    public ScaleSettings scaleSettings;
    public RotateSettings rotateSettings;
    public WorldPositionSettings worldPositionSettings;
    public AnchoredPositionSettings anchoredPositionSettings;
    public ColourSettings colourSettings;
    public FadeSettings fadeSettings;

    private IEnumerator animationRoutine;

    private float lerpDuration;
    private Action lerpStartAction;
    private Action<float> lerpLoopAction;
    private Action lerpEndAction;

    public AnimationOperation() { }

    public AnimationOperation(GameObject target, UIAnimationType animationType, EaseType easeType, float startDelay, float animationDuration) {
        targetObject = target;
        type = animationType;
        easing = easeType;
        delay = startDelay;
        duration = animationDuration;
    }

    public AnimationOperation(AnimationOperation animationOperation) {
        targetObject = animationOperation.targetObject;
        type = animationOperation.type;
        easing = animationOperation.easing;
        delay = animationOperation.delay;
        duration = animationOperation.duration;

        activate = animationOperation.activate;
        scaleSettings = animationOperation.scaleSettings;
        rotateSettings = animationOperation.rotateSettings;
        worldPositionSettings = animationOperation.worldPositionSettings;
        anchoredPositionSettings = animationOperation.anchoredPositionSettings;
        colourSettings = animationOperation.colourSettings;
        fadeSettings = animationOperation.fadeSettings;
    }

    public void PlayWithDelegate(Action<IEnumerator> delegateForPlay, bool unscaledTime = false) {
        PrepareAnimationAction();
        delegateForPlay?.Invoke(Utility.LerpRoutine(lerpDuration, lerpStartAction, lerpLoopAction, lerpEndAction, unscaledTime));
    }

    private void ReverseSettings() {
        activate = true;
        scaleSettings = scaleSettings.Reversed();
        rotateSettings = rotateSettings.Reversed();
        worldPositionSettings = worldPositionSettings.Reversed();
        anchoredPositionSettings = anchoredPositionSettings.Reversed();
        colourSettings = colourSettings.Reversed();
        fadeSettings = fadeSettings.Reversed();
    }

    public AnimationOperation Reversed() {
        AnimationOperation operation = new AnimationOperation(this);
        operation.ReverseSettings();

        return operation;
    }

    private void PrepareAnimationAction() {
        if (!targetObject) return;

        Func<float, float> easeFunc = Easing.GetEaseFunc(easing);

        switch (type) {
            case UIAnimationType.Activate:
                lerpDuration = 0;
                lerpStartAction = null;
                lerpLoopAction = null;
                lerpEndAction = () => {
                    if (targetObject) targetObject.SetActive(activate);
                };

                break;
            case UIAnimationType.Scale:
                Vector3 startScale = scaleSettings.startScale;
                Vector3 targetScale = scaleSettings.targetScale;

                if (easeFunc == null) {
                    lerpDuration = 0;
                    lerpStartAction = null;
                    lerpLoopAction = null;
                    lerpEndAction = () => {
                        if (targetObject) targetObject.transform.localScale = targetScale;
                    };
                } else {
                    lerpDuration = duration;
                    lerpStartAction = () => {
                        if (targetObject) targetObject.transform.localScale = startScale;
                    };
                    lerpLoopAction = t => {
                        if (targetObject) targetObject.transform.localScale = Lerp.Value(startScale, targetScale, t, easeFunc);
                    };
                    lerpEndAction = () => {
                        if (targetObject) targetObject.transform.localScale = targetScale;
                    };
                }

                break;
            case UIAnimationType.Rotate:
                Quaternion startRotation = Quaternion.Euler(rotateSettings.startEuler);
                Quaternion targetRotation = Quaternion.Euler(rotateSettings.targetEuler);

                if (easeFunc == null) {
                    lerpDuration = 0;
                    lerpStartAction = null;
                    lerpLoopAction = null;
                    lerpEndAction = () => {
                        if (targetObject) targetObject.transform.rotation = targetRotation;
                    };
                } else {
                    lerpDuration = duration;
                    lerpStartAction = () => {
                        if (targetObject) targetObject.transform.rotation = startRotation;
                    };
                    lerpLoopAction = t => {
                        if (targetObject) targetObject.transform.rotation = Lerp.Value(startRotation, targetRotation, t, easeFunc);
                    };
                    lerpEndAction = () => {
                        if (targetObject) targetObject.transform.rotation = targetRotation;
                    };
                }

                break;
            case UIAnimationType.WorldPosition:
                Vector3 startPosition = worldPositionSettings.startPosition;
                Vector3 targetPosition = worldPositionSettings.targetPosition;

                if (easeFunc == null) {
                    lerpDuration = 0;
                    lerpStartAction = null;
                    lerpLoopAction = null;
                    lerpEndAction = () => {
                        if (targetObject) targetObject.transform.localPosition = targetPosition;
                    };
                } else {
                    lerpDuration = duration;
                    lerpStartAction = () => {
                        if (targetObject) targetObject.transform.localPosition = startPosition;
                    };
                    lerpLoopAction = t => {
                        if (targetObject) targetObject.transform.localPosition = Lerp.Value(startPosition, targetPosition, t, easeFunc);
                    };
                    lerpEndAction = () => {
                        if (targetObject) targetObject.transform.localPosition = targetPosition;
                    };
                }

                break;
            case UIAnimationType.AnchoredPosition:
                RectTransform rectTransform = targetObject.transform as RectTransform;
                if (rectTransform == null) return;

                Vector2 startMin = anchoredPositionSettings.startMin;
                Vector2 targetMin = anchoredPositionSettings.targetMin;
                Vector2 startMax = anchoredPositionSettings.startMax;
                Vector2 targetMax = anchoredPositionSettings.targetMax;

                if (easeFunc == null) {
                    lerpDuration = 0;
                    lerpStartAction = null;
                    lerpLoopAction = null;
                    lerpEndAction = () => {
                        if (targetObject) rectTransform.anchorMin = targetMin;
                        if (targetObject) rectTransform.anchorMax = targetMax;
                    };
                } else {
                    lerpDuration = duration;
                    lerpStartAction = () => {
                        if (targetObject) rectTransform.anchorMin = startMin;
                        if (targetObject) rectTransform.anchorMax = startMax;
                    };
                    lerpLoopAction = t => {
                        if (targetObject) rectTransform.anchorMin = Lerp.Value(startMin, targetMin, t, easeFunc);
                        if (targetObject) rectTransform.anchorMax = Lerp.Value(startMax, targetMax, t, easeFunc);
                    };
                    lerpEndAction = () => {
                        if (targetObject) rectTransform.anchorMin = targetMin;
                        if (targetObject) rectTransform.anchorMax = targetMax;
                    };
                }

                break;
            case UIAnimationType.Colour:
                if (!targetObject.TryGetComponent(out Image targetImage)) return;

                Color startColour = colourSettings.startColour;
                Color targetColour = colourSettings.targetColour;

                if (easeFunc == null) {
                    lerpDuration = 0;
                    lerpStartAction = null;
                    lerpLoopAction = null;
                    lerpEndAction = () => {
                        if (targetObject) targetImage.color = targetColour;
                    };
                } else {
                    lerpDuration = duration;
                    lerpStartAction = () => {
                        if (targetObject) targetImage.color = startColour;
                    };
                    lerpLoopAction = t => {
                        if (targetObject) targetImage.color = Lerp.Value(startColour, targetColour, t, easeFunc);
                    };
                    lerpEndAction = () => {
                        if (targetObject) targetImage.color = targetColour;
                    };
                }

                break;
            case UIAnimationType.Fade:
                if (!targetObject.TryGetComponent(out CanvasGroup targetGameObjectCanvasGroup)) return;

                float startAlpha = fadeSettings.startAlpha;
                float targetAlpha = fadeSettings.targetAlpha;

                if (easeFunc == null) {
                    lerpDuration = 0;
                    lerpStartAction = null;
                    lerpLoopAction = null;
                    lerpEndAction = () => {
                        if (targetObject) targetGameObjectCanvasGroup.alpha = targetAlpha;
                    };
                } else {
                    lerpDuration = duration;
                    lerpStartAction = () => {
                        if (targetObject) targetGameObjectCanvasGroup.alpha = startAlpha;
                    };
                    lerpLoopAction = t => {
                        if (targetObject) targetGameObjectCanvasGroup.alpha = Lerp.Value(startAlpha, targetAlpha, t, easeFunc);
                    };
                    lerpEndAction = () => {
                        if (targetObject) targetGameObjectCanvasGroup.alpha = targetAlpha;
                    };
                }

                break;
            default:
                return;
        }
    }

    private Action ToAction(Action<IEnumerator> playDelegate) {
        if (!targetObject) return null;

        Func<float, float> easeFunc = Easing.GetEaseFunc(easing);

        return type switch {
            UIAnimationType.Activate => () => {
                targetObject.SetActive(activate);
            },
            UIAnimationType.Scale => () => {
                Vector3 startScale = scaleSettings.startScale;
                Vector3 targetScale = scaleSettings.targetScale;

                if (easeFunc == null) {
                    targetObject.transform.localScale = targetScale;
                } else {
                    animationRoutine = Utility.LerpRoutine(duration, null, t => {
                        targetObject.transform.localScale = Lerp.Value(startScale, targetScale, t, easeFunc);
                    });

                    playDelegate?.Invoke(animationRoutine);
                }
            },
            UIAnimationType.Rotate => () => {
                Quaternion startRotation = Quaternion.Euler(rotateSettings.startEuler);
                Quaternion targetRotation = Quaternion.Euler(rotateSettings.targetEuler);

                if (easeFunc == null) {
                    targetObject.transform.rotation = targetRotation;
                } else {
                    animationRoutine = Utility.LerpRoutine(duration, null, t => {
                        targetObject.transform.rotation = Lerp.Value(startRotation, targetRotation, t, easeFunc);
                    });

                    playDelegate?.Invoke(animationRoutine);
                }
            },
            UIAnimationType.WorldPosition => () => {
                Vector3 startPosition = worldPositionSettings.startPosition;
                Vector3 targetPosition = worldPositionSettings.targetPosition;

                if (easeFunc == null) {
                    targetObject.transform.localPosition = targetPosition;
                } else {
                    animationRoutine = Utility.LerpRoutine(duration, null, t => {
                        targetObject.transform.localPosition = Lerp.Value(startPosition, targetPosition, t, easeFunc);
                    });

                    playDelegate?.Invoke(animationRoutine);
                }
            },
            UIAnimationType.AnchoredPosition => () => {
                RectTransform rectTransform = targetObject.transform as RectTransform;
                if (rectTransform == null) return;

                Vector2 startMin = anchoredPositionSettings.startMin;
                Vector2 targetMin = anchoredPositionSettings.targetMin;
                Vector2 startMax = anchoredPositionSettings.startMax;
                Vector2 targetMax = anchoredPositionSettings.targetMax;

                if (easeFunc == null) {
                    rectTransform.anchorMin = targetMin;
                    rectTransform.anchorMax = targetMax;
                } else {
                    animationRoutine = Utility.LerpRoutine(duration, null, t => {
                        rectTransform.anchorMin = Lerp.Value(startMin, targetMin, t, easeFunc);
                        rectTransform.anchorMax = Lerp.Value(startMax, targetMax, t, easeFunc);
                    });

                    playDelegate?.Invoke(animationRoutine);
                }
            },
            UIAnimationType.Colour => () => {
                if (!targetObject.TryGetComponent(out Image targetImage)) return;

                Color startColour = colourSettings.startColour;
                Color targetColour = colourSettings.targetColour;

                if (easeFunc == null) {
                    targetImage.color = targetColour;
                } else {
                    animationRoutine = Utility.LerpRoutine(duration, null, t => {
                        targetImage.color = Lerp.Value(startColour, targetColour, t, easeFunc);
                    });

                    playDelegate?.Invoke(animationRoutine);
                }
            },
            UIAnimationType.Fade => () => {
                if (!targetObject.TryGetComponent(out CanvasGroup targetGameObjectCanvasGroup)) return;

                float startAlpha = fadeSettings.startAlpha;
                float targetAlpha = fadeSettings.targetAlpha;

                if (easeFunc == null) {
                    targetGameObjectCanvasGroup.alpha = targetAlpha;
                } else {
                    animationRoutine = Utility.LerpRoutine(duration, null, t => {
                        targetGameObjectCanvasGroup.alpha = Lerp.Value(startAlpha, targetAlpha, t, easeFunc);
                    });

                    playDelegate?.Invoke(animationRoutine);
                }
            },
            _ => null
        };
    }

    [Serializable]
    public struct ScaleSettings {
        public Vector3 startScale;
        public Vector3 targetScale;

        public ScaleSettings Reversed() {
            Vector3 tmp = startScale;

            return new ScaleSettings {
                startScale = targetScale,
                targetScale = tmp
            };
        }
    }

    [Serializable]
    public struct RotateSettings {
        public Vector3 startEuler;
        public Vector3 targetEuler;

        public RotateSettings Reversed() {
            Vector3 tmp = startEuler;

            return new RotateSettings {
                startEuler = targetEuler,
                targetEuler = tmp
            };
        }
    }

    [Serializable]
    public struct WorldPositionSettings {
        public Vector3 startPosition;
        public Vector3 targetPosition;

        public WorldPositionSettings Reversed() {
            Vector3 tmp = startPosition;

            return new WorldPositionSettings {
                startPosition = targetPosition,
                targetPosition = tmp
            };
        }
    }

    [Serializable]
    public struct AnchoredPositionSettings {
        public Vector2 startMin;
        public Vector2 targetMin;
        public Vector2 startMax;
        public Vector2 targetMax;

        public AnchoredPositionSettings Reversed() {
            Vector2 tmpMin = startMin;
            Vector2 tmpMax = startMax;

            return new AnchoredPositionSettings {
                startMin = targetMin,
                targetMin = tmpMin,
                startMax = targetMax,
                targetMax = tmpMax
            };
        }
    }

    [Serializable]
    public struct ColourSettings {
        public Color startColour;
        public Color targetColour;

        public ColourSettings Reversed() {
            Color tmp = startColour;

            return new ColourSettings {
                startColour = targetColour,
                targetColour = tmp
            };
        }
    }

    [Serializable]
    public struct FadeSettings {
        public float startAlpha;
        public float targetAlpha;

        public FadeSettings Reversed() {
            float tmp = startAlpha;

            return new FadeSettings {
                startAlpha = targetAlpha,
                targetAlpha = tmp
            };
        }
    }
}

public enum UIAnimationType {
    Activate,
    Scale,
    Rotate,
    WorldPosition,
    AnchoredPosition,
    Colour,
    Fade
}