using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KModkit;
using System.Linq;
using System;

public class FTC : MonoBehaviour
{
    public KMAudio Audio;
    public KMBombInfo Bomb;
    public TextMesh[] Number;
    public Renderer[] ColorChanger;
    public KMSelectable[] Buttons;
    public Color[] Color = new Color[10];

    private bool _inputMode, _solved;
    private int _button, _gear, _moduleId, _stage = 0, _maxStage = 0;
    private float _answer;
    private double _index;

    private List<char> _serial;
    private int[] _nixies = new int[2], _mainDisplays = new int[2], _colorNums = new int[4], _nixieCorrect = new int[2], _storedValues;
    private double[] _tempStorage = new double[6];
    private IEnumerable<string> _solvable;

    readonly static private string[] _colors = { "Red", "Orange", "Yellow", "Green", "Cyan", "Blue", "Purple", "Pink", "Maroon", "Azure", "Gray" };
    readonly static private string[] _ignore = { "Forget The Colors", "14", "Bamboozling Time Keeper", "Brainf---", "Forget Enigma", "Forget Everything", "Forget It Not", "Forget Me Not", "Forget Me Later", "Forget Perspecive", "Forget Them All", "Forget This", "Forget Us Not", "Organization", "Purgatory", "Simon Forgets", "Simons's Stages", "Souvenir", "Tallordered Keys", "The Time Keeper", "The Troll", "The Very Annoying Button", "Timing Is Everything", "Turn The Key", "Ultimate Custom Night", "‹bermodule" };

    private static int _moduleIdCounter = 1;

    void Awake()
    {
        _moduleId = _moduleIdCounter++;
        for (int i = 0; i < 3; i++)
        {
            var btn = Buttons[i];
            btn.OnInteract += delegate { HandlePress(btn); return false; };
        }
    }
    void HandlePress(KMSelectable btn)
    {
        if (_solved)
            return;

        //if it's not ready for input, strike
        if (!_inputMode)
        {
            Audio.PlaySoundAtTransform("key", Buttons[2].transform);
            GetComponent<KMBombModule>().HandleStrike();
            return;
        }

        //gets the specific button pushed
        var c = Array.IndexOf(Buttons, btn);

        //NOT the key
        if (c != 2)
        {
            _nixies[c] = (_nixies[c] + 1) % 10;

            Buttons[c].AddInteractionPunch();
            Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, Buttons[c].transform);
        }

        //key
        else
        {
            Audio.PlaySoundAtTransform("keySuccess", Buttons[2].transform);

            //if both correct
            if (_nixies[0] == _nixieCorrect[0] && _nixies[1] == _nixieCorrect[1])
            {
                _solved = true;
                GetComponent<KMBombModule>().HandlePass();
                Audio.PlaySoundAtTransform("solved", Buttons[2].transform);

                Debug.LogFormat("[Forget The Colors #{0}]: Congration you done it.", _moduleId);
            }

            //if either incorrect
            else
            {
                Audio.PlaySoundAtTransform("key", Buttons[2].transform);
                GetComponent<KMBombModule>().HandleStrike();
                Debug.LogFormat("[Forget The Colors #{0}]: You did a goodn't, you submitted {1}{2} when I expected {3}{4}.", _moduleId, _nixies[0], _nixies[1], _nixieCorrect[0], _nixieCorrect[1]);
            }
        }
    }

    void Start()
    {
        _serial = Bomb.GetSerialNumber().ToList();
        _maxStage = Bomb.GetSolvableModuleNames().Where(a => !_ignore.Contains(a)).Count();
        _storedValues = new int[_maxStage + 1];

        //proper grammar!!
        if (_maxStage != 1)
            Debug.LogFormat("[Forget The Colors #{0}]: Welcome to FTC - On this bomb we will have {1} stages.", _moduleId, _maxStage);

        else
            Debug.LogFormat("[Forget The Colors #{0}]: Welcome to FTC - On this bomb we will have {1} stage.", _moduleId, _maxStage);

        Audio.PlaySoundAtTransform("start", Buttons[2].transform);
        StartCoroutine(Generate());
    }

    IEnumerator Generate()
    {
        //if solved, don't generate
        if (_solved)
            StopAllCoroutines();

        //plays sound
        if (_stage != 0)
            Audio.PlaySoundAtTransform("nextStage", Buttons[2].transform);

        //if this is the submission/final stage
        if (_stage == _maxStage || _answer != 0)
        {
            for (int i = 0; i < _nixies.Length; i++)
            {
                _mainDisplays[i] = 0;
                _nixies[i] = 0;
            }

            for (int i = 0; i < _colorNums.Length; i++)
                _colorNums[i] = 10;

            _gear = 0;

            CalculateAnswer();
            StopAllCoroutines();
        }

        //if it's supposed to be randomising
        if (_stage == 0 && !_solved && _stage != _maxStage && _answer == 0)
        {
            //stage 0: runs 75 times, stage 1+: runs 25 times
            for (int i = 0; i < 25 + ((Mathf.Clamp(_stage, 0, 1) - 1) * -25); i++)
            {
                for (int j = 0; j < _nixies.Length; j++)
                    _nixies[j] = UnityEngine.Random.Range(0, 10);

                for (int j = 0; j < _colorNums.Length; j++)
                    _colorNums[j] = UnityEngine.Random.Range(0, 10);

                _mainDisplays[0] = UnityEngine.Random.Range(0, 991);
                _mainDisplays[1] = UnityEngine.Random.Range(0, 100);
                _gear = UnityEngine.Random.Range(0, 10);

                yield return new WaitForSeconds(.075f);
            }

            _mainDisplays[1] = _stage;
        }

        //if it's not last stage
        if (_stage != _maxStage && _answer == 0)
        {
            Debug.LogFormat("[Forget The Colors #{0}]: Stage {1}: The large display is {2}. The Colors are {3}, {4}, and {5}. The Nixie numbers are {6}{7}, and the gear is numbered {8} and colored {9}.", _moduleId, _stage, _mainDisplays[0], _colors[_colorNums[0]], _colors[_colorNums[1]], _colors[_colorNums[2]], _nixies[0], _nixies[1], _gear, _colors[_colorNums[3]]);
            Calculate();
            StopCoroutine(Generate());
        }
    }

    void FixedUpdate()
    {
        //render initial displays
        Number[0].text = _mainDisplays[0].ToString();
        Number[1].text = (_mainDisplays[1] % 100).ToString();

        //if the large display lacks 3 characters, add 0's
        while (Number[0].text.Length < 3)
            Number[0].text = Number[0].text.Insert(0, "0");

        //if the small display lacks 2 characters, add 0's
        while (Number[1].text.Length < 2)
            Number[1].text = Number[1].text.Insert(0, "0");

        //set nixies
        for (int i = 0; i < _nixies.Length; i++)
            Number[i + 2].text = _nixies[i].ToString();

        //set gear
        Number[4].text = _gear.ToString();

        //set colors
        for (int i = 0; i < ColorChanger.Length; i++)
            ColorChanger[i].material.color = Color[_colorNums[i]];

        //if there are more stages left, generate new stage
        if (_stage != Bomb.GetSolvedModuleNames().Count() && !_solved)
        {
            _stage++;
            StartCoroutine(Generate());
        }
    }

    void CalculateAnswer()
    {
        for (int i = 0; i < _stage; i++)
            _answer += _storedValues[i];

        _inputMode = true;

        _answer = (float)Math.Abs(_answer / Math.Pow(10, 5) % 1);
        _answer = (float)Math.Truncate(Mathf.Acos(_answer) * Mathf.Rad2Deg);

        //gets correct answer
        _nixieCorrect[0] = (int)Math.Truncate((double)_answer / 10);
        _nixieCorrect[1] = (int)Math.Truncate((double)_answer % 10);

        Debug.LogFormat("[Forget The Colors #{0}]: The expected answer is {1}.", _moduleId, _answer);
    }

    void Calculate()
    {
        for (int i = 0; i < _nixies.Length; i++)
            _tempStorage[i + 3] = _nixies[i];

        _tempStorage[5] = _gear;

        //run for each color
        for (int i = 0; i < _colorNums.Length - 1; i++)
        {
            //each digit rule
            switch (_colorNums[i])
            {
                case 0:
                    _tempStorage[3] += 5f;
                    if (_tempStorage[3] > 9) { _tempStorage[3] -= 10f; }

                    _tempStorage[4] -= 1f;
                    if (_tempStorage[4] < 0) { _tempStorage[4] += 10f; }
                    break;

                case 1:
                    _tempStorage[3] -= 1f;
                    if (_tempStorage[3] < 0) { _tempStorage[3] += 10f; }

                    _tempStorage[4] -= 6f;
                    if (_tempStorage[4] < 0) { _tempStorage[4] += 10f; }
                    break;

                case 2:
                    _tempStorage[3] += 3f;
                    if (_tempStorage[3] > 9) { _tempStorage[3] -= 10f; }
                    break;

                case 3:
                    _tempStorage[3] += 7f;
                    if (_tempStorage[3] > 9) { _tempStorage[3] -= 10f; }

                    _tempStorage[4] -= 4f;
                    if (_tempStorage[4] < 0) { _tempStorage[4] += 10; }
                    break;

                case 4:
                    _tempStorage[3] -= 7f;
                    if (_tempStorage[3] < 0) { _tempStorage[3] += 10f; }

                    _tempStorage[4] -= 5;
                    if (_tempStorage[4] < 0) { _tempStorage[4] += 10; }
                    break;

                case 5:
                    _tempStorage[3] += 8f;
                    if (_tempStorage[3] > 9) { _tempStorage[3] -= 10f; }

                    _tempStorage[4] += 9;
                    if (_tempStorage[4] > 9) { _tempStorage[4] -= 10; }
                    break;

                case 6:
                    _tempStorage[3] += 5f;
                    if (_tempStorage[3] > 9) { _tempStorage[3] -= 10f; }

                    _tempStorage[4] -= 9;
                    if (_tempStorage[4] < 0) { _tempStorage[4] += 10; }
                    break;

                case 7:
                    _tempStorage[3] -= 9f;
                    if (_tempStorage[3] < 0) { _tempStorage[3] += 10f; }

                    _tempStorage[4] += 4;
                    if (_tempStorage[4] > 9) { _tempStorage[4] -= 10; }
                    break;

                case 8:
                    _tempStorage[4] += 7;
                    if (_tempStorage[4] > 9) { _tempStorage[4] -= 10; }
                    break;

                case 9:
                    _tempStorage[3] -= 3f;
                    if (_tempStorage[3] < 0) { _tempStorage[3] += 10f; }

                    _tempStorage[4] += 5;
                    if (_tempStorage[4] > 9) { _tempStorage[4] -= 10; }
                    break;
            }
        }

        //new gear = calculated nixies + gear
        _tempStorage[5] = _tempStorage[3] + _tempStorage[4] + _gear;

        //modulo
        while (_tempStorage[5] > 9)
            _tempStorage[5] -= 10;

        //move the index up and down according to calculated nixies
        _index = _colorNums[3] - _tempStorage[3] + _tempStorage[4];

        //modulo
        if (_index < 0) { _index += 10; }
        if (_index > 9) { _index -= 10; }

        //commit index
        switch ((int)_index)
        {
            case 0:
                _tempStorage[5] += Bomb.GetBatteryCount();
                break;

            case 1:
                _tempStorage[5] -= Bomb.GetPortCount();
                break;

            case 2:
                _tempStorage[5] += _serial.Last();
                break;

            case 3:
                _tempStorage[5] -= Bomb.GetSolvedModuleNames().Count();
                break;

            case 4:
                _tempStorage[5] += Bomb.GetPortPlateCount();
                break;

            case 5:
                _tempStorage[5] -= Bomb.GetModuleNames().Count();
                break;

            case 6:
                _tempStorage[5] += Bomb.GetBatteryHolderCount();
                break;

            case 7:
                _tempStorage[5] -= Bomb.GetOnIndicators().Count();
                break;

            case 8:
                _tempStorage[5] += Bomb.GetIndicators().Count();
                break;

            case 9:
                _tempStorage[5] -= Bomb.GetOffIndicators().Count();
                break;
        }

        //modulo
        while (_tempStorage[5] < 0)
            _tempStorage[5] += 10;

        while (_tempStorage[5] > 9)
            _tempStorage[5] -= 10;

        //calculate final answer for that stage
        _tempStorage[0] = Math.Floor(Math.Abs(Math.Cos(_mainDisplays[0] * Mathf.Deg2Rad) * Math.Pow(10, 5)));

        _tempStorage[2] = Math.Truncate(Math.Sin(int.Parse(String.Concat(_tempStorage[3], _tempStorage[4], _tempStorage[5])) * Mathf.Deg2Rad) * Math.Pow(10, 5));

        _tempStorage[1] = _tempStorage[0] + _tempStorage[2];

        Debug.LogFormat("[Forget The Colors #{0}]: Stage {1}: The stage number is {2}, the calculated values of the Nixie tubes are {3} and {4}, the calculated gear number is {5}, the modifier (sine) for the stage number is {6}.", _moduleId, _stage, _tempStorage[0], _tempStorage[3], _tempStorage[4],  _tempStorage[5], _tempStorage[2]);
        
        _storedValues[_stage] = (int)_tempStorage[1] % 100000;

        Debug.LogFormat("[Forget The Colors #{0}]: Stage {1}: The final value is {2}.", _moduleId, _stage, _storedValues[_stage]);
    }
}