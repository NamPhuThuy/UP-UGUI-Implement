using UnityEngine;
using UnityEngine.UI;

namespace NamPhuThuy.UI
{
    [RequireComponent(typeof(RawImage))]
    public class RawImageScroller : MonoBehaviour
    {
        [SerializeField] private RawImage target;
        [SerializeField] private Vector2 uvSpeed = new Vector2(0.1f, 0.0f);
        [SerializeField] private Vector2 tiling = Vector2.one; // > 1 to repeat more times
        [SerializeField] private Vector2 initialOffset = Vector2.zero;
        [SerializeField] private bool unscaledTime = false;
        [SerializeField] private bool play = true;

        private Rect uv;

        private void Reset()
        {
            target = GetComponent<RawImage>();
        }

        private void Awake()
        {
            if (target == null) target = GetComponent<RawImage>();
            uv = target != null ? target.uvRect : new Rect(0f, 0f, 1f, 1f);
            ApplyTiling();
            ApplyInitialOffset();
            EnsureRepeatWrap();
        }

        private void OnEnable()
        {
            if (target == null) target = GetComponent<RawImage>();
            uv = target != null ? target.uvRect : new Rect(0f, 0f, 1f, 1f);
            ApplyTiling();
            ApplyInitialOffset();
            EnsureRepeatWrap();
        }

        private void Update()
        {
            if (!play || target == null || target.texture == null) return;

            float dt = unscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;

            uv.x = Mathf.Repeat(uv.x + uvSpeed.x * dt, 1f);
            uv.y = Mathf.Repeat(uv.y + uvSpeed.y * dt, 1f);

            target.uvRect = uv;
        }

        private void ApplyTiling()
        {
            uv.width = Mathf.Max(0.0001f, tiling.x);
            uv.height = Mathf.Max(0.0001f, tiling.y);
            if (target != null) target.uvRect = uv;
        }

        private void ApplyInitialOffset()
        {
            uv.x = Mathf.Repeat(initialOffset.x, 1f);
            uv.y = Mathf.Repeat(initialOffset.y, 1f);
            if (target != null) target.uvRect = uv;
        }

        private void EnsureRepeatWrap()
        {
            if (target == null || target.texture == null) return;
            var tex = target.texture;
#if UNITY_2017_1_OR_NEWER
            if (tex.wrapModeU != TextureWrapMode.Repeat) tex.wrapModeU = TextureWrapMode.Repeat;
            if (tex.wrapModeV != TextureWrapMode.Repeat) tex.wrapModeV = TextureWrapMode.Repeat;
#else
            if (tex.wrapMode != TextureWrapMode.Repeat) tex.wrapMode = TextureWrapMode.Repeat;
#endif
        }

        public void SetSpeed(Vector2 speed) => uvSpeed = speed;

        public void SetTiling(Vector2 newTiling)
        {
            tiling = newTiling;
            ApplyTiling();
        }

        public void SetOffset(Vector2 offset)
        {
            initialOffset = offset;
            ApplyInitialOffset();
        }

        public void SetPlaying(bool isPlaying) => play = isPlaying;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (target == null) target = GetComponent<RawImage>();
            uv = target != null ? target.uvRect : new Rect(0f, 0f, 1f, 1f);
            ApplyTiling();
            ApplyInitialOffset();
        }
#endif
    }
}
