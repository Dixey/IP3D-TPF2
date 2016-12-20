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
        public Vector3 position, direction, target, gravity, speed;
        float scale, aceleration = 10f;
        public bool isShooting = false;

        public Bullet(GraphicsDevice device, ContentManager content, Tank tank)
        {
            worldMatrix = Matrix.Identity;
            effect = new BasicEffect(device);
            bullet = content.Load<Model>("bala");

            effect.VertexColorEnabled = true;
            effect.TextureEnabled = false;
            effect.LightingEnabled = false;

            scale = 0.20f;

            speed = Vector3.Zero;
            gravity = new Vector3(0f, -9.8f, 0f);
        }

        public void Initialize(Vector3 pos, Vector3 dir)
        {
            //target = position + 10f * tank.direction;
            //direction = target - position;
            //direction.Normalize();

            position = pos + new Vector3(0.0f, 2.0f, 0.0f);
            speed = dir * aceleration;
            
            isShooting = true;
        }

        public void Update(GameTime gametime)
        {
            //direction += gravity * (float)gametime.ElapsedGameTime.TotalSeconds;
            speed += gravity * (float)gametime.ElapsedGameTime.TotalSeconds;
            position += speed * (float)gametime.ElapsedGameTime.TotalSeconds;

            if (position.Y < 0)
            {
                isShooting = false;
            }
        }

        public void Draw(GraphicsDevice device, Camera camera)
        {
            if (isShooting == true)
            {
                worldMatrix = Matrix.CreateScale(scale) * Matrix.CreateTranslation(position);
                foreach (ModelMesh mesh1 in bullet.Meshes)
                {
                    foreach (BasicEffect effect1 in mesh1.Effects)
                    {
                        effect1.World = worldMatrix;
                        effect1.View = camera.viewMatrix;
                        effect1.Projection = camera.projectionMatrix;
                        effect1.EnableDefaultLighting();
                    }

                    mesh1.Draw();
                }
            }
        }
    }
}
