using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingInteractivePower : MonoBehaviour
{
    [Header("Power")]
    [SerializeField] private Powers Power;

    [Header("Inputs")]
    [SerializeField] private float degreesPerSecond = 15.0f;
    [SerializeField] private float amplitude = 0.5f;
    [SerializeField] private float frequency = 1f;

    // Position Storage Variables
    private Vector3 posOffset = new Vector3();
    private Vector3 tempPos = new Vector3();
    // Start is called before the first frame update
    void Start()
    {
        // Store the starting position & rotation of the object
        posOffset = transform.position;
    }

    void FixedUpdate()
    {
        // Spin object around Y-Axis
        transform.Rotate(new Vector3(0f, Time.deltaTime * degreesPerSecond, 0f), Space.World);

        // Float up/down with a Sin()
        tempPos = posOffset;
        tempPos.y += Mathf.Sin(Time.fixedTime * Mathf.PI * frequency) * amplitude;

        transform.position = tempPos;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            int aux = 0;
            switch (Power)
            {
                case (Powers.BoolPower):
                    aux = 0;
                    break;
                case (Powers.IntPower):
                    aux = 1;
                    break;
                case (Powers.FloatPower):
                    aux = 2;
                    break;
                case (Powers.StringPower):
                    aux = 3;
                    break;
            }
            other.GetComponent<ThidPersonMovementController>().SetVariablePower(aux, true);
            Destroy(gameObject);
        }
    }
}
