﻿using Microsoft.Xna.Framework;
using System.Collections.Generic;
using VoidEngine.VGame;
using VoidEngine.Helpers;

namespace Aura
{
	public class Enemy : Player
	{
		public enum MovementType
		{
			FLY,
			HORIZONTAL,
			BOUNCE,
			BOSSBOUNCE,
			BOSSHEAD,
			NONE
		}

		public int HP;

		public MovementType _MovementType;

		public Player _Player;

		public List<Rectangle> MapTiles;
		public List<Rectangle> MapSides;

		public bool DeleteMe = false;
		public bool MoveCircle = false;
		public bool DeleteCircle = false;

		public Enemy(Vector2 position, float gravity, MovementType movementType, Color color, List<AnimationSet> animationSetList, Player player, List<Rectangle> mapTiles, List<Rectangle> mapSides)
			: base(position, color, animationSetList)
		{
			SetAnimation("IDLE");

			if (movementType == MovementType.BOSSHEAD)
			{
				Lives = 30;
				SetAnimation("IDLE1");
			}
			else if (movementType == MovementType.FLY)
			{
				RotationCenter = new Vector2(animationSetList[0].frameSize.X / 2, animationSetList[0].frameSize.Y / 2);
				Offset = new Vector2(-(animationSetList[0].frameSize.X / 2), -(animationSetList[0].frameSize.Y / 2));
			}

			#region Reset Gravity
			DefaultGravityForce = gravity;
			GravityForce = gravity;
			isFalling = true;
			canFall = true;
			Direction = Vector2.Zero;
			#endregion

			#region Set default variables
			MapSides = mapSides;
			MapTiles = mapTiles;
			_Player = player;
			_MovementType = movementType;
			#endregion

			ProjectileList = new List<Projectile>();

			if (movementType != MovementType.HORIZONTAL)
			{
				playerCollisions = new Rectangle((int)Position.X, (int)Position.Y - 20, animationSetList[0].frameSize.X, animationSetList[0].frameSize.Y + 20);
			}
			else
			{
				playerCollisions = new Rectangle((int)Position.X, (int)Position.Y, animationSetList[0].frameSize.X, animationSetList[0].frameSize.Y);
			}
		}

		public override void Update(GameTime gameTime)
		{
			playerCollisions.X = (int)Position.X;
			playerCollisions.Y = (int)Position.Y;

			if (_MovementType == MovementType.FLY)
			{
				Direction = new Vector2((_Player.Position.X + (_Player.PositionCenter.X)) - (Position.X + (PositionCenter.X)), (_Player.Position.Y + (_Player.PositionCenter.Y)) - (Position.Y + (PositionCenter.Y)));
			}
			else if (_MovementType == MovementType.HORIZONTAL || _MovementType == MovementType.BOUNCE || _MovementType == MovementType.BOSSBOUNCE)
			{
				Direction.X = (_Player.Position.X + (_Player.PositionCenter.X)) - (Position.X + (PositionCenter.X));
			}
			else
			{
				Direction = Vector2.Zero;
			}

			if (_MovementType != MovementType.BOSSHEAD && CollisionHelper.Magnitude(Direction) <= 200)
			{
				if (_MovementType != MovementType.FLY)
				{
					Direction.X = CollisionHelper.UnitVector(Direction).X;
				}
				else
				{
					Direction = CollisionHelper.UnitVector(Direction);
				}

				if (_Player.Position.X == Position.X)
				{
					if (_MovementType == MovementType.FLY)
					{
						Direction = new Vector2(0, 0);
					}
					else
					{
						Direction.X = 0;
					}
				}

				if (_MovementType != MovementType.BOUNCE || _MovementType != MovementType.BOSSBOUNCE)
				{
					SetAnimation("CHASE");
				}

				Position += Direction;

				if (_MovementType == MovementType.FLY)
				{
					Rotation += 0.05f;
				}
			}
			else
			{
				if (_MovementType != MovementType.BOUNCE || _MovementType != MovementType.BOSSBOUNCE)
				{
					SetAnimation("IDLE");
				}
			}

			if (_MovementType != MovementType.BOSSBOUNCE)
			{
				foreach (Projectile p in _Player.ProjectileList)
				{
					if (playerCollisions.TouchLeftOf(p.projectileRectangle) || playerCollisions.TouchTopOf(p.projectileRectangle) || playerCollisions.TouchRightOf(p.projectileRectangle) || playerCollisions.TouchBottomOf(p.projectileRectangle))
					{
						DeleteMe = true;
					}
				}
			}
			else if (_MovementType == MovementType.BOSSHEAD)
			{
				foreach (Projectile p in _Player.ProjectileList)
				{
					if (playerCollisions.TouchLeftOf(p.projectileRectangle) || playerCollisions.TouchTopOf(p.projectileRectangle) || playerCollisions.TouchRightOf(p.projectileRectangle) || playerCollisions.TouchBottomOf(p.projectileRectangle))
					{
					}
				}
			}

			if (_MovementType == MovementType.BOSSHEAD)
			{
				if (Lives <= 27 && Lives > 18)
				{
					SetAnimation("IDLE1");
				}
				else if (Lives <= 18 && Lives > 9)
				{
					SetAnimation("IDLE2");
				}
				else if (Lives <= 9)
				{
					SetAnimation("IDLE3");
				}
			}

			if (Lives <= 0)
			{
				Dead = true;
			}

			foreach (Rectangle r in MapTiles)
			{
				CheckCollision(playerCollisions, r);
			}
			foreach (Rectangle r in MapSides)
			{
				CheckCollision(playerCollisions, r);
			}
			if (_MovementType == MovementType.BOUNCE || _MovementType == MovementType.BOSSBOUNCE)
			{
				if (!isJumping && !canFall)
				{
					isJumping = true;
					position.Y -= GravityForce * 1.03f;
					SetAnimation("CHASE");
				}
				if (canFall)
				{
					SetAnimation("FALLING");
				}
			}

			if (_MovementType != MovementType.FLY)
			{
				position.Y += Direction.Y;
			}

			UpdateGravity();

			LastFrameTime += gameTime.ElapsedGameTime.Milliseconds;

			if (LastFrameTime >= CurrentAnimation.framesPerMillisecond)
			{
				CurrentFrame.X++;

				if (CurrentFrame.X >= CurrentAnimation.sheetSize.X)
				{
					CurrentFrame.Y++;
					if (_MovementType != MovementType.BOUNCE)
					{
						CurrentFrame.X = 0;
					}
					else
					{
						CurrentFrame.X = CurrentAnimation.sheetSize.X;
					}

					if (CurrentFrame.Y >= CurrentAnimation.sheetSize.Y)
					{
						CurrentFrame.Y = 0;
					}
				}

				LastFrameTime = 0;
			}

			PositionCenter = new Vector2((CurrentAnimation.frameSize.X / 2), (CurrentAnimation.frameSize.Y / 2));
		}

		protected override void CheckCollision(Rectangle rectangle1, Rectangle rectangle2)
		{
			if (rectangle1.TouchTopOf(rectangle2))
			{
				position.Y = rectangle2.Top - rectangle1.Height - 2f;
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
				position.Y = rectangle1.Bottom - 21f;
				isJumping = false;
				canFall = true;
				isFalling = true;
			}
		}

		protected override void UpdateGravity()
		{
			base.UpdateGravity();
		}
	}
}