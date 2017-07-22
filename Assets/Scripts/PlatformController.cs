using UnityEngine;			// To inherit from MonoBehaviour 

public class PlatformController : MonoBehaviour {

	public float PWidth;

	// Creates a platform with size parameter for scale of middle piece
	public void CreatePlatform(int size) {
		// Load the objects from resources
		GameObject pLeft	= Resources.Load<GameObject>("Components/PLeft");
		GameObject pMid 	= Resources.Load<GameObject>("Components/PMid");
		GameObject pRight 	= Resources.Load<GameObject>("Components/PRight");
		// Instantiate the objects into the scene with parent gameobject
		pLeft 	= Instantiate(pLeft ,gameObject.transform) as GameObject;
		pMid 	= Instantiate(pMid  ,gameObject.transform) as GameObject;
		pRight 	= Instantiate(pRight,gameObject.transform) as GameObject;
		// Calculate transform variables. Offset is to make right edge of platform at x = 0 for easier spawning
		pMid.transform.localScale = new Vector3(pMid.transform.localScale.x * size,pMid.transform.localScale.y,pMid.transform.localScale.z);
		float pMidX = (pLeft.GetComponent<MeshRenderer>().bounds.size.x / 2f) + (pMid.GetComponent<MeshRenderer>().bounds.size.x / 2f);
		float pRightX = 2f * pMidX;
		float _offsetX = - pRightX - (pRight.GetComponent<MeshRenderer>().bounds.size.x / 2f);
		// Finalize transforms
		pLeft.transform.localPosition += Vector3.right * _offsetX;
		pMid.transform.localPosition += Vector3.right * (pMidX + _offsetX);
		pRight.transform.localPosition += Vector3.right * (pRightX + _offsetX);
		// Set name for collision check
		pLeft.name = "Platform";
		pMid.name = "Platform";
		pRight.name = "Platform";

		// Set public vars
		PWidth = 	pLeft.GetComponent<MeshRenderer>().bounds.size.x + 
					pMid.GetComponent<MeshRenderer>().bounds.size.x + 
					pRight.GetComponent<MeshRenderer>().bounds.size.x;
	}
	
	// Runs every frame
	void Update () {
		if(!GameController.Lost) {
			transform.localPosition -= Vector3.right * GameController.SpeedMult;
		}
	}

}
