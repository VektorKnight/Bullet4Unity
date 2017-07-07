using System;
using UnityEngine;
using System.Collections;
using BulletSharp;

namespace Bullet4Unity {
    /// <summary>
    /// Bullet Primitive Box Shape
    /// -VektorKnight
    /// </summary>
	[AddComponentMenu("BulletPhysics/Shapes/Box")]
    public class BulletBoxShape : BulletCollisionShape {
        
	    //Private Members: Extents, LocalScale
	    [SerializeField] private Vector3 _extents = new Vector3(0.5f, 0.5f, 0.5f);
        [SerializeField] private Vector3 _localScale = Vector3.one;
        
        //Public Properties
        public Vector3 Extents { get { return _extents; } }
        
        public Vector3 LocalScaling {
            get { return _localScale; }
            set {
                _localScale = value;
                if (CollisionShapePtr != null) {
                    ((BoxShape)CollisionShapePtr).LocalScaling = value.ToBullet();
                }
            }
        }             
        
        //Draw the shape in-editor for visualization
        public override void OnDrawGizmosSelected() {
            if (DrawGizmo == false) return;
        
            Vector3 position = transform.position;
            Quaternion rotation = transform.rotation;
            Vector3 scale = _localScale;
            B4UDebug.DebugDrawBox(position, rotation, scale, _extents, GizmoColor);
        }
        
        public override CollisionShape CopyCollisionShape() {
            var bs = new BoxShape(_extents.ToBullet()) {LocalScaling = _localScale.ToBullet()};
            return bs;
        }

        public override CollisionShape GetCollisionShape() {
            if (CollisionShapePtr != null) return CollisionShapePtr;
            CollisionShapePtr = new BoxShape(_extents.ToBullet());
            ((BoxShape)CollisionShapePtr).LocalScaling = _localScale.ToBullet();
            return CollisionShapePtr;
        }
    }
}
