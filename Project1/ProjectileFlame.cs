using System;
using System.Collections.Generic;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Input;

namespace Project1
{
	public class ProjectileFlame : Projectile
	{	
		//Kenematics
		private double speed = 1.5;
		private double xComponent;
		private double yComponent;
		private double distance = 0;
		
		//Color Presets
		private float red = 0f;
		private float blue = 1f;
		private float green = .2f;
		
		//Misc
		private Random rand = new Random();
		private float flameRange = 125;
		
		public ProjectileFlame (GraphicsContext tGraphics, float tX, float tY, int tDirection, Texture2D t) : base (tGraphics, tX, tY, tDirection, t)
		{
		}
		
		//A spread of slower moving semi-random projectiles
		public override void Update ()
		{			
			Direction += rand.Next (-15, 16);
			
			xComponent = speed *  Math.Cos (Math.PI * Direction / 180.0);
			yComponent = speed *  Math.Sin (Math.PI * Direction / 180.0);
			
			distance += Math.Abs(xComponent) + Math.Abs(yComponent);
			
			Psprite.Position.X += (float) xComponent;
			Psprite.Position.Y += (float) yComponent;
			
			if (Psprite.Position.X + Psprite.Width < 0 
			    || Psprite.Position.Y + Psprite.Height < 0
			    || Psprite.Position.X > Pgraphics.Screen.Rectangle.Width 
			    || Psprite.Position.Y > Pgraphics.Screen.Rectangle.Height
			    || distance > flameRange)
			{
				delete = true;
			}
		}
		
		//Turns color of "flames" from blue to orange to red
		public override void Render ()
		{
			Psprite.SetColor(red, green, blue, 1f);
			Psprite.Render();
			
			red += .05f;
			green += .0105f;
			blue -= .075f;
		}
		
		public override float HitBox
		{
			get { return Psprite.Height / 2; }
		}
	}
}

