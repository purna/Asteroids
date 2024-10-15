using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneGameManager : MonoBehaviour
{

  public void ReloadScene()
   {
       // Reload the scene after 3 seconds
       
        Invoke("ReloadLevel", 3);
   }

   private void ReloadLevel()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        }
}

