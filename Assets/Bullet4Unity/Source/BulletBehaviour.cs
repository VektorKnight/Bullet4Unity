using BulletSharp;
using UnityEngine;

namespace Bullet4Unity {
	/// <summary>
	/// Abstract base class for MonoBehaviors which intend to use Bullet
	/// </summary>
	public abstract class BulletBehaviour : MonoBehaviour {

		protected BulletRigidBody BRigidBody;
		
		//Attempt to register with the physics world
		protected virtual void Awake() {
			BulletPhysicsWorldManager.Register(this);
			BRigidBody = GetComponent<BulletRigidBody>();
			PersistentManifold.ContactProcessed += OnContactProcessed;
		}
		
		//Collision Added Callback
		private void OnContactProcessed(ManifoldPoint cp, CollisionObject body0, CollisionObject body1) {
			if (Equals(body0, BRigidBody.BRigidBody) || Equals(body1, BRigidBody.BRigidBody)) {
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
