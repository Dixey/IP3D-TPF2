#region
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
#endregion

namespace IP3D_TPF
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Camera SurfaceFollowCamera, ThirdPersonCamera;
        Field field;
        Tank tank, enemyTank;
        Bullet bullet;
        Vector3 positionBack1 = Vector3.Zero;
        Vector3 positionBack2 = Vector3.Zero;

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

            if (Window != null)
                Mouse.SetPosition(Window.ClientBounds.Width / 2, Window.ClientBounds.Height / 2);

            field = new Field(GraphicsDevice, Content);
            tank = new Tank(GraphicsDevice, Content, ChooseTank.tank);
            enemyTank = new Tank(GraphicsDevice, Content, ChooseTank.enemyTank);

            SurfaceFollowCamera = new Camera(GraphicsDevice, CameraType.SurfaceFollow, field, tank);
            ThirdPersonCamera = new Camera(GraphicsDevice, CameraType.ThirdPerson, field, tank);
        }

        protected override void UnloadContent()
        {

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            bool colisão1 = false;
            bool colisão2 = false;
            float raiotank1 = 2f, raiotank2 = 2f;
            

            SurfaceFollowCamera.Update(GraphicsDevice, gameTime, field, tank, CameraType.SurfaceFollow);
            ThirdPersonCamera.Update(GraphicsDevice, gameTime, field, tank, CameraType.ThirdPerson);
            tank.Move(field, ChooseTank.tank, bullet, gameTime);
            enemyTank.Move(field, ChooseTank.enemyTank, bullet, gameTime);

            colisão1 = tank.Colisão(tank.position, enemyTank.position, raiotank1, raiotank2);
            colisão2 = tank.Colisão(enemyTank.position, tank.position, raiotank1, raiotank2);

            if(colisão1 == false)
            {
                positionBack1 = tank.position;
            }

            else if (colisão1 == true)
            {
                tank.position = positionBack1;
            }

            if (colisão2 == false)
            {
                positionBack2 = enemyTank.position;
            }

            else if (colisão2 == true)
            {
                enemyTank.position = positionBack2;
            }

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
            //bullet.Draw(GraphicsDevice, ThirdPersonCamera);
            base.Draw(gameTime);
        }
    }
}
