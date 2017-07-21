using System;
using System.Security.Policy;
using BulletSharp;
using BulletSharp.Math;
using UnityEngine;
using UnityEngine.Networking;
using CollisionFlags = BulletSharp.CollisionFlags;
using Vector3 = UnityEngine.Vector3;

namespace Bullet4Unity {
	/// <summary>
	/// Interop class for a Bullet RigidBody
	/// -Author: vektorKnight
	/// </summary>
	[AddComponentMenu("BulletPhysics/PhysicsBodies/RigidBody")]
	[DisallowMultipleComponent]
	public class BulletRigidBody : BulletPhysicsBody {
		
		#region Unity Inspector
		//Unity Inspector
		[Header("Basic Settings")]
		[SerializeField] private float _mass = 1f;
		[SerializeField] private float _linearDamping = 0.05f;
		[SerializeField] private float _angularDamping = 0.05f;
		[SerializeField] [Range(0f,1f)] private float _restitution = 0.1f;
		
		[Header("Friction Settings")]
		[SerializeField] private float _friction = 0.5f;
		[SerializeField] private float _rollingFriction = 0.2f;

		[Header("World Factors")] 
		[SerializeField] private Vector3 _linearFactor = Vector3.one;
		[SerializeField] private Vector3 _angularFactor = Vector3.one;

		[Header("Sleep Settings")] 
		[SerializeField] private bool _allowSleeping = true;
		[SerializeField] private float _linearSleepThreshold = 0.01f;
		[SerializeField] private float _angularSleepThreshold = 0.01f;
		#endregion
		
		#region Private Members
		//Internal Private
		private bool _isKinematic;
		private Matrix _currentTransform;
		private BulletSharp.Math.Vector3 _localInternia;
		
		//Required components for a Bullet RigidBody
		private RigidBodyConstructionInfo _constructionInfo;

		#endregion

		#region Public Properties
		//Get Bullet RigidBody Instance
		public RigidBody RigidBody { get; private set; }

		//Get MotionState Instance
		public MotionState MotionState => PhysicsMotionState;
		
		//Get Collision Shape
		public CollisionShape CollisionShape => PhysicsCollisionShape.GetCollisionShape();

		//Property: Mass
		public float Mass {
			get { return _mass; }
			set {
				_mass = value;
				PhysicsCollisionShape.GetCollisionShape().CalculateLocalInertia(_mass, out _localInternia);
				RigidBody.SetMassProps(_mass, _localInternia);
			}
		}
		
		//Property: Linear Damping
		public float LinearDamping {
			get { return _linearDamping; }
			set {
				_linearDamping = value;
				RigidBody.SetDamping(_linearDamping, _angularDamping);
			}
		}
		
		//Property: Angular Damping
		public float AngularDamping {
			get { return _angularDamping; }
			set {
				_angularDamping = value;
				RigidBody.SetDamping(_linearDamping, _angularDamping);
			}
		}
		
		//Property: Restitution
		public float Restitution {
			get { return _restitution; }
			set {
				_restitution = value;
				RigidBody.Restitution = value;
			}
		}
		
		//Property: Friction
		public float Friction {
			get { return _friction; }
			set {
				_friction = value;
				RigidBody.Friction = _friction;
			}
		}
		
		//Property: Rolling Friction
		public float RollingFriction {
			get { return _rollingFriction; }
			set {
				_rollingFriction = value;
				RigidBody.RollingFriction = value;
			}
		}
		
		//Property: Linear Factor
		public Vector3 LinearFactor {
			get { return _linearFactor; }
			set {
				_linearFactor = value;
				RigidBody.LinearFactor = value.ToBullet();
			}
		}
		
		//Property: Angular Factor
		public Vector3 AngularFactor {
			get { return _linearFactor; }
			set {
				_angularFactor = value;
				RigidBody.AngularFactor = value.ToBullet();
			}
		}
		
		//Property: Allow Sleeping
		public bool AllowSleeping {
			get { return _allowSleeping; }
			set {
				_allowSleeping = value;
				if (_allowSleeping) RigidBody.ActivationState -= ActivationState.DisableDeactivation;
				else RigidBody.ActivationState = ActivationState.DisableDeactivation;
			}
		}
		
		//Property: Linear Sleep Threshold
		public float LinearSleepThreshold => _linearSleepThreshold;
		
		//Property: Angular Sleep Threshold
		public float AngularSleepThreshold => _angularSleepThreshold;
		
		//Property: Linear Velocity
		public Vector3 LinearVelocity {
			get { return RigidBody.LinearVelocity.ToUnity(); }
			set { RigidBody.LinearVelocity = value.ToBullet(); }
		}
		
		//Property: Angular Velocity
		public Vector3 AngularVelocity {
			get { return RigidBody.AngularVelocity.ToUnity(); }
			set { RigidBody.AngularVelocity = value.ToBullet(); }
		}
		#endregion

		#region Public Utility Methods
		/// <summary>
		/// Initializes the Bullet RigidBody and attempts to register it with the physics world.
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
			
			//Calculate the local intertial of the given shape
			PhysicsCollisionShape.GetCollisionShape().CalculateLocalInertia(_mass, out _localInternia);
			
			//Initialize the Bullet transform matrix and set the values based on Unity transform
			InitialTransform = Matrix.AffineTransformation(1f, transform.rotation.ToBullet(), transform.position.ToBullet());

			//Initialize the Bullet default motion state using the transform matrix
			PhysicsMotionState = new BulletMotionState(transform);
			
			//Initialize the Bullet rigidbody construction info and assign the relevant values
			_constructionInfo = new RigidBodyConstructionInfo(_mass, PhysicsMotionState, PhysicsCollisionShape.GetCollisionShape()) {
				LocalInertia = _localInternia,
				LinearDamping = _linearDamping,
				AngularDamping = _angularDamping,
				Friction = _friction,
				RollingFriction = _rollingFriction,
				Restitution = _restitution,
				LinearSleepingThreshold = _linearSleepThreshold,
				AngularSleepingThreshold = _angularSleepThreshold
			};
			
			//Create the Bullet RigidBody
			RigidBody = new RigidBody(_constructionInfo) {
				LinearFactor = _linearFactor.ToBullet(),
				AngularFactor = _angularFactor.ToBullet(),
				CollisionFlags = CollisionFlags.CustomMaterialCallback
			};
			//Set sleeping flag
			if (!_allowSleeping) RigidBody.ActivationState = ActivationState.DisableDeactivation;
			//_rigidBody.CcdMotionThreshold = 0.5f;
			//_rigidBody.CcdSweptSphereRadius = 0.25f;
			
			//Register with the physics world
			BulletWorldManager.Register(GetWorldName(), RigidBody);
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
			if (Registered) BulletWorldManager.Unregister(GetWorldName(), RigidBody);
			
			Disposing = true;
			RigidBody?.Dispose();
			_constructionInfo?.Dispose();
			PhysicsMotionState?.Dispose();
			PhysicsCollisionShape?.Dispose();
		}
		#endregion
		
		#region Forces & Torque
		/// <summary>
		/// Applies a continuous force to the RigidBody
		/// Should be called from BulletUpdate()
		/// <param name="force">The continuous force to be applied</param>>
		/// </summary>
		public void ApplyForce(Vector3 force) {
			RigidBody?.ApplyCentralForce(force.ToBullet());
		}
		
		/// <summary>
		/// Applies a continuous force to the RigidBody at a specified position
		/// Should be called from BulletUpdate()
		/// <param name="force">The continuous force to be applied</param>>
		/// <param name="position">Local position at which to apply the force</param>>
		/// </summary>
		public void ApplyForce(Vector3 force, Vector3 position) {
			RigidBody?.ApplyForce(force.ToBullet(), position.ToBullet());
		}
		
		/// <summary>
		/// Applies an instant force impulse to the RigidBody
		/// Should be called from BulletUpdate()
		/// </summary>
		/// <param name="impulse"></param>
		public void ApplyImpulse(Vector3 impulse) {
			RigidBody?.ApplyCentralImpulse(impulse.ToBullet());
		}

		/// <summary>
		/// Applies an instant force impulse to the RigidBody at a specified position
		/// Should be called from BulletUpdate()
		/// </summary>
		/// <param name="impulse">The force impulse to be applied</param>
		/// <param name="position">Local position at which to apply the force</param>
		public void ApplyImpulse(Vector3 impulse, Vector3 position) {
			RigidBody?.ApplyImpulse(impulse.ToBullet(), position.ToBullet());
		}
		
		/// <summary>
		/// Applies a torque force to the RigidBody
		/// Should be called from BulletUpdate()
		/// </summary>
		/// <param name="torque">The torque force to be applied</param>
		public void ApplyTorque(Vector3 torque) {
			RigidBody?.ApplyTorque(torque.ToBullet());
		}
		
		/// <summary>
		/// Applies a torque impulse to the RigidBody at a specified position
		/// Should be called from BulletUpdate()
		/// </summary>
		/// <param name="torque">The torque impulse to be applied</param>
		public void ApplyTorqueImpulse(Vector3 impulse) {
			RigidBody?.ApplyTorqueImpulse(impulse.ToBullet());
		}
		#endregion
		
		#region Private Methods
        //Unity OnEnable
        protected override void OnEnable() {
			//Check if initialized and already registered
			//TODO: Physics world should probably implement a method to return registration status
			if (!Initialized) InitializePhysicsBody();
	        if (Registered) return;
			
			//Register with the physics world
	        if (!Initialized) return;
			BulletWorldManager.Register(GetWorldName(), RigidBody);
			Registered = true;
		}
		
		//Unity OnDisable
		protected override void OnDisable() {
			//Check if registered
			//TODO: See OnEnable()
			if (!Registered || !Initialized) return;
			
			//Unregister from the physics world
			if (!Initialized) return;
			BulletWorldManager.Unregister(GetWorldName(), RigidBody);
			Registered = false;
		}

		//Unity Destroy
		protected override void OnDestroy() {
			Dispose();
		}
		#endregion
	}
}
