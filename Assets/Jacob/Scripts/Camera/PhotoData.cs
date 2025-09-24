using System.Collections.Generic;
using UnityEngine;

namespace PhotoCamera
{
    [System.Serializable]
    public class PhotoData
    {
        public byte[] pngData;
    }

    [System.Serializable]
    public class PhotoList
    {
        public List<PhotoData> photos = new List<PhotoData>();
    }
}
