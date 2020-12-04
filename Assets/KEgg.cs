using UnityEngine;

public class KEgg : MonoBehaviour {
    public GameObject self;

    public void Show()
    {
        self.GetComponent<MeshRenderer>().enabled = true;
    }

    public void Hide()
    {
        self.GetComponent<MeshRenderer>().enabled = false;
    }
}