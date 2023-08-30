using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshCutter : MonoBehaviour
{
    
    void Start()
    {
        // Create a new Mesh
        Mesh mesh = new Mesh();

        // Define the vertices of the cube, but cutting one corner
        Vector3[] vertices = {
            new Vector3(0, 0, 0),
            new Vector3(1, 0, 0),
            new Vector3(1, 1, 0),
            new Vector3(0.5f, 1, 0), // Cut corner vertex
            new Vector3(0, 0, 1),
            new Vector3(1, 0, 1),
            new Vector3(1, 1, 1),
            new Vector3(0.5f, 1, 1)  // Cut corner vertex
        };
        mesh.vertices = vertices;

        // Define the triangles that form the faces of the cube
        int[] triangles = {
            0, 2, 1, 0, 3, 2,
            4, 5, 6, 4, 6, 7,
            0, 1, 5, 0, 5, 4,
            1, 2, 6, 1, 6, 5,
            2, 3, 7, 2, 7, 6,
            3, 0, 4, 3, 4, 7
        };
        mesh.triangles = triangles;

        // Create the mesh and set it to the MeshFilter component
        GetComponent<MeshFilter>().mesh = mesh;
    }
}
