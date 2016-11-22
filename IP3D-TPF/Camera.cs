#region
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace IP3D_TPF
{
    public enum CameraType
    {
        ThirdPerson,
        SurfaceFollow,
        Free
    }
    
    public class Camera
    {
        //Variáveis da classe Camera
        float yaw, pitch, pitchAnterior, aspectRatio, scale = 0f, speed = 0.1f;
        public Vector3 position, direction, target, thirdPersonReference;
        public Matrix viewMatrix, projectionMatrix, rotationMatrix;
        BasicEffect effect;
        public int idCamera = 0;

        public Camera(GraphicsDevice device, CameraType camera, Field field, Tank tank)
        {
            //guardar o aspectRatio do ecrã de jogo numa variável
            aspectRatio = (float)(device.Viewport.Width / device.Viewport.Height);

            //coordenadas da posição inicial da câmera
            position = new Vector3(10f, 10f, 50.0f);

            //escala a usar quando o utilizador roda o rato em qualquer direção
            scale = MathHelper.ToRadians(15) / 50;

            //valores iniciais do yaw e do pitch
            yaw = -0.5f;
            pitch = -0.4f;

            //características do effect
            effect = new BasicEffect(device);
            effect.LightingEnabled = false;
            effect.VertexColorEnabled = true;
        }

        public void Update(GraphicsDevice device, GameTime gameTime, Field field, Tank tank, CameraType c)
        {
            //variáveis para usar tanto o teclado como o rato
            KeyboardState keys = Keyboard.GetState();

            if(keys.IsKeyDown(Keys.F1))
            {
                c = CameraType.ThirdPerson;
                idCamera = 1;
            }

            if(keys.IsKeyDown(Keys.F12))
            {
                c = CameraType.SurfaceFollow;
                idCamera = 0;
            }

            if(keys.IsKeyDown(Keys.F11))
            {
                c = CameraType.Free;
                idCamera = 2;
            }

            MouseState mouse = Mouse.GetState();

            //variáveis da posição do rato, diferença e o centro do ecrã
            Vector2 posMouse, diference = new Vector2(0f, 0f), center = new Vector2(device.Viewport.Width / 2, device.Viewport.Height / 2);

            //posição inicial da direção
            direction = new Vector3(0f, 0f, -1f);

            //guardar as coordenadas da posição e do pitch noutras variáveis para depois poder não saírem dos limites
            float positionBackX = position.X;
            float positionBackZ = position.Z;
            float positionBackY = position.Y;
            pitchAnterior = pitch;

            //definição da posição do rato
            posMouse.X = mouse.X;
            posMouse.Y = mouse.Y;

            //igualar a diferença à subtração do centro do ecrã à posição do rato para saber quanto se moveu
            diference = posMouse - center;

            //yaw e pitch vão ser iguais à diferença calculada em cima vezes a variável escala
            yaw -= diference.X * scale;
            pitch += diference.Y * scale;

            //limites para que a camera não dê uma volta, pondo limites no pitch
            if((pitch < -1.5f) || (pitch > 0.5f))
            {
                pitch = pitchAnterior;
            }
            
            //definição da rotationMatrix através do yaw e do pitch
            rotationMatrix = Matrix.CreateFromYawPitchRoll(yaw, pitch, 0);

            //transformação da direção através da rotationMatrix
            direction = Vector3.Transform(direction, rotationMatrix);


            //Movimento da Câmera
            if (keys.IsKeyDown(Keys.NumPad8))
            {
                position += direction * speed;
            }

            if (keys.IsKeyDown(Keys.NumPad5))
            {
                position -= direction * speed;
            }

            if (keys.IsKeyDown(Keys.NumPad6))
            {
                position -= Vector3.Cross(Vector3.Up, direction) * speed;
            }

            if (keys.IsKeyDown(Keys.NumPad4))
            {
                position += Vector3.Cross(Vector3.Up, direction) * speed;
            }

            //Limitar a posição dentro dos limites do terreno
            //Usamos a mesma lógica do pitch, ou seja, guardamos a posição numa variável que guarda a posição anterior a essa
            if (position.X - 1 < 0)
            {
                position.X = positionBackX;
                position.Z = positionBackZ;
            }

            if (position.Z - 1 < 0)
            {
                position.X = positionBackX;
                position.Z = positionBackZ;
            }

            if (position.X + 1 > field.width)
            {
                position.X = positionBackX;
                position.Z = positionBackZ;
            }

            if (position.Z + 1 > field.height)
            {
                position.X = positionBackX;
                position.Z = positionBackZ;
            }

            //chamada da função SurfaceFollow
            target = position + direction;

            if(idCamera == 0)
            {
                position.Y = SurfaceFollow(position) + 2f;
                viewMatrix = Matrix.CreateLookAt(position, target, Vector3.Up);
            }

            if (idCamera == 1)
            {
                position = field.ThirdPersonCamera(tank.position, tank.direction);
                viewMatrix = Matrix.CreateLookAt(position, tank.position + direction, Vector3.Up);
            }

            if(idCamera == 2)
            {              
                viewMatrix = Matrix.CreateLookAt(position, target, Vector3.Up);
            }

            //definição da projectionMatrix
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45f), aspectRatio, 0.1f, 1000f);
        }

        //função utilizada para que a camera faça surface follow
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
                    float yAB = (1 - (position.X - vetor1.X)) * yA + (position.X - vetor1.X) * yB;
                    float yCD = (1 - (position.X - vetor3.X)) * yC + (position.X - vetor3.X) * yD;
                    float y = (1 - (position.Z - vetor1.Z)) * yAB + (position.Z - vetor1.Z) * yCD;

                    //queremos que a função nos retorne o y
                    return y;
                }
            }

            return 0;
        }

        public void FreeCamera()
        {
            position = new Vector3(100, 200, 100);
        }

       /* public void ThirdPersonCamera(Vector3 camPos, Tank tank, Vector3 postank)
        {
            thirdPersonReference = new Vector3(0, tank.position.Y + 10, -100);
            rotationMatrix = Matrix.CreateRotationY(tank.yaw);
            Vector3 tranformedReference = Vector3.Transform(thirdPersonReference, rotationMatrix);
            camPos = tranformedReference + postank;
            viewMatrix = Matrix.CreateLookAt(camPos, postank + rotationMatrix.Forward * 3, Vector3.Cross(rotationMatrix.Left, tranformedReference));
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45f), aspectRatio, 0.1f, 1000f);
        }*/

        
    }
}
