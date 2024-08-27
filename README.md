# 2D-Procedural-Generation-Algorithm

This repository contains the scripts for generating a procedurally generated town in a 2D top-down tile-based game using Unity. The current implementation focuses on creating the town's layout, including roads, various buildings, and other essential structures.
Features

  Procedural Road Generation: Roads are randomly generated to form a basic layout of the town.

  Building Placement: Various key buildings, such as the player's apartment, parliament, court, university, hospital, and military base, are strategically placed based on       road positions.

  Dynamic Map Sizing: Choose between small, medium, and large map sizes, each with different dimensions and iterations for road generation.
  Tile-based System: The town is built on a tilemap using different tile types for roads, buildings, and surrounding structures.

How It Works

  Map Size Initialization: The map size (small, medium, large) determines the dimensions of the grid and the maximum number of iterations for road generation.
  Road Generation: Roads are generated starting from a random point and extended in random directions. The script ensures roads are single-tile wide to maintain a clean         layout.

  Building Placement: Buildings are placed adjacent to roads with considerations to avoid overlap and ensure logical placement. Surrounding structures, like parks and car       parks, are placed nearby.

  Rendering: After the roads and buildings are generated, the Tilemap component renders the tiles to create a visual representation of the town.

Setup

  Clone the repository to your local machine.
  Open the project in Unity.
  Attach the CityGenerator script to a GameObject in your scene.
  Assign the Tilemap and TileTypes references in the Unity editor.
  Configure the MapSize as desired.
  Play the scene to see the procedurally generated town.
