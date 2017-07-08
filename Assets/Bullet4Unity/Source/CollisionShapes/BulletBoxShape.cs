using System;
using System.Collections;
using System.Collections.Generic;
using BulletSharp;
using Bullet4Unity;
using UnityEngine;

namespace Bullet4Unity {
    /// <summary>
    /// Interop class for a Bullet box collision shape
    /// -Author: VektorKnight
    /// </summary>
    [AddComponentMenu("BulletPhysics/Collision/BoxShape")]
    public class BulletBoxShape : BulletCollisionShape {
        
        //Unity Inspector
        [Header("Shape Config")] 
        [SerializeField] private Vector3 _extents = new Vector3(0.5f, 0.5f, 0.5f);
        [SerializeField] private Vector3 _localScale = Vector3.one;
        
        //Draw Gizmos
        //TODO: Make this do stuff
        protected override void OnDrawGizmosSelected() {}
        
        //Get Collision shape
        public override CollisionShape GetCollisionShape() {
            if (Shape != null) return Shape;
            Shape = new BoxShape(_extents.ToBullet()) {LocalScaling = _localScale.ToBullet()};
            return Shape;
        }  
    }
}
