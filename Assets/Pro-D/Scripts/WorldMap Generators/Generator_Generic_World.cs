/* 
* This code has been designed and developed by Gray Lake Studios.
* You may only use this code if you?ve acquired the appropriate license.
* To acquire such licenses you may visit www.graylakestudios.com and/or Unity Asset Store
* For all inquiries you may contact contact@graylakestudios.com
* Copyright ? 2012 Gray Lake Studios
*/

using UnityEngine;
using System.Collections;

namespace ProD
{
	/// <summary>
	/// An example world generator, making a world of several maps, all using the same theme.
	/// It does not materialize the world yet, this functionality is kept seperate on purpose.
	/// </summary>
	public static class Generator_Generic_World
	{
		private static WorldMap worldMap;
		private static int _worldMap_Size_X = 3;
		private static int _worldMap_Size_Y = 3;
		private static int _map_Size_X = 29;
		private static int _map_Size_Y = 29;

		public static string theme = "Terminal Theme";
		
		public static WorldMap Generate(string generatorName, int world_Size_X, int world_Size_Y, int map_Size_X, int map_Size_Y)
		{
			_worldMap_Size_X = world_Size_X;
			_worldMap_Size_Y = world_Size_Y;
			_map_Size_X = map_Size_X;
			_map_Size_Y = map_Size_Y;

			worldMap = new WorldMap(_worldMap_Size_X, _worldMap_Size_Y);

			for (int i = 0; i < _worldMap_Size_X; i++) 
			{
				for (int j = 0; j < _worldMap_Size_Y; j++) 
				{
					Map tempMap = new Map();
					//tempMap = Generator_Dungeon.Generate();
					
					switch (generatorName) 
					{
					case "Cavern":
						Generator_Cavern.SetGenericProperties(_map_Size_X,_map_Size_Y, theme);
						tempMap = Generator_Cavern.Generate();
						break;
					case "Dungeon":
						Generator_Dungeon.SetGenericProperties(_map_Size_X,_map_Size_Y, theme);
						tempMap = Generator_Dungeon.Generate();
						break;
					case "AlternativeDungeon":
						Generator_AlternativeDungeon.SetGenericProperties(_map_Size_X, _map_Size_Y, theme);
						tempMap = Generator_AlternativeDungeon.Generate();
						break;
					case "DungeonRuins":
						Generator_DungeonRuins.SetGenericProperties(_map_Size_X,_map_Size_Y, theme);
						tempMap = Generator_DungeonRuins.Generate();
						break;
					case "RockyHill":
						Generator_RockyHill.SetGenericProperties(_map_Size_X,_map_Size_Y, theme);
						tempMap = Generator_RockyHill.Generate();
						break;
					case "Maze":
						Generator_Maze.SetGenericProperties(_map_Size_X,_map_Size_Y, theme);	
						tempMap = Generator_Maze.Generate();
						break;
					case "ObstacleBiome":
						Generator_ObstacleBiome.SetGenericProperties(_map_Size_X,_map_Size_Y, theme);
						tempMap = Generator_ObstacleBiome.Generate();
						break;
					case "PerlinLikeBiome":
						Generator_PerlinLikeBiome.SetGenericProperties(_map_Size_X,_map_Size_Y, theme);
						tempMap = Generator_PerlinLikeBiome.Generate();
						break;
					case "StickDungeon":
						Generator_StickDungeon.SetGenericProperties(_map_Size_X,_map_Size_Y, theme);
						tempMap = Generator_StickDungeon.Generate();
						break;
					case "StickBiome":
						Generator_StickBiome.SetGenericProperties(_map_Size_X,_map_Size_Y, theme);
						tempMap = Generator_StickBiome.Generate();
						break;
					case "RoundRooms":
						Generator_RoundRooms.SetGenericProperties(_map_Size_X, _map_Size_Y, theme);
						tempMap = Generator_RoundRooms.Generate();
						break;
					case "DwarfTown":
						Generator_DwarfTown.SetGenericProperties(_map_Size_X, _map_Size_Y, theme);
						tempMap = Generator_DwarfTown.Generate();
						break;
					case "Castle":
						Generator_Castle.SetGenericProperties(_map_Size_X, _map_Size_Y, theme);
						tempMap = Generator_Castle.Generate();
						break;
					default:
						Generator_Maze.SetGenericProperties(_map_Size_X,_map_Size_Y, theme);	
						tempMap = Generator_Maze.Generate();
					break;
					}
					
					tempMap.addressOnWorldMap = new Address(i,j);
					tempMap.worldMap = worldMap;
					worldMap.maps[i,j] = tempMap;
				}
			}
			return worldMap;
		}
		
	}
}
