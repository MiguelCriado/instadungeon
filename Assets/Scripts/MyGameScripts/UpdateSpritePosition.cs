using UnityEngine;
using System.Collections;

public class UpdateSpritePosition : MonoBehaviour {

	private float yBase;

	// Use this for initialization
	void Start () {
		yBase = transform.localPosition.y;
	}
	
	// Update is called once per frame
	void Update () {
		//updateHeight();
		updatePositionInLayer();
	}

	void FixedUpdate() {

	}

	void updateHeight() {
		Vector3 height = transform.localPosition;
		float zPos = this.transform.parent.transform.position.z;
		height.y = yBase +  zPos;
		transform.localPosition = height;
	}

	void updatePositionInLayer() {
		SpriteRenderer renderer = (SpriteRenderer)gameObject.GetComponent("SpriteRenderer");
		renderer.sortingOrder = Mathf.FloorToInt((transform.position.y - transform.position.z) * -100);
	}
}
