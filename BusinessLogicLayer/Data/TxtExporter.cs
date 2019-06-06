using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Core.Common.Interfaces;
using Core.Common.Items;

namespace BusinessLogicLayer.Data
{
    public class TxtExporter : ITxtExporter
    {
        private ISaveFileDialog _newSFD { get; set; }
        private IDataObjectsConverter _dataObjectsConverter { get; }
        public TxtExporter(
            ISaveFileDialog sfd,
            IDataObjectsConverter dataObjectsConverter)
        {
            _newSFD = sfd;
            _dataObjectsConverter = dataObjectsConverter;
        }

        public TxtExporter(
            IDataObjectsConverter dataObjectsConverter)
        {
            _dataObjectsConverter = dataObjectsConverter;
        }

        public async Task<Result<bool>> ExportToTxt(DataTable coverResultDataObjects,
            string suffix)
        {
            var bytes = GetTxtStream(coverResultDataObjects);
            return await SaveFileDialog(suffix, bytes);
        }

        public byte[] GetTxtStream(DataTable matrix)
        {
            var data = new List<byte>();
            foreach (DataRow line in matrix.Rows)
            {
                var array = line.ItemArray.ToList();
                array.RemoveAt(0);
                
                var row = array.Select(attribute => $"{attribute}\t").ToList();
                row.Add("\r\n");

                var rowString = string.Join(string.Empty, row);

                data.AddRange(rowString.Select(c => (byte)c).ToArray());
            }

            return data.ToArray();
        }


        //private void WriteToFile(DataTable matrix, string fileName)
        //{
        //    using (StreamWriter file = new StreamWriter(fileName))
        //    {
        //        foreach (DataRow line in matrix.Rows)
        //        {
        //            var array = line.ItemArray.ToList();
        //            array.RemoveAt(0);
        //            foreach (var attribute in line.ItemArray)
        //            {
        //                file.Write(attribute + "\t");
        //            }
        //            file.WriteLine("\n\r");
        //        }
        //    }

        //}

        private async Task<Result<bool>> SaveFileDialog(string suffix, byte[] bytes)
        {
            try
            {
                _newSFD.FileName = DateTime.Now.ToFileTimeUtc() + suffix;
                _newSFD.DefaultExt = ".txt";
                _newSFD.Filter = "Txt (.txt)|*.txt";

                var sfdResult = await _newSFD.ShowDialog(bytes);

                return new Result<bool>(sfdResult);
            }
            catch (Exception e)
            {
                return new Result<bool>(e);
            }

        }
    }
}
