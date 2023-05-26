using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InspectionCameraFunctions : MonoBehaviour
{
    Camera cam;
	public float interactLength;
	public Image aimUI;
	public Text dialogText;
	bool mouseOverTextChanged;

	// Start is called before the first frame update
	void Start()
    {
        cam = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        Ray ray = cam.ScreenPointToRay(aimUI.transform.position);
		IMouseOver Imouse;
		if (Physics.Raycast(ray, out hit, interactLength))
		{ Imouse = hit.transform.GetComponent<IMouseOver>(); Debug.DrawLine(cam.ScreenToWorldPoint(aimUI.transform.position), hit.point); }
		else
		{
			Imouse = null; Debug.DrawLine(cam.ScreenToWorldPoint(aimUI.transform.position), ray.direction * interactLength + cam.ScreenToWorldPoint(aimUI.transform.position));

		}
		if (Imouse != null)
		{
			Imouse.MouseOver();
			dialogText.text = Imouse.returnWord;
			mouseOverTextChanged = true;

		}
		else
		{
			if (mouseOverTextChanged)
			{
				dialogText.text = null;
				mouseOverTextChanged = false;
			}//好麻烦 但也不知道怎么简化。。。。
		}
	}
}
