using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace NamPhuThuy
{
    
    public class ButtonSwitchFade : MonoBehaviour
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
        [SerializeField] private Image backgroundImage;
        [SerializeField] private Image onStateImage;

        [Header("Indicator")]
        [SerializeField] private RectTransform indicator;
        [SerializeField] private RectTransform offTransform;
        [SerializeField] private RectTransform onTransform;

        [Header("Animation Settings")]
        [SerializeField] private float fadeDuration = 0.3f;
        [SerializeField] private float moveDuration = 0.3f;
        [SerializeField] private Ease moveEase = Ease.OutQuad;

        
        private Tween _fadeTween;
        private Tween _moveTween;
        
        public State CurrentState => currentState;

        #endregion

        #region Private Fields

        #endregion

        #region MonoBehaviour Callbacks

        void Start()
        {
            
        }

        void Update()
        {
            
        }

        #endregion

        #region Private Methods
        
        private void ApplyStateImmediate()
        {
            // Set overlay image (onStateImage) alpha based on state
            float targetAlpha = currentState == State.ON ? 1f : 0f;
            Color onColor = onStateImage.color;
            onColor.a = targetAlpha;
            onStateImage.color = onColor;

            // Move indicator to correct position
            indicator.anchoredPosition = currentState == State.ON
                ? onTransform.anchoredPosition
                : offTransform.anchoredPosition;
        }

        private void AnimateToState()
        {
            // Fade background
            Color targetColor = currentState == State.ON ? onStateImage.sprite.texture.GetPixel(100, 50) : backgroundImage.sprite.texture.GetPixel(100, 50);
            
            int targetAlpha = currentState == State.ON ? 1 : 0;

            _fadeTween = DOTween.Sequence()
                .Append(onStateImage.DOFade(targetAlpha, fadeDuration))
                .AppendCallback((() =>
                {
                    var c = onStateImage.color;
                    c.a = targetAlpha;              // force alpha to 1
                    onStateImage.color = c;
                }));

            if (isChangeColor)
            {
                _fadeTween = DOTween.Sequence()
                    .Append(onStateImage.DOColor(targetColor, fadeDuration))
                    .AppendCallback(() =>
                    {
                        onStateImage.color = Color.white;
                    });
            }            

            // Move indicator
            Vector2 targetPosition = currentState == State.ON ? onTransform.anchoredPosition : offTransform.anchoredPosition;
            _moveTween = indicator.DOAnchorPos(targetPosition, moveDuration).SetEase(moveEase).OnComplete((() =>
            {
                
            }));
        }

        private void OnDestroy()
        {
            _fadeTween?.Kill();
            _moveTween?.Kill();
        }
        
        #endregion

        #region Public Methods
        
        public void SetState(State newState, bool immediate = false)
        {
            if (currentState == newState && !immediate) return;

            currentState = newState;

            // Kill existing tweens
            _fadeTween?.Kill();
            _moveTween?.Kill();

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
            SetState(currentState == State.OFF ? State.ON : State.OFF);
        }
        
        #endregion

        #region Editor Methods

        public void ResetValues()
        {
            
        }
        
        #endregion
    }

    /*#if UNITY_EDITOR
    [CustomEditor(typeof(ButtonSwitchFade))]
    [CanEditMultipleObjects]
    public class ButtonSwitchFadeEditor : Editor
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
    #endif*/
}