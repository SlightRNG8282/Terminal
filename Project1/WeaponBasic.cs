using System;
using System.Collections.Generic;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Input;

namespace Project1
{
	public class WeaponBasic : Weapon
	{
		//Pretty empty in these for the moment
		private static int tShotInterval = 80;
		
		//Used during initialization
		public WeaponBasic (GraphicsContext tGraphics) : base (tGraphics)
		{
		}
		
		public WeaponBasic (GraphicsContext tGraphics, float tX, float tY, Texture2D t) : base (tGraphics, tX, tY, t, tShotInterval)
		{
		}
	}
}

