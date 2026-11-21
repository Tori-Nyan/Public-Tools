using UnityEngine;

namespace com.torinyan.PubTools.Runtime.Components
{
    //[AddComponentMenu("Torinyan/Set Menu Icon")]
    public class SetMenuIconSettings : ToriComponent
    {
        [HideInInspector]
        public const string Version = "v0.0.0";

        public Texture2D Icon;

        public string ItemPath = string.Empty;
    }
}
