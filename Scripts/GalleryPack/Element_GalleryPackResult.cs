using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Lean.Localization;
using NamPhuThuy.AnimateWithScripts;
using NamPhuThuy.DataManage;
using NamPhuThuy.Lean_Localization;
using DebugLogger = NamPhuThuy.Common.DebugLogger;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace NamPhuThuy.UGUIImplement
{

    public class Element_GalleryPackResult : MonoBehaviour
    {
        #region Private Serializable Fields

        public int currentPictureId;
        public Image currentPicture;
        public RectTransform pictureFrame;

        [Header("Placeholder")]
        [SerializeField] private CanvasGroup placeholder;
        [SerializeField] private Sprite placeholderSprite;

        #endregion

        #region Private Fields

        #endregion

        #region MonoBehaviour Callbacks


        #endregion

        #region Private Methods
        #endregion

        #region Remote Asset

        public IEnumerator WaitForRemoteAsset()
        {
            float interval = 0.2f;

            WaitForSeconds waitForSeconds = new WaitForSeconds(interval);

            /*AlbumRecord albumRecord = DataManager.Ins.AlbumData.data[currentPictureId];

            const float maxCooldownCheckInternet = 5;

            float cooldownCheckInternet = maxCooldownCheckInternet;
            bool isFirstElement = transform.GetSiblingIndex() == 0;

            if (albumRecord.IsLocalAvailable())
            {
                HidePlaceholderFast();
            }
            else
            {
                ShowPlaceholder();
            }

            while (!albumRecord.IsLocalAvailable())
            {
                if (isFirstElement && cooldownCheckInternet <= 0)
                {
                    if (Application.internetReachability == NetworkReachability.NotReachable)
                    {
                        var args = new ToastArgs
                        {
                            Message = LeanLocalization.GetTranslationText(LeanLocalizedConst.CHECK_INTERNET),
                            CustomAnchoredPos = AnimationConst.UPPER_ANCHORED_POS,
                            TextFont = GUIManager.Ins.defaultFont,
                            TextColor = Color.white,
                            customDuration = 0.5f,
                        };
                        AnimationManager.Ins.Play(args);
                    }

                    cooldownCheckInternet = maxCooldownCheckInternet;
                }
                else
                {
                    cooldownCheckInternet -= interval;
                }


                // _ = DataManager.Ins.FetchPictureData(currentPictureId);
                yield return waitForSeconds;
            }

            HidePlaceholder();

            currentPicture.sprite = albumRecord.localMainSprite;
            currentPicture.FitImageToRectTransformScreenSpaceOverlay(pictureFrame);*/
            yield return null;
        }

        #endregion

        #region Placeholder

        public void ShowPlaceholder()
        {
            placeholder.gameObject.SetActive(true);

            currentPicture.sprite = placeholderSprite;
            currentPicture.FitImageToRectTransformScreenSpaceOverlay(pictureFrame);
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