using System.Collections.Generic;
using UnityEngine;

namespace PhotoCamera
{
    [System.Serializable]
    
    [AddComponentMenu("Parkour Game/PhotoData")]
    public class PhotoData : MonoBehaviour
    {
        public byte[] pngData;
    }

    [System.Serializable]
    
    [AddComponentMenu("Parkour Game/PhotoList")]
    public class PhotoList : MonoBehaviour
    {
        public List<PhotoData> photos = new List<PhotoData>();
    }
}
