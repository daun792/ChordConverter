using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum ScaleType
{
    Major,
    Minor, 
    Diminished,
    Default
}

public class ChordConverter : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] TMP_InputField chordsInput;
    [SerializeField] TMP_InputField scaleInput;

    [Header("Output")]
    [SerializeField] TextMeshProUGUI analysisTMP;
    [SerializeField] TextMeshProUGUI firstTMP;

    [Header("Button")]
    [SerializeField] Button convertBtn;
    [SerializeField] Button copyBtn;
    [SerializeField] Button resetBtn;
    [SerializeField] Button diminishedCopyBtn;

    private Dictionary<int, string> RomanDic;

    private void Start()
    {
        RomanDic = new()
        {
            {1, "i"}, {2, "ii"}, {3, "iii"}, {4, "iv"},
            {5, "v"}, {6, "vi"}, {7, "vii"}
        };

        convertBtn.onClick.AddListener(OnClickConvert);
        copyBtn.onClick.AddListener(OnClickCopy);
        resetBtn.onClick.AddListener(OnClickReset);
        diminishedCopyBtn.onClick.AddListener(OnClickDiminisedCopy);
    }

    private void OnClickConvert()
    {
        string[] chords = chordsInput.text.Split(" ");
        string romanChord = string.Empty;

        if (chords == null)
        {
            Error();
        }

        char scale = scaleInput.text[0];

        foreach (var chord in chords)
        {
            char value = chord[0];
            int difference = Subtract(value, scale);
            string roman = RomanDic[difference];

            ScaleType type = CheckType(chord);

            switch (type)
            {
                case ScaleType.Major:
                    roman = roman.ToUpper();
                    break;
            }

            string rootNote;

            if (chord.Length > 1 && (chord[1] == 'b' || chord[1] == '#'))
            {
                rootNote = chord.Substring(0, 2);
            }
            else
            {
                rootNote = chord[0].ToString();
            }

            string newChord = chord.Replace(rootNote, roman);

            romanChord += newChord + " ";
        }

        analysisTMP.text = romanChord;
        firstTMP.text = romanChord.Split(" ")[0];
    }

    private int Subtract(char chord, char scale)
    {
        int result = chord - scale + 1;

        if (result < 0)
        {
            result += 7;
        }
        return result;
    }

    private ScaleType CheckType(string _chord)
    {
        if (_chord.Contains("m") || _chord.Contains("M"))
        {
            for (int i = 0; i < _chord.Length; i++)
            {
                if (_chord[i] == 'M')
                {
                    return ScaleType.Major;
                }
                else if (_chord[i] == 'm')
                {
                    return ScaleType.Minor;
                }
            }
        }
        else if (_chord.Contains("¡Æ"))
        {
            return ScaleType.Diminished;
        }

        return ScaleType.Default;
    }

    private void OnClickCopy()
    {
        GUIUtility.systemCopyBuffer = analysisTMP.text;
    }

    private void OnClickReset()
    {
        chordsInput.text = string.Empty;
        scaleInput.text = string.Empty;

        analysisTMP.text = string.Empty;
        firstTMP.text = string.Empty;
    }

    private void OnClickDiminisedCopy()
    {
        GUIUtility.systemCopyBuffer = "¡Æ";
    }

    private void Error()
    {

    }
}
