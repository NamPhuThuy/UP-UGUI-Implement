using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace NamPhuThuy.UGUIAdapter
{
    [CreateAssetMenu(fileName = "EnglishOnlyValidator", menuName = "TMPro/Validators/English Only")]
    public class EnglishOnlyValidator : TMPro.TMP_InputValidator
    {
        public override char Validate(ref string text, ref int pos, char ch)
        {
            // ASCII characters from 32 ' ' to 126 '~'
            var index = (int)ch;

            if (index >= 32 && index <= 126)
            {
                text += ch;
                pos += 1;
                return ch;
            }

            return '\0';
        }
    }

}