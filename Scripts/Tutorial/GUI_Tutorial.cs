using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.UI;
using NamPhuThuy.DataManage;
using NamPhuThuy.PuzzleTutorial;
using PrimeTween;
using TMPro;
using Sequence = PrimeTween.Sequence;
using Ease = PrimeTween.Ease;

namespace NamPhuThuy.UGUIImplement
{
    public class GUI_Tutorial : GUIBase
    {
        #region Serialize Private Fields

        [SerializeField] private CanvasGroup tutorialGroup;
        
        [SerializeField] private Image movingBoosterImage;
        [SerializeField] private Button claimButton;

        [SerializeField] private RectTransform[] referenceBoosters;
        [SerializeField] private RectTransform booster;

        [SerializeField] private Sprite[] boosterSprites;
        [SerializeField] private string[] boosterDescriptionTranslations;
        
        [Header("Components")]
        [SerializeField] private Image boosterImage;
        [SerializeField] private TextMeshProUGUI descriptionText;

        #endregion

        #region Private Fields

        private int _boosterIndex;
        private Vector3 _initialBoosterPosition;
        private Vector2 _initialBoosterSizeDelta;

        #endregion

        #region MonoBehaviour Callbacks

        void Awake()
        {
            claimButton.onClick.AddListener(OnClickClaim);

            _initialBoosterPosition = booster.position;
            _initialBoosterSizeDelta = booster.sizeDelta;
        }

        #endregion

        #region Private Methods

        private void Setup(BoosterType boosterType)
        {
            if (boosterType == BoosterType.UNDO)
            {
                _boosterIndex = 0;
            }
            else if (boosterType == BoosterType.SHUFFLE)
            {
                _boosterIndex = 1;
            }
            else
            {
                _boosterIndex = 2;
            }

            boosterImage.sprite = boosterSprites[_boosterIndex];
            movingBoosterImage.sprite = boosterSprites[_boosterIndex];
            // boosterDescription.TranslationName = boosterDescriptionTranslations[_boosterIndex];
        }

        private void OnClickClaim()
        {
            claimButton.interactable = false;
            Hide();
            return;

            /*Sequence seq = Sequence.Create();

            seq.Group(Tween.Alpha(tutorialGroup, 0, 0.3f, Ease.InOutSine))
                .Group(Tween.UISizeDelta(booster, 0.65f * referenceBoosters[_boosterIndex].sizeDelta, 0.3f, Ease.InOutSine))
                .Group(Tween.Position(booster.transform, referenceBoosters[_boosterIndex].transform.position, 0.3f, Ease.InOutSine))
                .ChainCallback(() => AudioManager.Ins.Play(AudioEnum.SFX_REWAR_APPEAR))
                .Chain(Tween.Scale(booster.transform, 1.1f, 0.1f))
                .Chain(Tween.Scale(booster.transform, 1f, 0.1f))
                .Chain(Tween.Alpha(booster.GetComponent<Image>(), 0, 0.3f))
                .OnComplete(() =>
                {
                    Reset();
                    // HideImmediately();
                    HideFast();
                    GUIManager.Ins.GUIHUD.EnableInteract();
                    // TutorialManager.Ins.TutorialHand.StartTutorial(referenceBoosters[_boosterIndex].parent);
                });*/
        }

        #endregion

        #region Override Methods

        public override void Show(params object[] parameters)
        {
            // Setup((BoosterType)parameters[0]);
            base.Show(parameters);

            // GamePlayManager.Ins.GamePlayStateMachine.PlayLevelState.GameInput.DisactiveInput();
            // GUIManager.Ins.GUIHUD.DisableInteract();

            boosterImage.sprite = TutorialManager.Ins.Data.GetTutRecord(DataManager.Ins.PProgressData.LevelId)
                .TutorialImage;
            boosterImage.SetNativeSize();

            descriptionText.text =
                TutorialManager.Ins.Data.GetTutRecord(DataManager.Ins.PProgressData.LevelId).Description;
            
            claimButton.interactable = true;
        }

        public override void Hide(params object[] parameters)
        {
            base.Hide(parameters);
            UGUIManager.Ins.GUIHUD.EnableInteract();
            // MMEventManager.TriggerEvent(new EGUITutorialHidden());
            MMEventManager.TriggerEvent(new EGUIHidden()
            {
                guiId = GUIId.GUI_TUTORIAL
            });
        }


        #endregion

        private void Reset()
        {
            tutorialGroup.alpha = 1;

            booster.position = _initialBoosterPosition;
            booster.sizeDelta = _initialBoosterSizeDelta;
            booster.localScale = Vector3.one;

            booster.GetComponent<Image>().color = Color.white;
        }
    }
}
