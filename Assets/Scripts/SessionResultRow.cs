using System;
using TMPro;
using UnityEngine;

public class SessionResultRow : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI sessionNameText;
    [SerializeField] private TextMeshProUGUI sessionScoreText;
    [SerializeField] private MenuButton detailViewButton;

    private Action detailViewButtonAction;

    private void Awake() {
        detailViewButton.onPress.AddListener(OnDetailViewPress);
    }

    private void OnDestroy() {
        detailViewButton.onPress.RemoveListener(OnDetailViewPress);
    }

    public void Setup(string sessionName, string sessionScore, Action viewButtonAction) {
        sessionNameText.text = sessionName;
        sessionScoreText.text = sessionScore;
        detailViewButtonAction = viewButtonAction;
    }

    private void OnDetailViewPress(MenuButton button) {
        detailViewButtonAction?.Invoke();
    }
}