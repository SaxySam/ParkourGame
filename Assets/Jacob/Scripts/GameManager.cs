using UnityEngine;

[AddComponentMenu("Parkour Game/GameManager")]
public class GameManager : MonoBehaviour
{
    public delegate void OnPhoneOpen();
    public delegate void OnGalleryOpen();
    public delegate void OnGalleryClose();
    public delegate void OnGalleryButtonPressed(Texture texture);

    public delegate void OnPlayerLanded();
    public delegate void OnSlideStop();

    public static OnPhoneOpen PhoneOpenEvent;
    public static OnGalleryOpen GalleryOpenEvent;
    public static OnGalleryClose GalleryCloseEvent;
    public static OnGalleryButtonPressed GalleryButtonPressedEvent;
    
    public static OnPlayerLanded PlayerLandedEvent;
    public static OnSlideStop SlideStopEvent;

    void Start()
    {
        uint bankID;
        AkUnitySoundEngine.LoadBank("PlayerSound", out bankID);
        AkUnitySoundEngine.LoadBank("Ambience", out bankID);
    }
}
