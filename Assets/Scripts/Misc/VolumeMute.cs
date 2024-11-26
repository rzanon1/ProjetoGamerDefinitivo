using UnityEngine;
using System.Collections;

public class VolumeMute : MonoBehaviour {

	// Permite controlar o volume atual
    public float volume = 1.0f;

	void Start () {
		AudioListener.volume = volume;
	}
}
