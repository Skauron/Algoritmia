using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    public TargetType type;
    public string objName;
    public GameObject Object;
    public float force;

    public void Hit()
    {
        ThidPersonMovementController player = GameObject.FindWithTag("Player").GetComponent<ThidPersonMovementController>();
        switch (type)
        {
            case (TargetType.Light):
                if (player.GetVariablePower(0))
                {
                    Object.SetActive(!Object.activeSelf);
                }
                break;
            case (TargetType.Object):
                if (player.GetVariablePower(1))
                {
                    player.SetTelekinesis(!player.GetTelekinesis(), gameObject);
                }
                break;
        }
    }
}
