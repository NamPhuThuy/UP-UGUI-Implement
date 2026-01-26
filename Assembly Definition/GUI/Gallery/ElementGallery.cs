/*using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace NamPhuThuy.UI
{

    public class ElementGallery : MonoBehaviour
    {
        #region Private Serializable Fields

        [Header("UI Components")]
        public Image contentImage;
        public RectTransform imageFrame;
        public TextMeshProUGUI pictureIdText;
        public Image animatedTag;

        [Header("Stats")]
        public int pictureId;
        // public PlayerPictureData.LikeState likeState;

        [Header("Placeholder")]
        [SerializeField] private CanvasGroup placeholder;

        [SerializeField] private Button pictureButton;

        #endregion

        #region Private Fields

        #endregion

        #region MonoBehaviour Callbacks

        private void OnEnable()
        {
            pictureButton.onClick.AddListener(OnClickPicture);
        }

        private void OnDisable()
        {
            pictureButton.onClick.RemoveAllListeners();
        }

        #endregion

        #region Button Events

        private void OnClickPicture()
        {
            /*if (!DataManager.Ins.PictureDatas.allPictureDatas[pictureId].IsAvailable())
            {
                string message = VFXPopupTextMessage.VIDEO_DOWNLOADING;

                if (Application.internetReachability == NetworkReachability.NotReachable)
                {
                    message = VFXPopupTextMessage.CHECK_INTERNET_TO_DOWNLOAD_VIDEO;
                }

                VFXManager.Ins.PlayAt(
                    VFXType.POPUP_TEXT,
                    message: message,
                    initialParent: GUIManager.Ins.GUIGallery.transform);

                return;
            }

            GUIManager.Ins.ShowGUI(GUIManager.Ins.GUIPictureDetails, 0f, pictureId, GUIManager.Ins.GUIGallery.CurrentPictureIdList);#1#
        }

        #endregion

        #region Private Methods

        #endregion

        #region Public Methods

        public void FitImageToRectTransform()
        {
            contentImage.FitImageToRectTransform(imageFrame);
        }

        public void UpdatePictureIdText()
        {
            pictureIdText.text = $"{pictureId}";
            pictureIdText.gameObject.SetActive(true);
        }

        public void TurnOffPictureIdText()
        {
            pictureIdText.gameObject.SetActive(false);
        }

        public void EnableAnimatedTag(bool isEnable)
        {
            animatedTag.gameObject.SetActive(isEnable);
        }

        public void ShowPlaceholder()
        {
            placeholder.gameObject.SetActive(true);
        }

        public void HidePlaceholderImmediately()
        {
            placeholder.gameObject.SetActive(false);
        }

        public void HidePlaceholder()
        {
            placeholder.DOFade(0, duration: 0.3f).OnComplete(() =>
            {
                placeholder.gameObject.SetActive(false);
            });
        }

        #endregion
    }
}*/