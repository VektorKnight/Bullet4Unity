using System.Collections.Generic;
using System.Linq;
using BulletSharp;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;
using BVector3 = BulletSharp.Math.Vector3;

namespace Bullet4Unity {
    /// <summary>
    /// Interop class for a Bullet box collision shape
    /// -Author: VektorKnight
    /// </summary>
    [AddComponentMenu("BulletPhysics/Collision/ConvexHullShape")]
    [RequireComponent(typeof(MeshFilter))]
    public class BulletConvexHullShape : BulletCollisionShape {

        //Unity Inspector
        [Header("Shape Config")]
        [Tooltip("The mesh to use for convex hull generation")]
        [SerializeField] private Mesh _hullMesh;
        [Tooltip("Enable this to have Bullet automatically remove unnecessary geometry")]
        [SerializeField] private bool _optimizeHull = true;

        //Private Internal (Hull Generation)
        private List<BVector3> _meshVertices = new List<BVector3>();

        //Generate and return a Bullet Convex Hull Shape
        private void GenerateConvexHull() {
            //Build the optimized hull
            if (_hullMesh == null)
                _hullMesh = GetComponent<MeshFilter>().sharedMesh;

            _meshVertices = _hullMesh.vertices.Select(vertex => vertex.ToBullet()).ToList();

            //Optimize the hull if optimization is enabled
            if (_optimizeHull) {
                Shape = new ConvexHullShape(_meshVertices, 128) { LocalScaling = transform.localScale.ToBullet() };
                ((ConvexHullShape)Shape).OptimizeConvexHull();
            }
            else {
                Shape = new ConvexHullShape(_meshVertices) { LocalScaling = transform.localScale.ToBullet() };
                ((ConvexHullShape)Shape).OptimizeConvexHull();
                if (((ConvexHullShape)Shape).NumPoints > 128) Debug.LogWarning("Convex hull contains more than 128 vertices!\n" +
                                                                "Please consider enabling optimization or specifying an optimized hull mesh");
            }
            
            //Cleanup
            _meshVertices.Clear();
        }

        #if UNITY_EDITOR
        private Mesh _gizmoMesh = null;

        //Draw Shape Gizmo
        protected override void OnDrawGizmosSelected() {
            if (!DrawGizmo)
                return;

            if (Shape == null && !Application.isPlaying)
                GenerateConvexHull();

            BulletGizmos.DrawConvexHull(Shape as ConvexHullShape, _gizmoMesh, transform, GizmoColor);
        }
        #endif

        //Get Collision shape
        public override CollisionShape GetCollisionShape() {
            if (Shape != null) return Shape;
            GenerateConvexHull();
            return Shape;
        }

        //Get Collision Shape Type
        public override CollisionShapeType GetShapeType() {
            return CollisionShapeType.ConvexHull;
        }
    }
}
