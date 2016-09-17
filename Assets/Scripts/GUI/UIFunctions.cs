using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Linq;
using Zenject;
using DG.Tweening;

namespace Algorithms
{

	//TODO this monolithic god-class is a horrible mess, and I should sort it out

    public class UIFunctions : MonoBehaviour {

        GameObject _startBtn;
        GameObject _quitBtn;
        GameObject _sliderGO;
        GameObject _quitPanel;
        GameObject _inputPanel;
        GameObject _inputString;
        GameObject _toggleHgt;
		GameObject _errorText;
		GameObject _startAnmBtn;
        GameObject _retryBtn;
        
        [Inject]
        Settings _settings = null;

    	Slider _sld;
		InputField _inpHgtCmp;
		Toggle _toggleHgtCmp;

		//Misc----
		Selectable[] _selecs;
		Toggle _txt2x;
		Toggle _txt3x;
		
		[PostInject]
    	void Init(CoroutinesManager crMng)
    	{
            _startBtn = _settings.Buttons.StartBtn;
            _quitBtn = _settings.Buttons.QuitBtn;
            _sliderGO = _settings.Sliders.SliderGO;
            _quitPanel = _settings.Panels.QuitPanel;
            _inputString = _settings.InputFields.InputString;
            _toggleHgt = _settings.Toggles.ToggleHgt;
			_inputPanel = _settings.Panels.InputPanel;
			_errorText = _settings.Texts.ErrorText;
			_startAnmBtn = _settings.Buttons.StartAnmBtn;
            _retryBtn = _settings.Buttons.RetryBtn;

            _sld = _sliderGO.GetComponent<Slider>();
			_inpHgtCmp = _inputString.GetComponent<InputField>();
			_toggleHgtCmp = _toggleHgt.GetComponent<Toggle>();
			_selecs = _sliderGO.GetComponentsInChildren<Selectable> (true);
			_selecs = _selecs.Where (sel => sel.gameObject.GetInstanceID () != _sliderGO.GetInstanceID ()).ToArray();
			_txt2x = _settings.Texts.SldText2x.GetComponent<Toggle>();
			_txt3x = _settings.Texts.SldText3x.GetComponent<Toggle>();

			EventManager.CameraMovementEnded += CameraMovementEndedHandler;
            EventManager.SceneEnded += SceneEndedHandler;
		}

    	public void TimeSpeed()
    	{
			if (_selecs.Length != 0) 
			{
				if (_sld.value == 1f) 
				{
					foreach (Selectable selec in _selecs)
						selec.interactable = true;
				}
				else if (_selecs [0].interactable != false) 
				{
					foreach (Selectable selec in _selecs)
						selec.interactable = false;
				}
			}
    		Time.timeScale = _sld.value;
    		Time.fixedDeltaTime = 0.02F * Time.timeScale;
    	}
		//TODO get rid of repetitions
		public void TimeSpeed2x()
		{
			if (_txt2x.isOn && !_txt3x.isOn)
			{
				Time.timeScale *= 2f;
				_txt3x.interactable = false;
			}
			else
			{
				Time.timeScale = 1f;
				_txt3x.interactable = true;
			}
			Time.fixedDeltaTime = 0.02F * Time.timeScale;
		}

		public void TimeSpeed3x()
		{
			if (_txt3x.isOn && !_txt2x.isOn)
			{
				Time.timeScale *= 3f;
				_txt2x.interactable = false;
			}
			else
			{
				Time.timeScale = 1f;
				_txt2x.interactable = true;
			}
			Time.fixedDeltaTime = 0.02F * Time.timeScale;
		}

    	public void TriggerStartGameEvent()
    	{

			if (GUIEventManager.StartButtonAction != null)
				GUIEventManager.StartButtonAction ();
			else 
				Debug.Log ("StartButtonAction event is empty");

    		_startBtn.SetActive (false);
    		_sliderGO.SetActive (true);
			_quitBtn.SetActive (true);
			_inputPanel.SetActive (true);
    	}

		public void BringQuittingPanel()
		{
			_quitPanel.SetActive (true);
		}

		public void HideQuittingPanel()
		{
			_quitPanel.SetActive (false);
		}

        public void TriggerQuitGameEvent()
        {
			if (GUIEventManager.QuitButtonAction != null)
				GUIEventManager.QuitButtonAction ();
			else 
				Debug.Log ("QuitButtonAction event is empty");
        }

		public void TriggerStartAnimEvent()
		{
			GUIEventManager.StartAnimEvent ();
			_startAnmBtn.SetActive (false);
		}

		//Input panel------------------------------------------------------

		public void OnToggleHgt()
		{
			
			if (_toggleHgtCmp.isOn)
			{
				_inpHgtCmp.text = "";
				_inpHgtCmp.interactable = false;
				
			}
			else
				_inpHgtCmp.interactable = true;
		}

		public void GenerateBtn()
		{
			if (GUIEventManager.GenerateButtonEvent != null) {
				if(!GUIEventManager.GenerateButtonEvent (_inpHgtCmp.text))
                {
					_errorText.SetActive(true);
                    _inputPanel.SetActive(true);
                }
                else
                {
					_errorText.SetActive(false);
                    _inputPanel.SetActive(false);
                }
            }
			else
			{
				Debug.Log ("GenBtn Event is empty");
			}
		}
        public void RetryBtn()
        {
            if (GUIEventManager.RetryBtnEvent != null)
                GUIEventManager.RetryBtnEvent();
            else
                Debug.Log("RetryBtnEvent event is empty");
            RectTransform retBtn = _retryBtn.GetComponent<RectTransform>();
            retBtn.DOAnchorPos(new Vector2(92, 0), 1f).
                OnComplete(()=>_retryBtn.SetActive(false));
            _inputPanel.SetActive(true);
        }
		//Event handlers
		void CameraMovementEndedHandler()
		{
			_startAnmBtn.SetActive (true);
		}
        void SceneEndedHandler()
        {
            _retryBtn.SetActive(true);
            RectTransform retBtn = _retryBtn.GetComponent<RectTransform>();
            retBtn.DOAnchorPos(new Vector2(-95, 0), 1f).SetDelay(1f);
        }

		#region Serializable classes-containers
		[Serializable]
		public class Settings
		{
			public ButtonsGO Buttons;
			public SlidersGO Sliders; 
			public PanelsGO Panels; 
			public InputFieldsGO InputFields; 
			public TogglesGO Toggles;
			public TextsGO Texts;
			
			[Serializable]
			public class ButtonsGO
			{
				public GameObject StartBtn;
				public GameObject QuitBtn;
				public GameObject StartAnmBtn;
                public GameObject RetryBtn;
			}
			[Serializable]
			public class SlidersGO
			{
				public GameObject SliderGO;
			}
			[Serializable]
			public class PanelsGO
			{
				public GameObject QuitPanel;
				public GameObject InputPanel;
			}
			[Serializable]
			public class InputFieldsGO
			{
				public GameObject InputString;
			}
			[Serializable]
			public class TogglesGO
			{
				public GameObject ToggleHgt;
			}
			[Serializable]
			public class TextsGO
			{
				public GameObject ErrorText;
				public GameObject SldText2x;
				public GameObject SldText3x;
			}
		}
		#endregion

    }
}