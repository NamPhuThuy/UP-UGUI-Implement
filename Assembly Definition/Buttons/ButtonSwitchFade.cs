using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using NamPhuThuy.Common;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UI;
#endif

namespace NamPhuThuy.UGUIAdapter
{
    
    public class ButtonSwitchFade : Button
    {
        
        public enum State
        {
            NONE = 0,
            OFF = 1,
            ON = 2
        }
        #region Private Serializable Fields

        [Header("Flags")] 
        [SerializeField] private bool isChangeColor;
        [SerializeField] private State currentState = State.NONE;
        
        [Header("Background")]
        [FormerlySerializedAs("backgroundImage")]
        [SerializeField] private Image offStateImage;
        [SerializeField] private Image onStateImage;
        [SerializeField] private GameObject additionOnStateObj;
        [SerializeField] private GameObject additionOffStateObj;
        
        
        [Header("Indicator")]
        [SerializeField] private RectTransform indicator;
        [SerializeField] private RectTransform indicatorOffPivot;
        [SerializeField] private RectTransform indicatorOnPivot;

        [Header("Animation Settings")]
        [SerializeField] private float fadeDuration = 0.3f;
        [SerializeField] private float moveDuration = 0.3f;
        [SerializeField] private Ease moveEase = Ease.OutQuad;

        
        private Tween _fadeTween;
        private Tween _moveTween;
        
        public State CurrentState => currentState;
        public Image OffStateImage => offStateImage;
        public Image OnStateImage => onStateImage;

        #endregion

        #region Private Methods
        
        private void ApplyStateImmediate()
        {
            // Set state image alphas based on state
            float onTargetAlpha = currentState == State.ON ? 1f : 0f;
            float offTargetAlpha = currentState == State.ON ? 0f : 1f;

            Color onColor = onStateImage.color;
            onColor.a = onTargetAlpha;
            onStateImage.color = onColor;

            Color offColor = offStateImage.color;
            offColor.a = offTargetAlpha;
            offStateImage.color = offColor;

            if (additionOnStateObj != null) additionOnStateObj.SetActive(currentState == State.ON);
            if (additionOffStateObj != null) additionOffStateObj.SetActive(currentState == State.OFF);

            // Move indicator to correct position
            indicator.anchoredPosition = currentState == State.ON
                ? indicatorOnPivot.anchoredPosition
                : indicatorOffPivot.anchoredPosition;
        }

        private void AnimateToState()
        {
            // Fade state images
            float onTargetAlpha = currentState == State.ON ? 1f : 0f;
            float offTargetAlpha = currentState == State.ON ? 0f : 1f;

            Sequence fadeSeq = DOTween.Sequence()
                .Join(onStateImage.DOFade(onTargetAlpha, fadeDuration).OnComplete(() =>
                {
                    Color c = onStateImage.color;
                    c.a = onTargetAlpha;
                    onStateImage.color = c;
                }))
                .Join(offStateImage.DOFade(offTargetAlpha, fadeDuration).OnComplete(() =>
                {
                    Color c = offStateImage.color;
                    c.a = offTargetAlpha;
                    offStateImage.color = c;
                }));

            if (isChangeColor)
            {
                Image targetImage = currentState == State.ON ? onStateImage : offStateImage;
                Color targetColor = targetImage.sprite.texture.GetPixel(100, 50);
                fadeSeq.Join(targetImage.DOColor(targetColor, fadeDuration).OnComplete(() =>
                {
                    targetImage.color = Color.white;
                }));
            }

            _fadeTween = fadeSeq;

            if (additionOffStateObj != null) additionOffStateObj.SetActive(false);
            if (additionOnStateObj != null) additionOnStateObj.SetActive(false);
            
            // Move indicator
            Vector2 targetPosition = currentState == State.ON ? indicatorOnPivot.anchoredPosition : indicatorOffPivot.anchoredPosition;
            _moveTween = indicator.DOAnchorPos(targetPosition, moveDuration).SetEase(moveEase).OnComplete(() =>
            {
                if (currentState == State.ON)
                {
                    if (additionOnStateObj != null) additionOnStateObj.SetActive(true);
                }
                else if (currentState == State.OFF)
                {
                    if (additionOffStateObj != null) additionOffStateObj.SetActive(true);
                }
            });
        }

        private void OnDestroy()
        {
            _fadeTween?.Kill();
            _moveTween?.Kill();
        }
        
        #endregion

        #region Spam-Click Handling

        /// <summary>
        /// Returns true if the switch is currently animating.
        /// </summary>
        public bool IsTweening()
        {
            return (_moveTween != null && _moveTween.IsActive() && _moveTween.IsPlaying()) ||
                   (_fadeTween != null && _fadeTween.IsActive() && _fadeTween.IsPlaying());
        }

        /// <summary>
        /// Handles player spam-clicking by completing running tweens before transitioning.
        /// </summary>
        public void HandleSpamClick()
        {
            _fadeTween?.Kill(complete: true);
            _moveTween?.Kill(complete: true);
        }

        #endregion

        #region Public Methods
        
        public void SetState(State newState, bool immediate = false)
        {
            if (currentState == newState && !immediate) return;

            currentState = newState;

            HandleSpamClick();

            if (immediate)
            {
                ApplyStateImmediate();
            }
            else
            {
                AnimateToState();
            }
        }

        public void ToggleState()
        {
            if (IsTweening())
            {
                HandleSpamClick();
            }

            SetState(currentState == State.OFF ? State.ON : State.OFF);
        }
        
        #endregion

        #region Editor Methods

        public void ResetValues()
        {
            
        }
        
        #endregion
    }

    #if UNITY_EDITOR
    [CustomEditor(typeof(ButtonSwitchFade))]
    [CanEditMultipleObjects]
    public class ButtonSwitchFadeEditor : ButtonEditor
    {
        private ButtonSwitchFade script;
        private Texture2D frogIcon;
        
        private void OnEnable()
        {
            frogIcon = Resources.Load<Texture2D>("frog"); // no extension needed
        }
        
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            script = (ButtonSwitchFade)target;

            ButtonResetValues();
        }

        private void ButtonResetValues()
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(new GUIContent("Reset Values", frogIcon), GUILayout.Width(InspectorConst.BUTTON_WIDTH_MEDIUM)))
            {
                script.ResetValues();
                EditorUtility.SetDirty(script); // Mark the object as dirty
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
    }
    #endif
}