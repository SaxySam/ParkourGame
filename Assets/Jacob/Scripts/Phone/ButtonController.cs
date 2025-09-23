using UnityEngine;
using UnityEngine.UI;

namespace Phone
{
    public class ButtonController : MonoBehaviour
    {
        public static GameManager.OnGalleryPhotoClick galleryPhotoEvent;

        void ButtonClick()
        {
            galleryPhotoEvent.Invoke(gameObject.GetComponent<RawImage>().texture);
        }
    }
}
