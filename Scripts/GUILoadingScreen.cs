using System;
using System.Collections;
using System.Collections.Generic;
using NamPhuThuy.AdNetworkAdapter;
using NamPhuThuy.Common;
using NamPhuThuy.DataManage;
using UnityEngine;
using UnityEngine.UI;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace NamPhuThuy.UGUIImplement
{

    public class GUILoadingScreen : GUIBase
    {
        #region Private Serializable Fields

        [SerializeField] private CanvasGroup canvasGroupTotal;
        [SerializeField] private Slider loadingSlider;
        [SerializeField] private Image loadingImage;
        [SerializeField] private Image blackBackground;

        [SerializeField] private Sprite placeholderSprite;

        public Image LoadingImage
        {
            set => loadingImage = value;
            get => loadingImage;
        }

        #endregion

        #region Private Fields

        private PrimeTween.Tween _loadTween;
        private Action _onLoadingComplete;

        #endregion

        #region Override Methods

        public override void Show(params object[] parameters)
        {
            base.Show(parameters);
            AdsManager.Ins.TryShow_MRec_MAX();
            
            DebugLogger.Log();

            _onLoadingComplete = null;

            if (parameters is { Length: > 0 } && parameters[0] is float durationParam)
            {
                DebugLogger.Log(message:$"Check 1");
                loadDuration = durationParam;
            }
            else
            {
                DebugLogger.Log(message:$"Check 2");
                loadDuration = UGUIConst.FAKE_LOAD_DURATION;
            }

            if (parameters is { Length: > 1 } && parameters[1] is Action onComplete)
            {
                DebugLogger.Log(message:$"Check 3");
                _onLoadingComplete = onComplete;
            }
            
            _loadTween.Stop();

            canvasGroupTotal.alpha = 1;

            loadingSlider.value = 0f;
            _loadTween = PrimeTween.Tween.Custom(0f, 1f, loadDuration, ease: PrimeTween.Ease.Linear,
                    onValueChange: val => loadingSlider.value = val)
                .OnComplete(() =>
                {
                    AdsManager.Ins.Hide_MRec_MAX();
                    _onLoadingComplete?.Invoke();

                    PrimeTween.Tween.Alpha(canvasGroupTotal, 0f, duration: 0.4f).OnComplete(() =>
                    {
                        PrimeTween.Tween.Delay(0.1f, (() => HideFast()));
                    });
                });

            blackBackground.gameObject.SetActive(false);

            // UpdateBackImage(DataManager.Ins.PProgressData.CurrentBackgroundId);
        }

        [SerializeField] private float loadDuration;
        public override void Hide(params object[] parameters)
        {
            base.Hide(parameters);
            _loadTween.Complete();
            AdsManager.Ins.Hide_MRec_MAX();
            
        }

        public override void HideFast(params object[] parameters)
        {
            base.HideFast(parameters);
            _loadTween.Complete();
            AdsManager.Ins.Hide_MRec_MAX();
        }

        #endregion

        #region Private Methods
        #endregion

        #region Public Methods


        #endregion
    }
}
