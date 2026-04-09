using System.Collections.Generic;
using DG.Tweening;
using Lean.Localization;
using NamPhuThuy.AnimateWithScripts;
using NamPhuThuy.Common;
using NamPhuThuy.DataManage;
using NamPhuThuy.Lean_Localization;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;
using ColorHelper = NamPhuThuy.Common.ColorHelper;

namespace NamPhuThuy.UGUIImplement
{
    public class GUI_Favorite : GUIBase
    {
        [SerializeField] private List<GirlFavoriteItem> girlPreferenceItems;

        [SerializeField] private int currentCenterItemIndex;

        [SerializeField] private RectTransform girlPreferenceItemContainer;
        [SerializeField] private ButtonClicky confirmButton;
        [SerializeField] private ButtonClicky leftButton;
        [SerializeField] private ButtonClicky rightButton;
        [SerializeField] private SkeletonGraphic selectedPackSpine;
        [SerializeField] private Image background;

        private Vector2 startTouchPosition;
        private Vector2 endTouchPosition;
        private bool swipeDetected = false;
        private bool _isGoLevelOnConfirm;

        [SerializeField] private float minSwipeDistance = 50f;

        private bool _isInTransition;
        private int _prevCenterItemIndex;

        public bool IsGoLevelOnConfirm
        {
            get => _isGoLevelOnConfirm;
            set => _isGoLevelOnConfirm = value;
        }

        #region MonoBehaviour Callbacks

        private void Awake()
        {
            // RemoteConfigController.remoteConfigReadyEvent += OnRemoteConfigLoaded;

            confirmButton.onClick.AddListener(OnClickConfirm);
            leftButton.onClick.AddListener(OnSwipeRight);
            rightButton.onClick.AddListener(OnSwipeLeft);
        }

        private void OnDestroy()
        {
            // RemoteConfigController.remoteConfigReadyEvent -= OnRemoteConfigLoaded;
        }

        private void OnEnable()
        {
            // currentCenterItemIndex = DataUtility.Load("Selected Gallery Pack Index", 1);
            _prevCenterItemIndex = currentCenterItemIndex;

            SetStateForItems();

            // selectedPackSpine.color = Color.white;

            // background.FitImageToRectTransform();
        }

        void Update()
        {
            if (_isInTransition)
            {
                return;
            }

            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Began)
                {
                    swipeDetected = false;
                    startTouchPosition = touch.position;
                }
                else if (touch.phase == TouchPhase.Ended)
                {
                    endTouchPosition = touch.position;
                    DetectSwipe();
                }
            }
        }

        #endregion

        private void ApplyGalleryPack()
        {
            string packName = girlPreferenceItems[currentCenterItemIndex].PackName;

            /*GalleryPackConfigItem item = GamePersistentVariable.galleryPackConfig.items.FirstOrDefault(item => item.packName == packName);

            LevelConfigUtilities.RemoteNormalConfig = item.resourceId;

            ConfigController.Ins.TryGetConfigOnline(onCompleteAction: () =>
            {
                GamePlayManager.Ins.SetupBackground();
            });

            GalleryRemoteConfig.Ins.SetPack(packName);*/
        }

        #region Button Events
        
        private void OnSwipeRight()
        {
            OnSwipeRight(1);
        }

        private void OnSwipeLeft()
        {
            OnSwipeLeft(1);
        }

        private void OnClickConfirm()
        {
            if (!_isGoLevelOnConfirm)
            {
                if (Application.internetReachability == NetworkReachability.NotReachable)
                {
                    UGUIManager.Ins.ShowGUI(UGUIManager.Ins.GUINotification, 0f, new object[2]{LeanLocalization.GetTranslationText(LeanLocalizedConst.NO_INTERNET), LeanLocalization.GetTranslationText(LeanLocalizedConst.CHECK_INTERNET)});

                    return;
                }
            }

            bool isDirty = _prevCenterItemIndex != currentCenterItemIndex;

            if (isDirty)
            {
                ApplyGalleryPack();

                DataManager.Ins.PProgressData.CurrentFavoriteStyleId = currentCenterItemIndex;

                /*var args = new PopupTextArgs
                {
                    Message = LeanLocalization.GetTranslationText(LeanLocalizedConst.VIDEO_DOWNLOADING),
                    CustomAnchoredPos = AnimationConst.UPPER_ANCHORED_POS,
                    TextColor = Color.white,
                    customDuration = 0.5f,
                };
                AnimationManager.Ins.Play(args);*/

                _prevCenterItemIndex = currentCenterItemIndex;
            }

            PrimeTween.Tween.Alpha(selectedPackSpine, 1f, duration: 0.3f);

            if (_isGoLevelOnConfirm)
            {
                _isGoLevelOnConfirm = false;

                // MMEventManager.TriggerEvent(new ELevelLoad(0));
            }
            Hide();
        }

        #endregion

        private void SetStateForItems()
        {
            for (int i = 0; i < girlPreferenceItems.Count; i++)
            {
                GirlFavoriteItem item = girlPreferenceItems[i];

                if (i == currentCenterItemIndex)
                {
                    item.MoveToCenter();
                }
                else
                {
                    item.MoveOutOfCenter();
                }
            }

            selectedPackSpine.transform.SetSiblingIndex(selectedPackSpine.transform.parent.childCount - 2);

            girlPreferenceItemContainer.DOLocalMoveX(((girlPreferenceItems.Count - 1) / 2f - currentCenterItemIndex) * 300, duration: 0.3f);
            selectedPackSpine.transform.DOLocalMoveX(-((girlPreferenceItems.Count - 1) / 2f - currentCenterItemIndex) * 300, duration: 0.3f);
        }

        private void DetectSwipe()
        {
            float distance = Vector2.Distance(startTouchPosition, endTouchPosition);
            if (distance >= minSwipeDistance && !swipeDetected)
            {
                Vector2 swipeVector = endTouchPosition - startTouchPosition;

                if (Mathf.Abs(swipeVector.x) > Mathf.Abs(swipeVector.y))
                {
                    if (swipeVector.x > 0)
                    {
                        OnSwipeRight();
                    }
                    else
                    {
                        OnSwipeLeft();
                    }
                }

                swipeDetected = true;
            }
        }

       

        private void OnSwipeRight(int swipeIndexDistance = 1)
        {
            if (_isInTransition)
            {
                return;
            }

            _isInTransition = true;

            if (currentCenterItemIndex - swipeIndexDistance < 0)
            {
                Sequence invalidSwipeSequence = DOTween.Sequence();

                invalidSwipeSequence.Append(girlPreferenceItemContainer.DOLocalMoveX(girlPreferenceItemContainer.localPosition.x + 80, duration: 0.2f));
                invalidSwipeSequence.Append(girlPreferenceItemContainer.DOLocalMoveX(girlPreferenceItemContainer.localPosition.x, duration: 0.1f));
                invalidSwipeSequence.OnComplete(() =>
                {
                    _isInTransition = false;
                });

                return;
            }

            for (int i = 0; i < girlPreferenceItems.Count; i++)
            {
                int index = i;
                GirlFavoriteItem item = girlPreferenceItems[index];

                if (index == currentCenterItemIndex - swipeIndexDistance)
                {
                    item.MoveToCenter();

                    girlPreferenceItemContainer.DOLocalMoveX(girlPreferenceItemContainer.localPosition.x + 300 * swipeIndexDistance, duration: 0.3f);
                }
                else
                {
                    item.MoveOutOfCenter();
                }
            }

            selectedPackSpine.transform.SetSiblingIndex(selectedPackSpine.transform.parent.childCount - 2);
            selectedPackSpine.transform.localPosition = selectedPackSpine.transform.localPosition.ChangeX(-girlPreferenceItemContainer.localPosition.x - 300 * swipeIndexDistance);

            PrimeTween.Sequence selectedPackSequence = PrimeTween.Sequence.Create();

            selectedPackSpine.color = ColorHelper.WithAlpha(0f);
            selectedPackSpine.gameObject.SetActive(false);

            PrimeTween.Tween.Delay(0.29f, () => { selectedPackSpine.gameObject.SetActive(true);});

            selectedPackSequence.Chain(PrimeTween.Tween.Alpha(selectedPackSpine, 1f, duration: 0.15f,
                startDelay: 0.3f));
            
            selectedPackSequence.OnComplete(() =>
            {
                _isInTransition = false;
            });

            currentCenterItemIndex -= swipeIndexDistance;
        }

        private void OnSwipeLeft(int swipeIndexDistance = 1)
        {
            if (_isInTransition)
            {
                return;
            }

            _isInTransition = true;

            if (currentCenterItemIndex + swipeIndexDistance >= girlPreferenceItems.Count)
            {
                Sequence invalidSwipeSequence = DOTween.Sequence();

                invalidSwipeSequence.Append(girlPreferenceItemContainer.DOLocalMoveX(girlPreferenceItemContainer.localPosition.x - 80, duration: 0.2f));
                invalidSwipeSequence.Append(girlPreferenceItemContainer.DOLocalMoveX(girlPreferenceItemContainer.localPosition.x, duration: 0.1f));
                invalidSwipeSequence.OnComplete(() =>
                {
                    _isInTransition = false;
                });

                return;
            }

            for (int i = 0; i < girlPreferenceItems.Count; i++)
            {
                int index = i;
                GirlFavoriteItem item = girlPreferenceItems[index];

                if (index == currentCenterItemIndex + swipeIndexDistance)
                {
                    item.MoveToCenter();

                    girlPreferenceItemContainer.DOLocalMoveX(girlPreferenceItemContainer.localPosition.x - 300 * swipeIndexDistance, duration: 0.3f);
                }
                else
                {
                    item.MoveOutOfCenter();
                }
            }

            selectedPackSpine.transform.SetSiblingIndex(selectedPackSpine.transform.parent.childCount - 2);
            selectedPackSpine.transform.localPosition = selectedPackSpine.transform.localPosition.ChangeX(-girlPreferenceItemContainer.localPosition.x + 300 * swipeIndexDistance);

            PrimeTween.Sequence selectedPackSequence = PrimeTween.Sequence.Create();

            selectedPackSpine.color = ColorHelper.WithAlpha(0f);
            selectedPackSpine.gameObject.SetActive(false);

            PrimeTween.Tween.Delay(0.29f, () => { selectedPackSpine.gameObject.SetActive(true);});

            selectedPackSequence.Chain(PrimeTween.Tween.Alpha(selectedPackSpine, 1f, duration: 0.15f,
                startDelay: 0.3f));
            selectedPackSequence.OnComplete(() =>
            {
                _isInTransition = false;
            });

            currentCenterItemIndex += swipeIndexDistance;
        }

        public void MoveToItem(GirlFavoriteItem girlFavoriteItem)
        {
            if (_isInTransition)
            {
                return;
            }

            int targetItemIndex = girlPreferenceItems.IndexOf(girlFavoriteItem);
            int moveIndexDistance = targetItemIndex - currentCenterItemIndex;
            int absoluteMoveIndexDistance = Mathf.Abs(targetItemIndex - currentCenterItemIndex);

            if (moveIndexDistance > 0)
            {
                OnSwipeLeft(absoluteMoveIndexDistance);
            }
            else if (moveIndexDistance < 0)
            {
                OnSwipeRight(absoluteMoveIndexDistance);
            }
        }

        private void OnRemoteConfigLoaded()
        {
            ApplyGalleryPack();
        }
    }
}
