namespace BusinessLogic.Services
{
    public interface IConverterService<U,V>
    {
        U Convert(V obj);
    }
}
