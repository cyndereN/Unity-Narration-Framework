using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ExcelDataReader;
using UnityEngine;

public class ExcelReader : MonoBehaviour
{
    public struct ExcelData
    {
        public string speaker;
        public string content;
        public string avatarImageFileName;
        public string vocalAudioFileName;
        public string backgroundImageFileName;
        public string backgroundMusicFileName;
        // Assume only 2 characrers are allow in 1 "frame"
        // todo: dynamically extend the character array in excel?
        public string character1Action;
        public string character1ImageFileName;
		public string character2Action;
		public string character2ImageFileName;
	}


    public static List<ExcelData> ReadExcel(string filePath)
    {
        List<ExcelData> excelData = new List<ExcelData>();
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read)) 
        {
            using (var reader = ExcelReaderFactory.CreateReader(stream))
            {
                do 
                {
                    while (reader.Read())
                    {
                        ExcelData data = new ExcelData();
                        data.speaker = reader.IsDBNull(0) ? string.Empty : reader.GetValue(0)?.ToString();
                        data.content = reader.IsDBNull(1) ? string.Empty : reader.GetValue(1)?.ToString();
						data.avatarImageFileName = reader.IsDBNull(2) ? string.Empty : reader.GetValue(2)?.ToString();
                        data.vocalAudioFileName = reader.IsDBNull(3) ? string.Empty : reader.GetValue(3)?.ToString();
						data.backgroundImageFileName = reader.IsDBNull(4) ? string.Empty : reader.GetValue(4)?.ToString();
                        data.backgroundMusicFileName = reader.IsDBNull(5) ? string.Empty : reader.GetValue(5)?.ToString();
                        data.character1Action = reader.IsDBNull(6) ? string.Empty : reader.GetValue(6)?.ToString();
                        data.character1ImageFileName = reader.IsDBNull(7) ? string.Empty : reader.GetValue(7)?.ToString();
						data.character2Action = reader.IsDBNull(8) ? string.Empty : reader.GetValue(8)?.ToString();
						data.character2ImageFileName = reader.IsDBNull(9) ? string.Empty : reader.GetValue(9)?.ToString();

						excelData.Add(data);
                    }
                }while(reader.NextResult());
            }
        }
        return excelData;
    }

}
