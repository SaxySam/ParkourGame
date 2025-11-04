using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace PhotoCamera
{
    public class CameraFlash : MonoBehaviour
    {
        [SerializeField] private Image flashImage;
        [SerializeField] private float flashAlpha = 0.8f;
        [SerializeField] private float flashDuration = 0.1f;

        void Start()
        {
            flashImage.color = new Color(flashImage.color.r, flashImage.color.g, flashImage.color.b, 0);
            flashImage.gameObject.SetActive(false);
        }

        public void FlashCamera()
        {
            flashImage.gameObject.SetActive(true);
            flashImage.color = new Color(flashImage.color.r, flashImage.color.g, flashImage.color.b, flashAlpha);
            StartCoroutine(FlashFade());
        }

        private IEnumerator FlashFade()
        {
            float elapsed = 0f;
            while (elapsed < flashDuration)
            {
                elapsed += Time.deltaTime;
                float alpha = Mathf.Lerp(flashAlpha, 0, elapsed / flashDuration);
                flashImage.color = new Color(flashImage.color.r, flashImage.color.g, flashImage.color.b, alpha);
                yield return null;
            }
            flashImage.gameObject.SetActive(false);
        }
    }
}