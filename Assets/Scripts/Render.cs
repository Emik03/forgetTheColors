using System;
using System.Linq;
using UnityEngine;
using Rnd = UnityEngine.Random;

public class Render 
{
    public Render(FTCScript FTC, Init init)
    {
        this.FTC = FTC;
        this.init = init;
    }

    internal bool colorblind, turnKey;
    internal const int angleIncreasePerSolve = 2;
    internal int currentAngle;
    internal float ease;

    private readonly FTCScript FTC;
    private readonly Init init;

    internal void AssignRandom()
    {
        if (init.legacy)
            Assign(displays: new[] { Rnd.Range(0, 1000), Rnd.Range(0, 100) },
                   gears: new[] { Rnd.Range(0, 10), Rnd.Range(0, 10) },
                   nixies: new[] { Rnd.Range(0, 10), Rnd.Range(0, 10) },
                   cylinders: new[] { Rnd.Range(0, 10), Rnd.Range(0, 10), Rnd.Range(0, 10) });

        else
            Assign(displays: new[] { Rnd.Range(0, 1000), Rnd.Range(0, 100) },
                   gears: new[] { Rnd.Range(0, 10), Rnd.Range(0, 8) },
                   nixies: new[] { Rnd.Range(0, 10), Rnd.Range(0, 10) },
                   cylinders: Functions.Random(3, 0, 8));
    }

    internal void Assign(int[] displays, int[] gears, int[] nixies, int[] cylinders)
    {
        if (displays == null || gears == null || nixies == null || cylinders == null)
        {
            FTC.DisplayTexts[0].text = string.Empty;
            FTC.DisplayTexts[1].text = string.Empty;

            for (byte i = 0; i < FTC.ColoredObjects.Length; i++)
                FTC.ColoredObjects[i].material.mainTexture = FTC.ColorTextures[10];

            FTC.ColoredObjects[3].material.SetTextureScale("_MainTex", new Vector2(0, 0));

            for (byte i = 0; i < FTC.CylinderDisks.Length; i++)
                FTC.CylinderDisks[i].localRotation = new Quaternion(0, -90, 0, 0);

            FTC.GearText.characterSize = 0.1f;
            FTC.GearText.text = string.Empty;

            return;
        }

        if (displays.Length != FTC.DisplayTexts.Length || gears.Length != 2 || nixies.Length != FTC.NixieTexts.Length || cylinders.Length != FTC.ColoredObjects.Length - 1)
            throw new ArgumentOutOfRangeException("displays, gears, nixies, cylinders", "CoroutineScript.Render failed to run because the parameters provided had incorrect length!: " + displays.Length.ToString() + gears.Length.ToString() + nixies.Length.ToString() + cylinders.Length.ToString());

        SetGear(gears[0].ToString());

        for (int i = 0; i < cylinders.Length; i++)
            FTC.ColoredObjects[i].material.mainTexture = FTC.ColorTextures[cylinders[i]];

        FTC.ColoredObjects[3].material.mainTexture = FTC.ColorTextures[gears[1]];

        for (int i = 0; i < displays.Length; i++)
        {
            FTC.DisplayTexts[i].text = displays[i].ToString();
            FTC.NixieTexts[i].text = nixies[i].ToString();

            while (FTC.DisplayTexts[i].text.Length < 3 - i)
            {
                FTC.DisplayTexts[i].text = '0' + FTC.DisplayTexts[i].text;
            }
        }
    }

    internal void Colorblind(bool colorblind)
    {
        for (int i = 0; i < FTC.CylinderDisks.Length; i++)
            FTC.CylinderDisks[i].localRotation = new Quaternion(90 * Convert.ToByte(colorblind), -90, 0, 0);

        for (int i = 0; i < FTC.ColoredObjects.Length; i++)
            FTC.ColoredObjects[i].material.SetTextureOffset("_MainTex", new Vector2(0.5f * Convert.ToByte(colorblind) * Convert.ToByte(init.maxStage != init.stage), -0.05f));

        FTC.GearText.characterSize = 0.05f - (Convert.ToByte(colorblind) * 0.025f);

        FTC.ColoredObjects[3].material.SetTextureScale("_MainTex", new Vector2(0, 0));

        SetGear(FTC.GearText.text.Last().ToString());
    }

    private void SetGear(string value)
    {
        FTC.GearText.text = value.ToString();
        int gearIndex = Functions.GetColorIndex(3, FTC);

        if (colorblind)
            FTC.GearText.text = gearIndex != 9 ? Arrays.ColorLog[gearIndex].First() + FTC.GearText.text
                                               : FTC.GearText.text = 'I' + FTC.GearText.text;
    }

    internal bool Animate(bool animating)
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
        else if (turnKey)
        {
            ease += 0.04f;
            FTC.Selectables[2].transform.localRotation = Quaternion.Euler(0, (Functions.ElasticOut(ease) - ease) * 69, 0);

            if (ease >= 1)
            {
                turnKey = false;
                ease = 0;
            }
        }

        else
            return init.stage < init.fakeStage + FTC.Info.GetSolvedModuleNames().Where(module => !Arrays.Ignore.Contains(module)).Count();

        return false;
    }
}
