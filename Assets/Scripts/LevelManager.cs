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
    [SerializeField] private Color RedColor,BlueColor;
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
            OpenOrCloseDoor(listFirstClosedDoors[0], "Open", true);
            OpenOrCloseDoor(listFirstClosedDoors[1], "Open", true);
        }

        //Second Puzzle
        secondSuccessful = pressurePlate.GetIsActive();
        if (secondSuccessful)
        {
            foreach (var item in listLightsSecondPuzzle)
            {
                item.color = BlueColor;
            }
            OpenOrCloseDoor(listSecondClosedDoor[0], "Close", false);
            OpenOrCloseDoor(listSecondClosedDoor[1], "Close", false);
            OpenOrCloseDoor(listSecondClosedDoor[0], "Open", true);
            OpenOrCloseDoor(listSecondClosedDoor[1], "Open", true);
        }
        else
        {
            foreach (var item in listLightsSecondPuzzle)
            {
                item.color = RedColor;
            }
            OpenOrCloseDoor(listSecondClosedDoor[0], "Close", true);
            OpenOrCloseDoor(listSecondClosedDoor[1], "Close", true);
        }
    }

    private void OpenOrCloseDoor(GameObject door, string animation, bool State)
    {
        door.GetComponent<Animator>().SetBool(animation, State);
    }
}
