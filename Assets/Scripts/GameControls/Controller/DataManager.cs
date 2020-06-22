using System.Threading;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using MoreLinq;
using Nito.AsyncEx;

public class DataManager : MonoBehaviour
{
    public static AsyncManualResetEvent DataLoaded = new AsyncManualResetEvent();

    public const string InjuryIconsPath = "Graphics/Sprites/Medical/Injuries";
    public const string InjuriesPath = "MedicalData/Injuries";
    public const string SkillsPath = "SkillsData";
    public const string SpeciesPath = "Species";


    public void LoadObjectData()
    {
        ItemTypes.LoadData();
        LoadInjuries();
        LoadSkills();
        LoadSpecies();
        DataLoaded.Set();
    }

    [Serializable]
    public struct DataStruct<T>
    {
        public List<T> DataArray;
    }
    
    public static Sprite LoadSprite(string FileName, string AssetPath)
    {
        string FolderPath = Path.Combine(Application.streamingAssetsPath, AssetPath);
        DirectoryInfo Info = new DirectoryInfo(FolderPath);
        FileInfo[] File = Info.GetFiles(FileName);

        if (File.Length != 1) throw new Exception($"{File.Length} files were found for search {FileName}");

        byte[] ImageBytes = System.IO.File.ReadAllBytes(Path.Combine(FolderPath, File[0].Name));
        Texture2D Tex = new Texture2D(24,24, TextureFormat.ARGB32, true);
        Tex.LoadImage(ImageBytes);

        Sprite Result = Sprite.Create(Tex, new Rect(0, 0, 128, 128), new Vector2(0, 1));
        return Result;
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
            DataStruct<T> LoadedData = Newtonsoft.Json.JsonConvert.DeserializeObject<DataStruct<T>>(JsonData);
            //DataStruct<T> LoadedData = JsonUtility.FromJson<DataStruct<T>>(JsonData);

            AllLoadedData.DataArray = Utility.Collections.CombineLists(AllLoadedData.DataArray, LoadedData.DataArray);

            Debug.Log($"Loaded '{Files[i].Name}'! {LoadedData.DataArray.Count} object(s) were loaded.");
        }
        FileCount = Files.Length;
        return AllLoadedData;
    }

    #region Injuries
    public void InitInjuries(DataStruct<Medical.Injury> Data)
    {
        Medical.Health.LoadedInjuries = Data.DataArray;
        Medical.Injury.MaxSeverity = Medical.Health.LoadedInjuries.MaxBy(i => i.SeverityCost).First().SeverityCost;
        Medical.Injury.MinSeverity = Medical.Health.LoadedInjuries.MinBy(i => i.SeverityCost).First().SeverityCost;
    }
    public void LoadInjuries()
    {
        DataStruct<Medical.Injury> Data = LoadArrayedData<Medical.Injury>(InjuriesPath, out int FileCount);
        InitInjuries(Data);
        Debug.Log($"Finished loading injuries; {FileCount} files and {Data.DataArray.Count} objects loaded");
    }
    #endregion

    #region Skills
    public void InitSkills(DataStruct<StatsAndSkills.Skill> Data)
    {
        foreach(StatsAndSkills.Skill skill in Data.DataArray)
        {
            StatsAndSkills.LoadedSkills.Add(skill.SkillName, skill);
        }
    }
    public void LoadSkills()
    {
        DataStruct<StatsAndSkills.Skill> Data = LoadArrayedData<StatsAndSkills.Skill>(SkillsPath, out int FileCount);
        InitSkills(Data);
        Debug.Log($"Finished loading skills; {FileCount} files and {Data.DataArray.Count} objects loaded");
    }
    #endregion

    #region Species
    public void InitSpecies(DataStruct<Species> Data)
    {
        Species.LoadedSpecies = Data.DataArray;
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
