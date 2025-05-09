using UnityEngine.SceneManagement;

public static class SceneLoader
{
    private const string SCENEPATH = "Assets/01. Scenes/";

    public static int NextSceneIndex;
    public static string SceneName;

    public static void LoadSceneWithLoading(int nextSceneIndex)
    {
        NextSceneIndex = nextSceneIndex;
        SceneManager.LoadScene("LoadingScene");
    }

    public static void LoadSceneWithLoading(string nextSceneName)
    {
        SceneName = $"{SCENEPATH}{nextSceneName}.unity";
        NextSceneIndex = SceneUtility.GetBuildIndexByScenePath(SceneName);
        SceneManager.LoadScene("LoadingScene");
    }
}