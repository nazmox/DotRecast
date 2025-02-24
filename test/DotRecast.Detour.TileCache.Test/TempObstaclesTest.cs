/*
Copyright (c) 2009-2010 Mikko Mononen memon@inside.org
recast4j copyright (c) 2015-2019 Piotr Piastucki piotr@jtilia.org
DotRecast Copyright (c) 2023 Choi Ikpil ikpil@naver.com

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
using DotRecast.Detour.TileCache.Test.Io;
using DotRecast.Recast;
using DotRecast.Recast.Geom;
using NUnit.Framework;

namespace DotRecast.Detour.TileCache.Test;

[Parallelizable]
public class TempObstaclesTest : AbstractTileCacheTest
{
    [Test]
    public void TestDungeon()
    {
        bool cCompatibility = true;
        IInputGeomProvider geom = ObjImporter.Load(Loader.ToBytes("dungeon.obj"));
        TestTileLayerBuilder layerBuilder = new TestTileLayerBuilder(geom);
        List<byte[]> layers = layerBuilder.Build(RcByteOrder.LITTLE_ENDIAN, cCompatibility, 1);
        DtTileCache tc = GetTileCache(geom, RcByteOrder.LITTLE_ENDIAN, cCompatibility);
        foreach (byte[] data in layers)
        {
            long refs = tc.AddTile(data, 0);
            tc.BuildNavMeshTile(refs);
        }

        List<DtMeshTile> tiles = tc.GetNavMesh().GetTilesAt(1, 4);
        DtMeshTile tile = tiles[0];
        Assert.That(tile.data.header.vertCount, Is.EqualTo(16));
        Assert.That(tile.data.header.polyCount, Is.EqualTo(6));
        long o = tc.AddObstacle(RcVec3f.Of(-1.815208f, 9.998184f, -20.307983f), 1f, 2f);
        bool upToDate = tc.Update();
        Assert.That(upToDate, Is.True);
        tiles = tc.GetNavMesh().GetTilesAt(1, 4);
        tile = tiles[0];
        Assert.That(tile.data.header.vertCount, Is.EqualTo(22));
        Assert.That(tile.data.header.polyCount, Is.EqualTo(11));
        tc.RemoveObstacle(o);
        upToDate = tc.Update();
        Assert.That(upToDate, Is.True);
        tiles = tc.GetNavMesh().GetTilesAt(1, 4);
        tile = tiles[0];
        Assert.That(tile.data.header.vertCount, Is.EqualTo(16));
        Assert.That(tile.data.header.polyCount, Is.EqualTo(6));
    }

    [Test]
    public void TestDungeonBox()
    {
        bool cCompatibility = true;
        IInputGeomProvider geom = ObjImporter.Load(Loader.ToBytes("dungeon.obj"));
        TestTileLayerBuilder layerBuilder = new TestTileLayerBuilder(geom);
        List<byte[]> layers = layerBuilder.Build(RcByteOrder.LITTLE_ENDIAN, cCompatibility, 1);
        DtTileCache tc = GetTileCache(geom, RcByteOrder.LITTLE_ENDIAN, cCompatibility);
        foreach (byte[] data in layers)
        {
            long refs = tc.AddTile(data, 0);
            tc.BuildNavMeshTile(refs);
        }

        List<DtMeshTile> tiles = tc.GetNavMesh().GetTilesAt(1, 4);
        DtMeshTile tile = tiles[0];
        Assert.That(tile.data.header.vertCount, Is.EqualTo(16));
        Assert.That(tile.data.header.polyCount, Is.EqualTo(6));
        long o = tc.AddBoxObstacle(
            RcVec3f.Of(-2.315208f, 9.998184f, -20.807983f),
            RcVec3f.Of(-1.315208f, 11.998184f, -19.807983f)
        );
        bool upToDate = tc.Update();
        Assert.That(upToDate, Is.True);
        tiles = tc.GetNavMesh().GetTilesAt(1, 4);
        tile = tiles[0];
        Assert.That(tile.data.header.vertCount, Is.EqualTo(22));
        Assert.That(tile.data.header.polyCount, Is.EqualTo(11));
        tc.RemoveObstacle(o);
        upToDate = tc.Update();
        Assert.That(upToDate, Is.True);
        tiles = tc.GetNavMesh().GetTilesAt(1, 4);
        tile = tiles[0];
        Assert.That(tile.data.header.vertCount, Is.EqualTo(16));
        Assert.That(tile.data.header.polyCount, Is.EqualTo(6));
    }
}