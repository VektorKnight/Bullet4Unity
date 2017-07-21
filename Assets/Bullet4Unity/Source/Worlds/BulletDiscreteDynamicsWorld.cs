using System;
using System.Collections.Generic;
using BulletSharp;
using UnityEngine;

namespace Bullet4Unity {
    /// <summary>
    /// Implements a Bullet Discrete Dynamics World
    /// - Authors: VektorKnight, Techgeek1
    /// </summary>
    [AddComponentMenu("BulletPhysics/Worlds/RigidBodyDynamics")]
    public class BulletDiscreteDynamicsWorld : BulletPhysicsWorld {
        //Unity Inspector
        [Header("Physics World Config")] 
        [SerializeField] private string _worldName;
        [SerializeField] private float _timeStep = 0.01666667f;
        [SerializeField] private int _maxSubSteps = 4;
        [SerializeField] private Vector3 _gravity = new Vector3(0f, -9.81f, 0f);

        [Header("Advanced World Config")] 
        [Tooltip("Multithreaded solver is now the default")]
        [SerializeField] private WorldSolverType _solverType = WorldSolverType.ExperimentalMultiThreaded;
        [SerializeField] private int _solverIterations = 12;
        [Tooltip("DynamicAABB for many dynamic objects, AxisSweep for mostly static objects")]
        [SerializeField] private WorldBroadphaseType _broadphaseType = WorldBroadphaseType.DynamicAabb;
        [Tooltip("How many units from origin on each axis that the AxisSweep broadphase will test")]
        [SerializeField] private Vector3 _axisSweepMargin = new Vector3(1000f, 1000f, 1000f);
        
        [Header("Multihreaded Solver Config")]
        [SerializeField] private TaskSchedulerType _schedulerType = TaskSchedulerType.Ppl;
        [SerializeField] private int _threadPoolSize = 4;
        [SerializeField] private int _solverThreads = 2;
        
        [Header("Physics World Debugging")]
        [SerializeField] private bool _debugging;

        //Inspector Advanced World Config Interop
        [Serializable] private enum WorldSolverType {SequentialImpulse, NonSmoothNonLinearConjugate, ExperimentalMultiThreaded}
        [Serializable] private enum TaskSchedulerType {Sequential, OpenMp, Ppl}
        [Serializable] private enum WorldBroadphaseType { DynamicAabb, AxisSweep}
        
        //Public Debug Readout Property
        public string DebugReadout { get; private set; }

        //Internal Private
        private bool _initialized;
        private bool _disposing;
        private BulletSharp.Math.Vector3 _btGravity;

        //Bullet RigidBodies registered with the simulation
        private readonly List<RigidBody> _bulletRigidBodies = new List<RigidBody>();
        
        //Bullet Constraints registered with the simulation
        private readonly  List<TypedConstraint> _bulletConstraints = new List<TypedConstraint>();

        //Bullet Behaviors to update with the simulation
        private readonly List<BulletBehaviour> _bulletBehaviors = new List<BulletBehaviour>();

        //Required components to initialize a Bullet Discrete Dynamics World
        private DefaultCollisionConfiguration _collisionConfig;
        private CollisionDispatcher _collisionDispatcher;
        private BroadphaseInterface _broadphaseInterface;
        private ConstraintSolver _constraintSolver;
        private DiscreteDynamicsWorld _dynamicsWorld;
        
        //Multithreaded Physics Simulation
        private TaskScheduler _threadedTaskScheduler;
        private ConstraintSolverPoolMultiThreaded _threadedSolver;
        
        //Public GetWorldName function
        public override string GetWorldName() {
            //The first world (0) must be default
            if (Index == 0) {
                _worldName = "default";
                return _worldName;
            }
            
            //Generate a name if one isn't specified
            if (_worldName == string.Empty) {
                _worldName = $"DiscreteWorld{Index}";
            }
            return _worldName;
        }

        //Public initialize method
        public override void Initialize() {
            //Initialize the world if it hasn't already been initialized
            if (!_initialized) InitializeWorld(BulletUpdate);
        }
        
        //Initialize the physics world
        private void InitializeWorld(DynamicsWorld.InternalTickCallback tickCallBack) {
            //Increment the world index counter
            Index++;
            
            //Collision configuration and dispatcher
            var collisionConfigInfo = new DefaultCollisionConstructionInfo {
                DefaultMaxCollisionAlgorithmPoolSize = 80000,
                DefaultMaxPersistentManifoldPoolSize = 80000
            };
            
            _collisionConfig = new DefaultCollisionConfiguration(collisionConfigInfo);
            
            //Solver Type Config
            switch (_solverType) {
                case WorldSolverType.SequentialImpulse:
                    _constraintSolver = new MultiBodyConstraintSolver();
                    _collisionDispatcher = new CollisionDispatcher(_collisionConfig);
                    break;
                case WorldSolverType.NonSmoothNonLinearConjugate:
                    _constraintSolver = new NncgConstraintSolver();
                    _collisionDispatcher = new CollisionDispatcher(_collisionConfig);
                    break;
                case WorldSolverType.ExperimentalMultiThreaded: //EXPERIMENTAL
                    switch (_schedulerType) {
                        case TaskSchedulerType.Sequential:
                            Threads.TaskScheduler = Threads.GetSequentialTaskScheduler();
                            break;
                        case TaskSchedulerType.OpenMp:
                            Threads.TaskScheduler = Threads.GetOpenMPTaskScheduler();
                            break;
                        case TaskSchedulerType.Ppl:
                            Threads.TaskScheduler = Threads.GetPplTaskScheduler();
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    _threadedSolver = new ConstraintSolverPoolMultiThreaded(_solverThreads);
                    Threads.TaskScheduler.NumThreads = _threadPoolSize;
                    _collisionDispatcher = new CollisionDispatcherMultiThreaded(_collisionConfig);
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
            if (_solverType == WorldSolverType.ExperimentalMultiThreaded) {
                _dynamicsWorld = new DiscreteDynamicsWorldMultiThreaded(_collisionDispatcher, _broadphaseInterface, _threadedSolver, _collisionConfig);
            }
            else {
                _dynamicsWorld = new DiscreteDynamicsWorld(_collisionDispatcher, _broadphaseInterface, _constraintSolver, _collisionConfig);
            }
           
            //Configure the physics world
            _dynamicsWorld.SolverInfo.NumIterations = _solverIterations;
            _dynamicsWorld.SolverInfo.SolverMode = SolverModes.Simd | SolverModes.UseWarmStarting;
            _dynamicsWorld.SetInternalTickCallback(tickCallBack);
            _btGravity = _gravity.ToBullet();
            _dynamicsWorld.SetGravity(ref _btGravity);
            _initialized = true;
        }
        
        //Dispose of components and clean up
        public override void Dispose() {
            if (_disposing)
                return;

            //Unregister all registered rigidbodies
            foreach (var rb in _bulletRigidBodies) {
                _dynamicsWorld.RemoveRigidBody(rb);
            }
            
            //Find all constraints and tell them to dispose
            var constraints = (BulletTypedConstraint[])FindObjectsOfType(typeof(BulletTypedConstraint));
            if (constraints.Length == 0) return;
            foreach (var tc in constraints) {
                tc.Dispose();
            }

            //Find all Bullet rigidbody instances and tell them to dispose
            var rigidBodies = (BulletRigidBody[])FindObjectsOfType(typeof(BulletRigidBody));
            if (rigidBodies.Length == 0) return;
            foreach (var rb in rigidBodies) {
                rb.Dispose();
            }

            //Dispose of the physics world components in reverse order
            _disposing = true;
            _dynamicsWorld.Dispose();
            _constraintSolver?.Dispose();
            _broadphaseInterface.Dispose();
            _collisionDispatcher.Dispose();
            _collisionConfig.Dispose();
            _bulletBehaviors.Clear();
            _threadedSolver?.Dispose();
        }
        
        //Step the physics simulation
        public override void StepSimulation() {
            //Make sure we are in play mode and cleanup hasnt started
            if (_disposing) return;
    
            //Step with fixed interval (fixed timestep)
            _dynamicsWorld.StepSimulation(Time.deltaTime, _maxSubSteps, _timeStep);
        }

        //Register a Bullet RigidBody with the simulation
        public override void Register(RigidBody rigidBody) {
            //Initialize the world and warn the user if it hasn't already been initialized
            if (!_initialized) {
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
            if (!_initialized) {
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

            //Remove any active constraints on the rigidbody
            if (rigidBody.NumConstraintRefs > 0) {
                for (var i = 0; i < rigidBody.NumConstraintRefs; i++) {
                    var constraint = rigidBody.GetConstraintRef(i);
                    rigidBody.RemoveConstraintRef(constraint);
                }
            }
            
            //Unregister the rigidbody with the simulation
            _bulletRigidBodies.Remove(rigidBody);
            _dynamicsWorld.RemoveRigidBody(rigidBody);
        }
        
        //Register a constraint with the simulation
        public override void Register(TypedConstraint constraint) {
            if (!_initialized) {
                InitializeWorld(BulletUpdate);
                Debug.LogWarning("A Constraint attempted to register with the simulation before it was initialized!\n" +
                                 "Please check your script execution order");
            }
            
            //Check if already registered and warn the user
            if (_bulletConstraints.Contains(constraint)) {
                Debug.LogError("Specified Constraint has already been registered with the simulation!\n");
                return;
            }

            //Register the Bullet RigidBody with the simulation and list for tracking
            _bulletConstraints.Add(constraint);
            _dynamicsWorld.AddConstraint(constraint);
        }
        
        //Unregister a constraint with the simulation
        public override void Unregister(TypedConstraint constraint) {
            //Warn the user if the physics world has not been initialized
            if (!_initialized) {
                Debug.LogError("A Constraint attempted to unregister from the simulation before it was initialized!\n" +
                               "Please check your scene setup!");
                return;
            }

            //Check if the specified Object has been registered
            if (!_bulletConstraints.Contains(constraint)) {
                Debug.LogError("Specified Constraint has not been registered with this simulation!\n" +
                               "Please check your scene setup!");
                return;
            }

            //Unregister the Bullet object from the simulation and callback
            _bulletConstraints.Remove(constraint);
            _dynamicsWorld.RemoveConstraint(constraint);
        }

        //Register a BulletBehavior with the simulation callback
        public override void Register(BulletBehaviour behavior) {
            //Initialize the world and warn the user if it hasn't already been initialized
            if (!_initialized) {
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
        public override void Unregister(BulletBehaviour behavior) {
            //Warn the user if the physics world has not been initialized
            if (!_initialized) {
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
                DebugReadout = "[BulletDiscreteDynamicsWorld]\n" +
                               $"RenderDelta: {1000f * Time.deltaTime:n3}(ms)\n" +
                               $"PhysicsDelta: {1000f * bulletTimeStep:n3}(ms)\n" +
                               $"PhysicsBodies: {_bulletRigidBodies.Count}\n" +
                               $"SimCallback: {_bulletBehaviors.Count} Listeners\n";
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
