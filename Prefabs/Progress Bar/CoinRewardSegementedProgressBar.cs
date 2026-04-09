using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using NamPhuThuy.AnimateWithScripts;
using NamPhuThuy.Common;
using NamPhuThuy.DataManage;
using Spine;
using Spine.Unity;
using UnityEngine;
using DebugLogger = NamPhuThuy.Common.DebugLogger;

namespace NamPhuThuy.UGUIImplement
{
    public class CoinRewardSegmentedProgressBar : SegmentedProgressBar
    {
        #region GiftBox Animation
        [Header("Gift Animation")]
        [SerializeField] private SkeletonGraphic giftSkeleton;
        public SkeletonGraphic GiftSkeleton => giftSkeleton; 
        [SerializeField] private RectTransform giftTargetPivot;
        [SerializeField] private float giftFlyDuration = 1f;
        private float pathCurveHeight = 1f;

        private Vector3 _giftInitialScale;
        private const string AnimAppearString = "appear";
        private const string AnimBeginString = "begin";
        private const string AnimEndString = "end";
        private const string AnimIdleOpenString = "idle_open";
        private const string AnimOpenString = "open";
        #endregion

        private Action onHandleRewardProgressCompleted;

        public override void HandleRewardCompleted(Action onCompletedAction)
        {
            DebugLogger.Log();
            base.HandleRewardCompleted(onCompletedAction);

            _giftInitialScale = giftSkeleton.transform.localScale;

            StartCoroutine(IE_FlyTheGiftBox());

            onHandleRewardProgressCompleted = onCompletedAction;
        }

        IEnumerator IE_FlyTheGiftBox()
        {
            DebugLogger.Log();
            // Create path points for curved movement
            Vector3 startPos = giftSkeleton.transform.position;
            Vector3 endPos = giftTargetPivot.position;
            Vector3 controlPoint = (startPos + endPos) / 2f + Vector3.up * pathCurveHeight;
            
            // DebugLogger.Log(message:$"startPos: {startPos}, endPos: {startPos}, controlPoint: {controlPoint}");

            Vector3[] path = new Vector3[3];
            path[0] = startPos;
            path[1] = controlPoint;
            path[2] = endPos;

            // Turn off some things
            PrimeTween.Tween.Alpha(groupRewardProgress, 0f, 0.3f)
                .OnComplete(() =>
                {
                    groupRewardProgress.gameObject.SetActive(false);

                    rewardProgressBackground.gameObject.SetActive(false);
                    rewardProgressText.gameObject.SetActive(false);
                });
            groupRewardProgress.transform.DOScale(0, duration: 0.3f);

            // Animate gift box along the path
            bool flyComplete = false;
            giftSkeleton.transform
                .DOPath(path, giftFlyDuration, PathType.CatmullRom)
                .SetEase(Ease.OutQuad)/*.OnWaypointChange(value => DebugLogger.Log(message:$"wayPoint Value: {value}"))*/
                .OnComplete(() => flyComplete = true);

            giftSkeleton.transform.DOScale(_giftInitialScale * 2.5f, giftFlyDuration).SetEase(Ease.OutQuad);

            yield return new WaitUntil(() => flyComplete);

            giftSkeleton.gameObject.SetActive(true);

            giftSkeleton.AnimationState.SetAnimation(0, AnimOpenString, false);
            giftSkeleton.AnimationState.Complete += OnCompleteOpen;
        }
        
        void OnCompleteOpen(TrackEntry trackEntry)
        {
            DebugLogger.Log();
            var track = giftSkeleton.AnimationState.SetAnimation(0, AnimIdleOpenString, true);
            track.MixDuration = 0f;
            giftSkeleton.AnimationState.Complete -= OnCompleteOpen;

            DOVirtual.DelayedCall(0.8f, () =>
            {
                OnGiftOpenCompleted();
            });
        }

        void OnGiftOpenCompleted(TrackEntry trackEntry = null)
        {
            DebugLogger.Log();
            groupRewardProgress.gameObject.SetActive(true);
            PrimeTween.Tween.Alpha(groupRewardProgress, 1f, 0.3f);
            groupRewardProgress.transform.DOScale(1, duration: 0.3f);

            giftSkeleton.transform.DOScale(1.2f * giftSkeleton.transform.localScale, duration: 0.3f)
            .OnComplete(() =>
            {
                giftSkeleton.transform.DOScale(0, 0.4f)
                .OnComplete(() =>
                {
                    giftSkeleton.Skeleton.SetToSetupPose();
                    giftSkeleton.AnimationState.ClearTrack(0);

                    giftSkeleton.transform.localScale = _giftInitialScale;

                    giftSkeleton.gameObject.SetActive(false);
                });
            });
            
            

            List<ResourceAmount> rewards = DataManager.Ins.EventRewardData.GetRewards(EventRewardType.WIN_LEVEL);

            var args = new ItemFlyArgs
            {
                AddValue = rewards.GetCoinAmount(),
                PrevValue = DataManager.Ins.PInventoryData.Coin,
                TargetText = UGUIManager.Ins.GUILevelWin.CoinPanel.CoinText.transform,
                StartPosition = giftSkeleton.transform.position,
                TargetInteractTransform = UGUIManager.Ins.GUILevelWin.CoinPanel.transform, // For positioning the target
                ItemAmount = 6,
                ItemSprite = DataManager.Ins.ResourceData.GetResourceRecord(ResourceType.COIN).gameplayImage,
                OnItemInteract = () =>
                {
                    // TurnOnStatChangeVFX(coinText)
                    
#if USE_AUDIO
                    AudioManager.Ins.Play(AudioEnum.SFX_COIN_3);
#endif
                },
                OnComplete = () =>
                {
                    Debug.Log("Animation complete!");
#if USE_AUDIO
                    AudioManager.Ins.Play(AudioEnum.SFX_COIN_1);
#endif
                    
                    DataManager.Ins.PInventoryData.TryApplyRewards(rewards, 1, isUseUpdateAnim: false); 
                }
            };
            
            AnimationManager.Ins.Play(args);
           

            DOVirtual.DelayedCall(2.5f, () =>
            {
                onHandleRewardProgressCompleted?.Invoke();
            });
        }

       

        public void SetGiftPosition()
        {
            DebugLogger.Log();
            giftSkeleton.gameObject.SetActive(true);

            Vector3 giftPositon = giftSkeleton.transform.localPosition.ChangeX(
                rewardProgressBackground.transform.localPosition.x + 0.5f * rewardProgressBackground.sizeDelta.x
            );

            giftSkeleton.transform.localPosition = giftPositon;
        }
    }
}
