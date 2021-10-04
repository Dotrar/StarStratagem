using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.SpriteShape;
using UnityEngine;
using UnityEngine.Animations;


public class ReticleRotation : MonoBehaviour
{
    // Start is called before the first frame update
    public SpriteRenderer layerOneSpriteRenderer;
    public SpriteRenderer layerTwoSpriteRenderer;
    public SpriteRenderer layerThreeSpriteRenderer;
    [Range(1f, 100.0f)] public float layerOneSpeed;
    [Range(1f, 100.0f)] public float layerTwoSpeed;
    [Range(1f, 100.0f)] public float layerThreeSpeed;

    void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        //this code is dumb
        // note layer 2 is backwards for effect
        float dt = Time.deltaTime;
        layerOneSpriteRenderer.gameObject.transform.Rotate(0,0, layerOneSpeed * dt);
        layerTwoSpriteRenderer.gameObject.transform.Rotate(0,0,layerTwoSpeed * -dt);
        layerThreeSpriteRenderer.gameObject.transform.Rotate(0,0,layerThreeSpeed * dt);
    }
}
