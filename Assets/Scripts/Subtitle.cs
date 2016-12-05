using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Subtitle : MonoBehaviour {

    private Text output = null;
	private SpeechToText speechToText = null;
	private string[] script = null;

	void Start(Page state, Text foo) {
		output = foo;
		script = state.lines;
		speechToText = new SpeechToText();
	}

	void OnError() {
		// TODO: Display error
	}

	public bool Active {
		get { return speechToText.IsListening; }
		set {
			if (value && !speechToText.IsListening) {
				speechToText.DetectSilence = true;
				speechToText.EnableWordConfidence = true;
				speechToText.EnableTimestamps = true;
				speechToText.SilenceThreshold = 0.03;
				speechToText.MaxAlternatives = 1;
				speechToText.EnableContinousRecognition = true;
				speechToText.EnableInterimResults = true;
				speechToText.OnError = OnError;
				speechToText.StartListening(OnRecognize);
				// TODO: Display state
			} else if (!value && speechToText.IsListening) {
				speechToText.StopListening();
				// TODO: Display state
			}
		}
	}
	
	private void OnRecognize(SpeechRecognitionEvent result) {
		if (result != null && result.results.Length > 0) {
			foreach (var res in result.results) {
				foreach (var alt in res.alternatives) {

					string[] detected = alt.transcript.Split (' ');
					string[] target = script.Split (' ');
					List<string> text = new List<string> { "<color=#000000ff>" };
					bool stopped = false;

					for (var i = 0; i < target.Length; i += 1) {
						if (!stopped && (i >= detected.Length || detected [i].ToLower() != target [i].ToLower())) {
							text.Add ("</color>");
							stopped = true;
						}

						text.Add(target [i]);
					}

					if (!stopped) {
						text.Add ("</color>");
					}

					output.text = String.Join(" ", text.ToArray());
					
				}
			}
		}
	}
}
