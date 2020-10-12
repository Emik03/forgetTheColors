using ForgetTheColors;
using LegacyForgetTheColors;
using System;
using System.Collections;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class TPScript : MonoBehaviour
{
    public CoroutineScript Coroutine;
    public FTCScript FTC;
    public LegacyFTC Legacy;

    private Calculate calculate;
    private Init init;
    private Render render;
    private Selectable selectable;

    private int stagesRewarded;

    private void Start()
    {
        init = FTC.init;

        calculate = init.calculate;
        render = init.render;
        selectable = init.selectable;
    }

    internal string TwitchHelpMessage = @"!press {0} <...> (Presses the left & right nixies from left to right until a strike is given) !{0} key <#> (Turns the key once or '#' times if specified)";

    private bool IsValid(string command)
    {
        return command.All(c => "LRlr".Contains(c));
    }

    private IEnumerator ProcessTwitchCommand(string command)
    {
        string[] buttonPressed = command.Split(' ');

        if (!Legacy.active)
        {
            // Debug command.
            if (Application.isEditor && Regex.IsMatch(command, @"^\s*next\s*$", RegexOptions.IgnoreCase))
            {
                yield return null;
                init.fakeStage++;
            }

            // Colorblind mode command.
            else if (Regex.IsMatch(command, @"^\s*colorblind\s*$", RegexOptions.IgnoreCase))
            {
                yield return null;
                render.Colorblind(render.colorblind = !render.colorblind);
            }

            else if (Regex.IsMatch(buttonPressed[0], @"^\s*key\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
            {
                yield return null;
                byte times;

                if (buttonPressed.Length < 2)
                    FTC.Selectables[2].OnInteract();

                else if (buttonPressed.Length > 2)
                    yield return "sendtochaterror Too many parameters specified! This command only expects 1 number!";

                else if (!byte.TryParse(buttonPressed[1], out times))
                    yield return "sendtochaterror Invalid submission! Only (reasonable) numbers are allowed.";

                else
                {
                    for (byte i = 0; i < times; i++)
                    {
                        FTC.Selectables[2].OnInteract();
                        yield return new WaitForSeconds(0.6f);
                    }
                }
            }

            // Submit command.
            else if (Regex.IsMatch(buttonPressed[0], @"^\s*press\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
            {
                yield return null;

                if (buttonPressed.Length < 2)
                    yield return "sendtochaterror Please specify the buttons to press! (Valid: L/R)";

                else if (buttonPressed.Length > 2)
                    yield return "sendtochaterror Too many parameters specified! If you are trying to input multiple buttons, do not use seperators!";

                else if (!IsValid(buttonPressed[1]))
                    yield return "sendtochaterror Invalid submission! Only L's and R's are valid.";

                // Valid command.
                else
                {
                    for (int i = 0; i < buttonPressed[1].Length; i++)
                    {
                        if ("Ll".Contains(buttonPressed[1][i]))
                            FTC.Selectables[0].OnInteract();

                        else
                            FTC.Selectables[1].OnInteract();

                        if (selectable.strike)
                            break;

                        yield return new WaitForSeconds(0.1f);
                    }

                    yield return "awardpoints " + Math.Floor((stagesRewarded - selectable.stagesCompleted) * Arrays.TPAwardPerStage);
                    stagesRewarded = selectable.stagesCompleted;
                }
            }
        }

        else
        {
            //submit command
            if (Regex.IsMatch(buttonPressed[0], @"^\s*submit\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
            {
                yield return null;
                byte n;

                //turn the key to turn off
                if (Legacy.allowCycleStage)
                    Legacy.Selectables[2].OnInteract();

                //if command has no parameters
                else if (buttonPressed.Length < 2)
                    yield return "sendtochaterror Please specify the value to submit! (Valid: 0-99)";

                //if command has too many parameters
                else if (buttonPressed.Length > 2)
                    yield return "sendtochaterror Too many parameters! Please submit only a single 2-digit number.";

                //if command has an invalid parameter
                else if (!byte.TryParse(buttonPressed[1], out n) || n >= 100)
                    yield return "sendtochaterror Invalid number! Only values 0-99 are valid.";

                //if command is valid, push button accordingly
                else
                {
                    //splits values
                    byte[] values = new byte[2] { (byte)(byte.Parse(buttonPressed[1]) / 10), (byte)LegacyEase.Modulo(byte.Parse(buttonPressed[1]), 10) };

                    if (string.Concat(Legacy.NixieTexts[0].text, Legacy.NixieTexts[1].text) == Legacy._solution.ToString())
                        yield return "awardpointsonsolve " + Math.Floor(init.maxStage * Arrays.LegacyTPAwardPerStage);

                    //submit answer only if it's ready
                    if (Legacy.canSolve)
                        for (byte i = 0; i < Legacy.Selectables.Length - 1; i++)
                        {
                            //keep pushing until button value is met by player
                            while (int.Parse(Legacy.NixieTexts[i].text) != values[i])
                            {
                                yield return new WaitForSeconds(0.05f);
                                Legacy.Selectables[i].OnInteract();
                            }
                        }

                    //key
                    yield return new WaitForSeconds(0.1f);
                    Legacy.Selectables[2].OnInteract();
                }
            }

            else if (Regex.IsMatch(buttonPressed[0], @"^\s*preview\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
            {
                ushort n;

                //if not in strike mode
                if (!Legacy.allowCycleStage)
                    yield return "sendtochaterror This command can only be executed when the module is in strike mode!";

                //if command has no parameters
                else if (buttonPressed.Length < 2)
                    yield return "sendtochaterror Please specify the value to submit! (Valid: 0-<Max number of stages>)";

                //if command has too many parameters
                else if (buttonPressed.Length > 2)
                    yield return "sendtochaterror Too many parameters! Please submit only 1 stage number.";

                //if command has an invalid parameter
                else if (!ushort.TryParse(buttonPressed[1], out n) || n >= Legacy.maxStage)
                    yield return "sendtochaterror Invalid number! Make sure you aren't exceeding the amount of stages!";

                else
                {
                    //keep pushing until button value is met by player
                    do
                    {
                        yield return new WaitForSeconds(0.02f);
                        Legacy.Selectables[1].OnInteract();
                    } while (n != Legacy.stage);
                }
            }
        }
    }

    private IEnumerator TwitchHandleForcedSolve()
    {
        Debug.LogFormat("[Forget The Colors #{0}]: An auto-solve has been issued. Thank you for attempting FTC. You gave up on stage {1}.", init.moduleId, init.stage + 1);

        if (!Legacy.active)
        {
            while (!Coroutine.animating && init.stage < init.maxStage && init.stage < init.fakeStage + FTC.Info.GetSolvedModuleNames().Where(m => !Arrays.Ignore.Contains(m)).Count())
                yield return true;

            while (calculate.modifiedSequence.Count > 0)
            {
                FTC.Selectables[Convert.ToByte(calculate.modifiedSequence[0])].OnInteract();
                yield return new WaitForSeconds(0.1f);
            }

            FTC.Selectables[2].OnInteract();
        }

        else
        {
            yield return new WaitForSeconds(1f);

            for (byte i = 0; i < 2; i++)
                while (Legacy._solution.ToString().ToCharArray()[i].ToString() != Legacy.NixieTexts[i].text)
                {
                    Legacy.Selectables[i].OnInteract();
                    yield return new WaitForSeconds(0.05f);
                }

            if (int.Parse(string.Concat(Legacy.NixieTexts[0].text, Legacy.NixieTexts[1].text)) == Legacy._solution)
            {
                yield return new WaitForSeconds(0.1f);
                Legacy.Selectables[2].OnInteract();
            }
        }
    }
}
