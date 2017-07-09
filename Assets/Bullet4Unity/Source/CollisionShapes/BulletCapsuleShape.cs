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
        [SerializeField] private Vector3 _localScale = Vector3.one;
        
        //Draw Shape Gizmo
        protected override void OnDrawGizmosSelected() {
            if (!DrawGizmo) return;
            BUtility.DebugDrawCapsule(transform.position, transform.rotation, _localScale, _radius, _height / 2f, 1, GizmoColor);
        }
        
        //Get Collision shape
        public override CollisionShape GetCollisionShape() {
            if (Shape != null) return Shape;
            Shape = new CapsuleShape(_radius, _height) {LocalScaling = _localScale.ToBullet()};
            return Shape;
        }  
    }
}
