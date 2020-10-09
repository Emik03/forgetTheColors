using System;
using System.Linq;
using UnityEngine;
using Rnd = System.Random;

public class Render 
{
    public Render(FTCScript FTC, Init init)
    {
        this.FTC = FTC;
        this.init = init;

        int seed = UnityEngine.Random.Range(0, int.MaxValue);
        rnd = new Rnd(seed);
        Debug.LogFormat("[Forget The Colors #{0}]: Seed is {1}.", init.moduleId, seed);
    }

    internal bool colorblind;

    private readonly FTCScript FTC;
    private readonly Init init;

    private bool isRotatingKey, isRotatingGear, allowCycleStage, canSolve;
    private short currentAngle;
    private const int angleIncreasePerSolve = 2;
    private float ease;
    private readonly Rnd rnd;

    internal void Colorblind()
    {
        for (int i = 0; i < FTC.CylinderDisks.Length; i++)
            FTC.CylinderDisks[i].localRotation = new Quaternion(90 * Convert.ToByte(colorblind), -90, 0, 0);

        for (int i = 0; i < FTC.ColoredObjects.Length; i++)
            FTC.ColoredObjects[i].material.SetTextureOffset("_MainTex", new Vector2(0.5f * Convert.ToByte(colorblind) * Convert.ToByte(init.maxStage != init.stage), -0.04f));

        FTC.GearText.characterSize = 0.1f - (Convert.ToByte(colorblind) * 0.04f);

        FTC.ColoredObjects[3].material.SetTextureScale("_MainTex", new Vector2(0, 0));
        int gearIndex = Functions.GetColorIndex(3, FTC);

        if (colorblind)
            FTC.GearText.text = gearIndex != 7 ? Arrays.ColorLog[gearIndex].First() + FTC.GearText.text.Last().ToString()
                                               : FTC.GearText.text = 'I' + FTC.GearText.text.Last().ToString();
    }
    internal void AssignRandom(bool legacy)
    {
        if (legacy)
            Assign(displays: new[] { rnd.Next(0, 1000), rnd.Next(0, 100) },
                   gears: new[] { rnd.Next(0, 10), rnd.Next(0, 10) },
                   nixies: new[] { rnd.Next(0, 10), rnd.Next(0, 10) },
                   cylinders: new[] { rnd.Next(0, 10), rnd.Next(0, 10), rnd.Next(0, 10) });

        else
            Assign(displays: new[] { rnd.Next(0, 1000), rnd.Next(0, 100) },
                   gears: new[] { rnd.Next(0, 10), rnd.Next(0, 8) },
                   nixies: new[] { rnd.Next(0, 10), rnd.Next(0, 10) },
                   cylinders: Functions.Random(3, 0, 8));
    }

    internal void Assign(int[] displays, int[] gears, int[] nixies, int[] cylinders)
    {
        if (displays.Length != FTC.DisplayTexts.Length || gears.Length != 2 || nixies.Length != FTC.NixieTexts.Length || cylinders.Length != FTC.ColoredObjects.Length - 1)
            throw new ArgumentOutOfRangeException("displays, gears, nixies, cylinders", "CoroutineScript.Render failed to run because the parameters provided had incorrect length!: " + displays.Length.ToString() + gears.Length.ToString() + nixies.Length.ToString() + cylinders.Length.ToString());

        FTC.GearText.text = gears[0].ToString();
        FTC.ColoredObjects[3].material.mainTexture = FTC.ColorTextures[gears[1]];

        for (int i = 0; i < cylinders.Length; i++)
            FTC.ColoredObjects[i].material.mainTexture = FTC.ColorTextures[cylinders[i]];

        for (int i = 0; i < displays.Length; i++)
        {
            FTC.DisplayTexts[i].text = displays[i].ToString();
            FTC.NixieTexts[i].text = nixies[i].ToString();
        }

        for (int i = 0; i < FTC.DisplayTexts.Length; i++)
        {
            while (FTC.DisplayTexts[i].text.Length < 3 - i)
            {
                FTC.DisplayTexts[i].text = '0' + FTC.DisplayTexts[i].text;
            }
        }
    }

    internal bool Animate()
    {
        if (FTC.init.solved)
        {
            // Expansion of the cylinder.
            if (ease <= 1)
            {
                ease += 0.02f;

                FTC.Selectables[2].transform.localRotation = Quaternion.Euler(0, Functions.BackOut(ease) * 420, 0);
                FTC.Key.localScale = new Vector3(Functions.ElasticOut(ease) * 0.5f, 1, Functions.ElasticOut(ease) * 0.5f);
            }

            // Retraction of the cylinder.
            else if (ease <= 2)
            {
                ease += 0.04f;

                FTC.Selectables[2].transform.localPosition = new Vector3(0, (Functions.BackIn(ease - 1) * -3) - 0.91f, 0);
                FTC.Key.localScale = new Vector3((1 - Functions.ElasticIn(ease - 1)) / 2, 1, (1 - Functions.ElasticIn(ease - 1)) / 2);
            }

            // Last frame. (Make it invisible)
            else
                FTC.Key.localPosition = new Vector3(0, -0.2f, 0);
        }

        // Failed key spin.
        else if (isRotatingKey)
        {
            ease += 0.04f;
            FTC.Selectables[2].transform.localRotation = Quaternion.Euler(0, (Functions.ElasticOut(ease) - ease) * 69, 0);

            if (ease >= 1)
            {
                isRotatingKey = false;
                ease = 0;
            }
        }

        //spin to next destination, every solve will give a new angle clockwise to itself
        else if (FTC.Gear.localRotation.y != angleIncreasePerSolve + currentAngle && !allowCycleStage)
        {
            ease += 0.025f;
            isRotatingGear = ease <= 1;

            //when finished generating stages, spin counter-clockwise to the nearest neutral position
            if (canSolve)
            {
                FTC.Gear.localRotation = Quaternion.Euler(0, currentAngle % 360 * Math.Abs(Functions.CubicOut(ease) - 1), 0);

                if (ease > 1)
                    FTC.Gear.localRotation = Quaternion.Euler(0, 0, 0);
            }

            //when generating stages, spin clockwise randomly
            else
            {
                FTC.Gear.localRotation = Quaternion.Euler(0, Functions.CubicOut(ease) * angleIncreasePerSolve + currentAngle, 0);

                if (ease > 1)
                    FTC.Gear.localRotation = Quaternion.Euler(0, angleIncreasePerSolve + currentAngle, 0);
            }
        }

        else
            return !init.solved && !allowCycleStage && !isRotatingGear && 
                   init.stage < FTC.Info.GetSolvedModuleNames().Where(module => !Arrays.Ignore.Contains(module)).Count();

        return false;
    }
}
