using System;
using BulletSharp;
using BulletSharp.SoftBody;
using UnityEngine;

namespace Bullet4Unity {
    [RequireComponent(typeof(BulletWorldManager))]
    public abstract class BulletPhysicsWorld : MonoBehaviour, IDisposable {
        private const string UNSUPPORTED_WORLD_TYPE_MESSAGE = "Type not supported by the world! Make sure you register the object with the correct world type." +
                                                              "If you are extending this class do not call the base implementation as it is empty.";
        //Index of the physics world, used for determining default assignment
        protected static int Index;
        
        //Name Property of the world, referenced by the manager class
        public abstract string GetWorldName();
        
        // Initialize the physics world
        public abstract void Initialize();

        // Dispose of the physics world
        public abstract void Dispose();

        // Step the physics simulation
        public abstract void StepSimulation();

        // Register with the physics world
        public virtual void Register(BulletBehaviour behaviour) {
            throw new UnsupportedWorldTypeException(UNSUPPORTED_WORLD_TYPE_MESSAGE);
        }

        // Unregister with the physics world
        public virtual void Unregister(BulletBehaviour behaviour) {
            throw new UnsupportedWorldTypeException(UNSUPPORTED_WORLD_TYPE_MESSAGE);
        }

        // Register with the physics world
        public virtual void Register(RigidBody rigidBody) {
            throw new UnsupportedWorldTypeException(UNSUPPORTED_WORLD_TYPE_MESSAGE);
        }

        // Unregister with the physics world
        public virtual void Unregister(RigidBody rigidBody) {
            throw new UnsupportedWorldTypeException(UNSUPPORTED_WORLD_TYPE_MESSAGE);
        }

        // Register with the physics world
        public virtual void Register(SoftBody softbody) {
            throw new UnsupportedWorldTypeException(UNSUPPORTED_WORLD_TYPE_MESSAGE);
        }

        // Unregister with the physics world
        public virtual void Unregister(SoftBody softbody) {
            throw new UnsupportedWorldTypeException(UNSUPPORTED_WORLD_TYPE_MESSAGE);
        }
        
        // Register a constraint with the physics world
        public virtual void Register(TypedConstraint constraint) {
            throw new UnsupportedWorldTypeException(UNSUPPORTED_WORLD_TYPE_MESSAGE);
        }
        //Unregister a constraint with the physics world
        public virtual void Unregister(TypedConstraint constraint) {
            throw new UnsupportedWorldTypeException(UNSUPPORTED_WORLD_TYPE_MESSAGE);
        }
    }
}
