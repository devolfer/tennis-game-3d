using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[AddComponentMenu("UI/Extensions/Scroll To Selection XY")]
[RequireComponent(typeof(ScrollRect))]
public class ScrollToSelectionXY : MonoBehaviour {
    [SerializeField] private float scrollSpeed;
    [SerializeField] private RectTransform layoutListGroup;

    private RectTransform targetScrollObject;
    private bool scrollToSelection = true;

    private RectTransform scrollWindow;
    private ScrollRect targetScrollRect;

    private void Start() {
        TryGetComponent(out targetScrollRect);
        TryGetComponent(out scrollWindow);
    }

    private void Update() {
        ScrollRectToLevelSelection();
    }

    private void ScrollRectToLevelSelection() {
        EventSystem eventSystem = EventSystem.current;
        if (!eventSystem) return;
        if (!targetScrollRect || !scrollWindow || !layoutListGroup) return;

        if (!eventSystem.currentSelectedGameObject) return;
        if (!eventSystem.currentSelectedGameObject.TryGetComponent(out RectTransform selection)) return;
        
        // allow for nesting up to 3 layers
        if (selection.transform.parent != layoutListGroup.transform) {
            selection.transform.parent.TryGetComponent(out selection);
            if (selection.transform.parent != layoutListGroup.transform) {
                selection.transform.parent.TryGetComponent(out selection);
                if (selection.transform.parent != layoutListGroup.transform) return;
            }
        }
        
        if (selection != targetScrollObject) scrollToSelection = true;
        if (!scrollToSelection) return;

        bool finishedX;
        bool finishedY;

        if (targetScrollRect.vertical) {
            // move the current scroll rect to correct position
            float selectionPos = -selection.anchoredPosition.y;

            //float elementHeight = layoutListGroup.sizeDelta.y / layoutListGroup.transform.childCount;
            //float maskHeight = currentCanvas.sizeDelta.y + scrollWindow.sizeDelta.y;
            float listPixelAnchor = layoutListGroup.anchoredPosition.y;

            // get the element offset value depending on the cursor move direction
            float offLimitsValue = listPixelAnchor - selectionPos + selection.rect.height / 2f;
            // move the target scroll rect
            targetScrollRect.verticalNormalizedPosition += offLimitsValue / layoutListGroup.sizeDelta.y * Time.deltaTime * scrollSpeed;

            finishedY = Mathf.Abs(offLimitsValue) < 2f;
        } else {
            finishedY = true;
        }

        if (targetScrollRect.horizontal) {
            // move the current scroll rect to correct position
            float selectionPos = -selection.anchoredPosition.x;
            // Debug.Log($"Selection Pos: {selectionPos}");

            //float elementWidth = layoutListGroup.sizeDelta.x / layoutListGroup.transform.childCount;
            //float maskWidth = currentCanvas.sizeDelta.y + scrollWindow.sizeDelta.y;
            float listPixelAnchor = layoutListGroup.anchoredPosition.x;
            // Debug.Log($"List Pixel Anchor: {listPixelAnchor}");

            // get the element offset value depending on the cursor move direction
            float offLimitsValue = listPixelAnchor - selectionPos + selection.rect.width / 2f;
            // Debug.Log($"Off Limits Value: {offLimitsValue}");
            
            // move the target scroll rect
            targetScrollRect.horizontalNormalizedPosition += offLimitsValue / layoutListGroup.sizeDelta.x * Time.deltaTime * scrollSpeed;

            finishedX = Mathf.Abs(offLimitsValue) < 2f;
        } else {
            finishedX = true;
        }

        // check if we reached our destination
        if (finishedX && finishedY) scrollToSelection = false;

        // save last object we were "heading to" to prevent blocking
        targetScrollObject = selection;
    }
}