using System;
namespace GCL.Collections.Pool{


    public class PoolClosedException : Exception{

        ///
        /// 
        ///
        public PoolClosedException():base(){

        }

        ///
        /// @param message
        ///
        public PoolClosedException(string message):base(message){
        }

        ///
        /// @param message
        /// @param cause
        ///
        public PoolClosedException(string message, Exception cause):base(message,cause){
        }
    }
}
