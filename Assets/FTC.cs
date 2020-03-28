using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using KModkit;
using System.Text.RegularExpressions;

public class FTC : MonoBehaviour
{
    public KMAudio Audio;
    public KMBombModule Module;
    public KMBombInfo Bomb;
    public TextMesh[] Number;
    public Renderer[] ColorChanger;
    public KMSelectable[] Buttons;
    public Color[] Color = new Color[10];

    //large souvenir dump
    bool solved;
    int stage = 0, maxStage = 3;
    List<byte> gear = new List<byte>(0);
    List<short> largeDisplay = new List<short>(0);
    List<int> sineNumber = new List<int>(0);
    List<string> gearColor = new List<string>(0), ruleColor = new List<string>(0);

    private bool _inputMode, _debug = false;
    private int _button, _gear, _moduleId;
    private float _answer;
    private double _index;

    private int[] _nixies = new int[2], _mainDisplays = new int[2], _colorNums = new int[4], _nixieCorrect = new int[2], _storedValues;
    private double[] _tempStorage = new double[6];
    private IEnumerable<string> _solvable;

    readonly static private string[] _colors = { "Red", "Orange", "Yellow", "Green", "Cyan", "Blue", "Purple", "Pink", "Maroon", "Azure", "Gray" };
    readonly static private string[] _ignore = { "Forget The Colors", "14", "Bamboozling Time Keeper", "Brainf---", "Forget Enigma", "Forget Everything", "Forget It Not", "Forget Me Not", "Forget Me Later", "Forget Perspecive", "Forget Them All", "Forget This", "Forget Us Not", "Organization", "Purgatory", "Simon Forgets", "Simons's Stages", "Souvenir", "Tallordered Keys", "The Time Keeper", "The Troll", "The Very Annoying Button", "Timing Is Everything", "Turn The Key", "Ultimate Custom Night", "Übermodule" };

    private static int _moduleIdCounter = 1;

    void Awake()
    {
        _moduleId = _moduleIdCounter++;
        for (int i = 0; i < Buttons.Length; i++)
        {
            var btn = Buttons[i];
            btn.OnInteract += delegate 
            {
                HandlePress(btn);
                return false;
            };
        }
    }

    void Start()
    {
        //if debug is turned on, max stage should equal the initial value assigned, otherwise set it to the proper value
        if (!_debug)
            maxStage = Bomb.GetSolvableModuleNames().Where(a => !_ignore.Contains(a)).Count();

        _storedValues = new int[maxStage + 1];

        //proper grammar!!
        if (maxStage != 1)
            Debug.LogFormat("[Forget The Colors #{0}]: Welcome to FTC - On this bomb we will have {1} stages.", _moduleId, maxStage);

        else
            Debug.LogFormat("[Forget The Colors #{0}]: Welcome to FTC - On this bomb we will have {1} stage.", _moduleId, maxStage);

        Audio.PlaySoundAtTransform("start", Buttons[2].transform);
        StartCoroutine(Generate());
    }

    void FixedUpdate()
    {
        //if there are more stages left, generate new stage
        if (stage < Bomb.GetSolvedModuleNames().Where(a => !_ignore.Contains(a)).Count() && !solved)
        {
            stage++;
            StartCoroutine(Generate());
        }
    }

    IEnumerator Generate()
    {
        //if solved, don't generate
        if (solved)
            StopAllCoroutines();

        //plays sound
        if (stage != 0)
            Audio.PlaySoundAtTransform("nextStage", Buttons[2].transform);

        //if this is the submission/final stage
        if (stage == maxStage || _answer != 0)
        {
            for (int i = 0; i < _nixies.Length; i++)
            {
                _mainDisplays[i] = 0;
                _nixies[i] = 0;
            }

            for (int i = 0; i < _colorNums.Length; i++)
                _colorNums[i] = 10;

            _gear = 0;

            Render();
            CalculateAnswer();
            StopAllCoroutines();
        }

        //if it's supposed to be randomising
        if (!solved && stage != maxStage && _answer == 0)
        {
            //stage 0: runs 20 times, stage 1+: runs 10 times
            for (int i = 0; i < 10 + ((Mathf.Clamp(stage, 0, 1) - 1) * -10); i++)
            {
                for (int j = 0; j < _nixies.Length; j++)
                    _nixies[j] = UnityEngine.Random.Range(0, 10);

                for (int j = 0; j < _colorNums.Length; j++)
                    _colorNums[j] = UnityEngine.Random.Range(0, 10);

                _mainDisplays[0] = UnityEngine.Random.Range(0, 991);
                _mainDisplays[1] = UnityEngine.Random.Range(0, 100);
                _gear = UnityEngine.Random.Range(0, 10);

                Render();

                yield return new WaitForSeconds(.075f);
            }

            _mainDisplays[1] = stage;

            //souvenir
            gear.Add((byte)_gear);
            largeDisplay.Add((short)_mainDisplays[0]);
            gearColor.Add(_colors[_colorNums[3]]);

            Render();
        }

        //if it's not last stage
        if (stage != maxStage && _answer == 0)
        {
            Debug.LogFormat("[Forget The Colors #{0}]: Stage {1}: The large display is {2}. The Colors are {3}, {4}, and {5}. The Nixie numbers are {6}{7}, and the gear is numbered {8} and colored {9}.", _moduleId, stage, _mainDisplays[0], _colors[_colorNums[0]], _colors[_colorNums[1]], _colors[_colorNums[2]], _nixies[0], _nixies[1], _gear, _colors[_colorNums[3]]);
            Calculate();
            StopCoroutine(Generate());
        }
    }

    void HandlePress(KMSelectable btn)
    {
        if (solved)
            return;

        //if it's not ready for input, strike
        if (!_inputMode && !_debug)
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
            //debugging
            if (_debug && stage != maxStage)
            {
                stage++;
                StartCoroutine(Generate());
            }

            //if both correct
            else if (_nixies[0] == _nixieCorrect[0] && _nixies[1] == _nixieCorrect[1])
            {
                solved = true;
                GetComponent<KMBombModule>().HandlePass();
                Audio.PlaySoundAtTransform("keySuccess", Buttons[2].transform);
                Audio.PlaySoundAtTransform("solved", Buttons[2].transform);

                Debug.LogFormat("[Forget The Colors #{0}]: Congration, you did it.", _moduleId);
            }

            //if either incorrect
            else
            {
                Audio.PlaySoundAtTransform("key", Buttons[2].transform);
                GetComponent<KMBombModule>().HandleStrike();
                Debug.LogFormat("[Forget The Colors #{0}]: You did a goodn't, you submitted {1}{2} when I expected {3}{4}.", _moduleId, _nixies[0], _nixies[1], _nixieCorrect[0], _nixieCorrect[1]);
            }
        }

        Render();
    }

    private void Render()
    {
        //render initial displays
        Number[0].text = _mainDisplays[0].ToString();
        Number[1].text = (_mainDisplays[1] % 100).ToString();

        //if the large display lacks 3 characters, add 0's
        for (int i = 0; i < 2; i++)
            while (Number[i].text.Length < 3 - i)
                Number[i].text = Number[i].text.Insert(0, "0");

        //set nixies
        for (int i = 0; i < _nixies.Length; i++)
            Number[i + 2].text = _nixies[i].ToString();

        //set gear
        Number[4].text = _gear.ToString();

        //set colors
        for (int i = 0; i < ColorChanger.Length; i++)
            ColorChanger[i].material.color = Color[_colorNums[i]];
    }

    void CalculateAnswer()
    {
        for (int i = 0; i < stage; i++)
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

        //this will run through the changes applied to both nixie tubes during step 1 of second page on manual
        for (int i = 0; i < _colorNums.Length - 1; i++)
            //each digit rule
            switch (_colorNums[i])
            {
                case 0:
                    _tempStorage[3] += 5;
                    _tempStorage[4] -= 1;
                    break;

                case 1:
                    _tempStorage[3] -= 1;
                    _tempStorage[4] -= 6;
                    break;

                case 2:
                    _tempStorage[3] += 3;
                    break;

                case 3:
                    _tempStorage[3] += 7;
                    _tempStorage[4] -= 4;
                    break;

                case 4:
                    _tempStorage[3] -= 7;
                    _tempStorage[4] -= 5;
                    break;

                case 5:
                    _tempStorage[3] += 8;
                    _tempStorage[4] += 9;
                    break;

                case 6:
                    _tempStorage[3] += 5;
                    _tempStorage[4] -= 9;
                    break;

                case 7:
                    _tempStorage[3] -= 9;
                    _tempStorage[4] += 4;
                    break;

                case 8:
                    _tempStorage[4] += 7;
                    break;

                case 9:
                    _tempStorage[3] -= 3;
                    _tempStorage[4] += 5;
                    break;
            }

        //modulo
        _tempStorage[3] = (_tempStorage[3] + 100) % 10;
        _tempStorage[4] = (_tempStorage[4] + 100) % 10;

        //new gear = calculated nixies + gear
        _tempStorage[5] = _tempStorage[3] + _tempStorage[4] + _gear;
        _tempStorage[5] %= 10;

        //move the index up and down according to calculated nixies
        _index = _colorNums[3] - _tempStorage[3] + _tempStorage[4];

        //modulo
        _index = (_index + 10) % 10;

        //get serial
        List<char> serial = Bomb.GetSerialNumber().ToList();

        //this will run through the changes applied to the gear during step 2 of second page on manual
        switch ((int)_index)
        {
            case 0:
                _tempStorage[5] += Bomb.GetBatteryCount();
                break;

            case 1:
                _tempStorage[5] -= Bomb.GetPortCount();
                break;

            case 2:
                _tempStorage[5] += serial.Last();
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

        ruleColor.Add(_colors[(int)_index]);

        //modulo
        while (_tempStorage[5] < 0)
            _tempStorage[5] += 10;

        _tempStorage[5] %= 10;

        //calculate final answer for that stage
        _tempStorage[0] = Math.Floor(Math.Abs(Math.Cos(_mainDisplays[0] * Mathf.Deg2Rad) * Math.Pow(10, 5)));
        _tempStorage[2] = Math.Truncate(Math.Sin(int.Parse(string.Concat(_tempStorage[3], _tempStorage[4], _tempStorage[5])) * Mathf.Deg2Rad) * Math.Pow(10, 5));
        _tempStorage[1] = _tempStorage[0] + _tempStorage[2];

        _storedValues[stage] = (int)_tempStorage[1] % 100000;
        sineNumber.Add((int)_tempStorage[2]);

        Debug.LogFormat("[Forget The Colors #{0}]: Stage {1}: The stage number is {2}, the calculated values of the Nixie tubes are {3} and {4}, the rule applied was for Step 2 was {5}, the calculated gear number is {6}, the modifier (sine) for the stage number is {7}.", _moduleId, stage, _tempStorage[0], _tempStorage[3], _tempStorage[4], ruleColor.Last(), _tempStorage[5], _tempStorage[2]);
        Debug.LogFormat("[Forget The Colors #{0}]: Stage {1}: The final value for this stage is {2}.", _moduleId, stage, _storedValues[stage]);
    }

    private bool IsValid(string par)
    {
        //if number is 00-99, return true, otherwise return false
        byte b;
        if (byte.TryParse(par, out b))
            return b < 100;

        return false;
    }

#pragma warning disable 414
    private readonly string TwitchHelpMessage = @"!{0} submit <##> (Cycles through both nixies to match '##', then hits submit. | Valid numbers are from 0-99)";
#pragma warning restore 414

    IEnumerator ProcessTwitchCommand(string command)
    {
        string[] buttonPressed = command.Split(' ');

        //if command is formatted correctly
        if (Regex.IsMatch(buttonPressed[0], @"^\s*submit\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            yield return null;

            //if command has no parameters
            if (buttonPressed.Length < 2)
                yield return "sendtochaterror Please specify the value to submit! (Valid: 0-99)";

            //if command has too many parameters
            else if (buttonPressed.Length > 2)
                yield return "sendtochaterror Too many parameters! Please submit only a single 2-digit number.";

            //if command has an invalid parameter
            else if (!IsValid(buttonPressed.ElementAt(1)))
                yield return "sendtochaterror Invalid number! Only values 0-99 are valid.";

            //if command is valid, push button accordingly
            else
            {
                byte user = 0;
                byte.TryParse(buttonPressed[1], out user);

                //splits values
                byte[] values = new byte[2] { (byte)(user / 10), (byte)(user % 10) };

                //submit answer only if it's ready
                if (_inputMode)
                    for (int i = 0; i < Buttons.Length - 1; i++)
                    {
                        //keep pushing until button value is met by player
                        while (_nixies[i] != values[i])
                        {                            
							yield return new WaitForSeconds(0.5f);
                            Buttons[i].OnInteract();
                        }
                    }

                //key
				yield return new WaitForSeconds(0.8f);
                Buttons[2].OnInteract();
            }
        }
    }

    IEnumerator TwitchHandleForcedSolve()
    {
        Debug.LogFormat("[Forget The Colors #{0}]: Thank you for attempting FTC. You gave up on stage {1}", _moduleId, stage);

        while (!_inputMode)
            yield return true;

        yield return new WaitForSeconds(1f);

        for (int i = 0; i < 2; i++)
            while (_nixieCorrect[i] != _nixies[i])
            {
                Buttons[i].OnInteract();
                yield return new WaitForSeconds(.1f);
                Render();
            }

        if (_nixies[0] == _nixieCorrect[0] && _nixies[1] == _nixieCorrect[1])
        {
            yield return new WaitForSeconds(.5f);
            Buttons[2].OnInteract();
        }
        yield return null;
    }
}