using System;
using System.Linq;
using Core.Common.Helpers;
using Core.Common.Interfaces;
using Core.Common.Items;

namespace BusinessLogicLayer.Reader
{
    public class FileReaderProvider : IFileReaderProvider
    {
        private const string NieprawidloweRozszerzenie = "Nieprawidłowe rozszerzenie";

        private readonly IDataReader _dataReader;
        private readonly IAttributeColumnConverter _attributeColumnConverter;
        private readonly ITrainingObjectsConverter _trainingObjectsConverter;
        private readonly IFileChecker _fileChecker;

        public FileReaderProvider
            (IDataReader dataReader,
            IAttributeColumnConverter attributeColumnConverter,
            ITrainingObjectsConverter trainingObjectsConverter, 
            IFileChecker fileChecker)
        {
            _dataReader = dataReader;
            _attributeColumnConverter = attributeColumnConverter;
            _trainingObjectsConverter = trainingObjectsConverter;
            _fileChecker = fileChecker;
        }

        private Result<InputData> GetDataModel(string[] rawData)
        {
            var dataModel = _dataReader.GetDataModel(rawData);

            if (dataModel.HasErrors())
            {
                dataModel.GetError();
            }
            return dataModel;

        }

        private DataObject[] ConvertRows2DataObjects(
            AttributeDescription[] attributes,
            string[] observations,
            long startId = 0)
        {
            return _trainingObjectsConverter.ConvertRows2DataObjects(attributes, observations, startId);

        }
        
        public Result<FileData> ConvertFile(string filename, string[] data)
        {
            try
            {
                var isFileHasAppriopriateExtension = _fileChecker.IsFileHasAppriopriateExtension(filename);

                if (!isFileHasAppriopriateExtension)
                {
                    return new Result<FileData>(NieprawidloweRozszerzenie);
                }
                var result = new FileData
                {
                    FileName = filename,
                    RawData = data,
                };

                var dataModel = GetDataModel(result.RawData);
                result.Observations = dataModel.Value.Rows;
                result.DataDescription = dataModel.Value.Columns;
                result.Attributes = _attributeColumnConverter.ConvertColumns2Attributes(result.DataDescription).ToArray();
                var length = result.Observations.Length;
                var dataObject = result.Observations.Take(length * 2 / 3).ToArray();
                var testObjects = result.Observations.Except(dataObject).ToArray();
                DataObjectHelper.SchemeObject = null;
                result.DataObjects = ConvertRows2DataObjects(result.Attributes, dataObject);
                result.TestObjects = ConvertRows2DataObjects(result.Attributes, testObjects, result.DataObjects.Max(x => x.Id + 1));

                var joinedSring = string.Empty;
                if (result.RawData != null)
                    joinedSring = result.RawData.Aggregate(joinedSring,
                        (current, nextRow) => current + nextRow + Environment.NewLine);

                var newLineSignLength = Environment.NewLine.Length;

                result.JoinedString = joinedSring.Remove(joinedSring.Length - newLineSignLength, newLineSignLength); ;

                return new Result<FileData>(result);
            }
            catch (Exception e)
            {
                return new Result<FileData>(e);
            }
        }
    }
}
