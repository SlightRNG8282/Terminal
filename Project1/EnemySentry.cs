using System;
using System.Collections.Generic;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Input;

namespace Project1
{
	public class EnemySentry : Enemy
	{
		//Sprites and Graphics
		private Sprite exclaim = null;
		
		//Kenematics
		private double normalSpeed = 1;
		private double alertSpeed = 20;	
		private float targetX;
		private float targetY;
		private double distance;
		
		//Time Values
		private long elapsedTime = 0;
		private long initialWalkTime;
		private long waitTime = 0;
		private long currentTime;
		
		//States of Being (some overlap)
		private bool stopNormal = false;
		private bool stopAlert = false;
		private bool inPosition = false;
		private bool charge = false;
		
		//Textures
		private Texture2D exTex;
		private Texture2D normTex;
		
		//Used by the Spawner
		public EnemySentry (GraphicsContext tGraphics, float tX, float tY, Texture2D t1, Texture2D t3) : base  (tGraphics, tX, tY, t1)
		{
			direction = (float) (rand.Next (0, 360) * (Math.PI / 180));
			initialWalkTime = rand.Next(750, 4000);
			normTex = t1;
			exTex = t3;
			Points = 50;
			Esprite.Center = new Vector2(.5f, .5f);	
			Esprite.Rotation = (float) ((direction * (180 / Math.PI) + 90) * (Math.PI / 180));		
		}
		
		//Used by the Boss Enemy
		public EnemySentry (GraphicsContext tGraphics, float tX, float tY, Texture2D t1, Texture2D t3, int tDirection, long walkTime) : base (tGraphics, tX, tY, t1)
		{
			direction = (float) (tDirection * (Math.PI / 180));
			initialWalkTime = walkTime;
			normTex = t1;
			exTex = t3;
			Points = 50;
			Esprite.Center = new Vector2(.5f, .5f);
			Esprite.Rotation = (float) (tDirection + 90 * (Math.PI / 180));
		}
		
		//Guards an area until the player comes in range, then charges at it very quickly, and waits (catches it's breath) before charging again
		public override void Update (float datX, float datY, long deltaTime)
		{
			elapsedTime += deltaTime;
			
			if (waitTime > 0 && elapsedTime - currentTime < waitTime)
				return;
			else 
			{
				if (stopNormal) //Sets charge target and removes exlaimation
				{
					stopNormal = false;
					exclaim = null;
					charge = true;
					waitTime = 0;
					targetX = datX + 18;
					targetY = datY + 18;
					direction = (float) (Math.Atan2 ((Esprite.Position.Y - targetY), (Esprite.Position.X - targetX)));
					Esprite.Rotation = (float) ((direction * (180 / Math.PI) - 90) * (Math.PI / 180));
				}
				if (stopAlert) //Turns the sentry back to standby mode
				{
					stopAlert = false;
					waitTime = 0;
					targetX = 0;
					targetY = 0;
				}

				//charges at target point, then waits 1.5 seconds
				if (charge) 
				{
					distance = Math.Sqrt(((Esprite.Position.X - targetX) * (Esprite.Position.X - targetX)) 
					                     + ((Esprite.Position.Y - targetY) * (Esprite.Position.Y - targetY)));
					
					//This makes sure the sentry wont walk around after it charges if it charges before it can stop the 1st time.
					inPosition = true;
					
					if (distance <= 21)
					{
						charge = false;
						stopAlert = true;
						currentTime = elapsedTime;
						waitTime = 1500;
						Esprite.Position.X = (float) targetX;
						Esprite.Position.Y = (float) targetY;
					}
					else
					{
						Esprite.Position.X += (float) -(alertSpeed * Math.Cos (direction));
						Esprite.Position.Y += (float) -(alertSpeed * Math.Sin (direction));
						
					}
				} 
				else
				{
					distance = Math.Sqrt (((Esprite.Position.X - datX) * (Esprite.Position.X - datX)) 
					                      + ((Esprite.Position.Y - datY) * (Esprite.Position.Y - datY)));
					
					//If a daring soul gets too close, then this enemy will alert and charge
					if (distance <= 225) 
					{
						stopNormal = true;
						Texture2D ex = exTex;
						
						exclaim = new Sprite(Egraphics, ex);
						exclaim.Position.X = (float) Esprite.Position.X - 8;
						exclaim.Position.Y = (float) Esprite.Position.Y - 40;
						
						currentTime = elapsedTime;
						waitTime = 300;
						
					} else //when first created, this allows the sentry to walk a set time AKA distance
						if (!inPosition && elapsedTime < initialWalkTime)
						{
							Esprite.Position.X += (float) (normalSpeed *  Math.Cos (direction));
							Esprite.Position.Y += (float) (normalSpeed *  Math.Sin (direction));
						} else
							inPosition = true;
				}
			}
			
			base.Update(Esprite.Position.X, Esprite.Position.Y);
		}

		public override void Render ()
		{
			Esprite.Render ();
			if (exclaim != null)
			{
				exclaim.Render ();
			}
		}
	}
}

