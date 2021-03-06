using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using VoidEngine.VGame;
using VoidEngine.VGUI;

namespace Aura
{
	/// <summary>
	/// This is a game component that implements IUpdateable.
	/// </summary>
	public class OptionsManager : Microsoft.Xna.Framework.DrawableGameComponent
	{
		SpriteBatch spriteBatch;
		Game1 myGame;

		Texture2D backgroundTexture;
		Texture2D buttonTexture;

		List<Sprite.AnimationSet> buttonAnimationSetList;
		List<Sprite.AnimationSet> infoButtonAnimationSetList;
		List<Sprite.AnimationSet> arrowButtonAnimationSetList;

		Button applyButton;
		Button cancelButton;

		public OptionsManager(Game1 game)
			: base(game)
		{
			myGame = game;

			Initialize();
		}

		public override void Initialize()
		{
			buttonAnimationSetList = new List<Sprite.AnimationSet>();
			infoButtonAnimationSetList = new List<Sprite.AnimationSet>();
			arrowButtonAnimationSetList = new List<Sprite.AnimationSet>();

			base.Initialize();
		}

		protected override void LoadContent()
		{
			spriteBatch = new SpriteBatch(Game.GraphicsDevice);
			backgroundTexture = Game.Content.Load<Texture2D>(@"images\screens\menu");
			buttonTexture = Game.Content.Load<Texture2D>(@"images\gui\button");

			buttonAnimationSetList.Add(new Sprite.AnimationSet("IDLE", buttonTexture, new Point(170, 46), new Point(1, 1), new Point(0, 0), 0, false));
			buttonAnimationSetList.Add(new Sprite.AnimationSet("HOVER", buttonTexture, new Point(170, 46), new Point(1, 1), new Point(170, 0), 0, false));
			buttonAnimationSetList.Add(new Sprite.AnimationSet("PRESSED", buttonTexture, new Point(170, 46), new Point(1, 1), new Point(340, 0), 0, false));

			applyButton = new Button(new Vector2((myGame.WindowSize.X / 2) - (buttonAnimationSetList[0].frameSize.X + 50), myGame.WindowSize.Y - buttonTexture.Height - 25), myGame.segoeUIRegular, 1f, Color.Black, "APPLY", Color.White, buttonAnimationSetList);

			cancelButton = new Button(new Vector2((myGame.WindowSize.X / 2) + 50, myGame.WindowSize.Y - buttonTexture.Height - 25), myGame.segoeUIRegular, 1f, Color.Black, "CANCEL", Color.White, buttonAnimationSetList);

			base.LoadContent();
		}

		public override void Update(GameTime gameTime)
		{
			applyButton.Update(gameTime);
			cancelButton.Update(gameTime);

			if (applyButton.Clicked())
			{
				myGame.SetCurrentLevel(Game1.GameLevels.MENU);
			}
			if (cancelButton.Clicked())
			{
				myGame.SetCurrentLevel(Game1.GameLevels.MENU);
			}

			base.Update(gameTime);
		}

		public override void Draw(GameTime gameTime)
		{
			spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointWrap, null, null);
			{
				spriteBatch.Draw(backgroundTexture, new Rectangle(0, 0, (int)myGame.WindowSize.X, (int)myGame.WindowSize.Y), Color.White);
			}
			spriteBatch.End();

			spriteBatch.Begin();
			{
				applyButton.Draw(gameTime, spriteBatch);
				cancelButton.Draw(gameTime, spriteBatch);
			}
			spriteBatch.End();

			base.Draw(gameTime);
		}
	}
}
