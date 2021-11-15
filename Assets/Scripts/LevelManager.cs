using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [Header("First Puzzle")]
    [SerializeField] private List<Light> listLightsFirstPuzzle;
    [SerializeField] private bool firstSuccessful;
    [SerializeField] private List<GameObject> listFirstClosedDoors;

    [Header("Second Puzzle")]
    [SerializeField] private List<Light> listLightsSecondPuzzle;
    [SerializeField] private PressurePlateController pressurePlate;
    [SerializeField] private bool secondSuccessful;
    [SerializeField] private List<GameObject> listSecondClosedDoor;
    private bool avaible = true;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //First Puzzle
        if (listLightsFirstPuzzle.TrueForAll(c => c.gameObject.activeSelf == true) && !firstSuccessful)
        {
            firstSuccessful = true;
            OpenOrCloseDoor(listFirstClosedDoors[0], "Open");
            OpenOrCloseDoor(listFirstClosedDoors[1], "Open");
        }

        //Second Puzzle
        secondSuccessful = pressurePlate.GetIsActive();
        if (secondSuccessful)
        {
            if (avaible)
            {
                foreach (var item in listLightsSecondPuzzle)
                {
                    item.color = new Color(144f, 191f, 255f, 255f);
                }
                OpenOrCloseDoor(listSecondClosedDoor[0], "Open");
                OpenOrCloseDoor(listSecondClosedDoor[1], "Open");
                avaible = false;
            }
        }
        else
        {
            if (avaible)
            {
                foreach (var item in listLightsSecondPuzzle)
                {
                    item.color = new Color(255f, 60f, 54f, 255f);
                }
                OpenOrCloseDoor(listSecondClosedDoor[0], "Close");
                OpenOrCloseDoor(listSecondClosedDoor[1], "Close");
                avaible = false;
            }
        }
    }

    private void OpenOrCloseDoor(GameObject door, string animation)
    {
        door.GetComponent<Animator>().SetBool(animation, true);
    }
}
