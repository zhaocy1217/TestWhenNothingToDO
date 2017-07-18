namespace com.tencent.pandora
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class ImageMgr : MonoBehaviour
    {
        private Dictionary<string, Texture2D> dictPictures = new Dictionary<string, Texture2D>();

        public void AddTexture(string url, Texture2D texture)
        {
            Logger.DEBUG(string.Empty);
            this.dictPictures[url] = texture;
        }

        public Texture2D GetTexture(string url)
        {
            Logger.DEBUG(string.Empty);
            if (this.dictPictures.ContainsKey(url))
            {
                Logger.DEBUG(string.Empty);
                return this.dictPictures[url];
            }
            return null;
        }

        private void OnDestroy()
        {
            Logger.DEBUG(string.Empty);
            this.dictPictures.Clear();
        }

        private void Start()
        {
        }

        private void Update()
        {
        }
    }
}

