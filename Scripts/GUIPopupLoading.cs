using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace NamPhuThuy.UGUIImplement
{

    public class GUIPopupLoading : GUIBase
    {
        #region Private Serializable Fields

        [SerializeField] private Slider loadingSlider;
        [SerializeField] private TextMeshProUGUI titleText;


        [Header("Flags")]
        [SerializeField] private bool playOnShow = true;
        [SerializeField] private bool useUnscaledTime = true;


        [Header("Behavior")]
        [Tooltip("Default duration in seconds if none is provided to Show()/Begin().")]
        [SerializeField] private float defaultDuration = 2f;

        [Tooltip("Progress curve from 0→1 over time.")]
        [SerializeField] private AnimationCurve progressCurve = AnimationCurve.Linear(0, 0, 1, 1);

        /// <summary>
        /// Fired when the slider reaches the end (value = 1).
        /// </summary>
        public event Action OnLoadComplete;
        #endregion

        private Coroutine _routine;

        #region MonoBehaviour Methods

        private void OnDisable()
        {
            OnLoadComplete = null; // Clear all listeners
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Starts the loading animation. If duration <= 0, completes immediately.
        /// </summary>
        public void Begin(float duration = 2f)
        {
            StopLoading();
            _routine = StartCoroutine(RunLoading(duration));
        }

        /// <summary>
        /// Stops the loading animation (does not fire completion).
        /// </summary>
        public void StopLoading()
        {
            if (_routine != null)
            {
                StopCoroutine(_routine);
                _routine = null;
            }
        }

        #endregion

        #region Private Methods

        private IEnumerator RunLoading(float duration)
        {
            if (loadingSlider == null)
            {
                Debug.LogWarning("[GUIPopupLoading] No Slider assigned.");
                yield break;
            }

            if (duration <= 0f)
            {
                loadingSlider.value = 1f;
                OnLoadComplete?.Invoke();
                yield break;
            }

            float t = 0f;
            while (t < duration)
            {
                t += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
                float normalized = Mathf.Clamp01(t / duration);
                loadingSlider.value = Mathf.Clamp01(progressCurve.Evaluate(normalized));
                yield return null;
            }

            loadingSlider.value = 1f;
            _routine = null;
            OnLoadComplete?.Invoke();
        }

        #endregion

        #region Override Methods

        public override void Show(params object[] parameters)
        {
            base.Show(parameters);

            if (loadingSlider != null)
            {
                loadingSlider.minValue = 0f;
                loadingSlider.maxValue = 1f;
                loadingSlider.value = 0f;
            }

            // If caller passed a duration as the first parameter, use it. Otherwise use defaults.
            if (parameters != null && parameters.Length > 0 && parameters[0] is float durFromParams)
            {
                Begin(durFromParams);
            }
            else if (playOnShow)
            {
                Begin(defaultDuration);
            }
        }

        public override void Hide(params object[] parameters)
        {
            StopLoading();
            base.Hide(parameters);
        }

        #endregion

    }

}