using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IP3D_TPF
{
    public class Bullet
    {
        Model bullet;
        Matrix worldMatrix;
        BasicEffect effect;
        public Vector3 position, direction, target, gravity;
        float scale;
        public bool isShooting = false;

        public Bullet(GraphicsDevice device, ContentManager content, Tank tank)
        {
            worldMatrix = Matrix.Identity;
            effect = new BasicEffect(device);
            bullet = content.Load<Model>("bala");

            effect.VertexColorEnabled = true;
            effect.TextureEnabled = false;
            effect.LightingEnabled = false;

            scale = 0.005f;

            gravity = new Vector3(0f, -9.8f, 0f);

            position = tank.position + 2f * tank.direction;
            target = position + 10f * tank.direction;
            direction = target - position;
            direction.Normalize();
        }

        public void Update(GameTime gametime)
        {
            isShooting = true;
            position += direction * (float)gametime.ElapsedGameTime.TotalSeconds;
            direction += gravity * (float)gametime.ElapsedGameTime.TotalSeconds;

            if(position.Y < 0)
            {
                isShooting = false;
            }
        }

        public void Draw(GraphicsDevice device, Camera camera)
        {
            //if (isShooting == true)
            //{
                bullet.Root.Transform = Matrix.CreateScale(scale) * Matrix.CreateTranslation(position);

                foreach (ModelMesh mesh in bullet.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.World = worldMatrix;
                        effect.View = camera.viewMatrix;
                        effect.Projection = camera.projectionMatrix;
                        effect.EnableDefaultLighting();
                    }

                    mesh.Draw();
                }
            //}
        }
    }
}
