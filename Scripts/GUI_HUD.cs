using System;
using System.Collections;
using System.Collections.Generic;

using NamPhuThuy.Common;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using MoreMountains.Tools;

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

        [Header("Buttons")]
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button shopButton;
        [SerializeField] private Button replayButton;
        [SerializeField] private Button backButton;
        [SerializeField] private Button removeAdsButton;
        [SerializeField] private Button cheatButton;

        [Header("Texts")]
        [SerializeField] private TextMeshProUGUI levelTitleText;

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
            }
        }

        public void TurnOffTutorialImage()
        {
            if (tutorialImage == null) return;
            if (!tutorialImage.gameObject.activeSelf) return;

            tutorialImage.gameObject.SetActive(false);
        }

        
        void ValidateCheatMode()
        {
           
        }

        public void ValidateNoAdsButton()
        {
            removeAdsButton.gameObject.SetActive(false);
        }

        #endregion

        #region Helper Methods


        public void DisableInteract()
        {
            DebugLogger.Log(message:$"");
            isInteractable = false;
            
        }

        public void ToggleIsHandTutShowing(bool toggleValue)
        {
            isHandTutShowing = toggleValue;
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
            
        }

        private void OnClickShop()
        {
           
        }

        private void OnClickReplayLevel()
        {
           
        }

        private void OnClickBack()
        {
          
        }

        #endregion
    }

}