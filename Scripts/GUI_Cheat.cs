using System.Collections.Generic;
using MoreMountains.Tools;
using NamPhuThuy.AnimateWithScripts;
using NamPhuThuy.DataManage;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NamPhuThuy.UGUIImplement
{

    public class GUI_Cheat : GUIBase
    {
        [SerializeField] private Button oneLevelUnlockPictureButton;
        [SerializeField] private TMP_InputField timeScaleInput;
        [SerializeField] private Button setTimeScaleButton;
        #region MonoBehaviour Callbacks

        private void Awake()
        {
            // RESOURCES
            getCoinAndBoostersButton.onClick.AddListener(OnClickEarnResources);
            clearAllGold.onClick.AddListener(OnClickClearAllGold);
            clearAllBoosters.onClick.AddListener(OnClickClearAllBoosters);

            unlockAllPicturesButton.onClick.AddListener(OnClickUnlockAllPictures);
            oneLevelUnlockPictureButton.onClick.AddListener(EnableOneLevelUnlockPicture);
            closeButton.onClick.AddListener((() => Hide()));

            // Level-buttons
            triggerWinButton.onClick.AddListener(OnClickTriggerWin);
            replayButton.onClick.AddListener(OnClickReplay);
            nextLevelButton.onClick.AddListener(OnClickNextLevel);
            prevLevelButton.onClick.AddListener(OnClickPrevLevel);
            playLevelButton.onClick.AddListener(OnClickPlayLevelInput);

            // FEATURES
            resetFortuneWheelButton.onClick.AddListener(OnClickResetFortuneWheel);
            openFortuneWheelButton.onClick.AddListener(OnClickOpenFortuneWheel);
            changeTileSetNameButton.onClick.AddListener(OnClickChangeTileSetName);

            //
            setTimeScaleButton.onClick.AddListener(SetTimeScale);
        }

       

        private void OnDestroy()
        {
            
            getCoinAndBoostersButton.onClick.RemoveAllListeners();
            clearAllGold.onClick.RemoveAllListeners();
            clearAllBoosters.onClick.RemoveAllListeners();
            unlockAllPicturesButton.onClick.RemoveAllListeners();
            oneLevelUnlockPictureButton.onClick.RemoveAllListeners();
            closeButton.onClick.RemoveAllListeners();


            triggerWinButton.onClick.RemoveAllListeners();
            replayButton.onClick.RemoveAllListeners();
            nextLevelButton.onClick.RemoveAllListeners();
            prevLevelButton.onClick.RemoveAllListeners();
            playLevelButton.onClick.RemoveAllListeners();

            resetFortuneWheelButton.onClick.RemoveAllListeners();
            openFortuneWheelButton.onClick.RemoveAllListeners();
            changeTileSetNameButton.onClick.RemoveAllListeners();

            setTimeScaleButton.onClick.RemoveAllListeners();
            
        }

        #endregion

        #region RESOURCES

        [Header("Resources")]
        [SerializeField] private Button getCoinAndBoostersButton;
        [SerializeField] private Button clearAllGold;
        [SerializeField] private Button clearAllBoosters;

        [SerializeField] private Button unlockAllPicturesButton;
        [SerializeField] private Button closeButton;
        

        private void OnClickEarnResources()
        {
            List<ResourceAmount> rewards = new List<ResourceAmount>()
            {
                new ResourceAmount(ResourceType.COIN, amount: DataConst.CHEAT_COIN_AMOUNT),
                new ResourceAmount(ResourceType.BOOSTER, boosterType: BoosterType.UNDO, amount: DataConst.CHEAT_BOOSTER_AMOUNT),
                new ResourceAmount(ResourceType.BOOSTER, boosterType: BoosterType.SHUFFLE, amount: DataConst.CHEAT_BOOSTER_AMOUNT),
                new ResourceAmount(ResourceType.BOOSTER, boosterType: BoosterType.MAGIC_PICK, amount: DataConst.CHEAT_BOOSTER_AMOUNT),
                new ResourceAmount(ResourceType.BOOSTER, boosterType: BoosterType.HINT, amount: DataConst.CHEAT_BOOSTER_AMOUNT),
            };

            if (DataManager.Ins.PInventoryData.TryApplyRewards(rewards))
            {
                Debug.Log($"GUICheat: Cheat resources successfully");
            }
        }

        private void OnClickClearAllGold()
        {
            DataManager.Ins.PInventoryData.ClearAllCoins();
        }

        private void OnClickClearAllBoosters()
        {
            DataManager.Ins.PInventoryData.ClearBoosters();
        }

        private void OnClickUnlockAllPictures()
        {
            AnimationManager.Ins.PlayBasicPopupText("Not implement");
            /*foreach (AlbumRecord record in DataManager.Ins.AlbumData.data)
            {
                DataManager.Ins.PAlbumData.TryUnlockAlbum(record.recordId);
            }*/
        }

        private void EnableOneLevelUnlockPicture()
        {
            AnimationManager.Ins.PlayBasicPopupText("Not implement");
        }

        #endregion

        #region LEVEL


        [Header("Levels")]
        [SerializeField] private Button triggerWinButton;
        [SerializeField] private Button replayButton;
        [SerializeField] private Button nextLevelButton;
        [SerializeField] private Button prevLevelButton;
        [SerializeField] private TMP_InputField levelInputField;
        [SerializeField] private Button playLevelButton;

        private void PlayLevel(int level)
        {
            DataManager.Ins.PProgressData.LevelId = level;
            // StartCoroutine(GUIManager.Ins.GUIHUD.IEUpdateUI());

            MMEventManager.TriggerEvent(new ELevelLoad_Fire()
            {
                levelId = DataManager.Ins.PProgressData.LevelId
            });

            Hide();
        }

        private void OnClickTriggerWin()
        {
            MMEventManager.TriggerEvent(new ELevelFinished()
            {
                IsWin = true,
                LevelId = DataManager.Ins.PProgressData.LevelId
            });

            // StartCoroutine(GUIManager.Ins.GUIHUD.IEUpdateUI());

            Hide();
        }
        
        private void OnClickReplay()
        {
            MMEventManager.TriggerEvent(new ELevelLoad_Fire()
            {
                levelId = DataManager.Ins.PProgressData.LevelId
            });
            
            Hide();
        }

        private void OnClickNextLevel()
        {
            PlayLevel(DataManager.Ins.PProgressData.LevelId + 1);
        }

        private void OnClickPrevLevel()
        {
            PlayLevel(DataManager.Ins.PProgressData.LevelId - 1);
        }

        private void OnClickPlayLevelInput()
        {
            string input = levelInputField.text.Trim();

            if (int.TryParse(input, out int level))
            {
                // level = Mathf.Clamp(level, 0, GameConstants.maxLevel - 1);

                PlayLevel(level - 1);
            }
            else
            {
                Debug.LogError("Invalid level number. Please enter a valid integer.");
            }
        }
        #endregion

        #region FEATURES

        [Header("Features")]
        [SerializeField] private Button resetFortuneWheelButton;
        [SerializeField] private Button openFortuneWheelButton;
        [SerializeField] private Button changeTileSetNameButton;
        [SerializeField] private TMP_InputField tileSetNameInputField;

        private void OnClickResetFortuneWheel()
        {
            // DataManager.Ins.PlayerData.LastFreeFortuneSpinTs = 0;

        }

        private void OnClickOpenFortuneWheel()
        {
            // GUIManager.Ins.ShowGUI(GUIManager.Ins.GUIFortuneWheel);
        }

        private void OnClickChangeTileSetName()
        {
            AnimationManager.Ins.PlayBasicPopupText("Not implement");
        }
        #endregion

        private void SetTimeScale()
        {
            Time.timeScale = float.Parse(timeScaleInput.text);

            // GamePersistentVariable.levelDifficultyConfig.reducedTileDiversityRatio = 0.92f;
        }
    }
}
