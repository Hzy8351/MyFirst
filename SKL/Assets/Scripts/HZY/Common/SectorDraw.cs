using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public class SectorDraw : MonoBehaviour
{
    public float Radius = 6;          //��뾶  
    public float innerRadius = 3;     //�ڰ뾶
    public float angleDegree = 360;   //���λ�����ĽǶ�
    public int Segments = 60;         //�ָ���  
    private MeshFilter meshFilter;

    private void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = CreateMesh(Radius, innerRadius, angleDegree, Segments);
    }

    #region  CreateMesh(float radius, float innerradius, float angledegree, int segments)
    private Mesh CreateMesh(float radius, float innerradius, float angledegree, int segments)
    {
        //vertices(����):
        int vertices_count = segments * 2 + 2;              //��Ϊvertices(����)�ĸ�����triangles�����������ζ�����������ƥ��
        Vector3[] vertices = new Vector3[vertices_count];
        float angleRad = Mathf.Deg2Rad * angledegree;
        float angleCur = angleRad;
        float angledelta = angleRad / segments;
        for (int i = 0; i < vertices_count; i += 2) 
        {
            float cosA = Mathf.Cos(angleCur);
            float sinA = Mathf.Sin(angleCur);
            vertices[i] = new Vector3(radius * cosA, 0, radius * sinA);
            vertices[i + 1] = new Vector3(innerradius * cosA, 0, innerradius * sinA);
            angleCur -= angledelta;
        }
        //triangles:
        int triangle_count = segments * 6;
        int[] triangles = new int[triangle_count];
        for (int i = 0, vi = 0; i < triangle_count; i += 6, vi += 2)
        {
            triangles[i] = vi;
            triangles[i + 1] = vi + 3;
            triangles[i + 2] = vi + 1;
            triangles[i + 3] = vi + 2;
            triangles[i + 4] = vi + 3;
            triangles[i + 5] = vi;
        }
        //uv:
        Vector2[] uvs = new Vector2[vertices_count];
        for (int i = 0; i < vertices_count; i++)
        {
            uvs[i] = new Vector2(vertices[i].x / radius / 2 + 0.5f, vertices[i].z / radius / 2 + 0.5f);
        }
        //����������mesh
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        return mesh;
    }
    #endregion

}
