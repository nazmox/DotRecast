using DotRecast.Core;
using DotRecast.Recast.Demo.Draw;
using static DotRecast.Core.RcMath;
using static DotRecast.Recast.Demo.Tools.Gizmos.GizmoHelper;

namespace DotRecast.Recast.Demo.Tools.Gizmos;

public class CapsuleGizmo : IColliderGizmo
{
    private readonly float[] vertices;
    private readonly int[] triangles;
    private readonly float[] center;
    private readonly float[] gradient;

    public CapsuleGizmo(RcVec3f start, RcVec3f end, float radius)
    {
        center = new float[]
        {
            0.5f * (start.x + end.x), 0.5f * (start.y + end.y),
            0.5f * (start.z + end.z)
        };
        RcVec3f axis = RcVec3f.Of(end.x - start.x, end.y - start.y, end.z - start.z);
        RcVec3f[] normals = new RcVec3f[3];
        normals[1] = RcVec3f.Of(end.x - start.x, end.y - start.y, end.z - start.z);
        RcVec3f.Normalize(ref normals[1]);
        normals[0] = GetSideVector(axis);
        normals[2] = RcVec3f.Zero;
        RcVec3f.Cross(ref normals[2], normals[0], normals[1]);
        RcVec3f.Normalize(ref normals[2]);
        triangles = GenerateSphericalTriangles();
        var trX = RcVec3f.Of(normals[0].x, normals[1].x, normals[2].x);
        var trY = RcVec3f.Of(normals[0].y, normals[1].y, normals[2].y);
        var trZ = RcVec3f.Of(normals[0].z, normals[1].z, normals[2].z);
        float[] spVertices = GenerateSphericalVertices();
        float halfLength = 0.5f * axis.Length();
        vertices = new float[spVertices.Length];
        gradient = new float[spVertices.Length / 3];
        RcVec3f v = new RcVec3f();
        for (int i = 0; i < spVertices.Length; i += 3)
        {
            float offset = (i >= spVertices.Length / 2) ? -halfLength : halfLength;
            float x = radius * spVertices[i];
            float y = radius * spVertices[i + 1] + offset;
            float z = radius * spVertices[i + 2];
            vertices[i] = x * trX.x + y * trX.y + z * trX.z + center[0];
            vertices[i + 1] = x * trY.x + y * trY.y + z * trY.z + center[1];
            vertices[i + 2] = x * trZ.x + y * trZ.y + z * trZ.z + center[2];
            v.x = vertices[i] - center[0];
            v.y = vertices[i + 1] - center[1];
            v.z = vertices[i + 2] - center[2];
            RcVec3f.Normalize(ref v);
            gradient[i / 3] = Clamp(0.57735026f * (v.x + v.y + v.z), -1, 1);
        }
    }

    private RcVec3f GetSideVector(RcVec3f axis)
    {
        RcVec3f side = RcVec3f.Of(1, 0, 0);
        if (axis.x > 0.8)
        {
            side = RcVec3f.Of(0, 0, 1);
        }

        RcVec3f forward = new RcVec3f();
        RcVec3f.Cross(ref forward, side, axis);
        RcVec3f.Cross(ref side, axis, forward);
        RcVec3f.Normalize(ref side);
        return side;
    }

    public void Render(RecastDebugDraw debugDraw)
    {
        debugDraw.Begin(DebugDrawPrimitives.TRIS);
        for (int i = 0; i < triangles.Length; i += 3)
        {
            for (int j = 0; j < 3; j++)
            {
                int v = triangles[i + j] * 3;
                float c = gradient[triangles[i + j]];
                int col = DebugDraw.DuLerpCol(DebugDraw.DuRGBA(32, 32, 0, 160), DebugDraw.DuRGBA(220, 220, 0, 160),
                    (int)(127 * (1 + c)));
                debugDraw.Vertex(vertices[v], vertices[v + 1], vertices[v + 2], col);
            }
        }

        debugDraw.End();
    }
}