using UnityEngine;

public class PixelData : MonoBehaviour {
    private int state = 0;

    public bool Next()
    {
        state += 1;
        return (state == 2);
    }
}
