using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IP3D_TPF
{
    public class ParticleSystem
    {
        BasicEffect effect;
        Matrix worldMatrix;
        VertexPositionColor[] pó;
        Vector3 initialPosition, position, direction, center, gravity;
        Random random;
        int radius;
        float directionAngle, lifeTime, speed;

        public ParticleSystem(GraphicsDevice device, Random r, Tank tank)
        {
            effect = new BasicEffect(device);
            position = tank.position - 2f * tank.direction;
            speed = 0.01f;
            pó = new VertexPositionColor[2];
            lifeTime = 0;
        }

        public void Update(GameTime gametime)
        {

        }

        public void Draw(GraphicsDevice device)
        {

        }
    }
}
