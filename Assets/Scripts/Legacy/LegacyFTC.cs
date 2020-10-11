using LegacyForgetTheColors;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LegacyFTC : MonoBehaviour
{
    //import assets
    public KMAudio Audio;
    public KMBombModule Module;
    public KMBossModule Boss;
    public KMBombInfo BombInfo;
    public KMColorblindMode Colorblind;
    public KMRuleSeedable Rule;
    public KMSelectable[] Selectables;
    public Renderer[] ColoredObjects;
    public Transform Gear, Key;
    public Transform[] CylinderDisks;
    public TextMesh GearText, DebugText;
    public TextMesh[] DisplayTexts, NixieTexts;
    public Texture[] ColorTextures;

    //required in TP
    internal bool active, allowCycleStage, canSolve;
    internal sbyte _solution = -1;
    internal int stage, maxStage = 5;

    //large souvenir dump
    bool solved;
    List<byte> gear = new List<byte>(0);
    List<short> largeDisplay = new List<short>(0);
    List<int> sineNumber = new List<int>(0);
    List<string> gearColor = new List<string>(0), ruleColor = new List<string>(0);

    //variables for solving
    private bool _isRotatingGear, _isRotatingKey, _colorblind;
    private sbyte _debugPointer;
    private const int _angleIncreasePerSolve = 2;
    private int _moduleId, _currentAngle;
    private int[] _colorValues = new int[4];
    private float _ease, _sum;
    private List<byte> _cylinder = new List<byte>(0), _nixies = new List<byte>(0);
    private List<int> _calculatedValues = new List<int>(0);

    private readonly LegacyGenerate generate = new LegacyGenerate();
    private readonly LegacyHandleSelect handleSelect = new LegacyHandleSelect();
    private readonly LegacyInit init = new LegacyInit();
    private readonly LegacyRender moduleRender = new LegacyRender();
    private static LegacyRule[][] _rules;

    internal void Activate(ref int moduleId)
    {
        active = true;
        _isRotatingKey = true;
        _moduleId = moduleId;

        //establish buttons
        for (byte i = 0; i < Selectables.Length; i++)
        {
            byte j = i;
            Selectables[j].OnInteract += delegate
            {
                handleSelect.Press(j, ref Audio, ref Selectables, ref solved, ref canSolve, ref allowCycleStage, ref stage, ref maxStage, ref DisplayTexts, ref largeDisplay, ref NixieTexts, ref _nixies, ref GearText, ref gear, ref _colorblind, ref gearColor, ref ColoredObjects, ref ColorTextures, ref _cylinder, ref CylinderDisks, ref _colorValues, ref _debugPointer, ref _moduleId, ref _rules, ref BombInfo, ref ruleColor, ref _calculatedValues, ref sineNumber, ref _sum, ref _solution, ref _isRotatingGear, ref _currentAngle, _angleIncreasePerSolve, ref _ease, ref Module, ref _isRotatingKey);
                return false;
            };
        }

        init.Start(ref Boss, ref _colorblind, ref Colorblind, ref Rule, ref _moduleId, ref _rules, ref maxStage, ref BombInfo, ref _cylinder, ref _nixies, ref gear, ref largeDisplay, ref _calculatedValues, ref sineNumber, ref gearColor, ref ruleColor, ref NixieTexts, ref Audio, ref Module, ref DisplayTexts, ref DebugText);
        StartCoroutine(generate.NewStage(solved, Audio, Module, _colorValues, 0, maxStage, _solution, DisplayTexts, GearText, NixieTexts, canSolve, ColoredObjects, ColorTextures, CylinderDisks, _colorblind, _moduleId, _calculatedValues, _sum, gear, gearColor, largeDisplay, _nixies, _cylinder, _rules, BombInfo, ruleColor, sineNumber));
    }

    private void FixedUpdate()
    {
        if (!active)
            return;

        if (moduleRender.Animate(ref Gear, _angleIncreasePerSolve, ref _currentAngle, ref allowCycleStage, ref _ease, ref _isRotatingGear, ref canSolve, ref solved, ref Selectables, ref Key, ref _isRotatingKey, ref stage, ref BombInfo) &&
            !solved && !allowCycleStage && !_isRotatingGear && stage < BombInfo.GetSolvedModuleNames().Where(module => !LegacyStrings.Ignore.Contains(module)).Count())
        {
            //generate a stage
            StartCoroutine(generate.NewStage(solved, Audio, Module, _colorValues, ++stage, maxStage, _solution, DisplayTexts, GearText, NixieTexts, canSolve, ColoredObjects, ColorTextures, CylinderDisks, _colorblind, _moduleId, _calculatedValues, _sum, gear, gearColor, largeDisplay, _nixies, _cylinder, _rules, BombInfo, ruleColor, sineNumber));

            //allows rotation
            _currentAngle += _angleIncreasePerSolve;
            _ease = 0;
        }
    }
}
