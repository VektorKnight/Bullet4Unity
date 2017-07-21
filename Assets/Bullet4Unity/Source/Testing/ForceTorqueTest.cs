using Bullet4Unity;
using BulletSharp;
using UnityEngine;

public class ForceTorqueTest : BulletBehaviour {

	public float Force;

	private Vector2 _input;
	private BulletRigidBody _rigidBody;

	private void Start() {
		_rigidBody = GetComponent<BulletRigidBody>();
	}

	private void Update() {
		_input.x = Input.GetAxis("Horizontal");
		_input.y = Input.GetAxis("Vertical");
	}

	public override void BulletUpdate(DynamicsWorld world, float bulletTimeStep) {
		_rigidBody.ApplyImpulse(_input.x * Vector3.right * Force * bulletTimeStep);
		_rigidBody.ApplyImpulse(_input.y * Vector3.forward * Force * bulletTimeStep);
	}

	public override void OnContactAdded(CollisionObject other) {
	}
}
