/* 
* This code has been designed and developed by Gray Lake Studios.
* You may only use this code if you’ve acquired the appropriate license.
* To acquire such licenses you may visit www.graylakestudios.com and/or Unity Asset Store
* For all inquiries you may contact contact@graylakestudios.com
* Copyright © 2012 Gray Lake Studios
*/

using UnityEngine;
using System.Collections;

namespace ProD
{
	public static class Generator_Maze
	{
		#region PROD DEFINED DEFAULT CONTENT
		private static Map map;
		private static void PrepareMap()
		{
			map = new Map(map_Size_X, map_Size_Y);
			map.theme = theme;
		}
		public static void SetGenericProperties(int map_Size_X_, int map_Size_Y_, string theme_)
		{
			map_Size_X = map_Size_X_;
			map_Size_Y = map_Size_Y_;
			theme = theme_;
		}
		#endregion PROD DEFINED DEFAULT CONTENT

		#region USER DEFINED DEFAULT CONTENT
		//You should set all these variables to your liking.
		private static int map_Size_X = 29;
		private static int map_Size_Y = 29;
		private static string theme = "Terminal Theme";

		//These variables are here for runtime alteration, mainly for debug testing.
		private static string type_Abyss = "Abyss";
		private static string type_Path = "Path";
		private static string type_Wall = "Wall";

		//This method is only here for runtime alteration, mainly for debug testing.
		public static void SetSpecificProperties(string type_Abyss_, string type_Path_, string type_Wall_)
		{
			type_Abyss = type_Abyss_;
			type_Path = type_Path_;
			type_Wall = type_Wall_;
		}

		//Generate() has the actual methods that generate the map data. 
		//You may customize this method with MethodLibrary methods.
		public static Map Generate()
		{
			PrepareMap();
			MethodLibrary.CreateMaze(map, type_Path, type_Abyss);
			MethodLibrary.SetCellsOfTypeAToB(map, type_Abyss, type_Wall);
			MethodLibrary.ConvertUnreachableCells(map, type_Wall, type_Abyss);
			MethodLibrary.FrameMap(map, type_Wall, 1);
			return map;
		}
		#endregion USER DEFINED DEFAULT CONTENT
	}
}