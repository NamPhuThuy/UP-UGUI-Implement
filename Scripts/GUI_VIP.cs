using DG.Tweening;
using MoreMountains.Tools;
using NamPhuThuy.DataManage;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace NamPhuThuy.UGUIImplement
{
    public enum VIPPlan
    {
        NONE = 0,
        MONTHLY = 1,
        LIFETIME = 2
    }

    
    public class GUI_VIP : GUIBase, MMEventListener<EIAPInfoFetched>
    {
        [Header("Flags")] 
        [SerializeField] private VIPPlan currentVIPPlan;
        
        [Header("Buttons")]
        [SerializeField] private Button closeButton;
        [SerializeField] private Button buyButton;
        [SerializeField] private TextMeshProUGUI priceText;
        // [SerializeField] private SkeletonGraphic vipSkeleton;

        [Header("Supsription Plan")]
        [SerializeField] private Button monthlyPlanButton;
        [SerializeField] private Button lifeTimePlanButton;
        [SerializeField] private Image background;
        [SerializeField] private Image benefitBackground;
        [SerializeField] private Sprite[] backgrounds;
        [SerializeField] private Sprite[] benefitBackgrounds;

        [SerializeField] private IAPRecord vipIAPRecord;

        private bool _isSwitchingPlan;

        #region MonoBehaviour Callbacks

        private void Awake()
        {
            monthlyPlanButton.onClick.AddListener(() => SwitchSubscriptionPlan(VIPPlan.MONTHLY));
            lifeTimePlanButton.onClick.AddListener(() => SwitchSubscriptionPlan(VIPPlan.LIFETIME));
            buyButton.onClick.AddListener(Buy);
            closeButton.onClick.AddListener(() => Hide());
        }

        #endregion

        public override void Show(params object[] parameters)
        {
            base.Show(parameters);

            // vipSkeleton.SetAlpha(1);
        }

        public override void Hide(params object[] parameters)
        {
            base.Hide(parameters);

            // vipSkeleton.DOFade(0, duration: 0.3f);
        }

        private void Buy()
        {
            // IAPManager.Ins.BuyProduct(vipIAPRecord.BundleId);
        }

        private void SwitchSubscriptionPlan(VIPPlan plan)
        {
            if (plan == currentVIPPlan)
            {
                return;
            }

            if (_isSwitchingPlan)
            {
                return;
            }
            else
            {
                _isSwitchingPlan = true;
            }

            /*background.DOFade(0, duration: 0.3f).SetEase(Ease.InOutSine).OnComplete(() =>
            {
                background.sprite = backgrounds[(int)plan];
                benefitBackground.sprite = benefitBackgrounds[(int)plan];

                background.DOFade(1, duration: 0.3f).SetEase(Ease.InOutSine);
            });

            benefitBackground.DOFade(0, duration: 0.3f).SetEase(Ease.InOutSine).OnComplete(() =>
            {
                benefitBackground.DOFade(1, duration: 0.3f).SetEase(Ease.InOutSine).OnComplete(() =>
                {
                    _isSwitchingPlan = false;
                });
            });*/

            if (plan == VIPPlan.MONTHLY)
            {
                monthlyPlanButton.transform.DOScale(1, duration: 0.3f).SetEase(Ease.InOutSine);
                lifeTimePlanButton.transform.DOScale(1, duration: 0.3f).SetEase(Ease.InOutSine);
            }
            else
            {
                monthlyPlanButton.transform.DOScale(0.75f, duration: 0.3f).SetEase(Ease.InOutSine);
                lifeTimePlanButton.transform.DOScale(1.25f, duration: 0.3f).SetEase(Ease.InOutSine);
            }

            currentVIPPlan = plan;
        }

        public void OnMMEvent(EIAPInfoFetched eventArgs)
        {
            foreach (var iapData in eventArgs.iapData)
            {
                if (iapData.BundleId == vipIAPRecord.BundleId)
                {
                    priceText.text = iapData.Price;

                    break;
                }
            }
        }
    }
}
