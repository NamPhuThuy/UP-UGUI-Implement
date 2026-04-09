using System;
using DG.Tweening;
using NamPhuThuy.Common;
using NamPhuThuy.DataManage;
using UnityEngine;

namespace NamPhuThuy.UGUIImplement
{
    public class PictureRewardSegmentedProgressBar : SegmentedProgressBar
    {
        [SerializeField] private RectTransform picture;

        #region Public Methods

        public void ScaleDownThePicture()
        {
            DebugLogger.Log();
            picture.DOScale(1.2f * picture.localScale, 0.1f)
                .OnComplete(() =>
                {
                    picture.DOScale(Vector3.zero, 0.2f);
                });
        }

        #endregion

        public void SetPicturePosition()
        {
            Vector3 position = picture.transform.localPosition;

            position.x = rewardProgressBackground.transform.localPosition.x + 0.5f * rewardProgressBackground.sizeDelta.x;
            position.y = rewardProgressBackground.transform.localPosition.y - 0.1f * rewardProgressBackground.sizeDelta.y;

            picture.transform.localPosition = position;
            picture.localEulerAngles = new Vector3(0, 0, -15);
            picture.transform.localScale = 0.15f * Vector3.one;
        }
    }
}
