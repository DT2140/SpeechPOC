using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoryState : MonoBehaviour {

	[SerializeField]
	private int currentPage = 0;

	[SerializeField]
	private Text textOutput = null;

	[SerializeField]
	private Subtitle subtitle = null;

	private Page[] pages;
	private string[][] script = {
		{ "Once upon a time there was a hummingbird" },
		{
			"One day the wind made the tree shake really hard",
			"The tree shook so hard that the hummingbird fell out of its nest"
		}
	};

	// Use this for initialization
	void Start () {
		foreach (string[] lines in script) {
			pages.Add(new Page(lines));
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public bool Page {
      get { return pages[value]; }
      set {
		  currentPage = value;
		  subtitle.Active = false;
		  subtitle = new Subtitle(pages[currentPage], textOutput);
	  }
	}
}
