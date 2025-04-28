using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Ray_Tracing.Objects;

public class MeshRender(Mesh mesh) : Object
{
    public Material Material = new();

    public Mesh mesh = mesh;

    public bool Intersection(Vector3 origin, Vector3 direction, out IntersectionResult result)
    {
        result = default;

        direction = Vector3.Normalize(direction);

        bool firstWasFind = false;

        for (int i = 0; i < mesh.Tringles.Length; i += 3)
        {
            Vector3 v0 = mesh.Vertices[mesh.Tringles[i]] * transform.Scale;
            Vector3 v1 = mesh.Vertices[mesh.Tringles[i + 1]] * transform.Scale;
            Vector3 v2 = mesh.Vertices[mesh.Tringles[i + 2]] * transform.Scale;

            Vector3 e1 = v1 - v0;
            Vector3 e2 = v2 - v0;

            Vector3 pvec = Vector3.Cross(direction, e2);
            float det = Vector3.Dot(e1, pvec);

            if (det < 1e-8 && det > -1e-8)
                continue;

            float inv_det = 1 / det;
            Vector3 tvec = origin - transform.Position - v0;
            float u = Vector3.Dot(tvec, pvec) * inv_det;
            if (u < 0 || u > 1)
                continue;

            Vector3 qvec = Vector3.Cross(tvec, e1);
            float v = Vector3.Dot(direction, qvec) * inv_det;
            if (v < 0 || u + v > 1)
                continue;

            float t = Vector3.Dot(e2, qvec) * inv_det;

            if (firstWasFind && Vector3.Distance(origin, result.point) < t)
                continue;

            result.point = origin + t * direction;
            result.normal = Vector3.Normalize(Vector3.Cross(e1, e2));
            firstWasFind = true;
        }

        return firstWasFind;
    }

    public struct IntersectionResult
    {
        public Vector3 point;
        public Vector3 normal;
    }
}
