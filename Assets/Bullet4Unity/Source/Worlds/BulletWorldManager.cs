using System;
using System.Collections.Generic;
using BulletSharp;
using BulletSharp.SoftBody;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Bullet4Unity {
	/// <summary>
	/// New world manager class designed to work with multiple concurrent physics worlds
	/// </summary>
	[AddComponentMenu("BulletPhysics/Worlds/WorldManager")]
    [ScriptExecutionOrder(Order = -30000)]
	public class BulletWorldManager : MonoBehaviour, IDisposable {
		
		//Enum for Object Initialization
		public enum BulletObjectTypes {PhysicsBody, PhysicsConstraint, PhysicsBehaviour}

		//Delegate & Static Events for Initialization
		public delegate void BulletObjectHandler(BulletObjectTypes objectType);
		public static event BulletObjectHandler OnInitializeObjects;
		
		//Dictionary of active Physics Worlds <WorldName, WorldInstance>
        private static readonly Dictionary<string,BulletPhysicsWorld> BulletPhysicsWorlds = new Dictionary<string, BulletPhysicsWorld>();
	    
	    //Static Singleton Instance
		private static BulletWorldManager _instance;
	    private static BulletWorldManager Instance {
	        get {
	            //Locate/Initialize instance if missing. Acts as a failsafe in the event the user doesn't manually configure the physics system
	            if (_instance != null) return _instance;
	            //Look for pre-existing instance
	            _instance = FindObjectOfType<BulletWorldManager>();
	            if (_instance != null || !Application.isPlaying) return _instance;
	            var instanceObj = new GameObject("[BulletWorldManager]") {hideFlags = HideFlags.HideAndDontSave};
	            _instance = instanceObj.AddComponent<BulletWorldManager>();
	            return _instance;
	        }
	    }
		
		//Private Members
		private bool _initialized;
        private bool _disposing;
		private static bool _decommisioning;
		
		//Initialize the world manager
		private void Initialize() {
			//Ensure all transform properties are zeroed or set to identity
			transform.position = Vector3.zero;
			transform.rotation = Quaternion.identity;
			transform.localScale = Vector3.one;
			
			//Find all physics worlds
			var bulletWorlds = (BulletPhysicsWorld[])FindObjectsOfType(typeof(BulletPhysicsWorld));
			
			//Make sure there are worlds to intialize or smack the user
			if (bulletWorlds.Length == 0) {
				Debug.LogError("No physics worlds to initialize!\n" +
				               "Please add at least one Bullet Physics World to your scene");
				return;
			}
			
			//Initialize the physics worlds
			foreach (var world in bulletWorlds) {
				world.Initialize();
				BulletPhysicsWorlds.Add(world.GetWorldName(), world);
			}
			
			//Find all Physics Bodies and tell them to register with the Initalization event
			var bodies = (BulletPhysicsBody[]) FindObjectsOfType(typeof(BulletPhysicsBody));
			foreach (var body in bodies) {
				body.RegisterEvent();
			}
			
			//Find all Physics Constraints and tell them to register with the Initalization event
			var constraints = (BulletTypedConstraint[]) FindObjectsOfType(typeof(BulletTypedConstraint));
			foreach (var constraint in constraints) {
				constraint.RegisterEvent();
			}
			
			//Find all Physics Behaviours and tell them to register with the Initalization event
			var behaviours = (BulletBehaviour[]) FindObjectsOfType(typeof(BulletBehaviour));
			foreach (var behaviour in behaviours) {
				behaviour.RegisterEvent();
			}
			
			//Invoke the initialization events in proper order
			OnInitializeObjects?.Invoke(BulletObjectTypes.PhysicsBody);
			OnInitializeObjects?.Invoke(BulletObjectTypes.PhysicsConstraint);
			OnInitializeObjects?.Invoke(BulletObjectTypes.PhysicsBehaviour);
		}
		
		//Get world entry from the dictionary
		private static BulletPhysicsWorld GetWorldEntry(string worldName) {
			if (BulletPhysicsWorlds.ContainsKey(worldName)) {
				return BulletPhysicsWorlds[worldName];
			}
			
			//Throw an exception if the specified world name doesn't exist
			throw new InvalidWorldNameException($"{worldName} Does not refer to a valid physics world!\n" +
			                                    "Please check your configuration or use default world names.");
		}
		
		//Unity Pre-Initialization
		private void Awake() {
            //Enforce Single Instance
            if (_instance == null) _instance = this;
            else Destroy(this);
            
			//Initialize the Manager
			Initialize();
		}
		
		//Unity Per-Frame Update
		private void Update() {
            //Make sure we are in play mode and cleanup hasnt started
            if (!Application.isPlaying && _disposing) return;

			foreach (var world in BulletPhysicsWorlds) {
				world.Value.StepSimulation();
			}
		}

	    //Unity OnDestroy
		private void OnDestroy() {
            if (_disposing) return;

            Dispose();
		}
        
	    //Dispose
        public void Dispose() {
            if (_disposing)
                return;

            _disposing = true;
            _decommisioning = true;
	        
	        //Find all physics worlds
	        var bulletWorlds = (BulletPhysicsWorld[])FindObjectsOfType(typeof(BulletPhysicsWorld));
			
	        //Call Dispose() on the physics worlds
	        foreach (var world in bulletWorlds) {
		        world.Dispose();
	        }
        }

        // World registration
        private void Register_Internal(string worldName, BulletBehaviour behaviour) {
            GetWorldEntry(worldName).Register(behaviour);
        }

        private void Unregister_Internal(string worldName, BulletBehaviour behaviour) {
	        GetWorldEntry(worldName).Unregister(behaviour);
        }

        private void Register_Internal(string worldName, RigidBody rigidBody) {
	        GetWorldEntry(worldName).Register(rigidBody);
        }

        private void Unregister_Internal(string worldName, RigidBody rigidBody) {
	        GetWorldEntry(worldName).Unregister(rigidBody);
        }

        private void Register_Internal(string worldName, SoftBody softbody) {
	        GetWorldEntry(worldName).Register(softbody);
        }

        private void Unregister_Internal(string worldName, SoftBody softbody) {
	        GetWorldEntry(worldName).Unregister(softbody);
        }

	    private void Register_Internal(string worldName, TypedConstraint constraint) {
		    GetWorldEntry(worldName).Register(constraint);
	    }

	    private void Unregister_Internal(string worldName, TypedConstraint constraint) {
		    GetWorldEntry(worldName).Unregister(constraint);
	    }

        #region StaticAccessors
        //Register a BulletBehavior with the simulation callback
        public static void Register(string worldName, BulletBehaviour behavior) {
            if (_decommisioning)
                return;

            Instance.Register_Internal(worldName, behavior);
        }

        //Unregister a BulletBehavior from the simulation callback
        public static void Unregister(string worldName, BulletBehaviour behavior) {
            if (_decommisioning)
                return;

            Instance.Unregister_Internal(worldName, behavior);
        }

        //Register a RigidBody with the simulation callback
        public static void Register(string worldName, RigidBody rigidbody) {
            if (_decommisioning)
                return;

            Instance.Register_Internal(worldName, rigidbody);
        }

        //Unregister a RigidBody from the simulation callback
        public static void Unregister(string worldName, RigidBody rigidbody) {
            if (_decommisioning)
                return;

            Instance.Unregister_Internal(worldName, rigidbody);
        }

        //Register a SoftBody with the simulation callback
        public static void Register(string worldName, SoftBody softbody) {
            if (_decommisioning)
                return;

            Instance.Register_Internal(worldName, softbody);
        }

        //Unregister a SoftBody from the simulation callback
        public static void Unregister(string worldName, SoftBody softbody) {
            if (_decommisioning)
                return;

            Instance.Unregister_Internal(worldName, softbody);
        }
	    
	    //Register a Constraint with the world
	    public static void Register(string worldName, TypedConstraint constraint) {
	        if (_decommisioning) return;
	        
	        Instance.Register_Internal(worldName, constraint);
	    }
	    
	    //Unregister a constraint with the physics world
		public static void Unregister(string worldName, TypedConstraint constraint) {
			if (_decommisioning) return;
			Instance.Unregister_Internal(worldName, constraint);
		}
		#endregion StaticAccessors
    }
}
