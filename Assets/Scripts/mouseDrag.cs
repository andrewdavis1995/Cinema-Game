using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(BoxCollider2D))]

public class mouseDrag : MonoBehaviour {

    public float offset;
    private Animator animator;

    bool triggerSet = false;
    
    void Update()
    {
    }

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void OnMouseDrag()
    {
        transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector3(transform.position.x + 0.75f, transform.position.y -1f, 0);
        transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);

       
        if (!triggerSet)
        {
            animator.SetTrigger("fly");
            triggerSet = true;
        }

        GameObject.Find("table").GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("table glow");
        
    }


    void OnMouseUp()
    {
        transform.localScale = new Vector3(1, 1, 1);
        animator.SetTrigger("land");
        triggerSet = false;
        transform.position = new Vector3(transform.position.x, transform.position.y + 1f, 0);

        GameObject.Find("table").GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("table");

        

    }
}