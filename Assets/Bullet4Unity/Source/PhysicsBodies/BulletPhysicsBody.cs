using System;
using BulletSharp;
using BulletSharp.Math;
using UnityEngine;

namespace Bullet4Unity {
    /// <summary>
    /// Abstract base class for all Bullet Physics Bodies
    /// </summary>
    public abstract class BulletPhysicsBody : MonoBehaviour, IDisposable {

        //protected Members
        protected bool Initialized;
        protected bool Registered;
        protected bool Disposing;

        protected BulletCollisionShape PhysicsCollisionShape;
        protected Matrix InitialTransform;
        protected BulletMotionState PhysicsMotionState;

        protected abstract void OnEnable();
        protected abstract void OnDisable();
        protected abstract void OnDestroy();

        public abstract void InitializePhysicsBody();

        public abstract void Dispose();
    }
}
