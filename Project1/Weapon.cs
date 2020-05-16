using System;
using System.Collections.Generic;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Input;

namespace Project1
{
	public class Weapon
	{
		//Sprite & Graphics
		private Sprite sprite;
		private GraphicsContext graphics;
		
		//Time Calculations
		private long shotInterval;
		private long elapsedTime = 0;
		private long currentTime;
		
		//Projectile Creation
		private bool shotsFired = false;
		private bool makeProjectile = false;
		
		//Direction Calculation and enumerations
		private Dir direction;
		private bool isEast = false;
		private bool isWest = false;
		private Dir shotDirection;
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
		
		//Called during initialization
		public Weapon (GraphicsContext tGraphics)
		{
			graphics = tGraphics;
			Texture2D t = new Texture2D ("Application/Assets/Pistol.png", false);
			
			sprite = new Sprite(graphics, t);
			sprite.Position.X = (graphics.Screen.Rectangle.Width / 2) - 15;
			sprite.Position.Y = (graphics.Screen.Rectangle.Height / 2) - 15;
			
			shotInterval = 200;
		}
		
		//Called when making a new weapon.
		public Weapon (GraphicsContext tGraphics, float tX, float tY, Texture2D wpn, long tShotInterval)
		{
			graphics = tGraphics;
			Texture2D t = wpn;
			
			sprite = new Sprite(graphics, t);		
			sprite.Position.X = tX;
			sprite.Position.Y = tY;
			
			shotInterval = tShotInterval;
		}

		public virtual void Update (float datX, float datY, int dir, GamePadData gamePadData, long timeDelta)
		{
			direction = (Dir) dir;  //Assume direction is the way dat is moving
			
			elapsedTime += timeDelta;
			
			//If you aren't cooling down your weapon, allows you to shoot with arrow keys in 8 directions
			//MakeShot tells AppMain to create a projectile
			if (!shotsFired)
			{			
				if((gamePadData.Buttons & GamePadButtons.Left) != 0)
				{
					shotDirection = Dir.west;
					isWest = true;
					currentTime = elapsedTime;
					shotsFired = true;
					makeProjectile = true;
				}
				if((gamePadData.Buttons & GamePadButtons.Right) != 0)
				{
					shotDirection = Dir.east;
					isEast = true;
					currentTime = elapsedTime;
					shotsFired = true;
					makeProjectile = true;
				}
				if((gamePadData.Buttons & GamePadButtons.Up) != 0)
				{
					shotDirection = Dir.north;
					currentTime = elapsedTime;
					shotsFired = true;
					makeProjectile = true;
					if (isWest)
					{
						shotDirection = Dir.northwest;
						currentTime = elapsedTime;	
						shotsFired = true;
						makeProjectile = true;
					} else
					if (isEast)
					{
						shotDirection = Dir.northeast; 
						currentTime = elapsedTime;
						shotsFired = true;
						makeProjectile = true;
					}
				}
				if((gamePadData.Buttons & GamePadButtons.Down) != 0)
				{
					shotDirection = Dir.south;
					currentTime = elapsedTime;
					shotsFired = true;
					makeProjectile = true;
					if (isWest)
					{
						shotDirection = Dir.southwest;
						currentTime = elapsedTime;
						shotsFired = true;
						makeProjectile = true;
					} else
					if (isEast)
					{
						shotDirection = Dir.southeast;
						currentTime = elapsedTime;
						shotsFired = true;
						makeProjectile = true;
					}
				}
				
				isEast = false;
				isWest = false;
			} 
			else
			{
				//Holds weapon in the direction that the player last shot for 1 extra frame
				direction = shotDirection;
				
				//Limits projectiles per shot interval
				if(shotsFired && !(currentTime + shotInterval > elapsedTime))
				{
					shotsFired = false;
				}
			}
			
			if(shotsFired)
			{
				//Holds weapon in the direction that the player last shot
				direction = shotDirection;
			}
			
			setWeaponDirection(datX, datY);
		}
		
		//Decides the visual direction the weapon should point when firing
		private void setWeaponDirection(float datX, float datY)
		{
			if (direction == Dir.east)
			{
				sprite.Rotation = (float) (90 * Math.PI / 180);
				sprite.Position.X = datX + 35;
				sprite.Position.Y = datY;
			} else
			if (direction == Dir.west)
			{
				sprite.Rotation = (float) (270 * Math.PI / 180);
				sprite.Position.X = datX;
				sprite.Position.Y = datY + 35;
			} else
			if (direction == Dir.north)
			{
				sprite.Rotation = (float) (Math.PI / 180);
				sprite.Position.X = datX;
				sprite.Position.Y = datY;
			} else
			if (direction == Dir.south)
			{
				sprite.Rotation = (float) (Math.PI);
				sprite.Position.X = datX + 35;
				sprite.Position.Y = datY + 35;
			} else
			if (direction == Dir.northeast)
			{
				sprite.Rotation = (float) (45 * Math.PI / 180);
				sprite.Position.X = datX + 17;
				sprite.Position.Y = datY - 8;
			} else
			if (direction == Dir.northwest)
			{
				sprite.Rotation = (float) (315 * Math.PI / 180);
				sprite.Position.X = datX - 8;
				sprite.Position.Y = datY + 17;
			} else
			if (direction == Dir.southwest)
			{
				sprite.Rotation = (float) (225 * Math.PI / 180);
				sprite.Position.X = datX + 35 - 13;
				sprite.Position.Y = datY + 40;
			} else
			if (direction == Dir.southeast)
			{
				sprite.Rotation = (float) (135 * Math.PI / 180);
				sprite.Position.X = datX + 41;
				sprite.Position.Y = datY + 35 - 21;
			}
		}
				
		public void Render ()
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
		
		public bool MakeShot
		{
			get { return makeProjectile; }
			set { makeProjectile = value; }
		}
		
		public int Direction
		{
			get { return (int) shotDirection; }
		}
	}
}

