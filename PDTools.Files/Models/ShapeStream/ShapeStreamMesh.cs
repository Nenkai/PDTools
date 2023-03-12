using PDTools.Files.Models.ModelSet3.ShapeStream;


namespace PDTools.Files.Models.ShapeStream
{
    public class ShapeStreamMesh
    {
        public ShapeStreamChunk ShapeStreamChunk;
        public MDL3ShapeStreamingInfoMeshEntry InfoMeshEntry;
        public uint MeshDataSize;
        public uint VerticesOffset;
        public uint TriOffset;
        public uint BBoxOffset;
    }
}
