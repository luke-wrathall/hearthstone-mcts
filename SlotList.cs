using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotList : MonoBehaviour
{
    // Start is called before the first frame update
    public List<Transform> slots;
    public int numSlots;
    public int startX;
    public int endX;

    void Start()
    {
        int num = slots.Count;
        float spacing = (endX - startX) / num;
        float currentX = startX;
        foreach (Transform t in slots)
        {
            t.transform.position = new Vector3(currentX, this.transform.position.y, this.transform.position.z);
            currentX += spacing;
        }
    }
}
