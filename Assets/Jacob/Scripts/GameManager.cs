using UnityEngine;
using UnityEngine.Events;

[AddComponentMenu("Parkour Game/GameManager")]
public class GameManager : MonoBehaviour
{
    public delegate void OnPhoneOpen();
    public delegate void OnGalleryOpen();
    public delegate void OnGalleryClose();
    public delegate void OnGalleryButtonPressed(Texture texture);

    public static OnPhoneOpen phoneOpenEvent;
    public static OnGalleryOpen galleryOpenEvent;
    public static OnGalleryClose galleryCloseEvent;
    public static OnGalleryButtonPressed galleryButtonPressedEvent;
}
