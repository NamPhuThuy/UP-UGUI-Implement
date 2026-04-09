using System;
using System.Collections.Generic;
using MoreMountains.Tools;
using NamPhuThuy.IAPAdapter;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NamPhuThuy.UGUIImplement
{

    public class IAPElementBundle : IAPElementBase, MMEventListener<EIAPInfoFetched>
    {
        #region Private Serializable Fields

        [Header("Texts")]
        public TextMeshProUGUI priceText;
        public TextMeshProUGUI bundleNameText;
        public TextMeshProUGUI[] rewardAmountTexts;
        public Image[] rewardIcons;

        [Header("Images")]
        public Image titleImage;
        public Image subTitleImage;
        public Image backgroundImage;

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
            IAPManager.localizedPriceFetchedEvent += RefreshPrice;
            RefreshPrice();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            IAPManager.localizedPriceFetchedEvent -= RefreshPrice;
        }

        #endregion

        #region Button Events

        private void OnClickBuy()
        {
            BuyWithGuard();
        }

        // not called
        public void OnMMEvent(EIAPInfoFetched eventArgs)
        {
            foreach (var iapRecord in eventArgs.iapData)
            {
                if (iapRecord.BundleId == iapPackId)
                {
                    priceText.text = iapRecord.Price;

                    break;
                }
            }
        }

        #endregion


        #region Private Methods

        private void RefreshPrice()
        {
            priceText.text = IAPManager.Ins.GetLocalizedPrice(iapPackId);
        }

        #endregion

        #region Public Methods
        #endregion
    }
}