using System;
using System.Collections.Generic;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Input;

namespace Project1
{
	public class EnemyBoss : Enemy
	{		
		//Kenematics
		private float speed;
		
		//Time Calculations
		private long elapsedTime = 0;
		private long dumbRingRate = 2600;
		private long lockStreamRate = 400;
		private long sentryWallRate = 5177; // Weird numbers to offset with changeDirection timings
		private long changeDirectionRate = 2600; // DumbRing hides the new direction of the boss
		
		//Temporary Vars
		private EnemyLockOn temp;
		private EnemyDumb tmp;
		private EnemySentry t;
		
		//Misc
		private int healthPoints;
		
		//SpriteSheet
		private int frameTime = 0;
		private int activeFrame = 0;
		private const int FRAME_CELL_WIDTH = 45;
		private const int FRAME_CELL_HEIGHT = 45;
		private const int FRAME_DURATION = 5;

		public EnemyBoss (GraphicsContext tGraphics, float tX, float tY, Texture2D t) : base (tGraphics, tX, tY, t)
		{
			direction = rand.Next (0, 360);
			speed = 1.5f;
			Points = 1250;
			healthPoints = 10;
			
			Esprite.Center = new Vector2 (.5f, .5f);
			
			Esprite.Width = FRAME_CELL_WIDTH;
			Esprite.Height = FRAME_CELL_HEIGHT;
		}
		
		/** Creates other enemies in patterns: 
		 * 		a ring of Dumb Enemies, 
		 * 		a constant stream of LockOn Enemies,
		 * 		and a wall of Sentry Enemies. */
		public override void Update (long deltaTime, List<Enemy> e)
		{
			elapsedTime += deltaTime;
			
			frameTime ++;
			
			if (frameTime > FRAME_DURATION)
			{
				frameTime = 0;
				if (activeFrame + 1 == 3)
					activeFrame = 0;
				else
				{
					activeFrame++;
				}
			}
			
			move (deltaTime);
			
			if (elapsedTime % dumbRingRate < deltaTime)
				createDumbRing(e);
			
			//Constant stream of LockOnEnemies
			if (elapsedTime % lockStreamRate < deltaTime)
			{
				temp = new EnemyLockOn (Egraphics, Esprite.Position.X + Esprite.Width / 2 - 32,
				                        Esprite.Position.Y + Esprite.Height / 2 - 32,
				       					AppMain.lockTex);
				temp.Points = 0;
				e.Add (temp);
			}
			
			if (elapsedTime % sentryWallRate < deltaTime)
				createSentryWall(e);
			
			base.Update (Esprite.Position.X, Esprite.Position.Y);
		}
		
		//Changes direction based on changeDirectionRate and moves in a line otherwise.
		private void move (long deltaTime)
		{
			Esprite.Rotation += 3.14f / 200f;
			
			if (elapsedTime % changeDirectionRate < deltaTime)
				direction = (float) (((Math.Atan2 ((Esprite.Position.Y - 272), (Esprite.Position.X - 480))) * 180 / Math.PI) + rand.Next (0, 45));
			else
			{
				Esprite.Position.X -= (float) (speed * Math.Cos (Math.PI * direction / 180.0));
				Esprite.Position.Y -= (float) (speed * Math.Sin (Math.PI * direction / 180.0));
			}
		}
		
		/*  Creates a ring of DumbEnemies with the same random speed and at different intervals--
			between 4 and 24 DumbEnemies created*/
		private void createDumbRing (List<Enemy> e)
		{
			int speed = rand.Next (3, 6);
			int interval = rand.Next (1, 4) * 15;
			
			for (int i = 0; i < 360; i += interval)
			{
				tmp = new EnemyDumb (Egraphics,
				                       Esprite.Position.X + Esprite.Width / 2 - 32,
				                       Esprite.Position.Y + Esprite.Height / 2 - 32,
				                       AppMain.dumbTex, i, speed);
				tmp.Points = 0;
				e.Add (tmp);
			}
		}
		
		//Creates a line of Sentries out from the position of the Boss in the opposite direction
		private void createSentryWall (List<Enemy> e)
		{			
			for (int i = 0; i < 4; i++)
			{
				t = new EnemySentry (Egraphics,
				                       	Esprite.Position.X + Esprite.Width / 2 - 32,
				                      	Esprite.Position.Y + Esprite.Height / 2 - 32,
				                        AppMain.sent1Tex, AppMain.sent3Tex, (int) - direction, (long) (i * 500.0));
				t.Points = 0;
				e.Add (t);
			}
		}
		
		public override void Render ()
		{
			Esprite.SetTextureCoord (FRAME_CELL_WIDTH * activeFrame, 0, 
			                     (FRAME_CELL_WIDTH * (activeFrame + 1)) - 1, FRAME_CELL_HEIGHT);
			base.Render();
		}
		
		public int Health
		{
			get { return healthPoints; }
			set { healthPoints = value; }
		}
	}
}

