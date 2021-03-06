﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshop
{
    class GameObject
    {
        public Vector3 Position {
            get { return m_Position; }
            set { m_Position = value; m_World_changed = true; }
        }
        public Vector3 Rotation {
            get { return m_Rotation; }
            set { m_Rotation = value; m_World_changed = true; }
        }
        Vector3 m_Position;
        Vector3 m_Rotation;
        Matrix m_World;
        bool m_World_changed;
        Model m_Model;
        Effect m_Effect;
        Texture2D m_Texture;
        Texture2D m_NormalMap;

        public GameObject(Model model, Effect effect = null, Texture2D texture = null, Texture2D normalMap = null)
        {
            m_Position = Vector3.Zero;
            m_Rotation = Vector3.Zero;
            m_Model = model;
            m_Effect = effect;
            m_Texture = texture;
            m_NormalMap = normalMap;

            m_World_changed = true;
        }

        public void Render(Matrix view, Matrix projection, 
            float? texRepetitions = null, float? normalTexRepetition = null,
            Vector3? light = null, float? ell_value = null, 
            float? ambient_light_intensity = null, Texture2D shadowMap = null)
        {
            if (m_World_changed)
            {
                m_World = Matrix.CreateWorld(m_Position, Vector3.Forward, Vector3.Up) * Matrix.CreateRotationX(m_Rotation.X) * Matrix.CreateRotationY(m_Rotation.Y) * Matrix.CreateRotationZ(m_Rotation.Z);
                m_World_changed = false;
            }
            
            if (m_Effect == null)
            {
                m_Model.Draw(m_World, view, projection);
                return;
            }

            foreach(ModelMesh mesh in m_Model.Meshes)
            {
                foreach(ModelMeshPart part in mesh.MeshParts)
                {
                    part.Effect = m_Effect;
                    
                    m_Effect.Parameters["World"].SetValue(m_World);
                    m_Effect.Parameters["View"].SetValue(view);
                    m_Effect.Parameters["Projection"].SetValue(projection);
                    if(texRepetitions != null)
                    {
                        m_Effect.Parameters["TexRepetitions"].SetValue(texRepetitions.Value);
                    }
                    if(ambient_light_intensity != null)
                    {
                        m_Effect.Parameters["AmbientLightEll"].SetValue(ambient_light_intensity.Value);
                    }
                    if(light != null)
                    {
                        if (ell_value != null)
                        {
                            m_Effect.Parameters["LightPosition"].SetValue(light.Value);
                            m_Effect.Parameters["EllValue"].SetValue(ell_value.Value);
                        }
                        else
                        {
                            m_Effect.Parameters["LightDirection"].SetValue(light.Value);
                        }
                    }
                    if(m_Texture != null)
                    {
                        m_Effect.Parameters["Texture"].SetValue(m_Texture);
                    }
                    if(m_NormalMap != null)
                    {
                        m_Effect.Parameters["NormalMap"].SetValue(m_NormalMap);
                        if(normalTexRepetition != null)
                        {
                            m_Effect.Parameters["NormalTexRepetitions"].SetValue(normalTexRepetition.Value);
                        }
                    }
                    if(shadowMap != null)
                    {
                        m_Effect.Parameters["ShadowMap"].SetValue(shadowMap);
                    }
                }
                mesh.Draw();
            }
        }
    }
}
