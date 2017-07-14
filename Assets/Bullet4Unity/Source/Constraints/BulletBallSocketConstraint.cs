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
			Debug.DrawLine(_bodyA.transform.position, _bodyB.transform.position, GizmoColor);
		}
		#endif

		public override void InitializeConstraint() {
			Constraint = new Point2PointConstraint(_bodyA.BRigidBody, _bodyB.BRigidBody, _pivotA.ToBullet(), _pivotB.ToBullet()) { };
			BulletPhysicsWorldManager.Register(Constraint);
		}

		public override TypedConstraint GetConstraint() {
			throw new System.NotImplementedException();
		}

		public override ConstraintType GetConstraintType() {
			throw new System.NotImplementedException();
		}
	}
}
