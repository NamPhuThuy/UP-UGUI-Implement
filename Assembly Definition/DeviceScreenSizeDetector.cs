using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace NamPhuThuy.UGUIAdapter
{
    public class DeviceScreenSizeDetector : MonoBehaviour
    {
        private void Start()
        {
            StartCoroutine(DelayDetect());
        }

        private IEnumerator DelayDetect()
        {
            yield return new WaitForEndOfFrame();

            RectTransform canvas = GetComponent<RectTransform>();
            UGUIConst.CANVAS_SIZE_MOBILE = canvas.rect.size;
        }
    }

}

