using Ray_Tracing.Objects;
using System.Collections.ObjectModel;
using System;
using System.Diagnostics.Contracts;
using System.Collections.Generic;
using System.Numerics;
using System.Linq;

namespace Ray_Tracing;

public class Scene
{
    public List<Object> Objects = [];

    public bool Raycast(Vector3 origin, Vector3 direction, out RaycastResult raycastResult, MeshRender[]? excludedObjects = null)
    {
        raycastResult = default;

        bool firstWasFind = false;

        foreach (Object obj in Objects)
        {
            if (obj is not MeshRender meshRender)
                continue;

            if (excludedObjects is not null && excludedObjects.Contains(meshRender))
                continue;

            if (!meshRender.Intersection(origin, direction, out var result))
                continue;

            if (firstWasFind && Vector3.Distance(origin, raycastResult.point) < Vector3.Distance(origin, result.point))
                continue;

            raycastResult.point = result.point;
            raycastResult.normal = result.normal;
            raycastResult.meshRender = meshRender;
            firstWasFind = true;
        }

        return firstWasFind;
    }

    public struct RaycastResult
    {
        public Vector3 point;
        public Vector3 normal;
        public MeshRender meshRender;
    }
}
