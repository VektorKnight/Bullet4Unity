using System;
using System.Collections;
using System.Collections.Generic;
using BulletSharp;
using Bullet4Unity;
using UnityEngine;

namespace Bullet4Unity {
    /// <summary>
    /// Interop class for a Bullet cylinder collision shape
    /// -Author: VektorKnight
    /// </summary>
    [AddComponentMenu("BulletPhysics/Collision/CylinderShape")]
    public class BulletCylinderShape : BulletCollisionShape {
        
        //Unity Inspector
        [Header("Shape Config")] 
        [SerializeField] private Vector3 _halfExtents = new Vector3(0.5f, 1f, 0.5f);
        [SerializeField] private Vector3 _localScale = Vector3.one;
        
        #if UNITY_EDITOR
        //Draw Shape Gizmo
        protected override void OnDrawGizmosSelected() {
            if (!DrawGizmo) return;
            BUtility.DebugDrawCylinder(transform.position, transform.rotation, _localScale, _halfExtents.x, _halfExtents.y, 1, GizmoColor);
        }
        #endif
        
        //Get Collision shape
        public override CollisionShape GetCollisionShape() {
            if (Shape != null) return Shape;
            Shape = new CylinderShape(_halfExtents.ToBullet()) {LocalScaling = _localScale.ToBullet()};
            return Shape;
        }  
    }
}
