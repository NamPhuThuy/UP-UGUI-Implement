
using DG.Tweening;

namespace NamPhuThuy.UGUIImplement
{
    public class GUILevelDifficultyAlert : GUIBase
    {
        // [SerializeField] private LeanLocalizedTextMeshProUGUI messageText;

        public override void Show(params object[] parameters)
        {
            // GamePlayManager.Ins.GamePlayStateMachine.PlayLevelState.GameInput.DisactiveInput();
            // GUIManager.Ins.GUIHUD.DisableInteract();
            
            DOVirtual.DelayedCall(1.5f, () =>
            {
                ShowMessage();
                base.Show(parameters);
            });
            
        }

        public override void Hide(params object[] parameters)
        {
            base.Hide(parameters);
            // GamePlayManager.Ins.GamePlayStateMachine.PlayLevelState.GameInput.ActiveInput();
            // GUIManager.Ins.GUIHUD.EnableInteract();
        }

        private void ShowMessage()
        {
            /*float levelDifficulty = GamePersistentVariable.currentLevelDifficulty;
            float percent = (1 - levelDifficulty / 1000) * 90 + Random.Range(-10f, 10f);
            percent = Mathf.Clamp(percent, 0.1f, 99.9f);

            string[] listTranslations = GameConstants.beforeLevelPercentPlayerPassLevelTranslationNames;

            messageText.TranslationName = listTranslations[Random.Range(0, listTranslations.Length)];
            messageText.UpdateTranslationWithParameter(LocalizationConst.PERCENT, $"<color=#FF3B56>{percent.ToString("F1")}%</color>");

            GamePersistentVariable.percentPlayerPassLevel = percent;

            DOVirtual.DelayedCall(2, () =>
            {
                Hide();
            });*/
        }
    }
}
