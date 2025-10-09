using UnityEngine;
using UnityEngine.UI;

namespace Phone
{
    [AddComponentMenu("Parkour Game/ButtonController")]
    public class ButtonController : MonoBehaviour
    {
        public void ButtonClick()
        {
            GameManager.GalleryButtonPressedEvent(gameObject.GetComponent<RawImage>().texture);
        }
    }
}
