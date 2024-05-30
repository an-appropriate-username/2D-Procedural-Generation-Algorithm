using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class CityGenerator : MonoBehaviour
{
    public Tilemap tilemap;
    public TileTypes tileTypes;

    public enum MapSize
    {
        Small,
        Medium,
        Large
    }

    public MapSize mapSize;

    public int width;
    public int height;
    private int maxIterations;
    private int[,] grid;
    public List<Vector2Int> roadPositions = new List<Vector2Int>();

    void Start()
    {
        SetMapSize();
        grid = new int[width, height];
        GenerateRoads();
        PlacePlayerApartment(); 
        PlaceParliamentAndSurroundings(); 
        PlaceCourtAndSurroundings();
        PlaceUniversityAndSurroundings();
        PlaceHospitalAndSurroundings();
        PlaceMilitaryBaseAndSurroundings();
        PlaceTownCenterAndSurroundings();
        PlacePoliceStations();
        //PlaceBuildingTiles();
        RenderRoads();
    }

    void SetMapSize()
    {
        switch (mapSize)
        {
            case MapSize.Small:
                width = 100;
                height = 100;
                maxIterations = 3000;
                break;
            case MapSize.Medium:
                width = 200;
                height = 200;
                maxIterations = 6000;
                break;
            case MapSize.Large:
                width = 400;
                height = 400;
                maxIterations = 12000;
                break;
        }
    }

    void GenerateRoads()
    {
        int startX = Random.Range(5, width - 5);
        int startY = Random.Range(5, height - 5);
        grid[startX, startY] = 1; // Start with a random central road
        roadPositions.Add(new Vector2Int(startX, startY));

        int currentIteration = 0;

        while (currentIteration <= maxIterations)
        {
            Vector2Int randomDirection = GetRandomDirection();
            Vector2Int newPosition = roadPositions[Random.Range(0, roadPositions.Count)] + randomDirection;

            if (IsWithinBounds(newPosition.x, newPosition.y) && grid[newPosition.x, newPosition.y] == 0)
            {
                if (IsSingleTileWideRoad(newPosition))
                {
                    grid[newPosition.x, newPosition.y] = 1; // Road
                    roadPositions.Add(newPosition);
                }
            }

            currentIteration++;
        }
    }

    bool IsSingleTileWideRoad(Vector2Int position)
    {
        int count = 0;
        for (int x = position.x - 1; x <= position.x + 1; x++)
        {
            for (int y = position.y - 1; y <= position.y + 1; y++)
            {
                if (IsWithinBounds(x, y) && grid[x, y] == 1)
                {
                    count++;
                }
            }
        }

        return count <= 2;
    }

    Vector2Int GetRandomDirection()
    {
        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
        Vector2Int randomDirection = directions[Random.Range(0, directions.Length)];
        return randomDirection;
    }

    void PlacePlayerApartment()
    {
        List<Vector2Int> potentialPositions = new List<Vector2Int>();

        foreach (Vector2Int roadPosition in roadPositions)
        {
            Vector2Int[] adjacentPositions = new Vector2Int[]
            {
                new Vector2Int(roadPosition.x, roadPosition.y + 1), // Top
                new Vector2Int(roadPosition.x, roadPosition.y - 1), // Bottom
                new Vector2Int(roadPosition.x + 1, roadPosition.y), // Right
                new Vector2Int(roadPosition.x - 1, roadPosition.y)  // Left
            };

            foreach (Vector2Int pos in adjacentPositions)
            {
                if (IsWithinBounds(pos.x, pos.y) && grid[pos.x, pos.y] == 0)
                {
                    potentialPositions.Add(pos);
                }
            }
        }

        if (potentialPositions.Count > 0)
        {
            Vector2Int apartmentPosition = potentialPositions[Random.Range(0, potentialPositions.Count)];
            grid[apartmentPosition.x, apartmentPosition.y] = 3; // Assign a unique value to player apartment

            Vector3Int tilePosition = new Vector3Int(apartmentPosition.x, apartmentPosition.y, 0);
            tilemap.SetTile(tilePosition, tileTypes.playerApartment);
            
            Vector3 worldPosition = tilemap.CellToWorld(tilePosition);
            Camera.main.transform.position = new Vector3(worldPosition.x, worldPosition.y, Camera.main.transform.position.z);
        }
    }

    void PlaceParliamentAndSurroundings()
    {
    List<Vector2Int> potentialPositions = new List<Vector2Int>();

    foreach (Vector2Int roadPosition in roadPositions)
    {
        Vector2Int[] adjacentPositions = new Vector2Int[]
        {
            new Vector2Int(roadPosition.x, roadPosition.y + 1), // Top
            new Vector2Int(roadPosition.x, roadPosition.y - 1), // Bottom
            new Vector2Int(roadPosition.x + 1, roadPosition.y), // Right
            new Vector2Int(roadPosition.x - 1, roadPosition.y)  // Left
        };

        foreach (Vector2Int pos in adjacentPositions)
        {
            if (IsWithinBounds(pos.x, pos.y) && grid[pos.x, pos.y] == 0)
            {
                potentialPositions.Add(pos);
            }
        }
    }

    if (potentialPositions.Count > 0)
    {
        Vector2Int parliamentPosition = potentialPositions[Random.Range(0, potentialPositions.Count)];
        grid[parliamentPosition.x, parliamentPosition.y] = 4; // Assign a unique value to parliament

        Vector3Int tilePosition = new Vector3Int(parliamentPosition.x, parliamentPosition.y, 0);
        tilemap.SetTile(tilePosition, tileTypes.parliament);

        // Check if the tile above is blocked by a road
        bool isAboveRoad = IsWithinBounds(parliamentPosition.x, parliamentPosition.y + 1) && grid[parliamentPosition.x, parliamentPosition.y + 1] == 1;
        
        // Check if the tile below is blocked by a road
        bool isBelowRoad = IsWithinBounds(parliamentPosition.x, parliamentPosition.y - 1) && grid[parliamentPosition.x, parliamentPosition.y - 1] == 1;

        // Place the surrounding tiles accordingly
        if (!isAboveRoad)
        {
            PlaceSurroundingTile(parliamentPosition, tileTypes.courtYard, 5, Vector2Int.up); // Place courtyard above parliament
        }
        else if (!isBelowRoad)
        {
            PlaceSurroundingTile(parliamentPosition , tileTypes.office, 6, Vector2Int.down); // Place office below parliament
        }
        else
        {
            // If both above and below are blocked by roads, place courtyard on the left
            PlaceSurroundingTile(parliamentPosition , tileTypes.courtYard, 5, Vector2Int.left);
        }
    }
    }   

    void PlaceCourtAndSurroundings()

    {
    List<Vector2Int> potentialPositions = new List<Vector2Int>();

    foreach (Vector2Int roadPosition in roadPositions)
    {
        Vector2Int[] adjacentPositions = new Vector2Int[]
        {
            new Vector2Int(roadPosition.x, roadPosition.y + 1), // Top
            new Vector2Int(roadPosition.x, roadPosition.y - 1), // Bottom
            new Vector2Int(roadPosition.x + 1, roadPosition.y), // Right
            new Vector2Int(roadPosition.x - 1, roadPosition.y)  // Left
        };

        foreach (Vector2Int pos in adjacentPositions)
        {
            if (IsWithinBounds(pos.x, pos.y) && grid[pos.x, pos.y] == 0)
            {
                potentialPositions.Add(pos);
            }
        }
    }

    if (potentialPositions.Count > 0)
    {
        Vector2Int courtPosition = potentialPositions[Random.Range(0, potentialPositions.Count)];
        grid[courtPosition.x, courtPosition.y] = 8; // Assign a unique value to court

        Vector3Int tilePosition = new Vector3Int(courtPosition.x, courtPosition.y, 0);
        tilemap.SetTile(tilePosition, tileTypes.court);

        // Check if the tile above is blocked by a road
        bool isAboveRoad = IsWithinBounds(courtPosition.x, courtPosition.y + 1) && grid[courtPosition.x, courtPosition.y + 1] == 1;
        
        // Check if the tile below is blocked by a road
        bool isBelowRoad = IsWithinBounds(courtPosition.x, courtPosition.y - 1) && grid[courtPosition.x, courtPosition.y - 1] == 1;

        // Place the surrounding tiles accordingly
        if (!isAboveRoad)
        {
            PlaceSurroundingTile(courtPosition, tileTypes.jail, 9, Vector2Int.up); // Place jail above court
        }
        else if (!isBelowRoad)
        {
            PlaceSurroundingTile(courtPosition , tileTypes.jail, 9, Vector2Int.down); // Place jail below court
        }
        else
        {
            // If both above and below are blocked by roads, place jail on the left
            PlaceSurroundingTile(courtPosition , tileTypes.jail, 9, Vector2Int.left);
        }
        
        // Place car park opposite to jail
        Vector2Int carParkDirection = isAboveRoad || isBelowRoad ? Vector2Int.right : Vector2Int.down;
        PlaceSurroundingTile(courtPosition , tileTypes.carPark, 10, carParkDirection);
    }
    }

    void PlaceUniversityAndSurroundings()
    {
    List<Vector2Int> potentialPositions = new List<Vector2Int>();

    foreach (Vector2Int roadPosition in roadPositions)
    {
        Vector2Int[] adjacentPositions = new Vector2Int[]
        {
            new Vector2Int(roadPosition.x, roadPosition.y + 1), // Top
            new Vector2Int(roadPosition.x, roadPosition.y - 1), // Bottom
            new Vector2Int(roadPosition.x + 1, roadPosition.y), // Right
            new Vector2Int(roadPosition.x - 1, roadPosition.y)  // Left
        };

        foreach (Vector2Int pos in adjacentPositions)
        {
            if (IsWithinBounds(pos.x, pos.y) && grid[pos.x, pos.y] == 0)
            {
                potentialPositions.Add(pos);
            }
        }
    }

    if (potentialPositions.Count > 0)
    {
        Vector2Int universityPosition = potentialPositions[Random.Range(0, potentialPositions.Count)];
        grid[universityPosition.x, universityPosition.y] = 8; // Assign a unique value to university

        Vector3Int tilePosition = new Vector3Int(universityPosition.x, universityPosition.y, 0);
        tilemap.SetTile(tilePosition, tileTypes.university);

        // Check if the tile above is blocked by a road
        bool isAboveRoad = IsWithinBounds(universityPosition.x, universityPosition.y + 1) && grid[universityPosition.x, universityPosition.y + 1] == 1;
        
        // Check if the tile below is blocked by a road
        bool isBelowRoad = IsWithinBounds(universityPosition.x, universityPosition.y - 1) && grid[universityPosition.x, universityPosition.y - 1] == 1;

        // Place the surrounding tiles accordingly
        if (!isAboveRoad)
        {
            PlaceSurroundingTile(universityPosition, tileTypes.park, 9, Vector2Int.up); // Place park above university
        }
        else if (!isBelowRoad)
        {
            PlaceSurroundingTile(universityPosition , tileTypes.park, 9, Vector2Int.down); // Place park below university
        }
        else
        {
            // If both above and below are blocked by roads, place park on the left
            PlaceSurroundingTile(universityPosition , tileTypes.park, 9, Vector2Int.left);
        }
        
        // Place park opposite to the other park
        Vector2Int parkDirection = isAboveRoad || isBelowRoad ? Vector2Int.right : Vector2Int.down;
        PlaceSurroundingTile(universityPosition , tileTypes.park, 10, parkDirection);
    }
    }

    void PlaceHospitalAndSurroundings()
    {
    List<Vector2Int> potentialPositions = new List<Vector2Int>();

    foreach (Vector2Int roadPosition in roadPositions)
    {
        Vector2Int[] adjacentPositions = new Vector2Int[]
        {
            new Vector2Int(roadPosition.x, roadPosition.y + 1), // Top
            new Vector2Int(roadPosition.x, roadPosition.y - 1), // Bottom
            new Vector2Int(roadPosition.x + 1, roadPosition.y), // Right
            new Vector2Int(roadPosition.x - 1, roadPosition.y)  // Left
        };

        foreach (Vector2Int pos in adjacentPositions)
        {
            if (IsWithinBounds(pos.x, pos.y) && grid[pos.x, pos.y] == 0)
            {
                potentialPositions.Add(pos);
            }
        }
    }

    if (potentialPositions.Count > 0)
    {
        Vector2Int hospitalPosition = potentialPositions[Random.Range(0, potentialPositions.Count)];
        grid[hospitalPosition.x, hospitalPosition.y] = 8; // Assign a unique value to hospital

        Vector3Int tilePosition = new Vector3Int(hospitalPosition.x, hospitalPosition.y, 0);
        tilemap.SetTile(tilePosition, tileTypes.hospital);

        // Check if the tile to the right is blocked by a road
        bool isRightRoad = IsWithinBounds(hospitalPosition.x + 1, hospitalPosition.y) && grid[hospitalPosition.x + 1, hospitalPosition.y] == 1;
        
        // Check if the tile to the left is blocked by a road
        bool isLeftRoad = IsWithinBounds(hospitalPosition.x - 1, hospitalPosition.y) && grid[hospitalPosition.x - 1, hospitalPosition.y] == 1;

        // Place the surrounding tiles accordingly
        if (!isRightRoad)
        {
            PlaceSurroundingTile(hospitalPosition, tileTypes.carPark, 9, Vector2Int.right); // Place car park to the right of hospital
        }
        else if (!isLeftRoad)
        {
            PlaceSurroundingTile(hospitalPosition, tileTypes.carPark, 9, Vector2Int.left); // Place car park to the left of hospital
        }
        else
        {
            // If both right and left are blocked by roads, place car park below
            PlaceSurroundingTile(hospitalPosition, tileTypes.carPark, 9, Vector2Int.down);
        }
        
        // Place park opposite to car park
        Vector2Int parkDirection = isRightRoad || isLeftRoad ? Vector2Int.up : Vector2Int.left;
        PlaceSurroundingTile(hospitalPosition, tileTypes.park, 10, parkDirection);
    }
    }

    void PlaceMilitaryBaseAndSurroundings()
    {
    List<Vector2Int> potentialPositions = new List<Vector2Int>();

    foreach (Vector2Int roadPosition in roadPositions)
    {
        Vector2Int[] adjacentPositions = new Vector2Int[]
        {
            new Vector2Int(roadPosition.x, roadPosition.y + 1), // Top
            new Vector2Int(roadPosition.x, roadPosition.y - 1), // Bottom
            new Vector2Int(roadPosition.x + 1, roadPosition.y), // Right
            new Vector2Int(roadPosition.x - 1, roadPosition.y)  // Left
        };

        foreach (Vector2Int pos in adjacentPositions)
        {
            if (IsWithinBounds(pos.x, pos.y) && grid[pos.x, pos.y] == 0)
            {
                potentialPositions.Add(pos);
            }
        }
    }

    if (potentialPositions.Count > 0)
    {
        Vector2Int militaryBasePosition = potentialPositions[Random.Range(0, potentialPositions.Count)];
        grid[militaryBasePosition.x, militaryBasePosition.y] = 11; // Assign a unique value to military base

        Vector3Int tilePosition = new Vector3Int(militaryBasePosition.x, militaryBasePosition.y, 0);
        tilemap.SetTile(tilePosition, tileTypes.militaryBase);

        // Check if the tile above, below, right, or left is blocked by a road
        bool isAboveRoad = IsWithinBounds(militaryBasePosition.x, militaryBasePosition.y + 1) && grid[militaryBasePosition.x, militaryBasePosition.y + 1] == 1;
        bool isBelowRoad = IsWithinBounds(militaryBasePosition.x, militaryBasePosition.y - 1) && grid[militaryBasePosition.x, militaryBasePosition.y - 1] == 1;
        bool isRightRoad = IsWithinBounds(militaryBasePosition.x + 1, militaryBasePosition.y) && grid[militaryBasePosition.x + 1, militaryBasePosition.y] == 1;
        bool isLeftRoad = IsWithinBounds(militaryBasePosition.x - 1, militaryBasePosition.y) && grid[militaryBasePosition.x - 1, militaryBasePosition.y] == 1;

        // Place the surrounding tiles accordingly
        if (!isRightRoad)
        {
            PlaceSurroundingTile(militaryBasePosition, tileTypes.weaponsDepot, 12, Vector2Int.right); // Place weapons depot to the right of military base
        }
        else if (!isLeftRoad)
        {
            PlaceSurroundingTile(militaryBasePosition, tileTypes.weaponsDepot, 12, Vector2Int.left); // Place weapons depot to the left of military base
        }
        else if (!isBelowRoad)
        {
            PlaceSurroundingTile(militaryBasePosition, tileTypes.weaponsDepot, 12, Vector2Int.down); // Place weapons depot below military base
        }
        else
        {
            // If all directions are blocked by roads, place weapons depot above
            PlaceSurroundingTile(militaryBasePosition, tileTypes.weaponsDepot, 12, Vector2Int.up);
        }
    }
    }

    void PlaceTownCenterAndSurroundings()
    {
    List<Vector2Int> potentialPositions = new List<Vector2Int>();

    foreach (Vector2Int roadPosition in roadPositions)
    {
        Vector2Int[] adjacentPositions = new Vector2Int[]
        {
            new Vector2Int(roadPosition.x, roadPosition.y + 1), // Top
            new Vector2Int(roadPosition.x, roadPosition.y - 1), // Bottom
            new Vector2Int(roadPosition.x + 1, roadPosition.y), // Right
            new Vector2Int(roadPosition.x - 1, roadPosition.y)  // Left
        };

        foreach (Vector2Int pos in adjacentPositions)
        {
            if (IsWithinBounds(pos.x, pos.y) && grid[pos.x, pos.y] == 0)
            {
                potentialPositions.Add(pos);
            }
        }
    }

    if (potentialPositions.Count > 0)
    {
        Vector2Int townCenterPosition = potentialPositions[Random.Range(0, potentialPositions.Count)];
        grid[townCenterPosition.x, townCenterPosition.y] = 13; // Assign a unique value to town center

        Vector3Int tilePosition = new Vector3Int(townCenterPosition.x, townCenterPosition.y, 0);
        tilemap.SetTile(tilePosition, tileTypes.townCenter);

        // Check if the tile to the right is blocked by a road
        bool isRightRoad = IsWithinBounds(townCenterPosition.x + 1, townCenterPosition.y) && grid[townCenterPosition.x + 1, townCenterPosition.y] == 1;
        
        // Check if the tile to the left is blocked by a road
        bool isLeftRoad = IsWithinBounds(townCenterPosition.x - 1, townCenterPosition.y) && grid[townCenterPosition.x - 1, townCenterPosition.y] == 1;

        // Check if the tile above is blocked by a road
        bool isAboveRoad = IsWithinBounds(townCenterPosition.x, townCenterPosition.y + 1) && grid[townCenterPosition.x, townCenterPosition.y + 1] == 1;

        // Check if the tile below is blocked by a road
        bool isBelowRoad = IsWithinBounds(townCenterPosition.x, townCenterPosition.y - 1) && grid[townCenterPosition.x, townCenterPosition.y - 1] == 1;

        // Place the surrounding tiles accordingly
        if (!isAboveRoad)
        {
            PlaceSurroundingTile(townCenterPosition, tileTypes.courtYard, 14, Vector2Int.up); // Place courtyard above town center
        }
        else if (!isBelowRoad)
        {
            PlaceSurroundingTile(townCenterPosition, tileTypes.courtYard, 14, Vector2Int.down); // Place courtyard below town center
        }
        else if (!isRightRoad)
        {
            PlaceSurroundingTile(townCenterPosition, tileTypes.courtYard, 14, Vector2Int.right); // Place courtyard to the right of town center
        }
        else
        {
            // If all other directions are blocked by roads, place courtyard to the left
            PlaceSurroundingTile(townCenterPosition, tileTypes.courtYard, 14, Vector2Int.left);
        }
    }
    }

    void PlacePoliceStations()
    {
    int policeCount = 0;
    int minDistance = 0;

    // Set the number of police stations and minimum distance based on the map size
    switch (mapSize)
    {
        case MapSize.Small:
            policeCount = 4;
            minDistance = 7;
            break;
        case MapSize.Medium:
            policeCount = 5;
            minDistance = 10;
            break;
        case MapSize.Large:
            policeCount = 7;
            minDistance = 15;
            break;
    }

    List<Vector2Int> potentialPositions = new List<Vector2Int>();
    List<Vector2Int> placedPolicePositions = new List<Vector2Int>();

    foreach (Vector2Int roadPosition in roadPositions)
    {
        Vector2Int[] adjacentPositions = new Vector2Int[]
        {
            new Vector2Int(roadPosition.x, roadPosition.y + 1), // Top
            new Vector2Int(roadPosition.x, roadPosition.y - 1), // Bottom
            new Vector2Int(roadPosition.x + 1, roadPosition.y), // Right
            new Vector2Int(roadPosition.x - 1, roadPosition.y)  // Left
        };

        foreach (Vector2Int pos in adjacentPositions)
        {
            if (IsWithinBounds(pos.x, pos.y) && grid[pos.x, pos.y] == 0)
            {
                potentialPositions.Add(pos);
            }
        }
    }

    for (int i = 0; i < policeCount; i++)
    {
        bool validPositionFound = false;

        while (!validPositionFound && potentialPositions.Count > 0)
        {
            Vector2Int policePosition = potentialPositions[Random.Range(0, potentialPositions.Count)];
            potentialPositions.Remove(policePosition);

            // Check if this position is far enough from all placed police stations
            bool isFarEnough = true;
            foreach (Vector2Int placedPosition in placedPolicePositions)
            {
                if (Vector2Int.Distance(policePosition, placedPosition) < minDistance)
                {
                    isFarEnough = false;
                    break;
                }
            }

            if (isFarEnough)
            {
                grid[policePosition.x, policePosition.y] = 15; // Assign a unique value to police station

                Vector3Int tilePosition = new Vector3Int(policePosition.x, policePosition.y, 0);
                tilemap.SetTile(tilePosition, tileTypes.police);

                placedPolicePositions.Add(policePosition);
                validPositionFound = true;
            }
        }
    }
    }

    void PlaceSurroundingTile(Vector2Int startPosition, TileBase tileType, int tileValue, Vector2Int direction)
    {
        Vector2Int newPosition = startPosition + direction;

        if (IsWithinBounds(newPosition.x, newPosition.y) && grid[newPosition.x, newPosition.y] == 0)
        {
            grid[newPosition.x, newPosition.y] = tileValue; // Assign a unique value to the tile
            tilemap.SetTile(new Vector3Int(newPosition.x, newPosition.y, 0), tileType);
        }
    }

    void RenderRoads()
    {
        foreach (Vector2Int position in roadPositions)
        {
            tilemap.SetTile(new Vector3Int(position.x, position.y, 0), tileTypes.roadTile);

            // Place building tiles beside the road tiles
            PlaceBuildingTiles(position);
        }
    }

    void PlaceBuildingTiles(Vector2Int roadPosition)
    {
    Vector2Int[] adjacentPositions = new Vector2Int[]
    {
        new Vector2Int(roadPosition.x, roadPosition.y + 1), // Top
        new Vector2Int(roadPosition.x, roadPosition.y - 1), // Bottom
        new Vector2Int(roadPosition.x + 1, roadPosition.y), // Right
        new Vector2Int(roadPosition.x - 1, roadPosition.y)  // Left
    };

    foreach (Vector2Int pos in adjacentPositions)
    {
        if (IsWithinBounds(pos.x, pos.y) && grid[pos.x, pos.y] == 0)
        {
            // Check if the adjacent position is within bounds and is a road position
            bool isRoadPosition = IsWithinBounds(pos.x, pos.y) && roadPositions.Contains(pos);

            if (!isRoadPosition)
            {
                // Place a building based on the specified percentages
                TileBase randomTile = GetRandomBuildingTile();
                tilemap.SetTile(new Vector3Int(pos.x, pos.y, 0), randomTile);
            }
        }
    }
    }

    TileBase GetRandomBuildingTile()
    {
    // Define the tile types and their corresponding weights
    Dictionary<TileBase, int> tileWeights = new Dictionary<TileBase, int>
    {
        { tileTypes.apartments, 30 },
        { tileTypes.smallHouse, 10 },
        { tileTypes.largeHouse, 6 },
        { tileTypes.park, 6 },
        { tileTypes.carPark, 6 },
        { tileTypes.factory, 10 },
        { tileTypes.school, 2 },
        { tileTypes.office, 5 },
        { tileTypes.embassy, 5 },
        { tileTypes.storage, 5 },
        { tileTypes.medicalCenter, 5 },
        { tileTypes.kiosik, 5 },
        { tileTypes.bar, 5 }
    };

    // Create a list to hold the weighted tile types
    List<TileBase> weightedTiles = new List<TileBase>();

    foreach (var tileWeight in tileWeights)
    {
        for (int i = 0; i < tileWeight.Value; i++)
        {
            weightedTiles.Add(tileWeight.Key);
        }
    }

    // Return a randomly selected tile from the weighted list
    return weightedTiles[Random.Range(0, weightedTiles.Count)];
    }

    bool IsWithinBounds(int x, int y)
    {
        return x >= 0 && x < width && y >= 0 && y < height;
    }

}

