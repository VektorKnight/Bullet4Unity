using System;
using System.Collections;
using System.Collections.Generic;
using BulletSharp;
using Bullet4Unity;
using UnityEngine;

namespace Bullet4Unity {
    /// <summary>
    /// Interop class for a Bullet sphere collision shape
    /// -Author: VektorKnight
    /// </summary>
    [AddComponentMenu("BulletPhysics/Collision/SphereShape")]
    public class BulletSphereShape : BulletCollisionShape {
        
        //Unity Inspector
        [Header("Shape Config")] 
        [SerializeField] private float _radius = 0.5f;
        [SerializeField] private Vector3 _localScale = Vector3.one;
        
        #if UNITY_EDITOR
        //Draw Shape Gizmo
        protected override void OnDrawGizmosSelected() {
            if (!DrawGizmo) return;
            BUtility.DebugDrawSphere(transform.position, transform.rotation, _localScale, _radius * Vector3.one, GizmoColor);
        }
        #endif
        
        //Get Collision shape
        public override CollisionShape GetCollisionShape() {
            if (Shape != null) return Shape;
            Shape = new SphereShape(_radius) {LocalScaling = _localScale.ToBullet()};
            return Shape;
        }  
    }
}
