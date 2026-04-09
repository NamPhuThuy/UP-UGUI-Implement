using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Lean.Localization;
using MoreMountains.Tools;
using NamPhuThuy.AnimateWithScripts;
using NamPhuThuy.Common;
using NamPhuThuy.DataManage;
using NamPhuThuy.FirebaseAdapter;
using NamPhuThuy.Lean_Localization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NamPhuThuy.UGUIImplement
{
    public class Element_GalleryPack : MonoBehaviour, MMEventListener<ERemoteGalleryDataUpdated>
    {
        #region Private Serializable Fields

        [Header("Stats")]
        public int currentPackId;
        public int packPrice;
        public int packQuantity;

        public int PackQuantity
        {
            get { return packQuantity; }
            set
            {
                packQuantity = value;
                UpdateUI();
            }
        }

        public int TotalPrice => packPrice * packQuantity;


        public PictureStyle currentPackStyle;

        [Header("Texts")]
        public TextMeshProUGUI priceText;
        public TextMeshProUGUI titleText;
        public TextMeshProUGUI packAmountText;
        public TextMeshProUGUI amountDescriptionText;

        [Header("Buttons")]
        public Button plusButton;
        public Button minusButton;
        public ButtonClicky buyButton;
        public ButtonSwitch buyButtonSwitch;

        [Header("Images")]
        public Image backGroundImage;
        public Image titleImage;
        public GameObject descriptionImage;

        [Header("Remote Content")]
        [SerializeField] private CanvasGroup remoteButtonGroup;

        #endregion

        #region Private Fields

        #endregion

        #region MonoBehaviour Callbacks

        private void Awake()
        {
            plusButton.onClick.AddListener(OnClickPlus);
            minusButton.onClick.AddListener(OnClickMinus);

            buyButton.onClick.AddListener(OnClickBuy);
            MMEventManager.RegistCurrentEvents(this);
        }

        private void OnDestroy()
        {
            plusButton.onClick.RemoveAllListeners();
            minusButton.onClick.RemoveAllListeners();
            buyButton.onClick.RemoveAllListeners();

            MMEventManager.UnregistCurrentEvents(this);
        }

        void OnEnable()
        {
            PackQuantity = 1;
            StartCoroutine(IEUpdateUI());

            // CheckAvailablePictures();
            CheckUnlockedAllState();
           
        }


        #endregion

        #region Private Methods

        private void UpdateUI()
        {
            priceText.text = $"{packPrice * packQuantity}";
            packAmountText.text = $"{packQuantity}";
        }

        private IEnumerator IEUpdateUI()
        {
            yield return YieldHelper.WaitForSeconds(0.15f);
            UpdateUI();
        }

        private List<AlbumRecord> TakeAListOfLockPictures()
        {
            List<AlbumRecord> result = new List<AlbumRecord>();

            /*foreach (var p in DataManager.Ins.AlbumData.data)
            {
                if (p.style != currentPackStyle) continue;
                if (DataManager.Ins.PAlbumData.IsContain(p.recordId)) continue;

                result.Add(p);
            }*/

            return result;
        }

        private List<AlbumRecord> TakeAListOfLockLocalPictures()
        {
            List<AlbumRecord> result = new List<AlbumRecord>();

            /*foreach (var p in DataManager.Ins.AlbumData.data)
            {
                if (p.style != currentPackStyle) continue;
                if (!p.IsLocalAvailable()) continue;
                if (DataManager.Ins.PAlbumData.IsContain(p.recordId)) continue;

                result.Add(p);
            }*/

            return result;
        }

        private void UnlockPictures(List<AlbumRecord> pics)
        {
            /*foreach (var p in pics)
            {
                if (DataManager.Ins.PAlbumData.IsContain(p.recordId)) continue;
                DataManager.Ins.PAlbumData.TryUnlockAlbum(p.recordId);
            }*/
        }

        public void CheckUnlockedAllState()
        {
            if (buyButtonSwitch != null)
            {
                List<AlbumRecord> lockedPictures = TakeAListOfLockPictures();

                if (lockedPictures.Count > 0)
                {
                    buyButtonSwitch.SetState(ButtonSwitch.ButtonSwitchState.ON);
                    buyButton.interactable = true;

                    SetUnlockedAllStatePictureOfStyle(false);
                }
                else
                {
                    buyButtonSwitch.SetState(ButtonSwitch.ButtonSwitchState.OFF);
                    buyButton.interactable = false;

                    SetUnlockedAllStatePictureOfStyle(true);
                }
            }
        }

        private void SetUnlockedAllStatePictureOfStyle(bool isUnlockedAll)
        {
            /*switch (currentPackStyle)
            {
                case PictureStyle.ANIME:
                    DataManager.Ins.PAlbumData.isUnlockedAllAnimePics = isUnlockedAll;
                    break;

                case PictureStyle.SEXY:
                    DataManager.Ins.PAlbumData.isUnlockedAllSexyPics = isUnlockedAll;
                    break;

                case PictureStyle.UNIFORM:
                    DataManager.Ins.PAlbumData.isUnlockedAllUniformPics = isUnlockedAll;
                    break;
            }*/
        }

        private void SetUnlockedAllPictureOfStyle()
        {
            /*switch (currentPackStyle)
            {
                case PictureStyle.ANIME:
                    DataManager.Ins.PAlbumData.isUnlockedAllAnimePics = true;
                    break;

                case PictureStyle.SEXY:
                    DataManager.Ins.PAlbumData.isUnlockedAllSexyPics = true;
                    break;

                case PictureStyle.UNIFORM:
                    DataManager.Ins.PAlbumData.isUnlockedAllUniformPics = true;
                    break;
            }*/
        }

        private void ResetValues()
        {
            PackQuantity = 1;
        }
        #endregion

        #region Public Methods
        #endregion
        
        #region Button Events

        private void OnClickPlus()
        {
            /*DataManager.Ins.PAlbumData.DictStyleCount.TryGetValue(currentPackStyle, out var value);
            int ownedAmount = value;

            int totalAmount = DataManager.Ins.AlbumData.AlbumAmountOnStyle(currentPackStyle);

            if ((PackQuantity + 1) + ownedAmount >
                totalAmount)
            {
                var args = new ToastArgs
                {
                    Message = LeanLocalization.GetTranslationText(LeanLocalizedConst.OUT_OF_PICTURES),
                    CustomAnchoredPos = AnimationConst.UPPER_ANCHORED_POS,
                    TextColor = Color.white,
                    TextFont = GUIManager.Ins.DefaultFont,
                    customDuration = 0.5f,
                };
                AnimationManager.Ins.Play(args);
                return;
            }

            if (PackQuantity < 20)
                PackQuantity += 1;
            else
            {
                var args = new ToastArgs
                {
                    Message = LeanLocalization.GetTranslationText(LeanLocalizedConst.CANT_INCREASE),
                    CustomAnchoredPos = AnimationConst.UPPER_ANCHORED_POS,
                    TextColor = Color.white,
                    TextFont = GUIManager.Ins.DefaultFont,
                    customDuration = 0.5f,
                };
                AnimationManager.Ins.Play(args);
            }*/
        }

        private void OnClickMinus()
        {
            if (PackQuantity > 1)
                PackQuantity -= 1;
            else
            {
                var args = new ToastArgs
                {
                    Message = LeanLocalization.GetTranslationText(LeanLocalizedConst.CANT_DECREASE),
                    CustomAnchoredPos = AnimationConst.UPPER_ANCHORED_POS,
                    TextColor = Color.white,
                    TextFont = UGUIManager.Ins.DefaultFont,
                    customDuration = 0.5f,
                };
                AnimationManager.Ins.Play(args);
            }
        }

        private void OnClickBuy()
        {
            bool isEnoughCoin = DataManager.Ins.PInventoryData.Coin >= TotalPrice;

            AnalyticsAdapter.Log_Picture_TryBuy(DataManager.Ins.PProgressData.LevelId + 1, isEnoughCoin,
                currentPackStyle.ToString());
            
            if (!isEnoughCoin)
            {
                UGUIManager.Ins.ShowGUI(UGUIManager.Ins.GUINotEnoughCoin);
                return;
            }

            List<AlbumRecord> lockedPictures = TakeAListOfLockPictures();
            List<AlbumRecord> lockedLocalPictures = TakeAListOfLockLocalPictures();
            int picNumTake = Mathf.Min(PackQuantity, lockedPictures.Count);

            lockedPictures.FisherYatesShuffle();

            // prioritize local pictures instead of remote ones
            for (int i = 0; i < lockedPictures.Count; i++)
            {
                if (i >= lockedLocalPictures.Count)
                {
                    break;
                }
                else
                {
                    bool isPictureDuplicate = lockedPictures.Any(item => item.recordId == lockedLocalPictures[i].recordId);

                    if (!isPictureDuplicate)
                    {
                        lockedPictures[i] = lockedLocalPictures[i];
                    }
                }
            }

            List<AlbumRecord> picked = lockedPictures.GetRange(0, picNumTake);

            if (picNumTake >= lockedPictures.Count)
            {
                buyButtonSwitch.SetState(ButtonSwitch.ButtonSwitchState.OFF);
                buyButton.interactable = false;
                SetUnlockedAllPictureOfStyle();
            }

            // GamePersistentVariable.spendCoinReason = "PictureGacha";

            if (DataManager.Ins.PInventoryData.TrySpendCoins(TotalPrice))
            {
                // Buy successfully
                #if USE_AUDIO
                AudioManager.Ins.Play(AudioEnum.SFX_COIN_1);
                #endif
               
                
                ShowLoadingPopup();
                UnlockPictures(picked);
                PackQuantity = 1;
            }
            else
            {
                UGUIManager.Ins.ShowGUI(UGUIManager.Ins.GUINotEnoughCoin);
            }

            void ShowLoadingPopup()
            {
                UGUIManager.Ins.ShowGUI(UGUIManager.Ins.GUIPopupLoading);
                UGUIManager.Ins.GUIPopupLoading.OnLoadComplete += HandleLoaded;
            }

            void HandleLoaded()
            {
                UGUIManager.Ins.GUIPopupLoading.OnLoadComplete -= HandleLoaded;
                UGUIManager.Ins.ShowGUI(UGUIManager.Ins.GUIGalleryPackResults, 0f, picked);

                UGUIManager.Ins.GUIPopupLoading.Hide();
            }
        }

        public void OnMMEvent(ERemoteGalleryDataUpdated eventArgs)
        {
            CheckUnlockedAllState();
        }

        #endregion
    }
}