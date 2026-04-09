using NamPhuThuy.DataManage;
using UnityEngine;
using UnityEngine.UI;

namespace NamPhuThuy.UGUIImplement
{

    public class GUIConfirmReturnHome : GUIBase
    {
        #region Private Serializable Fields

        [SerializeField] private Button closeButton;
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button returnHomeButton;

        #endregion

        #region Private Fields

        #endregion

        #region MonoBehaviour Callbacks

        private void OnEnable()
        {
            closeButton.onClick.AddListener((() => { UGUIManager.Ins.HideGUI(this); }));
            resumeButton.onClick.AddListener((() => { UGUIManager.Ins.HideGUI(this); }));
            returnHomeButton.onClick.AddListener((() => { OnClickBack(); }));
        }

        private void OnDisable()
        {
            closeButton.onClick.RemoveAllListeners();
            resumeButton.onClick.RemoveAllListeners();
            returnHomeButton.onClick.RemoveAllListeners();
        }

        #endregion

        #region Private Methods

        private void OnClickBack()
        {
            /*GUIManager.Ins.ShowGUI(GUIManager.Ins.GUIHome);
            GamePlayManager.Ins.GamePlayStateMachine.ChangeState(GamePlayManager.Ins.GamePlayStateMachine.HomeState.gameObject);
            Hide();
            GUIManager.Ins.GUIHUD.Hide();

            SaferioTracking.TrackReturnHome(DataManager.Ins.PProgressData.LevelId + 1);*/
        }

        #endregion

        #region Public Methods

        #endregion

    }

}
