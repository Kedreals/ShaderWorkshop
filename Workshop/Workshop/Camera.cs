using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshop
{
    class Camera
    {
        Vector3 m_eye;
        Vector3 m_forward;
        Vector3 m_up;

        float m_fov;
        float m_aspect;
        float m_near;
        float m_far;

        public Matrix View { get { return Matrix.CreateLookAt(m_eye, m_eye + m_forward, m_up); } }
        public Matrix Projection { get { return Matrix.CreatePerspectiveFieldOfView(m_fov, m_aspect, m_near, m_far); } }

        public Camera(Vector3 position, Vector3 lookat, Vector3 up, float fov = 45.0f*(float)Math.PI/180.0f, float aspect = 1024.0f / 800.0f, float nearPlane = 0.2f, float farPlane = 1000)
        {
            m_eye = position;
            m_forward = lookat - m_eye;
            m_up = up;
            m_fov = fov;
            m_aspect = aspect;
            m_near = nearPlane;
            m_far = farPlane;
        }
    }
}
