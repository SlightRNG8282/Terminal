using System;
using System.Collections.Generic;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Input;

namespace Project1
{
	public class Dat
	{
		//Sprite and Graphics
		private Sprite sprite;
		private GraphicsContext graphics;		
		private int activeFrame;
		private int xBuffer;
		private int yBuffer;
		
		//Kenematics
		private float tSpeed;
		private float xAccel;
		private float yAccel;
		
		//Timers
		private int frameTime;
		private long waitTime;
		private long elapsedTime;
		private long saveTime;
		
		//Directions and Enumeration
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
	 		
		private bool isEast = false;
		private bool isWest = false;
		private bool isNorth = true;
		private bool isSouth = false;
		private Dir direction = Dir.north;
		
		//Health/Death
		private int health = 5;
		private bool delete = false;
		
		//Constants
		private const int FRAME_CELL_WIDTH = 35;
		private const int FRAME_CELL_HEIGHT = 35;
		private const int FRAME_DURATION = 5;
		
		public Dat (GraphicsContext tGraphics)
		{
			graphics = tGraphics;
			Texture2D t = new Texture2D("Application/Assets/Hero_Char.png", false);
			sprite = new Sprite(graphics, t);
						
			sprite.Position.X = (graphics.Screen.Rectangle.Width / 2) - (FRAME_CELL_WIDTH / 2);
			sprite.Position.Y = (graphics.Screen.Rectangle.Height / 2) - (FRAME_CELL_HEIGHT / 2);	
			
			activeFrame = 0;
			
			// Had to edit Sprite.cs before this code would work
			sprite.Width = FRAME_CELL_WIDTH;
			sprite.Height = FRAME_CELL_HEIGHT;
			
			tSpeed = 1f;
		}
		
		public void Update (GamePadData gamePadData, long deltaTime)
		{	
			elapsedTime += deltaTime;
			
			if (activeFrame != 0 && (waitTime < elapsedTime - saveTime))
			{
				activeFrame = 0;
			}
				
			move (gamePadData);
			
			setDirection ();
			
			tSpeed = 1f;
		}
		
		/**Acceleration-Influenced Movement
		 * Opposite directions cancel each other out correctly 
		 * Handles sprite sheet frame changes for movement */
		private void move(GamePadData gamePadData)
		{	
			//Movement and Position calculations need to be shifted back to "actual" position
			sprite.Position.X -= xBuffer;
			sprite.Position.Y -= yBuffer;
			
			if(((gamePadData.Buttons & GamePadButtons.Square) != 0) 
			   || ((gamePadData.Buttons & GamePadButtons.Triangle) != 0)
			   || ((gamePadData.Buttons & GamePadButtons.Circle) != 0)
			   || ((gamePadData.Buttons & GamePadButtons.Cross) != 0))
			{
				//This handles the Spritesheet frame incrementation
				frameTime ++;
			
				if (frameTime > FRAME_DURATION)
				{
					frameTime = 0;
					if (activeFrame + 1 == 3)
						activeFrame = 0;
					else
					{
						activeFrame++;
						waitTime = 350;
						saveTime = elapsedTime;
					}
				}
				
				//Now we handle input for movement (WASD basic); 
				//tests to make sure only one direction in X or Y direction is being pressed
				if((gamePadData.Buttons & GamePadButtons.Square) != 0
				   && (gamePadData.Buttons & GamePadButtons.Circle) == 0)
				{
					isWest = true;
					//If it's not against a wall, add the speed to the acceleration
					if(sprite.Position.X + xAccel - tSpeed >= 0 
					   && sprite.Position.X + FRAME_CELL_WIDTH + xAccel - tSpeed <= graphics.Screen.Rectangle.Width)
					{
						xAccel -= tSpeed;
						sprite.Position.X += xAccel;
					}
					else
					{
						//So we're against a wall, so we test if we are against the left side, if not then we know we're against the right side.
						if (!(sprite.Position.X + xAccel - tSpeed >= 0))
							sprite.Position.X = 0;
						else
						{
							sprite.Position.X = graphics.Screen.Rectangle.Width - FRAME_CELL_WIDTH;
							
							/**Keeps sprite from getting stuck on the walls waiting for 
							   acceleration natural decay to pull xAccel down between -1 and 1. 
							   We only need to do this for one wall per direction we are moving in
							   because in this case we are trying to move LEFT but stuck on the RIGHT
							   wall which normally disables our ability to add more speed to the acceleration*/
							xAccel -= tSpeed / 2;
						}
					}
				}
				if((gamePadData.Buttons & GamePadButtons.Circle) != 0
				   && (gamePadData.Buttons & GamePadButtons.Square) == 0)
				{
					isEast = true;
					if(sprite.Position.X + xAccel + tSpeed >= 0 
					   && sprite.Position.X + FRAME_CELL_WIDTH + xAccel + tSpeed <= graphics.Screen.Rectangle.Width)
					{
						xAccel += tSpeed;
						sprite.Position.X += xAccel;
					}
					else
					{
						if (!(sprite.Position.X + xAccel + tSpeed >= 0))
						{
							sprite.Position.X = 0;
							xAccel += tSpeed / 2;
						}
						else
							sprite.Position.X = graphics.Screen.Rectangle.Width - FRAME_CELL_WIDTH;
					}
				}
				if((gamePadData.Buttons & GamePadButtons.Triangle) != 0
				   && (gamePadData.Buttons & GamePadButtons.Cross) == 0)
				{
					isNorth = true;
					if(sprite.Position.Y + yAccel - tSpeed >= 0 
					   && sprite.Position.Y + FRAME_CELL_HEIGHT + yAccel - tSpeed <= graphics.Screen.Rectangle.Height)
					{
						yAccel -= tSpeed;
						sprite.Position.Y += yAccel;
					}
					else
					{
						if (!(sprite.Position.Y + yAccel - tSpeed >= 0))
							sprite.Position.Y = 0;
						else
						{
							sprite.Position.Y = graphics.Screen.Rectangle.Height - FRAME_CELL_HEIGHT + 1;
							yAccel -= tSpeed / 2;
						}
					}
				}
				if((gamePadData.Buttons & GamePadButtons.Cross) != 0
				   && (gamePadData.Buttons & GamePadButtons.Triangle) == 0)
				{
					isSouth = true;
					if(sprite.Position.Y + yAccel + tSpeed >= 0 
						&& sprite.Position.Y + FRAME_CELL_HEIGHT + yAccel + tSpeed <= graphics.Screen.Rectangle.Height)
					{
						yAccel += tSpeed;
						sprite.Position.Y += yAccel;
					}
					else
					{
						if(!(sprite.Position.Y + yAccel + tSpeed >= 0))
						{
							sprite.Position.Y = 0;
							yAccel += tSpeed / 2;
						}
						else
							sprite.Position.Y = graphics.Screen.Rectangle.Height - FRAME_CELL_HEIGHT + 1;
					}
				}
				
				//Keeps diagonal speeds from going faster than cardinal speeds
				if (isEast && isSouth)
				{
					xAccel -= (tSpeed / 2);
					yAccel -= (tSpeed / 2);
				} else
				if (isEast && isNorth)
				{
					xAccel -= (tSpeed / 2);
					yAccel += (tSpeed / 2);
				} else
				if (isWest && isNorth)
				{
					xAccel += (tSpeed / 2);
					yAccel += (tSpeed / 2);
				} else
				if (isWest && isSouth)
				{
					xAccel += (tSpeed / 2);
					yAccel -= (tSpeed / 2);
				}
			}
			
			//This allows the player to Slide after gaining acceleration
			if (sprite.Position.X + xAccel >= 0 && sprite.Position.X + FRAME_CELL_WIDTH + xAccel <= graphics.Screen.Rectangle.Width)
			{
				if(!(isEast || isWest))
					sprite.Position.X += xAccel;
			}
			else
				if (!(sprite.Position.X + xAccel >= 0))
					sprite.Position.X = 0;
				else
					sprite.Position.X = graphics.Screen.Rectangle.Width - FRAME_CELL_WIDTH;
				
			if (sprite.Position.Y + yAccel >= 0 && sprite.Position.Y + FRAME_CELL_HEIGHT + yAccel <= graphics.Screen.Rectangle.Height)
			{
				if(!(isNorth || isSouth))
					sprite.Position.Y += yAccel;
			}
			else
				if(!(sprite.Position.Y + yAccel >= 0))
					sprite.Position.Y = 0;
				else
					sprite.Position.Y = graphics.Screen.Rectangle.Height - FRAME_CELL_HEIGHT + 1;

			
			//Acceleration Decay AND Speed limits (Hardcoded for now)
			if (xAccel > 6)
			{
				xAccel = 7f;
				xAccel -= tSpeed;
			}
			else
				if (xAccel > 4)
					xAccel -= tSpeed - .5f;
				else
					if (xAccel > 2)
						xAccel -= tSpeed - .75f;
					else
						if (xAccel > 0)		
							xAccel -= tSpeed - .9f;
			if (xAccel < -6)
			{
				xAccel = -7f;
				xAccel += tSpeed;
			}
			else
				if (xAccel < -4)
					xAccel += tSpeed - .5f;
				else
					if (xAccel < -2)
						xAccel += tSpeed - .75f;
					else
						if (xAccel < 0)
							xAccel += tSpeed - .9f;
			
			if (xAccel > -.15 && xAccel < .15)
				xAccel = 0;
			
			if (yAccel > 6)
			{
				yAccel = 7f;
				yAccel -= tSpeed;
			}
			else
				if (yAccel > 4)
					yAccel -= tSpeed - .5f;
				else
					if (yAccel > 2)
						yAccel -= tSpeed - .75f;
					else
						if (yAccel > 0)
							yAccel -= tSpeed - .9f;
			if (yAccel < -6)
			{
				yAccel = -7f;
				yAccel += tSpeed;
			}
			else
				if (yAccel < -4)
					yAccel += tSpeed - .5f;
				else
					if (yAccel < -2)
						yAccel += tSpeed - .75f;
					else
						if (yAccel < 0)
							yAccel += tSpeed - .90f;
			
			if (yAccel > -.15 && yAccel < .15)
				yAccel = 0;
			
			//Rotation Buffers added back after movement calculation
			sprite.Position.X += xBuffer;
			sprite.Position.Y += yBuffer;
		}
		
		//Bools instantiated from the input get converted to enumerated directions and saved in 'direction'
		private void setDirection()
		{
			if(isEast && !(isNorth || isSouth))
			{
				direction = Dir.east;
			} else
			if(isWest && !(isNorth || isSouth))
			{
				direction = Dir.west;
			} else
			if(isNorth && !(isEast || isWest))
			{
				direction = Dir.north;
			} else
			if(isSouth && !(isEast || isWest))
			{
				direction = Dir.south;
			} else
			if(isEast && isNorth)
			{
				direction = Dir.northeast;
			} else
			if(isWest && isNorth)
			{
				direction = Dir.northwest;
			} else
			if(isEast && isSouth)
			{
				direction = Dir.southeast;
			} else
			if(isWest && isSouth)
			{
				direction = Dir.southwest;
			}
			
			isEast = false;
			isWest = false;
			isNorth = false;
			isSouth = false;
		}
		
		//Nothing interesting here
		public void Render ()
		{
			setRotation();
			sprite.SetTextureCoord (FRAME_CELL_WIDTH * activeFrame, 0, 
			                     (FRAME_CELL_WIDTH * (activeFrame + 1)) - 1, FRAME_CELL_HEIGHT);
			sprite.Render();
		}
		
		// This is much less of a headache when I learned about Sprite.Center...  Left as is.
		// Rotates and updates the sprite position offset values for the player so that rotation occurs correctly.
		private void setRotation()
		{
			/**Strip current rotation buffers from position before changing their values
			   to get the "actual" position of the sprite */
			sprite.Position.X -= xBuffer;
			sprite.Position.Y -= yBuffer;
			
			if (direction == Dir.east)
			{
				sprite.Rotation = (float) (Math.PI);
				xBuffer = FRAME_CELL_WIDTH;
				yBuffer = FRAME_CELL_HEIGHT;
			} else
			if (direction == Dir.west)
			{
				sprite.Rotation = (float) (Math.PI / 180);
				xBuffer = 0;
				yBuffer = 0;
			} else
			if (direction == Dir.north)
			{
				sprite.Rotation = (float) (90 * Math.PI / 180);
				xBuffer = FRAME_CELL_WIDTH;
				yBuffer = 0;
			} else
			if (direction == Dir.south)
			{
				sprite.Rotation = (float) (270 * Math.PI / 180);
				xBuffer = 0;
				yBuffer = FRAME_CELL_HEIGHT;
			} else
			if (direction == Dir.northeast)
			{
				sprite.Rotation = (float) (135 * Math.PI / 180);
				xBuffer = (int) (FRAME_CELL_WIDTH * 1.25);
				yBuffer = (int) (FRAME_CELL_HEIGHT / 2);
			} else
			if (direction == Dir.northwest)
			{
				sprite.Rotation = (float) (45 * Math.PI / 180);
				xBuffer = (int) (FRAME_CELL_WIDTH / 2);
				yBuffer = (int) (-FRAME_CELL_HEIGHT * .25) ;
			} else
			if (direction == Dir.southwest)
			{
				sprite.Rotation = (float) (315 * Math.PI / 180);
				xBuffer = FRAME_CELL_WIDTH - 43;
				yBuffer = (int) (FRAME_CELL_HEIGHT / 2);
				
			} else
			if (direction == Dir.southeast)
			{
				sprite.Rotation = (float) (225 * Math.PI / 180);
				xBuffer = FRAME_CELL_WIDTH - 17;
				yBuffer = FRAME_CELL_HEIGHT + 7;
			}
			
			//Add the new rotation buffers to the original, unrotated position
			sprite.Position.X += xBuffer;
			sprite.Position.Y += yBuffer;
		}
		
		//X, Y return unrotated position values
		public float X
		{
			get { return sprite.Position.X - xBuffer; }
			set { sprite.Position.X = value; }
		}
		
		public float Y
		{
			get { return sprite.Position.Y - yBuffer; }
			set { sprite.Position.Y = value; }
		}
		
		public float bX
		{
			get { return sprite.Position.X; }
			set { sprite.Position.X = value; }
		}
		
		public float bY
		{
			get { return sprite.Position.Y; }
			set { sprite.Position.Y = value; }
		}
		
		public Dir Direction
		{
			get { return direction; }
		}
		
		public bool Delete
		{
			get { return delete; }
			set { delete = value; }
		}
		
		public int Health
		{
			get { return health; }
			set { health = value; }
		}
		
		//This isn't tested for rotation buffers... YET!
		public Rectangle Area
		{
			get { return new Rectangle(sprite.Position.X, sprite.Position.Y, sprite.Width, sprite.Height); }
		}
		
		public float HitBox
		{
			get { return sprite.Height / 2; }
		}
	}
}

