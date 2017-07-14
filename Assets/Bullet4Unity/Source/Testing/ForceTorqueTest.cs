using Bullet4Unity;
using BulletSharp;
using UnityEngine;

public class ForceTorqueTest : BulletBehaviour {

	public float Force;

	private Vector2 _input;

	private void Update() {
		_input.x = Input.GetAxis("Horizontal");
		_input.y = Input.GetAxis("Vertical");
	}

	public override void BulletUpdate(DynamicsWorld world, float bulletTimeStep) {
		BRigidBody.ApplyImpulse(_input.x * Vector3.right * Force * bulletTimeStep);
		BRigidBody.ApplyImpulse(_input.y * Vector3.forward * Force * bulletTimeStep);
	}

	public override void OnContactAdded(CollisionObject other) {
	}
}
