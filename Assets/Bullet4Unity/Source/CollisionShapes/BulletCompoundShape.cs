using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using BulletSharp;
using Bullet4Unity;
using BulletSharp.Math;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace Bullet4Unity {
    /// <summary>
    /// Interop class for a Bullet compound collision shape
    /// -Author: VektorKnight
    /// </summary>
    [AddComponentMenu("BulletPhysics/Collision/CompoundShape")]
    public class BulletCompoundShape : BulletCollisionShape {
        
        //Private Internal (Compound Shape Generation)
        private CompoundShape _compoundShape;
        private BulletSharp.Math.Matrix _childTransform;
        private List<BulletCollisionShape> _childShapes = new List<BulletCollisionShape>();

        //Generate and return an optimized Bullet ConvexHullShape
        private void GenerateCompoundCollider() {
            //Get all child collision shapes
            _childShapes = GetComponentsInChildren<BulletCollisionShape>().ToList();
            
            //Warn the user if no child colliders
            if (_childShapes.Count < 2) {
                Debug.LogError("Bullet Compound Shape requires at least two or more child collision shapes!\n" +
                               "Please add two or more primitive collision shapes as child objects.");
                return;
            }
            
            //Generate the compound collider
            _compoundShape = new CompoundShape() {LocalScaling = transform.localScale.ToBullet()};
            _childTransform = new Matrix();
            foreach (var child in _childShapes) {
                if (child == this) continue;
                _childTransform.Origin = child.transform.localPosition.ToBullet();
                _childTransform.Orientation = child.transform.rotation.ToBullet();
                _compoundShape.AddChildShape(_childTransform, child.GetCollisionShape());
            }
            
            //Assign the compound collider to inherited member "Shape"
            Shape = _compoundShape;
        }
        
        #if UNITY_EDITOR
        //Draw Shape Gizmo
        protected override void OnDrawGizmosSelected() {
        }
        #endif
        
        //Get Collision shape
        public override CollisionShape GetCollisionShape() {
            if (Shape != null) return Shape;
            GenerateCompoundCollider();
            return Shape;
        } 
        
        //Get Collision Shape Type
        public override CollisionShapeType GetShapeType() {
            return CollisionShapeType.Compound;
        }
        
        //Overriden Dispose Method
        protected override void Dispose(bool disposing) {
            if (_compoundShape == null) return;
            _compoundShape.Dispose();
            _compoundShape = null;
        }
    }
}
