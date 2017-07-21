using System;
using BulletSharp.Math;
using UnityEngine;

namespace Bullet4Unity {
    /// <summary>
    /// Abstract base class for all Bullet Physics Bodies
    /// </summary>
    public abstract class BulletPhysicsBody : MonoBehaviour, IDisposable {

        //Inspector: World Assignment
        [Header("Physics World Assignment")]
        [SerializeField] protected string PhysicsWorld = "default";
        
        //Protected Members
        protected bool Initialized;
        protected bool Registered;
        protected bool Disposing;

        protected BulletCollisionShape PhysicsCollisionShape;
        protected Matrix InitialTransform;
        protected BulletMotionState PhysicsMotionState;

        public virtual string GetWorldName() {
            return string.IsNullOrEmpty(PhysicsWorld) ? "default" : PhysicsWorld;
        }

        public void RegisterEvent() {
            BulletWorldManager.OnInitializeObjects += Initialize;
        }

        private void Initialize(BulletWorldManager.BulletObjectTypes objectType) {
            if (objectType == BulletWorldManager.BulletObjectTypes.PhysicsBody) InitializePhysicsBody();
        }

        protected abstract void OnEnable();
        protected abstract void OnDisable();
        protected abstract void OnDestroy();

        public abstract void InitializePhysicsBody();

        public abstract void Dispose();
    }
}
