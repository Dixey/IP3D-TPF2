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
        public Vector3 position, direction, target, origin;

        public Bullet(GraphicsDevice device, ContentManager content, Tank tank)
        {
            worldMatrix = Matrix.Identity;
            effect = new BasicEffect(device);
            bullet = content.Load<Model>("bullet");

            position = tank.position + 2f * tank.direction;
            target = position + 10f * tank.direction;
            direction = target - position;
            direction.Normalize();
        }

        public void Update(GameTime gametime)
        {
            position += direction;
        }

        public void Draw(GraphicsDevice device, Camera camera)
        {
            effect.World = worldMatrix;
            effect.View = camera.viewMatrix;
            effect.Projection = camera.projectionMatrix;

            effect.CurrentTechnique.Passes[0].Apply();
        }
    }
}
