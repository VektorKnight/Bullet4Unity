using System;
using BulletSharp;
using UnityEngine;

namespace Bullet4Unity {
	/// <summary>
	/// Abstract interop class for Bullet collision shapes and Unity
	/// //TODO: Add a method to update parameters in Editor which depend on Transform
	/// - Author: VektorKnight
	/// </summary>
	[DisallowMultipleComponent]
	public abstract class BulletCollisionShape : MonoBehaviour, IDisposable {
		
		//Unity Inspector
		[SerializeField] protected bool DrawGizmo = true;
		[SerializeField] protected Color GizmoColor = new Color(0f, 0.125f, 1f, 1f);
		
		//Protected Internal
		protected CollisionShape Shape;
		
		#if UNITY_EDITOR
		//Draw Gizmos
		protected abstract void OnDrawGizmosSelected();
		#endif
		
		//Get Collision Shape
		public abstract CollisionShape GetCollisionShape();
		
		//Get Collision Shape Type
		public abstract CollisionShapeType GetShapeType();
		
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
	
	//Shape Type Enum
	public enum CollisionShapeType {Primitive, Compound, ConvexHull, StaticPlane, StaticMesh}
}