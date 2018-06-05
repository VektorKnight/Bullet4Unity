using BulletSharp;
using BulletSharp.Math;
using UnityEngine;
using Quaternion = BulletSharp.Math.Quaternion;

namespace Bullet4Unity {
	/// <summary>
	/// Interop class for a Bullet Motion State
	/// Handles passing transform data between Unity and Bullet
	/// </summary>
	public class BulletMotionState : MotionState {

		private readonly Transform _transform;

		public BulletMotionState(Transform transform) {
			_transform = transform;
		}

		public delegate void GetTransformDelegate(out Matrix worldTrans);

		public delegate void SetTransformDelegate(ref Matrix m);

		//Set the initial rigidbody transform in bullet
		public override void GetWorldTransform(out Matrix worldTrans) {
			worldTrans = Matrix.AffineTransformation(1f, _transform.rotation.ToBullet(), _transform.position.ToBullet());
		}

		//Bullet callback updates Unity transform
		public override void SetWorldTransform(ref Matrix m) {
            UnityEngine.Vector3 position = BulletExtensionMethods.ExtractTranslationFromMatrix(ref m);
            UnityEngine.Quaternion rotation = BulletExtensionMethods.GetUnityRotationFromMatrix(ref m);

            _transform.SetPositionAndRotation(position, rotation);
		}
	}
}
