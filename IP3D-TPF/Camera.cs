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
            MouseState mouse = Mouse.GetState();

            if (keys.IsKeyDown(Keys.F1))
            {
                c = CameraType.ThirdPerson;
                idCamera = 1;
            }

            if(keys.IsKeyDown(Keys.F2))
            {
                c = CameraType.SurfaceFollow;
                idCamera = 0;
            }

            if(keys.IsKeyDown(Keys.F3))
            {
                c = CameraType.Free;
                idCamera = 2;
            }
           
            //variáveis da posição do rato, diferença e o centro do ecrã
            Vector2 posMouse, diference = new Vector2(0f, 0f), center = new Vector2(device.Viewport.Width / 2, device.Viewport.Height / 2);
            Vector3 n;
            //posição inicial da direção
            direction = new Vector3(0f, 0f, -1f);

            //guardar as coordenadas da posição e do pitch noutras variáveis para depois poder não saírem dos limites
            Vector3 positionBack = position;
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

            rotationMatrix = Matrix.CreateFromYawPitchRoll(yaw, pitch, 0);
            //transformação da direção através da rotationMatrix
            direction = Vector3.Transform(direction, rotationMatrix);
            direction.Normalize();
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
                position = positionBack;
            }

            if (position.Z - 1 < 0)
            {
                position = positionBack;
            }

            if (position.X + 1 > field.width)
            {
                position = positionBack;
            }

            if (position.Z + 1 > field.height)
            {
                position = positionBack;
            }

            //chamada da função SurfaceFollow
            target = position + direction;

            if (idCamera == 0)
            {
                position.Y = field.SurfaceFollow(position) + 2f;
                target = position + direction;
                //definição da rotationMatrix através do yaw e do pitch

                viewMatrix = Matrix.CreateLookAt(position, target, Vector3.Up);
            }

            if (idCamera == 1)
            {
                //position = field.ThirdPersonCamera(tank.position, tank.direction);
                position = tank.position;
                direction = -tank.direction;
                n = field.NormalFollow(position);
                viewMatrix = Matrix.CreateLookAt(position - 10f * direction + n * 5f, position, Vector3.Up);
            }

            if(idCamera == 2)
            {
                //definição da rotationMatrix através do yaw e do pitch
                viewMatrix = Matrix.CreateLookAt(position, target, Vector3.Up);
            }

            //definição da projectionMatrix
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45f), aspectRatio, 0.1f, 1000f);
        }     
    }
}
