using UnityEngine;

namespace NamPhuThuy.UGUIAdapter
{
    /// Keeps a SpriteRenderer's apparent screen size constant as the camera zooms/changes.
    [ExecuteAlways]
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteKeepScreenSize : MonoBehaviour
    {
        [SerializeField] private Camera targetCamera; // Defaults to Camera.main
        [SerializeField] private Vector2 screenAnchor = new Vector2(0.5f, 0.5f);


        [SerializeField] private bool lockX = true; // Scale X to keep size
        [SerializeField] private bool lockY = true; // Scale Y to keep size
        [SerializeField] private bool lockZ = false; // Optional for 3D sprites/billboards

        [Header("Flags")] [SerializeField] private bool isKeepScreenPosition;

        private SpriteRenderer _sr;
        private Vector3 _baseLocalScale;

        // Baseline camera state
        private bool _isOrtho0;
        private float _ortho0; // initial orthographicSize
        private float _fov0; // initial fieldOfView (deg)
        private float _dist0; // initial distance camera->sprite along camera forward
        private float _viewHeight0; // initial "view height" in world units at sprite depth

        #region MonoBehaviour Callbacks

        private void Awake()
        {
            Init();
            targetCamera = Camera.main;
        }

        private void OnEnable() => Init();

        private void OnValidate()
        {
            if (!Application.isPlaying) Init();
        }

        #endregion

        void Init()
        {
            if (!_sr) _sr = GetComponent<SpriteRenderer>();
            if (!targetCamera) targetCamera = Camera.main;

            if (!targetCamera) return;

            _baseLocalScale = transform.localScale;

            _isOrtho0 = targetCamera.orthographic;
            if (_isOrtho0)
            {
                _ortho0 = Mathf.Max(0.0001f, targetCamera.orthographicSize);
                _viewHeight0 = 2f * _ortho0;
            }
            else
            {
                _fov0 = Mathf.Max(1e-3f, targetCamera.fieldOfView);
                _dist0 = DistanceAlongView(targetCamera, transform.position);
                _viewHeight0 = 2f * _dist0 * Mathf.Tan(_fov0 * 0.5f * Mathf.Deg2Rad);
            }

            ApplyScale(); // set once in editor too
        }

        void LateUpdate()
        {
            if (!targetCamera) return;

            if (isKeepScreenPosition)
                KeepScreenPosition();
            KeepScreenScale();
            // ApplyScale();
        }

        void ApplyScale()
        {
            // Compute current view height at the sprite's depth
            float viewH;
            if (targetCamera.orthographic)
            {
                float ortho = Mathf.Max(0.0001f, targetCamera.orthographicSize);
                viewH = 2f * ortho;
            }
            else
            {
                float dist = DistanceAlongView(targetCamera, transform.position);
                float fov = Mathf.Max(1e-3f, targetCamera.fieldOfView);
                viewH = 2f * dist * Mathf.Tan(fov * 0.5f * Mathf.Deg2Rad);
            }

            // Guard against uninitialized baseline
            if (_viewHeight0 <= 0f) return;

            // Scale proportionally to keep same screen coverage
            float ratio = viewH / _viewHeight0;

            Vector3 s = _baseLocalScale;
            if (lockX) s.x = _baseLocalScale.x * ratio;
            if (lockY) s.y = _baseLocalScale.y * ratio;
            if (lockZ) s.z = _baseLocalScale.z * ratio;

            transform.localScale = s;
        }

        static float DistanceAlongView(Camera cam, Vector3 worldPos)
        {
            // Positive distance in front of camera (project onto camera forward)
            return Vector3.Dot(worldPos - cam.transform.position, cam.transform.forward);
        }

        #region Private Methods

        void KeepScreenPosition()
        {
            // Get pixel coordinate from anchor
            float x = screenAnchor.x * Screen.width;
            float y = screenAnchor.y * Screen.height;

            Vector3 screenPos = new Vector3(x, y, _dist0 > 0 ? _dist0 : 10f);
            Vector3 worldPos = targetCamera.ScreenToWorldPoint(screenPos);

            transform.position = worldPos;
        }

        void KeepScreenScale()
        {
            // Compute current view height
            float viewH = targetCamera.orthographic
                ? 2f * targetCamera.orthographicSize
                : 2f * _dist0 * Mathf.Tan(targetCamera.fieldOfView * 0.5f * Mathf.Deg2Rad);

            if (_viewHeight0 <= 0f) return;

            float ratio = viewH / _viewHeight0;
            transform.localScale = _baseLocalScale * ratio;
        }

        #endregion
    }
}