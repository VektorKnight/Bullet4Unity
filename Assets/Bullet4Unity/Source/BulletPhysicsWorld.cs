using System;
using System.Collections;
using System.Collections.Generic;
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
		[SerializeField] private bool _debugging = false;
		[SerializeField] private float _timeStep = 0.02f;
		[SerializeField] private int _maxSubSteps = 10;
		[SerializeField] private Vector3 _gravity = new Vector3(0f, -9.81f, 0f);
		
		//Internal private
		private bool _initlialized;
		private BulletSharp.Math.Vector3 _btGravity;
		
		//Bullet Behaviors to update with the simulation
		private Dictionary<string, BulletBehavior> _bulletBehaviors = new Dictionary<string, BulletBehavior>();

		//Required components to initialize a Bullet Discrete Dynamics World
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
			_initlialized = true;
			if (_debugging) Debug.Log("<b>Bullet4Unity:</b> Initialized Bullet Physics World");
		}
		
		//Register a BulletBehavior with the simulation and callback
		public void RegisterBulletObject(BulletBehavior behavior, BulletSharp.RigidBody rigidBody) {
			//Initialize the world and warn the user if it hasn't already been initialized
			if (!_initlialized) {
				InitializeWorld(BulletUpdate);
				Debug.LogWarning("An object attempted to register with the simulation before it was initialized!\n" +
				                 "Please check your script execution order");
			}
			
			//Register the Bullet object with the simulation and callback
			_bulletBehaviors.Add(behavior.GetType().Name, behavior);
			_dynamicsWorld.AddRigidBody(rigidBody);
		}
		
		//Unregister a BulletBehavior with the simulation and callback
		public void UnregisterBulletObject(BulletBehavior behavior, BulletSharp.RigidBody rigidBody) {
			//Warn the user if the physics world has not been initialized
			if (!_initlialized) {
				Debug.LogError("An object attempted to unregister from the simulation before it was initialized!\n" +
				               "Please check your scene setup!");
				return;
			}

			//Check if the specified Object has been registered
			var objectKey = behavior.GetType().Name;
			if (!_bulletBehaviors.ContainsKey(objectKey)) {
				Debug.LogError("Specified object has not been registered with this simulation!\n" +
				               "Please chekc your scene setup!");
				return;
			}
			
			//Unregister the Bullet object from the simulation and callback
			_bulletBehaviors.Remove(objectKey);
			_dynamicsWorld.RemoveRigidBody(rigidBody);
		}
		
		//Unity Pre-Initialization
		private void Awake() {
			//Enforce Single Instance
			if (Instance == null) Instance = this;
			else if (Instance != this) Destroy(gameObject);
			
			//Initialize the world if it hasn't already been initialized
			if (!_initlialized) InitializeWorld(BulletUpdate);
		}
		
		//Unity Per-Frame Update
		private void Update() {
			//Step the simulation
			_dynamicsWorld.StepSimulation(Time.deltaTime, 10, _timeStep);
		}
		
		//Bullet callback method (equivalent to Unity's FixedUpdate but for Bullet)
		private void BulletUpdate(DynamicsWorld world, float bulletTimeStep) {
			//Log debug info if enabled
			if (_debugging) {
				Debug.Log(string.Format("<b>Bullet Callback:</b> Simulation stepped by {0} seconds\n", bulletTimeStep) +
						  string.Format("<b>Bullet Callback:</b> Updating {0} Bullet Behaviors", _bulletBehaviors.Count));
			}
			
			//Return if no behaviors to update
			if (_bulletBehaviors.Count == 0) return;
			
			//Update all bullet behaviors
			foreach (var kvp in _bulletBehaviors) {
				kvp.Value.BulletUpdate(world, bulletTimeStep);
			}
		}
	}
}
