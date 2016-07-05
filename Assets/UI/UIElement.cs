using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// Base class for all UI elements used for edition of upper layer elements.
public class UIElement : MonoBehaviour {

	public Sprite loadingIcon;

	public Sprite tickIcon;

	public Sprite errorIcon;

	public Image statusImage;

	public GameObject Beacon_Prefab;

	public enum UIStatus { Loading, OK, Error };

	// Sets the progress status of this element, updating its UI icon.
	public void setStatus(UIStatus status) {

		switch (status) {

		case UIStatus.Loading:
			statusImage.sprite = loadingIcon;
			break;

		case UIStatus.OK:
			statusImage.sprite = tickIcon;
			break;

		case UIStatus.Error:
			statusImage.sprite = errorIcon;
			break;
		}
	}
}
