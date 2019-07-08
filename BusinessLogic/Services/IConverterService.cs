using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public interface IConverterService<U,V>
    {
        Task<U> ConvertAsync(V obj);
    }
}
