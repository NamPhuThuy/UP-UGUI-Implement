using System;
using System.Collections;
using System.Collections.Generic;
using Lean.Localization;
using NamPhuThuy.AnimateWithScripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using NamPhuThuy.DataManage;
using NamPhuThuy.Lean_Localization;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace NamPhuThuy.UGUIImplement
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
        public AlbumData.LikeState likeState;

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
            /*if (!DataManager.Ins.AlbumData.data[pictureId].IsLocalAvailable())
            {
                string message = LeanLocalizedConst.PICTURE_DOWNLOADING;

                if (Application.internetReachability == NetworkReachability.NotReachable)
                {
                    message = LeanLocalizedConst.CHECK_INTERNET;
                }

                var args = new ToastArgs
                {
                    Message = LeanLocalization.GetTranslationText(message),
                    CustomAnchoredPos = AnimationConst.UPPER_ANCHORED_POS,
                    TextFont = GUIManager.Ins.DefaultFont,
                    TextColor = Color.white,
                    customDuration = 0.5f,
                };
                AnimationManager.Ins.Play(args);

                return;
            }

            GUIManager.Ins.ShowGUI(GUIManager.Ins.GUIPictureDetails, 0f, pictureId, GUIManager.Ins.GUIGallery.CurrentPictureIdList);*/
        }

        #endregion

        #region Private Methods

        #endregion

        #region Public Methods

        public void FitImageToRectTransform()
        {
            contentImage.FitImageToRectTransformScreenSpaceOverlay(imageFrame);
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

        public void ToggleAnimatedTag(bool isEnable)
        {
            animatedTag.gameObject.SetActive(isEnable);
        }

        public void ShowPlaceholder()
        {
            placeholder.gameObject.SetActive(true);
        }

        public void HidePlaceholderFast()
        {
            placeholder.gameObject.SetActive(false);
        }

        public void HidePlaceholder()
        {
            PrimeTween.Tween.Alpha(placeholder, 0, duration: 0.3f).OnComplete(() =>
            {
                placeholder.gameObject.SetActive(false);
            });
        }

        #endregion
    }
}