using System;
using System.Collections;
using System.Collections.Generic;
using BulletSharp;
using Bullet4Unity;
using UnityEngine;

namespace Bullet4Unity {
    /// <summary>
    /// Interop class for a Bullet cone collision shape
    /// -Author: VektorKnight
    /// </summary>
    [AddComponentMenu("BulletPhysics/Collision/ConeShape")]
    public class BulletConeShape : BulletCollisionShape {
        
        //Unity Inspector
        [Header("Shape Config")] 
        [SerializeField] private float _radius = 0.5f;
        [SerializeField] private float _height = 1f;
        
        #if UNITY_EDITOR
        //Draw Shape Gizmo
        protected override void OnDrawGizmosSelected() {
            if (!DrawGizmo) return;
            BUtility.DebugDrawCone(transform.position, transform.rotation, transform.localScale, _radius, _height, 1, GizmoColor);
        }
        #endif
        
        //Get Collision shape
        public override CollisionShape GetCollisionShape() {
            if (Shape != null) return Shape;
            Shape = new ConeShape(_radius, _height) {LocalScaling = transform.localScale.ToBullet()};
            return Shape;
        }
        
        //Get Collision Shape Type
        public override CollisionShapeType GetShapeType() {
            return CollisionShapeType.Primitive;
        }
    }
}
