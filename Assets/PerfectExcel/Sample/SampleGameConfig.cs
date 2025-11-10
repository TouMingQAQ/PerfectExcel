
using System.IO;
using PerfectExcel.Core;
using UnityEngine;

namespace PerfectExcel.Sample
{
    [CreateAssetMenu(menuName = "Excel/Sample/GameConfig",fileName = "GameConfig")]
    public class SampleGameConfig : ScriptableObject
    {
        public string directoryPath = @".\";

        public SampleExcel sampleExcel;
        [ContextMenu("Read")]
        public void Read()
        {
            ExcelReader reader = new(directoryPath);
            reader.Read(sampleExcel);
        }
        [ContextMenu("Open")]
        public void Open()
        {
            Application.OpenURL(directoryPath);
        }
        #if UNITY_EDITOR
        [ContextMenu("Select")]
        public void SelectDirectory()
        {
            directoryPath = ExcelReader.SelectDirectory();
        }
        #endif
    }
}

