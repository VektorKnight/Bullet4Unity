using System;
using System.Collections;
using System.Collections.Generic;
using BulletSharp;
using Bullet4Unity;
using BulletSharp.Math;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;
using Quaternion = UnityEngine.Quaternion;

namespace Bullet4Unity {
	/// <summary>
	/// Interop class for a basic Bullet RigidBody
	/// -Author: vektorKnight
	/// </summary>
	[AddComponentMenu("BulletPhysics/RigidBody")]
	public class BulletRigidBody : MonoBehaviour, IDisposable {
		
		#region Unity Inspector
		//Unity Inspector
		[Header("Basic Settings")]
		[SerializeField] private float _mass = 1f;
		[SerializeField] private float _linearDamping = 0.05f;
		[SerializeField] private float _angularDamping = 0.05f;
		[SerializeField] private float _restitution = 0f;
		
		[Header("Friction Settings")]
		[SerializeField] private float _friction = 0.2f;
		[SerializeField] private float _rollingFriction = 0.5f;

		[Header("World Factors")] 
		[SerializeField] private Vector3 _linearFactor = Vector3.one;
		[SerializeField] private Vector3 _angularFactor = Vector3.one;

		[Header("Sleep Settings")] 
		[SerializeField] private bool _neverSleep = false;
		[SerializeField] private float _linearSleepThreshold = 0.01f;
		[SerializeField] private float _angularSleepThreshold = 0.01f;
		#endregion
		
		#region Private Members
		//Internal Private
		private bool _initialized;
		private bool _registered;
		private bool _disposing;
		private Matrix _currentTransform;
		private BulletSharp.Math.Vector3 _localInternia;
		
		//Required components for a Bullet RigidBody
		private BulletCollisionShape _bulletCollisionShape;
		private Matrix _initialTransform;
		private DefaultMotionState _motionState;
		private RigidBodyConstructionInfo _constructionInfo;
		private RigidBody _rigidBody;
		#endregion

		#region Public Properties
		//Get Bullet RigidBody Instance
		public RigidBody RigidBody {
			get { return _rigidBody; }
		}
		
		//Get MotionState Instance
		public MotionState MotionState {
			get { return _motionState; }
		}
		
		//Get or Set RigidBody Mass
		public float Mass {
			get { return _mass; }
			set {
				_mass = value;
				_bulletCollisionShape.GetCollisionShape().CalculateLocalInertia(_mass, out _localInternia);
				_rigidBody.SetMassProps(_mass, _localInternia);
			}
		}
		
		//Get or Set Linear Damping
		public float LinearDamping {
			get { return _linearDamping; }
			set {
				_linearDamping = value;
				_rigidBody.SetDamping(_linearDamping, _angularDamping);
			}
		}
		
		//Get or Set Angular Damping
		public float AngularDamping {
			get { return _angularDamping; }
			set {
				_angularDamping = value;
				_rigidBody.SetDamping(_linearDamping, _angularDamping);
			}
		}
		
		//Get or Set Restitution
		public float Restitution {
			get { return _restitution; }
			set {
				_restitution = value;
				_rigidBody.Restitution = value;
			}
		}
		
		//Get or Set Friction
		public float Friction {
			get { return _friction; }
			set {
				_friction = value;
				_rigidBody.Friction = _friction;
			}
		}
		
		//Get or Set Rolling Friction
		public float RollingFriction {
			get { return _rollingFriction; }
			set {
				_rollingFriction = value;
				_rigidBody.RollingFriction = value;
			}
		}

		#endregion

		#region Public Methods
		/// <summary>
		/// Initializes the Bullet RigidBody and attempts to register it with the physics world.
		/// This should generally only be called internally or from a physics world instance.
		/// </summary>
		public void InitializeRigidBody() {
			//Check if already initialized
			if (_initialized) return;
			
			//Make sure a BulletCollisionShape is attached and reference it if possible
			_bulletCollisionShape = GetComponent<BulletCollisionShape>();
			if (_bulletCollisionShape == null) {
				Debug.LogError("No Bullet collision shape is attached!\n" +
				               "Please attach a Bullet collision shape to this object");
				return;
			}
			
			//Calculate the local intertial of the given shape
			_bulletCollisionShape.GetCollisionShape().CalculateLocalInertia(_mass, out _localInternia);
			
			//Initialize the Bullet transform matrix and set the values based on Unity transform
			_initialTransform = new Matrix { Origin = transform.position.ToBullet(), Orientation = transform.rotation.ToBullet() };
			
			//Initialize the Bullet default motion state using the transform matrix
			_motionState = new DefaultMotionState(_initialTransform);
			
			//Initialize the Bullet rigidbody construction info and assign the relevant values
			_constructionInfo = new RigidBodyConstructionInfo(_mass, _motionState, _bulletCollisionShape.GetCollisionShape()) {
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
			_rigidBody = new BulletSharp.RigidBody(_constructionInfo) {
				LinearFactor = _linearFactor.ToBullet(),
				AngularFactor = _angularFactor.ToBullet()
			};
			
			//Set sleeping flag
			if (_neverSleep) _rigidBody.ActivationState = ActivationState.DisableDeactivation;
			_rigidBody.CcdMotionThreshold = 0.5f;
			_rigidBody.CcdSweptSphereRadius = 0.25f;
			
			//Register with the physics world
			BulletPhysics.Register(_rigidBody);
			_registered = true;

			//Initialization complete
			_initialized = true;
		}
		
		/// <summary>
		/// Applies a continuous force to the RigidBody
		/// Should be called from BulletUpdate()
		/// <param name="force">The continuous force to be applied</param>>
		/// </summary>
		public void ApplyForce(Vector3 force) {
			if (!_initialized || _disposing || !_registered) return;
			_rigidBody.ApplyCentralForce(force.ToBullet());
		}
		
		/// <summary>
		/// Applies a continuous force to the RigidBody at a specified position
		/// Should be called from BulletUpdate()
		/// <param name="force">The continuous force to be applied</param>>
		/// <param name="position">Local position at which to apply the force</param>>
		/// </summary>
		public void ApplyForce(Vector3 force, Vector3 position) {
			if (!_initialized || _disposing || !_registered) return;
			_rigidBody.ApplyForce(force.ToBullet(), position.ToBullet());
		}
		
		/// <summary>
		/// Applies an instant force impulse to the RigidBody
		/// Should be called from BulletUpdate()
		/// </summary>
		/// <param name="impulse"></param>
		public void ApplyImpulse(Vector3 impulse) {
			if (!_initialized || _disposing || !_registered) return;
			_rigidBody.ApplyCentralImpulse(impulse.ToBullet());
		}

		/// <summary>
		/// Applies an instant force impulse to the RigidBody at a specified position
		/// Should be called from BulletUpdate()
		/// </summary>
		/// <param name="impulse">The force impulse to be applied</param>
		/// <param name="position">Local position at which to apply the force</param>
		public void ApplyImpulse(Vector3 impulse, Vector3 position) {
			if (!_initialized || _disposing || !_registered) return;
			_rigidBody.ApplyImpulse(impulse.ToBullet(), position.ToBullet());
		}

		public void ApplyTorque(Vector3 torque) {
			if (!_initialized || _disposing || !_registered) return;
			_rigidBody.ApplyTorque(torque.ToBullet());
			
		}
		
		/// <summary>
		/// Disposes of the RigidBody
		/// Should only be called internally or from a physics world instance.
		/// </summary>
		public void Dispose() {
			//Dispose of all the components in reverse order
			if (_disposing) return;
			if (_registered) BulletPhysics.Unregister(_rigidBody);
			
			_disposing = true;
			_rigidBody.Dispose();
			_constructionInfo.Dispose();
			_motionState.Dispose();
			_bulletCollisionShape.Dispose();
		}
		#endregion
		
		#region Private Methods
        //Unity OnEnable
        private void OnEnable() {
			//Check if initialized and already registered
			//TODO: Physics world should probably implement a method to return registration status
			if (!_initialized) InitializeRigidBody();
	        if (_registered) return;
			
			//Register with the physics world
			BulletPhysics.Register(_rigidBody);
			_registered = true;
		}
		
		//Unity OnDisable
		private void OnDisable() {
			//Check if registered
			//TODO: See OnEnable()
			if (!_registered || !_initialized) return;
			
			//Unregister from the physics world
			BulletPhysics.Unregister(_rigidBody);
			_registered = false;
		}
		
		//Unity Update
		private void Update() {
			//Pass transform data to Unity
			_motionState.GetWorldTransform(out _currentTransform);
			transform.position = BulletExtensionMethods.ExtractTranslationFromMatrix(ref _currentTransform);
			transform.rotation = BulletExtensionMethods.ExtractRotationFromMatrix(ref _currentTransform);
		}

		//Unity Destroy
		private void OnDestroy() {
			Dispose();
		}
		#endregion
	}
}
