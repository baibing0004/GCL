using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace GCL.IO {
    /// <summary>
    /// 分别使用读写指针进行数组读写。
    /// 使用列表 byte[] 管理数据的增长
    /// 允许进行同步与非同步方法访问
    /// </summary>
    public class SafeMemoryStream : Stream {
        protected long readPosition, writePosition, bufferBlockLength = 1024;

        //为真时读指针小于写指针 为假时 写指针小于读指针
        protected bool isForword = true;
        protected IList<byte[]> buf = new List<byte[]>();
        public SafeMemoryStream(int buffer) {
            this.bufferBlockLength = Math.Max(buffer, 1024);
            buf.Add(new byte[this.bufferBlockLength]);
        }

        public override bool CanRead {
            get { return true; }
        }

        public override bool CanSeek {
            get { return false; }
        }

        public override bool CanWrite {
            get { return true; }
        }

        public override void Flush() {
            this.buf.Clear();
            this.buf.Add(new byte[this.bufferBlockLength]);
            this.readPosition = 0;
            this.writePosition = 0;
            this.isForword = true;
        }

        public long WritePosition {
            get { return writePosition; }
        }

        public long ReadPosition {
            get { return readPosition; }
        }
        /// <summary>
        /// 返回数据区域总和
        /// </summary>
        public long Capacity {
            get {
                return (buf.Count * bufferBlockLength);
            }
        }
        /// <summary>
        /// 返回数据长度
        /// </summary>
        public override long Length {
            get {
                var datalength = writePosition - readPosition;
                return this.isForword ? datalength : Capacity + datalength;
            }
        }

        /// <summary>
        /// 返回缓冲区剩余的数据长度
        /// </summary>
        protected long Rest {
            get {
                var datalength = writePosition - readPosition;
                return this.isForword ? Capacity - datalength : -datalength;
            }
        }

        public override long Position {
            get;
            set;
        }

        protected object rsKey = DateTime.Now;
        public override int Read(byte[] buffer, int offset, int count) {
            if (buffer == null || buffer.Length < count + offset) {
                throw new InvalidOperationException("数据长度不足！");
            }
            lock (rsKey) {
                long wp = offset;
                long wl = Math.Min(count, this.Length);
                int ret = (int)wl;
                //先计算当前数据块中的剩余空间
                while (wl > 0) {
                    int wi = (int)(this.readPosition / this.bufferBlockLength);
                    long rest = this.bufferBlockLength - this.readPosition % this.bufferBlockLength;
                    long readLength = Math.Min(wl, rest);
                    Array.Copy(buf[wi], this.readPosition % this.bufferBlockLength, buffer, wp, readLength);
                    this.readPosition += readLength;
                    wp += readLength;
                    wl -= readLength;
                    if (this.readPosition >= this.Capacity) { this.readPosition = 0; this.isForword = !this.isForword; }
                }
                int restBlocks = (int)(this.Rest / this.bufferBlockLength);
                if (restBlocks >= 1) {
                    //如果出现空闲的区块进行合并
                    if (this.isForword) {
                        //说明是正向的 空闲区在两边
                        //左侧 小于这个区块的应该被清除
                        int ri = (int)(this.readPosition / this.bufferBlockLength);
                        //大于这个区块的数据应该被清除
                        int wlen = (int)Math.Ceiling(this.Length * 1.0 / this.bufferBlockLength);
                        for (int w = 0; w < ri; w++) {
                            this.buf.RemoveAt(0);
                            this.readPosition -= this.bufferBlockLength;
                            this.writePosition -= this.bufferBlockLength;
                        }
                        int wi = (int)(this.writePosition / this.bufferBlockLength);
                        while (this.buf.Count > wi + 1) {
                            this.buf.RemoveAt(wi + 1);
                        }
                        if (this.buf.Count > Math.Max(wlen, 1)) {
                            //处理两侧的数据块进行合并的情况。
                            Array.Copy(buf[wi], 0, buf[0], 0, this.writePosition % this.bufferBlockLength);
                            this.writePosition = this.writePosition % this.bufferBlockLength;
                            this.buf.RemoveAt(wlen);
                        }
                    } else {
                        //说明是反向的 空闲区在中间
                        //左侧 小于这个区块的应该被清除
                        int ri = (int)(this.readPosition / this.bufferBlockLength);
                        int wi = (int)(this.writePosition / this.bufferBlockLength);
                        //首先清除中间空闲区中完全独立的区块
                        for (int w = 0; w < ri - (wi + 1); w++) {
                            this.buf.RemoveAt(wi + 1);
                            this.readPosition -= this.bufferBlockLength;
                        }
                        if (this.Rest > this.bufferBlockLength) {
                            //判断两个相邻的区块
                            Array.Copy(this.buf[wi], 0, this.buf[wi + 1], 0, this.writePosition % this.bufferBlockLength);
                            this.buf.RemoveAt(wi);
                            this.readPosition -= this.bufferBlockLength;
                        }
                    }
                }
                return ret;
            }
        }

        public override long Seek(long offset, SeekOrigin origin) {
            return 0;
        }

        public override void SetLength(long value) {            
        }

        public override void Write(byte[] buffer, int offset, int count) {
            if (buffer == null || buffer.Length < count + offset) {
                throw new InvalidOperationException("数据长度不足！");
            }
            lock (rsKey) {
                if (this.Rest < count) {
                    //需要添加的数据长度
                    int addBlocks = (int)(Math.Ceiling((count - this.Rest) * 1.0 / this.bufferBlockLength));
                    if (this.isForword) {
                        //正常顺序直接添加
                        for (int w = 0; w < addBlocks; w++) {
                            this.buf.Add(new byte[this.bufferBlockLength]);
                        }
                    } else {
                        //反向顺序 数据移动
                        int index = (int)(this.writePosition / this.bufferBlockLength);
                        for (int w = 0; w < addBlocks; w++) {
                            this.buf.Insert(index, new byte[this.bufferBlockLength]);
                        }

                        Array.Copy(buf[index + addBlocks], buf[index], this.bufferBlockLength);
                        //挪动的块数
                        this.readPosition += this.bufferBlockLength * addBlocks;
                    }
                }
                long wp = offset;
                long wl = count;
                //先计算当前数据块中的剩余空间
                while (wl > 0) {
                    int wi = (int)(this.writePosition / this.bufferBlockLength);
                    long rest = this.bufferBlockLength - this.writePosition % this.bufferBlockLength;
                    long writeLength = Math.Min(wl, rest);
                    Array.Copy(buffer, wp, buf[wi], this.writePosition % this.bufferBlockLength, writeLength);
                    this.writePosition += writeLength;
                    wp += writeLength;
                    wl -= writeLength;
                    if (this.writePosition >= this.Capacity) { this.writePosition = 0; this.isForword = !this.isForword; }
                }
            }
        }

        /// <summary>
        /// 返回所有的数据并清理缓存
        /// </summary>
        /// <returns></returns>
        public byte[] ToArray() {
            if (this.Length <= 0) return null;
            lock (rsKey) {
                byte[] rets = new byte[this.Length];
                long wp = 0;
                long wl = rets.Length;
                //先计算当前数据块中的剩余空间
                while (wl > 0) {
                    int wi = (int)(this.readPosition / this.bufferBlockLength);
                    long rest = this.bufferBlockLength - this.readPosition % this.bufferBlockLength;
                    long readLength = Math.Min(wl, rest);
                    Array.Copy(buf[wi], this.readPosition % this.bufferBlockLength, rets, wp, readLength);
                    this.readPosition += readLength;
                    wp += readLength;
                    wl -= readLength;
                    if (this.readPosition >= this.Capacity) { this.readPosition = 0; this.isForword = !this.isForword; }
                }
                this.Flush();
                return rets;
            }
        }
    }
}
