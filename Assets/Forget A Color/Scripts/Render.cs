using System;
using System.Linq;
using UnityEngine;
using Rnd = UnityEngine.Random;

namespace ForgetAColor
{
    public class Render
    {
        public Render(Calculate calculate, FACScript FAC, Init init)
        {
            this.calculate = calculate;
            this.FAC = FAC;
            this.init = init;
        }

        internal bool colorblind, turnKey;
        internal const int angleIncreasePerSolve = 2;
        internal int currentAngle;
        internal float ease;

        private readonly Calculate calculate;
        private readonly FACScript FAC;
        private readonly Init init;

        internal void AssignRandom(bool log)
        {
            Assign(displays: new[] { Rnd.Range(0, 1000), Rnd.Range(0, 100) },
                   gears: new[] { Rnd.Range(0, 10), Rnd.Range(0, 8) },
                   nixies: new[] { Rnd.Range(0, 10), Rnd.Range(0, 10) },
                   cylinders: Functions.Random(3, 0, 8),
                   log: log);
        }

        internal void Assign(int[] displays, int[] gears, int[] nixies, int[] cylinders, bool log)
        {
            if (displays == null || gears == null || nixies == null || cylinders == null)
            {
                AssignFinal();
                return;
            }

            if (displays.Length != FAC.DisplayTexts.Length || gears.Length != 2 || nixies.Length != FAC.NixieTexts.Length || cylinders.Length != FAC.ColoredObjects.Length - 1)
                throw new ArgumentOutOfRangeException("displays, gears, nixies, cylinders", "CoroutineScript.Render failed to run because the parameters provided had incorrect length!: " + displays.Length.ToString() + gears.Length.ToString() + nixies.Length.ToString() + cylinders.Length.ToString());

            SetGear(gears);

            for (int i = 0; i < cylinders.Length; i++)
                FAC.ColoredObjects[i].material.mainTexture = FAC.ColorTextures[cylinders[i]];

            FAC.ColoredObjects[3].material.mainTexture = FAC.ColorTextures[gears[1]];

            for (int i = 0; i < displays.Length; i++)
            {
                FAC.DisplayTexts[i].text = displays[i].ToString();
                FAC.NixieTexts[i].text = nixies[i].ToString();

                while (FAC.DisplayTexts[i].text.Length < 3 - i)
                    FAC.DisplayTexts[i].text = '0' + FAC.DisplayTexts[i].text;
            }

            // Stores the cylinder colors if logging is true.
            if (log)
            {
                for (int i = 0; i < init.cylinders.GetLength(1); i++)
                    init.cylinders[init.stage, i] = cylinders[i];
            }
        }

        private void AssignFinal()
        {
            FAC.DisplayTexts[0].text = FAC.DisplayTexts[1].text = string.Empty;
            FAC.NixieTexts[0].text = FAC.NixieTexts[1].text = string.Empty;

            for (byte i = 0; i < FAC.ColoredObjects.Length; i++)
                FAC.ColoredObjects[i].material.mainTexture = FAC.ColorTextures[8];

            FAC.ColoredObjects[3].material.SetTextureScale("_MainTex", new Vector2(0, 0));

            for (byte i = 0; i < FAC.CylinderDisks.Length; i++)
                FAC.CylinderDisks[i].localRotation = new Quaternion(0, -90, 0, 0);

            FAC.GearText.characterSize = 0.05f;
            FAC.GearText.text = "?";

            SetNixieAsInputs();
        }

        internal void SetNixieAsInputs()
        {
            int length = calculate.modifiedSequence.Count();
            FAC.NixieTexts[0].text = (length / 10 % 10).ToString();
            FAC.NixieTexts[1].text = (length % 10).ToString();
        }

        internal void Colorblind(bool colorblind)
        {
            for (int i = 0; i < FAC.CylinderDisks.Length; i++)
                FAC.CylinderDisks[i].localRotation = new Quaternion(90 * Convert.ToByte(colorblind), -90, 0, 0);

            for (int i = 0; i < FAC.ColoredObjects.Length; i++)
                FAC.ColoredObjects[i].material.SetTextureOffset("_MainTex", new Vector2(0.5f * Convert.ToByte(colorblind) * Convert.ToByte(Init.maxStage != init.stage), -0.05f));

            FAC.GearText.characterSize = 0.05f - (Convert.ToByte(colorblind) * 0.025f);

            FAC.ColoredObjects[3].material.SetTextureScale("_MainTex", new Vector2(0, 0));

            SetGear(GetGear());
        }

        private void SetGear(int[] gears)
        {
            FAC.GearText.text = gears[0].ToString();

            if (colorblind)
                FAC.GearText.text = Arrays.ColorLog[gears[1]] != "Pink" ? Arrays.ColorLog[gears[1]].First() + FAC.GearText.text
                                                                        : FAC.GearText.text = 'I' + FAC.GearText.text;
        }

        internal int[] GetGear()
        {
            int number;
            int.TryParse(FAC.GearText.text.Last().ToString(), out number);

            return new[] { number, Functions.GetColorIndex(3, FAC) };
        }

        internal bool Animate(bool animating)
        {
            if (FAC.init.solved)
            {
                // Expansion of the cylinder.
                if (ease <= 1)
                {
                    ease += 0.02f;

                    FAC.Selectables[2].transform.localRotation = Quaternion.Euler(0, Functions.BackOut(ease) * 420, 0);
                    FAC.Key.localScale = new Vector3(Functions.ElasticOut(ease) * 0.5f, 1, Functions.ElasticOut(ease) * 0.5f);
                }

                // Retraction of the cylinder.
                else if (ease <= 2)
                {
                    ease += 0.04f;

                    FAC.Selectables[2].transform.localPosition = new Vector3(0, (Functions.BackIn(ease - 1) * -3) - 0.91f, 0);
                    FAC.Key.localScale = new Vector3((1 - Functions.ElasticIn(ease - 1)) / 2, 1, (1 - Functions.ElasticIn(ease - 1)) / 2);
                }

                // Last frame. (Make it invisible)
                else
                    FAC.Key.localPosition = new Vector3(0, -0.2f, 0);
            }

            // Failed key spin.
            else if (turnKey)
            {
                ease += 0.04f;
                FAC.Selectables[2].transform.localRotation = Quaternion.Euler(0, (Functions.ElasticOut(ease) - ease) * 69, 0);

                if (ease >= 1)
                {
                    turnKey = false;
                    ease = 0;
                }
            }

            else
                return !animating && init.stage < Init.maxStage && init.stage < init.fakeStage + FAC.Info.GetSolvedModuleNames().Where(m => !Arrays.Ignore.Contains(m)).Count();

            return false;
        }
    }
}
