using System;
using System.Collections.Generic;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Input;

namespace Project1
{
	public class EnemyLockOn : Enemy
	{	
		//Kinematics
		private float direction;
		private float speed;
		private float targetX = 0;
		private float targetY = 0;
		
		//SpriteSheet
		private int frameTime = 0;
		private int activeFrame = 0;
		private const int FRAME_CELL_WIDTH = 35;
		private const int FRAME_CELL_HEIGHT = 35;
		private int FRAME_DURATION = 5;
		
		public EnemyLockOn (GraphicsContext tGraphics, float tX, float tY, Texture2D t) : base (tGraphics, tX, tY, t)
		{
			Points = 25;

			Esprite.Width = FRAME_CELL_WIDTH;
			Esprite.Height = FRAME_CELL_HEIGHT;
		}
		
		//Follows the Dat
		public override void Update (float datX, float datY)
		{
			targetX = datX + 2;
			targetY = datY - 1; //Hardcoded values provide good aesthetics
			
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
			
			direction =  (float) (Math.Atan2 ((Esprite.Position.Y - targetY), (Esprite.Position.X - targetX)) * 180 / Math.PI);
			
			float distance = (float) Math.Sqrt(((Esprite.Position.X - targetX) * (Esprite.Position.X - targetX)) 
			                            + ((Esprite.Position.Y - targetY) * (Esprite.Position.Y - targetY)));
			
			FRAME_DURATION = (int) (7 - (distance / 100));
			
			//This enemy moves faster the further it is from the Dat based on a formula			
			speed = 0.014429f * distance + .765000f;
			
			Esprite.Position.X -= (float) (speed *  Math.Cos (direction * Math.PI / 180));
			Esprite.Position.Y -= (float) (speed *  Math.Sin (direction * Math.PI / 180));
			
			base.Update(Esprite.Position.X, Esprite.Position.Y);
		}
		
		public override void Render()
		{
			Esprite.SetTextureCoord (FRAME_CELL_WIDTH * activeFrame, 0, 
			                     (FRAME_CELL_WIDTH * (activeFrame + 1)) - 1, FRAME_CELL_HEIGHT);
			base.Render();
		}
	}
}

