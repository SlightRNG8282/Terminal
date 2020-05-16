using System;
using System.Collections.Generic;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Input;

namespace Project1
{
	public class WeaponLazer : Weapon
	{
		//Pretty empty in these for the moment
		private static int tShotInterval = 440;
		
		public WeaponLazer (GraphicsContext tGraphics, float tX, float tY, Texture2D t) : base (tGraphics, tX, tY, t, tShotInterval)
		{
		}
	}
}

