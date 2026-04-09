using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;



#if UNITY_EDITOR
using UnityEditor;
#endif

namespace NamPhuThuy.UGUIImplement
{

    public class GUIShop : GUIBase
    {
        #region Private Serializable Fields

        [Header("Buttons")]
        [SerializeField] private ButtonClicky backButton;

        [Header("Texts")]
        [Header("Components")]
        [SerializeField] private CoinPanel coinPanel;
        public CoinPanel CoinPanel => coinPanel;
        [SerializeField] private ScrollViewIAPBundles scrollViewIAPBundles;
        

        // [SerializeField] private GameObject noAdsElement;
        #endregion

        #region Private Fields

        #endregion

        #region MonoBehaviour Callbacks

        private void Start()
        {
            scrollViewIAPBundles.Setup();
            scrollViewIAPBundles.SetupScrollViewContent();
        }

        void OnEnable()
        {
            backButton.onClick.AddListener(OnClickBack);
            // ValidateNoAdsElement();
        }


        void OnDisable()
        {
            backButton.onClick.RemoveAllListeners();
        }

        #endregion

        #region Button Events

        private void OnClickBack()
        {
            /*
            if (VFXManager.Ins.isCoinFlyActive)
            {
                VFXManager.Ins.PlayAt(
                        VFXType.POPUP_TEXT,
                        message: VFXPopupTextMessage.CANT_CLICK,
                        initialParent: GUIManager.Ins.GUIShop.transform);

                return;
            }
            */

            Hide();
        }

        #endregion

        #region Override Methods
        
        public override void Show(params object[] parameters)
        {
            base.Show(parameters);

            scrollViewIAPBundles.ScrollRect.LockVerticalScrollALittle();
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

        /*private void ValidateNoAdsElement()
        {
            if (DataManager.Ins.PlayerData.IsRemoveAds)
            {
                noAdsElement.SetActive(false);
            }
            else
            {
                noAdsElement.SetActive(true);
            }
        }*/

        #endregion

        #region Public Methods
      
        #endregion
    }
}
