using System;
using System.Collections;
using System.Collections.Generic;
using NamPhuThuy.IAPAdapter;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace NamPhuThuy
{

    public class IAPElementNoAds : IAPElementBase
    {
        #region Private Serializable Fields

        [Header("Texts")]
        public TextMeshProUGUI priceText;
        public TextMeshProUGUI bundleNameText;
        public TextMeshProUGUI descriptionText;

        [Header("Images")]
        public Image titleImage;

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

        #endregion

        private void RefreshPrice()
        {
            priceText.text = IAPManager.Ins.GetLocalizedPrice(iapPackId);
        }

        #region Public Methods
        #endregion
    }
}