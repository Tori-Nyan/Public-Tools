/*
* Torinyan Public Tools
* Copyright (C) 2026  Torinyan
*
* This program is free software: you can redistribute it and/or modify
* it under the terms of the GNU General Public License as published by
* the Free Software Foundation, either version 3 of the License, or
* (at your option) any later version.
*
* This program is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
* GNU General Public License for more details.
*
* You should have received a copy of the GNU General Public License
* along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/
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

        public string RootBone = string.Empty;

        public string AnchorOverride = string.Empty;

        public bool MergeArmature = false;

        public string MergeSource = string.Empty;

        public string MergeTarget = string.Empty;

        public bool MergeAsChild = false;
    }
}
