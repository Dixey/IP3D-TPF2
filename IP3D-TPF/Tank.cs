using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IP3D_TPF
{
    public enum ChooseTank
    {
        tank,
        enemyTank
    }

    public class Tank
    {
        BasicEffect effect;
        Model tankModel;

        // Bones da torre e canhão
        ModelBone turretBone;
        ModelBone cannonBone;
        ModelBone leftBackWheelBone, rightBackWheelBone, leftFrontWheelBone, rightFrontWheelBone;
        ModelBone leftSteerBone, rightSteerBone;
        Matrix leftBackWheelTransform, rightBackWheelTransform, leftFrontWheelTransform, rightFrontWheelTransform;
        Matrix leftSteerTransform, rightSteerTransform;

        // Transformações iniciais
        // (posicionar torre e canhão)
        Matrix cannonTransform;
        Matrix turretTransform;

        // Guarda todas as transformações
        Matrix[] boneTransforms;

        Matrix wordlMatrix, rotationMatrix, r;
        public float scale, aspectRatio, yaw, pitch, speed = 0.2f;
        float wheelRotationValue = 0f, steerRotationValue = 0f, turretRotationValue = 0f, cannonRotationValue = 0f;
        public Vector3 position, direction, d, n, right;
        BoundingSphere bsphere;
        List<Bullet> bullets;

        public Tank(GraphicsDevice device, ContentManager content, ChooseTank tank)
        {
            wordlMatrix = Matrix.Identity;
            r = Matrix.Identity;

            direction = new Vector3(0f, 0f, -1f);

            bullets = new List<Bullet>();

            tankModel = content.Load<Model>("tank");

            scale = 0.005f;

            if(tank == ChooseTank.tank)
            {
                position = new Vector3(15f, -10.0f, 40f);
            }

            if(tank == ChooseTank.enemyTank)
            {
                position = new Vector3(20f, -10f, 40f);
            }

            effect = new BasicEffect(device);

            // Calcula a aspectRatio, a view matrix e a projeção
            aspectRatio = (float)device.Viewport.Width / device.Viewport.Height;

            bsphere = new BoundingSphere(position, 2);

            // Lê os bones
            leftBackWheelBone = tankModel.Bones["l_back_wheel_geo"];
            rightBackWheelBone = tankModel.Bones["r_back_wheel_geo"];
            leftFrontWheelBone = tankModel.Bones["l_front_wheel_geo"];
            rightFrontWheelBone = tankModel.Bones["r_front_wheel_geo"];
            leftSteerBone = tankModel.Bones["l_steer_geo"];
            rightSteerBone = tankModel.Bones["r_steer_geo"];
            turretBone = tankModel.Bones["turret_geo"];
            cannonBone = tankModel.Bones["canon_geo"];

            // Lê as transformações iniciais dos bones
            leftBackWheelTransform = leftBackWheelBone.Transform;
            rightBackWheelTransform = rightBackWheelBone.Transform;
            leftFrontWheelTransform = leftFrontWheelBone.Transform;
            rightFrontWheelTransform = rightFrontWheelBone.Transform;

            leftSteerTransform = leftSteerBone.Transform;
            rightSteerTransform = rightSteerBone.Transform;

            turretTransform = turretBone.Transform;
            cannonTransform = cannonBone.Transform;

            // cria o array que armazenará as transformações em cascata dos bones
            boneTransforms = new Matrix[tankModel.Bones.Count];
        }

        public void Update(Field field, ChooseTank tank, Bullet bullet, GameTime gametime)
        {
            //posição inicial da direção
            direction = new Vector3(0f, 0f, -1f);

            float turnAngle = MathHelper.ToRadians(10f);

            Vector3 positionBack = position;

            KeyboardState keys = Keyboard.GetState();

            //Tank
            if (tank == ChooseTank.tank)
            {
                //movimento do tank e respetiva rotação das rodas
                if (keys.IsKeyDown(Keys.A))
                {
                    yaw += turnAngle * speed;

                    if (steerRotationValue < 0.5f)
                    {
                        steerRotationValue += 0.1f;
                    }

                }

                if (keys.IsKeyDown(Keys.D))
                {
                    yaw -= turnAngle * speed;

                    if (steerRotationValue > -0.5f)
                    {
                        steerRotationValue -= 0.1f;
                    }
                }

                if (keys.IsKeyUp(Keys.A) && keys.IsKeyUp(Keys.D))
                {
                    steerRotationValue = 0f;
                }

                //definição da rotationMatrix através do yaw e do pitch
                rotationMatrix = Matrix.CreateFromYawPitchRoll(yaw, pitch, 0);

                //transformação da direção através da rotationMatrix
                direction = Vector3.Transform(direction, rotationMatrix);

                if (keys.IsKeyDown(Keys.W))
                {
                    position -= direction * speed;

                    wheelRotationValue += 0.2f;
                }

                if (keys.IsKeyDown(Keys.S))
                {
                    position += direction * speed;

                    wheelRotationValue -= 0.2f;
                }

                if (keys.IsKeyDown(Keys.Space))
                {
                    bullets.Add(bullet);

                    bullet.Update(gametime);

                    if (bullet.position.Y < 0)
                    {
                        bullets.Remove(bullet);
                    }
                }

                //Limitar o movimento aos limites do terreno
                if (position.X - 1 < 0)
                {
                    position = positionBack;
                }

                if (position.Z - 1 < 0)
                {
                    position = positionBack;
                }

                if (position.X + 2f > field.width)
                {
                    position = positionBack;
                }

                if (position.Z + 2f > field.height)
                {
                    position = positionBack;
                }

                position.Y = field.SurfaceFollow(position) + 0.15f;

                //chamada da função NormalFollow
                n = field.NormalFollow(position);

                right = Vector3.Cross(direction, n);
                d = Vector3.Cross(n, right);

                r = Matrix.Identity;
                r.Forward = d;
                r.Up = n;
                r.Right = right;
            }
        }

        public bool Colisão(Vector3 pos1, Vector3 pos2, float raioTank1, float raioTank2)
        {
            bool colisão = false;
            float d;

            d = Vector3.Distance(pos1, pos2);

            if (d < raioTank1 + raioTank2)
                colisão = true;

            return colisão;
        }

        public void EnemyUpdate(Vector3 posPlayer, Vector3 directionPlayer, Field field, GameTime gametime)
        {
            Vector3 target, vseek, a, newSpeed;
            Vector3 positionBack = position;
            KeyboardState keys = Keyboard.GetState();
            bool isMoving = false, isTurning = false;
            int turnId;

            target = posPlayer + directionPlayer;
            vseek = target - position;
            vseek.Normalize();
            vseek = vseek * speed;

            //a = direction * speed - vseek;
            //a = vseek - direction * speed;
            //a.Normalize();

            //newSpeed = direction * speed + a * (float)gametime.ElapsedGameTime.TotalSeconds;

            //direction = newSpeed;
            //direction.Normalize();

            direction = Vector3.Normalize(vseek);

            position += direction * speed/4;

            if(position != Vector3.Zero)
            {
                isMoving = true;
            }

            if(direction != Vector3.Zero)
            {
                isTurning = true;
            }

            //Limitar o movimento aos limites do terreno
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

            if(isMoving == true)
            {
                wheelRotationValue += 0.2f;
            }

            if(isTurning == true)
            {
                if(direction.Z > 0)
                {
                    if (steerRotationValue < 0.5f)
                    {
                        steerRotationValue += 0.1f;
                    }

                    if (steerRotationValue > -0.5f)
                    {
                        steerRotationValue -= 0.1f;
                    }
                }
            }

            if(isTurning == false)
            {
                steerRotationValue = 0f;
            }

            position.Y = field.SurfaceFollow(position) + 0.15f;

            //chamada da função NormalFollow
            n = field.NormalFollow(position);

            right = Vector3.Cross(-direction, n);
            d = Vector3.Cross(n, right);

            r = Matrix.Identity;
            r.Forward = d;
            r.Up = n;
            r.Right = right;
        }

        public void Draw(Camera camera, Field field)
        {
            // Aplica uma transformação qualquer no bone Root, no canhão e na torre
            tankModel.Root.Transform = Matrix.CreateScale(scale) * r * Matrix.CreateTranslation(position);

            bsphere.Transform(tankModel.Root.Transform);

            Matrix wheelRotation = Matrix.CreateRotationX(wheelRotationValue);
            Matrix steerRotation = Matrix.CreateRotationY(steerRotationValue);
            Matrix turrentRotation = Matrix.CreateRotationX(turretRotationValue);
            Matrix cannonRotation = Matrix.CreateRotationY(cannonRotationValue);

            leftBackWheelBone.Transform = wheelRotation * leftBackWheelTransform;
            rightBackWheelBone.Transform = wheelRotation * rightBackWheelTransform;
            leftFrontWheelBone.Transform = wheelRotation * leftFrontWheelTransform;
            rightFrontWheelBone.Transform = wheelRotation * rightFrontWheelTransform;
            leftSteerBone.Transform = steerRotation * leftSteerTransform;
            rightSteerBone.Transform = steerRotation * rightSteerTransform;

            // Aplica as transformações em cascata por todos os bones
            tankModel.CopyAbsoluteBoneTransformsTo(boneTransforms);

            // Desenha o modelo
            foreach (ModelMesh mesh in tankModel.Meshes) 
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = boneTransforms[mesh.ParentBone.Index];
                    effect.View = camera.viewMatrix;
                    effect.Projection = camera.projectionMatrix;
                    effect.EnableDefaultLighting();
                }

                mesh.Draw();
            }
        }
    }
}

