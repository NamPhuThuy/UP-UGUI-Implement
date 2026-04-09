using System;
using NamPhuThuy.DataManage;
using NamPhuThuy.GirlGallery;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace NamPhuThuy.UGUIImplement
{

    public class ScrollViewGalleryPack : MonoBehaviour
    {
        #region Private Serializable Fields

        [Header("View")]
        [SerializeField] private RectTransform content;

        [SerializeField] private Element_GalleryPack elementGalleryPack;
        public Element_GalleryPack ElementGalleryPack => elementGalleryPack;

        #endregion

        #region Private Fields
        private Vector2 _elementSpacing = new Vector2(0f, 50f);
        private Vector2 _elementPaddingLeftRight = new Vector2(15f, 0f);
        private Vector2 _contentBuffer = new Vector2(0f, 50f);
        private float _currentElementPositionY;
        #endregion

        #region MonoBehaviour Callbacks

        void Start()
        {
            /*Setup();
            UpdateContent();

            // more space to scroll
            ExpandContentSizeDelta(elementGalleryPack.GetComponent<RectTransform>());*/
        }

        #endregion

        #region Private Methods

        public void Setup()
        {
            content.sizeDelta = _contentBuffer;
            _currentElementPositionY = -_elementSpacing.y;
        }

        public void ExpandContentSizeDelta(RectTransform rect)
        {
            // Expand the content'size for the new element
            Vector2 size = rect.sizeDelta;
            content.sizeDelta = new Vector2(content.sizeDelta.x, content.sizeDelta.y + size.y + _elementSpacing.y);

            // Set position for the new element
            rect.anchorMin = new Vector2(0.5f, 1);
            rect.anchorMax = new Vector2(0.5f, 1);

            rect.anchoredPosition = new Vector2(0, _currentElementPositionY - 0.5f * rect.sizeDelta.y);
            _currentElementPositionY -= rect.sizeDelta.y + _elementSpacing.y;
        }

        private void LoadGalleryPack(GalleryPackRecord packData)
        {
            // Debug.Log($"ScrolLViewGallery.LoadGalleryPack()");
            Element_GalleryPack element = Instantiate(elementGalleryPack, content.transform);

            RectTransform rect = element.GetComponent<RectTransform>();
            ExpandContentSizeDelta(rect);

            element.backGroundImage.sprite = packData.backgroundSprite;
            // element.descriptionImage.sprite = packData.descriptionSprite;
            element.titleImage.sprite = packData.titleSprite;
            element.titleText./*GetComponent<LeanLocalizedTextMeshProUGUI>().TranslationName*/text = packData.packName;

            element.priceText.text = packData.price.ToString();
            element.currentPackStyle = packData.packType;
            element.packPrice = packData.price;

            element.CheckUnlockedAllState();
        }

        public void UpdateContent()
        {
            /*foreach (GalleryPackRecord packData in DataManager.Ins.GalleryPackData.data)
            {
                LoadGalleryPack(packData);
            }*/
        }
        #endregion

        #region Public Methods
        #endregion
    }
}