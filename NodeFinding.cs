using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeFinding : MonoBehaviour
{
    // Color of line that connect the nodes
    public Color lineColor;

    // This list will be used, to load up all the node position
    private List<Transform> nodes = new List<Transform>();

    void OnDrawGizmos() 
    {
        // Setting up the Gizmos color
        Gizmos.color = lineColor;

        // Gets all child position
        Transform[] pathTransform = GetComponentsInChildren<Transform>();
        nodes = new List<Transform>();

        // Looping through the list
        for (int i = 0; i < pathTransform.Length; i++) 
        {
            if (pathTransform[i] != transform)
            {
                nodes.Add(pathTransform[i]);
            }
        }

        // Looping through Array (2) for connecting the nodes
        for (int i = 0; i < nodes.Count; i++)
        {
            Vector3 currentNode = nodes[i].position;
            Vector3 previousNodes = Vector3.zero;
            if (i > 0)
            {
                previousNodes = nodes[i - 1].position;
            }
            else if (i == 0 && nodes.Count > 1)
            {
                previousNodes = nodes[nodes.Count - 1].position;
            }

            // Displaying the line that connects each nodes, and create a sphere form in nodes
            Gizmos.DrawLine(previousNodes, currentNode);
            Gizmos.DrawWireSphere(currentNode, 0.5f);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
