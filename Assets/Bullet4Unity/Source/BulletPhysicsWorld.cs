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
	public class BulletPhysicsWorld : MonoBehaviour, IDisposable {
		
		//Static Singleton Instance
		public static BulletPhysicsWorld Instance { get; private set; }
		
		//Unity Inspector
		[Header("Physics World Config")] 
		[SerializeField] private bool _debugging = false;
		[SerializeField] private float _timeStep = 0.02f;
		[SerializeField] private int _maxSubSteps = 10;
		[SerializeField] private Vector3 _gravity = new Vector3(0f, -9.81f, 0f);
		
		//Internal Private
		private bool _initlialized;
		private bool _disposing;
		private BulletSharp.Math.Vector3 _btGravity;
		
		//Bullet RigidBodies registered with the simulation
		private readonly List<BulletSharp.RigidBody> _bulletRigidBodies = new List<RigidBody>();
		
		//Bullet Behaviors to update with the simulation
		private readonly List<BulletBehavior> _bulletBehaviors = new List<BulletBehavior>();

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
		
		//Register a Bullet RigidBody with the simulation
		public void RegisterBulletRigidBody(BulletSharp.RigidBody rigidBody) {
			//Initialize the world and warn the user if it hasn't already been initialized
			if (!_initlialized) {
				InitializeWorld(BulletUpdate);
				Debug.LogWarning("A Bullet RigidBody attempted to register with the simulation before it was initialized!\n" +
				                 "Please check your script execution order");
			}
			
			//Check if already registered and warn the user
			if (_bulletRigidBodies.Contains(rigidBody)) {
				Debug.LogError("Specified Bullet RigidBody has already been registered with the simulation!\n");
				return;
			}
			
			//Register the Bullet RigidBody with the simulation and list for tracking
			_bulletRigidBodies.Add(rigidBody);
			_dynamicsWorld.AddRigidBody(rigidBody);
		}
		
		//Unregister a Bullet RigidBody from the simulation
		public void UnregisterBulletRigidBody(BulletSharp.RigidBody rigidBody) {
			//Warn the user if the physics world has not been initialized
			if (!_initlialized) {
				Debug.LogError("An object attempted to unregister from the simulation before it was initialized!\n" +
				               "Please check your scene setup!");
				return;
			}

			//Check if the specified Object has been registered
			if (!_bulletRigidBodies.Contains(rigidBody)) {
				Debug.LogError("Specified Bullet RigidBody has not been registered with this simulation!\n" +
				               "Please check your scene setup!");
				return;
			}
			
			//Unregister the Bullet object from the simulation and callback
			_bulletRigidBodies.Remove(rigidBody);
			_dynamicsWorld.RemoveRigidBody(rigidBody);
		}
		
		//Register a BulletBehavior with the simulation callback
		public void RegisterBulletBehavior(BulletBehavior behavior) {
			//Initialize the world and warn the user if it hasn't already been initialized
			if (!_initlialized) {
				InitializeWorld(BulletUpdate);
				Debug.LogWarning("A BulletBehavior attempted to register with the simulation callback before it was initialized!\n" +
				                 "Please check your script execution order");
			}
			
			//Check if already registered and warn the user
			if (_bulletBehaviors.Contains(behavior)) {
				Debug.LogError("Specified BulletBehavior has already been registered with the simulation callback!\n");
				return;
			}
			
			//Register the BulletBehavior with the simulation callback
			_bulletBehaviors.Add(behavior);
		}
		
		//Unregister a BulletBehavior from the simulation callback
		public void UnregisterBulletBehavior(BulletBehavior behavior) {
			//Warn the user if the physics world has not been initialized
			if (!_initlialized) {
				Debug.LogError("A BulletBehavior attempted to unregister from the simulation callback before it was initialized!\n" +
				               "Please check your scene setup!");
				return;
			}

			//Check if the specified Object has been registered
			if (!_bulletBehaviors.Contains(behavior)) {
				Debug.LogError("Specified object has not been registered with this simulation!\n" +
				               "Please check your scene setup!");
				return;
			}
			
			//Unregister the Bullet object from the simulation and callback
			_bulletBehaviors.Remove(behavior);
		}
		
		//Bullet callback method (equivalent to Unity's FixedUpdate but for Bullet)
		private void BulletUpdate(DynamicsWorld world, float bulletTimeStep) {
			//Log debug info if enabled
			if (_debugging) {
				Debug.Log(string.Format("<b>Bullet4Unity:</b> Simulation stepped by {0} seconds\n" +
				                        "<b>Bullet4Unity:</b> Simulation opdating {1} rigidbodies",
										bulletTimeStep, _bulletRigidBodies.Count));
				Debug.Log(string.Format("<b>Bullet4Unity:</b> Callback updating {0} Bullet Behaviors\n", _bulletBehaviors.Count));
			}
			
			//Return if no behaviors to update
			if (_bulletBehaviors.Count == 0) return;
			
			//Update all bullet behaviors
			foreach (var behavior in _bulletBehaviors) {
				behavior.BulletUpdate(world, bulletTimeStep);
			}
		}
		
		//Unity Pre-Initialization
		private void Awake() {
			//Enforce Single Instance
			if (Instance == null) Instance = this;
			else if (Instance != this) Destroy(gameObject);
			
			//Initialize the world if it hasn't already been initialized
			if (!_initlialized) InitializeWorld(BulletUpdate);
			
			//Find all Bullet rigidbodies and tell them to initialize
			var rigidBodies = (BulletRigidBody[])FindObjectsOfType(typeof(BulletRigidBody));
			if (rigidBodies.Length == 0) return;
			foreach (var rb in rigidBodies) {
				rb.InitializeRigidBody();
			}
			rigidBodies = null;
		}
		
		//Unity Per-Frame Update
		private void Update() {
			//Make sure we are in play mode and cleanup hasnt started
			if (!Application.isPlaying && _disposing) return;
			
			//Step the simulation world
			_dynamicsWorld.StepSimulation(Time.deltaTime, 10, _timeStep);
		}
		
		//Unity Destroy
		private void OnDestroy() {
			if (_disposing) return;
			Dispose();
		}
		
		//Dispose Method
		public void Dispose() {
			//Unregister all registered rigidbodies
			foreach (var rb in _bulletRigidBodies) {
				_dynamicsWorld.RemoveRigidBody(rb);
			}
			
			//Find all Bullet rigidbody instances and tell them to dispose
			var rigidBodies = (BulletRigidBody[])FindObjectsOfType(typeof(BulletRigidBody));
			if (rigidBodies.Length == 0) return;
			foreach (var rb in rigidBodies) {
				rb.Dispose();
			}
			rigidBodies = null;
			
			//Dispose of the physics world components in reverse order
			_disposing = true;
			_dynamicsWorld.Dispose();
			_constraintSolver.Dispose();
			_overlapPairCache.Dispose();
			_collisionDispatcher.Dispose();
			_collisionConfig.Dispose();		
			_bulletBehaviors.Clear();
		}
	}
}
