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
						excelData.Add(data);
                    }
                }while(reader.NextResult());
            }
        }
        return excelData;
    }

}
