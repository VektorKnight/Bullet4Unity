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
	public class BulletRigidBody : MonoBehaviour {
		
		//Unity Inspector
		[Header("Basic Settings")]
		[SerializeField] private float _mass;
		[SerializeField] private float _linearDamping = 0f;
		[SerializeField] private float _angularDamping = 0f;
		
		[Header("Friction Settings")]
		[SerializeField] private float _friction = 0f;
		[SerializeField] private float _rollingFriction;

		[Header("World COnstraints")] 
		[SerializeField] private Vector3 _linearConstraints;

		[Header("Advanced Settings")]
		[SerializeField] private float _restitution = 0f;
		[SerializeField] private float _linearSleepThreshold = 1f;
		[SerializeField] private float _angularSleepThreshold = 1f;
		
		//Internal Private
		private bool _initialized;
		
		//Required components for a Bullet RigidBody
		private BulletCollisionShape _bulletCollisionShape;
		private BulletSharp.Math.Matrix _transformMatrix;
		private DefaultMotionState _motionState;
		private RigidBodyConstructionInfo _constructionInfo;
		private BulletRigidBody _rigidBody;
		
		//Initialize the Bullet RigidBody
		private void InitializeRigidBody() {
			//Make sure a BulletCollisionShape is attached and reference it if possible
			_bulletCollisionShape = GetComponent<BulletCollisionShape>();
			if (_bulletCollisionShape == null) {
				Debug.LogError("No Bullet collision shape is attached!\n" +
				               "Please attach a Bullet collision shape to this object");
				return;
			}
			
			//Initialize the Bullet transform matrix and set the values based on Unity transform
			_transformMatrix = new Matrix { Origin = transform.position.ToBullet(), Orientation = transform.rotation.ToBullet() };
			
			//Initialize the Bullet default motion state using the transform matrix
			_motionState = new DefaultMotionState(_transformMatrix);
			
			//Initialize the Bullet rigidbody construction info and assign the relevant values
			_constructionInfo = new RigidBodyConstructionInfo(_mass, _motionState, _bulletCollisionShape.GetCollisionShape()) {
				LinearDamping = _linearDamping,
				AngularDamping = _angularDamping,
				Friction = _friction,
				RollingFriction = _rollingFriction,
				Restitution = _restitution,
				LinearSleepingThreshold = _linearSleepThreshold,
				AngularSleepingThreshold = _angularSleepThreshold
			};
		}
		
		//Unity Pre-Initialization
		private void Awake() {
			if (!_initialized) InitializeRigidBody();
		}
		
		//Unity OnEnable
		private void OnEnable() {
		}

		// Update is called once per frame
		void Update() { }
	}
}
