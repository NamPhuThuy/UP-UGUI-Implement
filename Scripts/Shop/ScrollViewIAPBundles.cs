using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Lean.Localization;
using MoreMountains.Tools;
using NamPhuThuy.DataManage;
using NamPhuThuy.UI;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace NamPhuThuy.UGUIImplement
{
    
    public class ScrollViewIAPBundles : MonoBehaviour, MMEventListener<ENoAdsActivated>
    {
        #region Private Serializable Fields

        [Header("View")] 
        [SerializeField] private RectTransform _content;
        [SerializeField] private GridLayoutGroup _contentGridLayoutGroup;
        [SerializeField] private ScrollRect _scrollRect;
        public ScrollRect ScrollRect => _scrollRect;
        
        [Header("IAP Packs")]
        [SerializeField] private IAPElementBundle iapElementBundlePrefab;
        [SerializeField] private IAPElementCoin iapElementCoinPrefab;
        [SerializeField] private IAPElementNoAds iapElementNoAdsPrefab;
        
        private Vector2 _elementSpacing = new Vector2(0f, 50f);
        private Vector2 _elementPaddingLeftRight = new Vector2(0f, 0f);
        private Vector2 _contentBuffer = new Vector2(0f, 50f);

        [Header("Components")] 
        [SerializeField] private GameObject noAdsElement;
        [SerializeField] private List<IAPElementNoAds> iapElementNoAdsList;
        #endregion

        #region Private Fields

        #endregion

        #region MonoBehaviour Callbacks

        private void Awake()
        {
            MMEventManager.RegistCurrentEvents(this);
        }

        private void OnDestroy()
        {
            MMEventManager.UnregistCurrentEvents(this);
        }

        #endregion

        #region Private Methods
        
        public void Setup()
        { 
            //GET REFERENCES
            _contentGridLayoutGroup = _content.GetComponent<GridLayoutGroup>();
            _scrollRect = GetComponent<ScrollRect>();
            

            _content.sizeDelta = _contentBuffer;
        }

        private void ExpandContentSizeDelta(RectTransform rect)
        {
            // Expand the content'size for the new element
            Vector2 size = rect.sizeDelta;
            _content.sizeDelta = new Vector2(_content.sizeDelta.x, _content.sizeDelta.y + size.y + _elementSpacing.y);
            
            // Set position for the new element
            rect.anchoredPosition = new Vector2(rect.anchoredPosition.x + _elementPaddingLeftRight.x, -(_content.sizeDelta.y - rect.sizeDelta.y / 2f - _elementSpacing.y));
        }
        
        private void LoadIAPBundle(IAPRecord iapRecord)
        {
            try
            {
                // Init Element
                IAPElementBundle element = Instantiate(iapElementBundlePrefab, _content.transform);
                RectTransform rect = element.GetComponent<RectTransform>();
                ExpandContentSizeDelta(rect);

                //Assign value 
                element.bundleNameText.GetComponent<LeanLocalizedTextMeshProUGUI>().TranslationName = iapRecord.BundleName;
                element.titleImage.sprite = iapRecord.TitleImage;
                element.titleImage.SetNativeSize();
                element.backgroundImage.sprite = iapRecord.BackgroundSprite;
                element.subTitleImage.sprite = iapRecord.SubTitleImage;
                
                for (int i = 0; i < iapRecord.Rewards.Count; i++)
                {
                    element.rewardAmountTexts[i].text = iapRecord.Rewards[i].amount.ToString();
                    element.rewardAmountTexts[i].gameObject.SetActive(true);
                }

                element.priceText.text = iapRecord.Price;
                element.iapPackId = iapRecord.BundleId;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        
        private void LoadIAPCoin(IAPRecord iapRecord)
        {
            try
            {
                // Init Element
                IAPElementCoin element = Instantiate(iapElementCoinPrefab, _content.transform);
                RectTransform rect = element.GetComponent<RectTransform>();
                ExpandContentSizeDelta(rect);

                //Assign value
                element.titleImage.sprite = iapRecord.TitleImage;
                element.titleImage.SetNativeSize();
                element.coinRewardText.text = iapRecord.Rewards.GetCoinAmount().ToString();
                element.priceText.text = iapRecord.Price;
                element.iapPackId = iapRecord.BundleId;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        
        private void LoadIAPNoAds(IAPRecord iapRecord)
        {
            try
            {
                // Init Element
                IAPElementNoAds element = Instantiate(iapElementNoAdsPrefab, _content.transform);
                // noAdsElement = element.gameObject;
                iapElementNoAdsList.Add(element);
                
                RectTransform rect = element.GetComponent<RectTransform>();
                ExpandContentSizeDelta(rect);

                //Assign value
                element.titleImage.sprite = iapRecord.TitleImage;
                element.bundleNameText.GetComponent<LeanLocalizedTextMeshProUGUI>().TranslationName = iapRecord.BundleName;
                element.descriptionText.GetComponent<LeanLocalizedTextMeshProUGUI>().TranslationName = iapRecord.Description;
                element.priceText.text = iapRecord.Price;
                element.iapPackId = iapRecord.BundleId;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        
        private void ClearContent()
        {
            foreach (Transform child in _content)
            {
                Destroy(child.gameObject);
            }
            
            _content.sizeDelta = _contentBuffer;
        }
        
        #endregion

        #region Public Methods

        public void SetupScrollViewContent()
        {
            //Resize the content-view-size
            

            foreach (var keyValuePair in DataManager.Ins.IAPDataShop.Data)
            {
                switch (keyValuePair.Value.Type)
                {
                    case IAPType.BUNDLE:
                        LoadIAPBundle(keyValuePair.Value);
                        break;
                    case IAPType.NO_ADS: 
                        if (!DataManager.Ins.PProgressData.IsAdsRemoved)
                            LoadIAPNoAds(keyValuePair.Value);
                        break;
                    case IAPType.COIN:
                        LoadIAPCoin(keyValuePair.Value);
                        break;
                }
            }
        }
        
        #endregion

        public void OnMMEvent(ENoAdsActivated eventArgs)
        {
            noAdsElement.SetActive(false);
            foreach (IAPElementNoAds elementNoAds in iapElementNoAdsList)
            {
                elementNoAds.gameObject.SetActive(false);
            }
            ClearContent();
            SetupScrollViewContent();
        }
    }

}