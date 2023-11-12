using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clue {

	private char clueID;
	private string[] left;
	private string[] middle;
	private string[] right;
	private int clueIndex = -1;
	private Material[] clueMat;
	private Material[] solutionMat;
	public Clue(string[] left, string[] middle, string[] right, int[] digits)
	{
		this.left = left;
		this.middle = middle;
		this.right = right;
		clueMat = new Material[3];
		pickClue(digits);
	}

	// Sets the clue ID
	public void setID(char id)
	{
		clueID = id;
	}

	// Returns the clue ID
	public char getID()
	{
		return clueID;
	}

	// Returns the clue index
	public int getClueIndex()
	{
		return clueIndex;
	}

	// Returns 1 part of the expression
	public string[] getExpressions()
	{
		return new string[] { getExpression(left), getExpression(middle), getExpression(right) };
	}
	public string[] getSolutionExpression()
	{
		return new string[] { left[clueIndex % left.Length], middle[clueIndex % middle.Length], right[clueIndex % right.Length] };
	}
	// Returns a string version of the expression
	private string getExpression(string[] part)
	{
		string output = part[0];
		for (int i = 1; i < part.Length; i++)
			output = output + ", " + part[i];
		return output;
	}
	// Sets the material for the clue
	public void setClueMat(Material[] mat)
	{
		clueMat = mat;
	}
	// Returns the 3 Materials used for each screen
	public Material[] getClueMat()
	{
		return clueMat;
	}
	// Sets the material for the clue solution
	public Material[] getSolutionMat()
	{
		return solutionMat;
	}
	// Returns the 3 Materials used for each screen of the solution
	public void setSolutionMat(Material[] mat)
	{
		solutionMat = mat;
	}
	// toString method
	public string toString()
	{
		string output = clueID + ": " + left[0] + " " + middle[0] + " " + right[0];
		for(int i = 1; i < left.Length || i < middle.Length || i < right.Length; i++)
			output = output + "     " + left[i % left.Length] + " " + middle[i % middle.Length] + " " + right[i % right.Length];
		return output;
	}
	// Sets the value of clueIndex by testing the clue with a 3 digit number.
	private void pickClue(int[] digits)
	{
		List<int> list = new List<int>();
		for (int i = 0; i < left.Length || i < middle.Length || i < right.Length; i++)
		{
			bool flag = test(i, digits);
			if (flag)
				list.Add(i);
		}
		if (list.Count > 0)
			clueIndex = list[UnityEngine.Random.Range(0, list.Count)];
	}
	// Tests the clue using an index and a 3 digit number
	private bool test(int index, int[] digits)
	{
		return test(left[index % left.Length], middle[index % middle.Length], right[index % right.Length], digits);
	}
	// Tests a 3 digit number
	public bool test(int[] digits)
	{
		return test(left[clueIndex % left.Length], middle[clueIndex % middle.Length], right[clueIndex % right.Length], digits);
	}

	// Returns the verifier that it used.
	public string getSolution()
	{
		return clueID + ": " + left[clueIndex % left.Length] + " " + middle[clueIndex % middle.Length] + " " + right[clueIndex % right.Length];
	}
	// Returns a boolean value by using the expression and a 3 digit number
	private bool test(string left, string middle, string right, int[] digits)
	{
		bool flag = true;
		int[] l = strToValues(left, digits);
		int[] r = strToValues(right, digits);
		switch (middle)
		{
			case "<":
				for(int i = 0; i < l.Length || i < r.Length; i++)
					flag = flag && (l[i % l.Length] < r[i % r.Length]);
				return flag;
			case "=":
				for (int i = 0; i < l.Length || i < r.Length; i++)
					flag = flag && (l[i % l.Length] == r[i % r.Length]);
				return flag;
			case ">":
				for (int i = 0; i < l.Length || i < r.Length; i++)
					flag = flag && (l[i % l.Length] > r[i % r.Length]);
				return flag;
			case "/":
				for (int i = 0; i < l.Length || i < r.Length; i++)
					flag = flag && ((l[i % l.Length] % r[i % r.Length]) == 0);
				return flag;
			case "!/":
				for (int i = 0; i < l.Length || i < r.Length; i++)
					flag = flag && ((l[i % l.Length] % r[i % r.Length]) != 0);
				return flag;
		}
		return false;
	}
	// Turns the given string to a value.
	private int[] strToValues(string str, int[] digits)
	{
		switch(str)
		{
			case "1st": return new int[] { digits[0] };
			case "2nd": return new int[] { digits[1] };
			case "3rd": return new int[] { digits[2] };
			case "1st and 2nd": return new int[] { digits[0], digits[1] };
			case "1st and 3rd": return new int[] { digits[0], digits[2] };
			case "2nd and 3rd": return new int[] { digits[1], digits[2] };
			case "1st + 2nd": return new int[] { digits[0] + digits[1] };
			case "1st + 3rd": return new int[] { digits[0] + digits[2] };
			case "2nd + 3rd": return new int[] { digits[1] + digits[2] };
			case "|1st - 2nd|": return new int[] { Math.Abs(digits[0] - digits[1]) };
			case "|1st - 3rd|": return new int[] { Math.Abs(digits[0] - digits[2]) };
			case "|2nd - 3rd|": return new int[] { Math.Abs(digits[1] - digits[2]) };
			case "1st + 2nd + 3rd": return new int[] { digits[0] + digits[1] + digits[2] };
			case "Evens":
				int numEven = 0;
				foreach(int digit in digits)
				{
					if (digit % 2 == 0)
						numEven++;
				}
				return new int[] { numEven };
			case "Odds":
				int numOdd = 0;
				foreach (int digit in digits)
				{
					if (digit % 2 == 1)
						numOdd++;
				}
				return new int[] { numOdd };
			case "Consecutive Pairs":
				int numCon = 0;
				for (int i = 1; i < 3; i++)
				{
					if (Math.Abs(digits[i - 1] - digits[i]) == 1)
						numCon++;
				}
				return new int[] { numCon };
			case "Distinct Numbers":
				int[] sums = new int[5];
				foreach (int digit in digits)
					sums[digit - 1]++;
				int sum = 0;
				foreach(int num in sums)
				{
					if (num > 0)
						sum++;
				}
				return new int[] { sum };
			case "Ascending Order":
				return (digits[0] < digits[1] && digits[1] < digits[2]) ? new int[] { 1 } : new int[] { 0 };
			case "Descending Order":
				return (digits[0] > digits[1] && digits[1] > digits[2]) ? new int[] { 1 } : new int[] { 0 };
			case "Chaotic Order":
				return !(((digits[0] < digits[1]) && (digits[1] < digits[2])) || ((digits[0] > digits[1]) && (digits[1] > digits[2]))) ? new int[] { 1 } : new int[] { 0 };
			default:
				if (str.Length == 2 && str[1] == 's')
				{
					int target = str[0] - '0';
					int s = 0;
					foreach(int digit in digits)
					{
						if (target == digit)
							s++;
					}
					return new int[] { s };
				}
				else
					return new int[] { Int32.Parse(str) };
		}
	}

}
