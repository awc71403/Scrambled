using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

namespace Michsky.UI.ModernUIPack {
    public class TooltipContent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler {
        [Header("CONTENT")]
        [TextArea] public string description;

        [Header("RESOURCES")]
        public GameObject tooltipRect;
        public TextMeshProUGUI descriptionText;

        [Header("OPTIONS")]
        public bool stationary;

        TooltipManager tpManager;
        [HideInInspector] public Animator tooltipAnimator;

        public static TooltipContent main;
        public static TooltipContent secondary;

        void Start() {
            if (tooltipRect == null || descriptionText == null) {
                try {
                    tooltipRect = GameObject.Find("Tooltip Rect");
                    descriptionText = tooltipRect.transform.GetComponentInChildren<TextMeshProUGUI>();
                }

                catch {
                    Debug.LogError("No Tooltip object assigned.", this);
                }
            }

            if (tooltipRect != null) {
                tpManager = tooltipRect.GetComponentInParent<TooltipManager>();
                tooltipAnimator = tooltipRect.GetComponentInParent<Animator>();
            }
        }

        public void OnPointerEnter(PointerEventData eventData) {
            if (tooltipRect != null) {
                descriptionText.text = description;
                tpManager.allowUpdating = true;
                tooltipAnimator.gameObject.SetActive(false);
                tooltipAnimator.gameObject.SetActive(true);
                tooltipAnimator.Play("In");
            }
        }

        public void OnPointerExit(PointerEventData eventData) {
            if (tooltipRect != null) {
                tooltipAnimator.Play("Out");
                tpManager.allowUpdating = false;
            }
        }

        public void OnPointerDown(PointerEventData eventData) {
            //
        }
    }
}