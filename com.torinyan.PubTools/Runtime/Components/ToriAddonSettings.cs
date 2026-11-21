using System;
using UnityEngine;
using VRC.SDK3.Avatars.ScriptableObjects;

namespace com.torinyan.PubTools.Runtime.Components
{
    //[AddComponentMenu("Torinyan/Torinyan Addon"), DisallowMultipleComponent]
    public class ToriAddonSettings : ToriComponent
    {
        [HideInInspector]
        public const string Version = "v0.0.1";

        public AddonAnimatorInfo[] Animators = Array.Empty<AddonAnimatorInfo>();

        public VRCExpressionParameters Parameters;

        public AddonMenuInfo[] Menus = Array.Empty<AddonMenuInfo>();

        public bool UpdateBounds = false;

        public Bounds NewBounds = new(Vector3.zero, new(2f, 2f, 2f));

        public bool UpdateAnchors = false;

        public string Anchor = string.Empty;

        public string AnchorOverride = string.Empty;

        public bool MergeArmature = false;

        public string MergeSource = string.Empty;

        public string MergeTarget = string.Empty;

        public bool MergeAsChild = false;
    }
}
