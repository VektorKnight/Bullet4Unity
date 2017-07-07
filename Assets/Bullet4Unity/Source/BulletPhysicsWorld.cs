using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Bullet4Unity;
using BulletSharp;
using UnityEngine;

namespace Bullet4Unity {
	/// <summary>
	/// Basic Bullet physics world implementation for Unity
	/// Currently only implements a Discrete Dynamics World
	/// Based on Page 6 of the documentation located at:
	/// https://github.com/bulletphysics/bullet3/blob/master/docs/BulletQuickstart.pdf
	/// TODO: Implement interop code for inspector-friendly components
	/// -Author: VektorKnight
	/// </summary>
	[AddComponentMenu("BulletPhysics/Worlds/PhysicsWorld")]
	public class BulletPhysicsWorld : MonoBehaviour {
		
		//Static Singleton Instance
		public static BulletPhysicsWorld Instance { get; private set; }
		
		//Unity Inspector
		[Header("Physics World Config")]
		[SerializeField] private float _timeStep = 0.02f;
		[SerializeField] private int _maxSubSteps = 10;
		[SerializeField] private Vector3 _gravity = new Vector3(0f, -9.81f, 0f);
		
		//Internal private
		private BulletSharp.Math.Vector3 _btGravity;

		private BulletSharp.DefaultCollisionConfiguration _collisionConfig;
		private BulletSharp.CollisionDispatcher _collisionDispatcher;
		private BulletSharp.BroadphaseInterface _overlapPairCache;
		private BulletSharp.SequentialImpulseConstraintSolver _constraintSolver;
		private BulletSharp.DiscreteDynamicsWorld _dynamicsWorld;
		
		//Initialize the Physics World
		private void InitializeWorld(BulletSharp.DynamicsWorld.InternalTickCallback tickCallBack) {
			//Set up default parameters
			_collisionConfig = new DefaultCollisionConfiguration();
			_collisionDispatcher = new CollisionDispatcher(_collisionConfig);
			_overlapPairCache = new DbvtBroadphase();
			_constraintSolver = new SequentialImpulseConstraintSolver();
			_dynamicsWorld = new DiscreteDynamicsWorld(_collisionDispatcher, _overlapPairCache, _constraintSolver, _collisionConfig);
			
			//Set up custom parameters
			_dynamicsWorld.SetInternalTickCallback(tickCallBack);
			_btGravity = _gravity.ToBullet();
			_dynamicsWorld.SetGravity(ref _btGravity);	
		}
		
		//Unity Pre-Initialization
		private void Awake() {
			//Enforce Single Instance
			if (Instance == null) Instance = this;
			else if (Instance != this) Destroy(gameObject);
			
			//Initialize the physics world
			InitializeWorld(BulletUpdate);
		}
		
		//Per-Frame Update
		private void Update() {
			//Step the simulation(deltaTime, max sub-steps, fixedTimeStep)
			_dynamicsWorld.StepSimulation(Time.deltaTime, 10, _timeStep);
		}
		
		//TODO: Implement a way streamlined ways to call this method on all Bullet objects
		//Bullet callback function (equivalent to Unity's FixedUpdate for Bullet)
		private void BulletUpdate(DynamicsWorld world, float timeStep) {
		}
	}
}