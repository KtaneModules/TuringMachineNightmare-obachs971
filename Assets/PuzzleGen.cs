using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleGen {

    private Dictionary<string, int> clueMatDict = new Dictionary<string, int>()
    {
        { "1st", 0},{ "2nd", 1},{ "3rd", 2},
        { "<, =", 3},{ "=, >", 4},{ "<, =, >", 5},
        { "1", 6},{ "2", 7},{ "3", 8},{ "4", 9},{ "5", 10},
        { "0, 1", 11}, { "/, !/", 12},
        { "1s", 13},{ "2s", 14},{ "3s", 15},{ "4s", 16},{ "5s", 17},
        { "=", 18},
        { "0, 1, 2, 3", 19},{ "0", 20},
        { "1st, 2nd, 3rd", 21}, { "2nd and 3rd, 1st and 3rd, 1st and 2nd", 22},
        { ">", 23},{ "<", 24},
        { "Evens", 25},{ "Odds", 26},
        { "<, >", 27},
        { "1st + 2nd + 3rd", 28},
        { "1st + 2nd", 29},{ "1st + 3rd", 30},{ "2nd + 3rd", 31},
        { "6", 32},{ "7", 33},{ "8", 34},{ "9", 35},{ "10", 36},
        { "|1st - 2nd|", 37},{ "|1st - 3rd|", 38},{ "|2nd - 3rd|", 39},
        { "Distinct Numbers", 40},
        { "1, 2, 3", 41},
        { "Ascending Order", 42},{ "Descending Order", 43},{ "Chaotic Order", 44},{ "Ascending Order, Descending Order, Chaotic Order", 45},
        { "11", 46},{ "12", 47},{ "13", 48},{ "14", 49},{ "15", 50},
		{ "Consecutive Pairs", 51},
        { "1st and 2nd, 1st and 3rd, 2nd and 3rd", 52},
        { "/", 53},
        { "!/", 54},
        { "3, 4, 5", 55},
        { "1st + 2nd, 1st + 3rd, 2nd + 3rd", 56},
        { "|1st - 2nd|, |1st - 3rd|, |2nd - 3rd|", 57},
        { "0, 1, 2", 58},
        { "1st and 2nd", 59 },{ "1st and 3rd", 60 },{ "2nd and 3rd", 61 },
        { "2nd + 3rd, 1st + 3rd, 1st + 2nd", 62},
        { "|2nd - 3rd|, |1st - 3rd|, |1st - 2nd|", 63},
        { "3rd, 2nd, 1st", 64}
    };

    private List<Clue> clues;
    private List<Clue> hardClues;
    private List<Clue> specialClues;
    private int[] solution;
    private int numClues;
    public PuzzleGen(Material blankMat, Material[] clueMats)
    {
        solution = new int[3];
        numClues = Random.Range(0, 3) + 4;
        //numClues = 4;
        generatePuzzle(blankMat, clueMats);
    }
    public void generatePuzzle(Material blankMat, Material[] clueMats)
    {
        clues = new List<Clue>();
        specialClues = new List<Clue>();
        hardClues = new List<Clue>();
        for (int i = 0; i < 3; i++)
            solution[i] = UnityEngine.Random.Range(0, 5) + 1;
        //solution = new int[] { 3, 2, 3 };
        generateAllClues();

        setClueMats(blankMat, clueMats, clues);
        setClueMats(blankMat, clueMats, specialClues);
        setClueMats(blankMat, clueMats, hardClues);

        // Removing Clues that don't have a valid Clue Index
        for (int i = 0; i < clues.Count; i++)
        {
            if (clues[i].getClueIndex() == -1)
                clues.RemoveAt(i--);
        }
        for (int i = 0; i < hardClues.Count; i++)
        {
            if (hardClues[i].getClueIndex() == -1)
                hardClues.RemoveAt(i--);
        }
        for (int i = 0; i < specialClues.Count; i++)
        {
            if (specialClues[i].getClueIndex() == -1)
                specialClues.RemoveAt(i--);
        }

        List<Clue> selected = selectClues();
        removeClues(selected);
        bool valid = checker(selected);
        while (!(valid))
        {
            selected = selectClues();
            removeClues(selected);
            valid = checker(selected);
        }
        clues = selected;
        setSolutionMats(blankMat, clueMats, clues);
        clues.Shuffle();
        string alpha = "ABCDEF";
        for (int i = 0; i < clues.Count; i++)
            clues[i].setID(alpha[i]);
    }
    public int[] getSolution()
    {
        return solution;
    }
    private bool checker(List<Clue> selected)
    {
        if (selected.Count != numClues)
            return false;
        // Add in checker to see if a hard clue was added.
        bool b1 = false, b2 = false;
        foreach(Clue clue in selected)
        {
            if (specialClues.Contains(clue))
                b1 = true;
            if (hardClues.Contains(clue))
                b2 = true;
        }
        return b1 && b2;
    }
    private void generateAllClues()
    {

        // Generic Clues

        // Position Compares Number
        string[] pos1 = { "1st", "2nd", "3rd" };
        for (int i = 0; i < 3; i++)
        {
            for(int j = 1; j <= 5; j++)
            {
                if (j == 1)
                    clues.Add(new Clue(new string[] { pos1[i] }, new string[] { "=", ">" }, new string[] { j + "" }, solution));
                else if (j == 5)
                    clues.Add(new Clue(new string[] { pos1[i] }, new string[] { "<", "=" }, new string[] { j + "" }, solution));
                else
                    clues.Add(new Clue(new string[] { pos1[i] }, new string[] { "<", "=", ">" }, new string[] { j + "" }, solution));
            }
        }
        // Position Divisible by 2
        for (int i = 0; i < 3; i++)
            clues.Add(new Clue(new string[] { pos1[i] }, new string[] { "/", "!/" }, new string[] { "2" }, solution));
        // Position Compares Position
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if(j != i)
                    clues.Add(new Clue(new string[] { pos1[i] }, new string[] { "<", "=", ">" }, new string[] { pos1[j] }, solution));
            }
        }
        // 2 Position Compares Number
        string[] pos2 = { "1st and 2nd", "1st and 3rd", "2nd and 3rd" };
        for(int i = 0; i < 3; i++)
        {
            clues.Add(new Clue(new string[] { pos2[i] }, new string[] { "=", ">" }, new string[] { "1" }, solution));
            for (int j = 2; j <= 4; j++)
                clues.Add(new Clue(new string[] { pos2[i] }, new string[] { "<", "=", ">" }, new string[] { j + "" }, solution));
            clues.Add(new Clue(new string[] { pos2[i] }, new string[] { "<", "=" }, new string[] { "5" }, solution));
        }
        // 2 Position Divisible by 2
        foreach(string pos in pos2)
            clues.Add(new Clue(new string[] { pos }, new string[] { "/", "!/" }, new string[] { "2" }, solution));
        // Position Compares 2 Positions
        for (int i = 0; i < 3; i++)
            clues.Add(new Clue(new string[] { pos1[i] }, new string[] { "<", ">" }, new string[] { pos2[2 - i] }, solution));
        // 2 Sum Compares Number
        string[] sum2 = { "1st + 2nd", "1st + 3rd", "2nd + 3rd" };
        for (int i = 0; i < 3; i++)
        {
            for (int j = 2; j <= 10; j++)
            {
                if (j == 2)
                    clues.Add(new Clue(new string[] { sum2[i] }, new string[] { "=", ">" }, new string[] { j + "" }, solution));
                else if (j == 10)
                    clues.Add(new Clue(new string[] { sum2[i] }, new string[] { "<", "=" }, new string[] { j + "" }, solution));
                else
                    clues.Add(new Clue(new string[] { sum2[i] }, new string[] { "<", "=", ">" }, new string[] { j + "" }, solution));
            }
        }
        // 2 Sum Divisible by 2
        for (int i = 0; i < 3; i++)
            clues.Add(new Clue(new string[] { sum2[i] }, new string[] { "/", "!/" }, new string[] { "2" }, solution));
        // Position Compares 2 Sum
        for (int i = 0; i < 3; i++)
            clues.Add(new Clue(new string[] { pos1[i] }, new string[] { "<", "=", ">" }, new string[] { sum2[2 - i] }, solution));
        // 2 Sum Divisible by Position
        for (int i = 0; i < 3; i++)
        {
            for(int j = 0; j < 3; j++)
                clues.Add(new Clue(new string[] { sum2[i] }, new string[] { "/", "!/" }, new string[] { pos1[j] }, solution));
        }
            
        // 2 Difference Compares Number
        string[] dif2 = { "|1st - 2nd|", "|1st - 3rd|", "|2nd - 3rd|" };
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j <= 4; j++)
            {
                if (j == 0)
                    clues.Add(new Clue(new string[] { dif2[i] }, new string[] { "=", ">" }, new string[] { j + "" }, solution));
                else if (j == 4)
                    clues.Add(new Clue(new string[] { dif2[i] }, new string[] { "<", "=" }, new string[] { j + "" }, solution));
                else
                    clues.Add(new Clue(new string[] { dif2[i] }, new string[] { "<", "=", ">" }, new string[] { j + "" }, solution));
            }
        }
        // 2 Difference Divisible by 2
        for (int i = 0; i < 3; i++)
            clues.Add(new Clue(new string[] { dif2[i] }, new string[] { "/", "!/" }, new string[] { "2" }, solution));
        // Position Compares 2 Difference
        for (int i = 0; i < 3; i++)
        {
            for(int j = 0; j < 3; j++)
                clues.Add(new Clue(new string[] { pos1[i] }, new string[] { "<", "=", ">" }, new string[] { dif2[j] }, solution));
        }
        // 3 Sum Compares Number
        for (int i = 3; i <= 15; i++)
        {
            if (i == 3)
                clues.Add(new Clue(new string[] { "1st + 2nd + 3rd" }, new string[] { "=", ">" }, new string[] { i + "" }, solution));
            else if (i == 15)
                clues.Add(new Clue(new string[] { "1st + 2nd + 3rd" }, new string[] { "<", "=" }, new string[] { i + "" }, solution));
            else
                clues.Add(new Clue(new string[] { "1st + 2nd + 3rd" }, new string[] { "<", "=", ">" }, new string[] { i + "" }, solution));
        }
        // 3 Sum Divisible by 2
        clues.Add(new Clue(new string[] { "1st + 2nd + 3rd" }, new string[] { "/", "!/" }, new string[] { "2" }, solution));
        clues.Add(new Clue(new string[] { "1st + 2nd + 3rd" }, new string[] { "/", "!/" }, new string[] { "2" }, solution));
        clues.Add(new Clue(new string[] { "1st + 2nd + 3rd" }, new string[] { "/", "!/" }, new string[] { "2" }, solution));
        // 3 Sum Divisible by Position
        foreach (string pos in pos1)
            clues.Add(new Clue(new string[] { "1st + 2nd + 3rd" }, new string[] { "/", "!/" }, new string[] { pos }, solution));

        //Special Clues

        // Order Compare Number
        string[] orders = { "Ascending Order", "Descending Order", "Chaotic Order" };
        specialClues.Add(new Clue(orders, new string[] { "=" }, new string[] { "1" }, solution));
        specialClues.Add(new Clue(orders, new string[] { "=" }, new string[] { "0" }, solution));
        for (int i = 0; i < 3; i++)
            specialClues.Add(new Clue(new string[] { orders[i] }, new string[] { "=" }, new string[] { "0", "1" }, solution));
        //Consecutive Numbers
        specialClues.Add(new Clue(new string[] { "Consecutive Pairs" }, new string[] { "=" }, new string[] { "0", "1", "2" }, solution));
        specialClues.Add(new Clue(new string[] { "Consecutive Pairs" }, new string[] { "=" }, new string[] { "0", "1", "2" }, solution));
        //Distinct Numbers
        specialClues.Add(new Clue(new string[] { "Distinct Numbers" }, new string[] { "=" }, new string[] { "1", "2", "3" }, solution));
        specialClues.Add(new Clue(new string[] { "Distinct Numbers" }, new string[] { "=" }, new string[] { "1", "2", "3" }, solution));
        // Largest/Smallest
        specialClues.Add(new Clue(pos1, new string[] { ">" }, new string[] { pos2[2], pos2[1], pos2[0] }, solution));
        specialClues.Add(new Clue(pos1, new string[] { "<" }, new string[] { pos2[2], pos2[1], pos2[0] }, solution));
        // Evens Compare Odds
        specialClues.Add(new Clue(new string[] { "Evens" }, new string[] { "<", ">" }, new string[] { "Odds" }, solution));
        // Evens Equals Numbers
        specialClues.Add(new Clue(new string[] { "Evens" }, new string[] { "=" }, new string[] { "0", "1", "2", "3" }, solution));
        // Odds Equals Number
        specialClues.Add(new Clue(new string[] { "Odds" }, new string[] { "=" }, new string[] { "0", "1", "2", "3" }, solution));
        // Number of #s Equals to Numbers
        for (int i = 1; i <= 5; i++)
            specialClues.Add(new Clue(new string[] { i + "s" }, new string[] { "=" }, new string[] { "0", "1", "2", "3" }, solution));

        //Harder Clues

        //Positions Equal Number
        for (int i = 1; i <= 5; i++)
            hardClues.Add(new Clue(pos1, new string[] { "=" }, new string[] { i + "" }, solution));
        //Positions Less Than Number
        for (int i = 2; i <= 5; i++)
            hardClues.Add(new Clue(pos1, new string[] { "<" }, new string[] { i + "" }, solution));
        //Positions Greater Than Number
        for (int i = 1; i <= 4; i++)
            hardClues.Add(new Clue(pos1, new string[] { ">" }, new string[] { i + "" }, solution));
        //Positions Divisible by 2
        hardClues.Add(new Clue(pos1, new string[] { "/" }, new string[] { "2" }, solution));
        //Positions Not Divisible by 2
        hardClues.Add(new Clue(pos1, new string[] { "!/" }, new string[] { "2" }, solution));
        // 2 Positions Less Than Number
        for (int i = 2; i <= 5; i++)
            hardClues.Add(new Clue(pos2, new string[] { "<" }, new string[] { i + "" }, solution));
        // 2 Positions Equal to Number
        for (int i = 1; i <= 5; i++)
            hardClues.Add(new Clue(pos2, new string[] { "=" }, new string[] { i + "" }, solution));
        // 2 Positions Greater Than Number
        for (int i = 1; i <= 4; i++)
            hardClues.Add(new Clue(pos2, new string[] { ">" }, new string[] { i + "" }, solution));
        // 2 Positions Divisible by 2
        hardClues.Add(new Clue(pos2, new string[] { "/" }, new string[] { "2" }, solution));
        hardClues.Add(new Clue(pos2, new string[] { "!/" }, new string[] { "2" }, solution));
        //2 Sums Equals Number
        for (int i = 2; i <= 10; i++)
            hardClues.Add(new Clue(sum2, new string[] { "=" }, new string[] { i + "" }, solution));
        //2 Sums Less Than Number
        for (int i = 3; i <= 10; i++)
            hardClues.Add(new Clue(sum2, new string[] { "<" }, new string[] { i + "" }, solution));
        //2 Sums Greater Than Number
        for (int i = 2; i <= 9; i++)
            hardClues.Add(new Clue(sum2, new string[] { ">" }, new string[] { i + "" }, solution));
        //2 Sums Divisible by 2
        hardClues.Add(new Clue(sum2, new string[] { "/" }, new string[] { "2" }, solution));
        hardClues.Add(new Clue(sum2, new string[] { "!/" }, new string[] { "2" }, solution));
        // Positions Compares 2 Sums
        hardClues.Add(new Clue(pos1, new string[] { "<" }, new string[] { sum2[2], sum2[1], sum2[0] }, solution));
        hardClues.Add(new Clue(pos1, new string[] { "=" }, new string[] { sum2[2], sum2[1], sum2[0] }, solution));
        hardClues.Add(new Clue(pos1, new string[] { ">" }, new string[] { sum2[2], sum2[1], sum2[0] }, solution));
        // 2 Sums Divisible by Position
        foreach(string pos in pos1)
            hardClues.Add(new Clue(sum2, new string[] { "/" }, new string[] { pos }, solution));
        // 2 Sums Not Divisible by Position
        foreach (string pos in pos1)
            hardClues.Add(new Clue(sum2, new string[] { "!/" }, new string[] { pos }, solution));
        // 2 Sum Divisible by Positions
        foreach (string sum in sum2)
            hardClues.Add(new Clue(new string[] { sum }, new string[] { "/" }, pos1, solution));
        // 2 Sum Not Divisible by Positions
        foreach (string sum in sum2)
            hardClues.Add(new Clue(new string[] { sum }, new string[] { "!/" }, pos1, solution));
        //2 Differences Equals Number
        for (int i = 0; i <= 4; i++)
            hardClues.Add(new Clue(dif2, new string[] { "=" }, new string[] { i + "" }, solution));
        //2 Differences Less Than Number
        for (int i = 1; i <= 4; i++)
            hardClues.Add(new Clue(dif2, new string[] { "<" }, new string[] { i + "" }, solution));
        //2 Differences Greater Than Number
        for (int i = 0; i <= 3; i++)
            hardClues.Add(new Clue(dif2, new string[] { ">" }, new string[] { i + "" }, solution));
        //2 Differences Divisible by 2
        hardClues.Add(new Clue(dif2, new string[] { "/" }, new string[] { "2" }, solution));
        hardClues.Add(new Clue(dif2, new string[] { "!/" }, new string[] { "2" }, solution));
        // Position Less Than 2 Differences
        foreach(string pos in pos1)
            hardClues.Add(new Clue(new string[] { pos }, new string[] { "<" }, dif2, solution));
        // Position Equal To 2 Differences
        foreach (string pos in pos1)
            hardClues.Add(new Clue(new string[] { pos }, new string[] { "=" }, dif2, solution));
        // Position Greater Than 2 Differences
        foreach (string pos in pos1)
            hardClues.Add(new Clue(new string[] { pos }, new string[] { ">" }, dif2, solution));
        // Positions Less Than 2 Difference
        foreach (string dif in dif2)
            hardClues.Add(new Clue(pos1, new string[] { "<" }, new string[] { dif }, solution));
        // Positions Equal To 2 Difference
        foreach (string dif in dif2)
            hardClues.Add(new Clue(pos1, new string[] { "=" }, new string[] { dif }, solution));
        // Positions Greater Than 2 Difference
        foreach (string dif in dif2)
            hardClues.Add(new Clue(pos1, new string[] { ">" }, new string[] { dif }, solution));

        //3 Sum Divisible by Numbers
        hardClues.Add(new Clue(new string[] { "1st + 2nd + 3rd" }, new string[] { "/" }, new string[] { "3", "4", "5" }, solution));
        hardClues.Add(new Clue(new string[] { "1st + 2nd + 3rd" }, new string[] { "!/" }, new string[] { "3", "4", "5" }, solution));
        //3 Sum Divisible by Positions
        hardClues.Add(new Clue(new string[] { "1st + 2nd + 3rd" }, new string[] { "/" }, pos1, solution));
        hardClues.Add(new Clue(new string[] { "1st + 2nd + 3rd" }, new string[] { "!/" }, pos1, solution));
        
    }

    // Shuffles the clues and selects the clues until the amount of possible digits is 1
    private List<Clue> selectClues()
    {
        for (int i = 0; i < 3; i++)
        {
            clues.Shuffle();
            hardClues.Shuffle();
            specialClues.Shuffle();
        }
        List<Clue> possClues = new List<Clue>();
        possClues.AddRange(clues);
        for (int i = 0; i < 6; i++)
        {
            possClues.Insert(0, hardClues[i]);
            possClues.Insert(0, specialClues[i]);
        }
        possClues.Shuffle();
        // Add in the first 6 hard clues here

        int index = 1;
        List<int[]> possDigits = new List<int[]>();
        for(int i = 1; i <= 5; i++)
        {
            for(int j = 1; j <= 5; j++)
            {
                for(int k = 1; k <= 5; k++)
                {
                    bool test = possClues[0].test(new int[] { i, j, k });
                    if (test)
                        possDigits.Add(new int[] { i, j, k });
                }
            }
        }
        while(possDigits.Count > 1)
        {
            for(int i = 0; i < possDigits.Count; i++)
            {
                bool test = possClues[index].test(possDigits[i]);
                if (!test)
                    possDigits.RemoveAt(i--);
            }
            index++;
        }
        List<Clue> selectedClues = new List<Clue>();
        for (int i = 0; i < index; i++)
            selectedClues.Add(possClues[i]);
        return selectedClues;
    }
    // Removes unneeded clues
    private void removeClues(List<Clue> selected)
    {
        selected.Shuffle();
        for (int i = 0; i < selected.Count; i++)
        {
            Clue temp = selected[i];
            selected.RemoveAt(i);
            int num = getNumPoss(selected);
            if (num > 1)
                selected.Insert(i, temp);
            else
                i--;
        }
    }
    // Returns the number of possible combinations with the selected clues.
    private int getNumPoss(List<Clue> selected)
    {
        List<int[]> possDigits = new List<int[]>();
        for (int i = 1; i <= 5; i++)
        {
            for (int j = 1; j <= 5; j++)
            {
                for (int k = 1; k <= 5; k++)
                    possDigits.Add(new int[] { i, j, k });
            }
        }
        for (int i = 0; i < selected.Count; i++)
        {
            for (int j = 0; j < possDigits.Count; j++)
            {
                bool test = selected[i].test(possDigits[j]);
                if (!(test))
                    possDigits.RemoveAt(j--);
            }
        }
        return possDigits.Count;
    }
    private void setClueMats(Material blankMat, Material[] clueMats, List<Clue> clueList)
    {
        foreach (Clue clue in clueList)
        {
            string[] expressions = clue.getExpressions();
            Material[] mats = new Material[expressions.Length];
            for (int i = 0; i < expressions.Length; i++)
            {
                int index = -1;
                if (clueMatDict.TryGetValue(expressions[i], out index))
                {
                    if(index >= clueMats.Length)
                    {
                        mats[i] = blankMat;
                        Debug.LogFormat("The following expression doesn't have a Material for it: {0}", expressions[i]);
                    }
                    else
                        mats[i] = clueMats[index];
                }
                else
                {
                    mats[i] = blankMat;
                    Debug.LogFormat("The following expression doesn't have a Dictionary for it: {0}", expressions[i]);
                }
            }
            clue.setClueMat(mats);
        }
    }
    private void setSolutionMats(Material blankMat, Material[] clueMats, List<Clue> clueList)
    {
        foreach (Clue clue in clueList)
        {
            string[] expressions = clue.getSolutionExpression();
            Material[] mats = new Material[expressions.Length];
            for (int i = 0; i < expressions.Length; i++)
            {
                int index = -1;
                if (clueMatDict.TryGetValue(expressions[i], out index))
                {
                    if (index >= clueMats.Length)
                    {
                        mats[i] = blankMat;
                        Debug.LogFormat("The following expression doesn't have a Material for it: {0}", expressions[i]);
                    }
                    else
                        mats[i] = clueMats[index];
                }
                else
                {
                    mats[i] = blankMat;
                    Debug.LogFormat("The following expression doesn't have a Dictionary for it: {0}", expressions[i]);
                }
            }
            clue.setSolutionMat(mats);
        }
    }
    // Returns the List of Clues
    public List<Clue> getClues()
    {
        return clues;
    }
}
