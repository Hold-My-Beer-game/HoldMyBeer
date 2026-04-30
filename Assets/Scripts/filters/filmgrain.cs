using UnityEngine;
using UnityEngine.UI;

public class GrainMover : MonoBehaviour
{
    private RawImage grainImage;

    void Start()
    {
        grainImage = GetComponent<RawImage>();
    }

    void Update()
    {
        // Randomly shifts the texture slightly to simulate moving film noise
        grainImage.uvRect = new Rect(Random.value, Random.value, grainImage.uvRect.width, grainImage.uvRect.height);
    }
}