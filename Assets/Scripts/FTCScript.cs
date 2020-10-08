using UnityEngine;

public class FTCScript : MonoBehaviour
{
    public CoroutineScript Coroutine;
    public KMAudio Audio;
    public KMBombModule Module;
    public KMBossModule Boss;
    public KMBombInfo BombInfo;
    public KMColorblindMode Colorblind;
    public KMRuleSeedable Rule;
    public KMSelectable[] Selectables;
    public Renderer[] ColoredObjects;
    public TPScript TP;
    public Transform Gear, Key;
    public Transform[] CylinderDisks;
    public TextMesh GearText, DebugText;
    public TextMesh[] DisplayTexts, NixieTexts;
    public Texture[] ColorTextures;

    private void Awake ()
    {
        Module.OnActivate += new Init(Coroutine, this, TP).Start;
	}
}
