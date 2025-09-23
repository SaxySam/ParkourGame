using UnityEngine;

public class GameManager : MonoBehaviour
{
    public delegate void OnGalleryOpen();
    public delegate void OnGalleryClose();

    public delegate void OnGalleryPhotoClick(Texture texture);
}
