using BulletSharp;
using UnityEngine;

namespace Bullet4Unity {
	/// <summary>
	/// Abstract base class for MonoBehaviors which intend to use Bullet
	/// </summary>
	public abstract class BulletBehavior : MonoBehaviour {

		protected BulletRigidBody RigidBody;
		
		//Attempt to register with the physics world
		protected virtual void Awake() {
			BulletPhysicsWorldManager.Register(this);
			RigidBody = GetComponent<BulletRigidBody>();
			PersistentManifold.ContactProcessed += OnContactProcessed;
		}
		
		//Collision Added Callback
		private void OnContactProcessed(ManifoldPoint cp, CollisionObject body0, CollisionObject body1) {
			if (Equals(body0, RigidBody.BRigidBody) || Equals(body1, RigidBody.BRigidBody)) {
				OnContactAdded(body0);
			}
		}

		/// <summary>
		/// Called once per Bullet time step
		/// </summary>
		/// <param name="world">The BulletPhysicsWorld from which the call originated</param>
		/// <param name="bulletTimeStep">The time (s) that the simulation has stepped</param>
		public abstract void BulletUpdate(DynamicsWorld world, float bulletTimeStep);

		public abstract void OnContactAdded(CollisionObject other);
	}
}
