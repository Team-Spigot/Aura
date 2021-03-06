using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using VoidEngine.Helpers;
using VoidEngine.VGame;

namespace Aura
{
	public class Player : Sprite
	{
		/// <summary>
		/// The mana struct for the player class.
		/// </summary>
		public struct Mana
		{
			/// <summary>
			/// The amount of mana the player has.
			/// </summary>
			public float mana;
			/// <summary>
			/// The maxium mana the player can have.
			/// </summary>
			public float maxMana;
			/// <summary>
			/// The ammount of time the mana recharges after.
			/// </summary>
			public float manaRechargeTime;
			/// <summary>
			/// The ammount of time the mana takes to charge at.
			/// </summary>
			public float manaInterval;

			/// <summary>
			/// Creates the Mana struct.
			/// </summary>
			/// <param name="maxMana">The maxium ammount of mana.</param>
			/// <param name="manaRechargeTime">The ammout of time the mana recharges after.</param>
			/// <param name="manaInterval">The ammount of time the mana takes to charge at.</param>
			public Mana(float maxMana, float manaRechargeTime, float manaInterval)
			{
				this.mana = maxMana;
				this.maxMana = maxMana;
				this.manaRechargeTime = manaRechargeTime;
				this.manaInterval = manaInterval;
			}
		}

		/// <summary>
		/// Gets or sets the game that the player is running off of.
		/// </summary>
		public Game1 myGame;

		public bool Dead = false;
		public float Lives = 3;

		#region Movement and Collision
		/// <summary>
		/// The player's collisions.
		/// </summary>
		public Rectangle playerCollisions;
		/// <summary>
		/// Gets or sets if the player is jumping. (Used for player or enemy classes)
		/// </summary>
		public bool isJumping
		{
			get;
			protected set;
		}
		/// <summary>
		/// Gets or sets if the player is grounded.
		/// </summary>
		public bool isGrounded
		{
			get;
			protected set;
		}
		/// <summary>
		/// Gets or sets if the sprite is falling.
		/// </summary>
		public bool isFalling
		{
			get;
			protected set;
		}
		/// <summary>
		/// Gets or sets if the sprite can fall.
		/// </summary>
		public bool canFall
		{
			get;
			set;
		}
		/// <summary>
		/// Gets or sets the players gravity.
		/// </summary>
		public float GravityForce
		{
			get;
			protected set;
		}
		/// <summary>
		/// Gets or sets the default players gravity.
		/// </summary>
		public float DefaultGravityForce
		{
			get;
			set;
		}
		/// <summary>
		/// The center of the player;
		/// </summary>
		public Vector2 PositionCenter;
		#endregion

		#region Projectiles
		/// <summary>
		/// The mana for the player class.
		/// </summary>
		public Mana _Mana;
		/// <summary>
		/// Gets or sets the projectiles animation sets.
		/// </summary>
		protected List<Sprite.AnimationSet> ProjectileAnimationSet
		{
			get;
			set;
		}
		/// <summary>
		/// Gets or sets the list of projectiles.
		/// </summary>
		public List<Projectile> ProjectileList
		{
			get;
			protected set;
		}
		/// <summary>
		/// Gets or sets if the projectile list is created.
		/// </summary>
		public bool ProjectileListCreated
		{
			get;
			protected set;
		}
		/// <summary>
		/// Gets or sets if the player has shot.
		/// </summary>
		public bool HasShotProjectile
		{
			get;
			set;
		}
		/// <summary>
		/// Gets or sets if the player can shoot.
		/// </summary>
		public bool CanShootProjectile
		{
			get;
			set;
		}
		/// <summary>
		/// Gets or sets if a new projectile can be created.
		/// </summary>
		public bool CreateNewProjectile
		{
			get;
			protected set;
		}
		#endregion

		public List<Keys> MovementKeys;
		public float Speed;
		public Vector2 Direction;
		public bool isMoving;

		/// <summary>
		/// Creates the player.
		/// </summary>
		/// <param name="position">The position the player starts at.</param>
		/// <param name="movementKeys">The keys to control the player.</param>
		/// <param name="gravity">The gravity of tha player.</param>
		/// <param name="mana">The maximum mana for the players projectiles.</param>
		/// <param name="color">The color to mask the player sprite with.</param>
		/// <param name="animationSetList">The animation set list for the player.</param>
		/// <param name="game">The game that the player runs off of.</param>
		public Player(Vector2 position, List<Keys> movementKeys, float gravity, Mana mana, Color color, List<AnimationSet> animationSetList, Game1 game)
			: base(position, color, animationSetList)
		{
			myGame = game;
			Scale = 0.34f;

			#region Set Animation Factors
			Offset = new Vector2(20, 5);
			#endregion

			#region Set Projectile Factors
			ProjectileList = new List<Projectile>();
			ProjectileAnimationSet = new List<AnimationSet>();
			_Mana = mana;
			CanShootProjectile = true;
			CreateNewProjectile = true;
			#endregion

			#region Set Movement and Collision Factors
			MovementKeys = movementKeys;
			Speed = 2.6f;
			GravityForce = gravity;
			DefaultGravityForce = gravity;
			SetAnimation("IDLE");
			isFalling = true;
			canFall = true;
			playerCollisions = new Rectangle((int)Position.X, (int)Position.Y, 20, 43);
			#endregion
		}

		/// <summary>
		/// Creates the player class with the bare minimum.
		/// Used for making child class for the player.
		/// Projectile lists created already.
		/// </summary>
		/// <param name="position">The position to start the player at.</param>
		/// <param name="color">The color to mask the player with.</param>
		/// <param name="animationSetList">The animation set list for the player.</param>
		public Player(Vector2 position, Color color, List<AnimationSet> animationSetList)
			: base(position, color, animationSetList)
		{
			ProjectileList = new List<Projectile>();
			ProjectileAnimationSet = new List<AnimationSet>();
		}

		/// <summary>
		/// Updates the Player class
		/// </summary>
		/// <param name="gameTime">To keep track of run time.</param>
		public override void Update(GameTime gameTime)
		{
			#region Updating Player Collision Points.
			playerCollisions.X = (int)Position.X;
			playerCollisions.Y = (int)Position.Y;
			#endregion

			#region Movement
			#region Detect Keys
			InputMethod(MovementKeys);
			#endregion

			#region Detect Collision
			foreach (Rectangle r in myGame.gameManager.platformRectangles)
			{
				CheckCollision(playerCollisions, r);
			}
			foreach (Rectangle r in myGame.gameManager.mapSegments)
			{
				CheckCollision(playerCollisions, r);
			}
			if (myGame.gameManager.level != 7 && (playerCollisions.TouchLeftOf(myGame.gameManager.mapSegments[1]) || playerCollisions.TouchTopOf(myGame.gameManager.mapSegments[1]) || playerCollisions.TouchRightOf(myGame.gameManager.mapSegments[1]) || playerCollisions.TouchBottomOf(myGame.gameManager.mapSegments[1])))
			{
				myGame.gameManager.wonLevel = true;
				Position = Vector2.Zero;
			}
			if (Position.Y >= myGame.gameManager.camera.Position.Y + myGame.gameManager.camera.Size.Y)
			{
				Dead = true;
				Lives -= 1;
			}
			if (Lives <= 0)
			{
				myGame.SetCurrentLevel(Game1.GameLevels.LOSE);

				SetPosition(new Vector2(0, 0));
			}
			foreach (Enemy e in myGame.gameManager.cEnemyList)
			{
				if (playerCollisions.TouchLeftOf(e.playerCollisions) || playerCollisions.TouchTopOf(e.playerCollisions) || playerCollisions.TouchRightOf(e.playerCollisions) || playerCollisions.TouchBottomOf(e.playerCollisions))
				{
					Dead = true;
					Lives -= 1;
				}
			}
			foreach (Enemy e in myGame.gameManager.sEnemyList)
			{
				if (playerCollisions.TouchLeftOf(e.playerCollisions) || playerCollisions.TouchTopOf(e.playerCollisions) || playerCollisions.TouchRightOf(e.playerCollisions) || playerCollisions.TouchBottomOf(e.playerCollisions))
				{
					Dead = true;
					Lives -= 1;
				}
			}
			foreach (Enemy e in myGame.gameManager.tEnemyList)
			{
				if (playerCollisions.TouchLeftOf(e.playerCollisions) || playerCollisions.TouchTopOf(e.playerCollisions) || playerCollisions.TouchRightOf(e.playerCollisions) || playerCollisions.TouchBottomOf(e.playerCollisions))
				{
					Dead = true;
					Lives -= 1;
				}
			}
			if (myGame.gameManager.BossCreated && !myGame.gameManager.bhEnemy.Dead)
			{
				if (playerCollisions.TouchLeftOf(myGame.gameManager.bflEnemy.GetPlayerRectangles()) || playerCollisions.TouchTopOf(myGame.gameManager.bflEnemy.GetPlayerRectangles()) || playerCollisions.TouchRightOf(myGame.gameManager.bflEnemy.GetPlayerRectangles()) || playerCollisions.TouchBottomOf(myGame.gameManager.bflEnemy.GetPlayerRectangles()))
				{
					Dead = true;
					Lives -= 1;
				}
				if (playerCollisions.TouchLeftOf(myGame.gameManager.bfrEnemy.GetPlayerRectangles()) || playerCollisions.TouchTopOf(myGame.gameManager.bfrEnemy.GetPlayerRectangles()) || playerCollisions.TouchRightOf(myGame.gameManager.bfrEnemy.GetPlayerRectangles()) || playerCollisions.TouchBottomOf(myGame.gameManager.bfrEnemy.GetPlayerRectangles()))
				{
					Dead = true;
					Lives -= 1;
				}
			}
			if (Dead == true)
			{
				ProjectileList.RemoveRange(0, ProjectileList.Count);
			}
			Lives = MathHelper.Clamp(Lives, 0, 3);
			#endregion

			UpdateGravity();
			#endregion

			#region Do Projectiles
			if (!ProjectileListCreated)
			{
				ProjectileAnimationSet.Add(new AnimationSet("IDLE", myGame.gameManager.ProjectileTexture, new Point(25, 25), new Point(1, 1), new Point(0, 0), 0, true));

				if (ProjectileAnimationSet != null && ProjectileList != null)
				{
					ProjectileListCreated = true;
				}
			}

			foreach (Projectile p in ProjectileList)
			{
				p.Update(gameTime);
			}
			#endregion

			#region Do Animations
			if (isJumping || (isFalling && Direction.Y < DefaultGravityForce))
			{
				SetAnimation("JUMP");
			}
			if (CurrentAnimation.name == "IDLE")
			{
				Offset = new Vector2(10, 5);
			}
			if (CurrentAnimation.name == "SHOOT")
			{
				Offset = new Vector2(10, 5);
			}
			if (CurrentAnimation.name == "JUMP")
			{
				if (isFlipped)
				{
					Offset = new Vector2(15, 5);
				}
				else
				{
					Offset = new Vector2(25, 5);
				}
			}
			if (CurrentAnimation.name == "WALK")
			{
				if (isFlipped)
				{
					Offset = new Vector2(0, 5);
				}
				else
				{
					Offset = new Vector2(13, 5);
				}
			}
			#endregion

			#region Mana
			if (_Mana.mana < _Mana.maxMana)
			{
				if (!HasShotProjectile)
				{
					_Mana.manaRechargeTime -= myGame.elapsedTime;
				}

				if (_Mana.mana <= 0)
				{
					CanShootProjectile = false;
				}
				else if (_Mana.mana >= 0)
				{
					CanShootProjectile = true;
				}

				if (_Mana.manaRechargeTime <= 0 && _Mana.mana < _Mana.maxMana && !HasShotProjectile)
				{
					_Mana.manaInterval -= myGame.elapsedTime;

					if (_Mana.manaInterval <= 0)
					{
						_Mana.mana += 9.5f;
						_Mana.manaInterval = 500;
					}
				}

				if (_Mana.mana >= _Mana.maxMana || (HasShotProjectile && CanShootProjectile))
				{
					_Mana.manaRechargeTime = 5000;
				}

				if (_Mana.mana > _Mana.maxMana)
				{
					_Mana.mana = _Mana.maxMana;
				}
			}
			#endregion

			foreach (Projectile p in ProjectileList)
			{
				if (!p.visible)
				{
					CreateNewProjectile = true;
					ProjectileList.RemoveAt(0);
					break;
				}
			}

			base.Update(gameTime);

			Position += Direction;

			PositionCenter = new Vector2(playerCollisions.Width / 2, playerCollisions.Height / 2);
		}

		/// <summary>
		/// To draw the Player class.
		/// </summary>
		/// <param name="gameTime">To keep track of run time.</param>
		/// <param name="spriteBatch">The spriteBatch to draw with.</param>
		public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
		{
			foreach (Projectile p in ProjectileList)
			{
				p.Draw(gameTime, spriteBatch);
			}

			base.Draw(gameTime, spriteBatch);
		}

		/// <summary>
		/// Returns the player collision rectangle
		/// </summary>
		/// <returns>Rectangle</returns>
		public virtual Rectangle GetPlayerRectangles()
		{
			return playerCollisions;
		}

		/// <summary>
		/// Makes the player shoot a projectile.
		/// </summary>
		/// <param name="shootFactor">The number of projectiles the player can shoot.</param>
		public void ShootBeam(float shootFactor)
		{
			if (CreateNewProjectile && CanShootProjectile && _Mana.mana >= _Mana.maxMana / shootFactor - 1)
			{
				_Mana.mana -= shootFactor;
				Projectile projectile = new Projectile(new Vector2(Position.X - 5, Position.Y + 11), Color.White, ProjectileAnimationSet, myGame, this);
				ProjectileList.Add(projectile);
				myGame.gameManager.shootSFX.Play(1f, 0f, 0f);
				HasShotProjectile = true;
			}

			if (_Mana.mana < 0)
			{
				_Mana.mana = 0;
			}
		}

		/// <summary>
		/// Updates the inputs for the player class.
		/// </summary>
		/// <param name="keyList">The key list to update the input method with, (for custom lists)</param>
		protected virtual void InputMethod(List<Keys> keyList)
		{
			if ((myGame.keyboardState.IsKeyDown(keyList[4]) || myGame.keyboardState.IsKeyDown(keyList[1])) && (!isJumping && !canFall))
			{
				isJumping = true;
				//Position.Y -= GravityForce * 5.5f;
				Direction.Y = -DefaultGravityForce;
			}
			if (myGame.keyboardState.IsKeyDown(keyList[0]))
			{
				Direction.X = -Speed;
				SetAnimation("WALK");
				FlipSprite(Axis.Y);
				isMoving = true;
			}
			if (myGame.keyboardState.IsKeyDown(keyList[2]))
			{
				Direction.X = Speed;
				SetAnimation("WALK");
				FlipSprite(Axis.NONE);
				isMoving = true;
			}
			if (!myGame.keyboardState.IsKeyDown(keyList[0]) && !myGame.keyboardState.IsKeyDown(keyList[2]))
			{
				Direction.X = 0f;
				isMoving = false;

				if (isGrounded && (CurrentAnimation.name != "SHOOT"))
				{
					SetAnimation("IDLE");
				}
			}
			if (!myGame.CheckKey(keyList[5]))
			{
				HasShotProjectile = false;
			}
			if (myGame.CheckKey(MovementKeys[5]))
			{
				if (CanShootProjectile)
				{
					SetAnimation("SHOOT");
					ShootBeam(20);
				}
			}
		}

		/// <summary>
		/// To update the collision of the player against areas.
		/// </summary>
		/// <param name="rectangle1">Use this with only the players collision area.</param>
		/// <param name="rectangle2">Use this with what the player will collide with.</param>
		protected virtual void CheckCollision(Rectangle rectangle1, Rectangle rectangle2)
		{
			if (rectangle1.TouchTopOf(rectangle2))
			{
				position.Y = rectangle2.Top - rectangle1.Height - 2f;
				Direction.Y = 0f;
				isGrounded = true;
				canFall = false;
				GravityForce = DefaultGravityForce;
			}
			if (rectangle1.TouchLeftOf(rectangle2))
			{
				position.X = rectangle2.Left - rectangle1.Width - 2f;
			}
			if (rectangle1.TouchRightOf(rectangle2))
			{
				position.X = rectangle2.Right + 2f;
			}
			if (rectangle1.TouchBottomOf(rectangle2))
			{
				position.Y = rectangle1.Bottom - 25f;
				isJumping = false;
				canFall = true;
				isFalling = true;
			}
		}

		/// <summary>
		/// Updates gravity of the player.
		/// </summary>
		protected virtual void UpdateGravity()
		{
			if (isJumping)
			{
				if (GravityForce > -DefaultGravityForce)
				{
					Direction.Y -= GravityForce;
					GravityForce -= 0.07f;
				}
				if (GravityForce <= 0f)
				{
					isJumping = false;
					canFall = true;
				}
			}

			if (!isJumping)
			{
				Direction.Y += GravityForce;
				GravityForce += 0.10f;
			}

			Direction.Y = MathHelper.Clamp(Direction.Y, -GravityForce - 1f, GravityForce);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="newPosition"></param>
		public void SetPosition(Vector2 newPosition)
		{
			Position = newPosition;
		}
	}
}