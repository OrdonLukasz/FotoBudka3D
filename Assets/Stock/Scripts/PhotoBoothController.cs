using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Dummiesman;
using TMPro;

public class PhotoBoothController : MonoBehaviour
{
    [SerializeField] private float rotationSpeed;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Button buttonPreviousObject;
    [SerializeField] private Button buttonNextObject;
    [SerializeField] private Button buttonShowOutputPath;
    [SerializeField] private Button buttonShowInputPath;
    [SerializeField] private List<GameObject> loadedObjects = new List<GameObject>();
    
    private GameObject loadedObject;
    private int currentIndex = 0;

    private void Start()
    {
        AddListeners();
        LoadMesh();
    }

    private void Update()
    {
        DragToRotate();
        CameraScrollZoom();
    }
    
    private void OnApplicationFocus()
    {
        LoadMesh();
    }

    private void AddListeners()
    {
        buttonPreviousObject.onClick.AddListener(() => { SwitchToPreviousObject(); });
        buttonNextObject.onClick.AddListener(() => { SwitchToNextObject(); });
        buttonShowOutputPath.onClick.AddListener(() => { ShowPath("Output"); });
        buttonShowInputPath.onClick.AddListener(() => { ShowPath("Input"); });
    }

    private void CameraScrollZoom()
    {
        if (Input.mouseScrollDelta.y > 0)
        {
            mainCamera.transform.position += new Vector3(0, 0, 0.2f);
            transform.Rotate(-0.01f / 2, 0, 0.2f);
        }
        if (Input.mouseScrollDelta.y < 0)
        {
            mainCamera.transform.position -= new Vector3(0, 0, 0.2f);
            transform.Rotate(0.01f / 2, 0, 0.2f);
        }
    }

    private void DragToRotate()
    {
        if (Input.GetMouseButton(0) && loadedObject != null)
        {
            float rotX = Input.GetAxis("Mouse X") * rotationSpeed * Mathf.Deg2Rad;
            float rotY = Input.GetAxis("Mouse Y") * rotationSpeed * Mathf.Deg2Rad;

            loadedObject.transform.Rotate(Vector3.up, -rotX);
            loadedObject.transform.Rotate(Vector3.right, rotY, Space.World);
        }
    }

    private void LoadMesh()
    {
        string objectsDirectory = Path.Combine(Application.persistentDataPath, "Input");
        if (Directory.Exists(objectsDirectory))
        {
            DirectoryInfo dir = new DirectoryInfo(objectsDirectory);
            FileInfo[] fileInfo = dir.GetFiles("*.*");
            List<string> files = new List<string>();
            foreach (FileInfo file in fileInfo)
            {
                if (file.Extension.Contains(".obj"))
                {
                    files.Add(file.ToString());
                }
            }
            foreach (string filePath in files)
            {
                bool listContainsObject = false;
                foreach (GameObject loadedObject in loadedObjects)
                {
                    if (loadedObject.name == Path.GetFileNameWithoutExtension(filePath))
                    {
                        listContainsObject = true;
                    }
                }
                if (listContainsObject)
                {
                    continue;
                }

                if (!File.Exists(filePath))
                {
                    Debug.Log("File doesn't exist.");
                }

                else
                {
                    GameObject newObject = new OBJLoader().Load(filePath);
                    newObject.SetActive(false);
                    loadedObjects.Add(newObject);
                }
            }
            LoadObject(0);
        }
    }

    private void LoadObject(int objectIndex)
    {
        if (loadedObjects.Count > 0)
        {
            foreach (GameObject model in loadedObjects)
            {
                model.SetActive(false);
            }
            loadedObjects[objectIndex].SetActive(true);
            loadedObject = loadedObjects[objectIndex];
        }
    }

    private void SwitchToNextObject()
    {
        if (loadedObjects.Count > 0)
        {
            if (currentIndex < loadedObjects.Count - 1)
            {
                currentIndex++;
            }
            else
            {
                currentIndex = 0;
            }
            LoadObject(currentIndex);
        }
    }

    private void SwitchToPreviousObject()
    {
        if (loadedObjects.Count > 0)
        {
            if (currentIndex > 0)
            {
                currentIndex--;
            }
            else
            {
                currentIndex = loadedObjects.Count - 1;
            }
            LoadObject(currentIndex);
        }
    }

    private void ShowPath(string path)
    {
        string inputDirectory = Path.Combine(Application.persistentDataPath, path);
        if (!Directory.Exists(inputDirectory))
        {
            Directory.CreateDirectory(inputDirectory);
        }
        inputDirectory = inputDirectory.Replace(@"/", @"\");
        System.Diagnostics.Process.Start("explorer.exe", "/select," + inputDirectory);
    }
}
