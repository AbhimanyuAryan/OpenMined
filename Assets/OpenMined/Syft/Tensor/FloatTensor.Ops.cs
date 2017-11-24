using UnityEngine;
using System;
using System.Threading.Tasks;

namespace OpenMined.Syft.Tensor
{
    public partial class FloatTensor
    {

        private void SameSizeDimensionsAndShape( ref FloatTensor tensor )
        {
            // Check if both tensors have same size
            if (tensor.Size != size)
            {
                throw new InvalidOperationException ("Tensors cannot be added since they have different sizes.");
            }
            // Check if both tensors have same number of dimensions
            if (tensor.Shape.Length != shape.Length)
            {
                throw new InvalidOperationException ("Tensors cannot be added since they have different number of dimensions.");
            }
            // Check if both tensors have same shapes
            for (int i = 0; i < shape.Length; i++)
            {
                if (shape [i] != tensor.Shape [i])
                {
                    throw new InvalidOperationException ("Tensors cannot be added since they have different shapes.");
                }
            }
        }
        private void SwapElements(ref int[] target, int index1, int index2)
        {
            int tmp = target[index1];
            target[index1] = target[index2];
            target[index2] = tmp;
        }

        private void SwapElements(ref long[] target, int index1, int index2)
        {
            long tmp = target[index1];
            target[index1] = target[index2];
            target[index2] = tmp;
        }
        
        public FloatTensor Transpose()
        {
            if (shape.Length != 2)
                throw new InvalidOperationException("Need to specify parameters for tensors with more than 2 dims.");

            return Transpose(0, 1);
        }

        public FloatTensor Transpose(int dimension1, int dimension2)
        {
            //TODO: Should we create a new Tensor object here?

            if (dimension1 < 0 || dimension1 >= shape.Length)
                throw new ArgumentOutOfRangeException("dimension1");
            if (dimension2 < 0 || dimension2 >= shape.Length)
                throw new ArgumentOutOfRangeException("dimension2");

            if (dimension1 == dimension2)
            {
                return this;
            }

            SwapElements(ref strides, dimension1, dimension2);
            SwapElements(ref shape, dimension1, dimension2);

            return this;
        }

        public FloatTensor Add(FloatTensor x)
        {

            // Check if both tensors are compatible for sum
            SameSizeDimensionsAndShape(ref x);
            
            if (dataOnGpu)
            {
                // GPU Add Code Here
            }
            else
            {
                var output = new float[this.size];
                var nCpu = SystemInfo.processorCount;
                Parallel.For(0, nCpu, workerId =>
                {
                    var max = this.size * (workerId + 1) / nCpu;
                    for (var i = this.size * workerId / nCpu; i < max; i++)
                        output[i] = x.Data[i] + this.Data[i];
                });
                return new FloatTensor(output, this.shape, false);
            }

            return this;
        }

        public FloatTensor Abs()
        {
            if (dataOnGpu)
            {
                // GPU Absolute Value Code Here
            }
            else
            {
                for (int i = 0; i < size; i++)
                {
                    if (Data[i] < 0)
                    {
                        Data[i] = -Data[i];
                    }
                }
            }
            return this;
        }


        public FloatTensor Neg()
        {
            if (dataOnGpu)
            {
                NegGPU();
            }
            else
            {
                int nCpu = SystemInfo.processorCount;
                Parallel.For(0, nCpu, workerId =>
                {
                    var max = data.Length * (workerId + 1) / nCpu;
                    for (int i = data.Length * workerId / nCpu; i < max; i++)
                        data[i] = -data[i];
                });
            }
            return this;
        }

		public FloatTensor Zero_()
		{
			if (dataOnGpu)
			{
				ZeroGPU_ ();
			}
			else
			{
				int nCpu = SystemInfo.processorCount;
				Parallel.For(0, nCpu, workerId =>
				{
					var max = data.Length * (workerId + 1) / nCpu;
					for (int i = data.Length * workerId / nCpu; i < max; i++)
						data[i] = 0;
				});
			}

			return this;
		}


        public FloatTensor ElementwiseMultiplication(FloatTensor other)
        {
            // Verify tensors are compatible for this operation
            SameSizeDimensionsAndShape (ref other);

            FloatTensor output = new FloatTensor(this.shape, dataOnGpu);

            if (this.dataOnGpu && other.DataOnGpu)
            {
                ElementwiseMultiplicationOnGpu(other);
            }
            else if (!this.dataOnGpu && !other.DataOnGpu)
            {
                for (int i = 0; i < this.size; i++)
                {
                    output.Data[i] = this.data[i] * other.Data[i];
                }
            }
            else
            {
                throw new InvalidOperationException ("Data for both Tensors needs to be colocated on the same device. - CPU != GPU");
            }
            return output;
        }

        public FloatTensor ScalarMultiplication(float scalar)
        {
            if (dataOnGpu)
            {
                ScalarMultiplicationOnGpu(scalar);
            }
            else
            {
                for (int i = 0; i < size; i++)
                {
                    Data[i] = Data[i] * scalar;
                }
            }
            return this;
        }

        public FloatTensor ElementwiseSubtract(FloatTensor other)
        {
            //Debug.LogFormat("<color=blue>FloatTensor.inline_elementwise_subtract dataOnGpu: {0}</color>", dataOnGpu);
            SameSizeDimensionsAndShape(ref other);

            if (dataOnGpu && other.DataOnGpu)
            {
                ElementwiseSubtractOnGpu(other);
            }
            else if (!dataOnGpu && !other.dataOnGpu)
            {
                for (int i = 0; i < size; i++)
                {
                    Data[i] = Data[i] - other.Data[i];
                }
            }
            else
            {
                throw new InvalidOperationException ("Data for both Tensors needs to be colocated on the same device. - CPU != GPU");
            }
            return this;
        }

        public FloatTensor AddMatrixMultiply(FloatTensor tensor1, FloatTensor tensor2)
        {
            // TODO: check for corner cases

            bool gpu = dataOnGpu & tensor1.DataOnGpu & tensor2.DataOnGpu;
            bool cpu = !(dataOnGpu | tensor1.DataOnGpu | tensor2.DataOnGpu);

            if (gpu)
            {
                AddMatrixMultiplyOnGpu(tensor1, tensor2);
            }
            else if (cpu)
            {
                //TODO: implement the function
            }
            else
            {
                Debug.Log("Data for all Tensors needs to be colocated on the same device. - CPU != GPU");
            }
            return this;
        }

        public FloatTensor MultiplyDerivative(FloatTensor other)
        {
            // TODO: check for corner cases
            if (dataOnGpu & other.DataOnGpu)
            {
                MultiplyDerivativeOnGpu(other);
            }
            else if (!dataOnGpu & !other.DataOnGpu)
            {
                //TODO: implement the function
            }
            else
            {
                Debug.Log("Data for all Tensors needs to be colocated on the same device. - CPU != GPU");
            }
            return this;
        }
    }
}