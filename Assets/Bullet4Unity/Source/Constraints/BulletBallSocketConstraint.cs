using System;
using System.Collections;
using System.Collections.Generic;
using BulletSharp;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace Bullet4Unity {
	[AddComponentMenu("BulletPhysics/Constraints/BallSocketConstraint")]
	[RequireComponent(typeof(BulletRigidBody))]
	public class BulletBallSocketConstraint : BulletTypedConstraint {
		
		//Unity Inspector: Gizmos
		[SerializeField] private Color _errorColor = Color.red;
		
		//Unity Inspector: Constraint Config
		[Header("Constraint Config")]
		[Tooltip("Force impulse required to break the constraint, a force of zero makes it unbreakable")]
		[SerializeField] private float _breakingForce = 0f;
		[SerializeField] private BulletRigidBody _connectedBody;
		[SerializeField] private Vector3 _pivotA;
		[SerializeField] private Vector3 _pivotB;
		
		//Draw Gizmo
		#if UNITY_EDITOR
		protected override void OnDrawGizmos() {
			if (BRigidBody == null) BRigidBody = GetComponent<BulletRigidBody>();
			if (BRigidBody == null || _connectedBody == null) return;
			Gizmos.color = _errorColor;
			Gizmos.DrawLine(BRigidBody.transform.TransformPointUnscaled(_pivotA), _connectedBody.transform.TransformPointUnscaled(_pivotB));
			Gizmos.color = GizmoColor;
			Gizmos.DrawWireSphere(BRigidBody.transform.TransformPointUnscaled(_pivotA), 0.05f);
			Gizmos.DrawWireSphere(_connectedBody.transform.TransformPointUnscaled(_pivotB), 0.05f);
		}
		#endif
		
		//Initialize and register the constraint
		public override void InitializeConstraint() {
			if (Initialized) return;
			
			if (BRigidBody == null) BRigidBody = GetComponent<BulletRigidBody>();
			
			Constraint = new Point2PointConstraint(BRigidBody.RigidBody, _connectedBody.RigidBody, _pivotA.ToBullet(), _pivotB.ToBullet());
			if (_breakingForce > 0f) Constraint.BreakingImpulseThreshold = _breakingForce;
			
			Initialized = true;

			if (Registered) return;
			
			BulletWorldManager.Register(BRigidBody.GetWorldName(), Constraint);
			
			Registered = true;
		}
		
		//Initialize and/or register the constraint if needed
		protected override void OnEnable() {
			if (Disposing) return;
			
			if (!Initialized) InitializeConstraint();

			if (Registered) return;
			BulletWorldManager.Register(BRigidBody.GetWorldName(), Constraint);
			Registered = true;
		}
		
		//Unregister the constraint if needed
		protected override void OnDisable() {
			if (Disposing) return;

			if (!Registered) return;
			BulletWorldManager.Unregister(BRigidBody.GetWorldName(), Constraint);
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
