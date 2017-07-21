using BulletSharp;
using UnityEngine;

namespace Bullet4Unity {
	/// <summary>
	/// Abstract base class for MonoBehaviors which intend to use Bullet
	/// </summary>
	public abstract class BulletBehaviour : MonoBehaviour {

		protected BulletPhysicsBody PhysicsBody;
		
		public void RegisterEvent() {
			BulletWorldManager.OnInitializeObjects += Initialize;
		}

		private void Initialize(BulletWorldManager.BulletObjectTypes objectType) {
			if (objectType != BulletWorldManager.BulletObjectTypes.PhysicsBehaviour) return;
			PhysicsBody = GetComponent<BulletPhysicsBody>();

			if (PhysicsBody == null) {
				Debug.LogError("A BulletBehavior requires a Bullet Physics Body to be attached in order to function!\n" +
				               "Please attach a Bullet Physics Body or switch to a normal MonoBehavior");
				return;
			}
			
			BulletWorldManager.Register(PhysicsBody.GetWorldName(), this);
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
