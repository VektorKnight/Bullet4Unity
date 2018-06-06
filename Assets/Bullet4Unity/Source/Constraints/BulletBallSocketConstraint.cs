using BulletSharp;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace Bullet4Unity {
    [AddComponentMenu("BulletPhysics/Constraints/BallSocketConstraint")]
    [RequireComponent(typeof(BulletRigidBody))]
    public sealed class BulletBallSocketConstraint : BulletConstraint {
        [Header("Ball & Socket")]

        [SerializeField]
        private bool _autoConfigurePivots = true;

        [SerializeField]
        private Vector3 _connectedPivot = Vector3.zero;

        [SerializeField]
        private Vector3 _localPivot = Vector3.zero;

        [SerializeField]
        private float _damping;

        [SerializeField]
        private float _impulseClamp;

        [SerializeField]
        private float _tau;

        //Draw Gizmo
#if UNITY_EDITOR
        private void OnDrawGizmosSelected() {
            if (_connectedBody == null)
                return;

            Vector3 localPivot = transform.TransformPointUnscaled(_localPivot);
            Vector3 connectedPivot = _connectedBody.transform.TransformPointUnscaled(_connectedPivot);

            Gizmos.color = GIZMO_COLOR;
            Gizmos.DrawLine(localPivot, connectedPivot);

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(localPivot, 0.05f);
            Gizmos.DrawWireSphere(connectedPivot, 0.05f);
        }

        protected override void OnValidate() {
            base.OnValidate();

            if (_autoConfigurePivots && _connectedBody != null) {
                Vector3 pivotOffset = (_connectedBody.transform.position - transform.position) * 0.5f;

                _localPivot = transform.InverseTransformVector(pivotOffset);
                _connectedPivot = _connectedBody.transform.InverseTransformVector(-pivotOffset);
            }
        }
#endif

        //Initialize and register the constraint
        protected override void InitializeConstraint() {
            _constraint = new Point2PointConstraint(_rigidbody.BodyInstance, _connectedBody.BodyInstance, _localPivot.ToBullet(), _connectedPivot.ToBullet());
        }

        public Vector3 LocalPivot {
            get {
                return _localPivot;
            }
            set {
                _localPivot = value;

                if (_constraint != null) {
                    ((Point2PointConstraint)_constraint).PivotInA = value.ToBullet();
                }
            }
        }

        public Vector3 ConnectedPivot {
            get {
                return _connectedPivot;
            }
            set {
                _connectedPivot = value;

                if (_constraint != null) {
                    ((Point2PointConstraint)_constraint).PivotInB = value.ToBullet();
                }
            }
        }

        public float Damping {
            get {
                return _damping;
            }
            set {
                _damping = value;
                ((Point2PointConstraint)_constraint).Setting.Damping = value;
            }
        }

        public float ImpulseClamp {
            get {
                return _impulseClamp;
            }
            set {
                _impulseClamp = value;
                ((Point2PointConstraint)_constraint).Setting.ImpulseClamp = value;
            }
        }

        public float Tau {
            get {
                return _tau;
            }
            set {
                _tau = value;
                ((Point2PointConstraint)_constraint).Setting.Tau = value;
            }
        }
    }
}
