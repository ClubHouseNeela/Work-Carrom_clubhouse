using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAdjuster : MonoBehaviour
{
	public SpriteRenderer rink;

	// Use this for initialization
	private void Awake()
	{
		Camera.main.orthographicSize = rink.bounds.size.x * Screen.height / Screen.width * 0.5f;
	}
}