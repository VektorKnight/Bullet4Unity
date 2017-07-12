using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using UnityEngine;
using Bullet4Unity;
using BulletSharp;
using Object = UnityEngine.Object;

namespace Bullet4Unity {
    /// <summary>
    /// Implements a Bullet Discrete Dynamics World
    /// - Authors: VektorKnight, Techgeek1
    /// </summary>
    [Serializable]
    public class DiscreteRigidDynamicsWorld : PhysicsWorld {
        //Unity Inspector
        [Header("Physics World Config")] 
        [SerializeField] private bool _syncToRender = false;
        [SerializeField] private float _timeStep = 0.02f;
        [SerializeField] private int _maxSubSteps = 10;
        [SerializeField] private Vector3 _gravity = new Vector3(0f, -9.81f, 0f);

        [Header("Advanced World Config")] 
        [Tooltip("NNCG appears to be more accurate and faster for now")]
        [SerializeField] private WorldSolverType _solverType = WorldSolverType.NonSmoothNonLinearConjugate;
        [SerializeField] private int _solverIterations = 6;
        [Tooltip("DynamicAABB for many dynamic objects, AxisSweep for mostly static objects")]
        [SerializeField] private WorldBroadphaseType _broadphaseType = WorldBroadphaseType.DynamicAabb;
        [Tooltip("How many units from origin on each axis that the AxisSweep broadphase will test")]
        [SerializeField] private Vector3 _axisSweepMargin = new Vector3(1000f, 1000f, 1000f);
        
        [Header("Physics World Debugging")]
        [SerializeField] private bool _debugging = false;
        private string _debugText;
        
        //Inspector Advanced World Config Interop
        [Serializable] private enum WorldSolverType {SequentialImpulse, NonSmoothNonLinearConjugate}
        [Serializable] private enum WorldBroadphaseType { DynamicAabb, AxisSweep}

        //Internal Private
        private bool _initlialized;
        private bool _disposing;
        private BulletSharp.Math.Vector3 _btGravity;

        //Bullet RigidBodies registered with the simulation
        private readonly List<RigidBody> _bulletRigidBodies = new List<RigidBody>();

        //Bullet Behaviors to update with the simulation
        private readonly List<BulletBehavior> _bulletBehaviors = new List<BulletBehavior>();

        //Required components to initialize a Bullet Discrete Dynamics World
        private DefaultCollisionConfiguration _collisionConfig;
        private CollisionDispatcher _collisionDispatcher;
        private BroadphaseInterface _broadphaseInterface;
        private ConstraintSolver _constraintSolver;
        private DiscreteDynamicsWorld _dynamicsWorld;
        
        //Public initialize method
        public override void Initialize() {
            //Initialize the world if it hasn't already been initialized
            if (!_initlialized) InitializeWorld(BulletUpdate);

            //Find all Bullet rigidbodies and tell them to initialize
            var rigidBodies = (BulletRigidBody[])Object.FindObjectsOfType(typeof(BulletRigidBody));
            if (rigidBodies.Length == 0) return;
            foreach (var rb in rigidBodies) {
                rb.InitializeRigidBody();
            }
        }
        
        //Initialize the physics world
        private void InitializeWorld(DynamicsWorld.InternalTickCallback tickCallBack) {
            //Collision configuration and dispatcher
            _collisionConfig = new DefaultCollisionConfiguration();
            _collisionDispatcher = new CollisionDispatcher(_collisionConfig);
            
            //Solver Type Config
            switch (_solverType) {
                case WorldSolverType.SequentialImpulse:
                    _constraintSolver = new SequentialImpulseConstraintSolver();
                    break;
                case WorldSolverType.NonSmoothNonLinearConjugate:
                    _constraintSolver = new NncgConstraintSolver();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            //Broadphase Type COnfig
            switch (_broadphaseType) {
                case WorldBroadphaseType.DynamicAabb:
                    _broadphaseInterface = new DbvtBroadphase();
                    break;
                case WorldBroadphaseType.AxisSweep:
                    _broadphaseInterface = new AxisSweep3(-_axisSweepMargin.ToBullet(), _axisSweepMargin.ToBullet());
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            //Create the physics world
            _dynamicsWorld = new DiscreteDynamicsWorld(_collisionDispatcher, _broadphaseInterface, _constraintSolver, _collisionConfig);

            //Configure the physics world
            _dynamicsWorld.SolverInfo.NumIterations = _solverIterations;
            _dynamicsWorld.SetInternalTickCallback(tickCallBack);
            _btGravity = _gravity.ToBullet();
            _dynamicsWorld.SetGravity(ref _btGravity);
            _initlialized = true;
            if (_debugging) Debug.Log("<b>Bullet4Unity:</b> Initialized Bullet Physics World");
        }
        
        //Dispose of components and clean up
        public override void Dispose() {
            if (_disposing)
                return;

            //Unregister all registered rigidbodies
            foreach (var rb in _bulletRigidBodies) {
                _dynamicsWorld.RemoveRigidBody(rb);
            }

            //Find all Bullet rigidbody instances and tell them to dispose
            var rigidBodies = (BulletRigidBody[])Object.FindObjectsOfType(typeof(BulletRigidBody));
            if (rigidBodies.Length == 0) return;
            foreach (var rb in rigidBodies) {
                rb.Dispose();
            }
            rigidBodies = null;

            //Dispose of the physics world components in reverse order
            _disposing = true;
            _dynamicsWorld.Dispose();
            _constraintSolver.Dispose();
            _broadphaseInterface.Dispose();
            _collisionDispatcher.Dispose();
            _collisionConfig.Dispose();
            _bulletBehaviors.Clear();
        }
        
        //Step the physics simulation
        public override void StepSimulation() {
            //Make sure we are in play mode and cleanup hasnt started
            if (_disposing) return;
            
            //Set the simulation
            if (_syncToRender) {
                //Step with renderer
                _dynamicsWorld.StepSimulation(Time.deltaTime, 0);
            }
            else {
                //Step with fixed interval (fixed timestep)
                _dynamicsWorld.StepSimulation(Time.deltaTime, 10, _timeStep);
            }
            
            
        }

        //Register a Bullet RigidBody with the simulation
        public override void Register(RigidBody rigidBody) {
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
        public override void Unregister(RigidBody rigidBody) {
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
        public override void Register(BulletBehavior behavior) {
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
        public override void Unregister(BulletBehavior behavior) {
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
            if (_debugging && _debugText != null) {
                _debugText = "[BulletDiscreteDynamicsWorld]\n" +
                                  string.Format("RenderDelta: {0:n3}(ms)\n", 1000f * Time.deltaTime) +
                                  string.Format("PhysicsDelta: {0:n3}(ms)\n", 1000f * bulletTimeStep) +
                                  string.Format("Rigidbodies: {0} Active\n", _bulletRigidBodies.Count) +
                                  string.Format("SimCallback: {0} Listeners\n", _bulletBehaviors.Count);
            }

            //Return if no behaviors to update
            if (_bulletBehaviors.Count == 0) return;

            //Update all bullet behaviors
            foreach (var behavior in _bulletBehaviors) {
                behavior.BulletUpdate(world, bulletTimeStep);
            }
        }
    }
}
