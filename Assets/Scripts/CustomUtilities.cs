using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace CustomUtilities {
    public static class Easing {
        public static float Linear(float t) {
            return t;
        }

        public static class Quadratic {
            public static float In(float x) {
                return x * x;
            }

            public static float Out(float x) {
                return 1f - (1f - x) * (1f - x);
            }

            public static float InOut(float x) {
                return x < 0.5f ? 2f * x * x : 1f - Mathf.Pow(-2f * x + 2f, 2) / 2f;
            }
        }

        public static class Cubic {
            public static float In(float x) {
                return x * x * x;
            }

            public static float Out(float x) {
                return 1f - Mathf.Pow(1f - x, 3);
            }

            public static float InOut(float x) {
                return x < 0.5f ? 4f * x * x * x : 1f - Mathf.Pow(-2f * x + 2f, 3) / 2f;
            }
        }

        public static class Quartic {
            public static float In(float x) {
                return x * x * x * x;
            }

            public static float Out(float x) {
                return 1 - Mathf.Pow(1f - x, 4);
            }

            public static float InOut(float x) {
                return x < 0.5f ? 8f * x * x * x * x : 1f - Mathf.Pow(-2f * x + 2f, 4) / 2f;
            }
        }

        public static class Quintic {
            public static float In(float x) {
                return x * x * x * x * x;
            }

            public static float Out(float x) {
                return 1f - Mathf.Pow(1f - x, 5);
            }

            public static float InOut(float x) {
                return x < 0.5f ? 16f * x * x * x * x * x : 1f - Mathf.Pow(-2f * x + 2f, 5) / 2f;
            }
        }

        public static class Exponential {
            public static float In(float x) {
                return x == 0f ? 0f : Mathf.Pow(2f, 10f * x - 10f);
            }

            public static float Out(float x) {
                return Math.Abs(x - 1f) < 0.01f ? 1f : 1f - Mathf.Pow(2f, -10f * x);
            }

            public static float InOut(float x) {
                return x == 0f ? 0f : Math.Abs(x - 1f) < 0.01f ? 1f : x < 0.5f ? Mathf.Pow(2f, 20f * x - 10f) / 2f : (2f - Mathf.Pow(2f, -20f * x + 10f)) / 2f;
            }
        }

        public static class Sine {
            public static float In(float x) {
                return 1f - Mathf.Cos(x * Mathf.PI / 2f);
            }

            public static float Out(float x) {
                return Mathf.Sin(x * Mathf.PI / 2f);
            }

            public static float InOut(float x) {
                return -(Mathf.Cos(Mathf.PI * x) - 1f) / 2f;
            }
        }

        public static class Circular {
            public static float In(float x) {
                return 1f - Mathf.Sqrt(1f - Mathf.Pow(x, 2));
            }

            public static float Out(float x) {
                return Mathf.Sqrt(1f - Mathf.Pow(x - 1f, 2));
            }

            public static float InOut(float x) {
                return x < 0.5f ? (1f - Mathf.Sqrt(1f - Mathf.Pow(2f * x, 2))) / 2f : (Mathf.Sqrt(1f - Mathf.Pow(-2f * x + 2f, 2)) + 1f) / 2f;
            }
        }

        public static class Elastic {
            public static float In(float x) {
                const float c4 = 2f * Mathf.PI / 3f;

                return x == 0f ? 0f : Math.Abs(x - 1f) < 0.01f ? 1f : -Mathf.Pow(2f, 10f * x - 10f) * Mathf.Sin((x * 10f - 10.75f) * c4);
            }

            public static float Out(float x) {
                const float c4 = 2f * Mathf.PI / 3f;

                return x == 0f ? 0f : Math.Abs(x - 1f) < 0.01f ? 1f : Mathf.Pow(2f, -10f * x) * Mathf.Sin((x * 10f - 0.75f) * c4) + 1f;
            }

            public static float InOut(float x) {
                const float c5 = 2 * Mathf.PI / 4.5f;

                return x == 0f ? 0f : Math.Abs(x - 1f) < 0.01f ? 1f : x < 0.5f ? -(Mathf.Pow(2f, 20f * x - 10f) * Mathf.Sin((20f * x - 11.125f) * c5)) / 2f : Mathf.Pow(2f, -20f * x + 10f) * Mathf.Sin((20f * x - 11.125f) * c5) / 2f + 1f;
            }
        }

        public static class Back {
            public static float In(float x) {
                const float c1 = 1.70158f;
                const float c3 = c1 + 1f;

                return c3 * x * x * x - c1 * x * x;
            }

            public static float Out(float x) {
                const float c1 = 1.70158f;
                const float c3 = c1 + 1f;

                return 1f + c3 * Mathf.Pow(x - 1f, 3) + c1 * Mathf.Pow(x - 1f, 2);
            }

            public static float InOut(float x) {
                const float c1 = 1.70158f;
                const float c2 = c1 * 1.525f;

                return x < 0.5f ? Mathf.Pow(2f * x, 2) * ((c2 + 1f) * 2f * x - c2) / 2f : (Mathf.Pow(2f * x - 2f, 2) * ((c2 + 1f) * (x * 2f - 2f) + c2) + 2f) / 2f;
            }
        }

        public static class Bounce {
            public static float In(float x) {
                return 1f - Out(1f - x);
            }

            public static float Out(float x) {
                const float n1 = 7.5625f;
                const float d1 = 2.75f;

                if (x < 1f / d1) {
                    return n1 * x * x;
                }

                if (x < 2f / d1) {
                    return n1 * (x -= 1.5f / d1) * x + 0.75f;
                }

                if (x < 2.5f / d1) {
                    return n1 * (x -= 2.25f / d1) * x + 0.9375f;
                }

                return n1 * (x -= 2.625f / d1) * x + 0.984375f;
            }

            public static float InOut(float x) {
                return x < 0.5f ? (1f - Out(1f - 2f * x)) / 2f : (1f + Out(2f * x - 1f)) / 2f;
            }
        }

        public static class SmoothStep {
            public static float Smooth(float x) {
                return x * x * (3f - 2f * x);
            }

            public static float Smoother(float x) {
                return x * x * x * (x * (6f * x - 15f) + 10f);
            }

            public static float Smoothest(float x) {
                return -20f * Mathf.Pow(x, 7) + 70 * Mathf.Pow(x, 6) - 84 * Mathf.Pow(x, 5) + 35 * Mathf.Pow(x, 4);
            }
        }

        public static Func<float, float> GetEaseFunc(EaseType easeType) {
            Func<float, float> easeFunc = easeType switch {
                EaseType.None => null,
                EaseType.Linear => Linear,
                EaseType.QuadraticIn => Quadratic.In,
                EaseType.QuadraticOut => Quadratic.Out,
                EaseType.QuadraticInOut => Quadratic.InOut,
                EaseType.CubicIn => Cubic.In,
                EaseType.CubicOut => Cubic.Out,
                EaseType.CubicInOut => Cubic.InOut,
                EaseType.QuarticIn => Quartic.In,
                EaseType.QuarticOut => Quartic.Out,
                EaseType.QuarticInOut => Quartic.InOut,
                EaseType.QuinticIn => Quintic.In,
                EaseType.QuinticOut => Quintic.Out,
                EaseType.QuinticInOut => Quintic.InOut,
                EaseType.ExponentialIn => Exponential.In,
                EaseType.ExponentialOut => Exponential.Out,
                EaseType.ExponentialInOut => Exponential.InOut,
                EaseType.SineIn => Sine.In,
                EaseType.SineOut => Sine.Out,
                EaseType.SineInOut => Sine.InOut,
                EaseType.CircularIn => Circular.In,
                EaseType.CircularOut => Circular.Out,
                EaseType.CircularInOut => Circular.InOut,
                EaseType.ElasticIn => Elastic.In,
                EaseType.ElasticOut => Elastic.Out,
                EaseType.ElasticInOut => Elastic.InOut,
                EaseType.BackIn => Back.In,
                EaseType.BackOut => Back.Out,
                EaseType.BackInOut => Back.InOut,
                EaseType.BounceIn => Bounce.In,
                EaseType.BounceOut => Bounce.Out,
                EaseType.BounceInOut => Bounce.InOut,
                EaseType.SmoothStepSmooth => SmoothStep.Smooth,
                EaseType.SmoothStepSmoother => SmoothStep.Smoother,
                EaseType.SmoothStepSmoothest => SmoothStep.Smoothest,
                _ => null
            };

            return easeFunc;
        }
    }

    public static class Lerp {
        public static float Value(float a, float b, float t, Func<float, float> easeFunc = null) {
            easeFunc ??= Easing.Linear;
            return Mathf.Lerp(a, b, easeFunc(t));
        }

        public static Vector2 Value(Vector2 a, Vector2 b, float t, Func<float, float> easeFunc = null) {
            easeFunc ??= Easing.Linear;
            return Vector2.Lerp(a, b, easeFunc(t));
        }

        public static Vector3 Value(Vector3 a, Vector3 b, float t, Func<float, float> easeFunc = null) {
            easeFunc ??= Easing.Linear;
            return Vector3.Lerp(a, b, easeFunc(t));
        }

        public static Color Value(Color a, Color b, float t, Func<float, float> easeFunc = null) {
            easeFunc ??= Easing.Linear;
            return Color.Lerp(a, b, easeFunc(t));
        }

        public static Quaternion Value(Quaternion a, Quaternion b, float t, Func<float, float> easeFunc = null) {
            easeFunc ??= Easing.Linear;
            return Quaternion.Lerp(a, b, easeFunc(t));
        }
    }

    public static class Slerp {
        public static Vector3 Value(Vector3 a, Vector3 b, float t, Func<float, float> easeFunc = null) {
            easeFunc ??= Easing.Linear;
            return Vector3.Slerp(a, b, easeFunc(t));
        }

        public static Quaternion Value(Quaternion a, Quaternion b, float t, Func<float, float> easeFunc = null) {
            easeFunc ??= Easing.Linear;
            return Quaternion.Slerp(a, b, easeFunc(t));
        }
    }

    public static class Utility {
        public static IEnumerator LerpRoutine(float duration, Action onStartAction = null, Action<float> loopAction = null, Action onEndAction = null, bool unscaledTime = false) {
            onStartAction?.Invoke();

            float timer = 0f;
            float timePercentage = 0;

            if (!unscaledTime) {
                while (timePercentage < 1) {
                    timer += Time.deltaTime;
                    timePercentage = timer / duration;

                    loopAction?.Invoke(timePercentage);

                    yield return null;
                }
            } else {
                while (timePercentage < 1) {
                    timer += Time.unscaledDeltaTime;
                    timePercentage = timer / duration;

                    loopAction?.Invoke(timePercentage);

                    yield return null;
                }
            }

            onEndAction?.Invoke();
        }

        public static IEnumerator SequenceRoutine(Func<float>[] functions, bool unscaledTime = false) {
            if (!unscaledTime) {
                for (int i = 0; i < functions?.Length - 1; i++) {
                    yield return new WaitForSeconds(functions[i]());
                }

                functions?[functions.Length - 1]();
            } else {
                for (int i = 0; i < functions?.Length - 1; i++) {
                    yield return new WaitForSecondsRealtime(functions[i]());
                }

                functions?[functions.Length - 1]();
            }
        }

        public static Queue<T> CreateMonoBehaviourPool<T>(MonoBehaviour behaviour, int amount, Transform parent) where T : MonoBehaviour {
            Queue<T> queue = new Queue<T>();
            for (int i = 0; i < amount; i++) {
                MonoBehaviour b = UnityEngine.Object.Instantiate(behaviour, parent);
                if (!b.TryGetComponent(out T tComponent)) break;
                b.gameObject.SetActive(false);
                queue.Enqueue(tComponent);
            }

            return queue;
        }
        
        public static void SetCursor(bool visible, CursorLockMode lockMode) {
            Cursor.visible = visible;
            Cursor.lockState = lockMode;
        }

        public static Vector2 GetStringDimensions(string contentString, float widthOffset = 0f, float heightOffset = 0f) {
            Vector2 stringDimension = GUI.skin.label.CalcSize(new GUIContent(contentString));

            return new Vector2(stringDimension.x + widthOffset, stringDimension.y + heightOffset);
        }

        public static Vector2 GetStringDimensions(GUIContent guiContent, float widthOffset = 0f, float heightOffset = 0f) {
            Vector2 stringDimension = GUI.skin.label.CalcSize(guiContent);

            return new Vector2(stringDimension.x + widthOffset, stringDimension.y + heightOffset);
        }
    }

    public static class Extensions {
        public static void DoRoutine(this MonoBehaviour monoBehaviour, float duration, Action startAction = null, Action<float> routineAction = null, Action endAction = null, bool unscaledTime = false) {
            monoBehaviour.StartCoroutine(Utility.LerpRoutine(duration, startAction, routineAction, endAction, unscaledTime));
        }

        public static void DoSequence(this MonoBehaviour monoBehaviour, Func<float>[] functions, bool unscaledTime = false) {
            if (functions == null || functions.Length == 0) return;

            monoBehaviour.StartCoroutine(Utility.SequenceRoutine(functions, unscaledTime));
        }

        public static string ToSpaceBeforeUpperCase(this string stringValue) {
            StringBuilder stringBuilder = new StringBuilder();

            for (int i = 0; i < stringValue.Length; i++) {
                stringBuilder.Append(stringValue[i]);

                int nextChar = i + 1;

                if (nextChar < stringValue.Length && char.IsUpper(stringValue[nextChar]) && !char.IsUpper(stringValue[i])) {
                    stringBuilder.Append(" ");
                }
            }

            return stringBuilder.ToString();
        }
    }
}