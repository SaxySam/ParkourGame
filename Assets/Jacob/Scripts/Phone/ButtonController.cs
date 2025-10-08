using UnityEngine;
using UnityEngine.UI;

namespace Phone
{
    [AddComponentMenu("Parkour Game/ButtonController")]
    public class ButtonController : MonoBehaviour
    {
        public void ButtonClick()
        {
            GameManager.galleryButtonPressedEvent(gameObject.GetComponent<RawImage>().texture);
        }
    }
}
