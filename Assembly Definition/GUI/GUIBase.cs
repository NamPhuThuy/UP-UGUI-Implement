using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NamPhuThuy.UGUIImplement
{
    public partial class GUIBase : MonoBehaviour
    {
        public enum Type
        {
            FULL_SCREEN = 0,
            POP_UP = 1,
            NONE = 999
        }
        
        public enum GUIId
        {
            GUI_HUD = 0,
            GUI_LEVEL_WIN = 1,
            GUI_LEVEL_LOSE = 2,
            GUI_SETTINGS = 3,
            GUI_OFFER_BOOSTER = 4,
            GUI_SHOP = 5,
            GUI_LEVEL_TRANSITION = 6,
            GUI_LOSE_HEART_WARNING = 7,
            GUI_RATING = 8,
            GUI_NOT_ENOUGH_COIN = 9,
            GUI_REWARD_EARN = 10,
            GUI_OFFER_GAME_OVER = 11,
            GUI_TUTORIAL = 12,
            GUI_NOTIFICATION = 13,
            
            GUI_CHEAT = 99,
            
        }

        [Header("Stats")]
        [SerializeField] protected float showDuration = 0.4f;
        [SerializeField] protected float hideDuration = 0.4f;
        [SerializeField] protected float GUIMaskInitialAlpha = 0.8f;

        [Space(10)]
        [SerializeField] protected float showDelay = 0f;
        [SerializeField] protected float hideDelay = 0f;

        [Header("Components")]
        [SerializeField] private Image guiMask;
        [SerializeField] private CanvasGroup canvasGroup;
        public CanvasGroup CanvasGroup => canvasGroup;

        [Header("Behaviour")]
        private List<Tween> showTweens = new List<Tween>();
        private List<Tween> hideTweens = new List<Tween>();
        private List<Sequence> _sequences = new List<Sequence>();

        [Header("Flags")]
        [SerializeField] private Type currentType = Type.FULL_SCREEN;
        public Type CurrentType => currentType;
        [SerializeField] private GUIId guiId;
        public GUIId GuiId => guiId;

        // [Header("Events")]
        public event Action OnShow;
        public event Action OnShowComplete;
        public event Action OnHide;

        #region Public Methods

        public virtual void Show(params object[] parameters)
        {
            transform.SetAsLastSibling(); // show the GUI on top

            transform.gameObject.SetActive(true);
            TriggerOnShow();

            if (hideTweens.Count > 0)
            {
                foreach (var tween in hideTweens)
                {
                    tween.Kill();
                }

                hideTweens.Clear();
            }

            if (guiMask != null)
            {
                Color color = guiMask.color;
                color.a = 0f;
                guiMask.color = color;
                showTweens.Add(guiMask.DOFade(GUIMaskInitialAlpha, showDuration * .75f).SetEase(Ease.InQuad));
            }

            switch (currentType)
            {
                case Type.FULL_SCREEN:
                    PlayFullscreenShowAnimation();
                    break;
                case Type.POP_UP:

                    canvasGroup.transform.localScale = Vector3.zero;
                    canvasGroup.alpha = 1;

                    Sequence showSequence = DOTween.Sequence();

                    showSequence.Append(canvasGroup.transform.DOScale(1.1f * Vector3.one, 0.7f * showDuration).SetEase(Ease.InOutSine));
                    showSequence.Append(canvasGroup.transform.DOScale(1f * Vector3.one, 0.3f * showDuration).SetEase(Ease.InOutSine)
                        .OnComplete((
                        () =>
                        {
                            // Play Audio
                            gameObject.SetActive(true);
                            Debug.Log(message:$"[{name}] SetActive(true)");
                            OnShowComplete?.Invoke();
                        }))
                    );

                    _sequences.Add(showSequence);
                    break;
                case Type.NONE:
                    gameObject.SetActive(true);
                    canvasGroup.interactable = true;
                    canvasGroup.alpha = 1;
                    Debug.Log(message:$"[{name}] SetActive(true)");
                    break;
            }
        }

        public virtual void Hide(params object[] parameters)
        {
            if (showTweens.Count > 0)
            {
                foreach (var tween in showTweens)
                {
                    tween.Kill();
                    
                }

                showTweens.Clear();
            }

            if (_sequences.Count > 0)
            {
                foreach (var tween in _sequences)
                {
                    tween.Kill();
                }

                _sequences.Clear();
            }

            if (guiMask != null)
            {
                hideTweens.Add(guiMask.DOFade(0f, showDuration * .75f).SetEase(Ease.InQuad));
            }

            switch (currentType)
            {
                case Type.FULL_SCREEN:
                    if (canvasGroup == null)
                    {
                        transform.gameObject.SetActive(false);
                        Debug.Log(message:$"[{name}] SetActive(false)");
                        break;
                    }

                    hideTweens.Add(canvasGroup.DOFade(0, hideDuration).SetEase(Ease.OutQuad).OnComplete(() =>
                    {
                        canvasGroup.interactable = false;
                        transform.gameObject.SetActive(false);
                        Debug.Log(message:$"[{name}] SetActive(false)");
                    }));
                    break;
                case Type.POP_UP:
                    if (canvasGroup == null)
                    {
                        transform.gameObject.SetActive(false);
                        Debug.Log(message:$"[{name}] SetActive(false)");
                        break;
                    }

                    hideTweens.Add(
                        canvasGroup.transform.DOScale(1.1f * Vector3.one, 0.5f * hideDuration).SetEase(Ease.OutQuad)
                        .OnComplete(() =>
                        {
                            hideTweens.Add(canvasGroup.transform.DOScale(0.5f * Vector3.one, 0.5f * hideDuration).SetEase(Ease.OutQuad));
                            hideTweens.Add(
                                canvasGroup.DOFade(0, 0.5f * hideDuration).SetEase(Ease.OutQuad)
                                .OnComplete(() =>
                                {
                                    transform.gameObject.SetActive(false);
                                    Debug.Log(message:$"[{name}] SetActive(false)");
                                })
                            );
                        })
                    );

                    // hideTweens.Add(canvasGroup.transform.DOScale(Vector3.zero, hideDuration).SetEase(Ease.OutQuad).OnComplete(() => transform.gameObject.SetActive(false)));
                    break;
                case Type.NONE:
                    
                    if (canvasGroup == null)
                    {
                        transform.gameObject.SetActive(false);
                        Debug.Log(message:$"[{name}] SetActive(false)");
                        break;
                    }
                    
                    hideTweens.Add(canvasGroup.DOFade(0, hideDuration).SetEase(Ease.OutQuad).OnComplete(() =>
                    {
                        canvasGroup.interactable = false;
                        transform.gameObject.SetActive(false);
                        Debug.Log(message:$"[{name}] SetActive(false)");
                    }));
                    break;
            }



            TriggerOnHide();
        }

        /// <summary>
        /// Hides the GUI immediately without any animations.
        /// </summary>
        public virtual void HideFast(params object[] parameters)
        {
            // Kill all running tweens to prevent them from interfering
            if (showTweens.Count > 0)
            {
                foreach (var tween in showTweens)
                {
                    tween.Kill();
                }
                showTweens.Clear();
            }
            
            if (hideTweens.Count > 0)
            {
                foreach (var tween in hideTweens)
                {
                    tween.Kill();
                }
                hideTweens.Clear();
            }

            if (_sequences.Count > 0)
            {
                foreach (var tween in _sequences)
                {
                    tween.Kill();
                }
                _sequences.Clear();
            }

            // Immediately set the final state without animation
            if (canvasGroup != null)
            {
                canvasGroup.interactable = false;
            }

            transform.gameObject.SetActive(false);
            
            TriggerOnHide();
        }

        public void SetGUIID(GUIId guiId)
        {
            this.guiId = guiId;
        }

        #endregion

        #region Private Methods

        private void PlayFullscreenShowAnimation()
        {
            if (canvasGroup == null) return;
            
            canvasGroup.alpha = 1;

            canvasGroup.alpha = 0f;

            showTweens.Add(canvasGroup.DOFade(1, showDuration).SetEase(Ease.InOutSine));

            canvasGroup.transform.localScale = Vector3.zero;

            showTweens.Add(canvasGroup.transform.DOScale(1.05f, 0.7f * showDuration).SetEase(Ease.InOutSine).OnComplete(() =>
            {
                showTweens.Add(canvasGroup.transform.DOScale(1f, 0.3f * showDuration).SetEase(Ease.InOutSine).OnComplete(() =>
                {
                    canvasGroup.interactable = true;
                    gameObject.SetActive(true);
                    Debug.Log(message:$"[{name}] SetActive(true)");
                    OnShowComplete?.Invoke();
                }));
            }));
        }

        #endregion

        #region Protected Methods

        protected void AddEventTrigger(GameObject obj, EventTriggerType type,
            UnityAction<BaseEventData> action)
        {
            EventTrigger trigger = obj.GetComponent<EventTrigger>() ?? obj.AddComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry { eventID = type };
            entry.callback.AddListener(action);
            trigger.triggers.Add(entry);
        }

        protected void TriggerOnShow()
        {
            Debug.Log(message:$"GUIBase.TriggerOnShow()");
            OnShow?.Invoke();
        }

        protected void TriggerOnHide()
        {
            Debug.Log(message:$"GUIBase.TriggerOnHide()");
            OnHide?.Invoke();
        }


        #endregion

        #region Editor Methods

        public virtual void ResetValues()
        {
            showDuration = 0.4f;
            hideDuration = 0.4f;

            showDelay = 0f;
            hideDelay = 0f;
        }

        #endregion
    }
}