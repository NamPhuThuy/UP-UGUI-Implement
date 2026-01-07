using NamPhuThuy;
using UnityEngine;

namespace NamPhuThuy.UGUIImplement
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteAutoScaler : MonoBehaviour
    {
        #region MonoBehaviour Callbacks

        void Start()
        {
            ScaleSpriteToScreen();
        }

        #endregion

        public void ScaleSpriteToScreen()
        {
            SpriteRenderer sr = GetComponent<SpriteRenderer>();

            if (sr.sprite == null)
            {
                Debug.LogWarning("SpriteAutoScaler: No sprite assigned to SpriteRenderer.");
                return;
            }

            // // Get sprite size in world units
            // float spriteWidth = sr.sprite.bounds.size.x;
            // float spriteHeight = sr.sprite.bounds.size.y;

            // // Get screen size in world units
            // float worldScreenHeight = Camera.main.orthographicSize * 2f;
            // float worldScreenWidth = worldScreenHeight * Screen.width / Screen.height;

            // // Calculate scale
            // float scaleX = worldScreenWidth / spriteWidth;
            // float scaleY = worldScreenHeight / spriteHeight;

            // // Apply scale — use the bigger scale to ensure it covers the screen
            // transform.localScale = new Vector3(scaleX, scaleY, 1f);

            // Maintain aspect ratio 
            Vector2 spriteSize = sr.sprite.bounds.size;

            float screenHeight = Camera.main.orthographicSize * 2f;
            float screenWidth = screenHeight * Screen.width / Screen.height;

            float scaleX = screenWidth / spriteSize.x;
            float scaleY = screenHeight / spriteSize.y;

            float finalScale = Mathf.Max(scaleX, scaleY);

            transform.localScale = new Vector3(finalScale, finalScale, 1f);
        }
    }
}