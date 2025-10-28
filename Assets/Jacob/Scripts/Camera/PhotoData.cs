using System.Collections.Generic;
using UnityEngine;

namespace PhotoCamera
{
    [System.Serializable]
    
    [AddComponentMenu("Parkour Game/PhotoData")]
    public class PhotoData
    {
        public byte[] pngData;
    }

    [System.Serializable]
    
    [AddComponentMenu("Parkour Game/PhotoList")]
    public class PhotoList
    {
        public List<PhotoData> photos = new List<PhotoData>();
    }
}
