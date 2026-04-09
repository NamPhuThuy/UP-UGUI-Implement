using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Lean.Localization;
using MoreMountains.Tools;
using NamPhuThuy.AdNetworkAdapter;
using NamPhuThuy.AnimateWithScripts;
using NamPhuThuy.Common;
using UnityEngine.Video;
using UnityEngine.Networking;
using NamPhuThuy.DataManage;
using NamPhuThuy.FirebaseAdapter;
using NamPhuThuy.Lean_Localization;
using DebugLogger = NamPhuThuy.Common.DebugLogger;


namespace NamPhuThuy.UGUIImplement
{

    public class GUIPictureNew : GUIBase
    {
        #region Private Serializable Fields

        [SerializeField] private int currentPictureId;
        [SerializeField] private PAlbumRecord currentPAlbumRecord;
        [SerializeField] private AlbumRecord currentAlbumRecord;

        [Header("Buttons")]
        [SerializeField] private ButtonClicky imageButton;
        [SerializeField] private ButtonClicky adsDownloadButton;
        [SerializeField] private ButtonClicky imageDownloadButton;
        [SerializeField] private ButtonClicky videoDownloadButton;
        [SerializeField] private ButtonClicky confirmButton;
        [SerializeField] private ButtonClicky shopButton;

        [Space(5)]
        [SerializeField] private ButtonClicky likeButton;
        [SerializeField] private ButtonClicky dislikeButton;

        [Header("Buttons Switch")]
        [SerializeField] private ButtonSwitch likeSwitch;
        [SerializeField] private ButtonSwitch dislikeSwitch;
        [SerializeField] private ButtonSwitch imageDownloadSwitch;
        [SerializeField] private ButtonSwitch videoDownloadSwitch;

        [Header("Texts")]
        [SerializeField] private TextMeshProUGUI imageDownloadText;
        [SerializeField] private TextMeshProUGUI videoDownloadText;

        [Header("Images")]
        [SerializeField] private RectTransform pictureContainer;
        [SerializeField] private Image pictureImage;
        [SerializeField] private Image pictureMaskImage;
        
        [SerializeField] private Image imageToCalulateSizeForVideo;
        [SerializeField] private RectTransform videoFrame;
     
        [SerializeField] private Image imageCompleteTick;
        [SerializeField] private Image videoCompleteTick;

        [Header("Video")]
        [SerializeField] private RectTransform videoContainer;
        [SerializeField] private CanvasGroup videoDummyCG; // placeHolder for the video
        
        [SerializeField] private VideoPlayer videoPlayer;
        [SerializeField] private RectTransform videoDisplay;
        
        [Header("Remote Assets Loading")]
        [SerializeField] private CanvasGroup remoteAssetsLoadingContainer;
        [SerializeField] private LeanLocalizedTextMeshProUGUI notiLoadRemoteText;

        private RectTransform _imageToCalulateSizeForVideoRT;
        private RectTransform _downloadImageButtonRT;
        private bool _isVideoPrepareCompleted;
        private Vector3 _downloadImageButtonPosition;

        public event Action ChangeLikeState;

        #endregion

        #region Private Fields

        private string _downloadedNoti = LeanLocalizedConst.IMAGE_DOWNLOADED;
        private PrimeTween.Tween _showPreviewImageForVideoTween;

        #endregion

        #region MonoBehaviour Callbacks

        private void Start()
        {
            likeSwitch = likeButton.GetComponent<ButtonSwitch>();
            dislikeSwitch = dislikeButton.GetComponent<ButtonSwitch>();

            _imageToCalulateSizeForVideoRT = imageToCalulateSizeForVideo.GetComponent<RectTransform>();
        }

        private void Awake()
        {
            imageButton.onClick.AddListener(OnClickImage);
            likeButton.onClick.AddListener(OnClickLike);
            dislikeButton.onClick.AddListener(OnClickDisLike);
            shopButton?.onClick.AddListener(OnClickShop);

            videoDownloadButton.onClick.AddListener(OnClickVideoDownload);
            imageDownloadButton.onClick.AddListener(OnClickImageDownloadVer2);


            confirmButton.onClick.AddListener(OnClickConfirm);
        }

        private void OnDestroy()
        {
            imageButton.onClick.RemoveListener(OnClickImage);
            likeButton.onClick.RemoveListener(OnClickLike);
            dislikeButton.onClick.RemoveListener(OnClickDisLike);
            shopButton.onClick.RemoveListener(OnClickShop);

            videoDownloadButton.onClick.RemoveAllListeners();
            imageDownloadButton.onClick.RemoveAllListeners();

            confirmButton.onClick.RemoveListener(OnClickConfirm);
        }

        #endregion

        #region Override Methods

        public override void Show(params object[] parameters)
        {
            base.Show(parameters);
            DebugLogger.Log(message:$"Album id: {(int)parameters[0]}");
            currentPictureId = (int)parameters[0];
            /*currentAlbumRecord = DataManager.Ins.AlbumData.data[currentPictureId];

            videoDownloadText.text = $"{DataManager.Ins.AlbumData.downloadVideoPrice.GetCoinAmount()}";
            imageDownloadText.text = $"{DataManager.Ins.AlbumData.downloadImagePrice.GetCoinAmount()}";*/

#if USE_AUDIO
            AudioManager.Ins.Play(AudioEnum.SFX_REWAR_APPEAR);
#endif

            imageButton.isUseClickyFX = false; // Turn off the ClickyFX 

            try
            {
                StartCoroutine(WaitForRemoteAssetsDownloaded());
            }
            catch (System.Exception e)
            {

            }

            /*PAlbumData playerGalleryData = DataManager.Ins.PAlbumData;
            if (!playerGalleryData.IsContain(currentPictureId))
                if (playerGalleryData.TryUnlockAlbum(currentPictureId))
                {
                    currentPAlbumRecord = DataManager.Ins.PAlbumData.GetRecord(currentPictureId);
                }*/

            SetStateForButton();
            SetStateDownloadSwitches();

            AnimateNewPicture();
        }

        public override void Hide(params object[] parameters)
        {
            base.Hide(parameters);
        }

        #endregion

        #region Button Events

        private void OnClickImage()
        {
            /*List<int> pictureIdList = new List<int>();
            foreach (PAlbumRecord data in DataManager.Ins.PAlbumData.playerAlbums)
            {
                pictureIdList.Add(data.albumId);
            }

            GUIManager.Ins.ShowGUI(GUIManager.Ins.GUIPictureDetails, 0f, currentPictureId, pictureIdList);*/
        }

        private void OnClickLike()
        {
            bool tmp = currentPAlbumRecord.likeState == AlbumData.LikeState.LIKE;

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

        private void OnClickShop()
        {
            UGUIManager.Ins.ShowGUI(UGUIManager.Ins.GUIShop);
        }

        /// <summary>
        /// Download image version 2 (reward ads)
        /// </summary>
        private void OnClickImageDownloadVer2()
        {
            DebugLogger.Log();
            _downloadedNoti = LeanLocalizedConst.DOWNLOADED;

            if (DataManager.Ins.PProgressData.IsVIP)
            {
                OnRewardReceived();

                return;
            }

            // Pause video to free memory before showing fullscreen ad
            PauseVideoBeforeAd();

            AdsManager.Ins.TryShow_RewardAd_MAX(OnRewardReceived, OnVideoNotAvailable, OnRewardHidden,AdWatchReason.DOWNLOAD_PICTURE);

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
                ResumeVideoAfterAd();

                var args = new ToastArgs
                {
                    Message = LeanLocalization.GetTranslationText(LeanLocalizedConst.CHECK_INTERNET),
                    CustomAnchoredPos = AnimationConst.UPPER_ANCHORED_POS,
                    TextColor = Color.white,
                    TextFont = UGUIManager.Ins.DefaultFont,
                    customDuration = 0.5f,
                };
                AnimationManager.Ins.Play(args);
            }

            void OnRewardHidden()
            {
                ResumeVideoAfterAd();

                int level = DataManager.Ins.PProgressData.LevelId;
                AnalyticsAdapter.Log_RewardAd_Watched(DataManager.Ins.PProgressData.LevelId + 1, nameof(AdWatchPlace.GUI_PICTURE_NEW));
            }
        }

        private void OnClickVideoDownload()
        {
            DebugLogger.Log();
            _downloadedNoti = LeanLocalizedConst.DOWNLOADED;

            if (DataManager.Ins.PProgressData.IsVIP)
            {
                StartCoroutine(IEShowLoadingPopup());
                StartCoroutine(IEDownLoadVideo());

                return;
            }

            int downloadPrice = 100;
            // downloadPrice = DataManager.Ins.AlbumData.downloadVideoPrice.GetCoinAmount();

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
                UGUIManager.Ins.ShowGUI(UGUIManager.Ins.GUINotEnoughCoin);
            }

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
                        _downloadedNoti = LeanLocalizedConst.DOWNLOAD_FAILED;
                        Debug.LogError($"Failed to load video: {www.error}");
                    }
                }
            }
        }

        private void OnClickConfirm()
        {
            ChangeBackground();
            Hide();

            void ChangeBackground()
            {
                /*
                if (DataManager.Ins.AlbumData.data[currentPictureId].IsLocalAvailable())
                {
                    MMEventManager.TriggerEvent(new EBackgroundUpdate_Fire
                    {
                        albumId = currentPictureId
                    });

                    var args = new ToastArgs
                    {
                        Message = LeanLocalization.GetTranslationText(LeanLocalizedConst.BACKGROUND_UPDATED),
                        CustomAnchoredPos = AnimationConst.UPPER_ANCHORED_POS,
                        TextColor = Color.white,
                        TextFont = GUIManager.Ins.DefaultFont,
                        customDuration = 0.5f,
                    };
                    AnimationManager.Ins.Play(args);
                }
                */

            }
        }

        private void AnimateNewPicture()
        {
            Transform pictureImageTransform = pictureMaskImage.rectTransform;

            Vector2 initialPosition = pictureMaskImage.rectTransform.localPosition;
            float initScale = pictureMaskImage.rectTransform.localScale.x;

            pictureMaskImage.rectTransform.localPosition += new Vector3(240, -280);
            pictureImageTransform.localScale = Vector2.zero;
            pictureImageTransform.eulerAngles = new Vector3(0, 0, 180);

            pictureMaskImage.rectTransform.DOLocalMove(1.2f * initialPosition, duration: 0.3f).SetEase(Ease.InOutSine).SetDelay(0.3f)
            .OnComplete(() =>
            {
                pictureMaskImage.rectTransform.DOLocalMove(initialPosition, duration: 0.1f).SetEase(Ease.InOutSine);
            });
            pictureImageTransform.DOScale(1.1f * initScale, duration: 0.3f).SetEase(Ease.InOutSine).SetDelay(0.3f)
            .OnComplete(() =>
            {
                pictureImageTransform.DOScale(1f * initScale, duration: 0.1f).SetEase(Ease.InOutSine);
            });
            pictureImageTransform.DORotate(Vector3.zero, duration: 0.3f).SetEase(Ease.InOutSine).SetDelay(0.3f);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Stop VideoPlayer to free memory before showing a fullscreen ad
        /// </summary>
        public void PauseVideoBeforeAd()
        {
            return;
            if (videoPlayer != null && videoPlayer.isPlaying)
            {
                videoPlayer.Stop();
                videoPlayer.targetTexture?.Release();
            }
        }

        /// <summary>
        /// Re-prepare and play video after ad is closed
        /// </summary>
        public void ResumeVideoAfterAd()
        {
            return;
            if (videoPlayer != null && !string.IsNullOrEmpty(videoPlayer.url))
            {
                videoPlayer.Prepare();
                videoPlayer.prepareCompleted += OnResumeVideoPrepared;
            }
        }

        private void OnResumeVideoPrepared(VideoPlayer vp)
        {
            vp.prepareCompleted -= OnResumeVideoPrepared;
            vp.Play();
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
                    Message = LeanLocalization.GetTranslationText(_downloadedNoti),
                    CustomAnchoredPos = AnimationConst.UPPER_ANCHORED_POS,
                    TextColor = Color.white,
                    TextFont = UGUIManager.Ins.DefaultFont,
                    customDuration = 0.5f,
                };
                AnimationManager.Ins.Play(args);
            }
            yield return null;
        }

        private void ShowLoadingPopup()
        {
             // Show Loading Popup
             UGUIManager.Ins.ShowGUI(UGUIManager.Ins.GUIPopupLoading, 0f, 2f, "DOWNLOADING...");
             UGUIManager.Ins.GUIPopupLoading.OnLoadComplete += HandleLoaded;

             void HandleLoaded()
             {
                 UGUIManager.Ins.GUIPopupLoading.OnLoadComplete -= HandleLoaded;
                 UGUIManager.Ins.GUIPopupLoading.Hide();
         
                 var args = new ToastArgs
                 {
                     Message = LeanLocalization.GetTranslationText(_downloadedNoti),
                     CustomAnchoredPos = AnimationConst.UPPER_ANCHORED_POS,
                     TextColor = Color.white,
                     TextFont = UGUIManager.Ins.DefaultFont,
                     customDuration = 0.5f,
                 };
                 AnimationManager.Ins.Play(args);
             }
        }

        private void ShowPermissionNotAllowedNotification()
        {
            _downloadedNoti = LeanLocalizedConst.PERMISSION_DENIED;

            var args = new ToastArgs
            {
                Message = LeanLocalization.GetTranslationText(_downloadedNoti),
                CustomAnchoredPos = AnimationConst.UPPER_ANCHORED_POS,
                TextColor = Color.white,
                TextFont = UGUIManager.Ins.DefaultFont,
                customDuration = 0.5f,
            };
            AnimationManager.Ins.Play(args);
            
        }

        IEnumerator LoadVideoFromStreamingAssets(string videoUrl)
        {
            string filePath = System.IO.Path.Combine(Application.streamingAssetsPath, $"{videoUrl}.mp4");

            string finalUrl = $"file://{filePath}";

            using (UnityWebRequest req = UnityWebRequest.Get(filePath))
            {
                yield return req.SendWebRequest();

                if (req.result == UnityWebRequest.Result.Success)
                {

                }
                else
                {
                    // finalUrl = videoUrl;
                }
            }

            videoPlayer.url = finalUrl;
            videoPlayer.isLooping = true;
            videoPlayer.Prepare();

            videoPlayer.prepareCompleted += OnVideoPreparedCompleted;

            _isVideoPrepareCompleted = false;

            // fallback when video can not complete prepared: show temp preview image
            DOVirtual.DelayedCall(3, () =>
            {
                if (!_isVideoPrepareCompleted)
                {
                    ShowPreviewImage();
                }
            });

            void ShowPreviewImage()
            {
                imageToCalulateSizeForVideo.gameObject.SetActive(true);

                imageToCalulateSizeForVideo.color = Common.ColorHelper.WithAlpha(0);

                _showPreviewImageForVideoTween = PrimeTween.Tween.Alpha(imageToCalulateSizeForVideo, 1, 0.3f);
            }
        }

        private void OnVideoPreparedCompleted(VideoPlayer preparedVideoPlayer)
        {
            if (_isVideoPrepareCompleted)
            {
                return;
            }

            _isVideoPrepareCompleted = true;

            if (imageToCalulateSizeForVideo.gameObject.activeSelf)
            {
                _showPreviewImageForVideoTween.Complete();

                _showPreviewImageForVideoTween = PrimeTween.Tween.Alpha(imageToCalulateSizeForVideo, 0, duration: 0.3f)
                    .OnComplete(() =>
                {
                    imageToCalulateSizeForVideo.gameObject.SetActive(false);
                });

                videoDummyCG.gameObject.SetActive(false);
            }
            else
            {
                HidePlaceholder();
            }

            videoPlayer.Play();
            videoPlayer.prepareCompleted -= OnVideoPreparedCompleted;

            void HidePlaceholder()
            {
                PrimeTween.Tween.Alpha(videoDummyCG, 0f, 0.3f)
                .OnComplete(() =>
                {
                    videoDummyCG.gameObject.SetActive(false);
                });
            }
        }
        #endregion

        #region Public Methods

        public void SetStateForButton()
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

        #region Wait for remote assets downloaded
        private IEnumerator WaitForRemoteAssetsDownloaded()
        {
            /*float interval = 0.2f;

            if (!currentAlbumRecord.IsLocalAvailable())
            {
                remoteAssetsLoadingContainer.gameObject.SetActive(true);
                pictureContainer.gameObject.SetActive(false);
                videoContainer.gameObject.SetActive(false);

                remoteAssetsLoadingContainer.alpha = 1;
            }
            else
            {
                remoteAssetsLoadingContainer.gameObject.SetActive(false);

                ShowNewVideoOrImage();

                yield break;
            }

            if (currentPictureId < 0 || currentPictureId >= DataManager.Ins.AlbumData.data.Count)
            {
                yield break;
            }

            notiLoadRemoteText.TranslationName = Application.internetReachability == NetworkReachability.NotReachable ?
                LeanLocalizedConst.NO_INTERNET : LeanLocalizedConst.VIDEO_DOWNLOADING;

            const float maxCooldownCheckInternet = 5;
            const float maxCooldownLoadAssets = 2;

            float cooldownCheckInternet = maxCooldownCheckInternet;

            while (!currentAlbumRecord.IsLocalAvailable())
            {
                currentAlbumRecord = DataManager.Ins.AlbumData.data[currentPictureId];

                if (cooldownCheckInternet <= 0)
                {
                    notiLoadRemoteText.TranslationName = Application.internetReachability == NetworkReachability.NotReachable ?
                        LeanLocalizedConst.NO_INTERNET : LeanLocalizedConst.VIDEO_DOWNLOADING;

                    if (Application.internetReachability == NetworkReachability.NotReachable)
                    {
                        var args = new ToastArgs
                        {
                            Message = LeanLocalization.GetTranslationText(LeanLocalizedConst.NO_INTERNET),
                            CustomAnchoredPos = AnimationConst.UPPER_ANCHORED_POS,
                            TextColor = Color.white,
                            TextFont = GUIManager.Ins.defaultFont,
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

                _ = DataManager.Ins.FetchPictureData(currentAlbumRecord.recordId);

                yield return YieldHelper.GetRealtime(interval);
            }

            yield return YieldHelper.GetRealtime(0.8f);

            PrimeTween.Tween.Alpha(remoteAssetsLoadingContainer, 0f, 0.3f).OnComplete(() =>
            {
                remoteAssetsLoadingContainer.gameObject.SetActive(false);
            });

            ShowNewVideoOrImage();*/
            yield return null;
        }

        private void ShowNewVideoOrImage()
        {
            /*if (_imageToCalulateSizeForVideoRT == null)
            {
                _imageToCalulateSizeForVideoRT = imageToCalulateSizeForVideo.GetComponent<RectTransform>();
            }

            if (_downloadImageButtonRT == null)
            {
                _downloadImageButtonRT = imageDownloadButton.GetComponent<RectTransform>();
                _downloadImageButtonPosition = _downloadImageButtonRT.anchoredPosition;
            }

            currentAlbumRecord = DataManager.Ins.AlbumData.data[currentPictureId];

            if (!string.IsNullOrEmpty(currentAlbumRecord.videoUrl)/* && GamePersistentVariable.isUseVideoBackground#1#)
            {
                videoDownloadButton?.gameObject.SetActive(true);
                imageDownloadButton?.gameObject.SetActive(true);

                pictureContainer.gameObject.SetActive(false);
                videoContainer.gameObject.SetActive(true);

                _downloadImageButtonRT.anchoredPosition = _downloadImageButtonPosition;

                videoFrame.sizeDelta = new Vector2(0.9f * UGUIConst.CANVAS_SIZE_MOBILE.x, 0.55f * UGUIConst.CANVAS_SIZE_MOBILE.y);

                imageToCalulateSizeForVideo.sprite = DataManager.Ins.AlbumData.data[currentPictureId].localMainSprite;
                imageToCalulateSizeForVideo.FitImageToRectTransformScreenSpaceOverlay(videoFrame);

                videoDisplay.sizeDelta = _imageToCalulateSizeForVideoRT.sizeDelta;
                videoDisplay.localPosition = _imageToCalulateSizeForVideoRT.localPosition;

                StartCoroutine(LoadVideoFromStreamingAssets(currentAlbumRecord.videoUrl));

                videoDummyCG.gameObject.SetActive(true);
                videoDummyCG.alpha = 1;
                videoDummyCG.GetComponent<RectTransform>().sizeDelta = _imageToCalulateSizeForVideoRT.sizeDelta;
            }
            else
            {
                videoDownloadButton?.gameObject.SetActive(false);
                imageDownloadButton?.gameObject.SetActive(true);

                pictureContainer.gameObject.SetActive(true);
                videoContainer.gameObject.SetActive(false);
                videoDummyCG.gameObject.SetActive(false);

                _downloadImageButtonRT.localPosition = _downloadImageButtonRT.localPosition.ChangeX(0);

                pictureImage.sprite = DataManager.Ins.AlbumData.data[currentPictureId].localMainSprite;
                pictureImage.FitImageToRectTransformScreenSpaceOverlay(pictureMaskImage.rectTransform);
            }*/
        }
        #endregion

        #region Refresh Video
        private void OnVideoErrorReceived(VideoPlayer source, string message)
        {
            RefreshVideo();

            videoPlayer.errorReceived -= OnVideoErrorReceived;
        }

        void RefreshVideo()
        {
            imageToCalulateSizeForVideo.gameObject.SetActive(true);
            imageToCalulateSizeForVideo.color = Common.ColorHelper.WithAlpha(0);

            PrimeTween.Tween.Alpha(imageToCalulateSizeForVideo, 1, duration: 0.3f)
            .OnComplete(() =>
            {
                videoPlayer.Stop();
                videoPlayer.Prepare();
                videoPlayer.prepareCompleted += OnPrepared;
            });
        }

        private void OnPrepared(VideoPlayer vp)
        {
            vp.prepareCompleted -= OnPrepared;
            vp.Play();

            PrimeTween.Tween.Alpha(imageToCalulateSizeForVideo, 0, duration: 0.3f)
            .OnComplete(() =>
            {
                imageToCalulateSizeForVideo.gameObject.SetActive(false);
            });
        }
        #endregion
    }
}
