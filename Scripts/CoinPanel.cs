using DG.Tweening;
using MoreMountains.Tools;
using NamPhuThuy.Common;
using TMPro;
using UnityEngine;


namespace NamPhuThuy.UGUIImplement
{

    public class CoinPanel : MonoBehaviour
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
                // Set coinText.text 
            }
        }

        private int targetValue = 12;
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
                targetValue,
                0.3f
            )
            .SetEase(Ease.Linear);
        }

        #endregion

        #region Public Methods

        public RectTransform CoinImage => coinImage;

        #endregion
    }
}