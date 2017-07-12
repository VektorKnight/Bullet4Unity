using System.Collections.Generic;
using System.Linq;
using BulletSharp;
using UnityEngine;

namespace Bullet4Unity {
    /// <summary>
    /// Interop class for a Bullet box collision shape
    /// -Author: VektorKnight
    /// </summary>
    [AddComponentMenu("BulletPhysics/Collision/ConvexHullShape")]
    [RequireComponent(typeof(MeshFilter))]
    public class BulletConvexHullShape : BulletCollisionShape {
        
        //Private Internal (Optimized Hull Generation)
        private Mesh _mesh;
        private List<BulletSharp.Math.Vector3> _vertices = new List<BulletSharp.Math.Vector3>();
        private ConvexHullShape _rawHull;
        private ShapeHull _optimizer;

        //Generate and return an optimized Bullet ConvexHullShape
        private void GenerateOptimizedConvexHull() {
            //Build the optimized hull
            _mesh = GetComponent<MeshFilter>().sharedMesh;
            _vertices = _mesh.vertices.Select(vertex => vertex.ToBullet()).ToList();
            _rawHull = new ConvexHullShape(_vertices);
            _optimizer = new ShapeHull(_rawHull);
            _optimizer.BuildHull(_rawHull.Margin);
            Shape = new ConvexHullShape(_optimizer.Vertices) {LocalScaling = transform.localScale.ToBullet()};
            
            //Cleanup BulletSharp components and placeholders
            _optimizer.Dispose();
            _rawHull.Dispose();
            _optimizer = null;
            _rawHull = null;
            _vertices.Clear();
        }
        
        #if UNITY_EDITOR
        //Draw Shape Gizmo
        protected override void OnDrawGizmosSelected() {
            //TODO: Visualize the actual hull, a bounding box is used for now
            if (!DrawGizmo) return;
            if (Shape ==null) GenerateOptimizedConvexHull();
            Gizmos.color = GizmoColor;
            Gizmos.DrawWireCube(transform.position, Vector3.Scale(2f * _mesh.bounds.extents, transform.localScale));
        }
        #endif
        
        //Get Collision shape
        public override CollisionShape GetCollisionShape() {
            if (Shape != null) return Shape;
            GenerateOptimizedConvexHull();
            return Shape;
        } 
        
        //Get Collision Shape Type
        public override CollisionShapeType GetShapeType() {
            return CollisionShapeType.ConvexHull;
        }
    }
}
