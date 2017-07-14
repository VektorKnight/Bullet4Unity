using System;
using BulletSharp;
using BulletSharp.SoftBody;

namespace Bullet4Unity {
    public abstract class BulletPhysicsWorld : IDisposable {
        private const string UNSUPPORTED_WORLD_TYPE_MESSAGE = "Type not supported by world! Make sure you register the body with the correct world type. If you are extending this class do not call the base implementation as it is empty.";

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
        public virtual void Register(RigidBody rigidbody) {
            throw new UnsupportedWorldTypeException(UNSUPPORTED_WORLD_TYPE_MESSAGE);
        }

        // Unregister with the physics world
        public virtual void Unregister(RigidBody rigidbody) {
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
        
        // Register/Unregister constraint with the physics world
        public abstract void RegisterConstraint(TypedConstraint constraint);

        public abstract void UnregisterConstraint(TypedConstraint constraint);

    }
}
