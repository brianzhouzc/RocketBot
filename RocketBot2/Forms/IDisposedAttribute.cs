using System;

namespace RocketBot2.Forms
{
    internal class IDisposedAttribute : Attribute
    {
        // Assume this type has some unmanaged resources.
        private bool disposed = false;
        private System.ComponentModel.IContainer components = null;

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            Dispose(disposing);
        }

        /*protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                // Dispose of resources held by this instance.

                // Violates rule: DisposableFieldsShouldBeDisposed.
                // Should call aFieldOfADisposableType.Dispose();

                disposed = true;
                // Suppress finalization of this disposed instance.
                if (disposing)
                {
                    GC.SuppressFinalize(this);
                }
            }
    }*/

    public void Dispose()
        {
            if (!disposed)
            {
                // Dispose of resources held by this instance.
                Dispose(true);
            }
        }

        // Disposable types implement a finalizer.
        ~IDisposedAttribute()
        {
            Dispose(false);
        }
    }
}