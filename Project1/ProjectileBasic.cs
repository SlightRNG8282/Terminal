using System;
using System.Collections.Generic;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Input;

namespace Project1
{
	public class ProjectileBasic : Projectile
	{	
		//Kenematics
		private double speed = 8;
		private double xComponent;
		private double yComponent;
		
		public ProjectileBasic (GraphicsContext tGraphics, float tX, float tY, int tDirection, Texture2D t) : base (tGraphics, tX, tY, tDirection, t)
		{
		}
		
		//Your basic PEW-in-one-direction projectile
		public override void Update ()
		{
			xComponent = speed *  Math.Cos (Math.PI * Direction / 180.0);
			yComponent = speed *  Math.Sin (Math.PI * Direction / 180.0);
			
			Psprite.Position.X += (float) xComponent;
			Psprite.Position.Y += (float) yComponent;
			
			if (Psprite.Position.X + Psprite.Width < 0 
			    || Psprite.Position.Y + Psprite.Height < 0
			    || Psprite.Position.X > Pgraphics.Screen.Rectangle.Width 
			    || Psprite.Position.Y > Pgraphics.Screen.Rectangle.Height)
			{
				delete = true;
			}
		}
		
		public override float HitBox
		{
			get { return Psprite.Height + 2; }
		}
	}
}

