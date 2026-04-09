using System.Collections;
using Lean.Localization;
using NamPhuThuy.AnimateWithScripts;
using NamPhuThuy.Common;
using NamPhuThuy.IAPAdapter;
using NamPhuThuy.Lean_Localization;
using NamPhuThuy.UGUIImplement;
using UnityEngine;
using UnityEngine.UI;

namespace NamPhuThuy
{

    public class IAPElementBase : MonoBehaviour
    {
        #region Private Serializable Fields

        [Header("General")]
        [SerializeField] protected Button buyButton;
        public string iapPackId;

        #endregion

        protected bool IsBuying;
        private Coroutine _reEnableCoroutine;

        #region MonoBehaviour Callbacks

        protected virtual void OnEnable()
        {
            IAPManager.purchaseFlowFinishedEvent += OnPurchaseFlowFinished;
            IsBuying = false;
        }

        protected virtual void OnDisable()
        {
            IAPManager.purchaseFlowFinishedEvent -= OnPurchaseFlowFinished;
            IsBuying = false;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// [FIX 6.1] Called by subclasses instead of directly calling IAPManager.Ins.BuyProduct.
        /// Uses a flag to prevent double-taps, resets on purchase flow finish or timeout.
        /// </summary>
        protected void BuyWithGuard()
        {
            if (IsBuying)
            {
                var args = new ToastArgs
                {
                    Message = LeanLocalization.GetTranslationText(LeanLocalizedConst.READYING),
                    CustomAnchoredPos = AnimationConst.UPPER_ANCHORED_POS,
                    TextFont = UGUIManager.Ins.DefaultFont,
                    TextColor = Color.white,
                    customDuration = 0.5f,
                };
                AnimationManager.Ins.Play(args);
                return;
            }
            IsBuying = true;

            IAPManager.Ins.BuyProduct(iapPackId);

            // Safety timeout: reset flag if purchase flow never completes (e.g., store dialog dismissed by OS)
            if (_reEnableCoroutine != null) StopCoroutine(_reEnableCoroutine);
            _reEnableCoroutine = StartCoroutine(ResetBuyingFlagAfterTimeout());
        }

        private IEnumerator ResetBuyingFlagAfterTimeout()
        {
            yield return YieldHelper.GetRealtime(10f);
            IsBuying = false;
            _reEnableCoroutine = null;
        }

        private void OnPurchaseFlowFinished()
        {
            if (_reEnableCoroutine != null)
            {
                StopCoroutine(_reEnableCoroutine);
                _reEnableCoroutine = null;
            }

            IsBuying = false;
        }

        #endregion

        #region Public Methods
        #endregion
    }
}