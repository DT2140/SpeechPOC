using System;
using System.Collections.Generic;
using IBM.Watson.DeveloperCloud.DataTypes;
using IBM.Watson.DeveloperCloud.Services.SpeechToText.v1;
using IBM.Watson.DeveloperCloud.Logging;
using UnityEngine;
using UnityEngine.UI;
using IBM.Watson.DeveloperCloud.Utilities;

#pragma warning disable 414

namespace IBM.Watson.DeveloperCloud.Widgets
{
	public class NarratorWidget : Widget
	{

		#region Inputs
		[SerializeField]
		private Input m_AudioInput = new Input("Audio", typeof(AudioData), "OnAudio");
		[SerializeField]
		private Input m_LanguageInput = new Input("Language", typeof(LanguageData), "OnLanguage");
		#endregion

		#region Outputs
		[SerializeField]
		private Output m_ResultOutput = new Output(typeof(SpeechToTextData), true);
		#endregion

		#region Private Data
		private string script = "Once upon a time there was a hummingbird";
		private SpeechToText m_SpeechToText = new SpeechToText();

		[SerializeField]
		private Text m_StatusText = null;
		[SerializeField]
		private bool m_DetectSilence = true;
		[SerializeField]
		private float m_SilenceThreshold = 0.03f;
		[SerializeField]
		private bool m_WordConfidence = false;
		[SerializeField]
		private bool m_TimeStamps = false;
		[SerializeField]
		private int m_MaxAlternatives = 1;
		[SerializeField]
		private bool m_EnableContinous = false;
		[SerializeField]
		private bool m_EnableInterimResults = false;
		[SerializeField]
		private Text m_Transcript = null;
		[SerializeField, Tooltip("Language ID to use in the speech recognition model.")]
		private string m_Language = "en-US";
		#endregion

		#region Public Properties
		/// <summary>
		/// This property starts or stop's this widget listening for speech.
		/// </summary>
		public bool Active
		{
			get { return m_SpeechToText.IsListening; }
			set
			{
				if (value && !m_SpeechToText.IsListening)
				{
					m_SpeechToText.DetectSilence = m_DetectSilence;
					m_SpeechToText.EnableWordConfidence = m_WordConfidence;
					m_SpeechToText.EnableTimestamps = m_TimeStamps;
					m_SpeechToText.SilenceThreshold = m_SilenceThreshold;
					m_SpeechToText.MaxAlternatives = m_MaxAlternatives;
					m_SpeechToText.EnableContinousRecognition = m_EnableContinous;
					m_SpeechToText.EnableInterimResults = m_EnableInterimResults;
					m_SpeechToText.OnError = OnError;
					m_SpeechToText.StartListening(OnRecognize);
					if (m_StatusText != null)
						m_StatusText.text = "LISTENING";
				}
				else if (!value && m_SpeechToText.IsListening)
				{
					m_SpeechToText.StopListening();
					if (m_StatusText != null)
						m_StatusText.text = "READY";
				}
			}
		}
		#endregion

		#region Widget Interface
		/// <exclude />
		protected override string GetName()
		{
			return "SpeechToText";
		}
		#endregion

		#region Event handlers
		/// <summary>
		/// Button handler to toggle the active state of this widget.
		/// </summary>
		public void OnListenButton()
		{
			Active = !Active;
		}

		/// <exclude />
		protected override void Start()
		{
			base.Start();

			m_Transcript.text = script;

			if (m_StatusText != null)
				m_StatusText.text = "READY";
			if (!m_SpeechToText.GetModels(OnGetModels))
				Log.Error("SpeechToTextWidget", "Failed to request models.");
		}

		private void OnDisable()
		{
			if (Active)
				Active = false;
		}

		private void OnError(string error)
		{
			Active = false;
			if (m_StatusText != null)
				m_StatusText.text = "ERROR: " + error;
		}

		private void OnAudio(Data data)
		{
			if (!Active)
				Active = true;

			m_SpeechToText.OnListen((AudioData)data);
		}

		private void OnLanguage(Data data)
		{
			LanguageData language = data as LanguageData;
			if (language == null)
				throw new WatsonException("Unexpected data type");

			if (!string.IsNullOrEmpty(language.Language))
			{
				m_Language = language.Language;

				if (!m_SpeechToText.GetModels(OnGetModels))
					Log.Error("SpeechToTextWidget", "Failed to rquest models.");
			}
		}

		private void OnGetModels(Model[] models)
		{
			if (models != null)
			{
				Model bestModel = null;
				foreach (var model in models)
				{
					if (model.language.StartsWith(m_Language)
						&& (bestModel == null || model.rate > bestModel.rate))
					{
						bestModel = model;
					}
				}

				if (bestModel != null)
				{
					Log.Status("SpeechToTextWidget", "Selecting Recognize Model: {0} ", bestModel.name);
					m_SpeechToText.RecognizeModel = bestModel.name;
				}
			}
		}

		private void OnRecognize(SpeechRecognitionEvent result)
		{
			m_ResultOutput.SendData(new SpeechToTextData(result));

			if (result != null && result.results.Length > 0)
			{
				foreach (var res in result.results)
				{
					foreach (var alt in res.alternatives)
					{

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

						m_Transcript.text = String.Join(" ", text.ToArray());
					}
				}
			}
		}
		#endregion
	}
}