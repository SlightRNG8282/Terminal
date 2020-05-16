using System;
using System.Collections.Generic;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Input;

using System.Diagnostics;

namespace Project1
{
	public class ProjectileLazer : Projectile
	{				
		//Kenematics
		private double speed = 24;
		private double xComponent;
		private double yComponent;
		private int dir;
		
		//Time Calculations
		private long elapsedTime = 0;
		private long waitTime = 540;

		//Controls Projectile Offspring
		private bool propogated = false;
		private bool propogated2 = false;
		
		//Texture
		private Texture2D tex;
		
		public ProjectileLazer (GraphicsContext tGraphics, float tX, float tY, int tDirection, Texture2D t) : base (tGraphics, tX, tY, tDirection, t)
		{
			Psprite.Rotation = (float) (Math.PI * (Direction - 90) / 180.0);
			dir = tDirection; //Saves current direction for projectile offspring
			tex = t;
		}
		
		//Shoots 3 Superfast moving projectiles that can kill multiple enemies but are delayed.
		public override void Update (long deltaTime, List<Projectile> projectiles)
		{			
			elapsedTime += deltaTime;
			
			if (elapsedTime > 270 && !propogated)
			{
				ProjectileLazer p =  new ProjectileLazer (Pgraphics, X, Y, dir, tex);
				p.Propogated = true;
				projectiles.Add (p);
				propogated = true;
			}
			
			if (elapsedTime > 540 && !propogated2)
			{
				ProjectileLazer p =  new ProjectileLazer (Pgraphics, X, Y, dir, tex);
				p.Propogated = true;
				projectiles.Add (p);
				propogated2 = true;
			}
			
			if (elapsedTime > waitTime)
			{
				move ();
			} 
		}
		
		private void move()
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
		
		private Vector2 getCenter()
		{
			float x = 0;
			float y = 0;
			switch ((int) Direction)
			{
			case 0:
				x = Psprite.Position.X + 30;
				y = Psprite.Position.Y - 10;
				break;
			case 45:
				x = Psprite.Position.X + 29;
				y = Psprite.Position.Y + 12;
				break;
			case 90:
				x = Psprite.Position.X + 8;
				y = Psprite.Position.Y + 30;
				break;
			case 135:
				x = Psprite.Position.X - 16;
				y = Psprite.Position.Y + 25;
				break;
			case 180:
				x = Psprite.Position.X - 30;
				y = Psprite.Position.Y + 8;
				break;
			case 225:
				x = Psprite.Position.X - 28;
				y = Psprite.Position.Y - 16;
				break;
			case 270:
				x = Psprite.Position.X - 10;
				y = Psprite.Position.Y - 30;
				break;
			case 315:
				x = Psprite.Position.X + 14;
				y = Psprite.Position.Y - 29;
				break;
			default:
				x = Psprite.Position.X;
				y = Psprite.Position.Y;
				break;
			}
			
			return new Vector2 (x, y);
		}
		
		//This makes sure that the projectiles created by this projectile don't create more projectiles.
		public bool Propogated
		{
			set { propogated = value; propogated2 = value; }
		}
		
		public override float HitBox
		{
			get { return Psprite.Height / 2 - 4; }
		}
		
		public Vector2 Cent
		{
			get { return getCenter(); }
		}
	}
}

