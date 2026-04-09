using System;
using NamPhuThuy.IAPAdapter;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NamPhuThuy
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
            // [FIX 6.3] Subscribe to price update events so stale "0.01$" is replaced when store responds
            IAPManager.localizedPriceFetchedEvent += RefreshPrice;
            RefreshPrice();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            IAPManager.localizedPriceFetchedEvent -= RefreshPrice;
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
            priceText.text = IAPManager.Ins.GetLocalizedPrice(iapPackId);
        }

        #endregion

        #region Public Methods
        #endregion
    }
}