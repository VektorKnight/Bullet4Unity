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
        private List<Vector3> _hullVertices = new List<Vector3>();
        
        //Generate and return a Bullet Convex Hull Shape
        private void GenerateConvexHull() {
            //Build the optimized hull
            if (_hullMesh == null) _hullMesh = GetComponent<MeshFilter>().sharedMesh; 
            _meshVertices = _hullMesh.vertices.Select(vertex => vertex.ToBullet()).ToList();
             
            //Optimize the hull if optimization is enabled
            if (_optimizeHull) {
                Shape = new ConvexHullShape(_meshVertices, 128) {LocalScaling = transform.localScale.ToBullet()};
                ((ConvexHullShape)Shape).OptimizeConvexHull();
            }
            else {
                Shape = new ConvexHullShape(_meshVertices) {LocalScaling = transform.localScale.ToBullet()};
                ((ConvexHullShape)Shape).OptimizeConvexHull();
                if (((ConvexHullShape)Shape).NumPoints > 128) Debug.LogWarning("Convex hull contains more than 128 vertices!\n" +
                                                                "Please consider enabling optimization or specifying an optimized hull mesh");
            }
            
            //Fetch hull vertices for later use
            ((ConvexHullShape) Shape).InitializePolyhedralFeatures();
            for (var i = 0; i < ((ConvexHullShape) Shape).NumVertices; i++) {
                BVector3 vertex;
                ((ConvexHullShape) Shape).GetVertex(i, out vertex);
                _hullVertices.Add(vertex.ToUnity());
            }
            
            //Cleanup
            _meshVertices.Clear();
        }
        
        #if UNITY_EDITOR
        //Draw Shape Gizmo
        protected override void OnDrawGizmosSelected() {
            //TODO: Visualize the actual hull, a bounding box is used for now
            if (!DrawGizmo) return;
            if (Shape == null) return;
            Gizmos.color = GizmoColor;
        }
        #endif
        
        //Draw mesh using array of Vertices
        private void DrawTrianglesVertices(Vector3[] vertices) {
            if (vertices.Length == 0) return;
            for (var i = 0; i + 3 < vertices.Length; i += 3) {
                var a = vertices[i];
                var b = vertices[i + 1];
                var c = vertices[i + 2];
                Debug.DrawLine(transform.position + a, transform.position + b, GizmoColor);
                Debug.DrawLine(transform.position + b, transform.position + c, GizmoColor);
                Debug.DrawLine(transform.position + c, transform.position + a, GizmoColor);
            }
        }
        
        
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
