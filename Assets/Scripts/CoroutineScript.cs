using System.CodeDom.Compiler;
using UnityEngine;

public class CoroutineScript : MonoBehaviour
{
    public FTCScript FTC;
    public TPScript TP;

    private Init init;
    private Render render;

    private void Start()
    {
        init = FTC.init;
        render = init.render;
    }
}
