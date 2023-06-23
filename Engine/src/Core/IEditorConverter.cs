using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Core
{
    public interface IEditorConverter<T>
    {
        public float ToEditorHeight(T value);
        public T FromEditorHeight(float value);
    }

    public class FloatConverter : IEditorConverter<float>
    {

        public float FromEditorHeight(float value) => value;

        public float ToEditorHeight(float value) => value;
    }

    public class DoubleConverter : IEditorConverter<double>
    {

        public double FromEditorHeight(float value) => (double)value;

        public float ToEditorHeight(double value) => (float)value;
    }

}
