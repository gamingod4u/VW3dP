using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class T9Button : MonoBehaviour {
	
	public bool selected;
	public float selectSpeed;
	public string btnType;
	public int t9Digit;

	private Renderer _progressRender;
	private GameObject _progressBar;
	private Vector3 _progressScale;
	
	private bool _btnEndReached;
	private string _btnText;
	private int _btnIdx;

	private GameObject _inputData;
	private InputField _inputField;
	private Text _txtLabel;

	private bool t9ModeEnabled;

	private T9 _t9;

	// Use this for initialization
	void Start () {
		Transform t = gameObject.transform.Find ("progress");
		_progressBar = t.gameObject;
		_progressRender = t.GetComponent<Renderer> ();
		_progressScale = t.localScale;

		_inputData = GameObject.Find ("InputField");
		_inputField = _inputData.GetComponent<InputField>();

		GameObject o = GameObject.Find ("T9");
		_t9 = o.GetComponent<T9>();

		o = GameObject.Find (gameObject.name + "/Text");
		_txtLabel = o.GetComponent<Text> ();

		if (selectSpeed == 0)
			selectSpeed = 0.25f;

		_btnText = btnType.ToUpper();

		t9ModeEnabled = true;
		_btnIdx = -2;

		hideProgressBar ();
	}
	
	public void lookingAt(bool state)
	{
		if (state == true) {
			showProgressBar ();
			if(_t9.getCurrentT9Word().Length == _t9.getCurrentSpelledWord().Length)
				t9ModeEnabled = true;
			else
				t9ModeEnabled = false;

			if (t9ModeEnabled)
				_btnIdx = -2;
			else
				_btnIdx = -1;

			selected = true;
			_btnEndReached = false;
		} else {
//			addCurrentLetter();

			_txtLabel.text = _btnText;

			hideProgressBar ();
			selected = false;
		}
	}

	private void addCurrentLetter()
	{
		if (_btnIdx == -1) {
			_t9.t9text += t9Digit.ToString();
		}else if (_btnIdx >= 0) {
			Debug.Log ("Adding: " + btnType.Substring(_btnIdx,1));
			_t9.spellingtext = _t9.spellingtext + btnType.Substring(_btnIdx,1);
		}

		if (t9ModeEnabled)
			_t9.updateT9Suggestions ();
		else
			_t9.setSearchText ();
	}

	private void removeLastLetter()
	{
		string strT9 = _t9.getCurrentT9Word ();
		string strSpell = _t9.getCurrentSpelledWord ();

		if (strT9.Length == strSpell.Length) {
			_t9.t9text = _t9.t9text.Remove (_t9.t9text.Length - 1);
		}

		_t9.spellingtext = _t9.spellingtext.Remove (_t9.spellingtext.Length - 1);

//		if (strT9.Length == strSpell.Length)
			_t9.updateT9Suggestions();
	}

	private void keyPressed()
	{
		if (btnType == "[OK]") {
			_t9.acceptSuggestion(0);
			_t9.updateT9Suggestions();

		}else if (btnType == "[DEL]") {
			if (_t9.spellingtext.Length == 0) {
				_btnEndReached = true;
				return;
			}

			removeLastLetter();

		}else if (btnType == "SPACE") {
			_txtLabel.text = "<color=red>" + _txtLabel.text + "</color>";

			_t9.t9text += " ";
			_t9.spellingtext += " ";

			_t9.updateT9Suggestions();

			_btnEndReached = true;
			t9ModeEnabled = true;
			_btnIdx = -2;
		} else {
			_btnIdx++;


			if (_btnIdx >= 0)
			{
				if (t9ModeEnabled || (!t9ModeEnabled && _btnIdx > 0))
				{
					removeLastLetter();
				}
				t9ModeEnabled = false;

				addCurrentLetter();
				_txtLabel.text = _btnText.Substring (0, _btnIdx) + "<color=red>" + _btnText.Substring (_btnIdx, 1) + "</color>" + _btnText.Substring (_btnIdx + 1);

			}else{
				addCurrentLetter();
				_txtLabel.text = "<color=red>" + _btnText + "</color>";
			}

			if (_btnIdx == (btnType.Length - 1)) {
				// End reached
//				addCurrentLetter();
				_btnEndReached = true;
				t9ModeEnabled = false;
//				_btnIdx = -1;
			}
		}
	}

	private void showProgressBar()
	{
		_progressRender.enabled = true;
		
		Vector3 size = _progressBar.transform.localScale;
		size.x = 0;
		
		_progressBar.transform.localScale = size;
	}
	
	private void hideProgressBar()
	{
		_progressRender.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		
		if (!_btnEndReached && selected && _progressBar.transform.localScale.x <= _progressScale.x) {
			Vector3 size = _progressBar.transform.localScale;
			size.x += (selectSpeed * Time.deltaTime);
			
			if (size.x > _progressScale.x)
			{
				size.x = _progressScale.x;	

				keyPressed();

				// Reset bar
				if (!_btnEndReached)
					size.x = 0;
			}
			
			_progressBar.transform.localScale = size;
		}
		
	}
}
