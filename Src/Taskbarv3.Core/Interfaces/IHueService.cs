using System.Threading.Tasks;

namespace Taskbarv3.Core.Interfaces
{
    public interface IHueService
    {
        Task<bool> PowerLight(bool on);
        /// <param name="dimValue">Procentage 0-100</param>
        Task<bool> DimLight(int dimValue);
        Task<int> GetBrightnessProcentage();
        Task<bool> RegisterAccount();
    }
}
