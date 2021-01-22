using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class GridArrangement : MonoBehaviour
{
    public Transform frame;
    public GameObject input;
    public Text textField;
    public Button submit;
    public Text displayResult;

    public class Sudoku
    {
        int[,] grid;
        int gridSize;
        int squareRoot;
        int noOfCells;

        public Sudoku(int gridSize, int noOfCells)
        {
            this.gridSize = gridSize;
            this.noOfCells = noOfCells;
            double squareRootDouble = System.Math.Sqrt(gridSize);
            squareRoot = System.Convert.ToInt32(squareRootDouble);
            grid = new int[gridSize, gridSize];
        }

        public int[,] Getgrid()
        {
            return grid;
        }

        public int GetGridSize()
        {
            return gridSize;
        }

        public int RandomNumberGenerator(int num)
        {
            return UnityEngine.Random.Range(1, num + 1);
        }

        bool IsUnusedInInnerGrid(int rowStart, int colStart, int num)
        {
            for (int i = 0; i < squareRoot; i++)
                for (int j = 0; j < squareRoot; j++)
                    if (grid[rowStart + i, colStart + j] == num)
                        return false;
            return true;
        }

        public void FillInnerGrid(int row, int col)
        {
            int num;
            for (int i = 0; i < squareRoot; i++)
            {
                for (int j = 0; j < squareRoot; j++)
                {
                    do
                    {
                        num = RandomNumberGenerator(gridSize);
                    }
                    while (!IsUnusedInInnerGrid(row, col, num));
                    grid[row + i, col + j] = num;
                }
            }
        }

        public void FillDiagonalMatrix()
        {
            for (int i = 0; i < gridSize; i = i + squareRoot)
                FillInnerGrid(i, i);
        }

        public bool IsUnusedInRow(int i, int num)
        {
            for (int j = 0; j < gridSize; j++)
                if (grid[i, j] == num)
                    return false;
            return true;
        }

        public bool IsUnusedInCol(int j, int num)
        {
            for (int i = 0; i < gridSize; i++)
                if (grid[i, j] == num)
                    return false;
            return true;
        }

        public bool CheckIfNumIsSafe(int i, int j, int num)
        {
            return (IsUnusedInRow(i, num) &&
                    IsUnusedInCol(j, num) &&
                    IsUnusedInInnerGrid(i - i % squareRoot, j - j % squareRoot, num));
        }

        public bool FillRemainingGrid(int i, int j)
        {
            if (j >= gridSize && i < gridSize - 1)
            {
                i = i + 1;
                j = 0;
            }
            if (i >= gridSize && j >= gridSize)
                return true;

            if (i < squareRoot)
            {
                if (j < squareRoot)
                    j = squareRoot;
            }
            else if (i < gridSize - squareRoot)
            {
                if (j == (int)(i / squareRoot) * squareRoot)
                    j = j + squareRoot;
            }
            else
            {
                if (j == gridSize - squareRoot)
                {
                    i = i + 1;
                    j = 0;
                    if (i >= gridSize)
                        return true;
                }
            }

            for (int num = 1; num <= gridSize; num++)
            {
                if (CheckIfNumIsSafe(i, j, num))
                {
                    grid[i, j] = num;
                    if (FillRemainingGrid(i, j + 1))
                        return true;
                    grid[i, j] = 0;
                }
            }
            return false;
        }

        public void RemoveCells()
        {
            int count = noOfCells;
            while (count != 0)
            {
                int cellToRemove = RandomNumberGenerator(gridSize * gridSize);
                int i = cellToRemove / gridSize;
                int j = cellToRemove % 9;
                if (j != 0)
                    j = j - 1;
                if (grid[i, j] != 0)
                {
                    count--;
                    grid[i, j] = 0;
                }
            }
        }

        public int[,] FillValues()
        {
            int[,] gridDup = new int[gridSize, gridSize];
            FillDiagonalMatrix();
            FillRemainingGrid(0, squareRoot);
            System.Array.Copy(grid, 0, gridDup, 0, grid.Length);
            RemoveCells();
            return gridDup;
        }

    }

    public void Awake()
    {
        Sudoku sudoku = new Sudoku(9, 10);
        int[,] gridCopy = sudoku.FillValues();
        List<int> removedElements = ConstructGrid(sudoku.Getgrid(), sudoku.GetGridSize(), gridCopy);
        submit.onClick.AddListener(() => EvaluateGrid(removedElements));
    }

    public List<int> ConstructGrid(int[,] grid, int gridSize, int[,] gridCopy)
    {
        List<int> removedData = new List<int>();
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                if (grid[i, j] != 0)
                {
                    Text tempTxt = (Text)Instantiate(textField);
                    tempTxt.text = grid[i, j].ToString();
                    tempTxt.transform.SetParent(frame, false);
                }
                else
                {
                    removedData.Add(gridCopy[i, j]);
                    GameObject inputObject = Instantiate(input);
                    inputObject.transform.SetParent(frame, false);
                }
            }
        }
        return removedData;
    }

    public void EvaluateGrid(List<int> removedCells)
    {
        List<int> inputFieldData = new List<int>();
        foreach (InputField inputField in frame.GetComponentsInChildren<InputField>())
        {
            foreach (Text textField in inputField.GetComponentsInChildren<Text>())
                inputFieldData.Add(System.Convert.ToInt32(textField.text));
        }
        for (int i = 0; i < inputFieldData.Count; i++)
        {
            if (removedCells[i] != inputFieldData[i])
            {
                displayResult.text = "Invalid answer!";
                break;
            }
        }
    }
}