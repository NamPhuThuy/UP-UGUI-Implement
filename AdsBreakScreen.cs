using System;
using PrimeTween;
using UnityEngine;
using UnityEngine.UI;

public class AdsBreakScreen : MonoBehaviour
{
    public static AdsBreakScreen Ins;

    [SerializeField] private RectTransform container;
    [SerializeField] private CanvasGroup canvasGroup;
    

    private void Awake()
    {
        if (Ins != null && Ins != this)
        {
            gameObject.SetActive(false);

            Destroy(gameObject);
        }
        else
        {
            Ins = this;
        }

        container.gameObject.SetActive(false);
    }

    public void Show(Action onCompleteAction = null)
    {
        container.gameObject.SetActive(true);


        canvasGroup.alpha = 0;
        PrimeTween.Tween.Alpha(canvasGroup, 1f, 0.5f);

        PrimeTween.Tween.Delay(1).OnComplete(() => { onCompleteAction?.Invoke(); });
    }

    public void Hide()
    {
        // container.gameObject.SetActive(false);

        if (!container.gameObject.activeInHierarchy)
        {
            return;
        }
        canvasGroup.alpha = 1f;
        PrimeTween.Tween.Alpha(canvasGroup, 0, duration: 0.5f)
            .OnComplete(() => { container.gameObject.SetActive(false); });
    }

    public void HideAnim()
    {
        if (!container.gameObject.activeInHierarchy)
        {
            return;
        }

        /*AddTween(Tween.Alpha(canvasGroup, 0, startDelay: 0.7f, duration: 0.3f)
            .OnComplete(() =>
            {
                container.gameObject.SetActive(false);
            })
        );*/
    }
}