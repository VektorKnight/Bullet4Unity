using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using BulletSharp;
using Bullet4Unity;
using UnityEngine;

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
        [SerializeField] private Vector3 _localScale = Vector3.one;
        
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
            Shape = new ConvexHullShape(_optimizer.Vertices) {LocalScaling = _localScale.ToBullet()};
            
            //Cleanup BulletSharp components
            _optimizer.Dispose();
            _rawHull.Dispose();
            _optimizer = null;
            _rawHull = null;
        }
        
        #if UNITY_EDITOR
        //Draw Shape Gizmo
        protected override void OnDrawGizmosSelected() {
            //TODO: Need to figure out a way to visualize the convex hull
        }
        #endif
        
        //Get Collision shape
        public override CollisionShape GetCollisionShape() {
            if (Shape != null) return Shape;
            GenerateOptimizedConvexHull();
            return Shape;
        }  
    }
}
