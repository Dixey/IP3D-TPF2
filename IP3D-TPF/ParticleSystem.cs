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

            effect.LightingEnabled = false;
            effect.VertexColorEnabled = true;
            effect.TextureEnabled = false;
            effect.LightingEnabled = false;

            position = tank.position - 2f * tank.direction;
            speed = 0.01f;
            pó = new VertexPositionColor[2];
            lifeTime = 0;
        }

        public void Update(GameTime gametime)
        {

        }

        public void Draw(GraphicsDevice device, Camera camera)
        {
            effect.World = worldMatrix;
            effect.View = camera.viewMatrix;
            effect.Projection = camera.projectionMatrix;
            effect.CurrentTechnique.Passes[0].Apply();

            device.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, pó, 0, 1);
        }
    }
}
