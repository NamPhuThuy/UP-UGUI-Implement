// csharp
using UnityEngine;
using UnityEngine.UI;

namespace NamPhuThuy.UI
{
    [RequireComponent(typeof(RectTransform), typeof(Image))]
    public class HeaderImageAutoFitter : MonoBehaviour
    {
        public enum AnchorPreset { TOP_CENTER, MIDDLE_CENTER, BOTTOM_CENTER }

        [Tooltip("Parent to fit within (defaults to root Canvas).")]
        [SerializeField] private RectTransform parent;

        [Tooltip("Header max height as a percentage of parent height (0..1).")]
        [Range(0.05f, 1f)]
        [SerializeField] private float maxHeightPercent = 0.3f;

        [Tooltip("Extra pixels added to width and height.")]
        [SerializeField] private float padding = 0f;

        [Tooltip("Where to anchor the header.")]
        [SerializeField] private AnchorPreset anchor = AnchorPreset.TOP_CENTER;

        private RectTransform rect;
        private Image image;

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
            Canvas.willRenderCanvases += Fit;
            Fit();
        }

        private void OnDisable()
        {
            Canvas.willRenderCanvases -= Fit;
        }

        private void OnValidate()
        {
            if (!isActiveAndEnabled) return;
            ApplyAnchor(anchor);
            Fit();
        }

        private void OnRectTransformDimensionsChange()
        {
            if (!isActiveAndEnabled) return;
            Fit();
        }

        private void ApplyAnchor(AnchorPreset preset)
        {
            switch (preset)
            {
                case AnchorPreset.TOP_CENTER:
                    rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 1f);
                    rect.pivot = new Vector2(0.5f, 1f);
                    break;
                case AnchorPreset.MIDDLE_CENTER:
                    rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
                    rect.pivot = new Vector2(0.5f, 0.5f);
                    break;
                case AnchorPreset.BOTTOM_CENTER:
                    rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0f);
                    rect.pivot = new Vector2(0.5f, 0f);
                    break;
            }
            rect.anchoredPosition = Vector2.zero;
        }

        private void Fit()
        {
            if (parent == null || image == null || image.sprite == null) return;

            Vector2 parentSize = parent.rect.size;
            float maxH = Mathf.Clamp01(maxHeightPercent) * parentSize.y;

            var spr = image.sprite;
            float spriteAspect = spr.rect.width / spr.rect.height;

            // Fit to parent width first
            float width = parentSize.x;
            float height = width / spriteAspect;

            // Clamp by max height while preserving aspect
            if (height > maxH)
            {
                height = maxH;
                width = height * spriteAspect;
            }

            rect.sizeDelta = new Vector2(width, height) + Vector2.one * padding;
        }
    }
}
