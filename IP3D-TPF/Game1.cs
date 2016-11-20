﻿#region
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
#endregion

namespace IP3D_TPF
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Camera SurfaceFollowCamera, ThirdPersonCamera;
        CameraType c;
        Field field;
        Tank tank, enemyTank;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            SurfaceFollowCamera = new Camera(GraphicsDevice, CameraType.SurfaceFollow, tank);
            ThirdPersonCamera = new Camera(GraphicsDevice, CameraType.ThirdPerson, tank);
            if (Window != null)
                Mouse.SetPosition(Window.ClientBounds.Width / 2, Window.ClientBounds.Height / 2);

            field = new Field(GraphicsDevice, Content);

            tank = new Tank(GraphicsDevice, Content, ChooseTank.tank);
            enemyTank = new Tank(GraphicsDevice, Content, ChooseTank.enemyTank);
        }

        protected override void UnloadContent()
        {

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            SurfaceFollowCamera.Update(GraphicsDevice, gameTime, field, c);

            tank.Move(field, ChooseTank.tank);
            enemyTank.Move(field, ChooseTank.enemyTank);

            if (Window != null)
                Mouse.SetPosition(Window.ClientBounds.Width / 2, Window.ClientBounds.Height / 2);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            field.Draw(GraphicsDevice, SurfaceFollowCamera);
            tank.Draw(SurfaceFollowCamera, field);
            enemyTank.Draw(SurfaceFollowCamera, field);
            base.Draw(gameTime);
        }
    }
}
