using System;
using System.Collections;
using System.Collections.Generic;
using BulletSharp;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace Bullet4Unity {
	[AddComponentMenu("BulletPhysics/Constraints/BallSocketConstraint")]
	public class BulletBallSocketConstraint : BulletTypedConstraint {
		
		//Unity Inspector
		[Header("Constraint Config")]
		[SerializeField] private BulletRigidBody _bodyA;
		[SerializeField] private BulletRigidBody _bodyB;
		[SerializeField] private Vector3 _pivotA;
		[SerializeField] private Vector3 _pivotB;
		
		//Draw Gizmo
		#if UNITY_EDITOR
		protected override void OnDrawGizmos() {
			if (_bodyA == null || _bodyB == null) return;
			Gizmos.color = GizmoColor;
			Gizmos.DrawLine(_bodyA.transform.TransformPoint(_pivotA), _bodyB.transform.TransformPoint(_pivotB));
			Gizmos.color = Color.black;
			Gizmos.DrawWireSphere(_bodyA.transform.TransformPoint(_pivotA), 0.05f);
			Gizmos.DrawWireSphere(_bodyB.transform.TransformPoint(_pivotB), 0.05f);
		}
		#endif
		
		//Initialize and register the constraint
		public override void InitializeConstraint() {
			if (Initialized) return;
			Constraint = new Point2PointConstraint(_bodyA.BRigidBody, _bodyB.BRigidBody, _pivotA.ToBullet(), _pivotB.ToBullet());
			Initialized = true;

			if (Registered) return;
			BulletPhysicsWorldManager.Register(Constraint);
			Registered = true;
		}
		
		//Initialize and/or register the constraint if needed
		protected override void OnEnable() {
			if (Disposing) return;
			
			if (!Initialized) InitializeConstraint();

			if (Registered) return;
			BulletPhysicsWorldManager.Register(Constraint);
			Registered = true;
		}
		
		//Unregister the constraint if needed
		protected override void OnDisable() {
			if (Disposing) return;

			if (!Registered) return;
			BulletPhysicsWorldManager.Unregister(Constraint);
			Registered = false;
		}

		public override TypedConstraint GetConstraint() {
			throw new System.NotImplementedException();
		}

		public override ConstraintType GetConstraintType() {
			throw new System.NotImplementedException();
		}
	}
}
