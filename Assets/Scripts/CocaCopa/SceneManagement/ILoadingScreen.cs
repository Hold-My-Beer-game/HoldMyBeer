using System.Threading.Tasks;

namespace CocaCopa.SceneManagement {
    public interface ILoadingScreen {
        Task Show();
        Task Hide();
    }
}