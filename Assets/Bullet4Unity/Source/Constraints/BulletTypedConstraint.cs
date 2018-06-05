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

        protected TypedConstraint _constraint;

        private void OnEnable() {
            if (_constraint == null) {
                InitializeConstraint();
                BulletPhysicsWorldManager.Register(_constraint);
            }
            else {
                _constraint.IsEnabled = true;
            }
        }

        private void OnDisable() {
            if (_constraint != null) {
                _constraint.IsEnabled = false;
            }
        }

        private void OnDestroy() {
            if (_constraint != null) {
                BulletPhysicsWorldManager.Unregister(_constraint);
                Dispose();
            }
        }

        protected abstract void InitializeConstraint();

        public virtual void Dispose() {
            GC.SuppressFinalize(this);

            _constraint?.Dispose();
            _constraint = null;
        }

        public TypedConstraint RawConstraint => _constraint;
    }
}

