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
#if UNITY_EDITOR
using com.torinyan.PubTools.Runtime.Components;
using UnityEngine;

namespace com.torinyan.PubTools.Editor.Features
{
    internal static class SetBounds
    {
        public static void Process(SetBoundsSettings settings) {
            SkinnedMeshRenderer[] skinnedMeshes = settings.GetComponentsInChildren<SkinnedMeshRenderer>();
            StateHolder.Meshes.SetBounds(skinnedMeshes, settings.NewBounds);
        }
    }
}
#endif
