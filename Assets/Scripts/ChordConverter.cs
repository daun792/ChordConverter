using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

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

    [Header("TextMeshPro")]
    [SerializeField] TextMeshProUGUI errorTMP;
    [SerializeField] TextMeshProUGUI noticeTMP;

    private Dictionary<int, string> RomanDic;

    private Sequence errorSequence;
    private Sequence noticeSequence;

    #region string value
    private const string chordEmptyError = "'코드 진행' 문자열이 비어있습니다.";
    private const string scaleEmptyError = "'스케일' 문자열이 비어있습니다.";
    private const string resultEmptyError = "결과값이 없습니다.";
    private const string convertError = "문자열을 제대로 입력하세요.";

    private const string convertNotice = "변환되었습니다.";
    private const string copyNotice = "결과가 복사되었습니다.";
    private const string resetNotice = "초기화 되었습니다.";
    private const string diminishedCopyNotice = "디미니쉬드 기호가 복사되었습니다.";
    #endregion

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
        if (string.IsNullOrEmpty(chordsInput.text))
        {
            Error(chordEmptyError);
            return;
        }

        if (string.IsNullOrEmpty(scaleInput.text))
        {
            Error(scaleEmptyError);
            return;
        }

        Convert();
    }

    private void Convert()
    {
        string[] chords = chordsInput.text.Split(" ");
        string romanChord = string.Empty;

        char scale = scaleInput.text[0];

        foreach (var chord in chords)
        {
            char value = chord[0];
            int difference = Subtract(value, scale);

            if (!RomanDic.TryGetValue(difference, out string roman))
            {
                Error(convertError);
                return;
            }

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

        Notice(convertNotice);
    }

    private int Subtract(char chord, char scale)
    {
        int result = chord - scale + 1;

        return result < 0 ? result + 7 : result;
    }

    private ScaleType CheckType(string _chord)
    {
        if (_chord.Contains("M"))
        {
            return ScaleType.Major;
        }

        if (_chord.Contains("m"))
        {
            return ScaleType.Minor;
        }

        if (_chord.Contains("°"))
        {
            return ScaleType.Diminished;
        }

        return ScaleType.Default;
    }

    private void OnClickCopy()
    {
        if (string.IsNullOrEmpty(analysisTMP.text))
        {
            Error(resultEmptyError);
            return;
        }

        GUIUtility.systemCopyBuffer = analysisTMP.text;

        Notice(copyNotice);
    }

    private void OnClickReset()
    {
        chordsInput.text = string.Empty;
        scaleInput.text = string.Empty;

        analysisTMP.text = string.Empty;
        firstTMP.text = string.Empty;

        Notice(resetNotice);
    }

    private void OnClickDiminisedCopy()
    {
        GUIUtility.systemCopyBuffer = "°";

        Notice(diminishedCopyNotice);
    }

    private void Error(string _errorLog)
    {
        errorSequence.Kill();

        errorSequence = DOTween.Sequence();

        errorSequence.AppendCallback(() => 
            { 
                errorTMP.alpha = 0f;
                errorTMP.text = _errorLog;
            })
            .Append(errorTMP.DOFade(1f, 0.25f))
            .AppendInterval(1f)
            .Append(errorTMP.DOFade(0f, 0.25f));
    }

    private void Notice(string _noticeLog)
    {
        noticeSequence.Kill();

        noticeSequence = DOTween.Sequence();

        noticeSequence.AppendCallback(() =>
            {
                noticeTMP.alpha = 0f;
                noticeTMP.text = _noticeLog;
            })
            .Append(noticeTMP.DOFade(1f, 0.25f))
            .AppendInterval(1f)
            .Append(noticeTMP.DOFade(0f, 0.25f));
    }
}
