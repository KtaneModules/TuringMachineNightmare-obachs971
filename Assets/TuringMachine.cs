using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class TuringMachine : MonoBehaviour {

	private int NUM_TEST_RESET = 125; //This is the number of tests that the user must do before the user can test duplicate combinations
	private static int moduleCount = 1;
	private int moduleId;
	private bool moduleSolved;
	public KMBombModule module;
	public new KMAudio audio;
	public KMSelectable[] verifiers;
	public KMSelectable[] numbers;
	public KMSelectable clear;
	public KMSelectable prev;
	public KMSelectable next;
	public KMSelectable submit;
	public KMSelectable mainButton;
	public MeshRenderer[] mainScreen;
	public TextMesh[] verifierText;
	public TextMesh[] numberText;
	public TextMesh[] resultText;
	public TextMesh[] pageNumbers;
	public Material blankMat;
	public Material[] clueMatList;
	public AudioClip buttonSFX;
	public AudioClip arrowSFX;
	public AudioClip testingSFX;
	public AudioClip solveSFX;

	private PuzzleGen gen;
	private int[] solution;
	private List<Clue> clues;
	private int clueCursor = 0;

	private int posIndex = 0;
	private List<string> results;
	private int currentPage = 0;
	void Awake()
	{
		moduleId = moduleCount++;
		gen = new PuzzleGen(blankMat, clueMatList);
		solution = gen.getSolution();
		clues = gen.getClues();
		results = new List<string>();

		Debug.LogFormat("[Turing Machine Nightmare #{0}] Solution: {1}{2}{3}", moduleId, solution[0], solution[1], solution[2]);

		foreach (Clue clue in clues)
		{
			Debug.LogFormat("[Turing Machine Nightmare #{0}] Clue {1}", moduleId, clue.toString());
			Debug.LogFormat("[Turing Machine Nightmare #{0}] Verifier {1}", moduleId, clue.getSolution());
		}
		clues.Shuffle();

		foreach(char letter in "ABCDEF")
			verifiers[letter - 'A'].OnInteract = delegate { pressedVerifier(letter); return false; };
		for (int i = clues.Count; i < 6; i++)
			verifiers[i].transform.localScale = new Vector3(0f, 0f, 0f);
		foreach (char num in "12345")
			numbers[num - '1'].OnInteract = delegate { pressedNumber(num - '0'); return false; };
		clear.OnInteract = delegate { pressedClear(); return false; };
		prev.OnInteract = delegate { pressedArrow(-1); return false; };
		next.OnInteract = delegate { pressedArrow(1); return false; };
		mainButton.OnInteract = delegate { pressedMainScreen(); return false; };
		submit.OnInteract = delegate { pressedSubmit(); return false; };
	}
	void pressedMainScreen()
	{
		audio.PlaySoundAtTransform(buttonSFX.name, transform);
		Material[] mats = clues[clueCursor].getClueMat();
		for (int i = 0; i < mats.Length; i++)
			mainScreen[i].material = mats[i];
		clueCursor = (clueCursor + 1) % clues.Count;
	}
	void pressedVerifier(char ID)
	{
		audio.PlaySoundAtTransform(buttonSFX.name, transform);
		if (posIndex > 2)
		{
			int[] digits = { numberText[0].text[0] - '0', numberText[1].text[0] - '0', numberText[2].text[0] - '0'};
			bool result = checkNumber(digits[0] + "" + digits[1] + "" + digits[2]);
			if(result)
			{
				currentPage = results.Count / 3;
				pageNumbers[0].text = (currentPage + 1) + "";
				pageNumbers[1].text = (currentPage + 1) + "";
				result = clues[getIndex(ID)].test(digits);
				string s = (result) ? "O" : "X";
				results.Add(digits[0] + "" + digits[1] + "" + digits[2] + "" + ID + "" + s);
				display();
				pressedClear();
				Debug.LogFormat("[Turing Machine Nightmare #{0}] Test #{1}: {2}{3}{4} {5} {6}", moduleId, results.Count, digits[0], digits[1], digits[2], ID, s);
			}
			else
			{
				Debug.LogFormat("[Turing Machine Nightmare #{0}] User tried to test {1}{2}{3} but it hasn't reset yet", moduleId, digits[0], digits[1], digits[2]);
				foreach (TextMesh text in numberText)
					text.color = Color.red;
			}
		}
	}
	void pressedNumber(int number)
	{
		audio.PlaySoundAtTransform(buttonSFX.name, transform);
		if(posIndex < 3)
			numberText[posIndex++].text = number + "";
	}
	void pressedClear()
	{
		audio.PlaySoundAtTransform(buttonSFX.name, transform);
		posIndex = 0;
		foreach (TextMesh text in numberText)
		{
			text.text = "";
			text.color = Color.white;
		}
	}
	void pressedArrow(int num)
	{
		audio.PlaySoundAtTransform(arrowSFX.name, transform);
		int totalPages = (results.Count / 3) + 1;
		if (results.Count % 3 == 0 && results.Count > 0)
			totalPages--;
		currentPage = mod(currentPage + num, totalPages);
		pageNumbers[0].text = (currentPage + 1) + "";
		display();
	}
	void pressedSubmit()
	{
		audio.PlaySoundAtTransform(buttonSFX.name, transform);
		bool flag = true;
		foreach (TextMesh text in numberText)
			flag = flag && (text.text.Length > 0);
		if(flag)
		{
			Debug.LogFormat("[Turing Machine Nightmare #{0}] User is submitting {1}{2}{3}", moduleId, numberText[0].text, numberText[1].text, numberText[2].text);
			foreach (KMSelectable verifier in verifiers)
				verifier.OnInteract = null;
			foreach (KMSelectable number in numbers)
				number.OnInteract = null;
			clear.OnInteract = null;
			prev.OnInteract = null;
			next.OnInteract = null;
			submit.OnInteract = null;
			mainButton.OnInteract = null;
			foreach (TextMesh text in verifierText)
				text.color = Color.white;
			foreach (MeshRenderer screen in mainScreen)
				screen.material = blankMat;
			foreach (TextMesh text in resultText)
				text.text = "";
			foreach (TextMesh text in pageNumbers)
				text.text = "";
			StartCoroutine(testingInput());
		}
	}
	IEnumerator testingInput()
	{
		int[] digits = new int[3];
		for (int i = 0; i < 3; i++)
			digits[i] = numberText[i].text[0] - '0';
		List<Clue> order = new List<Clue>();
		foreach (char ID in "ABCDEF".Substring(0, clues.Count))
			order.Add(clues[getIndex(ID)]);
		clues = order;
		bool[] results = new bool[clues.Count];
		float buffer = 7.0f / clues.Count;
		for (int i = 0; i < results.Length; i++)
			results[i] = clues[i].test(digits);
		yield return new WaitForSeconds(0.5f);
		audio.PlaySoundAtTransform(testingSFX.name, transform);
		bool solve = true;
		for(int i = 0; i < results.Length; i++)
		{
			for(int j = 0; j < 3; j++)
				mainScreen[j].material = clues[i].getSolutionMat()[j];
			yield return new WaitForSeconds(buffer);
			solve = solve && results[i];
			verifierText[i].color = results[i] ? Color.green : Color.red;
			yield return new WaitForSeconds(buffer);
		}
		foreach (MeshRenderer screen in mainScreen)
			screen.material = blankMat;
		yield return new WaitForSeconds(0.05f);
		if(solve)
		{
			foreach (TextMesh text in numberText)
				text.text = "";
			audio.PlaySoundAtTransform(solveSFX.name, transform);
			module.HandlePass();
			moduleSolved = true;
		}
		else
		{
			if (!_strikeAvoid)
				module.HandleStrike();
			else
				Debug.LogFormat("[Turing Machine Nightmare #{0}] Strike prevented in autosolver.", moduleId);
			_strikeAvoid = false;
			gen.generatePuzzle(blankMat, clueMatList);
			solution = gen.getSolution();
			clues = gen.getClues();
			this.results = new List<string>();
			posIndex = 0;
			currentPage = 0;
			yield return new WaitForSeconds(5.0f);
			Debug.LogFormat("[Turing Machine Nightmare #{0}] Solution: {1}{2}{3}", moduleId, solution[0], solution[1], solution[2]);
			foreach (Clue clue in clues)
				Debug.LogFormat("[Turing Machine Nightmare #{0}] Clue {1}", moduleId, clue.toString());
			foreach (char letter in "ABCDEF")
				verifiers[letter - 'A'].OnInteract = delegate { pressedVerifier(letter); return false; };
			foreach (char num in "12345")
				numbers[num - '1'].OnInteract = delegate { pressedNumber(num - '0'); return false; };
			foreach (TextMesh text in numberText)
				text.text = "";
			foreach (TextMesh text in verifierText)
				text.color = Color.white;
			clear.OnInteract = delegate { pressedClear(); return false; };
			prev.OnInteract = delegate { pressedArrow(-1); return false; };
			next.OnInteract = delegate { pressedArrow(1); return false; };
			submit.OnInteract = delegate { pressedSubmit(); return false; };
			mainButton.OnInteract = delegate { pressedMainScreen(); return false; };
			pageNumbers[0].text = "1";
			pageNumbers[1].text = "1";
			pageNumbers[2].text = "/";
		}
	}
	int getIndex(char ID)
	{
		for(int i = 0; i < clues.Count; i++)
		{
			if (ID == clues[i].getID())
				return i;
		}
		return -1;
	}
	void display()
	{
		int index = currentPage * 3;
		foreach (TextMesh text in resultText)
			text.text = "";
		for (int i = index; i < results.Count && i < index + 3; i++)
		{
			for(int j = 0; j < 5; j++)
				resultText[((i % 3) * 5) + j].text = results[i][j] + "";
		}
	}
	bool checkNumber(string number)
	{
		int sum = 0;
		foreach(string result in results)
		{
			if (result.StartsWith(number))
				sum++;
		}
		return sum <= (results.Count / NUM_TEST_RESET);
	}
	int mod(int n, int m)
	{
		while (n < 0)
			n += m;
		return (n % m);
	}
#pragma warning disable 414
	private readonly string TwitchHelpMessage = @"!{0} press|p [buttons] will press all the buttons within the buttons attribute. Button list: A, B, C, D, E, F, 1, 2, 3, 4, 5, Clear/X, Left/L, Right/R, Screen, Submit/Sub.";
#pragma warning restore 414
	IEnumerator ProcessTwitchCommand(string command)
	{
		string[] param = command.ToUpperInvariant().Split(' ');
		if ((Regex.IsMatch(param[0], @"^\s*PRESS\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant) || Regex.IsMatch(param[0], @"^\s*P\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)) && param.Length > 1)
		{
			if (isButton(param))
			{
				yield return null;
				for (int i = 1; i < param.Length; i++)
				{
					switch (param[i])
					{
						case "A":case "B":case "C":case "D":case "E":case "F":
							if(verifiers[param[i][0] - 'A'].OnInteract != null)
								verifiers[param[i][0] - 'A'].OnInteract();
							break;
						case "1":case "2":case "3":case "4":case "5":
							if(numbers[param[i][0] - '1'].OnInteract != null)
								numbers[param[i][0] - '1'].OnInteract();
							break;
						case "CLEAR":case "X":
							if(clear.OnInteract != null)
								clear.OnInteract();
							break;
						case "LEFT":case "L":
							if(prev.OnInteract != null)
								prev.OnInteract();
							break;
						case "RIGHT":case "R":
							if(next.OnInteract != null)
								next.OnInteract();
							break;
						case "SCREEN":
							if (mainButton.OnInteract != null)
								mainButton.OnInteract();
							break;
						case "SUBMIT":case "SUB":
							if(submit.OnInteract != null)
								submit.OnInteract();
							break;
						
					}
					yield return new WaitForSeconds(0.2f);
				}
			}
			else
				yield return "sendtochat An error occured because the user inputted something wrong.";
		}
		else
			yield return "sendtochat An error occured because the user inputted something wrong.";
	}
	private bool isButton(string[] param)
	{
		for (int i = 1; i < param.Length; i++)
		{
			switch (param[i])
			{
				case "A":
				case "B":
				case "C":
				case "D":
				case "1":
				case "2":
				case "3":
				case "4":
				case "5":
				case "CLEAR":
				case "X":
				case "LEFT":
				case "L":
				case "RIGHT":
				case "R":
				case "SUBMIT":
				case "SUB":
				case "SCREEN":
					break;
				case "E":
					if (clues.Count < 5)
						return false;
					break;
				case "F":
					if (clues.Count < 6)
						return false;
					break;
				default:
					return false;
			}
		}
		return true;
	}

	private bool _strikeAvoid;

	private IEnumerator TwitchHandleForcedSolve()
	{
		_strikeAvoid = true;
		while (clear.OnInteract == null)
			yield return true;
		_strikeAvoid = false;
		yield return new WaitForSeconds(0.1f);
		var sol = solution.Join("");
		var input = numberText[0].text + numberText[1].text + numberText[2].text;
		if (!sol.StartsWith(input))
        {
			clear.OnInteract();
			yield return new WaitForSeconds(0.1f);
        }
		for (int i = input.Length; i < sol.Length; i++)
		{
			numbers[sol[i] - '0' - 1].OnInteract();
			yield return new WaitForSeconds(0.1f);
		}
		submit.OnInteract();
		while (!moduleSolved)
			yield return true;
	}
}
