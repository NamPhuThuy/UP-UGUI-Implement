using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

using MoreMountains.Tools;

using NamPhuThuy.AnimateWithScripts;
using NamPhuThuy.Common;
using NamPhuThuy.DataManage;
using NamPhuThuy.FirebaseAdapter;

using UnityEngine.Networking;
using DebugLogger = NamPhuThuy.Common.DebugLogger;

#if USE_LEAN_LOCALIZATION
using Lean.Localization;
using NamPhuThuy.Lean_Localization;
#endif

#if USE_AD_NETWORKS
using NamPhuThuy.AdNetworkAdapter;
#endif


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace NamPhuThuy.UGUIImplement
{

    public class GUIPictureDetails : GUIBase
    {
        #region Public Fields

        public event Action ChangeLikeState;

        #endregion

        #region Private Serializable Fields

        [Header("Buttons")]
        [SerializeField] private ButtonClicky closeButton;

        [Space(5)]
        [SerializeField] private ButtonClicky prevButton;
        [SerializeField] private ButtonClicky nextButton;
        [SerializeField] private ButtonClicky shopButton;

        [Space(5)]
        [SerializeField] private ButtonClicky videoDownloadButton;
        [SerializeField] private ButtonClicky imageDownloadButton;
        [SerializeField] private ButtonClicky useAsBackgroundButton;
        [SerializeField] private Image imageCompleteTick;
        [SerializeField] private Image videoCompleteTick;

        [Space(5)]
        [SerializeField] private ButtonClicky likeButton;
        [SerializeField] private ButtonClicky dislikeButton;


        [Header("Buttons Switch")]
        [SerializeField] private ButtonSwitch likeSwitch;
        [SerializeField] private ButtonSwitch dislikeSwitch;
        [SerializeField] private ButtonSwitch imageDownloadSwitch;
        [SerializeField] private ButtonSwitch videoDownloadSwitch;

        [Header("Texts")]
        [SerializeField] private TextMeshProUGUI imageDownloadPriceText;
        [SerializeField] private TextMeshProUGUI videoDownloadPriceText;

        [Header("Images")]
        [SerializeField] private Image pictureImage;
        [SerializeField] private List<int> currentPictureIdList; // use this to implement the feature show next/prev picture

        [Header("Stats")]
        [SerializeField] private int currentPictureId;

        public int CurrentPictureId => currentPictureId;
        [SerializeField] private int currentListId;

        [SerializeField] private PAlbumRecord currentPAlbumRecord;
        [SerializeField] private AlbumRecord currentAlbumRecord;

        #endregion

        #region Private Fields

        
#if USE_LEAN_LOCALIZATION
        private string _downloadedNoti = LeanLocalizedConst.DOWNLOADED; 
#else
        private string _downloadedNoti = "";
#endif
        
        
        private RectTransform _downloadImageButtonRT;
        private Vector3 _downloadImageButtonPosition;

        #endregion

        #region Peek Picture
        private float _holdingDuration;
        private PrimeTween.Tween _peekingPictureTween;
        private bool _isPeekingPicture;
        #endregion

        #region MonoBehaviour Callbacks

        private void Awake()
        {
            GUIMaskInitialAlpha = 1f;
            
            likeSwitch = likeButton.GetComponent<ButtonSwitch>();
            dislikeSwitch = dislikeButton.GetComponent<ButtonSwitch>();
            
            closeButton.onClick.AddListener(OnClickClose);

            likeButton.onClick.AddListener(OnClickLike);
            dislikeButton.onClick.AddListener(OnClickDisLike);

            prevButton.onClick.AddListener(OnClickPrev);
            nextButton.onClick.AddListener(OnClickNext);
            shopButton?.onClick.AddListener(OnClickShop);

            videoDownloadButton.onClick.AddListener(OnClickVideoDownload);
            imageDownloadButton.onClick.AddListener(OnClickImageDownloadVer2);
            useAsBackgroundButton.onClick.AddListener(OnClickUseAsBackground);
        }

        private void OnDestroy()
        {
            closeButton.onClick.RemoveAllListeners();
            likeButton.onClick.RemoveAllListeners();
            dislikeButton.onClick.RemoveAllListeners();

            prevButton.onClick.RemoveAllListeners();
            nextButton.onClick.RemoveAllListeners();
            shopButton.onClick.RemoveListener(OnClickShop);

            videoDownloadButton.onClick.RemoveAllListeners();
            imageDownloadButton.onClick.RemoveAllListeners();
            useAsBackgroundButton.onClick.RemoveAllListeners();
        }

        void Update()
        {
            // PeekPicture();
        }

        #endregion

        #region Button Events

        private void OnClickClose()
        {
            Hide();
        }

        private void OnClickLike()
        {
            bool tmp = currentPAlbumRecord.likeState == AlbumData.LikeState.LIKE;
            DebugLogger.Log(message:$"likeState: {tmp}");

            if (tmp == true)
            {
                currentPAlbumRecord.likeState = AlbumData.LikeState.NONE;
            }
            else
            { 
                currentPAlbumRecord.likeState = AlbumData.LikeState.LIKE;
            }

            ChangeLikeState?.Invoke();
        }
        
        private void OnClickDisLike()
        {
            bool tmp = currentPAlbumRecord.likeState == AlbumData.LikeState.DISLIKE;
            DebugLogger.Log(message:$"disLikeState: {tmp}");

            if (tmp == true)
            {
                currentPAlbumRecord.likeState = AlbumData.LikeState.NONE;
            }
            else
            {
                currentPAlbumRecord.likeState = AlbumData.LikeState.DISLIKE;
            }

            ChangeLikeState?.Invoke();
        }

        public void OnClickPrev()
        {
            /*int nextListId = currentListId - 1;
            if (nextListId < 0)
            {
                var args = new ToastArgs
                {
                    Message = LeanLocalization.GetTranslationText(LeanLocalizedConst.NO_PICTURE),
                    CustomAnchoredPos = AnimationConst.UPPER_ANCHORED_POS,
                    TextColor = Color.white,
                    TextFont = GUIManager.Ins.defaultFont,
                    customDuration = 0.5f,
                };
                AnimationManager.Ins.Play(args);
                return;
            }

            int tmp = currentPictureIdList[nextListId];

            if (CheckValidatePictureData(tmp))
            {
                ChangePicture(tmp);

                currentAlbumRecord = DataManager.Ins.AlbumData.data[tmp];
                SetStateReactButtons();
                SetStateDownloadSwitches();
                CheckVideoAvailability();
                currentListId--;
            }
            else
            {
                var args = new ToastArgs
                {
                    Message = LeanLocalization.GetTranslationText(LeanLocalizedConst.NO_PICTURE),
                    CustomAnchoredPos = AnimationConst.UPPER_ANCHORED_POS,
                    TextColor = Color.white,
                    TextFont = GUIManager.Ins.DefaultFont,
                    customDuration = 0.5f,
                };
                AnimationManager.Ins.Play(args);
            }*/
        }

        public void OnClickNext()
        {
            /*int nextListId = currentListId + 1;
            if (nextListId >= currentPictureIdList.Count)
            {
                var args = new ToastArgs
                {
                    Message = LeanLocalization.GetTranslationText(LeanLocalizedConst.NO_PICTURE),
                    CustomAnchoredPos = AnimationConst.UPPER_ANCHORED_POS,
                    TextColor = Color.white,
                    TextFont = GUIManager.Ins.DefaultFont,
                    customDuration = 0.5f,
                };
                AnimationManager.Ins.Play(args);
                return;
            }


            int tmp = currentPictureIdList[nextListId];

            if (CheckValidatePictureData(tmp))
            {
                ChangePicture(tmp);

                currentAlbumRecord = DataManager.Ins.AlbumData.data[tmp];
                SetStateReactButtons();
                SetStateDownloadSwitches();
                CheckVideoAvailability();
                currentListId++;
            }
            else
            {
                var args = new ToastArgs
                {
                    Message = LeanLocalization.GetTranslationText(LeanLocalizedConst.NO_PICTURE),
                    CustomAnchoredPos = AnimationConst.UPPER_ANCHORED_POS,
                    TextColor = Color.white,
                    TextFont = GUIManager.Ins.DefaultFont,
                    customDuration = 0.5f,
                };
                AnimationManager.Ins.Play(args);
            }*/
        }

        private void OnClickShop()
        {
            UGUIManager.Ins.ShowGUI(UGUIManager.Ins.GUIShop);
        }

      

        /// <summary>
        /// Use coins to download video
        /// </summary>
        private void OnClickVideoDownload()
        {
            DebugLogger.Log();
            // _downloadedNoti = LeanLocalizedConst.DOWNLOADED;

            if (DataManager.Ins.PProgressData.IsVIP)
            {
                StartCoroutine(IEShowLoadingPopup());
                StartCoroutine(IEDownLoadVideo());

                return;
            }

            /*int downloadPrice = DataManager.Ins.AlbumData.downloadVideoPrice.GetCoinAmount();

            if (DataManager.Ins.PInventoryData.Coin >= downloadPrice)
            {
#if USE_AUDIO
                AudioManager.Ins.Play(AudioEnum.SFX_COIN_1);
#endif
                DataManager.Ins.PInventoryData.TrySpendCoins(downloadPrice);
                StartCoroutine(IEShowLoadingPopup());
                StartCoroutine(IEDownLoadVideo());
            }
            else
            {
                GUIManager.Ins.ShowGUI(GUIManager.Ins.GUINotEnoughCoin);
            }*/

            IEnumerator IEDownLoadVideo()
            {
                string videoPath = System.IO.Path.Combine(Application.streamingAssetsPath, $"{currentAlbumRecord.videoUrl}.mp4");

                string finalUrl = videoPath;

                using (UnityWebRequest req = UnityWebRequest.Get(videoPath))
                {
                    yield return req.SendWebRequest();

                    if (req.result == UnityWebRequest.Result.Success)
                    {
                        
                    }
                    else
                    {
                        finalUrl = currentAlbumRecord.videoUrl;
                    }
                }

                // Load video bytes from StreamingAssets
                using (UnityWebRequest www = UnityWebRequest.Get(finalUrl))
                {
                    yield return www.SendWebRequest();

                    if (www.result == UnityWebRequest.Result.Success)
                    {
                        byte[] videoBytes = www.downloadHandler.data;

                        // Save directly using byte array
                        /*NativeGallery.SaveVideoToGallery(
                            videoBytes,
                            "Tile Aqua Girl",
                            $"Video_{currentPictureId}.mp4",
                            (success, path) =>
                            {
                                if (success)
                                {
                                    // AudioManager.Ins.Play(AudioEnum.SFX_REWAR_APPEAR);
                                    currentPAlbumRecord.isDownloadedVideo = true;
                                    SetStateDownloadSwitches();
                                }
                                else
                                {
                                    _downloadedNoti = LeanLocalizedConst.DOWNLOAD_FAILED;
                                }
                            }
                        );*/
                    }
                    else
                    {
                        // _downloadedNoti = LeanLocalizedConst.DOWNLOAD_FAILED;
                        Debug.LogError($"Failed to load video: {www.error}");
                    }
                }
            }
        }

        private void OnClickImageDownloadVer2()
        {
            DebugLogger.Log();
            // _downloadedNoti = LeanLocalizedConst.DOWNLOADED;

            if (DataManager.Ins.PProgressData.IsVIP)
            {
                OnRewardReceived();

                return;
            }

            // Pause GUIPictureNew's video to free memory before showing fullscreen ad
            UGUIManager.Ins.GUIPictureNew?.PauseVideoBeforeAd();

#if USE_AD_NETWORKS
            AdsManager.Ins.TryShow_RewardAd_MAX(OnRewardReceived, OnVideoNotAvailable, OnRewardHidden,AdWatchReason.DOWNLOAD_PICTURE);
#endif

            void OnRewardReceived()
            {
                StartCoroutine(IEShowLoadingPopup());

                Texture2D texture = currentAlbumRecord.localMainSprite.texture;
                // Save to gallery with callback
                /*NativeGallery.SaveImageToGallery(
                    texture,
                    "Tile Aqua Girl", // Album name
                    $"Picture_{currentPictureId}.png", // Filename
                    (success, path) =>
                    {
                        if (success)
                        {
                            // AudioManager.Ins.Play(AudioEnum.SFX_REWAR_APPEAR);

                            currentPAlbumRecord.isDownloadedImage = true;

                            SetStateDownloadSwitches();
                        }
                        else
                        {
                            _downloadedNoti = LeanLocalizedConst.DOWNLOAD_FAILED;
                        }
                    }
                );*/
            }

            void OnVideoNotAvailable()
            {
                UGUIManager.Ins.GUIPictureNew?.ResumeVideoAfterAd();

                var args = new ToastArgs
                {
#if USE_LEAN_LOCALIZATION
                    Message = LeanLocalization.GetTranslationText(LeanLocalizedConst.CHECK_INTERNET),
#else
                    Message = "Check internet",
#endif
                    CustomAnchoredPos = AnimationConst.UPPER_ANCHORED_POS,
                    TextColor = Color.white,
                    TextFont = UGUIManager.Ins.DefaultFont,
                    customDuration = 0.5f,
                };
                AnimationManager.Ins.Play(args);
            }

            void OnRewardHidden()
            {
                UGUIManager.Ins.GUIPictureNew?.ResumeVideoAfterAd();

#if USE_FIREBASE_ANALYTICS
                AnalyticsAdapter.Log_RewardAd_Watched(DataManager.Ins.PProgressData.LevelId + 1, nameof(AdWatchPlace.GUI_PICTURE_DETAILS));
#endif
            }
        }

        private void OnClickUseAsBackground()
        {
            /*MMEventManager.TriggerEvent(new EBackgroundUpdate_Fire
            {
                albumId = currentPictureId
            });*/

            var args = new ToastArgs
            {
#if USE_LEAN_LOCALIZATION
                Message = LeanLocalization.GetTranslationText(LeanLocalizedConst.BACKGROUND_UPDATED),
#else
                Message = "downloading",
#endif
                CustomAnchoredPos = AnimationConst.UPPER_ANCHORED_POS,
                TextColor = Color.white,
                TextFont = UGUIManager.Ins.DefaultFont,
                customDuration = 0.5f,
            };
            AnimationManager.Ins.Play(args);
        }

        #endregion

        #region Private Methods

        public void ChangePicture(int pictureID)
        {
            DebugLogger.Log();
            /*currentPictureId = pictureID;
            pictureImage.sprite = DataManager.Ins.AlbumData.data[pictureID].localMainSprite;

            pictureImage.FitImageToRectTransformScreenSpaceOverlay(GUIManager.Ins.GetComponent<RectTransform>(), ImageFitMode.COVER);*/
        }

        private bool CheckValidatePictureData(int pictureID)
        {
            DebugLogger.Log(message:$"pictureId: {pictureID}");
            /*currentPAlbumRecord = DataManager.Ins.PAlbumData.GetRecord(pictureID);
            
            if (currentPAlbumRecord == null)
            {
                DebugLogger.Log(message:$"No picture data found for ID {pictureID}");
                return false;
            }
            
            currentAlbumRecord = DataManager.Ins.AlbumData.data[pictureID];

            if (currentAlbumRecord.localMainSprite == null)
            {
                DebugLogger.Log(message:$"No localMainSprite found for ID {pictureID}");
                return false;
            }*/

            return true;
        }

        private void CheckVideoAvailability()
        {
            if (_downloadImageButtonRT == null)
            {
                _downloadImageButtonRT = imageDownloadButton.GetComponent<RectTransform>();
                _downloadImageButtonPosition = _downloadImageButtonRT.anchoredPosition;
            }

            if (!string.IsNullOrEmpty(currentAlbumRecord.videoUrl))
            {
                videoDownloadButton?.gameObject.SetActive(true);
                imageDownloadButton?.gameObject.SetActive(true);

                _downloadImageButtonRT.anchoredPosition = _downloadImageButtonPosition;
            }
            else
            {
                videoDownloadButton?.gameObject.SetActive(false);
                imageDownloadButton?.gameObject.SetActive(true);

                _downloadImageButtonRT.localPosition = _downloadImageButtonRT.localPosition.ChangeX(0);
            }
        }

        private void SetStateDownloadSwitches()
        {
            if (currentPAlbumRecord.isDownloadedImage)
            {
                imageDownloadSwitch.SetState(ButtonSwitch.ButtonSwitchState.OFF);
                imageDownloadButton.interactable = false;
                imageDownloadButton.DisableButton();
                imageCompleteTick.gameObject.SetActive(true);

                ObjScaleAuto scaleAuto = imageDownloadButton.GetComponent<ObjScaleAuto>();
                scaleAuto?.Stop();
            }
            else
            {
                imageDownloadSwitch.SetState(ButtonSwitch.ButtonSwitchState.ON);
                imageDownloadButton.interactable = true;
                imageDownloadButton.EnableButton();
                imageCompleteTick.gameObject.SetActive(false);
            }

            if (currentPAlbumRecord.isDownloadedVideo)
            {
                videoDownloadSwitch.SetState(ButtonSwitch.ButtonSwitchState.OFF);
                videoDownloadButton.interactable = false;
                videoDownloadButton.DisableButton();
                videoCompleteTick.gameObject.SetActive(true);
                ObjScaleAuto scaleAuto = videoDownloadButton.GetComponent<ObjScaleAuto>();
                scaleAuto?.Stop();
            }
            else
            {
                videoDownloadSwitch.SetState(ButtonSwitch.ButtonSwitchState.ON);
                videoDownloadButton.interactable = true;
                videoDownloadButton.EnableButton();
                videoCompleteTick.gameObject.SetActive(false);
            }
        }

        private IEnumerator IEShowLoadingPopup()
        {
            // Show Loading Popup
            UGUIManager.Ins.ShowGUI(UGUIManager.Ins.GUIPopupLoading, 0f, 2f, "DOWNLOADING...");
            UGUIManager.Ins.GUIPopupLoading.OnLoadComplete += HandleLoaded;

            void HandleLoaded()
            {
                UGUIManager.Ins.GUIPopupLoading.OnLoadComplete -= HandleLoaded;
                UGUIManager.Ins.GUIPopupLoading.Hide();
#if USE_AUDIO
                AudioManager.Ins.Play(AudioEnum.SFX_REWAR_APPEAR);
#endif
                
                var args = new ToastArgs
                {
#if USE_LOCALIZATION
                    Message = LeanLocalization.GetTranslationText(_downloadedNoti),
#else
                    Message = "downloading",
#endif
                    CustomAnchoredPos = AnimationConst.UPPER_ANCHORED_POS,
                    TextColor = Color.white,
                    TextFont = UGUIManager.Ins.DefaultFont,
                    customDuration = 0.5f,
                };
                AnimationManager.Ins.Play(args);
            }
            yield return null;
        }

        #endregion

        #region Override Methods

        public override void Show(params object[] parameters)
        {
            base.Show(parameters);
            currentPictureId = parameters.Length > 0 ? (int)parameters[0] : -1;
            currentPictureIdList = parameters.Length > 1 ? (List<int>)parameters[1] : new List<int>();

            currentListId = 0;
            /*if (imageDownloadPriceText != null)
            {
                imageDownloadPriceText.text = $"{DataManager.Ins.AlbumData.downloadImagePrice.GetCoinAmount()}";
            }

            if (videoDownloadPriceText != null)
            {
                videoDownloadPriceText.text = $"{DataManager.Ins.AlbumData.downloadVideoPrice.GetCoinAmount()}";
            }

            for (int i = 0; i < currentPictureIdList.Count; i++)
            {
                if (currentPictureIdList[i] == currentPictureId)
                {
                    currentListId = i;
                    break;
                }
            }

            PAlbumData playerGalleryData = DataManager.Ins.PAlbumData;
            if (!playerGalleryData.IsContain(currentPictureId))
                if (playerGalleryData.TryUnlockAlbum(currentPictureId))
                {
                    currentPAlbumRecord = DataManager.Ins.PAlbumData.GetRecord(currentPictureId);
                }

            
            
            if (CheckValidatePictureData(currentPictureId))
                ChangePicture(currentPictureId);

            SetStateReactButtons();
            SetStateDownloadSwitches();
            CheckVideoAvailability();*/
        }

        public override void Hide(params object[] parameters)
        {
            base.Hide(parameters);
        }

        #endregion

        #region Public Methods

        public void SetStateReactButtons()
        {
            switch (currentPAlbumRecord.likeState)
            {
                case AlbumData.LikeState.LIKE:
                    likeSwitch.SetState(ButtonSwitch.ButtonSwitchState.ON);
                    dislikeSwitch.SetState(ButtonSwitch.ButtonSwitchState.OFF);
                    break;
                case AlbumData.LikeState.DISLIKE:
                    likeSwitch.SetState(ButtonSwitch.ButtonSwitchState.OFF);
                    dislikeSwitch.SetState(ButtonSwitch.ButtonSwitchState.ON);
                    break;
                case AlbumData.LikeState.NONE:
                    likeSwitch.SetState(ButtonSwitch.ButtonSwitchState.OFF);
                    dislikeSwitch.SetState(ButtonSwitch.ButtonSwitchState.OFF);
                    break;
            }
        }

        #endregion

    }

#if UNITY_EDITOR
    [CustomEditor(typeof(GUIPictureDetails))]
    public class GUIPictureDetailsEditor : Editor
    {
        private GUIPictureDetails target;
        public override void OnInspectorGUI()
        {
            // Draw the default inspector
            base.OnInspectorGUI();

            // Get the target component
            target = (GUIPictureDetails)serializedObject.targetObject;

            // Display the current picture ID
            EditorGUILayout.LabelField("Current Picture ID", target.CurrentPictureId.ToString());
        }
    }
#endif
}