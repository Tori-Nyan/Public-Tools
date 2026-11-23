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
        public string Prefix = string.Empty;
        public VRCExpressionsMenu Menu;
        public Texture2D Icon;
        public string Parameter = string.Empty;
        public float Value = 1f;
    }
}
