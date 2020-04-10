using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using KModkit;
using System.Text.RegularExpressions;
using Rnd = UnityEngine.Random;

public class FTC : MonoBehaviour
{
    public KMAudio Audio;
    public KMBombModule Module;
    public KMBossModule Boss;
    public KMBombInfo Bomb;
    public Transform Gear;
    public Transform CylinderKey;
    public TextMesh[] Number;
    public Renderer[] ColorChanger;
    public KMSelectable[] Buttons;
    public Color[] Color = new Color[10];

    //large souvenir dump
    bool solved = false;
    int stage = 0, maxStage = 5;
    List<byte> gear = new List<byte>(0);
    List<short> largeDisplay = new List<short>(0);
    List<int> sineNumber = new List<int>(0);
    List<string> gearColor = new List<string>(0), ruleColor = new List<string>(0);

    private bool _inputMode, _strike = false, _rotating = false;
    private int _button, _gear, _gearDir = 0, _currentDir = 0, _moduleId = 0;
    private float _answer, _easeSolve = 0, _easeGear = 0;
    private double _index;

    private int[] _nixies = new int[2], _mainDisplays = new int[2], _colorNums = new int[4], _nixieCorrect = new int[2], _storedValues;
    private double[] _tempStorage = new double[6];
    private IEnumerable<string> _solvable;
    
    readonly static private string[] _colors = { "Red", "Orange", "Yellow", "Green", "Cyan", "Blue", "Purple", "Pink", "Maroon", "White", "Gray" };
    readonly static private string[] _failPhrases = { "You did a goodn't", "Congratulations! You got a strike", "You have just won a free gift card containing 1 strike and no solve! In other words", "This is so sad", "This must be really embarrasing for you", "I just came back, where we again? Oh yeah", "Unsuprisingly, your 1/91 chance went unsuccessful", "Did Emik break the module or are you just bad?", "Did Cooldoom break the module or are you just bad?", "This looks like a WHITE ABORT to me", "Correct... your mistakes in the future", "?!", "‽", "The phrase \"It's just a module\" is such a weak mindset, you are okay with what happened, losing, imperfection of a craft.", "Good for you", "Have fun doing the math again", "Was that MAROON or RED?", "Are you sure the experts wrote it down correctly?", "Are you sure the defuser said it correctly?", "The key spun backwards", "THE ANSWER IS IN THE WRONG POSITION", "key.wav", "Module.HandleStrike()", "Is your calculator broken?", "Is your KTANE broken?", "A wide-screen monitor would really help here", "VR would make this easier", "E", "bruh moment", "Failed executing external process for 'Bake Runtime' job.", "Did Discord cut vital communication off?", "You failed the vibe check", "Looks like you failed your exam", "Could not find USER_ANSWER in ACTUAL_ANSWER", "nah", "noppen", "yesn't", "This is the moment where you quit out the bomb", "You just lost the game", "Noooo, why'd you do that?", "*pufferfish noises*", "I was thinking about being generous this round, it didn't change my mind though", "Have you tried turning this module on and off?", "It's been so long, since I last have seen an answer, lost to this monster", "Oof", "Yikes", "Good luck figuring out why you're wrong", "Oog", "Nice one buckaroo", ":̶.̶|̶:̶;̶  <--- Is this loss?" };
    readonly static private string[] _winPhrases = { "Hey, that's pretty good", "*intense cat noises*", "While you're at it, be sure to like, comment, favorite and subscribe", "This is oddly calming", "GG m8", "I just came back, where we again? Oh yeah", "Suprisingly, your 1/91 chance went successful", "Did Emik fix the module or are you just that good?", "Did Cooldoom fix the module or are you just that good?", "This looks like a NUT BUTTON to me", "Opposite of incorrect", "Damn, I should ban you from solving me", "You haven't forgotten the colors?", "Do you still think it's Very Hard?", "I think I'm supposed to Module.HandlePass()", "I really hope you didn't look at the logs", "I really hope you didn't use an auto-solver", "I should have just used Azure instead of White", "How many shrimps do I have to eat, before it makes my gears turn pink", "The key spun forwards", "THE ANSWER IS IN THE RIGHT POSITION", "keyCorrect.wav", "Module.HandlePass()", "Did you use a calculator?", "Did you enjoy it?", "Please rate us 5 stars in the ModuleStore at your KTaNEPhone", "Maybe I should've called myself \"Write Down Colors\"", "E", "bruh moment", "*happy music*", ":) good", "You passed the vibe check", "Looks like you passed your exam", "Successfully found USER_ANSWER in ACTUAL_ANSWER", "yes", "yesper", "non't", "This is the moment where you say \"LET'S GO!!\"", "You just won the game", "*key turned*", "opposite of bruh moment", "I was thinking about being generous this round, but you were correct anyway", ":joy: 99% IMPOSSIBLE :joy:", "Forget The Colors, is this where you want to be, I just don't get it, why do you want to stay?", "Mood", "!!", "Now go brag to your friends", "PogChamp", "Poggers", "You passed with flying colors" };
    static private string[] _ignore = { "Forget The Colors", "14", "Bamboozling Time Keeper", "Brainf---", "Forget Enigma", "Forget Everything", "Forget It Not", "Forget Me Not", "Forget Me Later", "Forget Perspective", "Forget Them All", "Forget This", "Forget Us Not", "Organization", "Purgatory", "Simon Forgets", "Simon's Stages", "Souvenir", "Tallordered Keys", "The Time Keeper", "The Troll", "The Very Annoying Button", "Timing Is Everything", "Turn The Key", "Ultimate Custom Night", "Übermodule" };
    static private int _moduleIdCounter = 1;

    void Awake()
    {
        string[] ignoredModules = Boss.GetIgnoredModules(Module, _ignore);
        if (ignoredModules != null)
            _ignore = ignoredModules;

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
        //if on unity, max stage should equal the initial value assigned, otherwise set it to the proper value
        if (!Application.isEditor)
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
        if (Gear.localRotation.y != _gearDir + _currentDir)
        {
            _easeGear += 0.025f;

            if (_inputMode)
            {
                Gear.localRotation = Quaternion.Euler(0, _currentDir % 360 * Math.Abs(CubicOut(_easeGear) - 1), 0);

                if (_easeGear > 1)
                    Gear.localRotation = Quaternion.Euler(0, 0, 0);
            }

            else
            {
                Gear.localRotation = Quaternion.Euler(0, CubicOut(_easeGear) * _gearDir + _currentDir, 0);

                if (_easeGear > 1)
                    Gear.localRotation = Quaternion.Euler(0, _gearDir + _currentDir, 0);
            }
            _rotating = _easeGear <= 1;
        }

        if (solved)
        {
            if (_easeSolve <= 1)
            {
                _easeSolve += 0.02f;

                Buttons[2].transform.localRotation = Quaternion.Euler(0, BackOut(_easeSolve) * 420, 0);
                CylinderKey.localScale = new Vector3(ElasticOut(_easeSolve) * 0.5f, 1, ElasticOut(_easeSolve) * 0.5f);
            }

            else if (_easeSolve <= 2)
            {
                _easeSolve += 0.04f;

                Buttons[2].transform.localPosition = new Vector3(0, (BackIn(_easeSolve - 1) * -3) - 0.91f, 0);
                CylinderKey.localScale = new Vector3((1 - ElasticIn(_easeSolve - 1)) / 2, 1, (1 - ElasticIn(_easeSolve - 1)) / 2);
            }

            else
                CylinderKey.localPosition = new Vector3(0, -0.2f, 0);
        }

        else if (_strike)
        {
            _easeSolve += 0.04f;
            Buttons[2].transform.localRotation = Quaternion.Euler(0, (ElasticOut(_easeSolve) - _easeSolve) * 69, 0);

            if (_easeSolve >= 1)
            {
                _strike = false;
                _easeSolve = 0;
            }
        }

        //if there are more stages left, generate new stage
        else if (stage < Bomb.GetSolvedModuleNames().Where(a => !_ignore.Contains(a)).Count() && !solved)
        {
            if (!_rotating)
            {
                _currentDir += _gearDir;
                _gearDir = Rnd.Range(180, 360);
                _easeGear = 0;
            }

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
            //reset visuals
            _nixies[0] = 0;
            _nixies[1] = 0;

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
            //stage 0: runs 40 times, stage 1+: runs 10 times
            for (int i = 0; i < 10 + ((Mathf.Clamp(stage, 0, 1) - 1) * -30); i++)
            {
                for (int j = 0; j < _nixies.Length; j++)
                    _nixies[j] = Rnd.Range(0, 10);

                for (int j = 0; j < _colorNums.Length; j++)
                    _colorNums[j] = Rnd.Range(0, 10);

                _mainDisplays[0] = Rnd.Range(0, 991);
                _mainDisplays[1] = Rnd.Range(0, 100);
                _gear = Rnd.Range(0, 10);

                Render();

                yield return new WaitForSeconds(.06f);
            }

            _mainDisplays[1] = stage;

            //souvenir
            gear.Add((byte)_gear);
            largeDisplay.Add((short)_mainDisplays[0]);
            gearColor.Add(_colors[_colorNums[3]]);

            Render();

            //if it's not last stage
            if (stage != maxStage && _answer == 0)
            {
                Calculate();
                StopCoroutine(Generate());
            }
        }
    }

    void HandlePress(KMSelectable btn)
    {
        if (solved)
            return;

        //gets the specific button pushed
        var c = Array.IndexOf(Buttons, btn);

        //if it's not ready for input, strike
        if (!_inputMode && !Application.isEditor)
        {
            Audio.PlaySoundAtTransform("key", Buttons[2].transform);
            GetComponent<KMBombModule>().HandleStrike();
            if (c == 2)
                _strike = true;
            return;
        }

        //NOT the key
        if (c != 2)
        {
            _nixies[c] = (int)Modulo(_nixies[c] + 1, 10);

            Buttons[c].AddInteractionPunch();
            Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, Buttons[c].transform);
        }

        //key
        else
        {
            //debugging
            if (Application.isEditor && stage != maxStage)
            {
                if (!_rotating)
                {
                    _currentDir += _gearDir;
                    _gearDir = Rnd.Range(180, 360);
                    _easeGear = 0;
                }

                stage++;
                StartCoroutine(Generate());
            }

            //if both correct
            else if (_nixies[0] == _nixieCorrect[0] && _nixies[1] == _nixieCorrect[1])
            {
                Audio.PlaySoundAtTransform("keySuccess", Buttons[2].transform);
                Audio.PlaySoundAtTransform("solved", Buttons[2].transform);
                solved = true;
                Module.HandlePass();

                Debug.LogFormat("[Forget The Colors #{0}]: {1}; module solved!", _moduleId, _winPhrases[Rnd.Range(0, _winPhrases.Length)]);
            }

            //if either incorrect
            else
            {
                Audio.PlaySoundAtTransform("key", Buttons[2].transform);
                _strike = true;
                Module.HandleStrike();
                Debug.LogFormat("[Forget The Colors #{0}]: {1}; you submitted {2}{3} when I expected {4}{5}.", _moduleId, _failPhrases[Rnd.Range(0, _failPhrases.Length)], _nixies[0], _nixies[1], _nixieCorrect[0], _nixieCorrect[1]);
            }
        }

        Render();
    }

    private void Render()
    {
        if (maxStage == stage)
        {
            //render turned off displays
            Number[0].text = "";
            Number[1].text = "";
        }

        else
        {
            //render initial displays
            Number[0].text = _mainDisplays[0].ToString();
            Number[1].text = Modulo(_mainDisplays[1], 100).ToString();
        }

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
        Debug.LogFormat("[Forget The Colors #{0}]: <-------=-------> FINAL STAGE <-------=------->", _moduleId);
        for (int i = 0; i < stage; i++)
        {
            _answer += _storedValues[i];
            Debug.LogFormat("[Forget The Colors #{0}]: Adding stage {1}'s {2}, now the total is {3}.", _moduleId, i, _storedValues[i], _answer);
        }

        _inputMode = true;

        _answer = (float)Modulo(Math.Abs(_answer / Math.Pow(10, 5)), 1);
        Debug.LogFormat("[Forget The Colors #{0}]: After forcing the number to be 5 digits long, the inverse cosine is {1} which returns {2}.", _moduleId, _answer, Math.Truncate(Mathf.Acos(_answer) * Mathf.Rad2Deg));
        _answer = (float)Math.Truncate(Mathf.Acos(_answer) * Mathf.Rad2Deg);
        
        //gets correct answer
        _nixieCorrect[0] = (int)Math.Truncate((double)_answer / 10);
        _nixieCorrect[1] = (int)Math.Truncate(Modulo(_answer, 10));

        Debug.LogFormat("[Forget The Colors #{0}]: <-------=-------> SOLUTION <-------=------->", _moduleId);

        Debug.LogFormat("[Forget The Colors #{0}]: The expected answer is {1}.", _moduleId, _answer);

        Debug.LogFormat("[Forget The Colors #{0}]: <-------=-------> LET'S SEE HOW THE USER DOES <-------=------->", _moduleId);
    }

    void Calculate()
    {
        Debug.LogFormat("[Forget The Colors #{0}]: <-------=-------> STAGE {1} (CALCULATED GEAR NUMBER - STEP 1) <-------=------->", _moduleId, stage);
        Debug.LogFormat("[Forget The Colors #{0}]: Stage {1}: The large display is {2}. The Colors are {3}, {4}, and {5}. The Nixie numbers are {6}{7}, and the gear is numbered {8} and colored {9}.", _moduleId, stage, _mainDisplays[0], _colors[_colorNums[0]], _colors[_colorNums[1]], _colors[_colorNums[2]], _nixies[0], _nixies[1], _gear, _colors[_colorNums[3]]);

        //get stage number
        _tempStorage[0] = Math.Floor(Math.Abs(Math.Cos(_mainDisplays[0] * Mathf.Deg2Rad) * Math.Pow(10, 5)));

        //floating point rounding fix
        if (Modulo(_tempStorage[0], 1000) == 999)
            _tempStorage[0] = Modulo(_tempStorage[0] + 1, (int)Math.Pow(10, 5));

        Debug.LogFormat("[Forget The Colors #{0}]: Stage {1}: The stage number is the absolute of the first five decimals of of cos({2}), which is {3}.", _moduleId, stage, _mainDisplays[0], _tempStorage[0]);

        for (int i = 0; i < _nixies.Length; i++)
            _tempStorage[i + 3] = _nixies[i];

        //this will run through the changes applied to both nixie tubes during step 1 of second page on manual
        for (int i = 0; i < _colorNums.Length - 1; i++)
        {
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
            Debug.LogFormat("[Forget The Colors #{0}]: Stage {1}: Applying the {2}-colored cylinder on Step 1, the nixies are now {3} and {4}.", _moduleId, stage, _colors[_colorNums[i]], _tempStorage[3], _tempStorage[4]);
        }

        //modulo
        _tempStorage[3] = Modulo(_tempStorage[3], 10);
        _tempStorage[4] = Modulo(_tempStorage[4], 10);
        Debug.LogFormat("[Forget The Colors #{0}]: Stage {1}: After modulo of both nixies by 10, their values are now {2} and {3}.", _moduleId, stage, _tempStorage[3], _tempStorage[4]);

        //new gear = calculated nixies + gear
        _tempStorage[5] = _tempStorage[3] + _tempStorage[4] + _gear;
        Debug.LogFormat("[Forget The Colors #{0}]: Stage {1}: The total number is calculated with the first nixie ({2}) + the second nixie ({3}) + the gear number ({4}) which totals to {5}.", _moduleId, stage, _tempStorage[3], _tempStorage[4], _gear, _tempStorage[5]);
        Debug.LogFormat("[Forget The Colors #{0}]: Stage {1}: After modulo of the total number {2} with 10, its value is {3}.", _moduleId, stage, _tempStorage[5], Modulo(_tempStorage[5], 10));
        _tempStorage[5] = Modulo(_tempStorage[5], 10);

        Debug.LogFormat("[Forget The Colors #{0}]: <-------=-------> STAGE {1} (CALCULATED GEAR NUMBER - STEP 2) <-------=------->", _moduleId, stage);

        //move the index up and down according to calculated nixies
        _index = _colorNums[3] - _tempStorage[3] + _tempStorage[4];
        Debug.LogFormat("[Forget The Colors #{0}]: Stage {1}: Starting on the color of the gear ({2}), move up the first calculated nixie tube ({3}) which lands on {4}, then move down the second calculated nixie tube ({5}) which lands us on {6}.", _moduleId, stage, _colors[_colorNums[3]], _tempStorage[3], _colors[(int)Modulo(_colorNums[3] - _tempStorage[3], 10)], _tempStorage[4], _colors[(int)Modulo(_colorNums[3] - _tempStorage[3] + _tempStorage[4], 10)]);

        //modulo
        _index = Modulo(_index, 10);
        double temp = _tempStorage[5];
        
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
                _tempStorage[5] += Bomb.GetSerialNumberNumbers().Last();
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

        Debug.LogFormat("[Forget The Colors #{0}]: Stage {1}: Apply the color rule {2} to the total number {3}, which gives us {4}.", _moduleId, stage, _colors[(int)_index], temp, _tempStorage[5]);

        ruleColor.Add(_colors[(int)_index]);

        //modulo
        Debug.LogFormat("[Forget The Colors #{0}]: Stage {1}: After modulo of the total number {2}, its value is {3}. This is the calculated gear number.", _moduleId, stage, _tempStorage[5], Modulo(_tempStorage[5], 10));
        _tempStorage[5] = Modulo(_tempStorage[5], 10);

        Debug.LogFormat("[Forget The Colors #{0}]: <-------=-------> STAGE {1} (CALCULATED STAGE NUMBER) <-------=------->", _moduleId, stage);

        //get the sine degrees
        Debug.LogFormat("[Forget The Colors #{0}]: Stage {1}: The nixies are {2} and {3}, and the calculated gear number is {4}, combining all of them gives us {5}", _moduleId, stage, _tempStorage[3], _tempStorage[4], _tempStorage[5], string.Concat(_tempStorage[3], _tempStorage[4], _tempStorage[5]));
        _tempStorage[2] = Math.Truncate(Math.Sin(int.Parse(string.Concat(_tempStorage[3], _tempStorage[4], _tempStorage[5])) * Mathf.Deg2Rad) * Math.Pow(10, 5));

        //floating point rounding fix
        if (Modulo(_tempStorage[2], 1000) == 999)
            _tempStorage[2] = Modulo(_tempStorage[2] + 1, (int)Math.Pow(10, 5));
        Debug.LogFormat("[Forget The Colors #{0}]: Stage {1}: The sine number is sin({2}), which gets us {3} after flooring all decimals.", _moduleId, stage, string.Concat(_tempStorage[3], _tempStorage[4], _tempStorage[5]), _tempStorage[2]);

        //get final value for the stage
        _tempStorage[1] = _tempStorage[0] + _tempStorage[2];
        _storedValues[stage] = (int)Modulo(_tempStorage[1], (int)Math.Pow(10, 5));
        Debug.LogFormat("[Forget The Colors #{0}]: Stage {1}: The final value for this stage is the initial stage number {2} and the sine number {3}, which gives the final value of {4}.", _moduleId, stage, _tempStorage[0], _tempStorage[2], _tempStorage[1]);

        sineNumber.Add((int)_tempStorage[2]);
    }

    private double Modulo(double num, int mod)
    {
        while (true)
        {
            //modulation for negatives
            if (num < 0)
            {
                num += mod;
                continue;
            }

            //modulation for positives
            else if (num >= mod)
            {
                num -= mod;
                continue;
            }

            //once it reaches here, we know it's modulated and we can return it
            return num;
        }
    }

    private float ElasticIn(float k)
    {
        if (Modulo(k, 1) == 0)
            return k;
        return -Mathf.Pow(2f, 10f * (k -= 1f)) * Mathf.Sin((k - 0.1f) * (2f * Mathf.PI) / 0.4f);
    }

    private float ElasticOut(float k)
    {
        if (Modulo(k, 1) == 0)
            return k;
        return Mathf.Pow(2f, -10f * k) * Mathf.Sin((k - 0.1f) * (2f * Mathf.PI) / 0.4f) + 1f;
    }

    private static float BackIn(float k)
    {
        float s = 1.70158f;
        return k * k * ((s + 1f) * k - s);
    }

    private static float BackOut(float k)
    {
        float s = 1.70158f;
        return (k -= 1f) * k * ((s + 1f) * k + s) + 1f;
    }

    private static float CubicOut(float k)
    {
        return 1f + ((k -= 1f) * k * k);
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
                byte[] values = new byte[2] { (byte)(user / 10), (byte)Modulo(user, 10) };

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