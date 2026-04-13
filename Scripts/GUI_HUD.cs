using System;
using System.Collections;
using System.Collections.Generic;

using NamPhuThuy.Common;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using MoreMountains.Tools;

using NamPhuThuy.AnimateWithScripts;
using NamPhuThuy.DataManage;
using NamPhuThuy.FirebaseAdapter;

using DebugLogger = NamPhuThuy.Common.DebugLogger;

#if USE_LEAN_LOCALIZATION
using NamPhuThuy.Lean_Localization;
using Lean.Localization;
#endif

#if USE_AD_NETWORKS
using NamPhuThuy.AdNetworkAdapter;
#endif


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace NamPhuThuy.UGUIImplement
{

    public class GUI_HUD : GUIBase
    {
        #region Private Serializable Fields

        [Header("Boosters")]
        [SerializeField] private List<Booster> boosterList;
        public List<Booster> BoosterList => boosterList;

        [Header("Buttons")]
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button shopButton;
        [SerializeField] private Button replayButton;
        [SerializeField] private Button backButton;
        [SerializeField] private Button removeAdsButton;
        [SerializeField] private Button cheatButton;

        [Header("Texts")]
        [SerializeField] private TextMeshProUGUI levelTitleText;
        [SerializeField] private LeanLocalizedTextMeshProUGUI levelTitleTextLocalized;

        [Header("Images")]
        [SerializeField] private Image tutorialImage;
        [SerializeField] private Image[] sideEffectImages;
        [SerializeField] private GameObject[] sideEffectUIParticles;
        [SerializeField] private ParticleSystem[] sideEffectParticleSystems;


        [Header("Flags")]
        [SerializeField] private bool isInteractable = true;
        [SerializeField] private bool isHandTutShowing = false;
        #endregion

        #region Private Fields

        #endregion

        #region MonoBehaviour Callbacks

        private void Awake()
        {
            // BUTTONS
            settingsButton.onClick.AddListener(OnClickSettings);
            shopButton.onClick.AddListener(OnClickShop);
            replayButton.onClick.AddListener(OnClickReplayLevel);
            backButton.onClick.AddListener(OnClickBack);
            /*removeAdsButton?.onClick.AddListener((() => { GUIManager.Ins.ShowGUI(GUIManager.Ins.GUINoAds); }));
            */
            
            cheatButton.onClick.AddListener((() => UGUIManager.Ins.ShowGUI(UGUIManager.Ins.GUICheat)));
        }

        private void OnEnable()
        {
            ValidateCheatMode();

            /*foreach (Booster booster in boosterList)
            {
                booster.SetState(Booster.BoosterState.AVAILABLE);
            }*/

            CheckTutorial();
            // ValidateNoAdsButton();

          

            void CheckTutorial()
            {
                /*
                 currentTutorRule.tutorialImage is always null, because the TutorialLevelRule is ScriptableObject (not understand yet
                 */

                try
                {
                    /*if (TutorialManager.Ins.CurrentConfig.GetForLevel(DataManager.Ins.PlayerData.currentLevelId)
                            .tutorialImage != null)
                    {
                        tutorialImage.sprite = TutorialManager.Ins.CurrentConfig
                            .GetForLevel(DataManager.Ins.PlayerData.currentLevelId).tutorialImage;
                    }*/
                }
                catch (Exception e)
                {
                    // Debug.LogError($"GUIHUD.OnEnable(): {e}");
                }
            }
        }

        private void OnDestroy()
        {
            //BUTTONS
            settingsButton.onClick.RemoveListener(OnClickSettings);
            shopButton.onClick.RemoveListener(OnClickShop);
            replayButton.onClick.RemoveAllListeners();
            backButton.onClick.RemoveListener(OnClickBack);
            removeAdsButton.onClick.RemoveAllListeners();
            cheatButton.onClick.RemoveAllListeners();
        }

        #endregion

        #region Private Methods

        private Coroutine _delayBoostersActiveCo;

        private void HideButtons()
        {
            settingsButton.gameObject.SetActive(false);
            shopButton.gameObject.SetActive(false);
            replayButton.gameObject.SetActive(false);
            backButton.gameObject.SetActive(false);
            removeAdsButton.gameObject.SetActive(false);
            cheatButton.gameObject.SetActive(false);
        }

        private void ShowButtons()
        {
            settingsButton.gameObject.SetActive(true);
            // shopButton.gameObject.SetActive(true);
            // replayButton.gameObject.SetActive(true);
            backButton.gameObject.SetActive(true);
            // removeAdsButton.gameObject.SetActive(true);
            ValidateCheatMode();
        }

        #endregion

        #region Public Methods
        public void UpdateLocalizedTextWithParams()
        {
            DebugLogger.Log();
            StartCoroutine(IE_Update());
           
            
            IEnumerator IE_Update()
            {
                yield return YieldHelper.GetRealtime(UGUIConst.TRANSLATION_DELAY);
                
                levelTitleTextLocalized.UpdateTranslationWithParameter(LeanLocalizedConst.PARAM_LEVEL, $"{DataManager.Ins.PProgressData.LevelId + 1}");
            }
        }

        public void TurnOffTutorialImage()
        {
            if (tutorialImage == null) return;
            if (!tutorialImage.gameObject.activeSelf) return;

            tutorialImage.gameObject.SetActive(false);
        }

        public void DelayBoostersActive(float seconds)
        {
            // DebugLogger.Log($"GUIHUD.DelayBoostersActive(): {seconds}");
            if (_delayBoostersActiveCo != null)
            {
                StopCoroutine(_delayBoostersActiveCo);
                _delayBoostersActiveCo = null;
            }

            _delayBoostersActiveCo = StartCoroutine(IEDelayBoostersActive(seconds));
        }
        public IEnumerator IEDelayBoostersActive(float seconds)
        {
            // DebugLogger.Log($"GUIHUD.IEDelayBoostersActive(): {seconds}");

            foreach (Booster booster in boosterList)
            {
                booster.IsTapTooFast = true;
            }

            yield return YieldHelper.WaitForSeconds(seconds);

            foreach (Booster booster in boosterList)
            {
                booster.IsTapTooFast = false;
            }
        }
        
        void ValidateCheatMode()
        {
            if (!DataManager.Ins.IsCheatMode)
            {
                cheatButton.gameObject.SetActive(false);
                return;
            }
            // cheatButton.gameObject.SetActive(true);
        }

        public void ValidateNoAdsButton()
        {
            removeAdsButton.gameObject.SetActive(false);

            if (DataManager.Ins.PProgressData.IsAdsRemoved)
            {
                removeAdsButton.gameObject.SetActive(false);
                return;
            }

            removeAdsButton.gameObject.SetActive(true);
        }

        public void ActiveSideEffectImages(float duration = 1.2f)
        {
            foreach (Image image in sideEffectImages)
            {
                image.gameObject.SetActive(true);
                PrimeTween.Tween.Alpha(image, 1f, duration / 2f).OnComplete(() =>
                {
                    PrimeTween.Tween.Alpha(image, 0f, duration / 2f).OnComplete((() =>
                    {
                        image.gameObject.SetActive(false);
                    }));
                    
                });
            }
            
            
            
        }

        #endregion

        #region Helper Methods

        public void EnableInteract()
        {
            DebugLogger.Log(message:$"");
            isInteractable = true;
            foreach (Booster booster in boosterList)
            {
                booster.UpdateState();
            }
        }

        public void DisableInteract()
        {
            DebugLogger.Log(message:$"");
            isInteractable = false;
            
        }

        public void ToggleIsHandTutShowing(bool toggleValue)
        {
            isHandTutShowing = toggleValue;
        }
        
        public void LockBoosters()
        {
            DebugLogger.Log(message:$"");
            foreach (Booster booster in boosterList)
            {
                booster.SetState(Booster.BoosterState.LOCK);
            }
        }

        public void LockBooster(int boosterId)
        {
            boosterList[boosterId].SetState(Booster.BoosterState.LOCK);
        }

        #endregion

        #region Override Methods

        public override void Show(params object[] parameters)
        {
            base.Show(parameters);
            /*if (DataManager.Ins.PProgressData.LevelId < GamePlayConst.LV_BACK_TO_HOME)
            {
                backButton.gameObject.SetActive(false);
            }*/

#if USE_AD_NETWORKS
            AdsManager.Ins.Hide_MRec_MAX();
#endif

            ShowButtons();

            UpdateLocalizedTextWithParams();
        }

        public override void Hide(params object[] parameters)
        {
            base.Hide(parameters);
        }

        #endregion

        #region Button Events

        private void OnClickSettings()
        {
            if (!isInteractable)
            {
                var args = new ToastArgs
                {
                    Message = LeanLocalization.GetTranslationText(LeanLocalizedConst.READYING),
                    CustomAnchoredPos = AnimationConst.UPPER_ANCHORED_POS,
                    TextColor = Color.white,
                    TextFont = UGUIManager.Ins.DefaultFont,
                    customDuration = 0.5f,
                };
                AnimationManager.Ins.Play(args);
                return;
            }
            
            if (isHandTutShowing)
            {
                DebugLogger.Log(message:$"The hand tut is showing");
                var args = new ToastArgs
                {
                    Message = LeanLocalization.GetTranslationText(LeanLocalizedConst.READYING),
                    CustomAnchoredPos = AnimationConst.UPPER_ANCHORED_POS,
                    TextColor = Color.white,
                    TextFont = UGUIManager.Ins.DefaultFont,
                    customDuration = 0.5f,
                };
                AnimationManager.Ins.Play(args);
                return;   
            }

            UGUIManager.Ins.ShowGUI(UGUIManager.Ins.GUISettings);
        }

        private void OnClickShop()
        {
            if (!isInteractable)
            {
                var args = new ToastArgs
                {
                    Message = LeanLocalization.GetTranslationText(LeanLocalizedConst.READYING),
                    CustomAnchoredPos = AnimationConst.UPPER_ANCHORED_POS,
                    TextColor = Color.white,
                    TextFont = UGUIManager.Ins.DefaultFont,
                    customDuration = 0.5f,
                };
                AnimationManager.Ins.Play(args);
                return;
            }

            if (isHandTutShowing)
            {
                DebugLogger.Log(message:$"The hand tut is showing");
                var args = new ToastArgs
                {
                    Message = LeanLocalization.GetTranslationText(LeanLocalizedConst.READYING),
                    CustomAnchoredPos = AnimationConst.UPPER_ANCHORED_POS,
                    TextColor = Color.white,
                    TextFont = UGUIManager.Ins.DefaultFont,
                    customDuration = 0.5f,
                };
                AnimationManager.Ins.Play(args);
                return;   
            }

            UGUIManager.Ins.ShowGUI(UGUIManager.Ins.GUIShop);
        }

        private void OnClickReplayLevel()
        {
            if (!isInteractable)
            {
                var args = new ToastArgs
                {
                    Message = LeanLocalization.GetTranslationText(LeanLocalizedConst.READYING),
                    CustomAnchoredPos = AnimationConst.UPPER_ANCHORED_POS,
                    TextColor = Color.white,
                    TextFont = UGUIManager.Ins.DefaultFont,
                    customDuration = 0.5f,
                };
                AnimationManager.Ins.Play(args);
                return;
            }

            UGUIManager.Ins.ShowGUI(UGUIManager.Ins.GUILoadingScreen);
            HideButtons();
            
            StartCoroutine(IEShowInter());

            void OnInterClose()
            {
                StartCoroutine(IEInterClose());
            }

            IEnumerator IEInterClose()
            {
                yield return YieldHelper.WaitForSeconds(0.1f);
                MMEventManager.TriggerEvent(new ELevelLoad_Fire()
                {
                    levelId = DataManager.Ins.PProgressData.LevelId
                });
            }

            IEnumerator IEShowInter()
            {
                yield return YieldHelper.WaitForSeconds(0.5f);
                StartCoroutine(IEInterClose());
               #if USE_AD_NETWORKS
                AdsManager.Ins.TryShow_DoubleInter(OnInterClose);
               #endif
            }

            #if USE_FIREBASE_ANALYTICS
            AnalyticsAdapter.Log_LevelState(DataManager.Ins.PProgressData.LevelId + 1, AnalyticsConst.EVENT_LEVEL_RESTARTED);
            #endif
        }

        private void OnClickBack()
        {
            if (!isInteractable)
            {
                var args = new ToastArgs
                {
                    Message = LeanLocalization.GetTranslationText(LeanLocalizedConst.READYING),
                    CustomAnchoredPos = AnimationConst.UPPER_ANCHORED_POS,
                    TextColor = Color.white,
                    TextFont = UGUIManager.Ins.DefaultFont,
                    customDuration = 0.5f,
                };
                AnimationManager.Ins.Play(args);
                return;
            }
            
            UGUIManager.Ins.HideGUI(this);

            float loadDuration = UGUIConst.FAKE_LOAD_DURATION;
            UGUIManager.Ins.ShowGUI(UGUIManager.Ins.GUILoadingScreen, delay: 0.1f, loadDuration);
            
            
            DebugLogger.Log(message: $"About to show gui home");
            UGUIManager.Ins.ShowGUI(UGUIManager.Ins.GUIHome, delay: loadDuration + 0.1f);  
            
            MMEventManager.TriggerEvent(new EGameQuit_Fire());
        }

        #endregion
    }

}