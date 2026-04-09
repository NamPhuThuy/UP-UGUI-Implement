
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using DG.Tweening;
using NamPhuThuy.DataManage;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace NamPhuThuy.UGUIImplement
{

    public class GUIGalleryPackResults : GUIBase
    {
        #region Private Serializable Fields

        [Header("Buttons")]
        [SerializeField] private ButtonClicky closeButton;

        [Header("ScrollView")]
        [SerializeField] private RectTransform contentParent;
        [SerializeField] private Element_GalleryPackResult resultPrefab;
        [SerializeField] private GridLayoutGroup contentGridLayoutGroup;
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] List<AlbumRecord> pictureDataList = new List<AlbumRecord>();

        private Vector2 _elementSpacing;
        private Vector2 _elementSize;
        private Vector2 _contentBuffer = new Vector2(0f, 250f);

        #endregion

        #region Private Fields

        #endregion

        #region MonoBehaviour Callbacks

        private void Start()
        {
            //GET REFERENCES
            contentGridLayoutGroup = contentParent.GetComponent<GridLayoutGroup>();

            UpdateSizesInfo();
        }

        private void OnEnable()
        {
            closeButton.onClick.AddListener((() => { Hide(); }));
        }

        private void OnDisable()
        {
            closeButton.onClick.RemoveAllListeners();
        }

        #endregion

        #region Private Methods

        private void InitElement()
        {
            List<int> pictureIds = new List<int>();

            foreach (AlbumRecord albumRecord in pictureDataList)
            {
                Element_GalleryPackResult element = Instantiate(resultPrefab, contentParent.transform);
                element.currentPictureId = albumRecord.recordId;

                StartCoroutine(element.WaitForRemoteAsset());

                pictureIds.Add(albumRecord.recordId);
            }

            // StartCoroutine(DataManager.Ins.FetchPictureDatas(pictureIds));
        }

        private void UpdateContentSizeDelta()
        {
            int itemNum = pictureDataList.Count;
            int rowNum = Mathf.CeilToInt((float)itemNum / contentGridLayoutGroup.constraintCount);

            contentParent.sizeDelta = rowNum * _elementSize + (rowNum + 1) * _elementSpacing;
        }

        private void UpdateSizesInfo()
        {
            //GET THE SIZES
            _elementSize = contentGridLayoutGroup.cellSize;
            _elementSpacing = contentGridLayoutGroup.spacing;

            _elementSize = new Vector2(contentGridLayoutGroup.cellSize.x * (scrollRect.horizontal ? 1 : 0), contentGridLayoutGroup.cellSize.y);
            _elementSpacing = new Vector2(0f, contentGridLayoutGroup.spacing.y);
        }

        private void AutoScrollToTop()
        {
            scrollRect.vertical = false;

            DOVirtual.DelayedCall(0.6f, () =>
            {
                float verticalNormalizedPosition = scrollRect.verticalNormalizedPosition;

                DOTween.To(() => verticalNormalizedPosition, x => verticalNormalizedPosition = x, 1f, 0.3f).OnUpdate(() =>
                {
                    scrollRect.verticalNormalizedPosition = verticalNormalizedPosition;
                })
                .OnComplete(() =>
                {
                    scrollRect.vertical = true;
                });
            });
        }

        #endregion

        #region Public Methods

        #endregion

        #region Override Methods

        public override void Show(params object[] parameters)
        {
            base.Show(parameters);

#if USE_AUDIO
            AudioManager.Ins.Play(AudioEnum.SFX_REWAR_APPEAR);
#endif
            ClearPrevResults();

            pictureDataList = (List<AlbumRecord>)parameters[0];
            InitElement();
            UpdateSizesInfo();
            UpdateContentSizeDelta();

            AutoScrollToTop();

            void ClearPrevResults()
            {
                int childCnt = contentParent.transform.childCount;
                for (int i = 0; i < childCnt; i++)
                {
                    DestroyImmediate(contentParent.transform.GetChild(0).gameObject, true);
                }
            }
        }

        public override void Hide(params object[] parameters)
        {
            base.Hide(parameters);
        }

        #endregion
    }
}