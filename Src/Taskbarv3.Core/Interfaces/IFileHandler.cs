namespace Taskbarv3.Core.Interfaces
{
    public interface IFileHandler<T>
    {
        void SaveToFile(T config);
        T LoadFromFile();
    }
}
