#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace UDT.LevelTools
{
    [ExecuteAlways]
    public class StreamedObject : MonoBehaviour
    {
#if UNITY_EDITOR
        [MenuItem("Toolkit/Level Components/Streamed Object")]
        public static void CreateLevel()
        {
            GameObject go = new GameObject("Streamed Object");
            
            GameObject selected = Selection.activeObject as GameObject;
            if (selected != null)
            {
                go.transform.parent = selected.transform;
                go.transform.position = selected.transform.position;
            }

            go.AddComponent<StreamedObject>().transform.position = Camera.current.ViewportToWorldPoint(new Vector3(0.5f,0.5f,1));
            Selection.activeGameObject = go;
        }
#endif
        
        private GameObject _instance;
        [SerializeField]
        private GameObject prefab;
        [SerializeField] private string path;
        private GameObject _usedPrefab;
        private string _oldPath;
        private string _oldName;

        private void Awake()
        {
            if(Application.isPlaying)
                Unload();
        }

        public void Load()
        {
            if(prefab == null) return;
            if (_instance != null) return;
            _instance = GameObject.Instantiate(prefab, transform);
        }

        public void Unload()
        {
            foreach(Transform child in transform.GetChildren()) if(child != transform) Destroy(child.gameObject);
        }

        void Update()
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
                return;
            bool generate = prefab == null;

            if (_oldName != name || _oldPath != path)
            {
                generate = true;
                _oldName = name;
                _oldPath = path;
                
                //Destroy the old Prefab if it exists
                if (prefab != null)
                {
                    try
                    {
                        DestroyImmediate(prefab, true);
                    }
                    catch
                    {
                        generate = false;
                    }
                }
            }

            string fullPath = "Assets/" + path + name+".prefab";
            if (generate)
            {
                //Generate
                GameObject generated = new GameObject();
                prefab = PrefabUtility.SaveAsPrefabAsset(generated, fullPath);
                DestroyImmediate(generated);
                
                //Instantiate the Instance of the Prefab
                _instance = PrefabUtility.InstantiatePrefab(prefab, transform) as GameObject;
                Selection.activeObject=AssetDatabase.LoadMainAssetAtPath(fullPath);
            }

            if (_instance == null)
                _instance = transform.GetChild(0).gameObject;
            else
                prefab = PrefabUtility.SaveAsPrefabAsset(_instance, fullPath);
#endif
        }
    }
}