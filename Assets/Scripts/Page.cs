using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State : MonoBehaviour {

	private List<string> lines;

	// Use this for initialization
	void Start (string[] newLines) {
		foreach (string line in newLines) {
            lines.Add(line);
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
