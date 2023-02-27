using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;

namespace Engine.OpenGL
{
    public abstract class GLResource
        : IDisposable
    {
        /// <summary>
        /// Gets a values specifying if this resource has already been disposed.
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Initializes a new instance of the GLObject class.
        /// </summary>
        protected GLResource()
        {
            IsDisposed = false;
        }

        /// <summary>
        /// Called by the garbage collector and an indicator for a resource leak because the manual dispose prevents this destructor from being called.
        /// </summary>
        ~GLResource()
        {
            Console.WriteLine($"GLResource leaked: {this}");
            Dispose(false);
#if DEBUG
            throw new Exception($"GLResource leaked: {this}");
#endif
        }

        /// <summary>
        /// Releases all OpenGL handles related to this resource.
        /// </summary>
        public void Dispose()
        {
            // safely handle multiple calls to dispose
            if (IsDisposed) return;
            IsDisposed = true;
            // dipose this resource
            Dispose(true);
            // prevent the destructor from being called
            GC.SuppressFinalize(this);
            // make sure the garbage collector does not eat our object before it is properly disposed
            GC.KeepAlive(this);
        }

        /// <summary>
        /// Releases all OpenGL handles related to this resource.
        /// </summary>
        /// <param name="manual">True if the call is performed explicitly and within the OpenGL thread, false if it is caused by the garbage collector and therefore from another thread and the result of a resource leak.</param>
        protected abstract void Dispose(bool manual);

        /// <summary>
        /// Automatically calls <see cref="Dispose()"/> on all <see cref="GLResource"/> objects found on the given object.
        /// </summary>
        /// <param name="obj"></param>
        public static void DisposeAll(object obj)
        {
            // get all fields, including backing fields for properties
            foreach (var field in obj.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                // check if it should be released
                if (typeof(GLResource).IsAssignableFrom(field.FieldType))
                {
                    // and release it
                    ((GLResource)field.GetValue(obj)).Dispose();
                }
            }
        }
    }
}
