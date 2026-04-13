using System;

using TMPro;
using UnityEngine;
using UnityEngine.UI;


#if USE_UNITY_IAP
using NamPhuThuy.IAPAdapter;
#endif

namespace NamPhuThuy.UGUIImplement
{

    public class IAPElementCoin : IAPElementBase
    {
        #region Private Serializable Fields

        [Header("Texts")]
        public TextMeshProUGUI priceText;
        public TextMeshProUGUI coinRewardText;



        [Header("Images")]
        public Image titleImage;

        #endregion

        #region Private Fields

        #endregion

        #region MonoBehaviour Callbacks

        private void Awake()
        {
              buyButton.onClick.AddListener(OnClickBuy);
        }

        private void OnDestroy()
        {
            buyButton.onClick.RemoveAllListeners();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
#if USE_UNITY_IAP
            IAPManager.localizedPriceFetchedEvent += RefreshPrice;
#endif
            RefreshPrice();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
#if USE_UNITY_IAP
            IAPManager.localizedPriceFetchedEvent -= RefreshPrice;
#endif
        }

        #region Button Events

        private void OnClickBuy()
        {
            BuyWithGuard();
        }

        #endregion

        #endregion

        #region Private Methods

        private void RefreshPrice()
        {
#if USE_UNITY_IAP
            priceText.text = IAPManager.Ins.GetLocalizedPrice(iapPackId);
#endif
        }

        #endregion

        #region Public Methods
        #endregion
    }
}