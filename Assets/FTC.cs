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
    //change this number before releasing a build!
    private static readonly string _version = "v1.0.4";

    //import assets
    public KMAudio Audio;
    public KMBombModule Module;
    public KMBossModule Boss;
    public KMBombInfo Bomb;
    public KMColorblindMode Colorblind;
    public KMRuleSeedable Rule;
    public KMSelectable[] Selectables;
    public Renderer[] Colors;
    public Transform Gear, CylinderKey;
    public Transform[] Disks;
    public TextMesh GearNumber, NumberBackground;
    public TextMesh[] Displays, NixieValues;
    public Texture[] Textures;
    
    //large souvenir dump
    bool solved;
    int stage, maxStage = 10;
    List<byte> gear = new List<byte>(0);
    List<short> largeDisplay = new List<short>(0);
    List<int> sineNumber = new List<int>(0);
    List<string> gearColor = new List<string>(0), ruleColor = new List<string>(0);

    //variables for solving
    private bool _canInteract, _colorblind, _allowCycleStage, _isRotating, _isAnimating;
    private sbyte _solution = -1;
    private byte _debugSelect;
    private int _gearAngle, _currentAngle, _moduleId;
    private int[] _colorValues = new int[4];
    private float _easeSolve, _gearEasing, _sum;
    private List<byte> _cylinder = new List<byte>(0), _nixies = new List<byte>(0);
    private List<int> _calculatedValues = new List<int>(0);

    //global attributes
    private static int _moduleIdCounter = 1;
    private static readonly IEnumerable<string> _solvable;
    private static Rule[][] _rules;
    private static string[] _ignore = { "Forget The Colors", "14", "Bamboozling Time Keeper", "Brainf---", "Forget Enigma", "Forget Everything", "Forget It Not", "Forget Me Not", "Forget Me Later", "Forget Perspective", "Forget Them All", "Forget This", "Forget Us Not", "Organization", "Purgatory", "Simon Forgets", "Simon's Stages", "Souvenir", "Tallordered Keys", "The Time Keeper", "The Troll", "The Very Annoying Button", "Timing Is Everything", "Turn The Key", "Ultimate Custom Night", "Übermodule" };

    //logging
    private static readonly string[] _colors = { "Red", "Orange", "Yellow", "Green", "Cyan", "Blue", "Purple", "Pink", "Maroon", "White", "Gray" };
    private static readonly string[] _failPhrases = { "You did a goodn't", "Congratulations! You got a strike", "You have just won a free gift card containing 1 strike and no solve! In other words", "This is so sad", "This must be really embarrasing for you", "I just came back, where we again? Oh yeah", "Unsuprisingly, your 1/91 chance went unsuccessful", "Did Emik break the module or are you just bad?", "Did Cooldoom break the module or are you just bad?", "This looks like a WHITE ABORT to me", "Correct... your mistakes in the future", "?!", "‽", "The phrase \"It's just a module\" is such a weak mindset, you are okay with what happened, striking, imperfection of a solve", "Good for you", "Have fun doing the math again", "Was that MAROON or RED?", "Are you sure the experts wrote it down correctly?", "Are you sure the defuser said it correctly?", "The key spun backwards", "THE ANSWER IS IN THE WRONG POSITION", "key.wav", "Module.HandleStrike()", "Is your calculator broken?", "Is your KTANE broken?", "A wide-screen monitor would really help here", "VR would make this easier", "A mechanical keyboard would make this easier", "A \"gaming mouse\" would make this easier", "E", "bruh moment", "Failed executing external process for 'Bake Runtime' job", "Did Discord cut vital communication off?", "You failed the vibe check", "Looks like you failed your exam", "Could not find USER_ANSWER in ACTUAL_ANSWER", "nah", "noppen", "yesn't", "This is the moment where you quit out the bomb", "You just lost the game", "Noooo, why'd you do that?", "*pufferfish noises*", "I was thinking about being generous this round, it didn't change my mind though", "Have you tried turning this module on and off?", "It's been so long, since I last have seen an answer, lost to this monster", "Oof", "Yikes", "Good luck figuring out why you're wrong", "Oog", "Nice one buckaroo", ":̶.̶|̶:̶;̶  <--- Is this loss?", "Oh, you got it wrong? Report it as a bug because it's definitely not your fault", "I'm not rated \"Very Hard\" for no reason after all", "Forget The Colors be like: cringe", "The manual said I is Pink, are you colorblind?", "Not cool, meet your doom", "What were you thinking!?", "Emmm, ik you messed up somewhere", "You should double check that part where you messed up", "Looks like the expert chose betray", "At least you've solved the other modules", "Did you even read the manual?", "The module's broken? No I'm not! What's 9+10? 18.9992", "When I shred, I shred using the entire bomb. But since you SUCK, you will only need this module, and zie key button", "ALT+F4", "Did you seriously mistake me for Forget Everything?", "I was kidding when I told you to Forget The Colors, I guess sarcasm didn't come through that time...", "The Defuser expired", "The Expert expired", "You just got bamboozl- ah, wrong module", "Module rain. Some stay solved and others feel the pain. Module rain. 3-digit displays will die before the sin()", "DEpvQ0klM93dC8GMWAo5TaYGeWCZfT8Vq1qNY6o     + // /", "mood", "This message should not appear. I'll be disappointed at the Defuser if it does", "Did you forget about the 0 solvable module unicorn?" };
    private static readonly string[] _winPhrases = { "Hey, that's pretty good", "*intense cat noises*", "While you're at it, be sure to like, comment, favorite and subscribe", "This is oddly calming", "GG m8", "I just came back, where we again? Oh yeah", "Suprisingly, your 1/91 chance went successful", "Did Emik fix the module or are you just that good?", "Did Cooldoom fix the module or are you just that good?", "This looks like a NUT BUTTON to me", "Opposite of incorrect", "Damn, I should ban you from solving me", "You haven't forgotten the colors?", "Do you still think it's Very Hard?", "I think I'm supposed to Module.HandlePass()", "I really hope you didn't look at the logs", "I really hope you didn't use an auto-solver", "I should have just used Azure instead of White", "How many shrimps do I have to eat, before it makes my gears turn pink", "The key spun forwards", "THE ANSWER IS IN THE RIGHT POSITION", "keyCorrect.wav", "Module.HandlePass()", "Did you use a calculator?", "Did you enjoy it?", "Please rate us 5 stars in the ModuleStore at your KTaNEPhone", "Alexa, play the victory tune", "VICTORY", "Maybe I should've called myself \"Write Down Colors\"", "E", "bruh moment", "*happy music*", ":) good", "You passed the vibe check", "Looks like you passed your exam", "Successfully found USER_ANSWER in ACTUAL_ANSWER", "yes", "yesper", "non't", "This is the moment where you say \"LET'S GO!!\"", "You just won the game", "*key turned*", "opposite of bruh moment", "I was thinking about being generous this round, but you were correct anyway", ":joy: 99% IMPOSSIBLE :joy:", "Forget The Colors, is this where you want to be, I just don't get it, why do you want to stay?", "Mood", "!!", "Now go brag to your friends", "PogChamp", "Poggers", "You passed with flying colors", "Oh, you got it right? Report it as a bug because I'm too easy, y'know?", "I agree, I'm just as easy as The Simpleton right beside me!", "Forget The Colors says: uncringe", "That seemed to easy for you, was colorblind enabled?", "And now, Souvenir", "I hope you wrote down the edgework-based rule for the first stage", "Emmm, ik that's correct", "Can you really say you've disabled Colorblind mode when you have a transparent bomb casing?", "Looks like the expert chose ally", "At least it's solved", "Clip it! Somebody highlight that or somebody clip that!", "Was I designed to be solved? Can't remember", "SPIINNN", "*roll credits*", "Hey! Your buttons are sorted- wait wrong module", "Do you have 200IQ or something?", "How would you have felt if I decided to strike?", "The module expired", "Forget The Colors expired", "The bomb expired", "A winner is you!", "All your module are belong to us", "BOB STOLE MY KEY", "Defuser achieved rank #1 on Being Cool:tm:" };

    private void Awake()
    {
        //boss module handler
        if (Boss.GetIgnoredModules(Module, _ignore) != null)
            _ignore = Boss.GetIgnoredModules(Module, _ignore);

        _moduleId = _moduleIdCounter++;

        //establish buttons
        for (byte i = 0; i < Selectables.Length; i++)
        {
            byte j = i;
            Selectables[j].OnInteract += delegate
            {
                HandlePress(j);
                return false;
            };
        }
    }

    private void Start()
    {
        //enables colorblind mode if needed
        _colorblind = Colorblind.ColorblindModeActive;

        //gets seed
        MonoRandom rnd = Rule.GetRNG();
        Debug.LogFormat("[Forget The Colors #{0}]: Using version {1} with rule seed: {2}.", _moduleId, _version, rnd.Seed);

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
                _rules[0][i] = new Rule { Cylinder = (byte)rnd.Next(10), Operator = (byte)rnd.Next(5) };

            //applies rule seeding for edgework
            for (byte i = 0; i < 10; i++)
                _rules[1][i] = new Rule { Edgework = (byte)rnd.Next(21), Operator = (byte)rnd.Next(5) };
        }

        //if on unity, max stage should equal the initial value assigned, otherwise set it to the proper value
        if (!Application.isEditor)
            maxStage = Bomb.GetSolvableModuleNames().Where(a => !_ignore.Contains(a)).Count();

        //proper grammar!!
        if (maxStage != 1)
            Debug.LogFormat("[Forget The Colors #{0}]: Welcome to FTC - This bomb will have {1} stages.", _moduleId, maxStage);

        else
            Debug.LogFormat("[Forget The Colors #{0}]: Welcome to FTC - This bomb will have a single stage.", _moduleId);

        //notice in case if i accidentally make _winPhrases and _failPhrases unbalanced
        if (_winPhrases.Length != _failPhrases.Length)
            Debug.LogFormat("[Forget The Colors #{0}]: If you see this message, it means that there are an unbalanced amount of flavor text when you strike or solve! Length of strike: {1}. Length of solve: {2}", _moduleId, _failPhrases.Length, _winPhrases.Length);

        //initialization of previous stage variables
        for (ushort i = 0; i < maxStage; i++)
        {
            for (byte j = 0; j < 4; j++)
                _cylinder.Add(0);

            _nixies.Add(0);
            _nixies.Add(0);
            gear.Add(0);
            largeDisplay.Add(0);
            _calculatedValues.Add(0);
            sineNumber.Add(0);
            gearColor.Add("Red");
            ruleColor.Add("Red");
        }

        //begin module
        NixieValues[0].text = "0";
        NixieValues[1].text = "0";
        StartCoroutine(Generate(0));
        Audio.PlaySoundAtTransform("start", Module.transform);

        //show that it's debug mode
        if (Application.isEditor)
        {
            Displays[1].fontSize = 35;
            NumberBackground.text = "";
        }
    }

    private void FixedUpdate()
    {
        //spin to next destination, every solve will give a new angle clockwise to itself
        if (Gear.localRotation.y != _gearAngle + _currentAngle && !_allowCycleStage)
        {
            _gearEasing += 0.025f;
            _isRotating = _gearEasing <= 1;

            //when finished generating stages, spin counter-clockwise to the nearest neutral position
            if (_canInteract)
            {
                Gear.localRotation = Quaternion.Euler(0, _currentAngle % 360 * Math.Abs(CubicOut(_gearEasing) - 1), 0);

                if (_gearEasing > 1)
                    Gear.localRotation = Quaternion.Euler(0, 0, 0);
            }

            //when generating stages, spin clockwise randomly
            else
            {
                Gear.localRotation = Quaternion.Euler(0, CubicOut(_gearEasing) * _gearAngle + _currentAngle, 0);

                if (_gearEasing > 1)
                    Gear.localRotation = Quaternion.Euler(0, _gearAngle + _currentAngle, 0);
            }
        }

        //when solved, do the cool solve animation
        if (solved)
        {
            //expansion
            if (_easeSolve <= 1)
            {
                _easeSolve += 0.02f;

                Selectables[2].transform.localRotation = Quaternion.Euler(0, BackOut(_easeSolve) * 420, 0);
                CylinderKey.localScale = new Vector3(ElasticOut(_easeSolve) * 0.5f, 1, ElasticOut(_easeSolve) * 0.5f);
            }

            //retraction
            else if (_easeSolve <= 2)
            {
                _easeSolve += 0.04f;

                Selectables[2].transform.localPosition = new Vector3(0, (BackIn(_easeSolve - 1) * -3) - 0.91f, 0);
                CylinderKey.localScale = new Vector3((1 - ElasticIn(_easeSolve - 1)) / 2, 1, (1 - ElasticIn(_easeSolve - 1)) / 2);
            }

            //last frame
            else
                CylinderKey.localPosition = new Vector3(0, -0.2f, 0);
        }

        //failed key spin
        else if (_isAnimating)
        {
            _easeSolve += 0.04f;
            Selectables[2].transform.localRotation = Quaternion.Euler(0, (ElasticOut(_easeSolve) - _easeSolve) * 69, 0);

            if (_easeSolve >= 1)
            {
                _isAnimating = false;
                _easeSolve = 0;
            }
        }

        //if there are more stages left, generate new stage
        else if (stage < Bomb.GetSolvedModuleNames().Where(a => !_ignore.Contains(a)).Count() && !solved && !_allowCycleStage && !_isRotating)
        {
            //generate a stage
            StartCoroutine(Generate(++stage));

            //allows rotation
            _currentAngle += _gearAngle;
            _gearAngle = 2;
            _gearEasing = 0;
        }
    }

    private IEnumerator Generate(int currentStage)
    {
        //if solved, don't generate
        if (solved)
            StopAllCoroutines();

        //plays sound
        if (currentStage != 0)
            Audio.PlaySoundAtTransform("nextStage", Module.transform);

        //if this is the submission/final stage
        if (currentStage == maxStage || _solution != -1)
        {
            //runs 25 times
            for (byte i = 0; i < 25; i++)
            {
                Displays[0].text = Rnd.Range(0, 991).ToString();
                Displays[1].text = Rnd.Range(0, 100).ToString();
                GearNumber.text = Rnd.Range(0, 10).ToString();

                for (byte j = 0; j < NixieValues.Length; j++)
                    NixieValues[j].text = Rnd.Range(0, 10).ToString();

                for (byte j = 0; j < _colorValues.Length; j++)
                    _colorValues[j] = Rnd.Range(0, 10);

                Render();

                yield return new WaitForSeconds(0.07f);
            }

            //reset visuals
            NixieValues[0].text = "0";
            NixieValues[1].text = "0";

            for (byte i = 0; i < _colorValues.Length; i++)
                _colorValues[i] = 10;

            Render();
            CalculateAnswer();
            StopAllCoroutines();
        }

        //if it's supposed to be randomising
        if (!solved && currentStage < maxStage && _solution == -1)
        {
            //stage 0: runs 25 times, stage 1+: runs 5 times
            for (byte i = 0; i < 5 + ((Mathf.Clamp(currentStage, 0, 1) - 1) * -20); i++)
            {
                Displays[0].text = Rnd.Range(0, 991).ToString();
                Displays[1].text = Rnd.Range(0, 100).ToString();
                GearNumber.text = Rnd.Range(0, 10).ToString();

                for (byte j = 0; j < NixieValues.Length; j++)
                    NixieValues[j].text = Rnd.Range(0, 10).ToString();

                for (byte j = 0; j < _colorValues.Length; j++)
                    _colorValues[j] = Rnd.Range(0, 10);

                Render();

                yield return new WaitForSeconds(0.07f);
            }

            //set stage number to display
            Displays[1].text = currentStage.ToString();

            //souvenir
            gear[currentStage] = byte.Parse(GearNumber.text[Convert.ToByte(_colorblind)].ToString());
            gearColor[currentStage] = _colors[_colorValues[3]];
            largeDisplay[currentStage] = short.Parse(Displays[0].text);

            //in case of strikes
            for (byte i = 0; i < 2; i++)
                _nixies[(currentStage * 2) + i] = byte.Parse(NixieValues[i].text);
            for (byte i = 0; i < 4; i++)
                _cylinder[(currentStage * 4) + i] = (byte)_colorValues[i];

            Render();

            //if it's not last stage
            if (currentStage < maxStage && _solution == -1)
                Calculate(currentStage);
        }
    }

    private void HandlePress(byte btn)
    {
        //adds interaction punch
        if (btn != 2)
        {
            Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, Selectables[btn].transform);
            Selectables[btn].AddInteractionPunch();
        }

        //if solved, buttons and key should do nothing
        if (solved || (!_canInteract && !Application.isEditor))
            return;

        if (_allowCycleStage)
        {
            switch (btn)
            {
                case 0: stage = (int)Modulo(stage - 1, maxStage); PreviousRender(); break;
                case 1: stage = (int)Modulo(stage + 1, maxStage); PreviousRender(); break;
                case 2: _allowCycleStage = false; stage = maxStage; Render(); break;
            }
            return;
        }

        //NOT the key
        else if (btn != 2)
        {
            //increments respective nixie by 1
            if (!Application.isEditor || _canInteract)
                NixieValues[btn].text = Modulo(int.Parse(NixieValues[btn].text) + 1, 10).ToString();

            //complete debugging
            else
            {
                //right nixie changes value selected by one
                if (btn == 1)
                    switch (_debugSelect)
                    {
                        case 0: Displays[0].text = Modulo(int.Parse(Displays[0].text) + 100, 1000).ToString(); break;
                        case 1: Displays[0].text = Modulo(int.Parse(Displays[0].text) + 10, 1000).ToString(); break;
                        case 2: Displays[0].text = Modulo(int.Parse(Displays[0].text) + 1, 1000).ToString(); break;
                        case 3: _colorValues[0] = (int)Modulo(_colorValues[0] + 1, 10); break;
                        case 4: _colorValues[1] = (int)Modulo(_colorValues[1] + 1, 10); break;
                        case 5: _colorValues[2] = (int)Modulo(_colorValues[2] + 1, 10); break;
                        case 6: GearNumber.text = Modulo(int.Parse(GearNumber.text[Convert.ToByte(_colorblind)].ToString()) + 1, 10).ToString(); break;
                        case 7: _colorValues[3] = (int)Modulo(_colorValues[3] + 1, 10); break;
                        case 8: NixieValues[0].text = Modulo(int.Parse(NixieValues[0].text) + 1, 10).ToString(); break;
                        case 9: NixieValues[1].text = Modulo(int.Parse(NixieValues[1].text) + 1, 10).ToString(); break;
                        case 10: stage++; break;

                        case 11:
                            //souvenir
                            gear[stage] = byte.Parse(GearNumber.text.Last().ToString());
                            gearColor[stage] = _colors[_colorValues[3]];
                            largeDisplay[stage] = short.Parse(Displays[0].text);

                            //in case of strikes
                            for (byte i = 0; i < 2; i++)
                                _nixies[(stage * 2) + i] = byte.Parse(NixieValues[i].text);
                            for (byte i = 0; i < 4; i++)
                                _cylinder[(stage * 4) + i] = (byte)_colorValues[i];

                            Calculate(stage);
                            CalculateAnswer();
                            break;
                    }

                //left nixie changes which value is selected
                else
                    _debugSelect = (byte)Modulo(_debugSelect + 1, 12);

                switch (_debugSelect)
                {
                    case 0: Displays[1].text = "largeH"; break;
                    case 1: Displays[1].text = "largeD"; break;
                    case 2: Displays[1].text = "large"; break;
                    case 3: Displays[1].text = "cylA"; break;
                    case 4: Displays[1].text = "cylB"; break;
                    case 5: Displays[1].text = "cylC"; break;
                    case 6: Displays[1].text = "gear#"; break;
                    case 7: Displays[1].text = "gearC"; break;
                    case 8: Displays[1].text = "nixieL"; break;
                    case 9: Displays[1].text = "nixieR"; break;
                    case 10: Displays[1].text = "stage" + stage; break;
                    case 11: Displays[1].text = "quit"; break;
                }

                Render();
            }
        }

        //key
        else
        {
            //debugging
            if (Application.isEditor && !_canInteract)
            {
                //souvenir
                gear[stage] = byte.Parse(GearNumber.text[Convert.ToByte(_colorblind)].ToString());
                gearColor[stage] = _colors[_colorValues[3]];
                largeDisplay[stage] = short.Parse(Displays[0].text);

                //in case of strikes
                for (byte i = 0; i < 2; i++)
                    _nixies[(stage * 2) + i] = byte.Parse(NixieValues[i].text);
                for (byte i = 0; i < 4; i++)
                    _cylinder[(stage * 4) + i] = (byte)_colorValues[i];

                Calculate(stage);

                if (!_isRotating)
                {
                    _currentAngle += _gearAngle;
                    _gearAngle = 2;
                    _gearEasing = 0;
                }
            }

            //if both correct
            else if (int.Parse(string.Concat(NixieValues[0].text, NixieValues[1].text)) == _solution)
            {
                Audio.PlaySoundAtTransform("keySuccess", Selectables[2].transform);
                Audio.PlaySoundAtTransform("solved", Module.transform);
                solved = true;
                Module.HandlePass();

                Debug.LogFormat("[Forget The Colors #{0}]: {1}; module solved!", _moduleId, _winPhrases[Rnd.Range(0, _winPhrases.Length)]);
            }

            //if either incorrect
            else
            {
                Audio.PlaySoundAtTransform("key", Selectables[2].transform);
                _allowCycleStage = true;
                _isAnimating = true;
                Module.HandleStrike();
                Debug.LogFormat("[Forget The Colors #{0}]: {1}; you submitted {2} when I expected {3}.", _moduleId, _failPhrases[Rnd.Range(0, _failPhrases.Length)], NixieValues[0].text + NixieValues[1].text, _solution);
            }
        }

        Render();
    }

    private void Render()
    {
        //set everything gray
        if (_canInteract)
        {
            Displays[0].text = "";
            Displays[1].text = "";
            GearNumber.text = "0";

            for (byte i = 0; i < Colors.Length; i++)
                Colors[i].material.mainTexture = Textures[10];

            Colors[3].material.SetTextureScale("_MainTex", new Vector2(0, 0));

            //reinstalls cylinders regardless of colorblind
            for (byte i = 0; i < Disks.Length; i++)
                Disks[i].localRotation = new Quaternion(0, -90, 0, 0);

            //set gear
            GearNumber.characterSize = 0.1f;
            GearNumber.text = GearNumber.text.Last().ToString();
        }

        else
        {
            //sets leading 0's
            for (byte i = 0; i < Displays.Length; i++)
                while (Displays[i].text.Length < 3 - i)
                    Displays[i].text = "0" + Displays[i].text;

            //set colors
            for (byte i = 0; i < Colors.Length; i++)
            {
                Colors[i].material.mainTexture = Textures[_colorValues[i]];
                Colors[i].material.SetTextureOffset("_MainTex", new Vector2(0.5f * Convert.ToByte(_colorblind) * Convert.ToByte(maxStage != stage), -0.04f));
            }

            Colors[3].material.SetTextureScale("_MainTex", new Vector2(0, 0));

            //deletes cylinders if needed
            for (byte i = 0; i < Disks.Length; i++)
                Disks[i].localRotation = new Quaternion(90 * Convert.ToByte(_colorblind), -90, 0, 0);

            //set gear size
            GearNumber.characterSize = 0.1f - (Convert.ToByte(_colorblind) * 0.04f);

            //render letter for colorblind
            if (_colorblind)
            {
                //checks for pink, since pink and purple start with the same letter
                if (_colorValues[3] != 7)
                    GearNumber.text = _colors[_colorValues[3]].First() + GearNumber.text.Last().ToString();
                else
                    GearNumber.text = 'I' + GearNumber.text.Last().ToString();
            }
        }
    }

    private void PreviousRender()
    {
        //render initial displays
        Displays[0].text = largeDisplay[stage].ToString();
        Displays[1].text = Modulo(stage, 100).ToString();

        //if the large display lacks 3 characters, add 0's
        for (byte i = 0; i < Displays.Length; i++)
            while (Displays[i].text.Length < 3 - i)
                Displays[i].text = "0" + Displays[i].text;

        //set nixies
        for (byte i = 0; i < NixieValues.Length; i++)
            NixieValues[i].text = _nixies[i + (stage * 2)].ToString();

        //set gear number and size
        GearNumber.text = gear[stage].ToString();
        GearNumber.characterSize = 0.1f - (Convert.ToByte(_colorblind) * Convert.ToByte(maxStage != stage) * 0.04f);

        //render letter for colorblind
        if (_colorblind)
        {
            //checks for pink, since pink and purple start with the same letter
            if (gearColor[stage] != "Pink")
                GearNumber.text = gearColor[stage].First() + GearNumber.text.Last().ToString();
            else
                GearNumber.text = 'I' + GearNumber.text.Last().ToString();
        }

        //set colors
        for (byte i = 0; i < Colors.Length; i++)
        {
            Colors[i].material.mainTexture = Textures[_cylinder[i + (stage * 4)]];
            Colors[i].material.SetTextureOffset("_MainTex", new Vector2(0.5f * Convert.ToByte(_colorblind) * Convert.ToByte(maxStage != stage), -0.04f));
        }
        Colors[3].material.SetTextureScale("_MainTex", new Vector2(0, 0));

        //deletes cylinders if needed
        for (byte i = 0; i < Disks.Length; i++)
            Disks[i].localRotation = new Quaternion(90 * Convert.ToByte(_colorblind) * Convert.ToByte(maxStage != stage), -90, 0, 0);
    }

    private void CalculateAnswer()
    {
        Debug.LogFormat("[Forget The Colors #{0}]: FINAL STAGE", _moduleId);
        Debug.LogFormat("[Forget The Colors #{0}]: <-------=-------> FINALE ~ ARCCOSINE <-------=------->", _moduleId);

        //prevents out of array exceptions in editor
        if (Application.isEditor)
            maxStage = _calculatedValues.Count;

        //adds all of the values
        for (byte i = 0; i < _calculatedValues.Count; i++)
        {
            _sum += _calculatedValues[i];
            Debug.LogFormat("[Forget The Colors #{0}]: Adding stage {1}'s {2}, the total is now {3}.", _moduleId, i, _calculatedValues[i], _sum);
        }

        //allow inputs in the module
        NixieValues[0].text = "0";
        NixieValues[1].text = "0";
        _canInteract = true;
        Render();

        //turns into decimal number
        Debug.LogFormat("[Forget The Colors #{0}]: First five digits of cos-1({1}) is {2}.", _moduleId, Modulo(Mathf.Abs(_sum) / 100000, 1), Math.Truncate(Mathf.Acos((float)Modulo(Mathf.Abs(_sum) / 100000, 1)) * Mathf.Rad2Deg));
        _solution = (sbyte)(Mathf.Acos((float)Modulo(Mathf.Abs(_sum) / 100000, 1)) * Mathf.Rad2Deg);

        Debug.LogFormat("[Forget The Colors #{0}]: The expected answer is {1}.", _moduleId, _solution);
        Debug.LogFormat("[Forget The Colors #{0}]: USER INPUT", _moduleId);
        Debug.LogFormat("[Forget The Colors #{0}]: <-------=-------> LET'S SEE HOW THE USER DOES <-------=------->", _moduleId);
    }

    private void Calculate(int currentStage)
    {
        Debug.LogFormat("[Forget The Colors #{0}]: STAGE {1}", _moduleId, currentStage);
        Debug.LogFormat("[Forget The Colors #{0}]: <-------=-------> MODULE APPEARANCE <-------=------->", _moduleId);
        Debug.LogFormat("[Forget The Colors #{0}]: Large Display: {1}. Cylinders (left-to-right): {2}, {3}, and {4}. Nixies: {5} and {6}. Gear: {7} and {8}.", _moduleId, Displays[0].text, _colors[_colorValues[0]], _colors[_colorValues[1]], _colors[_colorValues[2]], NixieValues[0].text, NixieValues[1].text, GearNumber.text[Convert.ToByte(_colorblind)], _colors[_colorValues[3]]);
        Debug.LogFormat("[Forget The Colors #{0}]: <-------=-------> NIXIES ~ FIRST TABLE <-------=------->", _moduleId);
        short nixie1 = sbyte.Parse(NixieValues[0].text), nixie2 = sbyte.Parse(NixieValues[1].text);

        if (_rules == null)
        {
            //this will run through the changes applied to both nixie tubes during step 1 of second page on manual
            for (byte i = 0; i < _colorValues.Length - 1; i++)
            {
                //each digit rule
                switch (_colorValues[i])
                {
                    case 0: nixie1 += 5; nixie2 -= 1; break;
                    case 1: nixie1 -= 1; nixie2 -= 6; break;
                    case 2: nixie1 += 3; break;
                    case 3: nixie1 += 7; nixie2 -= 4; break;
                    case 4: nixie1 -= 7; nixie2 -= 5; break;
                    case 5: nixie1 += 8; nixie2 += 9; break;
                    case 6: nixie1 += 5; nixie2 -= 9; break;
                    case 7: nixie1 -= 9; nixie2 += 4; break;
                    case 8: nixie2 += 7; break;
                    case 9: nixie1 -= 3; nixie2 += 5; break;
                }
                Debug.LogFormat("[Forget The Colors #{0}]: Applying the {1}-colored cylinder on the first table, the nixies are now {2} and {3}.", _moduleId, _colors[_colorValues[i]], nixie1, nixie2);
            }
        }

        else
        {
            for (byte i = 0; i < _colorValues.Length - 1; i++)
            {
                Rule rule = _rules[0][_colorValues[i]];

                switch (rule.Operator)
                {
                    case 0: nixie1 += (sbyte)rule.Cylinder; break;
                    case 1: nixie1 -= (sbyte)rule.Cylinder; break;
                    case 2: nixie1 *= (sbyte)rule.Cylinder; break;
                    case 3: if (nixie1 != 0) nixie1 = (sbyte)(nixie1 / rule.Cylinder); break;
                    case 4: if (rule.Cylinder != 0) nixie1 = (sbyte)Modulo(nixie1, rule.Cylinder); break;
                }

                rule = _rules[0][_colorValues[i] + 10];

                switch (rule.Operator)
                {
                    case 0: nixie2 += (sbyte)rule.Cylinder; break;
                    case 1: nixie2 -= (sbyte)rule.Cylinder; break;
                    case 2: nixie2 *= (sbyte)rule.Cylinder; break;
                    case 3: if (nixie2 != 0) nixie2 = (sbyte)(nixie2 / rule.Cylinder); break;
                    case 4: if (rule.Cylinder != 0) nixie2 = (sbyte)Modulo(nixie2, rule.Cylinder); break;
                }
                
                Debug.LogFormat("[Forget The Colors #{0}]: Applying the {1}-colored cylinder on the first table, the nixies are now {2} and {3}.", _moduleId, _colors[_colorValues[i]], nixie1, nixie2);
            }
        }

        //modulo
        nixie1 = (sbyte)Modulo(nixie1, 10);
        nixie2 = (sbyte)Modulo(nixie2, 10);
        Debug.LogFormat("[Forget The Colors #{0}]: Modulo 10, their values are now {1} and {2}.", _moduleId, nixie1, nixie2);

        Debug.LogFormat("[Forget The Colors #{0}]: <-------=-------> GEAR NUMBER ~ SECOND TABLE <-------=------->", _moduleId);

        //new gear = calculated nixies + gear
        int lsd = (byte)Modulo(nixie1 + nixie2 + int.Parse(GearNumber.text[Convert.ToByte(_colorblind)].ToString()), 10);
        Debug.LogFormat("[Forget The Colors #{0}]: Combine both nixies ({1}&{2}) and the gear number {3}. The sum is {4}. Modulo 10, its value is {5}.", _moduleId, nixie1, nixie2, GearNumber.text[Convert.ToByte(_colorblind)], nixie1 + nixie2 + int.Parse(GearNumber.text[Convert.ToByte(_colorblind)].ToString()), lsd);
        
        //move the index up and down according to calculated nixies
        Debug.LogFormat("[Forget The Colors #{0}]: Start on gear color ({1}), move up left nixie ({2}) which lands on {3}, then move down right nixie ({4}) which lands us on {5}.", _moduleId, _colors[_colorValues[3]], nixie1, _colors[(int)Modulo(_colorValues[3] - nixie1, 10)], nixie2, _colors[(int)Modulo(_colorValues[3] - nixie1 + nixie2, 10)]);

        if (_rules == null)
        {
            //this will run through the changes applied to the gear during step 2 of second page on manual
            switch ((int)Modulo(_colorValues[3] - nixie1 + nixie2, 10))
            {
                case 0: lsd += Bomb.GetBatteryCount(); break;
                case 1: lsd -= Bomb.GetPortCount(); break;
                case 2: lsd += Bomb.GetSerialNumberNumbers().Last(); break;
                case 3: lsd -= Bomb.GetSolvedModuleNames().Count(); break;
                case 4: lsd += Bomb.GetPortPlateCount(); break;
                case 5: lsd -= Bomb.GetModuleNames().Count(); break;
                case 6: lsd += Bomb.GetBatteryHolderCount(); break;
                case 7: lsd -= Bomb.GetOnIndicators().Count(); break;
                case 8: lsd += Bomb.GetIndicators().Count(); break;
                case 9: lsd -= Bomb.GetOffIndicators().Count(); break;
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

            Rule rule = _rules[0][(int)Modulo(_colorValues[3] - nixie1 + nixie2, 10)];

            int[] edgework;

            //the smaller display is used as a debug displayer in the editor, which cannot be parsed
            if (!Application.isEditor)
                edgework = new int[21] { Bomb.GetBatteryCount(), Bomb.GetBatteryCount(Battery.AA) + Bomb.GetBatteryCount(Battery.AAx3) + Bomb.GetBatteryCount(Battery.AAx4), Bomb.GetBatteryCount(Battery.D), Bomb.GetBatteryHolderCount(), Bomb.GetIndicators().Count(), Bomb.GetOnIndicators().Count(), Bomb.GetOffIndicators().Count(), Bomb.GetPortPlateCount(), Bomb.GetPorts().Distinct().Count(), Bomb.GetPorts().Count() - Bomb.GetPorts().Distinct().Count(), Bomb.GetPortCount(), Bomb.GetSerialNumberNumbers().First(), Bomb.GetSerialNumberNumbers().Last(), Bomb.GetSerialNumberNumbers().Count(), Bomb.GetSerialNumberLetters().Count(), Bomb.GetSolvedModuleNames().Count(), maxStage, Bomb.GetModuleNames().Count(), Bomb.GetSolvableModuleNames().Count() - Bomb.GetSolvedModuleNames().Count(), ignoredCount, int.Parse(Displays[1].text) };

            else
                edgework = new int[21] { Bomb.GetBatteryCount(), Bomb.GetBatteryCount(Battery.AA) + Bomb.GetBatteryCount(Battery.AAx3) + Bomb.GetBatteryCount(Battery.AAx4), Bomb.GetBatteryCount(Battery.D), Bomb.GetBatteryHolderCount(), Bomb.GetIndicators().Count(), Bomb.GetOnIndicators().Count(), Bomb.GetOffIndicators().Count(), Bomb.GetPortPlateCount(), Bomb.GetPorts().Distinct().Count(), Bomb.GetPorts().Count() - Bomb.GetPorts().Distinct().Count(), Bomb.GetPortCount(), Bomb.GetSerialNumberNumbers().First(), Bomb.GetSerialNumberNumbers().Last(), Bomb.GetSerialNumberNumbers().Count(), Bomb.GetSerialNumberLetters().Count(), Bomb.GetSolvedModuleNames().Count(), maxStage, Bomb.GetModuleNames().Count(), Bomb.GetSolvableModuleNames().Count() - Bomb.GetSolvedModuleNames().Count(), ignoredCount, 0 };

            switch (rule.Operator)
            {
                case 0: lsd += edgework[rule.Edgework]; break;
                case 1: lsd -= edgework[rule.Edgework]; break;
                case 2: lsd *= edgework[rule.Edgework]; break;
                case 3: if (edgework[rule.Edgework] != 0) lsd = nixie2 / edgework[rule.Edgework]; break;
                case 4: if (edgework[rule.Edgework] != 0) lsd = (int)Modulo(nixie2, edgework[rule.Edgework]); break;
            }
        }

        ruleColor[currentStage] = _colors[(int)Modulo(_colorValues[3] - nixie1 + nixie2, 10)];
        Debug.LogFormat("[Forget The Colors #{0}]: Apply the color rule {1} to the sum of the first nixie ({2}) + the second nixie ({3}) + the gear number ({4}). This gives us {5}. Modulo 10, its value is {6}.", _moduleId, _colors[(int)Modulo(_colorValues[3] - nixie1 + nixie2, 10)], nixie1, nixie2, GearNumber.text, nixie1 + nixie2 + int.Parse(GearNumber.text.Last().ToString()), Modulo(lsd, 10));

        //modulo
        lsd = (int)Modulo(lsd, 10);

        Debug.LogFormat("[Forget The Colors #{0}]: <-------=-------> STAGE VALUE ~ SINE/COSINE) <-------=------->", _moduleId);

        //get the sine degrees
        Debug.LogFormat("[Forget The Colors #{0}]: The nixies are {1} and {2}, and the number obtained before is {3}, combining all of them gives us {4}.", _moduleId, nixie1, nixie2, lsd, string.Concat(nixie1, nixie2, lsd));
        int sin = (int)(Math.Sin(int.Parse(string.Concat(nixie1, nixie2, lsd)) * Mathf.Deg2Rad) * 100000 % 100000);

        //floating point rounding fix, ensuring that it adds/subtracts 1 depending if it's a positive or negative number
        if (Modulo(Math.Abs(sin), 1000) == 999)
            if (sin > 0)
                sin = (sin + 1) % 100000;
            else
                sin = (sin - 1) % 100000;

        //get stage number
        int cos = (int)(Math.Abs(Math.Cos(int.Parse(Displays[0].text) * Mathf.Deg2Rad) * 100000) % 100000);

        //floating point rounding fix
        if (Modulo(cos, 1000) == 999)
            cos = (int)Modulo(cos + 1, 100000);

        Debug.LogFormat("[Forget The Colors #{0}]: The first five decimals of sin({1}) is {2}. The absolute of the first five decimals of cos({3}) is {4}.", _moduleId, string.Concat(nixie1, nixie2, lsd), sin, Displays[0].text, cos);

        //get final value for the stage
        _calculatedValues[currentStage] = cos + sin;
        Debug.LogFormat("[Forget The Colors #{0}]: Stage {1}'s value is sine's {2} + cosine's {3} which is {4}.", _moduleId, currentStage, sin, cos, (cos + sin).ToString("F0"));

        sineNumber[currentStage] = sin;
    }

    private double Modulo(double num, int mod)
    {
        //modulation for negatives
        if (num < 0)
        {
            num += mod;
            num = Modulo(num, mod);
        }

        //modulation for positives
        else if (num >= mod)
        {
            num -= mod;
            num = Modulo(num, mod);
        }

        //once it reaches here, we know it's modulated and we can return it
        return num;
    }

    //eases
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

    //twitch plays
    private bool IsValid(string par)
    {
        //if number is 00-99, return true, otherwise return false
        byte b;
        if (byte.TryParse(par, out b))
            return b < 100;

        return false;
    }

#pragma warning disable 414
    private readonly string TwitchHelpMessage = @"!{0} submit <##> (Cycles through both nixies to match '##', then hits submit. If in strike mode, submitting will get you out of strike mode and back to submission. | Valid numbers are from 0-99) !{0} preview <#> (If the module has struck, you can make # any valid stage number, which will show you what it displayed on that stage.)";
#pragma warning restore 414

    IEnumerator ProcessTwitchCommand(string command)
    {
        string[] buttonPressed = command.Split(' ');

        //colorblind
        if (Regex.IsMatch(command, @"^\s*colorblind\s*$", RegexOptions.IgnoreCase))
        {
            yield return null;
            _colorblind = !_colorblind;

            if (!_allowCycleStage)
                Render();
            else
                PreviousRender();
        }

        //submit command
        else if (Regex.IsMatch(buttonPressed[0], @"^\s*submit\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            yield return null;

            //turn the key to turn off
            if (_allowCycleStage)
                Selectables[2].OnInteract();

            //if command has no parameters
            else if (buttonPressed.Length < 2)
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
                if (_canInteract)
                    for (byte i = 0; i < Selectables.Length - 1; i++)
                    {
                        //keep pushing until button value is met by player
                        while (int.Parse(NixieValues[i].text) != values[i])
                        {
                            yield return new WaitForSeconds(0.05f);
                            Selectables[i].OnInteract();
                        }
                    }

                //key
                yield return new WaitForSeconds(0.1f);
                Selectables[2].OnInteract();
            }
        }

        else if (Regex.IsMatch(buttonPressed[0], @"^\s*preview\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            uint n;

            //if command has no parameters
            if (!_allowCycleStage)
                yield return "sendtochaterror This command can only be executed when the module is in strike mode!";

            //if command has no parameters
            else if (buttonPressed.Length < 2)
                yield return "sendtochaterror Please specify the value to submit! (Valid: 0-<Max number of stages>)";

            //if command has too many parameters
            else if (buttonPressed.Length > 2)
                yield return "sendtochaterror Too many parameters! Please submit only 1 number.";

            //if command has an invalid parameter
            else if (!uint.TryParse(buttonPressed[1], out n) || n >= maxStage)
                yield return "sendtochaterror Invalid number! Make sure you aren't exceeding the amount of stages!";

            else
            {
                //keep pushing until button value is met by player
                do
                {
                    yield return new WaitForSeconds(0.02f);
                    Selectables[1].OnInteract();
                } while (n != stage);
            }
        }
    }

    IEnumerator TwitchHandleForcedSolve()
    {
        Debug.LogFormat("[Forget The Colors #{0}]: Admin has initiated an auto-solve. Thank you for attempting FTC. You gave up on stage {1}.", _moduleId, stage);

        while (!_canInteract)
            yield return true;

        yield return new WaitForSeconds(1f);

        for (byte i = 0; i < 2; i++)
            while (_solution.ToString().ToCharArray()[i].ToString() != NixieValues[i].text)
            {
                Selectables[i].OnInteract();
                yield return new WaitForSeconds(0.05f);
                Render();
            }

        if (int.Parse(string.Concat(NixieValues[0].text, NixieValues[1].text)) == _solution)
        {
            yield return new WaitForSeconds(0.1f);
            Selectables[2].OnInteract();
        }
        yield return null;
    }
}

/// <summary>
/// Datatype for use in RuleSeed, containing 3 byte values.
/// </summary>
sealed class Rule
{
    public byte Cylinder, Edgework, Operator;
}