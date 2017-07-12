using System.Collections;
using System.Collections.Generic;
using BulletSharp;
using BulletSharp.Math;
using UnityEngine;
using CollisionFlags = BulletSharp.CollisionFlags;

namespace Bullet4Unity {
	/// <summary>
	/// Optimized RigidBody class for static objects
	/// -Author: VektorKnight
	/// </summary>
	[AddComponentMenu("BulletPhysics/PhysicsBodies/StaticBody")]
	public class BulletStaticBody : BulletPhysicsBody {

		//Required components for a Bullet StaticBody
		private RigidBodyConstructionInfo _constructionInfo;
		private RigidBody _staticBody;

		/// <summary>
		/// Initializes the Bullet StaticBody and attempts to register it with the physics world.
		/// This should generally only be called internally or from a physics world instance.
		/// </summary>
		public override void InitializePhysicsBody() {
			//Check if already initialized
			if (Initialized) return;

			//Make sure a BulletCollisionShape is attached and reference it if possible
			PhysicsCollisionShape = GetComponent<BulletCollisionShape>();
			if (PhysicsCollisionShape == null) {
				Debug.LogError("No Bullet collision shape is attached!\n" +
				               "Please attach a Bullet collision shape to this object");
				return;
			}

			//Initialize the Bullet transform matrix and set the values based on Unity transform
			InitialTransform = Matrix.AffineTransformation(1f, transform.rotation.ToBullet(), transform.position.ToBullet());

			//Initialize the Bullet default motion state using the transform matrix
			if (PhysicsCollisionShape.GetShapeType() == CollisionShapeType.StaticPlane) InitialTransform.Origin = transform.position.ToBullet() - transform.up.ToBullet();

			//Initialize the Bullet rigidbody construction info and assign the relevant values
			_constructionInfo = new RigidBodyConstructionInfo(0f, null, PhysicsCollisionShape.GetCollisionShape()) {StartWorldTransform = InitialTransform};

			//Create the Bullet RigidBody
			_staticBody = new RigidBody(_constructionInfo);

			//Register with the physics world
			BulletPhysicsWorldManager.Register(_staticBody);
			Registered = true;

			//Initialization complete
			Initialized = true;
		}
		
		/// <summary>
		/// Disposes of the RigidBody
		/// Should only be called internally or from a physics world instance.
		/// </summary>
		public override void Dispose() {
			//Dispose of all the components in reverse order
			if (Disposing) return;
			if (Registered) BulletPhysicsWorldManager.Unregister(_staticBody);
			
			Disposing = true;
			_staticBody.Dispose();
			_constructionInfo.Dispose();
			PhysicsCollisionShape.Dispose();
		}
		
		//Unity OnEnable
		protected override void OnEnable() {
			//Check if initialized and already registered
			//TODO: Physics world should probably implement a method to return registration status
			if (!Initialized) InitializePhysicsBody();
			if (Registered) return;
			
			//Register with the physics world
			BulletPhysicsWorldManager.Register(_staticBody);
			Registered = true;
		}
		
		//Unity OnDisable
		protected override void OnDisable() {
			//Check if registered
			//TODO: See OnEnable()
			if (!Registered || !Initialized) return;
			
			//Unregister from the physics world
			BulletPhysicsWorldManager.Unregister(_staticBody);
			Registered = false;
		}

		protected override void OnDestroy() {
			Dispose();
		}
	}
}
