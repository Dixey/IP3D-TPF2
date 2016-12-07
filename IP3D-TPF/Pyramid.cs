using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace IP3D_TPF
{
    class ClsPyramidVB
    {
        BasicEffect effect;
        public Matrix worldMatrix;
        VertexPositionColor[] vertices;
        short[] indices;
        VertexBuffer vertexBuffer;
        IndexBuffer indexBuffer;

        public ClsPyramidVB(GraphicsDevice device, int nlados, float altura, float raio)
        {
            worldMatrix = Matrix.Identity;
            // Vamos usar um efeito básico
            effect = new BasicEffect(device);

            // Calcula a aspectRatio, a view matrix e a projeção
            float aspectRatio = (float)device.Viewport.Width / device.Viewport.Height;
            effect.View = Matrix.CreateLookAt(new Vector3(1f, 2.0f, 2.0f), Vector3.Zero, Vector3.Up);
            effect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f), aspectRatio, 1.0f, 10.0f);

            effect.LightingEnabled = false;
            effect.VertexColorEnabled = true;

            // Cria os eixos 3D
            CreateGeometry(nlados, altura, raio);
            vertexBuffer = new VertexBuffer(device, typeof(VertexPositionColor), vertices.Length, BufferUsage.None);
            vertexBuffer.SetData<VertexPositionColor>(vertices);
            indexBuffer = new IndexBuffer(device, typeof(short), indices.Length, BufferUsage.None);
            indexBuffer.SetData<short>(indices);
        }

        int vertexCount;
        int indexCount;

        private void CreateGeometry(int nlados, float altura, float raio)
        {

            vertexCount = nlados + 1;

            double angulo = 0;
            float x, z;
            vertices = new VertexPositionColor[vertexCount];
            indexCount = (nlados + 1) * 2;
            indices = new short[indexCount];

            for (int i = 0; i < nlados + 1; i++)
            {
                angulo = ((i * (Math.PI * 2)) / nlados);
                x = (float)(raio * (Math.Cos(angulo)));
                z = (float)(raio * (-Math.Sin(angulo)));
                vertices[i] = new VertexPositionColor(new Vector3(x, 0.0f, z), Color.Blue);

            }
            vertices[nlados] = new VertexPositionColor(new Vector3(0.0f, altura, 0.0f), Color.White);

            for (int j = 0; j < nlados + 1; j++)
            {
                indices[2 * j + 0] = (short)(j % nlados);
                indices[2 * j + 1] = (short)nlados;
            }

        }

        public void Update(Vector3 pos)
        {
            worldMatrix = Matrix.CreateTranslation(pos);
        }

        public void Draw(GraphicsDevice device, Camera camera)
        {
            // World Matrix
            effect.World = worldMatrix;

            effect.View = camera.viewMatrix;
            effect.Projection = camera.projectionMatrix;

            // Indica o efeito para desenhar os eixos
            effect.CurrentTechnique.Passes[0].Apply();
            device.SetVertexBuffer(vertexBuffer);
            device.Indices = indexBuffer;

            device.DrawIndexedPrimitives(PrimitiveType.TriangleStrip, 0, 0, vertexCount, 0, indexCount - 2);
        }
    }
}