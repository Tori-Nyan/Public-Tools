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
using UnityEngine;

namespace com.torinyan.PubTools.Editor.Builders
{
    internal class MeshBuilder
    {
        public void SetAnchor(SkinnedMeshRenderer[] skinnedMeshes, MeshRenderer[] staticMeshes, string rootBone, string anchorOverride) {
            if ((skinnedMeshes.Length + staticMeshes.Length <= 0) ||
                (string.IsNullOrWhiteSpace(rootBone) && string.IsNullOrWhiteSpace(anchorOverride))
            )
                return;

            Transform newRoot = StateHolder.Avatar.transform.Find(rootBone);
            Transform newAnchor = StateHolder.Avatar.transform.Find(anchorOverride);

            foreach (var mesh in skinnedMeshes) {
                if (null != newRoot)
                    mesh.rootBone = newRoot;

                if (null != newAnchor)
                    mesh.probeAnchor = newAnchor;

                ToriUtils.MarkDirty(mesh);
            }

            if (null != newAnchor) {
                foreach (var mesh in staticMeshes) {
                    mesh.probeAnchor = newAnchor;
                    ToriUtils.MarkDirty(mesh);
                }
            }
        }

        public void SetBounds(SkinnedMeshRenderer[] skinnedMeshes, Bounds bounds) {
            if (skinnedMeshes.Length <= 0)
                return;

            foreach (var mesh in skinnedMeshes) {
                mesh.localBounds = bounds;
                ToriUtils.MarkDirty(mesh);
            }
        }
    }
}
#endif
