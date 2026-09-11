using TMPro;
using UnityEngine;

namespace NamPhuThuy.UGUIAdapter
{
    /// <summary>
    /// Animates a TextMeshProUGUI component to show "Loading", "Loading.", "Loading..", "Loading..."
    /// All frame strings are pre-built once in Awake, so Update() performs zero string
    /// allocations - it only swaps a cached string reference into TextMeshProUGUI.text.
    /// </summary>
    public class TMP_LoadingAnim : MonoBehaviour
    {
        [SerializeField] private string baseText = "Loading";
        [SerializeField, Range(1, 6)] private int minDots = 1;
        [SerializeField, Range(1, 6)] private int maxDots = 3;
        [SerializeField, Min(0.05f)] private float interval = 0.4f;
        [SerializeField] private bool useUnscaledTime = true;

        private TextMeshProUGUI _text;
        private string[] _frames;
        private int _frameIndex;
        private float _timer;

        #region MonoBehaviour Methods

        private void Awake()
        {
            _text = GetComponent<TextMeshProUGUI>();
            BuildFrames();
        }

        private void OnEnable()
        {
            _frameIndex = 0;
            _timer = 0f;
            _text.text = _frames[0];
        }

        private void Update()
        {
            _timer += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            if (_timer < interval) return;

            _timer -= interval;
            _frameIndex = (_frameIndex + 1) % _frames.Length;
            _text.text = _frames[_frameIndex]; // reference swap only, no allocation
        }
        

        #endregion
        private void BuildFrames()
        {
            
            // Ping-pong sequence, e.g. minDots=1, maxDots=3 -> . .. ... .. . .. ... .. . ...
            int up = maxDots - minDots;       // steps going up
            int down = maxDots - minDots;     // steps going back down (excludes repeating the peak)
            _frames = new string[up + down];
 
            int idx = 0;
            for (int dots = minDots; dots <= maxDots; dots++)
                _frames[idx++] = baseText + new string('.', dots);
 
            for (int dots = maxDots - 1; dots >= minDots; dots--)
                _frames[idx++] = baseText + new string('.', dots);
        }
        
        /// <summary>Changes the base label (e.g. "loading" -> "saving") and rebuilds the frame cache.</summary>
        public void SetBaseText(string newBaseText)
        {
            if (baseText == newBaseText) return;
            baseText = newBaseText;
            RebuildAndRefresh();
        }
 
        /// <summary>Changes the min/max dot count and rebuilds the ping-pong frame cache.</summary>
        public void SetDotRange(int newMinDots, int newMaxDots)
        {
            newMinDots = Mathf.Max(1, newMinDots);
            newMaxDots = Mathf.Max(newMinDots, newMaxDots);
            if (minDots == newMinDots && maxDots == newMaxDots) return;
 
            minDots = newMinDots;
            maxDots = newMaxDots;
            RebuildAndRefresh();
        }
 
        /// <summary>Changes both base label and dot range in one rebuild (avoids rebuilding twice).</summary>
        public void Configure(string newBaseText, int newMinDots, int newMaxDots)
        {
            baseText = newBaseText;
            minDots = Mathf.Max(1, newMinDots);
            maxDots = Mathf.Max(minDots, newMaxDots);
            RebuildAndRefresh();
        }
 
        private void RebuildAndRefresh()
        {
            BuildFrames();
            _frameIndex = 0;
            _timer = 0f;
            if (_text != null) _text.text = _frames[0];
        }
    }
}