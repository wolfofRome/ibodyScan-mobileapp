using UnityEngine;
using UnityEngine.SceneManagement;

namespace FitAndShape
{
    public sealed class BootView : MonoBehaviour
    {
        public void LoadFitAndShapeScene()
        {
            SceneManager.LoadScene("FitAndShape");
        }

        public void LoadAutotailorScene()
        {
            SceneManager.LoadScene("ModelView");
        }

        public void LoadLoginScene()
        {
            SceneManager.LoadScene(LoginPresenter.SceneName);
        }

        public void LoadAppScene()
        {
            SceneManager.LoadScene(FitAndShapePresenterApp.SceneName);
        }
    }
}