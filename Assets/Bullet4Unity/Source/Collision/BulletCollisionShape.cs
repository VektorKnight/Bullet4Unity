using System;
using System.Collections;
using UnityEngine;
using BulletSharp.Math;
using BulletSharp;

namespace Bullet4Unity {
    /// <summary>
    /// Abstract parent class for all Bullet Collision Shapes
    /// </summary>
    [System.Serializable]
    public abstract class BulletCollisionShape : MonoBehaviour, IDisposable {
        //Enable/Disable Collider Gizmo
        [SerializeField] protected bool DrawGizmo = true;
        
        //Default Gizmo Color
        [SerializeField] protected Color GizmoColor = Color.blue;
        
        //Collision Shape Types
        public enum CollisionShapeType {
            //Dynamic
            Box = 0,
            Sphere = 1,
            Capsule = 2,
            Cylinder = 3,
            Cone = 4,
            Convex = 5,
            Compound = 6,

            //Static Only
            BvhTriangleMesh = 7,
            StaticPlane = 8,
        };
        
        //Collision Shape Pointer
        protected CollisionShape CollisionShapePtr = null;
        
        //Unity Destroy Method
        private void OnDestroy() {
            Dispose(false);
        }
        
        //IDisposable Implementation
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        //Virtual Dispose Method
        protected virtual void Dispose(bool isdisposing) {
            if (CollisionShapePtr == null) return;
            CollisionShapePtr.Dispose();
            CollisionShapePtr = null;
        }
        
        //Abstract Methods
        public abstract void OnDrawGizmosSelected();
        public abstract CollisionShape CopyCollisionShape();
        public abstract CollisionShape GetCollisionShape();
    }
}


