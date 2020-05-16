using System;
using System.Collections.Generic;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Input;

namespace Project1
{
	public class Projectile
	{
		//Sprites and Graphics
		private Sprite sprite;
		private GraphicsContext graphics;
		
		//Directions and Dir enumeration
		private float direction;
		public enum Dir
		{
			north,
			northeast,
			east,
			southeast,
			south,
			southwest,
			west,
			northwest,
			nada
		};
		
		//Misc
		protected bool delete = false;

		public Projectile (GraphicsContext tGraphics, float tX, float tY, int tDirection, Texture2D tT)
		{
			graphics = tGraphics;
			
			Texture2D t = tT;
			
			sprite = new Sprite(graphics, t);		
			sprite.Position.X = tX;
			sprite.Position.Y = tY;
			calculateDirection((Dir) tDirection);
		}
		
		//Calculates the direction based on the enumeration passed from the Weapon in use
		protected virtual void calculateDirection(Dir dir)
		{
			if (dir == Dir.north)
				direction = 270;
			else
			if (dir == Dir.northeast)
				direction = 315;
			else
			if (dir == Dir.east)
				direction = 0;
			else
			if (dir == Dir.southeast)
				direction = 45;
			else
			if (dir == Dir.south)
				direction = 90;
			else
			if (dir == Dir.southwest)
				direction = 135;
			else
			if (dir == Dir.west)
			{
				if (this is ProjectileBasic)
				    sprite.Position.Y -= 4; // Otherwise, projectile wont show when shooting flush to the bottom of the screen
				direction = 180;
			}
			else
			if (dir == Dir.northwest)
				direction = 225;
		}
		
		//Used by the Basic and Flame Projectile
		public virtual void Update ()
		{
			
		}

		//Used by the Lazer Projectile
		public virtual void Update (long deltaTime, List<Projectile> projectiles)
		{
			
		}


		public virtual void Render ()
		{
			sprite.Render();
		}
		
		public bool Delete
		{
			get { return delete; }
			set { delete = value; }
		}
		
		public float X
		{
			get { return sprite.Position.X; }
		}
		
		public float Y
		{
			get { return sprite.Position.Y; }
		}
		
		public Sprite Psprite
		{
			get { return sprite; }
		}
		
		public GraphicsContext Pgraphics
		{
			get { return graphics; }
		}
		
		public float Direction
		{
			get { return direction; }
			set { direction = value; }
		}
		
		public virtual float HitBox
		{
			get { return 1f; }
		}
	}
}

