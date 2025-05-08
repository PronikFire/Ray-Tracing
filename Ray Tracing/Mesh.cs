using Ray_Tracing.Objects;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;

namespace Ray_Tracing;

public class Mesh
{
    public int[] Tringles => tringles;
    public Vector3[] Vertices => vertices;
 
    private int[] tringles;
    private Vector3[] vertices;

    public void OverrideMesh(Vector3[] vertices, int[] tringles)
    {
        if (tringles.Length % 3 != 0)
            throw new ArgumentException("The size of the tringles array is not divisible by 3.");

        this.tringles = tringles;
        this.vertices = vertices;
    }

    public static Mesh CreateSphere(int latitudeSegments, int longitudeSegments)
    {
        // Предварительный расчет размеров
        int vertexCount = (latitudeSegments + 1) * (longitudeSegments + 1);
        int triangleCount = latitudeSegments * longitudeSegments * 2 * 3; // 2 треугольника на квад, 3 индекса на треугольник

        // Предварительное выделение памяти
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[triangleCount];

        // Предварительный расчет тригонометрических значений для широты
        float[] sinTheta = new float[latitudeSegments + 1];
        float[] cosTheta = new float[latitudeSegments + 1];

        for (int lat = 0; lat <= latitudeSegments; lat++)
        {
            float theta = lat * MathF.PI / latitudeSegments;
            sinTheta[lat] = MathF.Sin(theta);
            cosTheta[lat] = MathF.Cos(theta);
        }

        // Генерация вершин
        int vertexIndex = 0;
        for (int lat = 0; lat <= latitudeSegments; lat++)
        {
            for (int lon = 0; lon <= longitudeSegments; lon++)
            {
                float phi = lon * 2 * MathF.PI / longitudeSegments;
                float sinPhi = MathF.Sin(phi);
                float cosPhi = MathF.Cos(phi);

                vertices[vertexIndex++] = new Vector3(
                    cosPhi * sinTheta[lat],
                    cosTheta[lat],
                    sinPhi * sinTheta[lat]) * 0.5f;
            }
        }

        // Генерация треугольников
        int triangleIndex = 0;
        for (int lat = 0; lat < latitudeSegments; lat++)
        {
            for (int lon = 0; lon < longitudeSegments; lon++)
            {
                int current = lat * (longitudeSegments + 1) + lon;
                int next = current + longitudeSegments + 1;

                // Первый треугольник
                triangles[triangleIndex++] = current;
                triangles[triangleIndex++] = next + 1;
                triangles[triangleIndex++] = next;

                // Второй треугольник
                triangles[triangleIndex++] = current;
                triangles[triangleIndex++] = current + 1;
                triangles[triangleIndex++] = next + 1;
            }
        }

        return new Mesh(vertices, triangles);
    }

    public Mesh(Vector3[] vertices, int[] tringles) => OverrideMesh(vertices, tringles);

    public readonly static Mesh Sphere = CreateSphere(16, 16);

    public readonly static Mesh Plane = new(
        [
        new Vector3(0.5f, 0, 0.5f),
        new Vector3(-0.5f, 0, 0.5f),
        new Vector3(-0.5f, 0, -0.5f),
        new Vector3(0.5f, 0, -0.5f)
        ]
    ,
        [
        1,
        0,
        2,
        0,
        3,
        2
        ]
    );

    public readonly static Mesh Cube = new(
        [
        new Vector3(0.5f, -0.5f, 0.5f),
        new Vector3(-0.5f, -0.5f, 0.5f),
        new Vector3(-0.5f, -0.5f, -0.5f),
        new Vector3(0.5f, -0.5f, -0.5f),

        new Vector3(0.5f, 0.5f, 0.5f),
        new Vector3(-0.5f, 0.5f, 0.5f),
        new Vector3(-0.5f, 0.5f, -0.5f),
        new Vector3(0.5f, 0.5f, -0.5f)
        ]
    ,
        [
        0, 1, 2,
        3, 0, 2,

        0, 4, 5,
        1, 0, 5,

        1, 5, 6,
        2, 1, 6,

        2, 6, 7,
        3, 2, 7,

        3, 7, 4,
        0, 3, 4,

        5, 4, 6,
        4, 7, 6
        ]
    );
}
