using System;
using System.Collections;
using System.Collections.Generic;
using BulletSharp;
using Bullet4Unity;
using UnityEngine;

namespace Bullet4Unity {
    /// <summary>
    /// Interop class for a Bullet box collision shape
    /// -Author: VektorKnight
    /// </summary>
    [AddComponentMenu("BulletPhysics/Collision/BoxShape")]
    public class BulletBoxShape : BulletCollisionShape {
        
        //Unity Inspector
        [Header("Shape Config")] 
        [SerializeField] private Vector3 _extents = new Vector3(0.5f, 0.5f, 0.5f);
        
        #if UNITY_EDITOR
        //Draw Shape Gizmo
        private static Mesh _shapeMesh;
        protected override void OnDrawGizmosSelected() {
            if (!DrawGizmo) return;
            if (_shapeMesh == null) _shapeMesh = UnityEditor.AssetDatabase.LoadAssetAtPath<Mesh>("Assets/Bullet4Unity/Resources/Primitives/BulletCube.fbx");
            Gizmos.color = GizmoColor;
            //Gizmos.DrawWireMesh(_shapeMesh, transform.position, transform.rotation, Vector3.Scale(_localScale, 2f * _extents));
            Gizmos.DrawWireMesh(_shapeMesh, transform.position, transform.rotation, Vector3.Scale(transform.localScale, 2f * _extents));
        }
        #endif
        
        //Get Collision shape
        public override CollisionShape GetCollisionShape() {
            if (Shape != null) return Shape;
            Shape = new BoxShape(_extents.ToBullet()) {LocalScaling = transform.localScale.ToBullet()};
            return Shape;
        }  
    }
}
