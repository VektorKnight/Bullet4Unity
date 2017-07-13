using Bullet4Unity;
using BulletSharp;
using UnityEngine;

public class ForceTorqueTest : BulletBehavior {

	public Vector3 Force;
	public Vector3 Torque;

	private BulletRigidBody _rigidBody;

	// Use this for initialization
	void Start () {
		_rigidBody = GetComponent<BulletRigidBody>();
	}

	public override void BulletUpdate(DynamicsWorld world, float bulletTimeStep) {
		_rigidBody.ApplyForce(Force);
		_rigidBody.ApplyTorque(Torque);
	}

	public override void OnContactAdded(CollisionObject other) {
		Debug.Log("TestingBlarg");
	}
}
