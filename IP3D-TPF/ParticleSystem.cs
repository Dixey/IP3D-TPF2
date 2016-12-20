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
        List<Particle> poeira;
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


        }

        public void CreateParticles(Vector3 pos, Vector3 dir)
        {

        }

        public void Update(GameTime gametime)
        {

        }

        public void Draw(GraphicsDevice device, Camera camera)
        {
            foreach(Particle p in poeira)
            {
                effect.World = worldMatrix;
                effect.View = camera.viewMatrix;
                effect.Projection = camera.projectionMatrix;
                effect.CurrentTechnique.Passes[0].Apply();

                device.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, p.pó, 0, 1);
            }
        }
    }
}
