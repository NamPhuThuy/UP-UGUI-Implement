using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace NamPhuThuy.UGUIAdapter
{
    
    public class TweenHelper : MonoBehaviour
    {
        public static void PopupScaleSquence(Transform objTransform, Action onComplete = null)
        {
            // Debug.Log($"TweenHelper: PopupScaleSquence - {objTransform.name}");
            objTransform.gameObject.SetActive(true);
            
            Vector3 originalScale = objTransform.localScale;
            objTransform.localScale = Vector3.zero;
                
            Sequence seq = DOTween.Sequence();
            seq.Append(objTransform.DOScale(originalScale * 1.2f, 0.3f));
            seq.Append(objTransform.DOScale(originalScale * .8f, 0.3f));
            seq.Append(objTransform.DOScale(originalScale, 0.3f))
                .OnComplete(() => onComplete?.Invoke());
        }
        
        public static void PopupScaleSquence(Transform objTransform, float targetScale, Action onComplete = null)
        {
            // Debug.Log($"TweenHelper: PopupScaleSquence - {objTransform.name}");
            objTransform.gameObject.SetActive(true);
            
            Vector3 originalScale = Vector3.one * targetScale;
            objTransform.localScale = Vector3.zero;
                
            Sequence seq = DOTween.Sequence();
            seq.Append(objTransform.DOScale(originalScale * 1.2f, 0.3f));
            seq.Append(objTransform.DOScale(originalScale * .8f, 0.3f));
            seq.Append(objTransform.DOScale(originalScale, 0.3f))
                .OnComplete(() => onComplete?.Invoke());
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
        ///       .OnComplete(() => Destroy(gameObject));
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
        
        
        public static Tweener OpenDropdown(RectTransform dropdown, CanvasGroup dropdownCanvasGroup)
        {
            dropdown.gameObject.SetActive(true);
            dropdownCanvasGroup.interactable = false;

            // Reset scale to start state
            dropdown.localScale = new Vector3(1, 0, 1);

            // We want to pivot from the top, so we calculate the top position
            // However, changing pivot at runtime can be messy for layout groups.
            // Instead, we can simulate the "slide down" effect by moving the position
            // while scaling, effectively keeping the top edge fixed.

            Vector3 initialLocalPos = dropdown.localPosition;
            float height = dropdown.rect.height;
            
            // If pivot.y is 0.5 (center), the top edge is at y + height/2.
            // When scale.y is 0, the center is at the same y.
            // To keep top edge fixed, as scale grows from 0 to 1:
            // At scale 0: center should be at (Top - 0) = Top
            // At scale 1: center should be at (Top - height/2) = InitialPos
            
            // Let's assume the initial position is the correct "open" position.
            // We calculate the top edge Y based on current pivot/anchors.
            // For a standard center-pivot UI element:
            float pivotOffset = (1f - dropdown.pivot.y) * height; 
            // If pivot is 0.5, offset is 0.5 * height.
            // If pivot is 1 (top), offset is 0.
            
            // The visual top of the rect in local space relative to its pivot is +pivotOffset.
            // We want that visual top to stay fixed in parent space.
            
            // Actually, the previous implementation logic was:
            // localPosition.y = topPosition - 0.5f * dropdown.sizeDelta.y * dropdown.localScale.y;
            // This assumes pivot is 0.5. Let's generalize or stick to the working logic but cleaner.
            
            // Optimization: Use a single tween on a float (0 to 1) and update both scale and position.
            // This avoids creating a separate OnUpdate delegate that captures variables every frame if possible,
            // though DOTween handles this well.
            
            // Better approach: Just set the pivot to (0.5, 1) if possible? 
            // If we can't change pivot, we simulate it.
            
            float topY = initialLocalPos.y + (1f - dropdown.pivot.y) * height;

            return DOVirtual.Float(0f, 1f, 0.3f, (val) =>
            {
                // Scale Y
                var s = dropdown.localScale;
                s.y = val;
                dropdown.localScale = s;

                // Position Y to keep top fixed
                // Current Height from pivot to top = (1 - pivot.y) * height * scaleY
                // We want the top edge to be at `topY`.
                // So pivot position = topY - (distance from pivot to top)
                var p = dropdown.localPosition;
                p.y = topY - (1f - dropdown.pivot.y) * height * val;
                dropdown.localPosition = p;
            })
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                dropdown.localScale = Vector3.one;
                dropdown.localPosition = initialLocalPos;
                dropdownCanvasGroup.interactable = true;
            });
        }

        public static List<Tweener> HideDropdown(RectTransform dropdown, CanvasGroup dropdownCanvasGroup)
        {
            List<Tweener> tweeners = new List<Tweener>();
            dropdownCanvasGroup.interactable = false;

            // Fade out
            tweeners.Add(dropdownCanvasGroup.DOFade(0, 0.15f));

            Vector3 initialLocalPos = dropdown.localPosition;
            float height = dropdown.rect.height;
            float topY = initialLocalPos.y + (1f - dropdown.pivot.y) * height;

            // Scale down
            var scaleTween = DOVirtual.Float(1f, 0f, 0.3f, (val) =>
            {
                var s = dropdown.localScale;
                s.y = val;
                dropdown.localScale = s;

                var p = dropdown.localPosition;
                p.y = topY - (1f - dropdown.pivot.y) * height * val;
                dropdown.localPosition = p;
            })
            .SetEase(Ease.InQuad)
            .OnComplete(() =>
            {
                dropdown.gameObject.SetActive(false);
                
                // Reset for next open
                dropdown.localScale = Vector3.one;
                dropdown.localPosition = initialLocalPos;
                dropdownCanvasGroup.alpha = 1f;
                dropdownCanvasGroup.interactable = true;
            });
            
            tweeners.Add(scaleTween);

            return tweeners;
        }
    }
    
    
}