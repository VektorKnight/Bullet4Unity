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
		[SerializeField] protected Color GizmoColor = new Color(0f, 0.75f, 1f, 1f);
		
		//Protected Internal
		protected TypedConstraint Constraint;
		protected bool Initialized;
		protected bool Disposing;
		
		//Initialize the constraint
		public abstract void InitializeConstraint();
		
		//Get Constraint
		public abstract TypedConstraint GetConstraint();
		
		//Get Constraint Type
		public abstract ConstraintType GetConstraintType();
		
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
			if (Constraint == null) return;
			Constraint.Dispose();
			Constraint = null;
		}
	}
	
	//Constraint Type Enum
	public enum ConstraintType {BallSocket, Hinge, Slider, ConeTwist, SixDof}
}

