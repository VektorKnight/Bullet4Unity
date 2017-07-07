using System;
using System.Collections;
using System.Collections.Generic;
using BulletSharp;
using Bullet4Unity;
using UnityEngine;

namespace Bullet4Unity {
	/// <summary>
	/// Abstract interop class for Bullet collision shapes and Unity
	/// //TODO: Add a method to update parameters in Editor which depend on Transform
	/// - Author: VektorKnight
	/// </summary>
	public abstract class BulletCollisionShape : MonoBehaviour, IDisposable {
		
		//Unity Inspector
		[Header("Debug View")]
		[SerializeField] protected bool DrawGizmo = true;
		[SerializeField] protected Color GizmoColor = Color.blue;
		
		//Protected Internal
		protected CollisionShape Shape;
		
		//Draw Gizmos
		protected abstract void OnDrawGizmosSelected();
		
		//Get Collision Shape
		public abstract CollisionShape GetCollisionShape();
		
		//IDisposable
		public void Dispose() {
			GC.SuppressFinalize(this);
			Dispose(true);
		}
		
		//Base Dispose Method
		protected virtual void Dispose(bool disposing) {
			if (Shape == null) return;
			Shape.Dispose();
			Shape = null;
		}
	}
}