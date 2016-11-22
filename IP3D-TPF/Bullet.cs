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
        BasicEffect effect;
        public Bullet(GraphicsDevice device)
        {
            effect = new BasicEffect(device);
        }
    }
}
