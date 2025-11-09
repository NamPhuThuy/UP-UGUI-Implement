using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace NamPhuThuy.UI
{
    
    public class TweenHelper : MonoBehaviour
    {
        public static void PopupScaleSquence(Transform objTransform)
        {
            // Debug.Log($"TweenHelper: PopupScaleSquence - {objTransform.name}");
            objTransform.gameObject.SetActive(true);
            
            Vector3 originalScale = objTransform.localScale;
            objTransform.localScale = Vector3.zero;
                
            Sequence seq = DOTween.Sequence();
            seq.Append(objTransform.DOScale(originalScale * 1.2f, 0.3f));
            seq.Append(objTransform.DOScale(originalScale * .8f, 0.3f));
            seq.Append(objTransform.DOScale(originalScale, 0.3f));
        }
        
        public static void PopupScaleSquence(Transform objTransform, float targetScale)
        {
            // Debug.Log($"TweenHelper: PopupScaleSquence - {objTransform.name}");
            objTransform.gameObject.SetActive(true);
            
            Vector3 originalScale = Vector3.one * targetScale;
            objTransform.localScale = Vector3.zero;
                
            Sequence seq = DOTween.Sequence();
            seq.Append(objTransform.DOScale(originalScale * 1.2f, 0.3f));
            seq.Append(objTransform.DOScale(originalScale * .8f, 0.3f));
            seq.Append(objTransform.DOScale(originalScale, 0.3f));
        }

        #region PUNCH

        /// <summary>
        /// Punch the local scale uniformly and return to the original.
        /// </summary>
        public static Tweener PunchScale(Transform target, float amplitude = 0.15f, float duration = 0.25f, int vibrato = 8, float elasticity = 1f)
        {
            if (target == null) return null;
            target.gameObject.SetActive(true);
            return target.DOPunchScale(Vector3.one * amplitude, duration, vibrato, elasticity);
        }

        /// <summary>
        /// Punch the local scale with a custom vector amplitude.
        /// </summary>
        public static Tweener PunchScale(Transform target, Vector3 amplitude, float duration = 0.25f, int vibrato = 8, float elasticity = 1f)
        {
            if (target == null) return null;
            target.gameObject.SetActive(true);
            return target.DOPunchScale(amplitude, duration, vibrato, elasticity);
        }

        /// <summary>
        /// Punch the world position of a Transform (relative offset) and return.
        /// </summary>
        public static Tweener PunchPosition(Transform target, Vector3 punchOffset, float duration = 0.35f, int vibrato = 8, float elasticity = 1f, bool snapping = false)
        {
            if (target == null) return null;
            target.gameObject.SetActive(true);
            return target.DOPunchPosition(punchOffset, duration, vibrato, elasticity, snapping);
        }

        /// <summary>
        /// Punch the rotation in Euler degrees and return.
        /// </summary>
        public static Tweener PunchRotation(Transform target, Vector3 punchEuler, float duration = 0.35f, int vibrato = 8, float elasticity = 1f)
        {
            if (target == null) return null;
            target.gameObject.SetActive(true);
            return target.DOPunchRotation(punchEuler, duration, vibrato, elasticity);
        }

        /// <summary>
        /// Kill all tweens attached to this Transform (useful before destroy/disable).
        /// </summary>
        public static void KillTweens(Transform target, bool complete = false)
        {
            if (target == null) return;
            DOTween.Kill(target, complete);
        }

        #endregion
        
        /// <summary>
        /// Kill all tweens in the list. Optionally complete them and clear the list.
        /// </summary>
        public static void StopAllTweens(IList<Tween> tweens, bool complete = false, bool clearList = true)
        {
            if (tweens == null) return;

            for (int i = 0; i < tweens.Count; i++)
            {
                var t = tweens[i];
                if (t == null) continue;
                if (t.IsActive())
                    t.Kill(complete);
                else
                    t.Kill(complete);
            }

            if (clearList)
                tweens.Clear();
        }



        #region PULSE

        // Pulse: scale up then back to start, repeated `loops` times.
        public static Sequence DOScalePulse(
            Transform target,
            float upMul = 1.15f,
            float upDuration = 0.2f,
            float downDuration = 0.18f,
            int loops = 2,
            Ease upEase = Ease.InOutSine,
            Ease downEase = Ease.InOutSine)
        {
            if (target == null) return null;

            var start = target.localScale;
            var seq = DOTween.Sequence();
            loops = Mathf.Max(0, loops);

            for (int i = 0; i < loops; i++)
            {
                seq.Append(target.DOScale(start * upMul, upDuration).SetEase(upEase));
                seq.Append(target.DOScale(start, downDuration).SetEase(downEase));
            }

            return seq;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="upMul"></param>
        /// <param name="upDuration"></param>
        /// <param name="downDuration"></param>
        /// <param name="loops"></param>
        /// <param name="upEase"></param>
        /// <param name="downEase"></param>
        /// <param name="shrinkDuration"></param>
        /// <param name="shrinkEase"></param>
        /// <example>
        /// Example usage:
        /// - Default (pulse twice then shrink):
        ///   TweenHelper.DOScalePulseAndShrink(transform);
        /// - Await inside a coroutine:
        ///   yield return TweenHelper.DOScalePulseAndShrink(transform).WaitForCompletion();
        /// - On complete cleanup:
        ///   TweenHelper.DOScalePulseAndShrink(transform, loops: 3)
        ///       .OnComplete(() =&gt; Destroy(gameObject));
        /// </example>
        /// <returns></returns>
        public static Sequence DOScalePulseAndShrink(
            Transform target,
            float upMul = 1.15f,
            float upDuration = 0.2f,
            float downDuration = 0.18f,
            int loops = 2,
            Ease upEase = Ease.InOutSine,
            Ease downEase = Ease.InOutSine,
            float shrinkDuration = 0.18f,
            Ease shrinkEase = Ease.InSine)
        {
            if (target == null) return null;

            var seq = DOScalePulse(target, upMul, upDuration, downDuration, loops, upEase, downEase);
            seq.Append(target.DOScale(0f, shrinkDuration).SetEase(shrinkEase));
            return seq;
        }
        
        #endregion
        
    }
}