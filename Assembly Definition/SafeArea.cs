using UnityEngine;

/* DEVLOG
Ver 1.0
====================================================================================================
MỤC ĐÍCH & CHỨC NĂNG (SafeArea):
- Tự động căn chỉnh (fit) RectTransform theo vùng an toàn (Safe Area / tai thỏ, notch, home indicator) của thiết bị di động (Screen.safeArea).
- Hỗ trợ thêm lề cố định (Extra Insets) theo pixel màn hình trực tiếp từ Inspector (_extraLeft, _extraRight, _extraTop, _extraBottom).
- Hỗ trợ lề động runtime (Runtime Insets) áp dụng cho toàn bộ UI (ví dụ: tự động đẩy UI lên trên khi hiển thị Banner Ad qua SetRuntimeBottomInset).
- Tùy biến linh hoạt: Bật/tắt căn chỉnh theo trục ngang (_conformX), trục dọc (_conformY), hoặc bỏ qua viền trên/dưới (_ignoreTop, _ignoreBottom).
- Đã được tối ưu chống đệ quy (re-entrancy guard) và tự động refit khi xoay màn hình / thay đổi độ phân giải.

HƯỚNG DẪN SỬ DỤNG:
- Gắn component này vào GameObject có RectTransform dạng full-stretch (anchorMin = (0,0), anchorMax = (1,1)), là con trực tiếp của Canvas toàn màn hình
- KHÔNG gắn vào các UI element con đã neo cố định theo góc (corner-anchored) vì script sẽ điều chỉnh lại anchorMin/anchorMax dạng stretch.
====================================================================================================
*/


namespace NamPhuThuy.UGUIAdapter
{
    /// <summary>
    /// Conforms this <see cref="RectTransform"/> to the device safe area
    /// (<see cref="Screen.safeArea"/>) plus optional extra insets.
    ///
    /// Place it on a full-stretch <see cref="RectTransform"/> that is a direct child of the
    /// screen-filling Canvas (e.g. the <c>UINagivator</c> containers in Home, or the
    /// Gameplay popup host). It must NOT be put on corner-anchored HUD pieces — setting
    /// <see cref="RectTransform.anchorMin"/>/<see cref="RectTransform.anchorMax"/> assumes
    /// a stretch layout.
    ///
    /// Insets are expressed in <b>screen pixels</b> and come from two sources, summed per edge:
    ///  - serialized <c>_extra*</c> fields: permanent design margins (default 0).
    ///  - static runtime insets (see <see cref="SetRuntimeInsets"/>): shared across every
    ///    instance and intended to be driven at runtime — e.g. the banner ad pushes the bottom
    ///    edge up via <see cref="SetRuntimeBottomInset"/>.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public sealed class SafeArea : MonoBehaviour
    {
        [Header("Axes")]
        [Tooltip("Conform to the safe area horizontally.")] [SerializeField] private bool _conformX = true;
        [Tooltip("Conform to the safe area vertically.")] [SerializeField] private bool _conformY = true;

        [Tooltip("Ignore the safe area on the top edge (anchor it to the top of the screen).")]
        [SerializeField] private bool _ignoreTop;

        [Tooltip("Ignore the safe area on the bottom edge (anchor it to the bottom of the screen).")]
        [SerializeField] private bool _ignoreBottom;

        [Header("Extra Insets (screen px)")]
        [Tooltip("Permanent extra inset in screen pixels, applied on top of the safe area and any runtime insets.")]
        [SerializeField] private float _extraLeft;
        [SerializeField] private float _extraRight;
        [SerializeField] private float _extraBottom;
        [SerializeField] private float _extraTop;

        [Header("Stats")]
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private Rect _lastSafeArea;
        [SerializeField] private Vector2Int _lastScreenSize;
        [SerializeField] private bool _isApplying;

        // ---- Runtime insets (shared across all instances) ----
        private static float _runtimeLeft;
        private static float _runtimeRight;
        private static float _runtimeBottom;
        private static float _runtimeTop;

        private static event System.Action _runtimeInsetsChanged;

        #region MonoBehaviour Methods

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        private void OnEnable()
        {
            _runtimeInsetsChanged += HandleRuntimeInsetsChanged;
            Apply();
        }

        private void OnDisable()
        {
            _runtimeInsetsChanged -= HandleRuntimeInsetsChanged;
        }

        private void Update()
        {
            // Cheap change check (mirrors Graphy's G_SafeArea). Screen.safeArea shifts on
            // orientation change / device with a notch, which is exactly when we must refit.
            Vector2Int currentScreenSize = new Vector2Int(Screen.width, Screen.height);
            if (Screen.safeArea != _lastSafeArea || currentScreenSize != _lastScreenSize)
            {
                Apply();
            }
        }


        #endregion
        
        /// <summary>
        /// called whenever a UI element's RectTransform changes its size, scale, pivot, or anchor configuration
        /// </summary>
        private void OnRectTransformDimensionsChange()
        {
            if (_isApplying) return;

            Vector2Int currentScreenSize = new Vector2Int(Screen.width, Screen.height);
            if (Screen.safeArea != _lastSafeArea || currentScreenSize != _lastScreenSize)
            {
                Apply();
            }
        }

        private void HandleRuntimeInsetsChanged()
        {
            Apply();
        }

        private void Apply()
        {
            if (_isApplying) return;

            _isApplying = true;
            try
            {
                Rect safeArea = Screen.safeArea;
                _lastSafeArea = safeArea;
                _lastScreenSize = new Vector2Int(Screen.width, Screen.height);

                float screenWidth = Screen.width > 0 ? Screen.width : 1f;
                float screenHeight = Screen.height > 0 ? Screen.height : 1f;

                float left = safeArea.xMin / screenWidth + (_extraLeft + _runtimeLeft) / screenWidth;
                float right = safeArea.xMax / screenWidth - (_extraRight + _runtimeRight) / screenWidth;
                // When ignoring an edge, skip the device safe-area (notch) inset but keep the
                // serialized extras and any runtime insets (e.g. banner-ads).
                float bottomSafe = _ignoreBottom ? 0f : safeArea.yMin / screenHeight;
                float bottom;
                if (_runtimeBottom > 0f)
                {
#if UNITY_IOS
                    // iOS-only fix: the background banner is rendered from the *screen edge* and its
                    // reported size (GetBackgroundBannerSize) already covers the device's bottom safe
                    // area (the home indicator). Stacking safeArea.yMin + bannerHeight double-counts
                    // the home indicator and pushes the UI up too far. Replace the safe-area
                    // contribution instead. Android keeps the original stacking path below. Max() keeps
                    // us from ever dipping below the safe area.
                    bottom = Mathf.Max(bottomSafe, _runtimeBottom / screenHeight) + _extraBottom / screenHeight;
#else
                    bottom = bottomSafe + (_extraBottom + _runtimeBottom) / screenHeight;
#endif
                }
                else
                {
                    bottom = bottomSafe + (_extraBottom + _runtimeBottom) / screenHeight;
                }
                float topSafe = _ignoreTop ? 1f : safeArea.yMax / screenHeight;
                float top = topSafe - (_extraTop + _runtimeTop) / screenHeight;

                if (!_conformX)
                {
                    left = 0f;
                    right = 1f;
                }

                if (!_conformY)
                {
                    bottom = 0f;
                    top = 1f;
                }

                // Guard against inverted rects if insets ever exceed the available space.
                right = Mathf.Max(right, left);
                top = Mathf.Max(top, bottom);

                if (_rectTransform == null)
                {
                    _rectTransform = GetComponent<RectTransform>();
                }

                Vector2 targetMin = new Vector2(left, bottom);
                Vector2 targetMax = new Vector2(right, top);

                if (_rectTransform.anchorMin != targetMin ||
                    _rectTransform.anchorMax != targetMax ||
                    _rectTransform.offsetMin != Vector2.zero ||
                    _rectTransform.offsetMax != Vector2.zero)
                {
                    _rectTransform.anchorMin = targetMin;
                    _rectTransform.anchorMax = targetMax;
                    _rectTransform.offsetMin = Vector2.zero;
                    _rectTransform.offsetMax = Vector2.zero;
                }
            }
            finally
            {
                _isApplying = false;
            }
        }

        // ---- Runtime inset API ----

        /// <summary>
        /// Sets the shared runtime insets (in screen pixels) applied on top of the safe area
        /// and the serialized extras for every active <see cref="SafeArea"/> instance.
        /// </summary>
        public static void SetRuntimeInsets(float left, float bottom, float right, float top)
        {
            _runtimeLeft = left;
            _runtimeBottom = bottom;
            _runtimeRight = right;
            _runtimeTop = top;
            _runtimeInsetsChanged?.Invoke();
        }

        /// <summary>
        /// Convenience for banner ads: pushes the bottom edge of every <see cref="SafeArea"/>
        /// up by the given screen-pixel height (e.g. <c>SayKit.GetBackgroundBannerSize()</c>).
        /// </summary>
        public static void SetRuntimeBottomInset(float screenPixels)
        {
            if (Mathf.Approximately(_runtimeBottom, screenPixels))
            {
                return;
            }

            _runtimeBottom = screenPixels;
            _runtimeInsetsChanged?.Invoke();
        }

        /// <summary>Clears the runtime bottom inset, restoring the bottom edge to the safe area.</summary>
        public static void ClearRuntimeBottomInset()
        {
            SetRuntimeBottomInset(0f);
        }
    }
}
