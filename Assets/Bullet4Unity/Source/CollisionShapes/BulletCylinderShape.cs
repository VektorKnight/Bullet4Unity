﻿using BulletSharp;
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
        
        #if UNITY_EDITOR
        //Draw Shape Gizmo
        protected override void OnDrawGizmosSelected() {
            if (!DrawGizmo) return;
            BUtility.DebugDrawCylinder(transform.position, transform.rotation, transform.localScale, _halfExtents.x, _halfExtents.y, 1, GizmoColor);
        }
        #endif
        
        //Get Collision shape
        public override CollisionShape GetCollisionShape() {
            if (Shape != null) return Shape;
            Shape = new CylinderShape(_halfExtents.ToBullet()) {LocalScaling = transform.localScale.ToBullet()};
            return Shape;
        }  
        
        //Get Collision Shape Type
        public override CollisionShapeType GetShapeType() {
            return CollisionShapeType.Primitive;
        }
    }
}
