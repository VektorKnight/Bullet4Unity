using UnityEngine;

public class SphereStackSpawner : MonoBehaviour {

	public GameObject SpherePrefab;
	public float Margin = 1.1f;
	public int Count = 100;

	// Use this for initialization
	void Start () {
		Instantiate(SpherePrefab, transform.position, Quaternion.identity);

		for (var i = 1; i < Count; i++) {
			Instantiate(SpherePrefab, transform.position + (Margin * i * Vector3.up) + Random.insideUnitSphere, Quaternion.identity);
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
