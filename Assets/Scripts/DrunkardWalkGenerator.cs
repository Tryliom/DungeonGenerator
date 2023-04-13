using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;

public class DrunkardWalkGenerator : MonoBehaviour
{
    [Header("Drunkard Walk")]
    [SerializeField][Range(1, 250)] private int _dWTilesNeeded;
    [SerializeField][Range(1, 99)] private int _dWMinStepSize;
    [SerializeField][Range(2, 100)] private int _dWMaxStepSize;
    [SerializeField][Range(0, 15)] private int _dWUExpandSize;
    [SerializeField] private bool _dWRandomizeStepSize;
    [SerializeField] private bool _dWUseFullNeighbors;

    private readonly HashSet<Vector3Int> _groundPositions = new HashSet<Vector3Int>();
    private readonly HashSet<Vector3Int> _wallPositions = new HashSet<Vector3Int>();
    private readonly HashSet<Vector3Int> _crossRoadsPositions = new HashSet<Vector3Int>();

    public void AddGroundPositions(HashSet<Vector3Int> positions, Vector3Int startPosition = default)
    {
        _groundPositions.Clear();
        _crossRoadsPositions.Clear();
        
        GenerateGroundPosition(startPosition);
        
        foreach (var position in _groundPositions)
        {
            positions.Add(position);
        }
    }
    
    public void AddWallPositions(HashSet<Vector3Int> positions)
    {
        _wallPositions.Clear();
        
        GenerateWallPositions();
        
        foreach (var position in _wallPositions)
        {
            positions.Add(position);
        }
    }

    private void GenerateGroundPosition(Vector3Int startPosition = default)
    {
        var neighborPositions = _dWUseFullNeighbors ? Utility.FullNeighborPositions : Utility.NeighborPositions;

        // Start drunkard walk at the center of the dungeon
        var drunkardWalkPosition = startPosition == default ? Vector3Int.zero : startPosition;
        
        // Add the starting position to the list of drunkard walk positions
        _groundPositions.Add(drunkardWalkPosition);
        _crossRoadsPositions.Add(drunkardWalkPosition);

        // Iterate the drunkard walk
        while (_groundPositions.Count < _dWTilesNeeded)
        {
            // Get a random direction
            var randomDirection = Random.Range(0, neighborPositions.Count);
            
            // Get a random step size
            var stepSize = _dWRandomizeStepSize ? Random.Range(_dWMinStepSize, _dWMaxStepSize) : _dWMaxStepSize;
            
            for (var i = 0; i < stepSize; i++)
            {
                // Get the next position
                drunkardWalkPosition += neighborPositions[randomDirection];

                _groundPositions.Add(drunkardWalkPosition);

                for (var j = 0; j < _dWUExpandSize; j++)
                {
                    // Add all neighbors to the list of drunkard walk positions
                    foreach (var neighborPosition in neighborPositions)
                    {
                        _groundPositions.Add(drunkardWalkPosition + neighborPosition * (j + 1));
                    }
                }
            }
            
            _crossRoadsPositions.Add(drunkardWalkPosition);
        }
    }

    private void GenerateWallPositions()
    {
        foreach (var groundPosition in _groundPositions)
        {
            foreach (var neighborPosition in Utility.FullNeighborPositions)
            {
                var wallPosition = groundPosition + neighborPosition;

                if (!_groundPositions.Contains(wallPosition))
                {
                    _wallPositions.Add(wallPosition);
                }
            }
        }
    }
    
    public HashSet<Vector3Int> GetCrossRoadsPositions()
    {
        return _crossRoadsPositions;
    }
}