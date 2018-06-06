using UnityEngine;
using BulletSharp;
using BulletSharp.Math;

namespace Bullet4Unity {
    [AddComponentMenu("BulletPhysics/Constraints/FixedConstraint")]
    public sealed class BulletFixedConstraint : BulletConstraint {

#if UNITY_EDITOR
        private void OnDrawGizmosSelected() {
            if (_connectedBody == null)
                return;

            Gizmos.color = GIZMO_COLOR;
            Gizmos.DrawLine(transform.position, _connectedBody.transform.position); 
        }
#endif

        protected override void InitializeConstraint() {
            Matrix frameA = _rigidbody.transform.localToWorldMatrix.ToBullet();
            Matrix frameB = _connectedBody.transform.localToWorldMatrix.ToBullet();

            _constraint = new FixedConstraint(_rigidbody.BodyInstance, _connectedBody.BodyInstance, frameA, frameB);// TODO: Actually make this fixed
        }
    }
}
