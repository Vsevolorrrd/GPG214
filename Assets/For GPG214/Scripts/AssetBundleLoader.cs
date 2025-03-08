using System.IO;
using UnityEngine;
using System.Collections;
using System.Linq;

public class AssetBundleLoader : MonoBehaviour
{
    public string bundleName = "levelassets";
    public string assetName = "EchoPoint";
    public Transform[] echoPointSpawns;
    private string bundlePath;

    void Start()
    {
        bundlePath = Path.Combine(Application.streamingAssetsPath, bundleName);
        StartCoroutine(LoadAssetBundle());
    }

    IEnumerator LoadAssetBundle()
    {
        if (!File.Exists(bundlePath))
        {
            Debug.LogError("Asset Bundle not found: " + bundlePath);
            yield break;
        }

        // Check if the bundle is already loaded
        AssetBundle existingBundle = AssetBundle.GetAllLoadedAssetBundles().FirstOrDefault(bundle => bundle.name == bundleName);

        if (existingBundle != null)
        {
            Debug.LogWarning("AssetBundle already loaded");
            yield return LoadAsset(existingBundle);
            yield break;
        }

        AssetBundleCreateRequest bundleRequest = AssetBundle.LoadFromFileAsync(bundlePath);
        yield return bundleRequest;

        AssetBundle assetBundle = bundleRequest.assetBundle;
        if (assetBundle == null)
        {
            Debug.LogError("Asset Bundle Failed");
            yield break;
        }

        yield return LoadAsset(assetBundle);
    }

    IEnumerator LoadAsset(AssetBundle assetBundle)
    {
        AssetBundleRequest assetRequest = assetBundle.LoadAssetAsync<GameObject>(assetName);
        yield return assetRequest;

        GameObject loadedObject = assetRequest.asset as GameObject;
        if (loadedObject != null)
        {
            foreach (Transform transform in echoPointSpawns)
            {
                Instantiate(loadedObject, transform.position, Quaternion.identity);
            }
            Debug.Log("Loaded and instantiated: " + assetName);
        }
        else
        {
            Debug.LogError("no asset found in bundle");
        }
    }
}