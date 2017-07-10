using System;
using System.Collections;
using System.Collections.Generic;
using BulletSharp;
using Bullet4Unity;
using UnityEngine;

namespace Bullet4Unity {
    /// <summary>
    /// Interop class for a Bullet capsule collision shape
    /// -Author: VektorKnight
    /// </summary>
    [AddComponentMenu("BulletPhysics/Collision/CapsuleShape")]
    public class BulletCapsuleShape : BulletCollisionShape {
        
        //Unity Inspector
        [Header("Shape Config")] 
        [SerializeField] private float _radius = 0.5f;
        [SerializeField] private float _height = 1f;
        
        #if UNITY_EDITOR
        //Draw Shape Gizmo
        private static Mesh _shapeMesh;
        private Vector3 _gizmoScale;
        protected override void OnDrawGizmosSelected() {
            if (!DrawGizmo) return;
            if (_shapeMesh == null) _shapeMesh = UnityEditor.AssetDatabase.LoadAssetAtPath<Mesh>("Assets/Bullet4Unity/Resources/Primitives/BulletCapsule.fbx");
            Gizmos.color = GizmoColor;
            _gizmoScale.x = 2f *_radius;
            _gizmoScale.y = _height;
            _gizmoScale.z =  2f *_radius;
            Gizmos.DrawWireMesh(_shapeMesh, transform.position, transform.rotation, Vector3.Scale(transform.localScale, _gizmoScale));
        }
        #endif
        
        //Get Collision shape
        public override CollisionShape GetCollisionShape() {
            if (Shape != null) return Shape;
            Shape = new CapsuleShape(_radius, _height) {LocalScaling = transform.localScale.ToBullet()};
            return Shape;
        }  
    }
}
