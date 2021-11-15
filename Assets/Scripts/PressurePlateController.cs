using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlateController : MonoBehaviour
{
    [SerializeField] private float massNeeded = 1f;
    [SerializeField] private float timeNeeded = 2f;
    [SerializeField] private bool isActive = false;
    [SerializeField] private Animation animation;

    private float timer = 0f;
    private bool once = true;
    // Start is called before the first frame update
    void Start()
    {
        //Get the animator from the gameObject
        animation = GetComponent<Animation>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") || (other.CompareTag("Object") && massNeeded >= other.gameObject.GetComponent<Rigidbody>().mass))
        {
            timer += Time.deltaTime;
            if(timer >= timeNeeded && once){
                animation.Play("Down");
                isActive = true;
                once = false;
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Object"))
        {
            timer = 0f;
            once = true;
            isActive = false;
            animation.Play("Up");
        }
    }

    public bool GetIsActive(){
        return isActive;
    }
}
