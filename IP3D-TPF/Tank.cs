﻿using Microsoft.Xna.Framework;
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
    class Tank
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
        float scale, aspectRatio, yaw, pitch, speed = 0.3f;
        float wheelRotationValue = 0, steerRotationValue = 0, turretRotationValue = 0, cannonRotationValue = 0;
        Vector3 position, direction, d, n, right;

        public Tank(GraphicsDevice device, ContentManager content)
        {
            wordlMatrix = Matrix.Identity;
            r = Matrix.Identity;

            tankModel = content.Load<Model>("tank");

            scale = 0.005f;

            position = new Vector3(10f, -10.0f, 40f);

            r.Forward = d;
            r.Up = n;
            r.Right = right;

            effect = new BasicEffect(device);

            // Calcula a aspectRatio, a view matrix e a projeção
            aspectRatio = (float)device.Viewport.Width / device.Viewport.Height;

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

        public void Move(Field field)
        {
            //posição inicial da direção
            direction = new Vector3(0f, 0f, -1f);

            float diference = MathHelper.ToRadians(10f);

            float positionBackX = position.X;
            float positionBackZ = position.Z;

            KeyboardState keys = Keyboard.GetState();

            if(keys.IsKeyDown(Keys.W))
            {
                position += direction * speed; 
            }

            if(keys.IsKeyDown(Keys.S))
            {
                position -= direction * speed;
            }

            if(keys.IsKeyDown(Keys.A))
            {
                yaw -= diference * speed;
            }

            if(keys.IsKeyDown(Keys.D))
            {
                yaw += diference * speed;
            }

            //Limitar o movimento aos limites do terreno
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

            position.Y = field.SurfaceFollow(position);
            //position.Y = field.NormalFollow(position);

            //definição da rotationMatrix através do yaw e do pitch
            rotationMatrix = Matrix.CreateFromYawPitchRoll(yaw, pitch, 0);

            //transformação da direção através da rotationMatrix
            direction = Vector3.Transform(direction, rotationMatrix);
        }

        public void Draw(Camera camera)
        {
            // Aplica uma transformação qualquer no bone Root, no canhão e na torre
            tankModel.Root.Transform = Matrix.CreateScale(scale) * rotationMatrix * Matrix.CreateTranslation(position);

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

            turretBone.Transform = Matrix.CreateRotationY(MathHelper.ToRadians(30.0f)) * turretTransform;
            cannonBone.Transform = Matrix.CreateRotationX(MathHelper.ToRadians(30.0f)) * cannonTransform;

            // Aplica as transformações em cascata por todos os bones
            tankModel.CopyAbsoluteBoneTransformsTo(boneTransforms);
            foreach (ModelMesh mesh in tankModel.Meshes) // Desenha o modelo
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = boneTransforms[mesh.ParentBone.Index];
                    effect.View = camera.viewMatrix; effect.Projection = camera.projectionMatrix;
                    effect.EnableDefaultLighting();
                }

                mesh.Draw();
            }
        }
    }
}

