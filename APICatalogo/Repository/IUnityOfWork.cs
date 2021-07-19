using System.Threading.Tasks;

namespace APICatalogo.Repository
{
    public interface IUnityOfWork
    {
        IProductRepository ProductRepository { get; }
        ICategoryRepository CategoryRepository { get; }
        Task Commit();
    }
}
