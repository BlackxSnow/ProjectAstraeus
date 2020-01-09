using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public string InjuriesPath = "MedicalData/Injuries";
    public string SpeciesPath = "Species";

    public void LoadObjectData()
    {
        LoadInjuries();
        LoadSpecies();
    }

    [Serializable]
    public struct DataStruct<T>
    {
        public List<T> DataArray;
    }

    private DataStruct<T> LoadArrayedData<T>(string AssetPath, out int FileCount)
    {
        string FolderPath = Path.Combine(Application.streamingAssetsPath, AssetPath);
        DirectoryInfo Info = new DirectoryInfo(FolderPath);
        FileInfo[] Files = Info.GetFiles("*.json");

        DataStruct<T> AllLoadedData = new DataStruct<T>();
        AllLoadedData.DataArray = new List<T>();

        for (int i = 0; i < Files.Length; i++)
        {
            string FilePath = Path.Combine(FolderPath, Files[i].Name);
            string JsonData = File.ReadAllText(FilePath);
            DataStruct<T> LoadedData = JsonUtility.FromJson<DataStruct<T>>(JsonData);

            AllLoadedData.DataArray = Utility.CombineLists(AllLoadedData.DataArray, LoadedData.DataArray);

            Debug.Log($"Loaded '{Files[i].Name}'! {LoadedData.DataArray.Count} object(s) were loaded.");
        }
        FileCount = Files.Length;
        return AllLoadedData;
    }

    #region Injuries
    public void InitInjuries(DataStruct<Medical.Health.Injury> Data)
    {
        Medical.Health.LoadedInjuries = Data.DataArray;
    }
    public void LoadInjuries()
    {
        DataStruct<Medical.Health.Injury> Data = LoadArrayedData<Medical.Health.Injury>(InjuriesPath, out int FileCount);
        InitInjuries(Data);
        Debug.Log($"Finished loading injuries; {FileCount} files and {Data.DataArray.Count} objects loaded");
    }
    #endregion

    #region Species
    public struct SpeciesData
    {
        public List<Species> Species;
    }
    public void InitSpecies(DataStruct<Species> Data)
    {
        
    }
    public void LoadSpecies()
    {
        DataStruct<Species> Data = LoadArrayedData<Species>(SpeciesPath, out int FileCount);
        InitSpecies(Data);
        Debug.Log($"Finished loading species; {FileCount} files and {Data.DataArray.Count} objects loaded");
    }
    #endregion

    private void Awake()
    {
        LoadObjectData();
    }
}
