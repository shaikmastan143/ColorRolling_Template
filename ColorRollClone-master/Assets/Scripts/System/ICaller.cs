namespace Sieunguoimay
{
    public interface ICaller
    {
        void InvokeCaller(int taskId);
    }

    public class TaskIdentity
    {
        public int id;
        public ICaller caller;
    }
}
