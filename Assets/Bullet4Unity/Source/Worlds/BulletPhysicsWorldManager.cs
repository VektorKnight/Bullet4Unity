using System;
using System.Collections;
using System.Collections.Generic;
using Bullet4Unity;
using BulletSharp;
using BulletSharp.SoftBody;
using UnityEngine;
using UnityEngine.Profiling;

namespace Bullet4Unity {
	/// <summary>
	/// Basic Bullet physics world implementation for Unity
	/// Currently only implements a Discrete Dynamics World
	/// Based on Page 6 of the documentation located at:
	/// https://github.com/bulletphysics/bullet3/blob/master/docs/BulletQuickstart.pdf
	/// TODO: Implement interop code for inspector-friendly components
	/// -Authors: VektorKnight, Techgeek1
	/// </summary>
	[AddComponentMenu("BulletPhysics/Worlds/PhysicsWorldManager")]
    [ScriptExecutionOrder(Order = -30000)] //ITS LESS THAN -9000!!!!
	public class BulletPhysicsWorldManager : MonoBehaviour, IDisposable {

        [SerializeField]
        private BulletDiscreteDynamicsWorld _discretePhysicsWorld = new BulletDiscreteDynamicsWorld();

        private bool _disposing = false;

		//Unity Pre-Initialization
		private void Awake() {
            //Enforce Single Instance
            if (_instance == null) {
                _instance = this;
            }
            else {
                Destroy(this);
            }
            
		    //Initialize the physics world
            _discretePhysicsWorld.Initialize();
		}
		
		//Unity Per-Frame Update
		private void Update() {
            //Make sure we are in play mode and cleanup hasnt started
            if (!Application.isPlaying && _disposing) return;

            _discretePhysicsWorld.StepSimulation();
		}
	    
	    //Unity OnGUI
	    private void OnGUI() {
	        if (_discretePhysicsWorld.DebugReadout == string.Empty) return;
	        var labelRect = new Rect(2f, 0f, 256f, 512f);
	        GUI.color = Color.yellow;
	        GUI.Label(labelRect, _discretePhysicsWorld.DebugReadout);
	    }

	    //Unity Destroy
		private void OnDestroy() {
            if (_disposing)
                return;

            Dispose();
		}
        
	    //Dispose
        public void Dispose() {
            if (_disposing)
                return;

            _disposing = true;
            _decommisioning = true;
            _discretePhysicsWorld.Dispose();
        }

        // World registration
        private void Register_Internal(BulletBehavior behaviour) {
            // Select the world and register with it
            _discretePhysicsWorld.Register(behaviour);
        }

        private void Unregister_Internal(BulletBehavior behaviour) {
            // Select the world and unregister with it
            _discretePhysicsWorld.Unregister(behaviour);
        }

        private void Register_Internal(RigidBody rigidBody) {
            // Select the world and register with it
            _discretePhysicsWorld.Register(rigidBody);
        }

        private void Unregister_Internal(RigidBody rigidBody) {
            // Select the world and unregister with it
            _discretePhysicsWorld.Unregister(rigidBody);
        }

        private void Register_Internal(SoftBody softbody) {
            // Select the world and register with it
            _discretePhysicsWorld.Register(softbody);
        }

        private void Unregister_Internal(SoftBody softbody) {
            // Select the world and unregister with it
            _discretePhysicsWorld.Unregister(softbody);
        }

        #region StaticAccessors

        //Register a BulletBehavior with the simulation callback
        public static void Register(BulletBehavior behavior) {
            if (_decommisioning)
                return;

            Instance.Register_Internal(behavior);
        }

        //Unregister a BulletBehavior from the simulation callback
        public static void Unregister(BulletBehavior behavior) {
            if (_decommisioning)
                return;

            Instance.Unregister_Internal(behavior);
        }

        //Register a RigidBody with the simulation callback
        public static void Register(RigidBody rigidbody) {
            if (_decommisioning)
                return;

            Instance.Register_Internal(rigidbody);
        }

        //Unregister a RigidBody from the simulation callback
        public static void Unregister(RigidBody rigidbody) {
            if (_decommisioning)
                return;

            Instance.Unregister_Internal(rigidbody);
        }

        //Register a SoftBody with the simulation callback
        public static void Register(SoftBody softbody) {
            if (_decommisioning)
                return;

            Instance.Register_Internal(softbody);
        }

        //Unregister a SoftBody from the simulation callback
        public static void Unregister(SoftBody softbody) {
            if (_decommisioning)
                return;

            Instance.Unregister_Internal(softbody);
        }

        //Static Singleton Instance
        private static BulletPhysicsWorldManager Instance {
            get
            {
                // Locate/Initialize instance if missing. Acts as a failsafe in the event the user doesn't manually configure the physics system
                if (_instance != null) return _instance;
                // Look for preexisting instance
                _instance = FindObjectOfType<BulletPhysicsWorldManager>();
                if (_instance != null || !Application.isPlaying) return _instance;
                var instanceObj = new GameObject("[BulletPhysicsWorldManager]") {hideFlags = HideFlags.HideAndDontSave};
                _instance = instanceObj.AddComponent<BulletPhysicsWorldManager>();
                return _instance;
            }
        }
        private static BulletPhysicsWorldManager _instance = null;

        private static bool _decommisioning = false;

        #endregion StaticAccessors
    }
}
