using System;
using System.Collections.Generic;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Input;

namespace Project1
{
	public class EnemyDumb : Enemy
	{		
		//Kinematics
		private float speed;
		
		//Misc
		private int maxRandSpeed = 20;
		
		//SpriteSheet
		private int frameTime = 0;
		private int activeFrame = 0;
		private const int FRAME_CELL_WIDTH = 35;
		private const int FRAME_CELL_HEIGHT = 35;
		private const int FRAME_DURATION = 5;
		
		//Used by Spawner
		public EnemyDumb (GraphicsContext tGraphics, float tX, float tY, Texture2D t) : base (tGraphics, tX, tY, t)
		{
			direction = rand.Next (0, 360);
			speed = rand.Next (3, maxRandSpeed);
			Points = 15;
			
			Esprite.Center = new Vector2 (.5f, .5f);
			Esprite.Rotation = (float) ((direction - 90) * Math.PI / 180.0);
			
			Esprite.Width = FRAME_CELL_WIDTH;
			Esprite.Height = FRAME_CELL_HEIGHT;
		}
		
		//Used by Boss Enemy
		public EnemyDumb (GraphicsContext tGraphics, float tX, float tY, Texture2D t, int tDirection, int tSpeed) : base (tGraphics, tX, tY, t)
		{
			direction = tDirection;
			speed = tSpeed;
			Points = 15;
			
			Esprite.Center = new Vector2 (.5f, .5f);
			Esprite.Rotation = (float) ((direction - 90) * Math.PI / 180.0);
			
			Esprite.Width = FRAME_CELL_WIDTH;
			Esprite.Height = FRAME_CELL_HEIGHT;
		}
		
		//Basic run-in-one-direction enemy
		public override void Update ()
		{			
			frameTime ++;
			
			if (frameTime > FRAME_DURATION)
			{
				frameTime = 0;
				if (activeFrame + 1 == 2)
					activeFrame = 0;
				else
				{
					activeFrame++;
				}
			}
			
			//direction += .5f;
			//Esprite.Rotation = (float) ((direction - 90) * Math.PI / 180.0);
			
			Esprite.Position.X += (float) (speed *  Math.Cos (Math.PI * direction / 180.0));
			Esprite.Position.Y += (float) (speed *  Math.Sin (Math.PI * direction / 180.0));
			
			base.Update(Esprite.Position.X, Esprite.Position.Y);
		}
		
		public override void Render ()
		{
			Esprite.SetTextureCoord (FRAME_CELL_WIDTH * activeFrame, 0, 
			                     (FRAME_CELL_WIDTH * (activeFrame + 1)) - 1, FRAME_CELL_HEIGHT);
			Esprite.SetColor (1f, 1f, 1f, .7f);
			base.Render();
		}
	}
}

