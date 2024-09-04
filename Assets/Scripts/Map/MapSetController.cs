using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MapSetController : MonoBehaviour
{
    [SerializeField] private Button[] _colUpButton;
    [SerializeField] private Button[] _colDownButton;
    [SerializeField] private Button[] _rowUpButton;
    [SerializeField] private Button[] _rowDownButton;
    [SerializeField] private TextMeshProUGUI[] _matrixText;

    private const int MAX_SIZE = 20;
    private const int MIN_SIZE = 8;

    private void Awake()
    {
        foreach (var button in _colUpButton)
        {
            button.onClick.AddListener(ColUp);
        }
        foreach (var button in _colDownButton)
        {
            button.onClick.AddListener(ColDown);
        }
        foreach (var button in _rowUpButton)
        {
            button.onClick.AddListener(RowUp);
        }
        foreach (var button in _rowDownButton)
        {
            button.onClick.AddListener(RowDown);
        }
    }
    private void ColUp()
    {
        int col = GlobalValues.COL;
        col++;
        if (col > MAX_SIZE)
            col = MAX_SIZE;
        GlobalValues.COL = col;
        UpdateUI();
    }
    private void ColDown()
    {
        int col = GlobalValues.COL;
        col--;
        if (col < MIN_SIZE)
            col = MIN_SIZE;
        GlobalValues.COL = col;
        UpdateUI();
    }
    private void RowUp()
    {
        int row = GlobalValues.ROW;
        row++;
        if (row > MAX_SIZE)
            row = MAX_SIZE;
        GlobalValues.ROW = row;
        UpdateUI();
    }
    private void RowDown()
    {
        int row = GlobalValues.ROW;
        row--;
        if (row < MIN_SIZE)
            row = MIN_SIZE;
        GlobalValues.ROW = row;
        UpdateUI();
    }
    private void UpdateUI()
    {
        foreach (var text in _matrixText)
            text.text = $"<color=red>{GlobalValues.COL}</color> X <color=red>{GlobalValues.ROW}</color>";
    }
}
