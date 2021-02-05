using System;
using System.Collections.Generic;
using CustomUtilities;
using TMPro;
using UnityEngine;

public class CountdownTimer : MonoBehaviour {
    [SerializeField] private ApplicationEventRelay applicationEventRelay;
    [SerializeField] private GameObject countdownPanel;
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private string endMessage;

    private void OnEnable() {
        if (applicationEventRelay) applicationEventRelay.OnCountdown += ShowCountdown;
    }

    private void OnDisable() {
        if (applicationEventRelay) applicationEventRelay.OnCountdown -= ShowCountdown;
    }

    private void Start() {
        countdownPanel.SetActive(false);
    }

    private void ShowCountdown(float duration, bool showAsWholeSeconds) {
        if (!showAsWholeSeconds) {
            float countdownTime = duration;

            this.DoRoutine(duration, () => {
                countdownPanel.SetActive(true);
            }, t => {
                if (countdownTime >= 0) {
                    countdownText.text = $"{countdownTime:F}";
                }

                countdownTime = (1 - t) * duration;
            }, () => {
                countdownPanel.SetActive(false);
            });
        } else {
            int durationWholeSeconds = (int) duration;
            Vector3 startScale = Vector3.one;
            Vector3 targetScale = new Vector3(1.5f, 1.5f, 1.5f);

            List<Func<float>> countdownSequenceFunctions = new List<Func<float>>();

            countdownSequenceFunctions.Add(() => {
                countdownText.text = $"{durationWholeSeconds}";
                countdownPanel.transform.localScale = startScale;
                countdownPanel.SetActive(true);

                return 0f;
            });

            for (int i = durationWholeSeconds; i >= 1; i--) {
                int secondsLeft = i;

                countdownSequenceFunctions.Add(() => {
                    this.DoRoutine(1f, () => {
                        countdownText.text = $"{secondsLeft}";
                    }, t => {
                        countdownPanel.transform.localScale = Lerp.Value(startScale, targetScale, t, Easing.Back.Out);
                    });

                    return 1f;
                });
            }
            
            countdownSequenceFunctions.Add(() => {
                this.DoRoutine(1f, () => {
                    countdownText.text = endMessage;
                }, t => {
                    countdownPanel.transform.localScale = Lerp.Value(startScale, targetScale, t, Easing.Back.Out);
                });

                return 1f;
            });

            countdownSequenceFunctions.Add(() => {
                countdownPanel.SetActive(false);

                return 0f;
            });

            this.DoSequence(countdownSequenceFunctions.ToArray());
        }
    }
}