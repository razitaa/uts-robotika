using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCar : MonoBehaviour
{
    public Vector3 com;
    public Rigidbody rb;

    public float speed = 1.0f;
    public Transform path;

    private List<Transform> nodes;
    private int currentNode = 0;

    public WheelCollider LeftWheel;
    public WheelCollider RightWheel;
    public WheelCollider LeftRearWheel;
    public WheelCollider RightRearWheel;

    public float maxSteerAngle = 45f;

    public bool isBraking = false;

    public float sensorLength = 10f;
    public Vector3 frontSensorPosition = new Vector3(0f, 0.2f, 0.5f);
    public float frontSideSensorPosition = 0.2f;
    public float frontSensorAngle = 30f;

    private bool isThereObstacle = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = com;

        Transform[] pathTransforms = path.GetComponentsInChildren<Transform>();

        nodes = new List<Transform>();

        for (int i = 0; i < pathTransforms.Length; i++)
        {
            if (pathTransforms[i] != path.transform)
            {
                nodes.Add(pathTransforms[i]);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        Sensors();
        RunSteer();
        CheckWaypointDistance();
        Braking();
        MoveWheels();
    }

    private void MoveWheels()
    {
        if (!isBraking)
        {
            LeftWheel.motorTorque = 300f;
            RightWheel.motorTorque = 300f;
        }
        else
        {
            LeftWheel.motorTorque = 0;
            RightWheel.motorTorque = 0;
        }
    }

    private void RunSteer()
    {
        if (isThereObstacle) return;
        Vector3 relativeVector = transform.InverseTransformPoint(nodes[currentNode].position);
        float newSteer = (relativeVector.x / relativeVector.magnitude) * maxSteerAngle;
        
        LeftWheel.steerAngle = newSteer;
        RightWheel.steerAngle = newSteer;
    }

    // Will be used to determine car position
    private void CheckWaypointDistance()
    {
        if (Vector3.Distance(transform.position, nodes[currentNode].position) < 9f)
        {
            if (currentNode == nodes.Count - 1)
            {
                currentNode = 0;
            }
            else{
                currentNode++;
            }
        }

        if (Vector3.Distance(transform.position, nodes[currentNode].position) < 6f)
        {
            isBraking = true;
        }

        if (Vector3.Distance(transform.position, nodes[currentNode].position) < 0.5f)
        {
            isBraking = false;
        }
    }

    private void Sensors()
    {
        RaycastHit hit;
        Vector3 sensorStartPosition = transform.position;
        sensorStartPosition += transform.forward * frontSensorPosition.z;
        sensorStartPosition += transform.up * frontSensorPosition.y;
        float obstacleAvoidanceMultiplier = 0;
        isThereObstacle = false;

        sensorStartPosition += transform.right * frontSideSensorPosition;
        if (Physics.Raycast(sensorStartPosition, transform.forward, out hit, sensorLength))
        {
            if (hit.collider.CompareTag("Terrain"))
            {
                Debug.DrawLine(sensorStartPosition, hit.point, Color.red);
                isThereObstacle = true;
                obstacleAvoidanceMultiplier -= 1f;
            }
        }

        else if (Physics.Raycast(sensorStartPosition, Quaternion.AngleAxis(frontSensorAngle, transform.up) * transform.forward, out hit, sensorLength))
        {
            if (hit.collider.CompareTag("Terrain"))
            {
                Debug.DrawLine(sensorStartPosition, hit.point, Color.blue);
                isThereObstacle = true;
                obstacleAvoidanceMultiplier -= 0.5f;
            }
        }

        sensorStartPosition -= transform.right * 2 * frontSideSensorPosition;
        if (Physics.Raycast(sensorStartPosition, transform.forward, out hit, sensorLength))
        {
            if (hit.collider.CompareTag("Terrain"))
            {
                Debug.DrawLine(sensorStartPosition, hit.point, Color.red);
                isThereObstacle = true;
                obstacleAvoidanceMultiplier += 1f;
            }
        }

        else if (Physics.Raycast(sensorStartPosition, Quaternion.AngleAxis(-frontSensorAngle, transform.up) * transform.forward, out hit, sensorLength))
        {
            if (hit.collider.CompareTag("Terrain"))
            {
                Debug.DrawLine(sensorStartPosition, hit.point, Color.blue);
                isThereObstacle = true;
                obstacleAvoidanceMultiplier += 0.5f;
            }
        }

        if (Physics.Raycast(sensorStartPosition, transform.forward, out hit, sensorLength))
        {
            if (obstacleAvoidanceMultiplier == 0) 
            {
                if (hit.collider.CompareTag("Terrain"))
                {
                    Debug.DrawLine(sensorStartPosition, hit.point, Color.yellow);
                    isThereObstacle = true;
                    if (hit.normal.x < 0)
                    {
                        obstacleAvoidanceMultiplier = 1f;
                    }   
                    else
                    {
                        obstacleAvoidanceMultiplier = -0.8f;
                    }
                }
            }
        }

        if (isThereObstacle) {
            LeftWheel.steerAngle = maxSteerAngle * obstacleAvoidanceMultiplier;
            RightWheel.steerAngle = maxSteerAngle * obstacleAvoidanceMultiplier;
        }
    }

    private void Braking()
    {
        if (isBraking)
        {
            LeftRearWheel.brakeTorque = 600f;
            RightRearWheel.brakeTorque = 600f;
        }
        else
        {
            LeftRearWheel.brakeTorque = 0;
            RightRearWheel.brakeTorque = 0;
        }
    }
}
