using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace GCL.Module.Trigger {
    public class FileTrigger : ATrigger {
        private FileInfo file;

        public FileInfo File {
            get { return file; }
        }
        private long length = 0;
        private DateTime lastModified;
        public FileTrigger(string path)
            : this(new FileInfo(path)) {
        }
        public FileTrigger(FileInfo file) {
            this.file = file;
            this.ReSet();
        }

        protected override GCL.Event.EventArg Attempt(GCL.Event.EventArg e) {
            FileInfo _file = new FileInfo(file.FullName);
            if (!_file.Exists || _file.LastWriteTime != lastModified || _file.Length != length) {
                this.ReSet();
                if (_file.Exists)
                    return new GCL.Event.EventArg(new object[] { _file.Exists, _file.LastWriteTime, _file.Length });
                else
                    return new GCL.Event.EventArg(new object[] { false, 0, 0 });
            }
            return null;
        }

        public override void ReSet() {
            FileInfo _file = new FileInfo(file.FullName);
            this.file = _file;
            if (_file.Exists) {
                this.length = this.file.Length;
                this.lastModified = this.file.LastWriteTime;
            } else {
                this.length = 0;
                this.lastModified = new DateTime();
            }
        }
    }
}
