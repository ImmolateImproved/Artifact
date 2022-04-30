using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public struct Face
{
    public List<Vector3> Vertices { get; private set; }
    public List<int> Triangles { get; private set; }
    public List<Vector2> Uvs { get; private set; }

    public Face(List<Vector3> vertices, List<int> triangles, List<Vector2> uvs)
    {
        Vertices = vertices;
        Triangles = triangles;
        Uvs = uvs;
    }
}

public class HexTileGenerator : MonoBehaviour
{
    public float innerRadius;
    public float outerRadius;
    public float height;
    public bool isFlatTopped;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            var mesh = CreateMesh();
            AssetDatabase.CreateAsset(mesh, "Assets/HexMesh.asset");
            AssetDatabase.SaveAssets();
        }
    }

    public Mesh CreateMesh()
    {
        var faces = CreateFaces();

        return CombineFaces(faces);
    }

    private List<Face> CreateFaces()
    {
        var faces = new List<Face>();

        for (int point = 0; point < 6; point++)
        {
            faces.Add(CreateFace(innerRadius, outerRadius, height / 2, height / 2, point));
        }

        for (int point = 0; point < 6; point++)
        {
            faces.Add(CreateFace(innerRadius, outerRadius, -height / 2, -height / 2, point, true));
        }

        for (int point = 0; point < 6; point++)
        {
            faces.Add(CreateFace(outerRadius, outerRadius, height / 2, -height / 2, point, true));
        }

        for (int point = 0; point < 6; point++)
        {
            faces.Add(CreateFace(innerRadius, innerRadius, height / 2, -height / 2, point, false));
        }

        return faces;
    }

    private Mesh CombineFaces(List<Face> faces)
    {
        var vertices = new List<Vector3>();
        var triangles = new List<int>();
        var uvs = new List<Vector2>();

        for (int i = 0; i < faces.Count; i++)
        {
            vertices.AddRange(faces[i].Vertices);
            uvs.AddRange(faces[i].Uvs);

            var offset = 4 * i;

            foreach (var triangle in faces[i].Triangles)
            {
                triangles.Add(triangle + offset);
            }
        }

        var mesh = new Mesh
        {
            vertices = vertices.ToArray(),
            triangles = triangles.ToArray(),
            uv = uvs.ToArray()
        };

        mesh.RecalculateNormals();

        return mesh;
    }

    private Face CreateFace(float innerRadius, float outerRadius, float heightA, float heightB, int point, bool reverse = false)
    {
        var pointA = GetPoint(innerRadius, heightB, point);
        var pointB = GetPoint(innerRadius, heightB, (point < 5) ? point + 1 : 0);
        var pointC = GetPoint(outerRadius, heightA, (point < 5) ? point + 1 : 0);
        var pointD = GetPoint(outerRadius, heightA, point);

        var vertices = new List<Vector3>() { pointA, pointB, pointC, pointD };
        var triangles = new List<int>() { 0, 1, 2, 2, 3, 0 };
        var uvs = new List<Vector2>()
        {
            new Vector2(0,0),
            new Vector2(1,0),
            new Vector2(1,1),
            new Vector2(0,1),
        };

        if (reverse)
        {
            vertices.Reverse();
        }

        return new Face(vertices, triangles, uvs);
    }

    private Vector3 GetPoint(float size, float height, int index)
    {
        var angleDeg = isFlatTopped ? 60 * index : 60 * index - 30;

        var angleRad = angleDeg * Mathf.Deg2Rad;

        return new Vector3(size * Mathf.Cos(angleRad), height, size * Mathf.Sin(angleRad));
    }
}