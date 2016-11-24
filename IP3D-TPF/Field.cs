#region
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace IP3D_TPF
{
    public class Field
    {
        //Variáveis da classe Field
        BasicEffect effect;
        Matrix worldMatrix;
        public static VertexPositionNormalTexture[] vertices;
        private VertexBuffer vertexBuffer;
        private IndexBuffer indexBuffer;
        Texture2D heightMap, texture;
        Color[] heightMapColors;
        short[] indices;
        public int width, height;

        public Field(GraphicsDevice device, ContentManager content)
        {
            //definição da worldMatrix e load das duas texturas
            worldMatrix = Matrix.Identity;
            heightMap = content.Load<Texture2D>("field");
            texture = content.Load<Texture2D>("map");

            //inicialização do do array de cores
            heightMapColors = new Color[heightMap.Width * heightMap.Height];

            //associar esse array de cores à textura
            heightMap.GetData<Color>(heightMapColors);

            //chamada das funções
            setVertices();
            setIndices();

            //definição das variáveis width e height
            width = heightMap.Width;
            height = heightMap.Height;

            //definição do vertexBuffer
            vertexBuffer = new VertexBuffer(device, typeof(VertexPositionNormalTexture), vertices.Length, BufferUsage.None);
            vertexBuffer.SetData<VertexPositionNormalTexture>(vertices);

            //definição do indexBuffer
            indexBuffer = new IndexBuffer(device, typeof(short), indices.Length, BufferUsage.None);
            indexBuffer.SetData<short>(indices);

            //Usamos efeito básico
            effect = new BasicEffect(device);

            // Calcula a aspectRatio, a view matrix e a projeção
            float aspectRatio = (float)device.Viewport.Width / device.Viewport.Height;
            effect.View = Matrix.CreateLookAt(new Vector3(1f, 2.0f, 2.0f), Vector3.Zero, Vector3.Up);
            effect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f), aspectRatio, 1.0f, 10.0f);

            //Características do effect

            //Luzes
            effect.LightingEnabled = true;
            effect.AmbientLightColor = new Vector3(0.4f, 0.4f, 0.4f);
            effect.DirectionalLight0.Enabled = true;
            effect.DirectionalLight0.DiffuseColor = new Vector3(1.0f, 1.0f, 1.0f);
            effect.DirectionalLight0.Direction = new Vector3(1.0f, -1.0f, 0.0f);

            //Texturas
            effect.VertexColorEnabled = false;
            effect.TextureEnabled = true;
            effect.Texture = heightMap;
            effect.Texture = texture;
            
        }

        //função de desenho do terreno através do array vértices
        public void setVertices()
        {
            int contador = 0;
            Vector3 vector1, vector2, vector3, vector4, vector5, vector6, vector7, vector8;
            vertices = new VertexPositionNormalTexture[heightMap.Width * heightMap.Height];
            for (int z = 0; z < heightMap.Height; z++)
            {
                for (int x = 0; x < heightMap.Width; x++)
                {
                    vertices[contador] = new VertexPositionNormalTexture(new Vector3((float)x,
                            (float)(heightMapColors[x + z * heightMap.Width].R) / 20.0f,
                            (float)z), 
                            Vector3.Up,
                            new Vector2(x, z));
                    contador++;
                }
            }

            //for (int z = 0; z < heightMap.Height; z++)
            //{
            //    for (int x = 0; x < heightMap.Width; x++)
            //    {
            //        if ((x - 1) >= 0 && (z - 1) >= 0)
            //        {
            //            vector1 = vertices[(x - 1) + (z - 1) * heightMap.Width].Position - vertices[(x + z * heightMap.Width)].Position;
            //        }

            //        if ((x - 1) >= 0 && z >= 0)
            //        {
            //            vector2 = vertices[(x - 1) + z * heightMap.Width].Position - vertices[(x + z * heightMap.Width)].Position;
            //        }

            //        if ((x - 1) >= 0 && (z + 1) < 128)
            //        {
            //            vector3 = vertices[(x - 1) + (z + 1) * heightMap.Width].Position - vertices[(x + z * heightMap.Width)].Position;
            //        }

            //        if (x >= 0 && (z + 1) < 128)
            //        {
            //            vector4 = vertices[x + (z + 1) * heightMap.Width].Position - vertices[(x + z * heightMap.Width)].Position;
            //        }

            //        if ((x + 1 < 128) && (z + 1 < 128))
            //        {
            //            vector5 = vertices[(x + 1) + (z + 1) * heightMap.Width].Position - vertices[(x + z * heightMap.Width)].Position;
            //        }

            //        if ((x + 1 < 128) && (z < 128))
            //        {
            //            vector6 = vertices[(x + 1) + z * heightMap.Width].Position - vertices[(x + z * heightMap.Width)].Position;
            //        }

            //        if ((x < 128) && (z < 128))
            //        {
            //            vector7 = vertices[(x + z * heightMap.Width)].Position - vertices[(x + z * heightMap.Width)].Position;
            //        }

            //        if ((x + 1) < 128 && (z - 1 >= 0))
            //        {
            //            vector8 = vertices[(x + 1) + (z - 1) * heightMap.Width].Position - vertices[(x + z * heightMap.Width)].Position;
            //        }
            //    }
            //}

            //vetores à volta do vértice do meio
            for (int z = 1; z < heightMap.Height - 1; z++)
            {
                for (int x = 1; x < heightMap.Width - 1; x++)
                {
                    vector1 = vertices[(x - 1) + (z - 1) * heightMap.Width].Position - vertices[(x + z * heightMap.Width)].Position;
                    vector2 = vertices[(x - 1) + (z + 0) * heightMap.Width].Position - vertices[(x + z * heightMap.Width)].Position;
                    vector3 = vertices[(x - 1) + (z + 1) * heightMap.Width].Position - vertices[(x + z * heightMap.Width)].Position;
                    vector4 = vertices[(x + 0) + (z + 1) * heightMap.Width].Position - vertices[(x + z * heightMap.Width)].Position;
                    vector5 = vertices[(x + 1) + (z + 1) * heightMap.Width].Position - vertices[(x + z * heightMap.Width)].Position;
                    vector6 = vertices[(x + 1) + (z + 0) * heightMap.Width].Position - vertices[(x + z * heightMap.Width)].Position;                    
                    vector7 = vertices[(x + 1) + (z - 1) * heightMap.Width].Position - vertices[(x + z * heightMap.Width)].Position;
                    vector8 = vertices[(x + 0) + (z - 1) * heightMap.Width].Position - vertices[(x + z * heightMap.Width)].Position;

                    Vector3 n1 = Vector3.Cross(vector1, vector2);
                    n1.Normalize();
                    Vector3 n2 = Vector3.Cross(vector2, vector3);
                    n2.Normalize();
                    Vector3 n3 = Vector3.Cross(vector3, vector4);
                    n3.Normalize();
                    Vector3 n4 = Vector3.Cross(vector4, vector5);
                    n4.Normalize();
                    Vector3 n5 = Vector3.Cross(vector5, vector6);
                    n5.Normalize();
                    Vector3 n6 = Vector3.Cross(vector6, vector7);
                    n6.Normalize();
                    Vector3 n7 = Vector3.Cross(vector7, vector8);
                    n7.Normalize();
                    Vector3 n8 = Vector3.Cross(vector8, vector1);
                    n8.Normalize();

                    Vector3 n = (n1 + n2 + n3 + n4 + n5 + n6 + n7 + n8) / 8.0f;
                    n.Normalize();
                    vertices[(x + z * heightMap.Width)].Normal = n;
                }
            }

            //vetores da margem de cima
            for (int x = 1; x < heightMap.Width - 1; x++)
            {
                vector2 = vertices[(x - 1) + (0 + 0) * heightMap.Width].Position - vertices[(x + 0 * heightMap.Width)].Position;
                vector3 = vertices[(x - 1) + (0 + 1) * heightMap.Width].Position - vertices[(x + 0 * heightMap.Width)].Position;                              
                vector4 = vertices[(x + 0) + (0 + 1) * heightMap.Width].Position - vertices[(x + 0 * heightMap.Width)].Position;
                vector5 = vertices[(x + 1) + (0 + 1) * heightMap.Width].Position - vertices[(x + 0 * heightMap.Width)].Position;
                vector6 = vertices[(x + 1) + (0 + 0) * heightMap.Width].Position - vertices[(x + 0 * heightMap.Width)].Position;

                Vector3 n1 = Vector3.Cross(vector2, vector3);
                n1.Normalize();
                Vector3 n2 = Vector3.Cross(vector3, vector4);
                n2.Normalize();
                Vector3 n3 = Vector3.Cross(vector4, vector5);
                n3.Normalize();
                Vector3 n4 = Vector3.Cross(vector5, vector6);
                n4.Normalize();

                Vector3 n = (n1 + n2 + n3 + n4) / 4.0f;
                n.Normalize();
                vertices[(x + 0 * heightMap.Width)].Normal = n;
            }

            //vetores da margem da esquerda
            for (int z = 1; z < heightMap.Height - 1; z++)
            {
                vector3 = vertices[(0 - 1) + (z + 1) * heightMap.Width].Position - vertices[(0 + z * heightMap.Width)].Position;
                vector4 = vertices[(0 + 0) + (z + 1) * heightMap.Width].Position - vertices[(0 + z * heightMap.Width)].Position;
                vector5 = vertices[(0 + 1) + (z + 0) * heightMap.Width].Position - vertices[(0 + z * heightMap.Width)].Position;
                vector8 = vertices[(0 + 1) + (z - 1) * heightMap.Width].Position - vertices[(0 + z * heightMap.Width)].Position;
                vector7 = vertices[(0 + 0) + (z - 1) * heightMap.Width].Position - vertices[(0 + z * heightMap.Width)].Position;

                Vector3 n1 = Vector3.Cross(vector3, vector4);
                n1.Normalize();
                Vector3 n2 = Vector3.Cross(vector4, vector5);
                n2.Normalize();
                Vector3 n3 = Vector3.Cross(vector5, vector8);
                n3.Normalize();
                Vector3 n4 = Vector3.Cross(vector8, vector7);
                n4.Normalize();

                Vector3 n = (n1 + n2 + n3 + n4) / 4.0f;
                n.Normalize();
                vertices[(0 + z * heightMap.Width)].Normal = n;
            }

            //vetores da margem de baixo
            for (int x = 1; x < heightMap.Width - 1; x++)
            {
                vector2 = vertices[(x - 1) + (1 + 0) * heightMap.Width].Position - vertices[(x + 1 * heightMap.Width)].Position;
                vector3 = vertices[(x - 1) + (1 + 1) * heightMap.Width].Position - vertices[(x + 1 * heightMap.Width)].Position;
                vector4 = vertices[(x + 0) + (1 + 1) * heightMap.Width].Position - vertices[(x + 1 * heightMap.Width)].Position;
                vector5 = vertices[(x + 1) + (1 + 1) * heightMap.Width].Position - vertices[(x + 1 * heightMap.Width)].Position;
                vector6 = vertices[(x + 1) + (1 + 0) * heightMap.Width].Position - vertices[(x + 1 * heightMap.Width)].Position;

                Vector3 n1 = Vector3.Cross(vector2, vector3);
                n1.Normalize();
                Vector3 n2 = Vector3.Cross(vector3, vector4);
                n2.Normalize();
                Vector3 n3 = Vector3.Cross(vector4, vector5);
                n3.Normalize();
                Vector3 n4 = Vector3.Cross(vector5, vector6);
                n4.Normalize();

                Vector3 n = (n1 + n2 + n3 + n4) / 4.0f;
                n.Normalize();
                vertices[(x + 1 * heightMap.Width)].Normal = n;
            }

            //vetores da margem da direita
            for (int z = 1; z < heightMap.Height - 1; z++)
            {
                vector3 = vertices[(1 + 0) + (z + 1) * heightMap.Width].Position - vertices[(1 + z * heightMap.Width)].Position;
                vector4 = vertices[(1 - 1) + (z + 1) * heightMap.Width].Position - vertices[(1 + z * heightMap.Width)].Position;
                vector5 = vertices[(1 + 1) + (z + 0) * heightMap.Width].Position - vertices[(1 + z * heightMap.Width)].Position;
                vector8 = vertices[(1 + 1) + (z - 1) * heightMap.Width].Position - vertices[(1 + z * heightMap.Width)].Position;
                vector7 = vertices[(1 + 0) + (z - 1) * heightMap.Width].Position - vertices[(1 + z * heightMap.Width)].Position;

                Vector3 n1 = Vector3.Cross(vector3, vector4);
                n1.Normalize();
                Vector3 n2 = Vector3.Cross(vector4, vector5);
                n2.Normalize();
                Vector3 n3 = Vector3.Cross(vector5, vector8);
                n3.Normalize();
                Vector3 n4 = Vector3.Cross(vector8, vector7);
                n4.Normalize();

                Vector3 n = (n1 + n2 + n3 + n4) / 4.0f;
                n.Normalize();
                vertices[(1 + z * heightMap.Width)].Normal = n;
            }
        }

        //função dos indices do indexBuffer, usamos 6 vértices por ser triangle list
        public void setIndices()
        {
            indices = new short[6 * (heightMap.Width - 1) * (heightMap.Height - 1)];
            int contador = 0;
            for (int x = 0; x < heightMap.Width - 1; x++)
            {
                for (int y = 0; y < heightMap.Height - 1; y++)
                {
                    indices[contador] = (short)(x + y * heightMap.Width);
                    indices[contador + 1] = (short)(x + y * heightMap.Width + 1);
                    indices[contador + 2] = (short)(x + (y + 1) * heightMap.Width);
                    indices[contador + 3] = (short)(x + y * heightMap.Width + 1);
                    indices[contador + 4] = (short)(x + (y + 1) * heightMap.Width + 1);
                    indices[contador + 5] = (short)(x + (y + 1) * heightMap.Width);

                    contador += 6;
                }
            }
        }

        //função utilizada para que o tank faça surface follow
        public float SurfaceFollow(Vector3 pos)
        {
            //4 pontos e 3 vetores
            float yA, yB, yC, yD;
            Vector3 vetor2, vetor3, vetor4;

            vetor2 = Vector3.Zero;
            vetor3 = Vector3.Zero;
            vetor4 = Vector3.Zero;

            for (int i = 0; i < Field.vertices.Length; i++)
            {
                //definição do 4º vetor
                Vector3 vetor1 = Field.vertices[i].Position;
                if ((int)pos.X == vetor1.X && (int)pos.Z == vetor1.Z)
                {
                    vetor2 = Field.vertices[i + 1].Position;
                    vetor3 = Field.vertices[i + 128].Position;
                    vetor4 = Field.vertices[i + 128 + 1].Position;

                    //igualar os pontos definidos em cima aos Y's dos vetores
                    yA = vetor1.Y;
                    yB = vetor2.Y;
                    yC = vetor3.Y;
                    yD = vetor4.Y;

                    //interpolação bilinear através da posição, das alturas e dos vetores
                    float yAB = (1 - (pos.X - vetor1.X)) * yA + (pos.X - vetor1.X) * yB;
                    float yCD = (1 - (pos.X - vetor3.X)) * yC + (pos.X - vetor3.X) * yD;
                    float y = (1 - (pos.Z - vetor1.Z)) * yAB + (pos.Z - vetor1.Z) * yCD;

                    //queremos que a função nos retorne o y
                    return y;
                }
            }

            return 0;
        }

        //função utilizada para que o tank faça normal follow
        public Vector3 NormalFollow(Vector3 pos)
        {
            //4 pontos
            Vector3 yA, yB, yC, yD;

            for (int i = 0; i < Field.vertices.Length; i++)
            {
                //definição do 4º vetor
                Vector3 vetor1 = Field.vertices[i].Position;

                if ((int)pos.X == vetor1.X && (int)pos.Z == vetor1.Z)
                {
                    //igualar os pontos definidos em cima às normais dos vetores
                    yA = Field.vertices[i].Normal;
                    yB = Field.vertices[i + 1].Normal;
                    yC = Field.vertices[i + 128].Normal;
                    yD = Field.vertices[i + 128 + 1].Normal;

                    // interpolacao das normais
                    Vector3 yAB = (1 - (pos.X - vetor1.X)) * yA + (pos.X - vetor1.X) * yB;
                    Vector3 yCD = (1 - (pos.X - vetor1.X)) * yC + (pos.X - vetor1.X) * yD;
                    Vector3 normal = (1 - (pos.Z - vetor1.Z)) * yAB + (pos.Z - vetor1.Z) * yCD;

                    //queremos que a função nos retorne a normal
                    return normal;
                }
            }

            return Vector3.Zero;
        }

        /*public Vector3 NormalFollow(Vector3 pos)
        {
            //4 pontos
            Vector3 yA, yB, yC, yD;
            
            //definição do 4º vetor
            Vector3 vetor1 = Field.vertices[i].Position;

            if ((int)pos.X == vetor1.X && (int)pos.Z == vetor1.Z)
            {
                //igualar os pontos definidos em cima às normais dos vetores
                yA = Field.vertices[i].Normal;
                yB = Field.vertices[i + 1].Normal;
                yC = Field.vertices[i + 128].Normal;
                yD = Field.vertices[i + 128 + 1].Normal;

                // interpolacao das normais
                Vector3 yAB = (1 - (pos.X - vetor1.X)) * yA + (pos.X - vetor1.X) * yB;
                Vector3 yCD = (1 - (pos.X - vetor1.X)) * yC + (pos.X - vetor1.X) * yD;
                Vector3 normal = (1 - (pos.Z - vetor1.Z)) * yAB + (pos.Z - vetor1.Z) * yCD;

                //queremos que a função nos retorne a normal
                return normal;
            }

            return Vector3.Zero;
        }*/

        public Vector3 ThirdPersonCamera(Vector3 posTank, Vector3 direction)
        {
            Vector3 cameraPosition;
            Vector3 aux;

            cameraPosition = (posTank + 15f * direction);
            aux = vertices[(int)cameraPosition.X + (int)cameraPosition.Z * 128].Position;

            cameraPosition.Y = SurfaceFollow(aux) + 10f;
            return cameraPosition;
        }

        public void Draw(GraphicsDevice device, Camera camera)
        {
            //associar a view e a projection do effect à viewMatrix e projectionMatrix da câmera
            effect.World = worldMatrix;
            effect.View = camera.viewMatrix;
            effect.Projection = camera.projectionMatrix;

            // Indica o efeito para desenhar os eixos
            effect.CurrentTechnique.Passes[0].Apply();

            //dizer qual o vertexBuffer e indexBuffer a usar
            device.SetVertexBuffer(vertexBuffer);
            device.Indices = indexBuffer;            

            //fazer o draw através de TriangleList
            device.DrawUserIndexedPrimitives<VertexPositionNormalTexture>(PrimitiveType.TriangleList, vertices, 0, vertices.Length,
                indices, 0, indices.Length / 3);
        }
    }
}

