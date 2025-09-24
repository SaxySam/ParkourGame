using UnityEngine;
using UnityEngine.UI;

namespace Phone
{
    public class ButtonController : MonoBehaviour
    {
        public void ButtonClick()
        {
            GameManager.galleryButtonPressedEvent(gameObject.GetComponent<RawImage>().texture);
        }
    }
}
