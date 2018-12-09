using System;

namespace SQLite {
    public abstract class Disposable : IDisposable {
        readonly Action disposeAction;
        
        protected Disposable(Action disposing) {
            disposeAction = disposing;
        }
        ~Disposable() {
            Dispose(false);
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        // ReSharper disable once VirtualMemberNeverOverridden.Global
        // ReSharper disable once UnusedParameter.Global
        protected virtual void Dispose(bool disposing) {
            disposeAction?.Invoke();
        }
    }
}