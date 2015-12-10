using UnityEngine;
using System.Collections;

public class NoSlideJump : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Destroy(GetComponentInChildren<EdgeCollider2D>());
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
