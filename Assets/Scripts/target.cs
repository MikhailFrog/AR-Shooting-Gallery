using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class target : MonoBehaviour
{
    Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
    }
    public void take_bullet()
    {
        animator.SetTrigger("down");
        StartCoroutine(return_target());
    }
    IEnumerator return_target()
    {
        yield return new WaitForSeconds(5);
        animator.SetTrigger("up");
    }
}
