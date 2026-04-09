using System.Collections.Generic;


using NamPhuThuy.AnimateWithScripts;
using NamPhuThuy.DataManage;

using NamPhuThuy.IAPAdapter;


#if USE_LEAN_LOCALIZATION
using NamPhuThuy.Lean_Localization;
using Lean.Localization;
#endif

#if USE_FIREBASE_SERVICES
using NamPhuThuy.FirebaseAdapter;
#endif

#if USE_AD_NETWORKS
using NamPhuThuy.AdNetworkAdapter;
#endif


using UnityEngine;
using TMPro;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace NamPhuThuy.UGUIImplement
{

    public class GUINotEnoughCoin : GUIBase
    {
        #region Private Serializable Fields

        [Header("Flags")] 
        [SerializeField] private bool isInteractable = true;
        
        [Header("Reward Ads Part")]
        [SerializeField] private TextMeshProUGUI coinAmountText;
        [SerializeField] private ButtonClicky watchAdsButton;
        [SerializeField] private CoinPanel coinPanel;


        [Header("IAP Part")] 
        [SerializeField] private IAPRecord currentIAPRecord;
        [SerializeField] private Image bundleImage;
        [SerializeField] private TextMeshProUGUI bundlePriceText;
        [SerializeField] private TextMeshProUGUI bundleAmountText;
        [SerializeField] private Button buyBundleButton;
        private readonly string _defaultBundleId = "";
        [SerializeField] private string _currentBundleId;
        
        [Header("General Part")]
        [SerializeField] private ButtonClicky closeButton;
        [SerializeField] private Button moreOfferButton;
        

        #endregion

        #region Private Fields

        #endregion

        #region MonoBehaviour Callbacks

        private void OnEnable()
        {
            closeButton.onClick.AddListener(OnClickClose);
            watchAdsButton.onClick.AddListener(OnClickWatchAds);
            
            moreOfferButton.onClick.AddListener(OnClickMoreOffer);
            buyBundleButton.onClick.AddListener(OnClickBuyBundle);

            UpdateUI();
        }

        private void OnDisable()
        {
            closeButton.onClick.RemoveAllListeners();
            watchAdsButton.onClick.RemoveAllListeners();
            
            moreOfferButton.onClick.RemoveAllListeners();
            buyBundleButton.onClick.RemoveAllListeners();
        }

        #endregion

        #region Private Methods

        private void UpdateUI()
        {
            int coinAmount = 0;
            foreach (ResourceAmount reward in DataManager.Ins.EventRewardData.GetRecord(EventRewardType.WATCH_ADS_FREE_COINS).rewards)
            {
                if (reward.resourceType == ResourceType.COIN)
                    coinAmount += reward.amount;
            }   

            coinAmountText.text = $"+{coinAmount}";
        }

        private void OnClickClose()
        {
            if (isInteractable)
            {
                UGUIManager.Ins.HideGUI(this);    
            }
            else
            {
                // AudioManager.Ins.Play(AudioEnum.SFX_CONFIRM);
                AnimationManager.Ins.PlayBasicPopupText(LeanLocalization.GetTranslationText(LeanLocalizedConst.READYING));
            }
        }
        
        
        private void OnClickWatchAds()
        {
            DebugLogger.Log();
#if USE_AD_NETWORKS
            AdsManager.Ins.TryShow_RewardAd_MAX(OnRewardReceived, OnVideoNotAvailable, OnRewardHidden, AdWatchReason.FREE_COINS);

            void OnRewardReceived()
            {
                DebugLogger.Log(message:$"OnRewardReceived");
                List<ResourceAmount> rewards =
                    DataManager.Ins.EventRewardData.GetRecord(EventRewardType.WATCH_ADS_FREE_COINS).rewards;

                var args = new ItemFlyArgs
                {
                    AddValue = rewards.GetCoinAmount(),
                    PrevValue = DataManager.Ins.PInventoryData.Coin, 
                    TargetText = coinPanel.CoinText.transform,
                    TargetInteractTransform = coinPanel.transform, // For positioning the target
                    StartPosition = this.transform.position,
                    ItemAmount = 6,
                    ItemSprite = DataManager.Ins.ResourceData.GetResourceRecord(ResourceType.COIN).gameplayImage,
                    OnItemInteract = () =>
                    {
                        // TurnOnStatChangeVFX(coinText)
                        // AudioManager.Ins.Play(AudioEnum.SFX_COIN_3);
                    },
                    OnComplete = () =>
                    {
                        // AudioManager.Ins.Play(AudioEnum.SFX_COIN_1);
                        isInteractable = true;
                        DataManager.Ins.PInventoryData.TryApplyRewards(rewards, 1, isUseUpdateAnim: false); 
                    }
                };

                isInteractable = false;
                AnimationManager.Ins.Play(args);
            }

            void OnVideoNotAvailable()
            {
                DebugLogger.Log(message:$"OnVideoNotAvailable");
                var args = new ToastArgs
                {
                    Message = LeanLocalization.GetTranslationText(LeanLocalizedConst.CHECK_INTERNET),
                    CustomAnchoredPos = AnimationConst.UPPER_ANCHORED_POS,
                    TextFont = UGUIManager.Ins.DefaultFont,
                    TextColor = Color.white,
                    customDuration = 0.5f,
                };
                AnimationManager.Ins.Play(args);
            }

            void OnRewardHidden()
            {
                DebugLogger.Log(message:$"OnRewardHidden");
                
                AnalyticsAdapter.Log_RewardAd_Watched(DataManager.Ins.PProgressData.LevelId + 1, nameof(AdWatchPlace.GUI_NOT_ENOUGH_COIN));
            }
#else
            GUIManager.Ins.HideGUI(this);
#endif
        }

        
        private void OnClickMoreOffer()
        {
            UGUIManager.Ins.ShowGUI(UGUIManager.Ins.GUIShop);
        }
        
        private void OnClickBuyBundle()
        {
#if USE_UNITY_IAP
            IAPManager.Ins.BuyProduct(_currentBundleId); 
#else
            DebugLogger.Log(message:"Nothing happens")
#endif
        }
        #endregion

        #region Override Methods

        public override void Show(params object[] parameters)
        {
            base.Show(parameters);
            
            _currentBundleId = _defaultBundleId;
            if (parameters is { Length: > 0 } && parameters[0] is string id && !string.IsNullOrWhiteSpace(id))
            {
                DebugLogger.Log(message:$"set new id");
                _currentBundleId = id;
            }

            DebugLogger.LogDictionary(DataManager.Ins.IAPDataShop.Data);
            
            currentIAPRecord = DataManager.Ins.IAPDataShop.GetRecord(_currentBundleId);

            if (currentIAPRecord != null)
            {
                // bundleImage.sprite = currentIAPData.titleImage;
                bundlePriceText.text = currentIAPRecord.Price;
                bundleAmountText.text = $"+{currentIAPRecord.Rewards.GetCoinAmount()}";
            }
        }

        public override void Hide(params object[] parameters)
        {
            base.Hide(parameters);
        }

        #endregion
        
        #region Public Methods
        #endregion

    }
}