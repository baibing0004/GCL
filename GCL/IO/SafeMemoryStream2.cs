using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace GCL.IO {
    /// <summary>
    /// 分别使用读写指针进行数组读写。
    /// 使用列表 byte[] 管理数据的增长
    /// 只从左侧删除，不进行数据的迁移和合并 牺牲一个数据块的空间 提高效率
    /// 允许进行同步与非同步方法访问
    /// </summary>
    public class SafeMemoryStream2 : SafeMemoryStream {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer">1个数据块的大小，不得小于1024</param>
        public SafeMemoryStream2(int buffer)
            : base(buffer) {
        }

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
                }
                int restBlocks = (int)(this.Rest / this.bufferBlockLength);
                if (restBlocks >= 1) {
                    //如果出现空闲的区块进行合并
                    //说明是正向的 空闲区在两边
                    //左侧 小于这个区块的应该被清除
                    int ri = (int)(this.readPosition / this.bufferBlockLength);
                    for (int w = 0; w < ri; w++) {
                        this.buf.RemoveAt(0);
                        this.readPosition -= this.bufferBlockLength;
                        this.writePosition -= this.bufferBlockLength;
                    }
                }
                return ret;
            }
        }

        public override void Write(byte[] buffer, int offset, int count) {
            if (buffer == null || buffer.Length < count + offset) {
                throw new InvalidOperationException("数据长度不足！");
            }
            lock (rsKey) {
                long rest = this.Rest - this.readPosition;
                if (rest < count) {
                    //需要添加的数据长度
                    int addBlocks = (int)(Math.Ceiling((count - rest) * 1.0 / this.bufferBlockLength));
                    //正常顺序直接添加
                    for (int w = 0; w < addBlocks; w++) {
                        this.buf.Add(new byte[this.bufferBlockLength]);
                    }
                }
                long wp = offset;
                long wl = count;
                //先计算当前数据块中的剩余空间
                while (wl > 0) {
                    int wi = (int)(this.writePosition / this.bufferBlockLength);
                    long rest2 = this.bufferBlockLength - this.writePosition % this.bufferBlockLength;
                    long writeLength = Math.Min(wl, rest2);
                    Array.Copy(buffer, wp, buf[wi], this.writePosition % this.bufferBlockLength, writeLength);
                    this.writePosition += writeLength;
                    wp += writeLength;
                    wl -= writeLength;
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
                }
                this.Flush();
                return rets;
            }
        }
    }
}
