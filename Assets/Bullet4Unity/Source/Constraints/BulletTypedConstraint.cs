using System;
using System.Collections;
using System.Collections.Generic;
using BulletSharp;
using Bullet4Unity;
using UnityEngine;

namespace Bullet4Unity {
	/// <summary>
	/// Abstract base class for Bullet Typed Constraints
	/// -Author: VektorKnight
	/// </summary>
	public abstract class BulletTypedConstraint : MonoBehaviour {
		//Unity Inspector
		[SerializeField] protected bool DrawGizmo = true;
		[SerializeField] protected Color GizmoColor = Color.black;
		
		//Protected Internal
		protected BulletRigidBody BRigidBody;
		protected TypedConstraint Constraint;
		protected bool Initialized;
		protected bool Registered;
		protected bool Disposing;
		
		//Initialize the constraint
		private void Initialize(BulletWorldManager.BulletObjectTypes objectType) {
			if (objectType == BulletWorldManager.BulletObjectTypes.PhysicsConstraint) InitializeConstraint();
		}
		
		public abstract void InitializeConstraint();
		
		//Get Constraint
		public abstract TypedConstraint GetConstraint();
		
		//Get Constraint Type
		public abstract ConstraintType GetConstraintType();
		
		public void RegisterEvent() {
			BulletWorldManager.OnInitializeObjects += Initialize;
		}

		//Unity OnEnable
		protected abstract void OnEnable();
		
		//Unity OnDisable
		protected abstract void OnDisable();
		
		//Unity OnDestroy
		private void OnDestroy() {
			if (!Registered) return;
			BulletWorldManager.Unregister(BRigidBody.GetWorldName(), Constraint);
			Dispose();
		}

		#if UNITY_EDITOR
		//Draw Gizmos
		protected abstract void OnDrawGizmos();
		#endif
		
		//IDisposable
		public void Dispose() {
			GC.SuppressFinalize(this);
			Dispose(true);
		}
		
		//Base Dispose Method
		protected virtual void Dispose(bool disposing) {
			Constraint?.Dispose();
			Constraint = null;
		}
	}
	
	//Constraint Type Enum
	public enum ConstraintType {BallSocket, Hinge, Slider, ConeTwist, SixDof}
}

