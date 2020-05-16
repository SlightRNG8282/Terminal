using System;
using System.Collections.Generic;
// Daniel Kelley
// Matthew Canada
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Input;
using Sce.PlayStation.Core.Audio;
using Sce.PlayStation.HighLevel.UI;
using System.IO;

using System.Diagnostics;

namespace Project1
{
	public class AppMain
	{
		//Sprites
		private static GraphicsContext graphics;
		private static Sprite portal;
		private static Sprite shine;
		private static Sprite flamethrower;
		private static Sprite lazer;
		private static Sprite pauseScr;
		private static Sprite startScr;
		private static Sprite directionsScr;
		private static Sprite endScr;
		private static Sprite highScr;
		private static Sprite enterScr;
		private static Rectangle flameBox;
		private static Rectangle lazerBox;
		private static Sprite tester; //*x for testing
		
		//"Home-Rolled" Static Objects
		private static Weapon wpn;
		private static Player player;
		
		//Clocks and Time
		private static Stopwatch clock;
		private static long startTime;
		private static long stopTime;
		private static long deltaTime;
		private static long gameTime;
		private static long currentChangeWeaponTime;
		private static long dumbElapsedTime;
		private static long lockElapsedTime;
		private static long sentryElapsedTime;
		private static long bossElapsedTime;
		
		//Spawn rates
		private static double DESpawnRate = 750; //750
		private static double LOSpawnRate = 3500; //3500
		private static double SESpawnRate = 5000; //5000
		private static double BESpawnRate = 32500; //32500
		
		//Lists
		private static List<Projectile> projectiles;
		private static List<Enemy> enemies;
		public static List<Sprite> testers;
		
		//GamePhases
		private static bool paused = false;
		private static bool exit = false;
		private static bool end = false;
		private static bool start = true;
		private static bool highscore = false;
		private static bool directions = false;
		private static bool enterscore = false;
		private static bool gotFlame = false;
		private static bool gotLazer = false;
		private static bool died = false;
		private static bool game = false;
		private static int level = 2;
		
		//Textures
		public static Texture2D bossTex;
		public static Texture2D dumbTex;
		public static Texture2D lockTex;
		public static Texture2D sent1Tex;
		public static Texture2D sent3Tex;
		public static Texture2D projBTex;
		public static Texture2D projFTex;
		public static Texture2D projLTex;
		public static Texture2D wpnBTex;
		public static Texture2D wpnFTex;
		public static Texture2D wpnLTex;
		
		//UISys
		private static Label label1;
		private static int initialL1FontSize;
		private static float initialL1Height;
		private static Label label2;
		private static int initialL2FontSize;
		private static float initialL2Height;
		private static float initialL2Y;
		private static Label label3;
		private static Label label4;
		private static Scene sceneGame;
		private static Scene sceneScores;
		private static Scene sceneHiScore;
		
		//Misc
		private static Random rand;
		private static int pauseFrameBuffer = 5;
		private static int menuBuffer = 10;
		public static double score = 0.0;
		private static double difficulty = 0;
		private static double hiScore;
		private static BgmPlayer bgmp;
		private static BgmPlayer bgmp1;
		private static bool gameMusic = false;
		
		//Constants
		private const long CHANGE_WEAPON_BUFFER = 3000;
		private const int MENU_INPUT_BUFFER = 35;
		
		
		/* Correctly Pauses on 'Z' and Exits on 'X' */
		public static void Main (string[] args)
		{
			Initialize ();

			while (!exit) 
			{
				var gamePadData = GamePad.GetData (0);
				
				startTime = clock.ElapsedMilliseconds;
				SystemEvents.CheckEvents ();
				
				Update ();
				Render ();
				
				stopTime = clock.ElapsedMilliseconds;
				deltaTime = stopTime - startTime;
				gameTime += deltaTime;
			}
		}
		
		private static void Initialize ()
		{
			graphics = new GraphicsContext ();
			
			//Initialize textures only once for their load
			bossTex = new Texture2D ("Application/Assets/Boss.png", false);
			dumbTex = new Texture2D ("Application/Assets/Fire_Ball.png", false);
			lockTex = new Texture2D ("Application/Assets/Copter.png", false);
			sent1Tex = new Texture2D ("Application/Assets/Sentry.png", false);
			sent3Tex = new Texture2D ("Application/Assets/sentryexclaimation.png", false);
			projBTex = new Texture2D ("Application/Assets/basicproj.png", false);
			projFTex = new Texture2D ("Application/Assets/flameproj.png", false);
			projLTex = new Texture2D ("Application/Assets/lazerproj.png", false);
			wpnBTex = new Texture2D ("Application/Assets/Pistol.png", false);
			wpnFTex = new Texture2D ("Application/Assets/Flamethrower2.png", false);
			wpnLTex = new Texture2D ("Application/Assets/LaserBeam.png", false);
			
			rand = new Random();
			
			//Screens
			startScr = new Sprite (graphics, new Texture2D ("Application/Assets/startscreen.png", false));
			directionsScr = new Sprite (graphics, new Texture2D ("Application/Assets/directionsscreen.png", false));
			endScr = new Sprite (graphics, new Texture2D ("Application/Assets/byescreen.png", false));
			highScr = new Sprite (graphics, new Texture2D ("Application/Assets/highscores.png", false));
			enterScr = new Sprite (graphics, new Texture2D ("Application/Assets/enterscorescreen.png", false));
					
			//Sounds
			Bgm bgm = new Bgm("/Application/Assets/intro.mp3");
			bgmp = bgm.CreatePlayer();
			bgmp.Loop = true;
			bgmp.Play();
			
			//Major components
			player = new Player(graphics);
			wpn = new WeaponBasic(graphics);
			enemies = new List<Enemy>();
			projectiles = new List<Projectile>();	
			testers = new List<Sprite>(); //*x for visually testing hitboxes
			
			//Create portal
			Texture2D t = new Texture2D("Application/Assets/portal.png", false);
			portal = new Sprite(graphics, t);
			portal.Position.X = (graphics.Screen.Rectangle.Width / 2)
				- (portal.Width / 2);
			portal.Position.Y = (graphics.Screen.Rectangle.Height / 2) 
				- (portal.Height / 2);
			
			//Create portal projectile shield
			t = new Texture2D("Application/Assets/shine.png", false);
			shine = new Sprite(graphics, t);
			shine.Position.X = (graphics.Screen.Rectangle.Width / 2)
				- (shine.Width / 2);
			shine.Position.Y = (graphics.Screen.Rectangle.Height / 2) 
				- (shine.Height / 2);
			
			//PausedScreen sprite setup
			pauseScr = new Sprite (graphics, new Texture2D ("Application/Assets/pausedscreen.png", false));
			pauseScr.SetColor (1f, 1f, 1f, .4f);
			
			//Add Item pickups
			flamethrower = new Sprite(graphics, wpnFTex);
			flamethrower.Position.X = (graphics.Screen.Rectangle.Width / 3)
				- (flamethrower.Width / 2);
			flamethrower.Position.Y = (graphics.Screen.Rectangle.Height / 3) 
				- (flamethrower.Height / 2);
			flameBox = new Rectangle (flamethrower.Position.X, flamethrower.Position.Y, flamethrower.Width, flamethrower.Height);
			
			lazer = new Sprite(graphics, wpnLTex);
			lazer.Position.X = (graphics.Screen.Rectangle.Width * 2 / 3) + rand.Next (-100, 101)
				- (lazer.Width / 2);
			lazer.Position.Y = (graphics.Screen.Rectangle.Height * 2 / 3) + rand.Next (100, 101)
				- (lazer.Height / 2);
			lazerBox = new Rectangle (lazer.Position.X, lazer.Position.Y, lazer.Width, lazer.Height);
			
			//Start clock
			clock = new Stopwatch();
			clock.Start();
			
			//Initialize UI
			UISystem.Initialize (graphics);
			sceneGame = new Scene ();
			
			label1 = new Label ();
			label1.X = 20;
			label1.Y = 10;
			label1.Width = 1000;
			label1.Text = "Score: " + score;
			initialL1FontSize = label1.Font.Size;
			initialL1Height = label1.Height;
			label1.TextColor = new UIColor(1f, .25f, .25f, 1f);
			sceneGame.RootWidget.AddChildLast (label1);
			
			label2 = new Label ();
			label2.X = 20;
			label2.Y = graphics.Screen.Height - 40;
			label2.Width = 800;
			label2.Text = "Health: " + player.Health;
			initialL2FontSize = label2.Font.Size;
			initialL2Height = label2.Height;
			initialL2Y = label2.Y;
			label2.TextColor = new UIColor(1f, .25f, .25f, 1f);
			sceneGame.RootWidget.AddChildLast (label2);
			
			sceneScores = new Scene ();
			
			label3 = new Label ();
			label3.X = 333;
			label3.Y = 400;
			label3.Width = 600;
			label3.Height += 35;
			
			try
			{
				StreamReader sr = new StreamReader("/Documents/highscores.txt");
				string tempS = sr.ReadLine();
				if (tempS.Length > 0)
				{
					hiScore = Double.Parse (tempS.Substring(3));
					label3.Text = tempS.Substring(0, 3) + ": " + hiScore;
				}
				else
					label3.Text = "No High Score, Yet!";
				sr.Close ();
			}
			catch (FileNotFoundException)
			{
				label3.Text = "File Not Found!";
			}
			
			label3.Font.Size += 30;
			label3.TextColor = new UIColor(0f, 0f, 0f, 1f);
			sceneScores.RootWidget.AddChildLast (label3);
			
			sceneHiScore = new Scene ();
			
			label4 = new Label ();
			label4.X = 550;
			label4.Y = 400;
			label4.Width = 500;
			label4.Height += 18;
			label4.Font.Size += 30;
			label4.Text = "A";
			label4.TextColor = new UIColor(0f, 0f, 0f, 1f);
			sceneHiScore.RootWidget.AddChildLast (label4);
			
			UISystem.SetScene (sceneGame, null);
			
			//*x tester = new Sprite (graphics, projBTex);
		}

		public static void Update ()
		{
			var gamePadData = GamePad.GetData (0);
			
			//tester.Position.X = dat.X;
			//tester.Position.Y = dat.Y;
			
			//Menu Handling
			if (start || directions || enterscore || highscore || end)
			{
				menuBuffer ++;
				if (start)
				{
					if((gamePadData.Buttons & GamePadButtons.Square) != 0//need != 0?
					   && menuBuffer >= MENU_INPUT_BUFFER)
					{
						start = false;
						directions = true;
						menuBuffer = 0;
					}
					if((gamePadData.Buttons & GamePadButtons.Cross) != 0
					   && menuBuffer >= MENU_INPUT_BUFFER)
					{
						start = false;
						game = true;
						
						menuBuffer = 0;
						score = 0;
						difficulty = 0;
						player = new Player (graphics);
						died = false;
						paused = false;
						for (int i = enemies.Count - 1; i >= 0; i--)
							enemies.RemoveAt (i);
						for (int i = projectiles.Count - 1; i >= 0; i--)
							projectiles.RemoveAt (i);
						gotFlame = false;
						gotLazer = false;
						wpn = new WeaponBasic(graphics);
						
						if (!gameMusic)
						{
							bgmp.Dispose();
							Bgm bgm1 = new Bgm("/Application/Assets/playstate.mp3");
							bgmp1 = bgm1.CreatePlayer();
							bgmp1.Loop = true;
							bgmp1.Play();
							gameMusic = true;
						}
						
					}
					if((gamePadData.Buttons & GamePadButtons.Circle) != 0
					   && menuBuffer >= MENU_INPUT_BUFFER)
					{
						start = false;
						highscore = true;
						menuBuffer = 0;
						UISystem.SetScene (sceneScores, null);
					}
					if((gamePadData.Buttons & GamePadButtons.Start) != 0
					   && menuBuffer >= MENU_INPUT_BUFFER)
					{
						start = false;
						end = true;
						menuBuffer = 0;
					}
				}
				if (directions)
				{
					if((gamePadData.Buttons & GamePadButtons.Square) != 0
					   && menuBuffer >= MENU_INPUT_BUFFER)
					{
						directions = false;
						start = true;
						menuBuffer = 0;
					}
				}
				if (highscore)
				{
					if((gamePadData.Buttons & GamePadButtons.Square) != 0
					   && menuBuffer >= MENU_INPUT_BUFFER)
					{
						highscore = false;
						start = true;
						menuBuffer = 0;
						UISystem.SetScene(sceneGame, null);
					}
					if((gamePadData.Buttons & GamePadButtons.Start) != 0
					   && menuBuffer >= MENU_INPUT_BUFFER)
					{
						highscore = false;
						end = true;
						menuBuffer = 0;
						UISystem.SetScene(sceneGame, null);
					}
				}
				if (enterscore)
				{
					if((gamePadData.Buttons & GamePadButtons.Up) != 0
					   && menuBuffer >= MENU_INPUT_BUFFER - 22)
					{
						if (label4.Text.Length == 1)
						{
							if (!label4.Text.Equals("z", StringComparison.OrdinalIgnoreCase))//look into why the oridinal ignore case
								label4.Text = ((char) (label4.Text.ToCharArray()[0] + 1)).ToString().ToUpper();
							else
								label4.Text = "A";
						} else
						if (label4.Text.Length == 2)
						{
							if (!label4.Text.Substring(1).Equals("z", StringComparison.OrdinalIgnoreCase))
								label4.Text = label4.Text.Substring(0, 1) + ((char) (label4.Text.ToCharArray()[1] + 1)).ToString().ToUpper();
							else
								label4.Text = label4.Text.Substring(0, 1) + "A";
						} else
						if (label4.Text.Length == 3)
						{
							if (!label4.Text.Substring(2).Equals("z", StringComparison.OrdinalIgnoreCase))
								label4.Text = label4.Text.Substring(0, 2) + ((char) (label4.Text.ToCharArray()[2] + 1)).ToString().ToUpper();
							else
								label4.Text = label4.Text.Substring(0, 2) + "A";
						}
						menuBuffer = 0;
					}
					if((gamePadData.Buttons & GamePadButtons.Down) != 0
					   && menuBuffer >= MENU_INPUT_BUFFER - 22)
					{
						if (label4.Text.Length == 1)
						{
							if (!label4.Text.Equals("a", StringComparison.OrdinalIgnoreCase))
								label4.Text = ((char) (label4.Text.ToCharArray()[0] - 1)).ToString().ToUpper();
							else
								label4.Text = "Z";
						}
						if (label4.Text.Length == 2)
						{
							if (!label4.Text.Substring(1).Equals("a", StringComparison.OrdinalIgnoreCase))
								label4.Text = label4.Text.Substring(0, 1) + ((char) (label4.Text.ToCharArray()[1] - 1)).ToString().ToUpper();
							else
								label4.Text = label4.Text.Substring(0, 1) + "Z";
						}
						if (label4.Text.Length == 3)
						{
							if (!label4.Text.Substring(2).Equals("a", StringComparison.OrdinalIgnoreCase))
								label4.Text = label4.Text.Substring(0, 2) + ((char) (label4.Text.ToCharArray()[2] - 1)).ToString().ToUpper();
							else
								label4.Text = label4.Text.Substring(0, 2) + "Z";
						}
						menuBuffer = 0;
					}
					if((gamePadData.Buttons & GamePadButtons.Cross) != 0
					   && menuBuffer >= MENU_INPUT_BUFFER - 20)
					{
						if (hiScore == (score - score % .01))
						{
							highscore = true;
							enterscore = false;
							UISystem.SetScene(sceneScores);
							
							try
							{
								StreamReader sr = new StreamReader("/Documents/highscores.txt");
								string tempS = sr.ReadLine();
								hiScore = Double.Parse (tempS.Substring(3));
								label3.Text = tempS.Substring(0, 3) + ": " + hiScore;
								sr.Close ();
							}
							catch (FileNotFoundException)
							{
								label3.Text = "File Not Found!";
							}
							
							label4.Text = "A";
							label4.Font.Size += 15;
						}
						if (label4.Text.Length < 3)
						{
							label4.Text += "A";
						} else
						{
							try
							{
								StreamWriter sw = new StreamWriter("/Documents/highscores.txt");
								sw.WriteLine(label4.Text + (score - score % .01));
								hiScore = score - score % .01;
								sw.Close();
								label4.Font.Size -= 15;
								label4.Text = "Entered! Press (s)";
							}
							catch (FileNotFoundException)
							{
								label4.Font.Size -= 10;
								label4.Text = "File not Found! Highscore not saved!";
							}
						}
						menuBuffer = 0;
					}
				}
				if (end)
				{
					if((gamePadData.Buttons & GamePadButtons.Start) != 0
					   && menuBuffer >= MENU_INPUT_BUFFER)
					{
						exit = true;
						menuBuffer = 0;
					}
				}
			} //end menu
			else
			{
				//Keeps you from pause spamming via 25-frame buffer
				pauseFrameBuffer++;
					
				if ((gamePadData.Buttons & GamePadButtons.Select) != 0 
				    && pauseFrameBuffer >= 25)
				{
					pauseFrameBuffer = 0;
					
					if (paused == false)
						paused = true;
					else
						paused = false;
				}
				
				if ((gamePadData.Buttons & GamePadButtons.Start) != 0)
					end = true;
				
				if (died && !paused)
					start = true;
				
				if (paused)
				{
					if (label1.Font.Size < initialL1FontSize + 30)//font constants
						label1.Font.Size += 3;
					
					if (label1.Height < initialL1Height + 18)
						label1.Height += 1.8f;
					
					if (label2.Font.Size < initialL2FontSize + 30)
						label2.Font.Size += 3;
					
					if (label2.Height < initialL2Height + 18)
						label2.Height += 1.8f;
					
					if (label2.Y > initialL2Y - 24)
						label2.Y -= 2.4f;
				}
					
				
				if (game && !paused)
				{
					//Add Time Score
					score += deltaTime / 500.0;
					//Increase Difficulty and recaculate spawn times
					difficulty += 0.5;
					if (DESpawnRate > 150)
						DESpawnRate = -0.2642 * difficulty + 755.9524;
					if (LOSpawnRate > 300)
						LOSpawnRate = -0.4628 * difficulty + 3000.5714;
					if (SESpawnRate > 300)
						SESpawnRate = -0.9358 * difficulty + 6907.1429;
					if (BESpawnRate > 12500)
						BESpawnRate = -0.9643 * difficulty + 34285.7143;
					
					
					//Checks for picking up the Flamethrower
					if (!gotFlame && distanceSquared(player.X + player.Area.Width / 2, flameBox.Position.X + flameBox.Width / 2,
						                    player.Y + player.Area.Height / 2, flameBox.Position.Y + flameBox.Height / 2)
						    				< Math.Pow (player.HitBox + flameBox.Height / 2, 2))
					{
						gotFlame = true;
						score += 1000;
					} //etc
					if (!gotLazer && distanceSquared(player.X + player.Area.Width / 2, lazerBox.Position.X + lazerBox.Width / 2,
						                    player.Y + player.Area.Height / 2, lazerBox.Position.Y + lazerBox.Height / 2)
						    				< Math.Pow (player.HitBox + lazerBox.Height / 2, 2))
					{
						gotLazer = true;
						score += 1000;
					}
					
					//Check for weapon change
					changeWeapons (gamePadData);
					
					//Spawn Enemies
					createEnemies();
					
					//Update Player
					player.Update(gamePadData, deltaTime);
					
					//Update Weapon in use
					wpn.Update(player.X, player.Y, (int) player.Direction, gamePadData, deltaTime);
					
					//This handles the creation of projectiles from the weapon.
					if (wpn.MakeShot && distanceSquared(player.X + player.Area.Width / 2, shine.Position.X + shine.Width / 2,
						                    player.Y + player.Area.Height / 2, shine.Position.Y + shine.Height / 2)
						    				> Math.Pow (player.HitBox + shine.Height / 2, 2))
					{
						if (wpn is WeaponBasic)
							projectiles.Add (new ProjectileBasic(graphics, wpn.X, wpn.Y, wpn.Direction, projBTex));
						if (wpn is WeaponFlame)
							projectiles.Add (new ProjectileFlame(graphics, wpn.X, wpn.Y, wpn.Direction, projFTex));
						if (wpn is WeaponLazer)
							projectiles.Add (new ProjectileLazer(graphics, wpn.X, wpn.Y, wpn.Direction, projLTex));
						
						wpn.MakeShot = false;
					}
					
					//Collision checking
					checkForCollisions();
					
					//Handle enemy updates and deletes
					updateLists();
					
					//Label updates and normalization
					label1.Text = "Score: " + (score - score % 1);
					
					if (label1.Font.Size > initialL1FontSize)
						label1.Font.Size -= 3;
					
					if (label1.Height > initialL1Height)
						label1.Height -= 1.8f;
					
					label2.Text = "Health: " + player.Health;
					
					if (label2.Font.Size > initialL2FontSize)
						label2.Font.Size -= 3;
					
					if (label2.Height > initialL2Height)
						label2.Height -= 1.8f;
					
					if (label2.Y < initialL2Y)
						label2.Y += 2.4f;
				}
				
				//When paused, show more detailed score
				if (paused)
					label1.Text = "Score: " + (score - score % .01);
				
				//End of Game Scenario
				if (player.Delete)
				{
					paused = true;
					if (score > hiScore)
					{
						enterscore = true;
						label1.Text = "Final Score: " + (score - score % .01);
						label2.Text = "You got the High Score! (z) to Enter Initials!";
						UISystem.SetScene (sceneHiScore, null);
					}
					else
					{
						died = true;
						label1.Text = "Final Score: " + (score - score % .01);
						label2.Text = "You have died! (z) to Main Menu.";
					}
				}
			}
		}
		
		private static void changeWeapons (GamePadData gamePadData)
		{
			if((gamePadData.Buttons & GamePadButtons.L) != 0
			   && gameTime - currentChangeWeaponTime > CHANGE_WEAPON_BUFFER)
			{
				currentChangeWeaponTime = gameTime;
				
				if (wpn is WeaponBasic)
				{
					if (gotFlame == true)
						wpn = new WeaponFlame(graphics, wpn.X, wpn.Y, wpnFTex);
					else
						if (gotLazer == true)
							wpn = new WeaponLazer(graphics, wpn.X, wpn.Y, wpnLTex);
				}
				else
					if (wpn is WeaponFlame)
					{	
						if (gotLazer == true)
							wpn = new WeaponLazer(graphics, wpn.X, wpn.Y, wpnLTex);
						else
							wpn = new WeaponBasic(graphics, wpn.X, wpn.Y, wpnBTex);	
					}
					else
						if (wpn is WeaponLazer)
							wpn = new WeaponBasic(graphics, wpn.X, wpn.Y, wpnBTex);
			}
			
			
		}
		
		private static void createEnemies ()
		{
			if (level == 1)
			{
				
			}
			if (level == 2)
			{
				dumbElapsedTime += deltaTime;
				lockElapsedTime += deltaTime;
				sentryElapsedTime += deltaTime;
				bossElapsedTime += deltaTime;
				
				if (bossElapsedTime > BESpawnRate)
				{
					enemies.Add (new EnemyBoss (graphics,
					                            (graphics.Screen.Rectangle.Width / 2), 
					                            (graphics.Screen.Rectangle.Height / 2),
					                            bossTex));
					bossElapsedTime -= (long) BESpawnRate;
				}
			
				if (dumbElapsedTime > DESpawnRate)
				{
					enemies.Add (new EnemyDumb(graphics, 
					                           (graphics.Screen.Rectangle.Width / 2), 
					                           (graphics.Screen.Rectangle.Height / 2),
					                           dumbTex));
					dumbElapsedTime -= (long) DESpawnRate;
				}
				
				if (lockElapsedTime > LOSpawnRate)
				{
					enemies.Add (new EnemyLockOn(graphics,
					                             (graphics.Screen.Rectangle.Width / 2) - 18,
					                             (graphics.Screen.Rectangle.Height / 2) - 18,
					                             lockTex));
					lockElapsedTime -= (long) LOSpawnRate;
				}
				
				if(sentryElapsedTime > SESpawnRate)
				{
					enemies.Add (new EnemySentry(graphics,
					                             (graphics.Screen.Rectangle.Width / 2),
					                             (graphics.Screen.Rectangle.Height / 2),
					                             sent1Tex, sent3Tex));
					sentryElapsedTime -= (long) SESpawnRate;
				}
			}		
			
		}
		
		//Breaks and continue keep each projectiles from killing more than one thing
		private static void checkForCollisions()
		{
			for (int p = 0; p < projectiles.Count; p++)
			{
				if (projectiles[p].Delete == false
				    && distanceSquared(projectiles[p].X + projectiles[p].Psprite.Width / 2, shine.Position.X + shine.Width / 2,
				                    projectiles[p].Y + projectiles[p].Psprite.Height / 2, shine.Position.Y + shine.Height / 2)
				    				< Math.Pow(projectiles[p].HitBox + (shine.Height / 2) + 3, 2))
				{
					projectiles[p].Delete = true;
					continue;
				}
				
				for (int e = 0; e < enemies.Count; e++)
				{	
					// First if statement makes sure that the enemy is not within the protective shine shield
					if (distanceSquared(enemies[e].X + enemies[e].Esprite.Width / 2, shine.Position.X + shine.Width / 2,
				                    enemies[e].Y + enemies[e].Esprite.Height / 2, shine.Position.Y + shine.Height / 2)
				    				> Math.Pow(projectiles[p].HitBox + (shine.Height / 2) - 20, 2))
						if (projectiles[p] is ProjectileLazer)  //ProjectileLazer has to "go through" small enemies (non-boss enemies)
						{
							if (projectiles[p].Delete == false
							    && distanceSquared((projectiles[p] as ProjectileLazer).Cent.X, enemies[e].X + enemies[e].Esprite.Width / 2,
					                    	(projectiles[p] as ProjectileLazer).Cent.Y, enemies[e].Y + enemies[e].Esprite.Height / 2)
					    					< Math.Pow(projectiles[p].HitBox + enemies[e].HitBox, 2))
							{
								if (!(enemies[e] is EnemyBoss))
								{
									enemies[e].Delete = true;
									break;
								} else
								{
									projectiles[p].Delete = true;
									(enemies[e] as EnemyBoss).Health--;
									if ((enemies[e] as EnemyBoss).Health <= 0)
										enemies[e].Delete = true;
									break;
								}
							}
						}
					else //Now all other projectiles
						if (projectiles[p].Delete == false
							&& distanceSquared(projectiles[p].X + projectiles[p].Psprite.Width / 2, enemies[e].X + enemies[e].Esprite.Width / 2,
					                    projectiles[p].Y + projectiles[p].Psprite.Height / 2, enemies[e].Y + enemies[e].Esprite.Height / 2)
					    				< Math.Pow(projectiles[p].HitBox + enemies[e].HitBox, 2))
						{
							if (!(enemies[e] is EnemyBoss))
							{
								enemies[e].Delete = true;
								break;
							} else
							{
								projectiles[p].Delete = true;
								(enemies[e] as EnemyBoss).Health--;
								if ((enemies[e] as EnemyBoss).Health <= 0)
									enemies[e].Delete = true;
								break;
							}
						}
				}
			} 
			foreach (Enemy e in enemies) //Handle player damage
			{
				if (distanceSquared(e.X + e.Esprite.Width / 2, player.X + player.Area.Width / 2,
				                    e.Y + e.Esprite.Height / 2, player.Y + player.Area.Height / 2)
				    				< Math.Pow(player.HitBox - 8 + e.HitBox, 2))
				{
					e.Delete = true;
					player.Health = player.Health - 1;
					score -= e.Points;
					if (player.Health <= 0)
						player.Delete = true;
				}
			}
		}
		
		private static float distanceSquared(float x1, float x2, float y1, float y2)
		{
			return ((x2 - x1) * (x2 - x1)) + ((y2 - y1) * (y2 - y1));
		}
		
		private static void updateLists ()
		{		
			for (int i = enemies.Count - 1; i >= 0; i--)
			{
				if (!enemies[i].Delete == true)
				{
					if (enemies[i] is EnemyBoss)
					{
						enemies[i].Update (deltaTime, enemies);
					}
					if (enemies[i] is EnemyDumb)
					{
						enemies[i].Update ();
					}
					if (enemies[i] is EnemyLockOn)
					{
						enemies[i].Update (player.X, player.Y);
					}
					if (enemies[i] is EnemySentry)
					{
						enemies[i].Update (player.X, player.Y, deltaTime);
					}
				}
				else
				{
					//Add enemy score
					score += enemies[i].Points;
					enemies.RemoveAt (i);
				}
			}
			
			for (int i = projectiles.Count - 1; i >= 0 ; i--)
			{
				if (!projectiles[i].Delete)
				{
					if (projectiles[i] is ProjectileLazer)
					{
						projectiles[i].Update (deltaTime, projectiles);
					}
					else
						projectiles[i].Update ();
				} 
				else
					projectiles.RemoveAt (i);
			}
		}

		public static void Render ()
		{
			graphics.SetClearColor (0.0f, 0.0f, 0.0f, 0.0f);
			graphics.Clear ();
			
			if (start || directions || highscore || enterscore || end)
			{
				if (start)
				{
					startScr.Render();
				}
				if (directions)
				{
					directionsScr.Render();
				}
				if (highscore)
				{
					highScr.Render();
					UISystem.Render();
				}
				if (enterscore)
				{
					enterScr.Render();
					UISystem.Render();
				}
				if (end)
				{
					endScr.Render();
				}
			}
			else
			{
				shine.Render();
				portal.Render ();
				if (!gotLazer)
					lazer.Render();
				if (!gotFlame)
					flamethrower.Render();
				renderLists ();
				player.Render ();
				wpn.Render ();
				//tester.Render (); //*x
				if (paused)
				{
					pauseScr.Render ();
				}
				UISystem.Render ();
			}
			
			graphics.SwapBuffers ();
		}
		
		private static void renderLists ()
		{
			foreach (Enemy e in enemies)
			{
				e.Render ();
			}
			
			foreach (Projectile p in projectiles)
			{
				p.Render ();
			}
			
			foreach (Sprite s in testers)
				s.Render();
		}
	}
}

// Music from:
// http://downloads.khinsider.com/game-soundtracks/album/deus-ex-game-of-the-year-edition-soundtrack/01-main-title.mp3