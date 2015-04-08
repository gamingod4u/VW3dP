using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using SimpleJSON;

public class T9Node 
{
	public SortedList<int, string> top_entries;
	public SortedList<int, T9Node> children;

	public T9Node()
	{
		top_entries = new SortedList<int, string> (new DescendedWeightComparer());
		children = new SortedList<int, T9Node> ();
	}
}

class DescendedWeightComparer : IComparer<int>
{
	public int Compare(int x, int y)
	{
		// use the default comparer to do the original comparison for datetimes
		int ascendingResult = x - y;
		
		// turn the result around
		return 0 - ascendingResult;
	}
}

public class T9 : MonoBehaviour {

	public string t9text;
	public string spellingtext;
	public string recommendedWord;

	private GameObject _inputData;
	private Text _inputField;

	private SortedList<int, T9Node> dictionary;
	private SortedList<int, string> suggestions = null;

	private bool _wordsLoaded;

	public void updateT9Suggestions()
	{
		recommendedWord = "";

		SortedList<int, T9Node> child = dictionary;

		suggestions = null;

		int lastT9 = t9text.LastIndexOf (" ");
		int idx = 0;

		if (lastT9 > 0)
			idx = lastT9 + 1;

		for (; idx<t9text.Length; idx++) 
		{
			int digit = int.Parse(t9text.Substring(idx,1));

			if (!child.ContainsKey(digit))
			{
				break;
			}

			if (idx == (t9text.Length - 1))
				suggestions = child[digit].top_entries;
			else
				child = child[digit].children;
		}

		if (suggestions != null) {
			foreach (KeyValuePair<int, string> kvp in suggestions)
			{
				if (recommendedWord == "")
					recommendedWord = kvp.Value;
			}

			if (recommendedWord != "") {
				int lastSpace = spellingtext.LastIndexOf (" ");
				if (lastSpace > 0)
					spellingtext = spellingtext.Substring(0,lastSpace+1) + recommendedWord.Substring (0, t9text.Length - (lastT9+1));
				else
					spellingtext = recommendedWord.Substring (0, t9text.Length);
			}
		}

		setSearchText ();
	}

	private IEnumerator loadT9Dictionary()
	{
		WWW www = new WWW ("https://www.tnaflix.com/ajax/t9_all.php");
		
		yield return www;
		
		JSONNode N = JSON.Parse (www.text);
		
		for (int i=0; i<N.Count; i++) {
			int weight = N[i]["weight"].AsInt;
			string word = N[i]["keyword"];

			SortedList<int, T9Node> child = dictionary;

			for (int k=0;k<word.Length;k++)
			{
				int digit = chrToDigit(word[k].ToString());

				if (!child.ContainsKey(digit))
					child.Add(digit, new T9Node());

				child[digit].top_entries.Add(weight, word);

				child = child[digit].children;
			}
		}

		GameObject o = GameObject.Find ("OVRPlayerController/OVRCameraRig/TrackingSpace/LeftEyeAnchor/Loading");
		o.SetActive (false);
//		o.GetComponent<Renderer> ().enabled = false;

		_wordsLoaded = true;
	}

	private int chrToDigit(string chr)
	{
		chr = chr.ToUpper ();

		if (chr.IndexOfAny(new char[]{'A','B','C'}) != -1)
			return 1;
		if (chr.IndexOfAny(new char[]{'D','E','F'}) != -1)
			return 2;
		if (chr.IndexOfAny(new char[]{'G','H','I'}) != -1)
			return 3;
		if (chr.IndexOfAny(new char[]{'J','K','L'}) != -1)
			return 4;
		if (chr.IndexOfAny(new char[]{'M','N','O'}) != -1)
			return 5;
		if (chr.IndexOfAny(new char[]{'P','Q','R','S'}) != -1)
			return 6;
		if (chr.IndexOfAny(new char[]{'T','U','V'}) != -1)
			return 7;
		if (chr.IndexOfAny(new char[]{'W','X','Y','Z'}) != -1)
			return 8;

		return 0;
	}

	public void setSearchText()
	{
		// Lets get the last words of each
		int lastT9 		= t9text.LastIndexOf (" ");
		int lastSpell 	= spellingtext.LastIndexOf (" ");

		string strT9 	= getCurrentT9Word ();
		string strSpell = getCurrentSpelledWord ();

		if (strT9.Length == strSpell.Length) {
			_inputField.text = (lastSpell != -1 ? spellingtext.Substring(0, lastSpell + 1) : "") + "<b>" + recommendedWord.Substring (0, strT9.Length) + "</b><color=grey>" + recommendedWord.Substring (strT9.Length) + "</color>";
		}else
			_inputField.text = spellingtext;
	}

	// Use this for initialization
	void Start () {
		_wordsLoaded = false;

		_inputData = GameObject.Find ("InputField/InputText");
		_inputField = _inputData.GetComponent<Text>();

		dictionary = new SortedList<int, T9Node> ();
		StartCoroutine( loadT9Dictionary ());
	}

	public string getCurrentT9Word()
	{
		// Lets get the last words of each
		int lastT9 		= t9text.LastIndexOf (" ");
		string strT9 	= t9text.Substring ((lastT9 == -1 ? 0 : lastT9+1));

		return strT9;
	}
	
	public string getCurrentSpelledWord()
	{
		int lastSpell 	= spellingtext.LastIndexOf (" ");
		string strSpell = spellingtext.Substring ((lastSpell == -1 ? 0 : lastSpell + 1));

		return strSpell;
	}

	public void acceptSuggestion(int index)
	{
		if (suggestions != null && suggestions.Count > index) {
			string word = recommendedWord;//suggestions[index];
			string t9word = "";

			for (int k=0;k<word.Length;k++)
			{
				int digit = chrToDigit(word[k].ToString());
				t9word += digit.ToString();
			}

			int lastT9 		= t9text.LastIndexOf (" ");
			int lastSpell 	= spellingtext.LastIndexOf (" ");

			if (lastT9 != -1 && lastSpell != -1) {
				t9text = t9text.Substring(0,lastT9) + " " + t9word + " ";
				spellingtext = spellingtext.Substring(0,lastSpell) + " " + word + " ";
			} else {
				t9text = t9word + " ";
				spellingtext = word + " ";
			}
		}
	}
}
