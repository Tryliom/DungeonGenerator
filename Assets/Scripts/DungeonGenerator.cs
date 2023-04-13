using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

public enum DungeonGeneratorType
{
    Empty,
    DrunkardWalk
}

public class DungeonGenerator : MonoBehaviour
{
    [SerializeField] private Tilemap _wallTilemap;
    [SerializeField] private Tilemap _groundTilemap;
    [SerializeField] private RuleTile _wallTile;
    [SerializeField] private RuleTile _groundTile;

    [Header("Dungeon Generation")] 
    [SerializeField] [Range(1, 250)] private int _width;
    [SerializeField] [Range(1, 250)] private int _height;

    [Header("Other Generators")]
    [SerializeField] private DrunkardWalkGenerator _drunkardWalkGeneratorGround;
    [SerializeField] private DrunkardWalkGenerator _drunkardWalkGeneratorCorridors;
    
    private readonly HashSet<Vector3Int> _groundPositions = new HashSet<Vector3Int>();
    private readonly HashSet<Vector3Int> _wallPositions = new HashSet<Vector3Int>();

    public void GenerateDungeon(DungeonGeneratorType type)
    {
        _wallTilemap.ClearAllTiles();
        _groundTilemap.ClearAllTiles();
        
        _groundPositions.Clear();
        _wallPositions.Clear();
        
        switch (type)
        {
            case DungeonGeneratorType.Empty:
                GenerateDungeonNormal();
                break;
            case DungeonGeneratorType.DrunkardWalk:
                _drunkardWalkGeneratorCorridors.AddGroundPositions(_groundPositions);
                _drunkardWalkGeneratorCorridors.AddWallPositions(_wallPositions);

                var crossRoads = _drunkardWalkGeneratorCorridors.GetCrossRoadsPositions();
                
                foreach (var crossRoad in crossRoads)
                {
                    _drunkardWalkGeneratorGround.AddGroundPositions(_groundPositions, crossRoad);
                    _drunkardWalkGeneratorGround.AddWallPositions(_wallPositions);
                }

                // Remove tiles that are both ground and wall from wall
                foreach (var position in _groundPositions)
                {
                    _wallPositions.Remove(position);
                }
                
                // Paint tiles
                foreach (var position in _groundPositions)
                {
                    PaintGroundTile(position);
                }
                
                foreach (var position in _wallPositions)
                {
                    PaintWallTile(position);
                }
                
                break;
        }
    }

    private void GenerateDungeonNormal()
    {
        for (int x = -_width / 2; x < _width / 2; x++)
        {
            for (int y = -_height / 2; y < _height / 2; y++)
            {
                PaintGroundTile(new Vector3Int(x, y, 0));
            }
        }
    }

    public void PaintGroundTile(Vector3Int position)
    {
        _groundTilemap.SetTile(position, _groundTile);
    }
    
    public void PaintWallTile(Vector3Int position)
    {
        _wallTilemap.SetTile(position, _wallTile);
    }
}
