using UnityEngine;

public class Button02 : MonoBehaviour {

	public float RealScale;
	public float CheckScale;

	void Awake () {
		name = name.Substring(0, name.Length-7);
		RealScale = Resources.Load<GameObject>("02/Prefabs/" + name).GetComponent<RectTransform>().localScale.x;
		CheckScale = RealScale/2f;
	}

}
