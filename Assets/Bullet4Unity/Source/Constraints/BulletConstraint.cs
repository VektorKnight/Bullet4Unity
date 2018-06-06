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
    public abstract class BulletConstraint : MonoBehaviour {
        protected static readonly Color GIZMO_COLOR = new Color(1f, 0.33f, 0f, 1f);

        [Header("Constraint")]

        [Tooltip("Body connected to this constraint")]
        [SerializeField]
        protected BulletRigidBody _connectedBody;

        [Tooltip("Force required to break this constraint")]
        [SerializeField]
        protected float _breakingForce = 0f;

        protected BulletRigidBody _rigidbody;
        protected TypedConstraint _constraint;

#if UNITY_EDITOR
        protected virtual void OnValidate() {
            if (Application.isPlaying && _constraint != null) {
                InitializeConstraintProperties();
            }
        }
#endif

        protected virtual void Awake() {
            _rigidbody = GetComponent<BulletRigidBody>();
        }

        protected virtual void OnEnable() {
            if (_constraint == null) {
                InitializeConstraint();
                InitializeConstraintProperties();

                BulletPhysicsWorldManager.Register(_constraint);
            }
            else {
                _constraint.IsEnabled = true;
            }
        }

        protected virtual void OnDisable() {
            if (_constraint != null) {
                _constraint.IsEnabled = false;
            }
        }

        protected virtual void OnDestroy() {
            if (_constraint != null) {
                BulletPhysicsWorldManager.Unregister(_constraint);
                Dispose();
            }
        }

        public virtual void Dispose() {
            GC.SuppressFinalize(this);

            _constraint?.Dispose();
            _constraint = null;
        }

        protected abstract void InitializeConstraint();

        /// <summary>
        /// Called after constraint initialization. Constraint is assumed to be valid at this time
        /// </summary>
        protected virtual void InitializeConstraintProperties() {
            _constraint.BreakingImpulseThreshold = (BreakingForce > 0f) ? _breakingForce : float.PositiveInfinity;
        }

        public TypedConstraint RawConstraint => _constraint;

        public int OverrideNumSolverIterations {
            get {
                return _constraint.OverrideNumSolverIterations;
            }
            set {
                _constraint.OverrideNumSolverIterations = value;
            }
        }

        public JointFeedback JointFeedback {
            get {
                return _constraint.JointFeedback;
            }
            set {
                _constraint.JointFeedback = value;
            }
        }

        public float BreakingForce {
            get {
                return _breakingForce;
            }
            set {
                _breakingForce = value;
                _constraint.BreakingImpulseThreshold = value;
            }
        }

        public float AppliedImpulse {
            get {
                return _constraint.AppliedImpulse;
            }
        }
    }
}

