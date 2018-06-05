using System;
using System.Collections;
using System.Collections.Generic;
using BulletSharp;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace Bullet4Unity {
    [AddComponentMenu("BulletPhysics/Constraints/BallSocketConstraint")]
    [RequireComponent(typeof(BulletRigidBody))]
    public sealed class BulletBallSocketConstraint : BulletConstraint {
        [Header("Constraint Config")]

        [SerializeField]
        private BulletRigidBody _connectedBody;

        [SerializeField]
        private bool _autoConfigurePivots = true;

        [SerializeField]
        private Vector3 _connectedPivot = Vector3.zero;

        [SerializeField]
        private Vector3 _localPivot = Vector3.zero;

        [SerializeField]
        private float _breakingForce = 0f;

        private BulletRigidBody _localBody;

        //Draw Gizmo
#if UNITY_EDITOR
        private void OnDrawGizmosSelected() {
            if (_connectedBody == null)
                return;

            Gizmos.color = GIZMO_COLOR;
            Gizmos.DrawLine(transform.TransformPointUnscaled(_localPivot), _connectedBody.transform.TransformPointUnscaled(_connectedPivot));
            Gizmos.color = Color.black;
            Gizmos.DrawWireSphere(transform.TransformPointUnscaled(_localPivot), 0.05f);
            Gizmos.DrawWireSphere(_connectedBody.transform.TransformPointUnscaled(_connectedPivot), 0.05f);
        }

        private void OnValidate() {
            if (_autoConfigurePivots && _connectedBody != null) {
                Vector3 pivotOffset = (_connectedBody.transform.position - transform.position) * 0.5f;

                _localPivot = transform.InverseTransformVector(pivotOffset);
                _connectedPivot = _connectedBody.transform.InverseTransformVector(-pivotOffset);
            }
        }
#endif

        //Initialize and register the constraint
        protected override void InitializeConstraint() {
            _localBody = GetComponent<BulletRigidBody>();
            _constraint = new Point2PointConstraint(_localBody.BodyInstance, _connectedBody.BodyInstance, _localPivot.ToBullet(), _connectedPivot.ToBullet());

            if (_breakingForce > 0f)
                _constraint.BreakingImpulseThreshold = _breakingForce;
        }
    }
}
