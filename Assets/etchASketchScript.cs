using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class etchASketchScript : MonoBehaviour {
    public KMBombModule Module;

    public KMSelectable LeftLeft;
    public KMSelectable LeftRight;
    public KMSelectable RightLeft;
    public KMSelectable RightRight;
    public Transform KnobLeft;
    public Transform KnobRight;
    public Transform Cursor;
    public GameObject Pixel;
    public Material PixelShaken;
    public Transform TopLeft;
    public Transform BottomRight;
    public Transform PixelContainer;

    private List<GameObject> Pixels = new List<GameObject>();

    private static int _moduleIdCounter = 1;
    private int _moduleId = 0;

    private float moveDelay = 0.02f;

    private int totalDrawn = 0;

    private bool _isSolved = false;

    private List<int> easterEggs = new List<int>();

    //Shake-to-clear stuff
    private static readonly float MAXIMUM_ROTATION_VALUE = 30.0f;
    private Transform _rootXForm = null;
    private Quaternion _rootObjectLastOrientation = Quaternion.identity;
    private Quaternion RootObjectOrientation
    {
        get
        {
            if (_rootXForm != null)
            {
                return _rootXForm.rotation;
            }

            return Quaternion.identity;
        }
    }

    // Use this for initialization
    void Start () {
        //Shake-to-clear
        _rootXForm = transform.root;
        //Logging
        _moduleId = _moduleIdCounter++;

        Module.OnActivate += Activate;
    }

	// Update is called once per frame
	void Update () {
        //Shake-to-clear
        ClearCheck();
    }

    private void Awake()
    {
        // Add button functionality
        LeftLeft.OnInteract += delegate { StartCoroutine("MoveLeft"); CheckEgg(0); return false; };
        LeftRight.OnInteract += delegate { StartCoroutine("MoveRight"); CheckEgg(1); return false; };
        RightLeft.OnInteract += delegate { StartCoroutine("MoveDown"); CheckEgg(2); return false; };
        RightRight.OnInteract += delegate { StartCoroutine("MoveUp"); CheckEgg(3); return false; };
        LeftLeft.OnInteractEnded += delegate { StopCoroutine("MoveLeft"); };
        LeftRight.OnInteractEnded += delegate { StopCoroutine("MoveRight"); };
        RightLeft.OnInteractEnded += delegate { StopCoroutine("MoveDown"); };
        RightRight.OnInteractEnded += delegate { StopCoroutine("MoveUp"); };
        //Initialize start pixel
        DrawNewPixel(Cursor.localPosition.x, Cursor.localPosition.z);
    }


    //Movement coroutines
    private IEnumerator MoveUp()
    {
        while (true)
        {
            if (TopLeft.localPosition.z > Cursor.localPosition.z)
            { 
                Cursor.Translate(new Vector3(0, 0, 0.001f));
            }
            DrawNewPixel(Cursor.localPosition.x, Cursor.localPosition.z);
            KnobRight.Rotate(new Vector3(0, 0, 2));
            yield return new WaitForSeconds(moveDelay);
        }
    }

    private IEnumerator MoveDown()
    {
        while (true)
        {
            if (BottomRight.localPosition.z < Cursor.localPosition.z)
                Cursor.Translate(new Vector3(0, 0, -0.001f));
            DrawNewPixel(Cursor.localPosition.x, Cursor.localPosition.z);
            KnobRight.Rotate(new Vector3(0, 0, -2));
            yield return new WaitForSeconds(moveDelay);
        }
    }

    private IEnumerator MoveLeft()
    {
        while (true)
        {
            if (TopLeft.localPosition.x < Cursor.localPosition.x)
                Cursor.Translate(new Vector3(-0.001f, 0, 0));
            DrawNewPixel(Cursor.localPosition.x, Cursor.localPosition.z);
            KnobLeft.Rotate(new Vector3(0, 0, -2));
            yield return new WaitForSeconds(moveDelay);
        }
    }

    private IEnumerator MoveRight()
    {
        while (true)
        {
            if(BottomRight.localPosition.x > Cursor.localPosition.x)
                Cursor.Translate(new Vector3(0.001f, 0, 0));
            DrawNewPixel(Cursor.localPosition.x, Cursor.localPosition.z);
            KnobLeft.Rotate(new Vector3(0, 0, 2));
            yield return new WaitForSeconds(moveDelay);
        }
    }

    //Draw new pixel
    private void DrawNewPixel(float x, float z)
    {
        foreach (GameObject p in Pixels)
        {
            if(p.transform.localPosition.x == x && p.transform.localPosition.z == z) { return; }
        }
        GameObject newPixel = Instantiate(Pixel);
        newPixel.transform.SetParent(PixelContainer);
        newPixel.transform.localPosition = new Vector3(x, Cursor.localPosition.y, z);
        Pixels.Add(newPixel);
        totalDrawn++;
        if (totalDrawn >= 1000 && !_isSolved)
        {
            Module.HandlePass();
            _isSolved = true;
            Debug.LogFormat("[Etch-A-Sketch #{0}] You have made some wonderful art! Good job. Module solved.", _moduleId);
        }
    }

    void Activate()
    {
        _rootObjectLastOrientation = RootObjectOrientation;
    }

    //Shake-to-clear function
    private void ClearCheck()
    {
        Quaternion currentOrientation = RootObjectOrientation;
        if (Mathf.Abs(Quaternion.Angle(currentOrientation, _rootObjectLastOrientation)) >= MAXIMUM_ROTATION_VALUE)
        {
            GameObject[] tmpPixels = new GameObject[Pixels.Count];
            Pixels.CopyTo(tmpPixels);
            foreach (GameObject p in tmpPixels)
            {
                if (Random.Range(0f, 100f) < 15f)
                {
                    if (p.GetComponent<PixelData>().Next())
                    {
                        p.GetComponent<MeshRenderer>().material = PixelShaken;
                    }
                    else
                    {
                        Pixels.Remove(p);
                        Destroy(p);
                    }
                }
            }
        }
        _rootObjectLastOrientation = currentOrientation;
    }

    void CheckEgg(int value)
    {
        int[] eggKonami = { 3, 3, 2, 2, 0, 1, 0, 1 };
        int[] eggBagels = { 1, 0, 0, 2, 1, 0, 1, 3, 1, 0, 3, 0, 1, 1, 0, 3 };
        List<int> eggKonamiList = new List<int>();
        List<int> eggBagelsList = new List<int>();
        eggKonamiList.AddRange(eggKonami);
        eggBagelsList.AddRange(eggBagels);
        easterEggs.Add(value);
        int[] eggsTmp2 = new int[easterEggs.Count];
        List<int> eggsTmp = new List<int>();
        easterEggs.CopyTo(eggsTmp2);
        eggsTmp.AddRange(eggsTmp2);
        while (eggsTmp.Count > 16) { eggsTmp.RemoveAt(0); }
        if (eggsTmp.Join() == eggBagelsList.Join()) { StartCoroutine("Bagels"); }
        while (eggsTmp.Count > 8) { eggsTmp.RemoveAt(0); }
        if (eggsTmp.Join() == eggKonamiList.Join()) { StartCoroutine("Konami"); }
    }

    private IEnumerator Konami()
    {
        Module.GetComponentInChildren<KEgg>().Show();
        yield return new WaitForSeconds(1f);
        Module.GetComponentInChildren<KEgg>().Hide();
    }

    private IEnumerator Bagels()
    {
        Module.GetComponentInChildren<BEgg>().Show();
        yield return new WaitForSeconds(1f);
        Module.GetComponentInChildren<BEgg>().Hide();
    }
}