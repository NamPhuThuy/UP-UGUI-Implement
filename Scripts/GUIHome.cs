
using System;
using System.Collections;
using System.Collections.Generic;
using Lean.Localization;
using MoreMountains.Tools;



using NamPhuThuy.AnimateWithScripts;
using NamPhuThuy.Common;
using NamPhuThuy.DataManage;
using NamPhuThuy.Lean_Localization;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;
using DebugLogger = NamPhuThuy.Common.DebugLogger;

#if USE_AD_NETWORKS
using NamPhuThuy.AdNetworkAdapter;
#endif

namespace NamPhuThuy.UGUIImplement
{

    public class GUIHome : GUIBase
    {
        #region Private Serializable Fields

        #region UI Elements

        [Header("Buttons")]
        [SerializeField] private ButtonClicky settingsButton;
        [SerializeField] private ButtonClicky shopButton;
        [SerializeField] private ButtonClicky addCoinButton;
        [SerializeField] private ButtonClicky galleryButton;
        [SerializeField] private Button cheatButton;
        [SerializeField] private Button forturnWheelButton;
        [SerializeField] private Button vipButton;
        [SerializeField] private Button favoriteButton;

        [Space(5)]
        [SerializeField] private ButtonClicky galleryPackButton;
        [SerializeField] private ButtonClicky noAdsButton;

        [SerializeField] private ButtonClicky playButton;

        [Header("Texts")]
        [SerializeField] private TextMeshProUGUI levelTitleText;
        [SerializeField] private LeanLocalizedTextMeshProUGUI levelTitleTextLocalized;
        [SerializeField] private TextMeshProUGUI deviceIdText;

        #endregion

        [Header("Stats")]
        private readonly float _intervalTime = .7f;
        [SerializeField] private float timer;
        #endregion

        #region Private Fields

        private List<string> _testDeviceIdList = new List<string>()
        {
            "4170b46a73985e5afe57eda31dafc77d",
            "7b7c834f9ff4bf0b9e018fc4fc2414f9ea7c6d3d",
            "4f1feb0ce86f58865396fb3e9298ed50"
        };

        #endregion

        #region MonoBehaviour Callbacks

        private void Awake()
        {
            settingsButton?.onClick.AddListener(OnClickSettings);
            shopButton?.onClick.AddListener(OnClickShop);
            addCoinButton?.onClick.AddListener(OnClickShop);
            galleryButton?.onClick.AddListener(OnClickGallery);
            // forturnWheelButton?.onClick.AddListener(OnClickFortuneWheel);
            // vipButton?.onClick.AddListener(OnClickVIP);
            // favoriteButton.onClick.AddListener(() => GUIManager.Ins.ShowGUI(GUIManager.Ins.GUIFavorite));
            
            cheatButton?.onClick.AddListener((() => UGUIManager.Ins.ShowGUI(UGUIManager.Ins.GUICheat)));

            galleryPackButton?.onClick.AddListener(OnClickGalleryPack);
            
            playButton?.onClick.AddListener(OnClickPlay);
            
            noAdsButton?.onClick.AddListener((() => { UGUIManager.Ins.ShowGUI(UGUIManager.Ins.GUINoAds); }));
        }
        
        private void OnDestroy()
        {
            settingsButton.onClick.RemoveAllListeners();
            playButton.onClick.RemoveAllListeners();
            shopButton.onClick.RemoveAllListeners();
            addCoinButton.onClick.RemoveAllListeners();
            galleryButton.onClick.RemoveAllListeners();
            // forturnWheelButton.onClick.RemoveAllListeners();
            // vipButton.onClick.RemoveAllListeners();
            // favoriteButton.onClick.RemoveAllListeners();

            cheatButton.onClick.RemoveAllListeners();

            galleryPackButton.onClick.RemoveAllListeners();
            noAdsButton.onClick.RemoveAllListeners();
        }

        void OnEnable()
        {
            if (!PlayerPrefs.HasKey("PersistentID"))
            {
                string newId = System.Guid.NewGuid().ToString();
                PlayerPrefs.SetString("PersistentID", newId);
                PlayerPrefs.Save();
            }
            deviceIdText.text = PlayerPrefs.GetString("PersistentID");

            ValidateCheatMode();
            ValidateNoAdsButton();

            UpdateLocalizedTextWithParams();
        }

      

        private void Update()
        {
            timer -= Time.deltaTime;
            if (timer < 0f)
            {
                // enable Play Button
                playButton.interactable = true;
            }
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
                
                int level = DataManager.Ins.PProgressData.LevelId + 1;
                levelTitleTextLocalized.UpdateTranslationWithParameter(LeanLocalizedConst.PARAM_LEVEL, level.ToString());
            }
        }

        #endregion

        #region Private Methods

        private void ValidateCheatMode()
        {
            /*if (!CheatManager.Ins.IsCheatMode)
            {
                cheatButton.gameObject.SetActive(false);
                return;
            }*/

            // cheatButton.gameObject.SetActive(true);
        }

        public void ValidateNoAdsButton()
        {
            DebugLogger.Log();
            if (DataManager.Ins.PProgressData.IsAdsRemoved)
            {
                noAdsButton.gameObject.SetActive(false);
                return;
            }

            noAdsButton.gameObject.SetActive(true);
        }

        private void HideButtons()
        {
            playButton.gameObject.SetActive(false);
            noAdsButton.gameObject.SetActive(false);
            cheatButton.gameObject.SetActive(false);
            
            
            settingsButton.gameObject.SetActive(false);
            shopButton.gameObject.SetActive(false);
            addCoinButton.gameObject.SetActive(false);
            galleryButton.gameObject.SetActive(false);
            // forturnWheelButton.gameObject.SetActive(false);
            // vipButton.gameObject.SetActive(false);
            // favoriteButton.gameObject.SetActive(false);
            galleryPackButton.gameObject.SetActive(false);
        }

        private void ShowButtons()
        {
            playButton.gameObject.SetActive(true);
           ValidateCheatMode();
           ValidateNoAdsButton();
           
           settingsButton.gameObject.SetActive(true);
           shopButton.gameObject.SetActive(true);
           addCoinButton.gameObject.SetActive(true);
           galleryButton.gameObject.SetActive(true);
           // forturnWheelButton.gameObject.SetActive(true);
           // vipButton.gameObject.SetActive(true);
           // favoriteButton.gameObject.SetActive(true);
           galleryPackButton.gameObject.SetActive(true);
        }
        #endregion

        #region Override Methods

        public override void Show(params object[] parameters)
        {
            base.Show(parameters);
           
            
            ShowButtons();
            UpdateLocalizedTextWithParams();
        }

        public override void Hide(params object[] parameters)
        {
            base.Hide(parameters);
        }

        public override void HideFast(params object[] parameters)
        {
            base.HideFast(parameters);
        }

        #endregion

        #region Button Events

        private void OnClickSettings()
        {
            UGUIManager.Ins.ShowGUI(UGUIManager.Ins.GUISettings);
        }
        
        private Action onLoadingComplete = () =>
        {
            MMEventManager.TriggerEvent(new ELevelLoad_Fire()
            {
                levelId = DataManager.Ins.PProgressData.LevelId
            });
        };

        private void OnClickPlay()
        {
            if (timer > 0f)
            {
                var args = new ToastArgs
                {
                    message = LeanLocalization.GetTranslationText(LeanLocalizedConst.READYING),
                    customAnchoredPos = AnimationConst.UPPER_ANCHORED_POS,
                    textFont = UGUIManager.Ins.DefaultFont,
                    textColor = Color.white,
                    customDuration = 0.5f,
                };
                AnimationManager.Ins.Play(args);
            }
            timer = _intervalTime;

#if USE_AD_NETWORKS
            if (AdsManager.Ins.CanShow_DoubleInter())
            {
                UGUIManager.Ins.ShowGUI(UGUIManager.Ins.GUILoadingScreen, 0f, UGUIConst.FAKE_LOAD_DURATION, onLoadingComplete);
                PrimeTween.Tween.Delay(AdsConst.MIN_MREC_DURATION, () =>
                {
                    AdsManager.Ins.TryShow_DoubleInter(null, true, "level_play");
                });
            }
            else
            {
                UGUIManager.Ins.ShowGUI(UGUIManager.Ins.GUILoadingScreen, 0f, AdsConst.MIN_MREC_DURATION, onLoadingComplete);
            }
#else
            UGUIManager.Ins.ShowGUI(UGUIManager.Ins.GUILoadingScreen, 0f, 1f, onLoadingComplete);
#endif
            
            HideButtons();
            playButton.interactable = false;
            
        }

        private void OnClickShop()
        {
            UGUIManager.Ins.ShowGUI(UGUIManager.Ins.GUIShop);
        }

        private void OnClickGallery()
        {
            UGUIManager.Ins.ShowGUI(UGUIManager.Ins.GUIGallery);
        }

        private void OnClickGalleryPack()
        {
            UGUIManager.Ins.ShowGUI(UGUIManager.Ins.GUIGalleryPack);
        }

        private void OnClickFortuneWheel()
        {
            // GUIManager.Ins.ShowGUI(GUIManager.Ins.GUIFortuneWheel);
        }

        private void OnClickVIP()
        {
            AnimationManager.Ins.PlayBasicPopupText(LeanLocalizedConst.COMING_SOON);
#if USE_AUDIO
            // AudioManager.Ins.Play(AudioEnum.SFX_CONFIRM); 
#endif
            
            return;
            
            UGUIManager.Ins.ShowGUI(UGUIManager.Ins.GUIVip);
        }

        #endregion
    }
}