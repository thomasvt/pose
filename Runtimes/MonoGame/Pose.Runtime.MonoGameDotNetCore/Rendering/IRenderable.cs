namespace Pose.Runtime.MonoGameDotNetCore.Rendering
{
    public interface IRenderable
    {
        /// <summary>
        /// For layered drawing. More is further back.
        /// </summary>
        float Depth { get; }

        void Draw(ICpuMeshRenderer quadRenderer);
    }
}