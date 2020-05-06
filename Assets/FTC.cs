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
    //import assets
    public KMAudio Audio;
    public KMBombModule Module;
    public KMBossModule Boss;
    public KMBombInfo Bomb;
    public KMColorblindMode Colorblind;
    public KMRuleSeedable Rule;
    public Transform Gear, CylinderKey;
    public Transform[] ColorblindCylinder;
    public TextMesh SmallBackground;
    public TextMesh[] Number;
    public Renderer[] ColorChanger;
    public KMSelectable[] Buttons;
    public Texture[] Texture = new Texture[10];

    //large souvenir dump
    bool solved = false;
    int stage = 0, maxStage = 3;
    List<byte> gear = new List<byte>(0);
    List<short> largeDisplay = new List<short>(0);
    List<int> sineNumber = new List<int>(0);
    List<string> gearColor = new List<string>(0), ruleColor = new List<string>(0);

    //variables for solving
    private bool _inputMode, _strike = false, _rotating = false, _colorblind;
    private byte _debugSelect = 0;
    private int _gear, _gearDir = 0, _currentDir = 0, _moduleId = 0;
    private int[] _nixies = new int[2], _colorNums = new int[4], _storedValues;
    private float _answer, _easeSolve = 0, _easeGear = 0;
    private double _index;

    //temporary storage
    readonly private int[] _mainDisplays = new int[2], _nixieCorrect = new int[2];
    readonly private double[] _tempStorage = new double[6];

    //global attributes
    static private int _moduleIdCounter = 1;
    static readonly private IEnumerable<string> _solvable;
    static private Rule[][] _rules;
    static private string[] _ignore = { "Forget The Colors", "14", "Bamboozling Time Keeper", "Brainf---", "Forget Enigma", "Forget Everything", "Forget It Not", "Forget Me Not", "Forget Me Later", "Forget Perspective", "Forget Them All", "Forget This", "Forget Us Not", "Organization", "Purgatory", "Simon Forgets", "Simon's Stages", "Souvenir", "Tallordered Keys", "The Time Keeper", "The Troll", "The Very Annoying Button", "Timing Is Everything", "Turn The Key", "Ultimate Custom Night", "Übermodule" };

    //logging
    readonly static private string[] _colors = { "Red", "Orange", "Yellow", "Green", "Cyan", "Blue", "Purple", "Pink", "Maroon", "White", "Gray" };
    readonly static private string[] _failPhrases = { "You did a goodn't", "Congratulations! You got a strike", "You have just won a free gift card containing 1 strike and no solve! In other words", "This is so sad", "This must be really embarrasing for you", "I just came back, where we again? Oh yeah", "Unsuprisingly, your 1/91 chance went unsuccessful", "Did Emik break the module or are you just bad?", "Did Cooldoom break the module or are you just bad?", "This looks like a WHITE ABORT to me", "Correct... your mistakes in the future", "?!", "‽", "The phrase \"It's just a module\" is such a weak mindset, you are okay with what happened, striking, imperfection of a solve", "Good for you", "Have fun doing the math again", "Was that MAROON or RED?", "Are you sure the experts wrote it down correctly?", "Are you sure the defuser said it correctly?", "The key spun backwards", "THE ANSWER IS IN THE WRONG POSITION", "key.wav", "Module.HandleStrike()", "Is your calculator broken?", "Is your KTANE broken?", "A wide-screen monitor would really help here", "VR would make this easier", "A mechanical keyboard would make this easier", "A \"gaming mouse\" would make this easier", "E", "bruh moment", "Failed executing external process for 'Bake Runtime' job", "Did Discord cut vital communication off?", "You failed the vibe check", "Looks like you failed your exam", "Could not find USER_ANSWER in ACTUAL_ANSWER", "nah", "noppen", "yesn't", "This is the moment where you quit out the bomb", "You just lost the game", "Noooo, why'd you do that?", "*pufferfish noises*", "I was thinking about being generous this round, it didn't change my mind though", "Have you tried turning this module on and off?", "It's been so long, since I last have seen an answer, lost to this monster", "Oof", "Yikes", "Good luck figuring out why you're wrong", "Oog", "Nice one buckaroo", ":̶.̶|̶:̶;̶  <--- Is this loss?", "Oh, you got it wrong? Report it as a bug because it's definitely not your fault", "I'm not rated \"Very Hard\" for no reason after all", "Forget The Colors be like: cringe", "The manual said I is Pink, are you colorblind?", "Not cool, meet your doom", "What were you thinking!?", "Emmm, ik you messed up somewhere", "You should double check that part where you messed up", "Looks like the expert chose betray", "At least you've solved the other modules", "Did you even read the manual?", "The module's broken? No I'm not! What's 9+10? 18.9992", "When I shred, I shred using the entire bomb. But since you SUCK, you will only need this module, and zie key button", "ALT+F4", "Did you seriously mistake me for Forget Everything?", "I was kidding when I told you to Forget The Colors, I guess sarcasm didn't come through that time...", "The Defuser expired", "The Expert expired", "You just got bamboozl- ah, wrong module", "Module rain. Some stay solved and others feel the pain. Module rain. 3-digit displays will die before the sin()", "DEpvQ0klM93dC8GMWAo5TaYGeWCZfT8Vq1qNY6o     + // /", "mood", "This message should not appear. I'll be disappointed at the Defuser if it does", "Did you forget about the 0 solvable module unicorn?" };
    readonly static private string[] _winPhrases = { "Hey, that's pretty good", "*intense cat noises*", "While you're at it, be sure to like, comment, favorite and subscribe", "This is oddly calming", "GG m8", "I just came back, where we again? Oh yeah", "Suprisingly, your 1/91 chance went successful", "Did Emik fix the module or are you just that good?", "Did Cooldoom fix the module or are you just that good?", "This looks like a NUT BUTTON to me", "Opposite of incorrect", "Damn, I should ban you from solving me", "You haven't forgotten the colors?", "Do you still think it's Very Hard?", "I think I'm supposed to Module.HandlePass()", "I really hope you didn't look at the logs", "I really hope you didn't use an auto-solver", "I should have just used Azure instead of White", "How many shrimps do I have to eat, before it makes my gears turn pink", "The key spun forwards", "THE ANSWER IS IN THE RIGHT POSITION", "keyCorrect.wav", "Module.HandlePass()", "Did you use a calculator?", "Did you enjoy it?", "Please rate us 5 stars in the ModuleStore at your KTaNEPhone", "Alexa, play the victory tune", "VICTORY", "Maybe I should've called myself \"Write Down Colors\"", "E", "bruh moment", "*happy music*", ":) good", "You passed the vibe check", "Looks like you passed your exam", "Successfully found USER_ANSWER in ACTUAL_ANSWER", "yes", "yesper", "non't", "This is the moment where you say \"LET'S GO!!\"", "You just won the game", "*key turned*", "opposite of bruh moment", "I was thinking about being generous this round, but you were correct anyway", ":joy: 99% IMPOSSIBLE :joy:", "Forget The Colors, is this where you want to be, I just don't get it, why do you want to stay?", "Mood", "!!", "Now go brag to your friends", "PogChamp", "Poggers", "You passed with flying colors", "Oh, you got it right? Report it as a bug because I'm too easy, y'know?", "I agree, I'm just as easy as The Simpleton right beside me!", "Forget The Colors says: uncringe", "That seemed to easy for you, was colorblind enabled?", "And now, Souvenir", "I hope you wrote down the edgework-based rule for the first stage", "Emmm, ik that's correct", "Can you really say you've disabled Colorblind mode when you have a transparent bomb casing?", "Looks like the expert chose ally", "At least it's solved", "Clip it! Somebody highlight that or somebody clip that!", "Was I designed to be solved? Can't remember", "SPIINNN", "*roll credits*", "Hey! Your buttons are sorted- wait wrong module", "Do you have 200IQ or something?", "How would you have felt if I decided to strike?", "The module expired", "Forget The Colors expired", "The bomb expired", "A winner is you!", "All your module are belong to us", "BOB STOLE MY KEY", "Defuser achieved rank #1 on Being Cool:tm:" };

    void Awake()
    {
        //boss module handler
        string[] ignoredModules = Boss.GetIgnoredModules(Module, _ignore);
        if (ignoredModules != null)
            _ignore = ignoredModules;

        //establish buttons
        _moduleId = _moduleIdCounter++;
        for (byte i = 0; i < Buttons.Length; i++)
        {
            KMSelectable btn = Buttons[i];
            btn.OnInteract += delegate
            {
                HandlePress(btn);
                return false;
            };
        }
    }

    void Start()
    {
        //enables colorblind mode if needed
        _colorblind = Colorblind.ColorblindModeActive;

        //gets seed
        MonoRandom rnd = Rule.GetRNG();
        Debug.LogFormat("[Forget The Colors #{0}]: Using rule seed: {1}", _moduleId, rnd.Seed);
        if (rnd.Seed == 1)
            _rules = null;
        else
        {
            //establishes new variable
            _rules = new Rule[2][];
            _rules[0] = new Rule[20];
            _rules[1] = new Rule[10];

            //applies rule seeding for cylinders
            for (byte i = 0; i < 20; i++)
                _rules[0][i] = new Rule { Cylinder = (byte)rnd.Next(10), Parameter = (byte)rnd.Next(5) };

            //applies rule seeding for edgework
            for (byte i = 0; i < 10; i++)
                _rules[1][i] = new Rule { Edgework = (byte)rnd.Next(21), Parameter = (byte)rnd.Next(5) };
        }

        //if on unity, max stage should equal the initial value assigned, otherwise set it to the proper value
        if (!Application.isEditor)
            maxStage = Bomb.GetSolvableModuleNames().Where(a => !_ignore.Contains(a)).Count();
        
        //length needs to be as long as there are maximum stages
        _storedValues = new int[maxStage + 1];

        //proper grammar!!
        if (maxStage != 1)
            Debug.LogFormat("[Forget The Colors #{0}]: Welcome to FTC - On this bomb we will have {1} stages.", _moduleId, maxStage);

        else
            Debug.LogFormat("[Forget The Colors #{0}]: Welcome to FTC - On this bomb we will have {1} stage.", _moduleId, maxStage);

        //notice in case if i accidentally make _winPhrases and _failPhrases unbalanced
        if (_winPhrases.Length != _failPhrases.Length)
            Debug.LogFormat("[Forget The Colors #{0}]: If you see this message, it means that there are an unbalanced amount of flavor text when you strike or solve! Length of strike: {1}. Length of solve: {2}", _moduleId, _failPhrases.Length, _winPhrases.Length);

        //begin module
        Audio.PlaySoundAtTransform("start", Buttons[2].transform);

        if (!Application.isEditor)
            StartCoroutine(Generate());

        //show that it's debug mode
        else
        {
            Number[1].fontSize = 35;
            Number[1].text = "DEBUG";
            SmallBackground.text = "";
            Render();
        }
    }

    void FixedUpdate()
    {
        //spin to next destination, every solve will give a new angle clockwise to itself
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

        //when finished, spin counter-clockwise to the nearest neutral position
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

        //failed key spin
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
            _gear = 0;

            for (byte i = 0; i < _colorNums.Length; i++)
                _colorNums[i] = 10;

            Render();
            CalculateAnswer();
            StopAllCoroutines();
        }

        //if it's supposed to be randomising
        if (!solved && stage != maxStage && _answer == 0)
        {
            //stage 0: runs 40 times, stage 1+: runs 10 times
            for (byte i = 0; i < 10 + ((Mathf.Clamp(stage, 0, 1) - 1) * -30); i++)
            {
                _mainDisplays[0] = Rnd.Range(0, 991);
                _mainDisplays[1] = Rnd.Range(0, 100);
                _gear = Rnd.Range(0, 10);

                for (byte j = 0; j < _nixies.Length; j++)
                    _nixies[j] = Rnd.Range(0, 10);

                for (byte j = 0; j < _colorNums.Length; j++)
                    _colorNums[j] = Rnd.Range(0, 10);

                Render();

                yield return new WaitForSeconds(.06f);
            }

            //set stage number to display
            _mainDisplays[1] = stage;

            //souvenir
            gear.Add((byte)_gear);
            largeDisplay.Add((short)_mainDisplays[0]);
            gearColor.Add(_colors[_colorNums[3]]);

            Render();

            //if it's not last stage
            if (stage != maxStage && _answer == 0 && !Application.isEditor)
            {
                Calculate();
                StopCoroutine(Generate());
            }
        }
    }

    void HandlePress(KMSelectable btn)
    {
        //if solved, buttons and key should do nothing
        if (solved)
            return;

        //gets the specific button pushed
        byte c = (byte)Array.IndexOf(Buttons, btn);

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
            //complete debugging
            if (Application.isEditor && maxStage != stage)
            {
                //right nixie changes value selected by one
                if (c == 1)
                    switch (_debugSelect)
                    {
                        case 0: _mainDisplays[0] = (int)Modulo(_mainDisplays[0] + 100, 1000); break;
                        case 1: _mainDisplays[0] = (int)Modulo(_mainDisplays[0] + 10, 1000); break;
                        case 2: _mainDisplays[0] = (int)Modulo(_mainDisplays[0] + 1, 1000); break;
                        case 3: _colorNums[0] = (int)Modulo(_colorNums[0] + 1, 10); break;
                        case 4: _colorNums[1] = (int)Modulo(_colorNums[1] + 1, 10); break;
                        case 5: _colorNums[2] = (int)Modulo(_colorNums[2] + 1, 10); break;
                        case 6: _gear = (int)Modulo(_gear + 1, 10); break;
                        case 7: _colorNums[3] = (int)Modulo(_colorNums[3] + 1, 10); break;
                        case 8: _nixies[0] = (int)Modulo(_nixies[0] + 1, 10); break;
                        case 9: _nixies[1] = (int)Modulo(_nixies[1] + 1, 10); break;
                    }

                //left nixie changes which value is selected
                else
                    _debugSelect = (byte)Modulo(_debugSelect + 1, 10);
                
                Render();

                switch (_debugSelect)
                {
                    case 0: Number[1].text = "large100"; break;
                    case 1: Number[1].text = "large10"; break;
                    case 2: Number[1].text = "large1"; break;
                    case 3: Number[1].text = "cyl1"; break;
                    case 4: Number[1].text = "cyl2"; break;
                    case 5: Number[1].text = "cyl3"; break;
                    case 6: Number[1].text = "gearNum"; break;
                    case 7: Number[1].text = "gearCol"; break;
                    case 8: Number[1].text = "nixieL"; break;
                    case 9: Number[1].text = "nixieR"; break;
                }
            }

            else
            {
                _nixies[c] = (int)Modulo(_nixies[c] + 1, 10);

                Buttons[c].AddInteractionPunch();
                Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, Buttons[c].transform);
            }
        }

        //key
        else
        {
            //debugging
            if (Application.isEditor && stage != maxStage)
            {
                Calculate();

                if (!_rotating)
                {
                    _currentDir += _gearDir;
                    _gearDir = Rnd.Range(180, 360);
                    _easeGear = 0;
                }
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

        if (!Application.isEditor)
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
            if (!Application.isEditor)
                Number[1].text = Modulo(_mainDisplays[1], 100).ToString();
        }

        //if the large display lacks 3 characters, add 0's
        for (byte i = 0; i < 2; i++)
            while (Number[i].text.Length < 3 - i)
                Number[i].text = Number[i].text.Insert(0, "0");

        //set nixies
        for (byte i = 0; i < _nixies.Length; i++)
            Number[i + 2].text = _nixies[i].ToString();

        //set gear
        Number[4].text = _gear.ToString();
        Number[4].characterSize = 0.1f - (Convert.ToByte(_colorblind) * Convert.ToByte(maxStage != stage) * 0.04f);

        if (_colorblind && maxStage != stage)
        {
            if (_colorNums[3] != 7)
                Number[4].text += _colors[_colorNums[3]].First();
            else
                Number[4].text += 'I';
        }

        //set colors
        for (byte i = 0; i < ColorChanger.Length; i++)
        {
            ColorChanger[i].material.mainTexture = Texture[_colorNums[i]];
            ColorChanger[i].material.SetTextureOffset("_MainTex", new Vector2(0.5f * Convert.ToByte(_colorblind) * Convert.ToByte(maxStage != stage), -0.04f));
        }
        ColorChanger[3].material.SetTextureScale("_MainTex", new Vector2(0, 0));

        //deletes cylinders if needed
        for (byte i = 0; i < ColorblindCylinder.Length; i++)
        {
            ColorblindCylinder[i].localRotation = new Quaternion(90 * Convert.ToByte(_colorblind) * Convert.ToByte(maxStage != stage), -90, 0, 0);
        }
    }

    void CalculateAnswer()
    {
        Debug.LogFormat("[Forget The Colors #{0}]: <-------=-------> FINAL STAGE ~ ARCCOSINE <-------=------->", _moduleId);
        //adds all of the values
        for (byte i = 0; i < stage; i++)
        {
            _answer += _storedValues[i];
            Debug.LogFormat("[Forget The Colors #{0}]: Adding stage {1}'s {2}, the total is now {3}.", _moduleId, i, _storedValues[i], _answer);
        }

        //turns to decimal number
        _answer = (float)Modulo(Mathf.Abs(_answer) / Math.Pow(10, 5), 1);

        //allow inputs in the module
        _inputMode = true;

        Debug.LogFormat("[Forget The Colors #{0}]: After forcing the number to be 5 digits long, the arccosine is {1} which returns {2}.", _moduleId, _answer, Math.Truncate(Mathf.Acos(_answer) * Mathf.Rad2Deg));
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
        Debug.LogFormat("[Forget The Colors #{0}]: <-------=-------> STAGE {1} (CALCULATED NIXIES ~ FIRST TABLE) <-------=------->", _moduleId, stage);
        Debug.LogFormat("[Forget The Colors #{0}]: Stage {1}: The large display is {2}. The colored cylinders (left-to-right) are {3}, {4}, and {5}. The nixie numbers are {6}{7}. The gear is numbered {8} and colored {9}.", _moduleId, stage, _mainDisplays[0], _colors[_colorNums[0]], _colors[_colorNums[1]], _colors[_colorNums[2]], _nixies[0], _nixies[1], _gear, _colors[_colorNums[3]]);

        for (byte i = 0; i < _nixies.Length; i++)
            _tempStorage[i + 3] = _nixies[i];

        if (_rules == null)
        {
            //this will run through the changes applied to both nixie tubes during step 1 of second page on manual
            for (byte i = 0; i < _colorNums.Length - 1; i++)
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
                Debug.LogFormat("[Forget The Colors #{0}]: Stage {1}: Applying the {2}-colored cylinder on the first table, the nixies are now {3} and {4}.", _moduleId, stage, _colors[_colorNums[i]], _tempStorage[3], _tempStorage[4]);
            }
        }
        else
        {
            for (byte i = 0; i < _colorNums.Length - 1; i++)
            {
                Rule rule = _rules[0][_colorNums[i]];

                switch (rule.Parameter)
                {
                    case 0: _tempStorage[3] += rule.Cylinder; break;
                    case 1: _tempStorage[3] -= rule.Cylinder; break;
                    case 2: _tempStorage[3] *= rule.Cylinder; break;
                    case 3: if (_tempStorage[3] != 0) _tempStorage[3] = Math.Truncate(_tempStorage[3] / rule.Cylinder); break;
                    case 4: if (rule.Cylinder != 0) _tempStorage[3] = Modulo(_tempStorage[3], rule.Cylinder); break;
                }

                rule = _rules[0][_colorNums[i] + 10];

                switch (rule.Parameter)
                {
                    case 0: _tempStorage[4] += rule.Cylinder; break;
                    case 1: _tempStorage[4] -= rule.Cylinder; break;
                    case 2: _tempStorage[4] *= rule.Cylinder; break;
                    case 3: if (_tempStorage[4] != 0) _tempStorage[4] = Math.Truncate(_tempStorage[4] / rule.Cylinder); break;
                    case 4: if (rule.Cylinder != 0) _tempStorage[4] = Modulo(_tempStorage[4], rule.Cylinder); break;
                }
                
                Debug.LogFormat("[Forget The Colors #{0}]: Stage {1}: Applying the {2}-colored cylinder on the first table, the nixies are now {3} and {4}.", _moduleId, stage, _colors[_colorNums[i]], _tempStorage[3], _tempStorage[4]);
            }
        }
        //modulo
        _tempStorage[3] = Modulo(_tempStorage[3], 10);
        _tempStorage[4] = Modulo(_tempStorage[4], 10);
        Debug.LogFormat("[Forget The Colors #{0}]: Stage {1}: After modulo of both nixies by 10, their values are now {2} and {3}.", _moduleId, stage, _tempStorage[3], _tempStorage[4]);

        Debug.LogFormat("[Forget The Colors #{0}]: <-------=-------> STAGE {1} (CALCULATED GEAR NUMBER ~ SECOND TABLE) <-------=------->", _moduleId, stage);

        //new gear = calculated nixies + gear
        Debug.LogFormat("[Forget The Colors #{0}]: Stage {1}: Combine the both nixies ({2}&{3}) as well as the gear number {4}. The sum of that whole number is {5}.", _moduleId, stage, _tempStorage[3], _tempStorage[4], _gear, _tempStorage[5]);
        _tempStorage[5] = _tempStorage[3] + _tempStorage[4] + _gear;
        Debug.LogFormat("[Forget The Colors #{0}]: Stage {1}: After modulo of the number above {2} with 10, its value is {3}.", _moduleId, stage, _tempStorage[5], Modulo(_tempStorage[5], 10));
        _tempStorage[5] = Modulo(_tempStorage[5], 10);

        //move the index up and down according to calculated nixies
        _index = _colorNums[3] - _tempStorage[3] + _tempStorage[4];
        Debug.LogFormat("[Forget The Colors #{0}]: Stage {1}: Starting on the color of the gear ({2}), move up the amount of squares equal to the left nixie tube ({3}) which lands on {4}, then move down the amount of squares equal to the right nixie tube ({5}) which lands us on {6}.", _moduleId, stage, _colors[_colorNums[3]], _tempStorage[3], _colors[(int)Modulo(_colorNums[3] - _tempStorage[3], 10)], _tempStorage[4], _colors[(int)Modulo(_colorNums[3] - _tempStorage[3] + _tempStorage[4], 10)]);

        //modulo
        _index = Modulo(_index, 10);
        double temp = _tempStorage[5];

        if (_rules == null)
        {
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
        }
        else
        {
            string[] ports = new string[Bomb.GetPorts().Count()];
            for (ushort i = 0; i < Bomb.GetPorts().Count(); i++)
                ports[i] = Bomb.GetPorts().ElementAt(i);
                
            int ignoredCount = 0;
            foreach (string module in Bomb.GetModuleNames())
                if (_ignore.Contains(module))
                    ignoredCount++;

            Rule rule = _rules[0][(int)_index];
            int[] edgework = new int[21] { Bomb.GetBatteryCount(), Bomb.GetBatteryCount(Battery.AA) + Bomb.GetBatteryCount(Battery.AAx3) + Bomb.GetBatteryCount(Battery.AAx4), Bomb.GetBatteryCount(Battery.D), Bomb.GetBatteryHolderCount(), Bomb.GetIndicators().Count(), Bomb.GetOnIndicators().Count(), Bomb.GetOffIndicators().Count(), Bomb.GetPortPlateCount(), Bomb.GetPorts().Distinct().Count(), Bomb.GetPorts().Count() - Bomb.GetPorts().Distinct().Count(), Bomb.GetPortCount(), Bomb.GetSerialNumberNumbers().First(), Bomb.GetSerialNumberNumbers().Last(), Bomb.GetSerialNumberNumbers().Count(), Bomb.GetSerialNumberLetters().Count(), Bomb.GetSolvedModuleNames().Count(), maxStage, Bomb.GetModuleNames().Count(), Bomb.GetSolvableModuleNames().Count() - Bomb.GetSolvedModuleNames().Count(), ignoredCount, _mainDisplays[1]};

            switch (rule.Parameter)
            {
                case 0: _tempStorage[5] += edgework[rule.Edgework]; break;
                case 1: _tempStorage[5] -= edgework[rule.Edgework]; break;
                case 2: _tempStorage[5] *= edgework[rule.Edgework]; break;
                case 3: if (edgework[rule.Edgework] != 0) _tempStorage[5] = Math.Truncate(_tempStorage[4] / edgework[rule.Edgework]); break;
                case 4: if (edgework[rule.Edgework] != 0) _tempStorage[5] = Modulo(_tempStorage[4], edgework[rule.Edgework]); break;
            }
        }

        ruleColor.Add(_colors[(int)_index]);
        Debug.LogFormat("[Forget The Colors #{0}]: Stage {1}: Apply the color rule {2} to the sum {3} which was calculated from the first nixie ({4}) + the second nixie ({5}) + the gear number ({6}), giving us {7}.", _moduleId, stage, _colors[(int)_index], temp, _tempStorage[3], _tempStorage[4], _gear, _tempStorage[5]);

        //modulo
        Debug.LogFormat("[Forget The Colors #{0}]: Stage {1}: After modulo of the sum {2}, its value is {3}. This is the number we need to construct a 3-digit number.", _moduleId, stage, _tempStorage[5], Modulo(_tempStorage[5], 10));
        _tempStorage[5] = Modulo(_tempStorage[5], 10);

        Debug.LogFormat("[Forget The Colors #{0}]: <-------=-------> STAGE {1} (CALCULATED STAGE NUMBER ~ SINE/COSINE) <-------=------->", _moduleId, stage);

        //get the sine degrees
        Debug.LogFormat("[Forget The Colors #{0}]: Stage {1}: The nixies are {2} and {3}, and the number obtained before is {4}, combining all of them gives us {5}.", _moduleId, stage, _tempStorage[3], _tempStorage[4], _tempStorage[5], string.Concat(_tempStorage[3], _tempStorage[4], _tempStorage[5]));
        _tempStorage[2] = Math.Truncate(Math.Sin(int.Parse(string.Concat(_tempStorage[3], _tempStorage[4], _tempStorage[5])) * Mathf.Deg2Rad) * Math.Pow(10, 5));

        //floating point rounding fix
        if (Modulo(Math.Abs(_tempStorage[2]), 1000) == 999)
            _tempStorage[2] = Modulo(_tempStorage[2] + 1, (int)Math.Pow(10, 5));
        Debug.LogFormat("[Forget The Colors #{0}]: Stage {1}: The sine number is sin({2}), which gets us {3} after flooring all decimals.", _moduleId, stage, string.Concat(_tempStorage[3], _tempStorage[4], _tempStorage[5]), _tempStorage[2]);

        //get stage number
        _tempStorage[0] = Math.Floor(Math.Abs(Math.Cos(_mainDisplays[0] * Mathf.Deg2Rad) * Math.Pow(10, 5)));

        //floating point rounding fix
        if (Modulo(Math.Abs(_tempStorage[0]), 1000) == 999)
            _tempStorage[0] = Modulo(_tempStorage[0] + 1, (int)Math.Pow(10, 5));

        Debug.LogFormat("[Forget The Colors #{0}]: Stage {1}: Taking the stage display, get the absolute of the first five decimals of cos({2}), which is {3}.", _moduleId, stage, _mainDisplays[0], _tempStorage[0]);

        //get final value for the stage
        _tempStorage[1] = _tempStorage[0] + _tempStorage[2];
        _storedValues[stage] = (int)_tempStorage[1];
        Debug.LogFormat("[Forget The Colors #{0}]: Stage {1}: The final value for this stage is the sum of the cosine number {2} and the sine number {3}, which gives the final value of {4}. This number will be important later.", _moduleId, stage, _tempStorage[0], _tempStorage[2], _tempStorage[1]);

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

        //colorblind
        if (Regex.IsMatch(command, @"^\s*colorblind\s*$", RegexOptions.IgnoreCase))
        {
            yield return null;
            _colorblind = !_colorblind;
            Render();
        }

        //submit command
        else if (Regex.IsMatch(buttonPressed[0], @"^\s*submit\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
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
                    for (byte i = 0; i < Buttons.Length - 1; i++)
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
        Debug.LogFormat("[Forget The Colors #{0}]: Admin has initiated an auto-solve. Thank you for attempting FTC. You gave up on stage {1}.", _moduleId, stage);

        while (!_inputMode)
            yield return true;

        yield return new WaitForSeconds(1f);

        for (byte i = 0; i < 2; i++)
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

sealed class Rule
{
    public byte Cylinder, Edgework, Parameter;
}