using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KModkit;
using System.Linq;
using System;

//whoops this code is outdated!! make sure you are using FTC.cs

#pragma warning disable 649
#pragma warning disable 414

public class FMNever : MonoBehaviour {

    public KMAudio Audio;
    public KMBombInfo Bomb;
    public TextMesh[] Number;
    public Renderer[] Colorchanger;
    public KMSelectable[] Buttons;
    public Color[] Color = new Color[10];

    int moduleID;

    private double[] TempStorage = new double[6];
	private List<string> solvable;
	private string[] ignor = {"Forget The Colors", "14", "Bamboozling Time Keeper", "Brainf---", "Forget Enigma", "Forget Everything", "Forget It Not", "Forget Me Not", "Forget Me Later", "Forget Perspecive", "Forget Them All", "Forget This", "Forget Us Not", "Organization", "Purgatory", "Simon Forgets", "Simons's Stages", "Souvenir", "Tallordered Keys", "The Time Keeper", "The Troll", "The Very Annoying Button", "Timing Is Everything", "Turn The Key", "Ultimate Custom Night", "Übermodule"};
	private int[] Nixies = new int[2];
	private int[] MainDisplays = new int[2];
	private int[] colornums = new int[4];
	private int GEER;
	private int[] StoredValues;
	private int Stage;
	private int index;
    static int moduleIdCounter = 1;
    void Awake() {

	}
	// Use this for initialization
	void Start () {
		Stage=0;
		/*solvable = Bomb.GetSolvableModuleNames();
		foreach (string moduleName in solvable)
        {if (ignor.Contains(moduleName)){solvable.Remove(moduleName);}}
		StoredValues = new int[solvable.Count()];*/
		StartCoroutine(Generate());
	}
	IEnumerator Generate() {
		if (Stage==0){
		for (int i=0; i<75; i++) {
			MainDisplays[0] = UnityEngine.Random.Range(0,991);
			MainDisplays[1] = UnityEngine.Random.Range(0,100);
			Nixies[0] = UnityEngine.Random.Range(0,10);
			Nixies[1] = UnityEngine.Random.Range(0,10);
			colornums[0] = UnityEngine.Random.Range(0,10);
			colornums[1] = UnityEngine.Random.Range(0,10);
			colornums[2] = UnityEngine.Random.Range(0,10);
			colornums[3] = UnityEngine.Random.Range(0,10);
			GEER = UnityEngine.Random.Range(0,10);
		yield return new WaitForSeconds(.075f);}}
		else{
			for (int i=0; i<25; i++) {
			MainDisplays[0] = UnityEngine.Random.Range(0,991);
			MainDisplays[1] = UnityEngine.Random.Range(0,100);
			Nixies[0] = UnityEngine.Random.Range(0,10);
			Nixies[1] = UnityEngine.Random.Range(0,10);
			colornums[0] = UnityEngine.Random.Range(0,10);
			colornums[1] = UnityEngine.Random.Range(0,10);
			colornums[2] = UnityEngine.Random.Range(0,10);
			colornums[3] = UnityEngine.Random.Range(0,10);
			GEER = UnityEngine.Random.Range(0,10);
			yield return new WaitForSeconds(.075f);}
		}
		Calculate();
		MainDisplays[1] = Stage;
		StopCoroutine(Generate());
	}
	// Update is called once per frame
	void Update () {
		if (MainDisplays[0]<10){
		Number[0].text = "00"+MainDisplays[0].ToString();}
		else if (MainDisplays[0]<100){
		Number[0].text = "0"+MainDisplays[0].ToString();}
		else{
	Number[0].text = MainDisplays[0].ToString();}
		if ((MainDisplays[1]%100)<10){
		Number[1].text = "0"+(MainDisplays[1]%100).ToString();}
		else{
		Number[1].text = (MainDisplays[1]%100).ToString();}
		Number[2].text = Nixies[0].ToString();
		Number[3].text = Nixies[1].ToString();
		Number[4].text = GEER.ToString();
		Colorchanger[0].material.SetColor("_Color", Color[colornums[0]]);
		Colorchanger[1].material.SetColor("_Color", Color[colornums[1]]);
		Colorchanger[2].material.SetColor("_Color", Color[colornums[2]]);
		Colorchanger[3].material.SetColor("_Color", Color[colornums[3]]);
		if (Stage != Bomb.GetSolvedModuleNames().Count()) {
			Stage = Stage+1;
			StartCoroutine(Generate());
		}
	}
	void Calculate() {
		TempStorage[3]=5;
		TempStorage[4]=5;
		TempStorage[5]=GEER;
		for(int i=0;i<3;i++){
			switch (colornums[i])
			{
				case 0:
					TempStorage[3] += 5f;
					if (TempStorage[3] > 9) { TempStorage[3] -= 10f; }
					break;
				case 1:
					TempStorage[3] -= 1f;
					if (TempStorage[3] < 0) { TempStorage[3] += 10f; }
					break;
				case 2:
					TempStorage[3] += 3f;
					if (TempStorage[3] > 9) { TempStorage[3] -= 10f; }
					break;
				case 3:
					TempStorage[3] += 7f;
					if (TempStorage[3] > 9) { TempStorage[3] -= 10f; }
					break;
				case 4:
					TempStorage[3] -= 7f;
					if (TempStorage[3] < 0) { TempStorage[3] += 10f; }
					break;
				case 5:
					TempStorage[3] += 8f;
					if (TempStorage[3] > 9) { TempStorage[3] -= 10f; }
					break;
				case 6:
					TempStorage[3] += 5f;
					if (TempStorage[3] > 9) { TempStorage[3] -= 10f; }
					break;
				case 7:
					TempStorage[3] -= 9f;
					if (TempStorage[3] < 0) { TempStorage[3] += 10f; }
					break;
				case 8:
					break;
				case 9:
					TempStorage[3] -= 3f;
					if (TempStorage[3] < 0) { TempStorage[3] += 10f; }
					break;
				default:
					break;
			}
			switch (colornums[i]) {
                case 0:
                    TempStorage[4] -= 1f;
					if (TempStorage[4]<0){TempStorage[4] += 10f;}
                    break;
                case 1:
                    TempStorage[4] -= 6f;
					if (TempStorage[4]<0){TempStorage[4] += 10f;}
                    break;
                case 2:
                    break;
                case 3:
                    TempStorage[4] -= 4f;
					if (TempStorage[4]<0){TempStorage[4] += 10;}
                    break;
                case 4:
                    TempStorage[4] -= 5;
					if (TempStorage[4]<0){TempStorage[4] += 10;}
                    break;
                case 5:
                    TempStorage[4]=TempStorage[4]+9;
					if (TempStorage[4]>9){TempStorage[4] -= 10;}
                    break;
                case 6:
                    TempStorage[4]=TempStorage[4]-9;
					if (TempStorage[4]<0){TempStorage[4] += 10;}
                    break;
                case 7:
                    TempStorage[4]=TempStorage[4]+4;
					if (TempStorage[4]>9){TempStorage[4] -= 10;}
                    break;
                case 8:
					TempStorage[4]=TempStorage[4]+7;
					if (TempStorage[4]>9){TempStorage[4] -= 10;}
                    break;
                case 9:
                    TempStorage[4]=TempStorage[4]+5;
					if (TempStorage[4]>9){TempStorage[4] -= 10;}
                    break;
                default:
                    break;
            }
		}
			TempStorage[0] = Math.Floor(Math.Abs(((Math.Cos(MainDisplays[0]*Mathf.Deg2Rad)))*Math.Pow(10,5)));
			Debug.LogFormat("[Forget The Colors #{0}]: For stage '{1}', the Main Number is {4}, the modified values of the Nixie tubes are {2} and {3}", moduleID, Stage, TempStorage[3],TempStorage[4], TempStorage[0]);
			
	}
}

#pragma warning restore 649
#pragma warning restore 414