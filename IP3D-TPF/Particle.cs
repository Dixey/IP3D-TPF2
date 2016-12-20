using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IP3D_TPF
{
    class Particle
    {
        public Vector3 position;
        public Vector3 speed;
        public VertexPositionColor[] pó;
        public float lifeTime;

        public Particle(Vector3 pos, Vector3 sp)
        {
            this.position = pos;
            this.speed = sp;
            pó = new VertexPositionColor[2];
            lifeTime = 0;
        }
    }
}
