///
 /// 
 ///
namespace GCL.Collections.Pool {

    ///
    /// @author baibing
    /// 
    ///
    public class PoolStaregyFactory {
        public static IPoolStaregy GetPoolStaregy(System.Collections.IList coll) {
            return new CollectionPoolStaregy(coll);
        }

        public static IPoolStaregy GetPoolStaregy(System.Collections.Queue coll) {
            return new QueuePoolStaregy();
        }

        public static IPoolStaregy GetPoolStaregy(System.Collections.Stack coll) {
            return new StackPoolStaregy();
        }
    }
}
