using UnityEngine;

public class BEgg : MonoBehaviour
{
    public GameObject self;

    public void Show()
    {
        self.GetComponent<SpriteRenderer>().enabled = true;
    }

    public void Hide()
    {
        self.GetComponent<SpriteRenderer>().enabled = false;
    }
}