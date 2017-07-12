using BulletSharp;
using UnityEngine;

namespace Bullet4Unity {
    /// <summary>
    /// Interop class for a Bullet infinite static plane collision shape
    /// -Author: VektorKnight
    /// </summary>
    [AddComponentMenu("BulletPhysics/Collision/StaticPlaneShape")]
    [RequireComponent(typeof(BulletStaticBody))]
    public class BulletStaticPlaneShape : BulletCollisionShape {
       
        #if UNITY_EDITOR
        //Draw Shape Gizmo
        private static Mesh _shapeMesh;
        protected override void OnDrawGizmosSelected() {
            if (!DrawGizmo) return;
            if (_shapeMesh == null) _shapeMesh = UnityEditor.AssetDatabase.LoadAssetAtPath<Mesh>("Assets/Bullet4Unity/Resources/Primitives/BulletCube.fbx");
            Gizmos.color = GizmoColor;
            Gizmos.DrawMesh(_shapeMesh, transform.position - (Vector3.one * 0.01f), transform.rotation, new Vector3(1000f, 0f, 1000f));
        }
        #endif

        //Get Collision shape
        public override CollisionShape GetCollisionShape() {
            if (Shape != null) return Shape;
            Shape = new StaticPlaneShape(transform.up.ToBullet(), 1f);
            return Shape;
        }
        
        //Get Collision Shape Type
        public override CollisionShapeType GetShapeType() {
            return CollisionShapeType.StaticPlane;
        }
    }
}
