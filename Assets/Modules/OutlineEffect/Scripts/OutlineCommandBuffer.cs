﻿using UnityEngine;
using UnityEngine.Rendering;
using System.Collections.Generic;

namespace TA.OutlineEffect
{
    public class OutlineCommandBuffer
    {
        private readonly int COLOR_ID = Shader.PropertyToID("_Color");

        public bool IsEmpty { get { return m_renderers.Count == 0; } }
        private List<Renderer> m_renderers;
        private RenderTexture m_renderTexture;
        private Material m_prepassMaterial;
        private CommandBuffer m_commandBuffer;

        public OutlineCommandBuffer(Renderer[] renderers, RenderTexture renderTexture, Material prepassMaterial, Color color)
        {
            m_renderers = new List<Renderer>();
            m_renderers.AddRange(renderers);
            m_renderTexture = renderTexture;
            m_prepassMaterial = prepassMaterial;
            m_prepassMaterial.SetColor(COLOR_ID, color);
        }

        public void UpdateCommandBuffer(bool clear)
        {
            m_commandBuffer = new CommandBuffer();
            m_commandBuffer.SetRenderTarget(m_renderTexture);

            if (clear)
            {
                m_commandBuffer.ClearRenderTarget(true, true, Color.black);
            }

            foreach (Renderer r in m_renderers)
            {
                m_commandBuffer.DrawRenderer(r, m_prepassMaterial);
            }
        }

        public void UpdateColor(Color color)
        {
            m_prepassMaterial.SetColor(COLOR_ID, color);
        }

        public void Execute()
        {
            Graphics.ExecuteCommandBuffer(m_commandBuffer);
        }

        public void Destroy()
        {
            if (m_prepassMaterial)
            {
                GameObject.DestroyImmediate(m_prepassMaterial);
                m_prepassMaterial = null;
            }

            if (m_commandBuffer != null)
            {
                m_commandBuffer.Dispose();
                m_commandBuffer = null;
            }

        }
    }
}