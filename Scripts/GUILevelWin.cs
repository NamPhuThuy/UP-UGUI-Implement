using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

using MoreMountains.Tools;

using NamPhuThuy.AnimateWithScripts;
using NamPhuThuy.Common;
using NamPhuThuy.DataManage;
using NamPhuThuy.FirebaseAdapter;

#if USE_AD_NETWORKS
using NamPhuThuy.AdNetworkAdapter;
#endif

#if USE_LEAN_LOCALIZATION
using NamPhuThuy.Lean_Localization;
using Lean.Localization;
#endif

#if USE_SPINE
using NamPhuThuy.SpineAdapter;
using Spine.Unity;
#endif

using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ColorHelper = NamPhuThuy.Common.ColorHelper;
using DebugLogger = NamPhuThuy.Common.DebugLogger;
using Random = UnityEngine.Random;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace NamPhuThuy.UGUIImplement
{
    public class GUILevelWin : GUIBase
    {
        #region Private Serializable Fields

        [Header("Flags")] 
        [SerializeField] private bool interactable = true;
        [SerializeField] private bool isCoinFlyActive;

        [Header("Texts")] 
        [SerializeField] private TextMeshProUGUI levelButtonText;
        [SerializeField] private LeanLocalizedTextMeshProUGUI levelButtonTextLocalized;
        [SerializeField] private TextMeshProUGUI levelTitleText;
        [SerializeField] private TextMeshProUGUI coinAdsText;
        [SerializeField] private TextMeshProUGUI winMessageText;
        [SerializeField] private LeanLocalizedTextMeshProUGUI winMessageTextLocalized;

        [SerializeField] private bool useUnscaledTime = true;
        [SerializeField] private AnimationCurve progressCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        
        [Header("Buttons")] 
        [SerializeField] private ButtonClicky settingsButton;
        [SerializeField] private ButtonClicky nextLevelButton;
        [SerializeField] private Button backButton;
        [SerializeField] private Button coinAdsButton;
        [SerializeField] private ButtonClicky shopButton;

        private bool[] isRewardProgressCompleted;

        [Header("Components")] [SerializeField]
        private SkeletonGraphic winSkeGrap;
        [SerializeField] private ParticleSystem confettiParticle;
        [SerializeField] private CoinPanel coinPanel;
        public CoinPanel CoinPanel => coinPanel;
      
        #endregion

        #region Private Fields

        private Coroutine _progressRoutine;

        #endregion

        #region MonoBehaviour Callbacks

        private void Awake()
        {
            settingsButton.onClick.AddListener(OnClickSettings);
            nextLevelButton.onClick.AddListener(OnClickNextLevel);
            backButton.onClick.AddListener(OnClickBack);

            coinAdsButton.onClick.AddListener(OnClickCoinAds);
            shopButton.onClick.AddListener(() => { UGUIManager.Ins.ShowGUI(UGUIManager.Ins.GUIShop); });

            // Set reward-visual 
            int coinRewardAmount = DataManager.Ins.EventRewardData.GetRecord(EventRewardType.WATCH_ADS_WIN_LEVEL)
                .rewards.GetCoinAmount();
            coinAdsText.text = $"x{coinRewardAmount}";

            pictureRewardProgressBar.InitRewardProgressBar();
            coinRewardProgressBar.InitRewardProgressBar();
        }

        private void OnDestroy()
        {
            settingsButton.onClick.RemoveAllListeners();
            nextLevelButton.onClick.RemoveAllListeners();
            backButton.onClick.RemoveAllListeners();
            coinAdsButton.onClick.RemoveAllListeners();
            shopButton.onClick.RemoveAllListeners();
        }

        #endregion
        
        #region Override Methods

        public override void Show(params object[] parameters)
        {
            DebugLogger.Log(message: $"");
            base.Show(parameters);

            StartCoroutine(IE_Show());
        }

        public override void Hide(params object[] parameters)
        {
            if (confettiParticle != null)
            {
                confettiParticle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                confettiParticle.gameObject.SetActive(false);
            }

            HideFast();
        }

        private IEnumerator IE_Show()
        {
            yield return null;
            TurnOffElements();
#if USE_AUDIO
            AudioManager.Ins.Play(AudioEnum.SFX_FEMALE_MOANING);
#endif
            
            // Play VFX and SFX
#if USE_SPINE
            SpineHelper.PlayAppearThenLoop(
                winSkeGrap,
                "appear", // Plays once (loop=false)
                "idle", // Loops forever (loop=true)
                timeScale: 1f
            );
#endif

            if (confettiParticle != null)
            {
                confettiParticle.gameObject.SetActive(true);
                confettiParticle.Play();
            }

            yield return StartCoroutine(IE_ShowCoinProgress());
            yield return StartCoroutine(IE_ShowPictureProgress());
            yield return StartCoroutine(IE_ShowButtons());
        }

        private IEnumerator IE_ShowCoinProgress()
        {
            bool isFinished = false;
            
            // Run the progress bar
            coinRewardProgressBar.gameObject.SetActive(true);
            coinRewardProgressBar.Reset();
            
            // Coin Reward Progress
            float progressPerLevel = 1f / DataConst.DEFAULT_LEVEL_REWARD_MILESTONE;

            float prevCoinRewardProgress = DataManager.Ins.PProgressData.CurrentCoinRewardProgress;
            float currentCoinRewardProgress = DataManager.Ins.PProgressData.CurrentCoinRewardProgress + progressPerLevel;

            if (currentCoinRewardProgress > 1)
            {
                currentCoinRewardProgress = progressPerLevel;
            }

            DataManager.Ins.PProgressData.CurrentCoinRewardProgress = currentCoinRewardProgress;
            
            if (currentCoinRewardProgress < 1)
            {
                coinRewardProgressText.color = Color.white;
                coinRewardProgressTextLocalized.TranslationName = "Only n levels left";
                coinRewardProgressTextLocalized.UpdateTranslationWithParameter(LeanLocalizedConst.PARAM_AMOUNT, $"<color=#78EB66>{Mathf.RoundToInt((1 - currentCoinRewardProgress) * DataConst.DEFAULT_LEVEL_REWARD_MILESTONE)}</color>");
            }
            else
            {
                coinRewardProgressText.color = ColorHelper.FromHex("#78EB66");
                coinRewardProgressTextLocalized.TranslationName = "Reward Completed";
            }

            // Delay to fix the segment background in wrong place in coinRewardProgress
            // In the first time this GUI is enable, the Awake() will called -> the coinRewardProgressBar.InitRewardProgressBar() will be called. And this method will need time to complete the init-process -> we need to delay for a bit
            PrimeTween.Tween.Delay(0.15f, () =>
            {
                coinRewardProgressBar.SetupRewardProgressBar(currentCoinRewardProgress, prevCoinRewardProgress,
                    DataConst.DEFAULT_LEVEL_REWARD_MILESTONE);
                coinRewardProgressBar.GiftSkeleton.AnimationState.SetAnimation(0, "appear", true);
                coinRewardProgressBar.SetGiftPosition();

                coinRewardProgressBar.Progress(
                    onNormalProgressCompleted: () =>
                    {
                        isFinished = true;
                    },
                    onLastProgressCompleted: () =>
                    {
                        coinRewardProgressBar.HandleRewardCompleted(onCompletedAction:(() =>
                        {
                            isFinished = true;
                        }));
                    });
            });

            yield return new WaitUntil(() => isFinished);
        }
        
        private IEnumerator IE_ShowPictureProgress()
        {
            /*if (DataManager.Ins.PAlbumData.IsUnlockAll())
            {
                EnableInteract();
                yield break;
            }

            bool isFinished = false;
            
            pictureRewardProgressBar.gameObject.SetActive(true);
            pictureRewardProgressBar.Reset();
            
            AlbumRewardData galleryRewardConfig = DataManager.Ins.AlbumRewardData;

            int currentLevel = DataManager.Ins.PProgressData.LevelId;

            int requiredStepToNextReward = galleryRewardConfig.GetCurrentMilestone(currentLevel);
            DebugLogger.Log(message:$"requiredStepToNextReward 1: {requiredStepToNextReward}");
            
            if (requiredStepToNextReward <= 0)
            {
                requiredStepToNextReward = DataConst.DEFAULT_GALLERY_MILESTONE;
            }
            
            DebugLogger.Log(message:$"requiredStepToNextReward 2: {requiredStepToNextReward}");

            pictureStepToNextReward = requiredStepToNextReward;
            
            pictureRewardProgressBar._currentRewardProgress = DataManager.Ins.AlbumRewardData.GetCurrentProgress(currentLevel);
            float prevRewardProgress = DataManager.Ins.AlbumRewardData.GetPrevProgress(currentLevel);
            
            UpdatePictureRewardTextWithParams();
            
            pictureRewardProgressBar.SetupRewardProgressBar(pictureRewardProgressBar._currentRewardProgress, prevRewardProgress, requiredStepToNextReward);
            pictureRewardProgressBar.SetPicturePosition();
            pictureRewardProgressBar.SetGroupRewardProgressAlpha(0);
            
            PrimeTween.Tween.Alpha(pictureRewardProgressBar.groupRewardProgress, 1f, 0.3f).OnComplete(() =>
            {
                DebugLogger.Log();
                pictureRewardProgressBar.Progress(
                    onNormalProgressCompleted: () =>
                    {
                        isFinished = true;
                        EnableInteract();
                    },
                    onLastProgressCompleted: () =>
                    {
                        pictureRewardProgressBar.RewardProgressBackground.gameObject.SetActive(false);
                        isFinished = true;
                        EnableInteract();
                        PAlbumData pGalleryData = DataManager.Ins.PAlbumData;

                        while (pGalleryData.IsContain(pGalleryData.CurrentProgressAlbumId))
                        {
                            pGalleryData.CurrentProgressAlbumId++;
                            DebugLogger.Log(message: $"pGalleryData.CurrentProgressAlbumId: {pGalleryData.CurrentProgressAlbumId}");
                        }

                        GUIManager.Ins.ShowGUI(GUIManager.Ins.GUIPictureNew, 0f, pGalleryData.CurrentProgressAlbumId);
                        DebugLogger.Log(message: $"Show the picture new, pGalleryData.CurrentProgressAlbumId: {pGalleryData.CurrentProgressAlbumId}");
                        pictureRewardProgressBar.ScaleDownThePicture();

                        SetRewardProgressComplete(0);
                    });
            });

            yield return new WaitUntil((() => isFinished));*/

            yield return null;
        }

        private IEnumerator IE_ShowButtons()
        {
            yield return null;
            PrimeTween.Sequence showButtonSeq = PrimeTween.Sequence.Create();
            showButtonSeq.Chain(PrimeTween.Tween.Delay(0.3f).OnComplete(ShowCoinAdsButton));
            showButtonSeq.Chain(PrimeTween.Tween.Delay(2f).OnComplete(ShowNextLevelButton));

            DebugLogger.Log(message: $"Done the show");
        }
        
        private void ShowCoinAdsButton()
        {
            coinAdsButton.gameObject.SetActive(true);
            coinAdsButton.transform.localScale = Vector3.zero;


            PrimeTween.Sequence primeSeq = PrimeTween.Sequence.Create();
            primeSeq.Chain(PrimeTween.Tween.Scale(coinAdsButton.transform, 1.1f, 0.3f));
            primeSeq.Chain(PrimeTween.Tween.Scale(coinAdsButton.transform, 1f, 0.1f)).OnComplete((() =>
            {
                coinAdsButton.interactable = true;
                coinAdsButton.GetComponent<ObjScaleAuto>()?.Play();
            }));
        }

        private void ShowNextLevelButton()
        {
            // DebugLogger.Log(message: $"");
            nextLevelButton.gameObject.SetActive(true);
            nextLevelButton.transform.localScale = Vector3.zero;

            PrimeTween.Sequence primeSeq = PrimeTween.Sequence.Create();
            primeSeq.Chain(PrimeTween.Tween.Scale(nextLevelButton.transform, 1.1f, 0.3f));
            primeSeq.Chain(PrimeTween.Tween.Scale(nextLevelButton.transform, 1f, 0.1f)).OnComplete((() =>
            {
                nextLevelButton.interactable = true;
            }));

            UpdateLocalizedTextWithParams();
        }

        #endregion

        #region Private Methods

        private void EnableInteract()
        {
            interactable = true;
            // GUIManager.Ins.GUIPictureNew.OnShow -= EnableInteract;
        }

        private bool IsReturnHome()
        {
            return false;
        }

        #endregion

        #region Private Methods

        private void TurnOffElements()
        {
            DebugLogger.Log(message: $"");
            coinAdsButton.gameObject.SetActive(false);
            nextLevelButton.gameObject.SetActive(false);
            
            pictureRewardProgressBar.gameObject.SetActive(false);
            coinRewardProgressBar.gameObject.SetActive(false);
            
            interactable = false;
        }

        #endregion

        #region Public Methods

        public void UpdateLocalizedTextWithParams()
        {
            StartCoroutine(IE_Update());
            IEnumerator IE_Update()
            {
                bool isReturnHome = IsReturnHome();
                
                if (isReturnHome)
                {
                    levelButtonTextLocalized.TranslationName = "Continue";
                }
                else
                {
                    levelButtonTextLocalized.TranslationName = "Level";
                }
                
                yield return YieldHelper.GetRealtime(UGUIConst.TRANSLATION_DELAY);
                
                int currentLevel = DataManager.Ins.PProgressData.LevelId;
                 
                if (!isReturnHome)
                {
                    levelButtonTextLocalized.UpdateTranslationWithParameter(LeanLocalizedConst.PARAM_LEVEL, $"{currentLevel + 1}");
                }
                
            }
        }

        #endregion
        

        #region Button Events

        private void OnClickSettings()
        {
            if (!interactable)
            {
                AnimationManager.Ins.PlayBasicPopupText(LeanLocalization.GetTranslationText(LeanLocalizedConst.READYING));
                
                return;
            }
            UGUIManager.Ins.ShowGUI(UGUIManager.Ins.GUISettings);
        }

        
        private Action onLoadingComplete = () =>
        {
            
            MMEventManager.TriggerEvent(new ELevelLoad_Fire()
            {
                levelId = DataManager.Ins.PProgressData.LevelId
            });
        };
        private void OnClickNextLevel()
        {
            // DebugLogger.Log();
            if (!interactable)
            {
                DebugLogger.Log(message: $"Interactable is false");
                AnimationManager.Ins.PlayBasicPopupText(LeanLocalization.GetTranslationText(LeanLocalizedConst.READYING));
                
                return;
            }

            if ( DataManager.Ins.isGrantRewardsAfterLevel)
            {
                UGUIManager.Ins.ShowGUI(UGUIManager.Ins.GUILoadingScreen);
                PrimeTween.Tween.Delay(1f, () =>
                {
#if USE_AD_NETWORKS
                    AdsManager.Ins.TryShow_Inter_MAX(true, "load_next_level", OnInterClose);
#endif
                    Hide();
                });

                return;
            }

            coinAdsButton.interactable = false;
            nextLevelButton.interactable = false;
            interactable = false;
            
#if USE_AD_NETWORKS
            if (AdsManager.Ins.CanShow_DoubleInter())
            {
                UGUIManager.Ins.ShowGUI(UGUIManager.Ins.GUILoadingScreen, 0f, UGUIConst.FAKE_LOAD_DURATION, onLoadingComplete);
                PrimeTween.Tween.Delay(AdsConst.MIN_MREC_DURATION, () =>
                {
                    AdsManager.Ins.TryShow_DoubleInter(/*OnInterClose*/null, true, "load_next_level");
                });
            }
            else
            {
                UGUIManager.Ins.ShowGUI(UGUIManager.Ins.GUILoadingScreen, 0f, AdsConst.MIN_MREC_DURATION, onLoadingComplete);
            }
#else
            UGUIManager.Ins.ShowGUI(UGUIManager.Ins.GUILoadingScreen, 0f, 1f, onLoadingComplete);
#endif
            
            UGUIManager.Ins.HideGUI(this);

            void OnInterClose()
            {
                if (IsReturnHome())
                {
                    UGUIManager.Ins.ShowGUI(UGUIManager.Ins.GUIHome);
                }
                else
                {
                    // Hide();

                    PrimeTween.Tween.Delay(UGUIConst.FAKE_LOAD_DURATION - 1.4f, () =>
                    {
                        MMEventManager.TriggerEvent(new ELevelLoad_Fire()
                        {
                            levelId = DataManager.Ins.PProgressData.LevelId
                        });
                    });
                }
            }
        }

        private void OnClickCoinAds()
        {
            if (!interactable) return;

            // Example: when a user finishes a level and watches a rewarded ad:
#if USE_AD_NETWORKS
                AdsManager.Ins.TryShow_RewardAd_MAX(OnRewardReceived, OnVideoNotAvailable, OnRewardHidden, AdWatchReason.END_LEVEL_REWARD);
#endif

            void OnRewardReceived()
            {
                List<ResourceAmount> rewards =
                    DataManager.Ins.EventRewardData.GetRecord(EventRewardType.WATCH_ADS_WIN_LEVEL).rewards;

                var args = new ItemFlyArgs
                {
                    AddValue = rewards.GetCoinAmount(),
                    PrevValue = DataManager.Ins.PInventoryData.Coin,
                    TargetText = UGUIManager.Ins.GUILevelWin.CoinPanel.CoinText.transform,
                    StartPosition = this.transform.position,
                    TargetInteractTransform = UGUIManager.Ins.GUILevelWin.CoinPanel.transform, // For positioning the target
                    ItemAmount = 6,
                    ItemSprite = DataManager.Ins.ResourceData.GetResourceRecord(ResourceType.COIN).gameplayImage,
                    OnItemInteract = () =>
                    {
                        #if USE_AUDIO
                        AudioManager.Ins.Play(AudioEnum.SFX_COIN_3);
                        #endif
                    },
                    OnComplete = () =>
                    {
                        StartCoroutine(IE_LoadNextLevel());
                        #if USE_AUDIO
                        AudioManager.Ins.Play(AudioEnum.SFX_COIN_1);
                        #endif
                        DataManager.Ins.PInventoryData.TryApplyRewards(rewards, 1, isUseUpdateAnim: false); 
                    }
                };

                AnimationManager.Ins.Play(args);

                coinAdsButton.interactable = false;
                nextLevelButton.interactable = false;
            }

            void OnVideoNotAvailable()
            {
                var args = new ToastArgs
                {
                    Message = LeanLocalization.GetTranslationText(LeanLocalizedConst.CHECK_INTERNET),
                    CustomAnchoredPos = AnimationConst.UPPER_ANCHORED_POS,
                    TextColor = Color.white,
                    TextFont = UGUIManager.Ins.DefaultFont,
                    customDuration = 0.5f,
                };
                AnimationManager.Ins.Play(args);
            }

            void OnRewardHidden()
            {
#if USE_FIREBASE_ANALYTICS
                AnalyticsAdapter.Log_RewardAd_Watched(DataManager.Ins.PProgressData.LevelId + 1, nameof(AdWatchPlace.GUI_LEVEL_WIN));
#endif
                
                // DebugLogger.Log($"GUILevelWin.OnCLickCoinAds.OnAdsClosed()");
            }
            
            IEnumerator IE_LoadNextLevel()
            {
                yield return new WaitForSeconds(0.5f);
                MMEventManager.TriggerEvent(new ELevelLoad_Fire()
                {
                    levelId = DataManager.Ins.PProgressData.LevelId,
                });
                Hide();
            }
        }

        private void OnClickBack()
        {
            if (isCoinFlyActive)
            {
                var args = new ToastArgs
                {
                    Message = LeanLocalization.GetTranslationText(LeanLocalizedConst.CANT_CLICK),
                    CustomAnchoredPos = AnimationConst.UPPER_ANCHORED_POS,
                    TextColor = Color.white,
                    TextFont = UGUIManager.Ins.DefaultFont,
                    customDuration = 0.5f,
                };
                AnimationManager.Ins.Play(args);
                return;
            }

            UGUIManager.Ins.ShowGUI(UGUIManager.Ins.GUIHome);
            Hide();
        }

        #endregion

        #region Rewards Progress 

        [Header("Reward Progresses")]
        [SerializeField] private CoinRewardSegmentedProgressBar coinRewardProgressBar;
        [SerializeField] private TextMeshProUGUI coinRewardProgressText;
        [SerializeField] private LeanLocalizedTextMeshProUGUI coinRewardProgressTextLocalized;
        [SerializeField] private int coinStepToNextReward;
        
        [SerializeField] private PictureRewardSegmentedProgressBar pictureRewardProgressBar;
        [SerializeField] private TextMeshProUGUI pictureRewardProgressText;
        [SerializeField] private LeanLocalizedTextMeshProUGUI pictureRewardProgressTextLocalized;
        [SerializeField] private int pictureStepToNextReward;
        
        private void UpdatePictureRewardTextWithParams()
        {
            StartCoroutine(IE_Update());
            IEnumerator IE_Update()
            {
                yield return YieldHelper.GetRealtime(UGUIConst.TRANSLATION_DELAY);
                
                if (pictureRewardProgressBar._currentRewardProgress < 1)
                {
                    pictureRewardProgressText.color = Color.white;
                    pictureRewardProgressTextLocalized.TranslationName = "Only n levels left";
                    pictureRewardProgressTextLocalized.UpdateTranslationWithParameter(LeanLocalizedConst.PARAM_AMOUNT, $"<color=#78EB66>{Mathf.RoundToInt((1 - pictureRewardProgressBar._currentRewardProgress) * pictureStepToNextReward)}</color>");
                }
                else
                {
                    pictureRewardProgressText.color = ColorHelper.FromHex("#78EB66");
                    pictureRewardProgressTextLocalized.TranslationName = "Reward Completed";
                }
            }
        }
        
        private void SetRewardProgressComplete(int i)
        {
            isRewardProgressCompleted[i] = true;

            bool isDone = true;

            for (int j = 0; j < isRewardProgressCompleted.Length; j++)
            {
                if (!isRewardProgressCompleted[j])
                {
                    isDone = false;

                    break;
                }
            }

            if (isDone)
            {
                EnableInteract();
            }
        }

        #endregion

        public enum NextLevelButtonEnum
        {
            NEXT_LEVEL_STATE = 0,
            HOME_STATE = 1
        }
    }
}