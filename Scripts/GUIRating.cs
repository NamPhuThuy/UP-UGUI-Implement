using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace NamPhuThuy.UGUIImplement
{

    public class GUIRating : GUIBase
    {
        [Header("Flags")]
        [SerializeField] private bool isActive = true;
        [SerializeField] private bool isClickedStar = false;

        [Header("Stars")]
        [SerializeField] private List<ButtonClicky> starButtons = new List<ButtonClicky>(5);
        [SerializeField] private List<ButtonSwitch> starButtonSwitchs = new List<ButtonSwitch>(5);

        [Header("Fill Direction")]
        [Tooltip("If true: clicking a star fills that star and all stars to its RIGHT.\nIf false: fills that star and all stars to its LEFT (classic).")]
        [SerializeField] private bool fillToRight = true;

        [Header("Buttons")]
        [SerializeField] private Button closeButton;
        [SerializeField] private Button rateButton;

        [Header("Store IDs")]
        [SerializeField] private string androidPackageName = "";
        [SerializeField] private string iOSAppId = "";

        [Header("Events")]
        public UnityEvent<int> OnRatingChanged;

        // Internal: which star index was clicked last (0..4). -1 means nothing selected yet.
        private int selectedIndex = -1;

        #region MonoBehaviour Callbacks

        void Awake()
        {
            // Wire star button clicks
            for (int i = 0; i < starButtons.Count; i++)
            {
                int idx = i; // capture
                if (starButtons[i] != null)
                    starButtons[i].onClick.AddListener(() => OnStarClicked(idx));
            }

            if (rateButton != null)
                rateButton.onClick.AddListener(OnClickRate);

            // Initialize visuals to empty
            UpdateStarVisuals();
        }

        private void OnEnable()
        {
            // _rateButtonEffect = rateButton.GetComponent<UIEffect>();
            // if (_rateButtonEffect == null)
            //     _rateButtonEffect = rateButton.AddComponent<UIEffect>();

            closeButton.onClick.AddListener((() =>
            {
                // Debug.Log($"GUIRating.Hide()");
                Hide();
            }));

            // _rateButtonEffect.toneFilter = ToneFilter.Grayscale;
            // _rateButtonEffect.toneIntensity = 1f;

            Reset();
        }

        private void OnDisable()
        {
            closeButton.onClick.RemoveAllListeners();
        }

        #endregion



        #region Button Events

        private void OnStarClicked(int index)
        {
            selectedIndex = Mathf.Clamp(index, 0, starButtons.Count - 1);
            UpdateStarVisuals();

            // Report a user-friendly rating number (1..5), left-to-right.
            int visibleRating = fillToRight
                ? starButtons.Count - selectedIndex
                : selectedIndex + 1;

            OnRatingChanged?.Invoke(visibleRating);
            // Debug.Log($"Rating selected: {visibleRating} star(s)");

            if (!isClickedStar)
            {
                isClickedStar = true;
                isActive = true;

                // DOTween.To(() => _rateButtonEffect.toneIntensity, x => _rateButtonEffect.toneIntensity = x, 0f, 0.8f).OnComplete((
                //     () =>
                //     {
                //         isActive = true;
                //     }));
            }
        }

        private void UpdateStarVisuals()
        {
            for (int i = 0; i < starButtons.Count; i++)
            {
                if (starButtons[i] == null) continue;

                var img = starButtons[i].GetComponent<Image>();
                if (img == null) continue;

                bool filled;
                if (selectedIndex < 0)
                {
                    filled = false;
                }
                else if (fillToRight)
                {
                    // Fill clicked star and everything to its RIGHT
                    filled = (i >= selectedIndex);
                }
                else
                {
                    // Classic: fill clicked star and everything to its LEFT
                    filled = (i <= selectedIndex);
                }

                // img.color = filled ? filledColor : emptyColor;

                if (filled)
                {
                    starButtonSwitchs[i].SetState(ButtonSwitch.ButtonSwitchState.ON);
                }
                else
                {
                    starButtonSwitchs[i].SetState(ButtonSwitch.ButtonSwitchState.OFF);
                }
            }
        }

        private void OnClickRate()
        {
            if (!isActive)
            {
               
                return;
            }

            if (selectedIndex != 4)
            {
                Hide();

                return;
            }

#if UNITY_ANDROID
            // Prefer the Play Store app; fall back to web
            if (!string.IsNullOrEmpty(androidPackageName))
            {
                string marketUrl = "market://details?id=" + androidPackageName;
                string webUrl = "https://play.google.com/store/apps/details?id=" + androidPackageName;

                try { Application.OpenURL(marketUrl); }
                catch { Application.OpenURL(webUrl); }
            }
            else
            {
                Debug.LogWarning("GUIRating: Android package name not set.");
            }
#elif UNITY_IOS
            // Direct to the rating/review page for your app
            if (!string.IsNullOrEmpty(iOSAppId))
            {
                // This format jumps straight to 'Write a Review'
                string url = $"itms-apps://itunes.apple.com/app/id{iOSAppId}?action=write-review";
                Application.OpenURL(url);
            }
            else
            {
                Debug.LogWarning("GUIRating: iOS App ID not set.");
            }
#else
            // In Editor or other platforms, just log
            // Debug.Log("GUIRating: Would open store page here.");
#endif
        }

        #endregion

        #region Private

        private void Reset()
        {
            selectedIndex = -1;
            isActive = false;
            isClickedStar = false;

            UpdateStarVisuals();
        }

        #endregion

        #region Helpers

        // Optional helpers if you need to fetch the current rating in code:

        /// <summary>
        /// Returns the visible rating (1..5, left-to-right), or 0 if none selected.
        /// </summary>
        public int GetRating()
        {
            if (selectedIndex < 0) return 0;
            return fillToRight ? starButtons.Count - selectedIndex : selectedIndex + 1;
        }

        /// <summary>
        /// Programmatically set the rating (1..5, left-to-right). Pass 0 to clear.
        /// </summary>
        public void SetRating(int rating)
        {
            rating = Mathf.Clamp(rating, 0, starButtons.Count);
            if (rating == 0) selectedIndex = -1;
            else selectedIndex = fillToRight ? (starButtons.Count - rating) : (rating - 1);

            UpdateStarVisuals();
            if (rating > 0) OnRatingChanged?.Invoke(rating);
        }

        #endregion

    }
}