using BulletSharp;
using UnityEngine;

namespace Bullet4Unity {
    /// <summary>
    /// Interop class for a Bullet sphere collision shape
    /// -Author: VektorKnight
    /// </summary>
    [AddComponentMenu("BulletPhysics/Collision/SphereShape")]
    public class BulletSphereShape : BulletCollisionShape {
        
        //Unity Inspector
        [Header("Shape Config")] 
        [SerializeField] private float _radius = 0.5f;
        
        #if UNITY_EDITOR
        //Draw Shape Gizmo
        protected override void OnDrawGizmosSelected() {
            if (!DrawGizmo) return;
            Gizmos.color = GizmoColor;
            Gizmos.DrawWireSphere(transform.position, _radius * Mathf.Max(Mathf.Max(transform.localScale.x, transform.localScale.y), transform.localScale.z));
        }
        #endif
        
        //Get Collision shape
        public override CollisionShape GetCollisionShape() {
            if (Shape != null) return Shape;
            Shape = new SphereShape(_radius) {LocalScaling = transform.localScale.ToBullet()};
            return Shape;
        }  
        
        //Get Collision Shape Type
        public override CollisionShapeType GetShapeType() {
            return CollisionShapeType.Primitive;
        }
    }
}
