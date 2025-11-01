// csharp
using UnityEngine;
using UnityEngine.UI;

namespace NamPhuThuy.UI
{
    [RequireComponent(typeof(RectTransform), typeof(Image))]
    public class FitToParentTag : MonoBehaviour
    {
        public enum AnchorPreset { TopCenter, MiddleCenter, BottomCenter }

        [Tooltip("Parent to cover. Defaults to the root Canvas RectTransform.")]
        [SerializeField] private RectTransform parent;

        [Tooltip("Extra size (in pixels) added to both width and height.")]
        [SerializeField] private float padding = 0f;

        [Tooltip("Where to anchor the background.")]
        [SerializeField] private AnchorPreset anchor = AnchorPreset.MiddleCenter;

        [Header("Components")]
        [SerializeField] private RectTransform rect;
        [SerializeField] private Image image;

        #region MonoBehaviour Callbacks
        private void Awake()
        {
            rect = GetComponent<RectTransform>();
            image = GetComponent<Image>();

            if (parent == null)
            {
                var canvas = GetComponentInParent<Canvas>();
                if (canvas != null) parent = canvas.GetComponent<RectTransform>();
            }

            ApplyAnchor(anchor);
        }

        private void OnEnable()
        {
            Canvas.willRenderCanvases += FitToParent;
            FitToParent();
        }

        private void OnDisable()
        {
            Canvas.willRenderCanvases -= FitToParent;
        }

        private void OnValidate()
        {
            if (!isActiveAndEnabled) return;
            ApplyAnchor(anchor);
            FitToParent();
        }
        #endregion

        private void OnRectTransformDimensionsChange()
        {
            if (!isActiveAndEnabled) return;
            FitToParent();
        }

        private void ApplyAnchor(AnchorPreset preset)
        {
            switch (preset)
            {
                case AnchorPreset.TopCenter:
                    rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 1f);
                    rect.pivot = new Vector2(0.5f, 1f);
                    break;
                case AnchorPreset.MiddleCenter:
                    rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
                    rect.pivot = new Vector2(0.5f, 0.5f);
                    break;
                case AnchorPreset.BottomCenter:
                    rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0f);
                    rect.pivot = new Vector2(0.5f, 0f);
                    break;
            }
            rect.anchoredPosition = Vector2.zero;
        }

        private void FitToParent()
        {
            if (parent == null || image == null || image.sprite == null) return;

            Vector2 parentSize = parent.rect.size;
            float parentAspect = parentSize.x / parentSize.y;

            var spr = image.sprite;
            float spriteAspect = spr.rect.width / spr.rect.height;

            Vector2 size;
            if (spriteAspect > parentAspect)
            {
                // Wider than parent: match height, extend width to cover
                size = new Vector2(parentSize.y * spriteAspect, parentSize.y);
            }
            else
            {
                // Taller than parent: match width, extend height to cover
                size = new Vector2(parentSize.x, parentSize.x / spriteAspect);
            }

            rect.sizeDelta = size + Vector2.one * padding;
        }
    }
}
