using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using BulletSharp;

namespace Bullet4Unity {
    public static class BulletGizmos {
        public static void DrawConvexHull(ConvexHullShape shape, Mesh mesh, Transform transform, Color color, bool isDirty = false) {
            if (shape == null)
                return;

            if (isDirty || mesh == null)
                CreateHullGizmoMesh(shape, ref mesh);

            Gizmos.color = color;
            Gizmos.DrawWireMesh(mesh, transform.position, transform.rotation);
        }

        private static void CreateHullGizmoMesh(ConvexHullShape shape, ref Mesh mesh) {
            if (mesh == null) {
                mesh = new Mesh();
            }
            else {
                mesh.Clear();
            }

            Vector3[] vertexBuffer;
            int[] triangleBuffer;
            using (ShapeHull hull = new ShapeHull(shape)) {
                hull.BuildHull(0.0f);
                vertexBuffer = hull.Vertices
                    .Select(v => v.ToUnity())
                    .ToArray();

                triangleBuffer = hull.Indices
                    .Select(i => (int)i)
                    .ToArray();
            }

            MergeVertices(vertexBuffer, 0.05f);

            mesh.vertices = vertexBuffer;
            mesh.triangles = triangleBuffer;
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
        }

        private static void MergeVertices(Vector3[] vertexBuffer, float tolerance) {
            for (int i = 0; i < vertexBuffer.Length; i++) {
                Vector3 v = vertexBuffer[i];
                for (int j = i + 1; j < vertexBuffer.Length; j++) {
                    if (Vector3.Distance(v, vertexBuffer[j]) < tolerance) {
                        vertexBuffer[j] = v;
                    }
                }
            }
        }
    }
}