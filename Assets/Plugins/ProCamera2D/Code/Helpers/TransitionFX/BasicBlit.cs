using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Com.LuisPedroFonseca.ProCamera2D
{
    /// <summary>
    /// Basic blit class used by the ProCamera2DTransitionFX extension
    /// </summary>

    [ExecuteInEditMode]
    public class BasicBlit : MonoBehaviour
    {
        public Material CurrentMaterial;

        void OnRenderImage(RenderTexture src, RenderTexture dst)
        {
            if (CurrentMaterial != null)
                Graphics.Blit(src, dst, CurrentMaterial);
        }
    }

    public class FullScreenQuad : ScriptableRendererFeature {
        [System.Serializable]
        public struct FullScreenQuadSettings
        {
            public RenderPassEvent renderPassEvent;
            public Material material;
        }

        public FullScreenQuadSettings m_Settings;

        // The actual render pass we are injecting.
        FullScreenQuadPass m_RenderQuadPass;

        public override void Create()
        {
            // Caches the render pass. Create method is called when the renderer instance is being constructed.
            m_RenderQuadPass = new FullScreenQuadPass(m_Settings);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            // Enqueues the render pass for execution. Here you can inject one or more render passes in the renderer
            // AddRenderPasses is called everyframe. 
            if (m_Settings.material != null)
                renderer.EnqueuePass(m_RenderQuadPass);
        }

    }

    public class FullScreenQuadPass : ScriptableRenderPass
    {
        string m_ProfilerTag = "DrawFullScreenPass";

        FullScreenQuad.FullScreenQuadSettings m_Settings;

        public FullScreenQuadPass(FullScreenQuad.FullScreenQuadSettings settings)
        {
            renderPassEvent = settings.renderPassEvent;
            m_Settings = settings;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            Camera camera = renderingData.cameraData.camera;

            var cmd = CommandBufferPool.Get(m_ProfilerTag);
            cmd.SetViewProjectionMatrices(Matrix4x4.identity, Matrix4x4.identity);
            cmd.DrawMesh(RenderingUtils.fullscreenMesh, Matrix4x4.identity, m_Settings.material);
            cmd.SetViewProjectionMatrices(camera.worldToCameraMatrix, camera.projectionMatrix);
            context.ExecuteCommandBuffer(cmd);

            CommandBufferPool.Release(cmd);
        }
    }

    [ExecuteInEditMode]
    public class URPBasicBlit : MonoBehaviour
    {
        public Material CurrentMaterial;

        void OnEnable()
        {
            RenderPipelineManager.endFrameRendering += RenderPipelineManager_endFrameRendering;
            // RenderPipelineManager.endCameraRendering += RenderPipelineManager_endCameraRendering;
        }

        void OnDisable()
        {
            RenderPipelineManager.endFrameRendering -= RenderPipelineManager_endFrameRendering;
            // RenderPipelineManager.endCameraRendering -= RenderPipelineManager_endCameraRendering;
        }

        static int cameraColorTextureId = Shader.PropertyToID("_CameraColorTexture");

        private void RenderPipelineManager_endCameraRendering(ScriptableRenderContext context, Camera camera)
        {
            // var cmd = CommandBufferPool.Get("DrawFullScreenPass");

            // context.SetupCameraProperties(camera);
            // cmd.GetTemporaryRT(cameraColorTextureId, camera.pixelWidth, camera.pixelHeight);

            // CameraClearFlags clearFlags = camera.clearFlags;
            // cmd.SetRenderTarget(cameraColorTextureId, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store);
            // cmd.Blit(cameraColorTextureId, BuiltinRenderTextureType.CameraTarget);
            // context.ExecuteCommandBuffer(cmd);
            // cmd.Clear();

            // context.Submit();

            // cmd.SetViewProjectionMatrices(Matrix4x4.identity, Matrix4x4.identity);
            // cmd.DrawMesh(RenderingUtils.fullscreenMesh, Matrix4x4.identity, CurrentMaterial);
            // cmd.SetViewProjectionMatrices(camera.worldToCameraMatrix, camera.projectionMatrix);
            // context.ExecuteCommandBuffer(cmd);

            // CommandBufferPool.Release(cmd);
            // context.Submit();
        }

        private void RenderPipelineManager_endFrameRendering(ScriptableRenderContext arg1, Camera[] arg2)
        {
            Graphics.Blit(arg2[0].activeTexture, CurrentMaterial);
        }
    }
}