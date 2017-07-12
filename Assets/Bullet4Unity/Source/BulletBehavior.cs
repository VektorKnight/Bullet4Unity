using BulletSharp;
using UnityEngine;

namespace Bullet4Unity {
	/// <summary>
	/// Abstract base class for MonoBehaviors which intend to use Bullet
	/// </summary>
	public abstract class BulletBehavior : MonoBehaviour {
		
		//Attempt to register with the physics world
		protected virtual void Awake() {
			BulletPhysicsWorldManager.Register(this);
		}

		/// <summary>
		/// Called once per Bullet time step
		/// </summary>
		/// <param name="world">The BulletPhysicsWorld from which the call originated</param>
		/// <param name="bulletTimeStep">The time (s) that the simulation has stepped</param>
		public abstract void BulletUpdate(DynamicsWorld world, float bulletTimeStep);
	}
}
