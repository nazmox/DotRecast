/*
recast4j Copyright (c) 2015-2019 Piotr Piastucki piotr@jtilia.org

This software is provided 'as-is', without any express or implied
warranty.  In no event will the authors be held liable for any damages
arising from the use of this software.
Permission is granted to anyone to use this software for any purpose,
including commercial applications, and to alter it and redistribute it
freely, subject to the following restrictions:
1. The origin of this software must not be misrepresented; you must not
 claim that you wrote the original software. If you use this software
 in a product, an acknowledgment in the product documentation would be
 appreciated but is not required.
2. Altered source versions must be plainly marked as such, and must not be
 misrepresented as being the original software.
3. This notice may not be removed or altered from any source distribution.
*/

using System.Collections.Generic;
using DotRecast.Core;
using NUnit.Framework;

namespace DotRecast.Detour.Test;

[Parallelizable]
public class GetPolyWallSegmentsTest : AbstractDetourTest
{
    private static readonly RcSegmentVert[][] VERTICES =
    {
        new RcSegmentVert[]
        {
            new(22.084785f, 10.197294f, -48.341274f, 22.684784f, 10.197294f, -44.141273f),
            new(22.684784f, 10.197294f, -44.141273f, 23.884785f, 10.197294f, -48.041275f),
            new(23.884785f, 10.197294f, -48.041275f, 22.084785f, 10.197294f, -48.341274f),
        },
        new RcSegmentVert[]
        {
            new(27.784786f, 10.197294f, 4.158730f, 28.384785f, 10.197294f, 2.358727f),
            new(28.384785f, 10.197294f, 2.358727f, 28.384785f, 10.197294f, -2.141273f),
            new(28.384785f, 10.197294f, -2.141273f, 27.784786f, 10.197294f, -2.741272f),
            new(27.784786f, 10.197294f, -2.741272f, 19.684784f, 10.197294f, -4.241272f),
            new(19.684784f, 10.197294f, -4.241272f, 19.684784f, 10.197294f, 4.158730f),
            new(19.684784f, 10.197294f, 4.158730f, 27.784786f, 10.197294f, 4.158730f),
        },
        new RcSegmentVert[]
        {
            new(22.384785f, 14.997294f, -71.741272f, 19.084785f, 16.597294f, -74.741272f),
            new(19.084785f, 16.597294f, -74.741272f, 18.184784f, 15.997294f, -73.541275f),
            new(18.184784f, 15.997294f, -73.541275f, 17.884785f, 14.997294f, -72.341278f),
            new(17.884785f, 14.997294f, -72.341278f, 17.584785f, 14.997294f, -70.841278f),
            new(17.584785f, 14.997294f, -70.841278f, 22.084785f, 14.997294f, -70.541275f),
            new(22.084785f, 14.997294f, -70.541275f, 22.384785f, 14.997294f, -71.741272f),
        },
        new RcSegmentVert[]
        {
            new(4.684784f, 10.197294f, -6.941269f, 1.984785f, 10.197294f, -8.441269f),
            new(1.984785f, 10.197294f, -8.441269f, -4.015217f, 10.197294f, -6.941269f),
            new(-4.015217f, 10.197294f, -6.941269f, -1.615215f, 10.197294f, -1.541275f),
            new(-1.615215f, 10.197294f, -1.541275f, 1.384785f, 10.197294f, 1.458725f),
            new(1.384785f, 10.197294f, 1.458725f, 7.984783f, 10.197294f, -2.441269f),
            new(7.984783f, 10.197294f, -2.441269f, 4.684784f, 10.197294f, -6.941269f),
        },
        new RcSegmentVert[]
        {
            new(-22.315216f, 6.597294f, -17.141273f, -23.815216f, 5.397294f, -13.841270f),
            new(-23.815216f, 5.397294f, -13.841270f, -24.115217f, 4.997294f, -12.041275f),
            new(-24.115217f, 4.997294f, -12.041275f, -22.315216f, 4.997294f, -11.441269f),
            new(-22.315216f, 4.997294f, -11.441269f, -17.815216f, 5.197294f, -11.441269f),
            new(-17.815216f, 5.197294f, -11.441269f, -22.315216f, 6.597294f, -17.141273f),
        }
    };

    private static readonly long[][] REFS =
    {
        new[] { 281474976710695L, 0L, 0L },
        new[] { 0L, 281474976710770L, 0L, 281474976710769L, 281474976710772L, 0L },
        new[] { 281474976710683L, 281474976710674L, 0L, 281474976710679L, 281474976710684L, 0L },
        new[] { 281474976710750L, 281474976710748L, 0L, 0L, 281474976710755L, 281474976710756L },
        new[] { 0L, 0L, 0L, 281474976710735L, 281474976710736L }
    };

    [Test]
    public void TestFindDistanceToWall()
    {
        var segmentVerts = new List<RcSegmentVert>();
        var segmentRefs = new List<long>();

        IDtQueryFilter filter = new DtQueryDefaultFilter();
        for (int i = 0; i < startRefs.Length; i++)
        {
            var result = query.GetPolyWallSegments(startRefs[i], true, filter, ref segmentVerts, ref segmentRefs);
            Assert.That(segmentVerts.Count, Is.EqualTo(VERTICES[i].Length));
            Assert.That(segmentRefs.Count, Is.EqualTo(REFS[i].Length));
            for (int v = 0; v < VERTICES[i].Length / 6; v++)
            {
                Assert.That(segmentVerts[v].vmin.x, Is.EqualTo(VERTICES[i][v].vmin.x).Within(0.001f));
                Assert.That(segmentVerts[v].vmin.y, Is.EqualTo(VERTICES[i][v].vmin.y).Within(0.001f));
                Assert.That(segmentVerts[v].vmin.z, Is.EqualTo(VERTICES[i][v].vmin.z).Within(0.001f));
                Assert.That(segmentVerts[v].vmax.x, Is.EqualTo(VERTICES[i][v].vmax.x).Within(0.001f));
                Assert.That(segmentVerts[v].vmax.y, Is.EqualTo(VERTICES[i][v].vmax.y).Within(0.001f));
                Assert.That(segmentVerts[v].vmax.z, Is.EqualTo(VERTICES[i][v].vmax.z).Within(0.001f));
            }

            for (int v = 0; v < REFS[i].Length; v++)
            {
                Assert.That(segmentRefs[v], Is.EqualTo(REFS[i][v]));
            }
        }
    }
}