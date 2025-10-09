using UnityEngine;

[AddComponentMenu("Parkour Game/GameManager")]
public class GameManager : MonoBehaviour
{
    public delegate void OnPhoneOpen();
    public delegate void OnGalleryOpen();
    public delegate void OnGalleryClose();
    public delegate void OnGalleryButtonPressed(Texture texture);

    public static OnPhoneOpen PhoneOpenEvent;
    public static OnGalleryOpen GalleryOpenEvent;
    public static OnGalleryClose GalleryCloseEvent;
    public static OnGalleryButtonPressed GalleryButtonPressedEvent;
}
