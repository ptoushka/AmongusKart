using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimPlayer : MonoBehaviour
{
    [SerializeField] private Animator animator;

    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
    }

    public void Play(string name)
    {
        animator.Play(name);
    }
}
