using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ItemAnnouncer : MonoBehaviour
{
    public TMP_Text name;
    public Animator animator;
    private Queue<string> queue = new Queue<string>();

    private void Update()
    {
        if(animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1)
        {
            if (queue.Count == 0)
                return;
            name.text = queue.Dequeue();
            animator.SetTrigger("Reanimate");
        }

    }

    public void animate(string str)
    {
        queue.Enqueue(str);
    }
}
