using System;
using UnityEngine;
using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Avatars.ScriptableObjects;

namespace com.torinyan.PubTools.Runtime
{
    [Serializable]
    public class AddonAnimatorInfo
    {
        public RuntimeAnimatorController Controller;
        public VRCAvatarDescriptor.AnimLayerType Type = VRCAvatarDescriptor.AnimLayerType.FX;
    }

    [Serializable]
    public class AddonMenuInfo
    {
        public VRCExpressionsMenu Menu;
        public string Prefix;
    }
}
