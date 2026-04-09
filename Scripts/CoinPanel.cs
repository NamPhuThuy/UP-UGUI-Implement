using DG.Tweening;
using MoreMountains.Tools;
using NamPhuThuy.Common;
using NamPhuThuy.DataManage;
using TMPro;
using UnityEngine;


namespace NamPhuThuy.UGUIImplement
{

    public class CoinPanel : MonoBehaviour, MMEventListener<EResourceUpdated>
    {
        #region Private Serializable Fields

        [SerializeField] private TextMeshProUGUI coinText;
        public TextMeshProUGUI CoinText => coinText;

        [SerializeField] private RectTransform coinImage;

        #endregion

        #region MonoBehaviour Callbacks

        void OnEnable()
        {
            UpdateUI(false);
            MMEventManager.RegistCurrentEvents(this);
        }

        private void OnDisable()
        {
            MMEventManager.UnregistCurrentEvents(this);
        }

        #endregion

        #region Private Methods

        private void UpdateUI(bool isUseUpdateAnim = true)
        {
            // DebugLogger.Log();
            // The cost of a null check (if (coinText != null)) is negligible compared to UI updates or string conversions.
            if (coinText == null) return;

            if (isUseUpdateAnim)
            {
                AnimateCoinUpdate();    
            }
            else
            {
                coinText.text = DataManager.Ins.PInventoryData.Coin.ToString();
            }
        }

        private void AnimateCoinUpdate()
        {
            int currentCoin = int.Parse(CoinText.text);

            int currentValue = currentCoin;

            DOTween.To
            (
                () => currentValue
                , x =>
                {
                    currentValue = x;
                    coinText.text = currentValue.ToString();
                },
                DataManager.Ins.PInventoryData.Coin,
                0.3f
            )
            .SetEase(Ease.Linear);
        }

        #endregion

        #region Public Methods

        public RectTransform CoinImage => coinImage;

        #endregion

        #region Events Listen

        public void OnMMEvent(EResourceUpdated eventArgs)
        {
            if (eventArgs.ResourceType == ResourceType.COIN)
                UpdateUI();
        }

        #endregion


    }
}