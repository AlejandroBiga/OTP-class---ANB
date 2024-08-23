using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lantern : MonoBehaviour
{
    public int LayerIndext = 1;
    public float transitionSpeed = 2f;
    private Animator animator;
    public bool isLayerActive = false;
    private float targetWeight = 0f;
    private InputManager inputManager;
    void Start()
    {
        animator = GetComponent<Animator>();
        inputManager = InputManager.Instance;
    }

    
    void Update()
    {
        if (inputManager.getLanter())
        {
            isLayerActive = !isLayerActive;
            targetWeight = isLayerActive ? 1.0f : 0.0f;
        }
        float currentWeight = animator.GetLayerWeight(LayerIndext);
        currentWeight = Mathf.Lerp(currentWeight, targetWeight, Time.deltaTime * transitionSpeed);
        animator.SetLayerWeight(LayerIndext, currentWeight);
    }
}
