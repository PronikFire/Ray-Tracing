﻿using System.Numerics;

namespace Ray_Tracing.Objects;

public class MeshRender : Object
{
    public Material material = new();

    public Mesh? mesh = null;

    public bool Intersection(Vector3 origin, Vector3 direction, out IntersectionResult result)
    {
        result = default;

        if (mesh is null)
            return false;

        direction = Vector3.Normalize(direction);

        bool hasResult = false;

        for (int i = 0; i < mesh.Tringles.Length; i += 3)
        {
            Vector3 v0 = mesh.Vertices[mesh.Tringles[i]] * transform.scale;
            Vector3 v1 = mesh.Vertices[mesh.Tringles[i + 1]] * transform.scale;
            Vector3 v2 = mesh.Vertices[mesh.Tringles[i + 2]] * transform.scale;

            Vector3 e1 = v1 - v0;
            Vector3 e2 = v2 - v0;

            Vector3 pvec = Vector3.Cross(direction, e2);
            float det = Vector3.Dot(e1, pvec);

            if (det < 1e-8 && det > -1e-8)
                continue;

            float inv_det = 1 / det;
            Vector3 tvec = origin - transform.position - v0;
            float u = Vector3.Dot(tvec, pvec) * inv_det;
            if (u < 0 || u > 1)
                continue;

            Vector3 qvec = Vector3.Cross(tvec, e1);
            float v = Vector3.Dot(direction, qvec) * inv_det;
            if (v < 0 || u + v > 1)
                continue;

            float t = Vector3.Dot(e2, qvec) * inv_det;

            if (t < 0)
                continue;

            if (hasResult && result.distance < t)
                continue;

            result.point = origin + t * direction;
            result.normal = Vector3.Normalize(Vector3.Cross(e1, e2));
            result.distance = t;
            hasResult = true;
        }

        return hasResult;
    }

    public struct IntersectionResult
    {
        public Vector3 point;
        public Vector3 normal;
        public float distance;
    }
}
