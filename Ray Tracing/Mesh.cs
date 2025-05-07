using Ray_Tracing.Objects;
using System;
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

    public Mesh(Vector3[] vertices, int[] tringles) => OverrideMesh(vertices, tringles);

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
