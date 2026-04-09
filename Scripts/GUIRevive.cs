using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using DG.Tweening;
using NamPhuThuy.DataManage;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace NamPhuThuy.UGUIImplement
{

    public class GUIRevive : GUIBase
    {
        [Header("Stats")]
        [SerializeField] private float restartTime = 5f;
        private float timer = 0f;

        [Header("Flags")]
        [SerializeField] private bool isTriggerRestart = false;
        [SerializeField] private bool isTimeOut = false;

        [Header("Buttons")]
        [SerializeField] private Button reviveButton;
        [SerializeField] private Button noThanksButton;

        [Header("Images")]
        [SerializeField] private Image upperTimerImage;

        [Header("Texts")]
        [SerializeField] private TextMeshProUGUI countdownText;
        [SerializeField] private TextMeshProUGUI messageText;

        [Header("Free Revive")]
        [SerializeField] private RectTransform normalReviveContainer;
        [SerializeField] private RectTransform freeReviveContainer;
        [SerializeField] private TMP_Text numFreeReviveText;

        private string[] messageList = new string[]
        {
            "Almost there",
            "You can do it, try again",
            "Not much further to good",
            "Keep playing",
            "You made it this far, don't give up",
            "You are doing great"
        };

        #region Private Serializable Fields

        #endregion

        #region Private Fields
        private int _prevTime;
        #endregion

        #region MonoBehaviour Callbacks

        private void OnEnable()
        {
            // GamePlayManager.Ins.DisableInteract();
            // AudioManager.Ins.Play(AudioEnum.SFX_LEVEL_LOSE_02);
            reviveButton.onClick.AddListener(OnClickRevive);
            noThanksButton.onClick.AddListener(RefuseRevive);
            timer = restartTime;
            _prevTime = (int)timer;

            int rand = Random.Range(0, messageList.Length);
            // messageText.GetComponent<LeanLocalizedTextMeshProUGUI>().TranslationName = messageList[rand];

            SetupFreeRevive();
        }

        private void OnDisable()
        {
            // GamePlayManager.Ins.EnableInteract();
            reviveButton.onClick.RemoveListener(OnClickRevive);
            noThanksButton.onClick.RemoveAllListeners();
        }

        private void Update()
        {
            if (isTriggerRestart) return;
            if (isTimeOut) return;
            // if (GamePersistentVariable.isPauseCountingRevive) return;
            timer -= Time.deltaTime;

            if ((int)timer != _prevTime)
            {
                countdownText.transform.DOPunchScale(0.2f * Vector3.one, 0.3f);
            }

            _prevTime = (int)timer;

            countdownText.text = $"{(int)timer}";
            upperTimerImage.fillAmount = timer / restartTime;
            if (timer <= 0f)
            {
                ShowNoThanksButton();

                isTimeOut = true;
            }
        }

        #endregion

        #region Public Methods
        #endregion

        #region Private Methods

        private void SetupFreeRevive()
        {
            if (IsFreeRevive())
            {
                normalReviveContainer.gameObject.SetActive(false);
                freeReviveContainer.gameObject.SetActive(true);

                // int currentFreeRevive = DataUtility.Load("current_level_num_free_revive", GamePersistentVariable.reviveConfig.numFreeLives);

                // numFreeReviveText.text = $"{currentFreeRevive}/{GamePersistentVariable.reviveConfig.numFreeLives}";
            }
            else
            {
                normalReviveContainer.gameObject.SetActive(true);
                freeReviveContainer.gameObject.SetActive(false);
            }

            noThanksButton.gameObject.SetActive(false);
        }

        private bool IsFreeRevive()
        {
            try
            {
                // int currentFreeRevive = DataUtility.Load("current_level_num_free_revive", GamePersistentVariable.reviveConfig.numFreeLives);

                return false;
                /*GamePersistentVariable.reviveConfig.isEnable &&
                DataManager.Ins.PlayerData.CurrentLevelId < GamePersistentVariable.reviveConfig.maxLevel &&
                currentFreeRevive > 0;*/
            }
            catch (System.Exception)
            {
                return true;
            }
        }

        private void ShowNoThanksButton()
        {
            noThanksButton.interactable = false;

            TweenHelper.PopupScaleSquence(noThanksButton.transform, onComplete: () =>
            {
                noThanksButton.interactable = true;
            });
        }

        private void RefuseRevive()
        {
            Hide();
            
            MMEventManager.TriggerEvent(new ELevelRestart_Fire());
        }

        #endregion

        #region Editor Methods

        public void ResetValues()
        {
            restartTime = 5f;
        }

        #endregion

        #region Override Methods

        public override void Show(params object[] parameters)
        {
            base.Show(parameters);
            isTriggerRestart = false;
            isTimeOut = false;
        }


        public override void Hide(params object[] parameters)
        {
            base.Hide(parameters);
        }

        #endregion

        #region Button Events

        private void OnClickRevive()
        {
            MMEventManager.TriggerEvent(new ELevelRevive_Fire()
            {
                LevelId = DataManager.Ins.PProgressData.LevelId
            });
            
            // WATCH ADS and revive player 
            // IF watched ads successfully 
            // Active undo-booster till there is only 1 tile left on the waiting line 
            // Active shuffle-booster

            /*if (IsFreeRevive())
            {
                RewardRevive();

                return;
            }

            AdsManager.Ins.ShowVideoReward(RewardRevive, OnLoadVideoFailed, OnAdsClosed, ActionWatchVideo.REVIVE,
                PlaceWatchVideo.GUI_REVIVE);

            void RewardRevive()
            {
                MMEventManager.TriggerEvent(new ELevelRevive_Fire());
                Hide();
            }

            void OnLoadVideoFailed()
            {
                // SHow VFX - no ads available right now
                AdsManager.Ins.ShowAdNotLoadedNotification();

                GamePersistentVariable.isPauseCountingRevive = true;
            }

            void OnAdsClosed()
            {
                int level = DataManager.Ins.PProgressData.LevelId + 1;

                SaferioTracking.TrackRewardedAdCompleted(level - 1, "Revive");
            }*/

        }

        #endregion
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(GUIRevive))]
    [CanEditMultipleObjects]
    public class GUILevelLoseEditor : Editor
    {
        private GUIRevive script;
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            script = (GUIRevive)target;

            ButtonResetValues();
        }

        private void ButtonResetValues()
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Reset Values", GUILayout.Width(InspectorConst.BUTTON_WIDTH_MEDIUM)))
            {
                script.ResetValues();
                EditorUtility.SetDirty(script); // Mark the object as dirty
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
    }
#endif
}