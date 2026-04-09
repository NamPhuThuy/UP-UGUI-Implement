using NamPhuThuy.DataManage;
using NamPhuThuy.IAPAdapter;
using TMPro;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace NamPhuThuy.UGUIImplement
{
    
    public class GUI_NoAds : GUIBase
    {
        #region Private Serializable Fields
        
        [Header("Buttons")] 
        [SerializeField] private ButtonClicky buyButton;
        [SerializeField] private ButtonClicky nextButton;

        [Header("Texts")] 
        [SerializeField] private TextMeshProUGUI priceText;
        
        #endregion

        #region MonoBehaviour Callbacks

        private void Awake()
        {
            buyButton.onClick.AddListener(OnClickBuy);
            nextButton.onClick.AddListener((() => { Hide();}));
        }

        private void OnDestroy()
        {
            buyButton.onClick.RemoveAllListeners();
            nextButton.onClick.RemoveAllListeners();
        }

        #endregion

        #region Override Methods

        public override void Show(params object[] parameters)
        {
            base.Show(parameters);
            
            UpdateUI();
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

        #region Private Methods
        
        private void OnClickBuy()
        {
            IAPManager.Ins.BuyProduct(IAPConst.REMOVEADS_PACK_01_ID);
        }

        private void UpdateUI()
        {
            IAPRecord iapData = DataManager.Ins.IAPDataShop.GetRecord(IAPConst.REMOVEADS_PACK_01_ID);
            priceText.text = iapData.Price;
        }
        
        
        #endregion

        #region Public Methods
        #endregion
    }
}