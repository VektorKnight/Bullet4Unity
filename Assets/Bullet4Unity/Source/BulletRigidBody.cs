using System.Collections;
using System.Collections.Generic;
using BulletSharp;
using Bullet4Unity;
using BulletSharp.Math;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace Bullet4Unity {
	/// <summary>
	/// Interop class for a basic Bullet RigidBody
	/// -Author: vektorKnight
	/// </summary>
	[AddComponentMenu("BulletPhysics/RigidBody")]
	public class BulletRigidBody : BulletBehavior {
		
		//Unity Inspector
		[Header("Basic Settings")]
		[SerializeField] private float _mass;
		[SerializeField] private float _linearDamping = 0f;
		[SerializeField] private float _angularDamping = 0f;
		
		[Header("Friction Settings")]
		[SerializeField] private float _friction = 0f;
		[SerializeField] private float _rollingFriction;

		[Header("World Constraints")] 
		[SerializeField] private Vector3 _linearConstraints = Vector3.one;
		[SerializeField] private Vector3 _angularConstraints = Vector3.one;

		[Header("Advanced Settings")]
		[SerializeField] private float _restitution = 0f;
		[SerializeField] private float _linearSleepThreshold = 1f;
		[SerializeField] private float _angularSleepThreshold = 1f;
		
		//Internal Private
		private bool _initialized;
		private BulletSharp.Math.Matrix _currentTransform;
		private BulletSharp.Math.Vector3 _localInternia;
		
		//Required components for a Bullet RigidBody
		private BulletCollisionShape _bulletCollisionShape;
		private BulletSharp.Math.Matrix _initialTransform;
		private DefaultMotionState _motionState;
		private RigidBodyConstructionInfo _constructionInfo;
		private BulletSharp.RigidBody _rigidBody;
		
		//Initialize the Bullet RigidBody
		private void InitializeRigidBody() {
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
				LinearFactor = _linearConstraints.ToBullet(),
				AngularFactor = _angularConstraints.ToBullet()
			};

			//Initialization complete
			_initialized = true;
		}
		
		//Unity Pre-Initialization
		private void Awake() {
			//Initialize the RigidBody
			if (!_initialized) InitializeRigidBody();
		}
		
		//Unity Start
		private void Start() {
			BulletPhysicsWorld.Instance.RegisterBulletObject(this, _rigidBody);
		}
		
		//Unity OnEnable
		private void OnEnable() {
		}

		// Update is called once per frame
		private void Update() {
			if (!_initialized) return;
			
			_motionState.GetWorldTransform(out _currentTransform);
			transform.position = _currentTransform.Origin.ToUnity();
			transform.rotation = _currentTransform.GetOrientation().ToUnity();
		}

		public override void BulletUpdate(DynamicsWorld world, float bulletTimeStep) {
			
		}
	}
}
