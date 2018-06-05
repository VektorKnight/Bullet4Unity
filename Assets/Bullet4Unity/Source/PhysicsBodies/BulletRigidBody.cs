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
	public sealed class BulletRigidBody : BulletPhysicsBody, IDisposable {
		
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
		public RigidBody BodyInstance { get; private set; }

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
				BodyInstance.SetMassProps(_mass, _localInternia);
			}
		}
		
		//Property: Linear Damping
		public float LinearDamping {
			get { return _linearDamping; }
			set {
				_linearDamping = value;
				BodyInstance.SetDamping(_linearDamping, _angularDamping);
			}
		}
		
		//Property: Angular Damping
		public float AngularDamping {
			get { return _angularDamping; }
			set {
				_angularDamping = value;
				BodyInstance.SetDamping(_linearDamping, _angularDamping);
			}
		}
		
		//Property: Restitution
		public float Restitution {
			get { return _restitution; }
			set {
				_restitution = value;
				BodyInstance.Restitution = value;
			}
		}
		
		//Property: Friction
		public float Friction {
			get { return _friction; }
			set {
				_friction = value;
				BodyInstance.Friction = _friction;
			}
		}
		
		//Property: Rolling Friction
		public float RollingFriction {
			get { return _rollingFriction; }
			set {
				_rollingFriction = value;
				BodyInstance.RollingFriction = value;
			}
		}
		
		//Property: Linear Factor
		public Vector3 LinearFactor {
			get { return _linearFactor; }
			set {
				_linearFactor = value;
				BodyInstance.LinearFactor = value.ToBullet();
			}
		}
		
		//Property: Angular Factor
		public Vector3 AngularFactor {
			get { return _linearFactor; }
			set {
				_angularFactor = value;
				BodyInstance.AngularFactor = value.ToBullet();
			}
		}
		
		//Property: Allow Sleeping
		public bool AllowSleeping {
			get { return _allowSleeping; }
			set {
				_allowSleeping = value;
				if (_allowSleeping) BodyInstance.ActivationState -= ActivationState.DisableDeactivation;
				else BodyInstance.ActivationState = ActivationState.DisableDeactivation;
			}
		}
		
		//Property: Linear Sleep Threshold
		public float LinearSleepThreshold => _linearSleepThreshold;
		
		//Property: Angular Sleep Threshold
		public float AngularSleepThreshold => _angularSleepThreshold;
		
		//Property: Linear Velocity
		public Vector3 LinearVelocity {
			get { return BodyInstance.LinearVelocity.ToUnity(); }
			set { BodyInstance.LinearVelocity = value.ToBullet(); }
		}
		
		//Property: Angular Velocity
		public Vector3 AngularVelocity {
			get { return BodyInstance.AngularVelocity.ToUnity(); }
			set { BodyInstance.AngularVelocity = value.ToBullet(); }
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
			BodyInstance = new RigidBody(_constructionInfo) {
				LinearFactor = _linearFactor.ToBullet(),
				AngularFactor = _angularFactor.ToBullet()
			};
			BodyInstance.CollisionFlags = CollisionFlags.CustomMaterialCallback;
			//Set sleeping flag
			if (!_allowSleeping) BodyInstance.ActivationState = ActivationState.DisableDeactivation;
			//_rigidBody.CcdMotionThreshold = 0.5f;
			//_rigidBody.CcdSweptSphereRadius = 0.25f;
			
			//Register with the physics world
			BulletPhysicsWorldManager.Register(BodyInstance);
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
			if (Registered) BulletPhysicsWorldManager.Unregister(BodyInstance);
			
			Disposing = true;
			BodyInstance?.Dispose();
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
			if (!Initialized || Disposing || !Registered) return;
			BodyInstance.ApplyCentralForce(force.ToBullet());
		}
		
		/// <summary>
		/// Applies a continuous force to the RigidBody at a specified position
		/// Should be called from BulletUpdate()
		/// <param name="force">The continuous force to be applied</param>>
		/// <param name="position">Local position at which to apply the force</param>>
		/// </summary>
		public void ApplyForce(Vector3 force, Vector3 position) {
			if (!Initialized || Disposing || !Registered) return;
			BodyInstance.ApplyForce(force.ToBullet(), position.ToBullet());
		}
		
		/// <summary>
		/// Applies an instant force impulse to the RigidBody
		/// Should be called from BulletUpdate()
		/// </summary>
		/// <param name="impulse"></param>
		public void ApplyImpulse(Vector3 impulse) {
			if (!Initialized || Disposing || !Registered) return;
			BodyInstance.ApplyCentralImpulse(impulse.ToBullet());
		}

		/// <summary>
		/// Applies an instant force impulse to the RigidBody at a specified position
		/// Should be called from BulletUpdate()
		/// </summary>
		/// <param name="impulse">The force impulse to be applied</param>
		/// <param name="position">Local position at which to apply the force</param>
		public void ApplyImpulse(Vector3 impulse, Vector3 position) {
			if (!Initialized || Disposing || !Registered) return;
			BodyInstance.ApplyImpulse(impulse.ToBullet(), position.ToBullet());
		}
		
		/// <summary>
		/// Applies a torque force to the RigidBody
		/// Should be called from BulletUpdate()
		/// </summary>
		/// <param name="torque">The torque force to be applied</param>
		public void ApplyTorque(Vector3 torque) {
			if (!Initialized || Disposing || !Registered) return;
			BodyInstance.ApplyTorque(torque.ToBullet());
		}
		
		/// <summary>
		/// Applies a torque impulse to the RigidBody at a specified position
		/// Should be called from BulletUpdate()
		/// </summary>
		/// <param name="torque">The torque impulse to be applied</param>
		public void ApplyTorqueImpulse(Vector3 impulse) {
			if (!Initialized || Disposing || !Registered) return;
			BodyInstance.ApplyTorqueImpulse(impulse.ToBullet());
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
			BulletPhysicsWorldManager.Register(BodyInstance);
			Registered = true;
		}
		
		//Unity OnDisable
		protected override void OnDisable() {
			//Check if registered
			//TODO: See OnEnable()
			if (!Registered || !Initialized) return;
			
			//Unregister from the physics world
			if (!Initialized) return;
			BulletPhysicsWorldManager.Unregister(BodyInstance);
			Registered = false;
		}

        //TODO: Transform handoff is now handled in BulletMotionState (Performance Increase)
        //Unity Update
        /*private void Update() {
			if (!Initialized || Disposing || !_rigidBody.IsActive) return;
			
			//Pass transform data to Unity
			_currentTransform = PhysicsMotionState.WorldTransform;
			transform.position = BulletExtensionMethods.ExtractTranslationFromMatrix(ref _currentTransform);
			//transform.rotation = BulletExtensionMethods.ExtractRotationFromMatrix(ref _currentTransform);
			transform.rotation = _currentTransform.GetOrientation().ToUnity();
		}*/

        //Unity Destroy
        protected override void OnDestroy() {
			Dispose();
		}
		#endregion
	}
}
