using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;

public class TPScript : MonoBehaviour
{
    public CoroutineScript Coroutine;
    public FTCScript FTC;

    private void Start()
    {
    }

    IEnumerator ProcessTwitchCommand(string command)
    {
        string[] buttonPressed = command.Split(' ');

        //colorblind
        if (Regex.IsMatch(command, @"^\s*colorblind\s*$", RegexOptions.IgnoreCase))
        {
            yield return null;
            //_colorblind = !_colorblind;

            //if (!_allowCycleStage)
            //    moduleRender.Update(ref _canSolve, ref DisplayTexts, ref GearText, ref ColoredObjects, ref ColorTextures, ref CylinderDisks, ref _colorValues, ref _colorblind, ref maxStage, ref stage);
            //else
            //    moduleRender.UpdateCycleStage(ref DisplayTexts, ref largeDisplay, ref stage, ref NixieTexts, ref _nixies, ref GearText, ref gear, ref _colorblind, ref maxStage, ref gearColor, ref ColoredObjects, ref ColorTextures, ref _cylinder, ref CylinderDisks);
        }

        //submit command
        else if (Regex.IsMatch(buttonPressed[0], @"^\s*submit\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            yield return null;
            byte n;

            //turn the key to turn off
            //if (_allowCycleStage)
            //    Selectables[2].OnInteract();

            //if command has no parameters
            if (buttonPressed.Length < 2)
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
                //byte[] values = new byte[2] { (byte)(byte.Parse(buttonPressed[1]) / 10), (byte)Ease.Modulo(byte.Parse(buttonPressed[1]), 10) };

                //submit answer only if it's ready
                //if (_canSolve)
                //    for (byte i = 0; i < Selectables.Length - 1; i++)
                //    {
                //        //keep pushing until button value is met by player
                //        while (int.Parse(NixieTexts[i].text) != values[i])
                //        {
                //            yield return new WaitForSeconds(0.05f);
                //            Selectables[i].OnInteract();
                //        }
                //    }

                //key
                yield return new WaitForSeconds(0.1f);
                //Selectables[2].OnInteract();
            }
        }

        else if (Regex.IsMatch(buttonPressed[0], @"^\s*preview\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            ushort n;

            //if not in strike mode
            //if (!_allowCycleStage)
            //    yield return "sendtochaterror This command can only be executed when the module is in strike mode!";

            //if command has no parameters
            if (buttonPressed.Length < 2)
                yield return "sendtochaterror Please specify the value to submit! (Valid: 0-<Max number of stages>)";

            //if command has too many parameters
            else if (buttonPressed.Length > 2)
                yield return "sendtochaterror Too many parameters! Please submit only 1 stage number.";

            //if command has an invalid parameter
            //else if (!ushort.TryParse(buttonPressed[1], out n) || n >= maxStage)
            //    yield return "sendtochaterror Invalid number! Make sure you aren't exceeding the amount of stages!";

            else
            {
                //keep pushing until button value is met by player
                //do
                //{
                //    yield return new WaitForSeconds(0.02f);
                //    Selectables[1].OnInteract();
                //} while (n != stage);
            }
        }
    }

    IEnumerator TwitchHandleForcedSolve()
    {
    //    Debug.LogFormat("[Forget The Colors #{0}]: Admin has initiated an auto-solve. Thank you for attempting FTC. You gave up on stage {1}.", _moduleId, stage);

        //while (!_canSolve)
        //    yield return true;

        yield return new WaitForSeconds(1f);

        //for (byte i = 0; i < 2; i++)
        //    while (_solution.ToString().ToCharArray()[i].ToString() != NixieTexts[i].text)
        //    {
        //        Selectables[i].OnInteract();
        //        yield return new WaitForSeconds(0.05f);
        //        moduleRender.Update(ref _canSolve, ref DisplayTexts, ref GearText, ref ColoredObjects, ref ColorTextures, ref CylinderDisks, ref _colorValues, ref _colorblind, ref maxStage, ref stage);
        //    }

        //if (int.Parse(string.Concat(NixieTexts[0].text, NixieTexts[1].text)) == _solution)
        //{
        //    yield return new WaitForSeconds(0.1f);
        //    Selectables[2].OnInteract();
        //}
    }
}
