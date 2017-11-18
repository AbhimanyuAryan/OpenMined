using UnityEngine;

namespace OpenMined.Syft.Tensor
{
    // TODO: Implement move data to GPU to CPU etc. in this file
    public partial class FloatTensor
    {   
        private bool dataOnGpu;

        private ComputeBuffer dataBuffer;
        private ComputeBuffer shapeBuffer;

        public bool DataOnGpu => dataOnGpu;

        public ComputeBuffer DataBuffer
        {
            get { return dataBuffer; }
            set { dataBuffer = value; }
        }
        
        public void Gpu()
        {
            if (!dataOnGpu)
            {
                CopyCputoGpu();
                EraseCpu();
                dataOnGpu = true;
            }
        }

        public void Cpu()
        {
            if (dataOnGpu)
            {
                CopyGpuToCpu();
                EraseGpu();
                dataOnGpu = false;
            } 
        }
        
        private void CopyGpuToCpu()
        {
            data = new float[size];
            dataBuffer.GetData(data);
        }
        
        private void CopyCputoGpu()
        {
            dataBuffer = new ComputeBuffer(size, sizeof(float));
            shapeBuffer = new ComputeBuffer(shape.Length, sizeof(int));

            dataBuffer.SetData(data);	
            shapeBuffer.SetData(shape);
        }

        private void EraseCpu()
        {
            data = null;
        }
        
        private void EraseGpu()
        {
            dataBuffer.Release();
            shapeBuffer.Release();
        }
    }
}