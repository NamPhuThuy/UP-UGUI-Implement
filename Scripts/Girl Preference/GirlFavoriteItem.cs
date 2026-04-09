using DG.Tweening;
using NamPhuThuy.Common;
using UnityEngine;
using UnityEngine.UI;

namespace NamPhuThuy.UGUIImplement
{
    public class GirlFavoriteItem : MonoBehaviour
    {
        [Header("Components")] 
        [SerializeField] private Image backgroundImage;
        [SerializeField] private RectTransform container;
        [SerializeField] private Button selectButton;
        [SerializeField] private Image previewImage;
        [SerializeField] private Image titleBanner;
        [SerializeField] private Image imageFrame;
        [SerializeField] private Image imageUpperFrame;
        
        
        [SerializeField] private string packName;
        [SerializeField] private ParticleSystem psEffect;

        [Header("Assets")] 
        [SerializeField] private Sprite centerTitleBanner;
        [SerializeField] private Sprite centerFrame;
        [SerializeField] private Sprite centerUpperFrame;

        
        
        [SerializeField] private Sprite sideTitleBanner;
        [SerializeField] private Sprite sideFrame;
        [SerializeField] private Sprite sideUpperFrame;
        
        public string PackName => packName;
        public Vector3 AnchoredPosition => container.anchoredPosition;

        #region MonoBehaviour Methods

        void Awake()
        {
            selectButton.onClick.AddListener(Select);
        }

        #endregion

        public void Select()
        {
            // GUIManager.Ins.GUIGirlPreference.MoveToItem(this);
        }

        public void MoveToCenter()
        {
            DebugLogger.Log(context: this);
            titleBanner.sprite = centerTitleBanner;
            imageUpperFrame.sprite = centerUpperFrame;
            backgroundImage.gameObject.SetActive(true);

            PrimeTween.Tween.Scale(container, 1f, 0.3f).OnComplete(() =>
            {
                container.localScale = Vector3.one;
                psEffect.gameObject.SetActive(true);
                psEffect.Play();
            });
            container.DOScale(1, duration: 0.3f);
            container.SetAsLastSibling();

            // previewImage.FitImageToRectTransformScreenSpaceCamera(container);
        }

        public void MoveOutOfCenter()
        {
            DebugLogger.Log(context: this);
            titleBanner.sprite = sideTitleBanner;
            imageUpperFrame.sprite = sideUpperFrame;
            
            backgroundImage.gameObject.SetActive(false);
            psEffect.Stop();
            psEffect.gameObject.SetActive(false);
            
            container.DOScale(0.5f, duration: 0.3f)
                .OnComplete(() =>
                {
                    // titleBanner.gameObject.SetActive(false);
                    container.localScale = 0.5f * Vector3.one;
                    
                });
        }
    }
}