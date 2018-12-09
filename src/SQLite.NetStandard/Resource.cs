using System;

namespace SQLite {
    public abstract class Resource : Disposable {
        public IntPtr Handle { get; }
        
        protected Resource(IntPtr handle, Action disposing) : base(disposing) {
            Handle = handle;
        }
    }
}