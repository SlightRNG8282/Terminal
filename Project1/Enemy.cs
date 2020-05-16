using System;
using System.Collections.Generic;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Input;

namespace Project1
{
	public class Enemy
	{
		//Sprites & Graphics
		private Sprite sprite;
		private GraphicsContext graphics;
		
		//kinematics
		protected float direction;
		
		//Misc
		private bool delete = false;
		protected Random rand = new Random();
		private int pointVal;
		
		public Enemy (GraphicsContext tGraphics, float tX, float tY, Texture2D tex)
		{
			graphics = tGraphics;
			
			Texture2D t = tex;
			
			sprite = new Sprite(graphics, t);		
			sprite.Position.X = tX;
			sprite.Position.Y = tY;
		}
		
		//Used by Dumb Enemy and LockOn Enemy
		public virtual void Update ()
		{
			
		}
		
		//Used to delete the Enemy if it goes off the screen.
		public virtual void Update (float x, float y)
		{
			if (sprite.Position.X + sprite.Width < 0 
			    || y + sprite.Height < 0
			    || x > graphics.Screen.Rectangle.Width 
			    || y > graphics.Screen.Rectangle.Height)
			{
				if (!(this is EnemyBoss))
				{
					Delete = true;
					Points = 0;
				}
			}
		}
		
		//Used by Boss Enemy
		public virtual void Update (long time, List<Enemy> elist)
		{
			
		}
		
		//Used by Sentry Enemy
		public virtual void Update (float x, float y, long time)
		{
			
		}	

		public virtual void Render ()
		{
			sprite.Render();
		}
		
		public float X
		{
			get { return sprite.Position.X; }
		}
		
		public float Y
		{
			get { return sprite.Position.Y; }
		}
		
		public bool Delete
		{
			get { return delete; }
			set { delete = value; }
		}
		
		public Sprite Esprite
		{
			get { return sprite; }
			set { sprite = value; }
		}
		
		public GraphicsContext Egraphics
		{
			get {return graphics; }
		}
		
		public virtual float HitBox
		{
			get { return sprite.Height / 2; }
		}
		
		public int Points
		{
			get { return pointVal; }
			set { pointVal = value; }
		}
	}
}

