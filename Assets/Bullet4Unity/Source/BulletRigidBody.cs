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
		[SerializeField] private float _linearSleepThreshold = 0.1f;
		[SerializeField] private float _angularSleepThreshold = 0.1f;
		
		//Internal Private
		private bool _initialized;
		private bool _registered;
		private bool _disposing;
		private BulletSharp.Math.Matrix _currentTransform;
		private BulletSharp.Math.Vector3 _localInternia;
		
		//Required components for a Bullet RigidBody
		private BulletCollisionShape _bulletCollisionShape;
		private BulletSharp.Math.Matrix _initialTransform;
		private DefaultMotionState _motionState;
		private RigidBodyConstructionInfo _constructionInfo;
		private BulletSharp.RigidBody _rigidBody;
		
		//Initialize the Bullet RigidBody and register with the simulation
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
			
			//Register with the physics world
			BulletPhysics.Register(_rigidBody);
			_registered = true;

			//Initialization complete
			_initialized = true;
		}
		
		//Dispose Method
		public void Dispose() {
			//Dispose of all the components in reverse order
			if (_disposing) return;
			
			_disposing = true;
			_rigidBody.Dispose();
			_constructionInfo.Dispose();
			_motionState.Dispose();
			_bulletCollisionShape.Dispose();
		}

        public BulletSharp.RigidBody GetUnderlyingRigidbodyInstance() {
            return _rigidBody;
        }

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

		// Update is called once per frame
		private void Update() {
			if (!_initialized || _disposing) return;
			
			_currentTransform = _motionState.WorldTransform;
			transform.position = _currentTransform.Origin.ToUnity();
			transform.rotation = _currentTransform.Orientation.ToUnity();
		}
		
		//Unity Destroy
		private void OnDestroy() {
			Dispose();
		}
	}
}
