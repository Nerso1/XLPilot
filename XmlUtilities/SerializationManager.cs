using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLPilot.Models.Containers;
using XLPilot.Models;
using System.IO;

namespace XLPilot.XmlUtilities
{
    /// <summary>
    /// Main manager class for handling serialization operations
    /// </summary>
    public class SerializationManager
    {
        private SerializationData _data;
        private readonly string _mainFilePath;

        public SerializationManager(string mainFilePath)
        {
            _mainFilePath = mainFilePath;
            _data = File.Exists(mainFilePath)
                ? XmlSerializer<SerializationData>.Deserialize(mainFilePath)
                : new SerializationData();
        }

        public SerializationData GetData()
        {
            return _data;
        }

        public void SaveAllData()
        {
            XmlSerializer<SerializationData>.Serialize(_data, _mainFilePath);
        }

        #region XLPaths Operations
        public void SaveXLPaths(string filePath)
        {
            var container = new XLPathsContainer { Items = _data.XLPathsList };
            XmlSerializer<XLPathsContainer>.Serialize(container, filePath);
        }

        public void LoadXLPaths(string filePath)
        {
            var container = XmlSerializer<XLPathsContainer>.Deserialize(filePath);
            _data.XLPathsList = container.Items;
        }

        public void ImportXLPaths(string filePath)
        {
            var container = XmlSerializer<XLPathsContainer>.Deserialize(filePath);
            _data.XLPathsList.AddRange(container.Items);

            // Remove duplicates based on Name property
            _data.XLPathsList = _data.XLPathsList
                .GroupBy(x => x.Name)
                .Select(g => g.First())
                .ToList();
        }
        #endregion

        #region XLIcons Operations
        public void SaveXLIcons(string filePath)
        {
            var container = new XLIconsContainer { Items = _data.XLIcons };
            XmlSerializer<XLIconsContainer>.Serialize(container, filePath);
        }

        public void LoadXLIcons(string filePath)
        {
            var container = XmlSerializer<XLIconsContainer>.Deserialize(filePath);
            _data.XLIcons = container.Items;
        }

        public void ImportXLIcons(string filePath)
        {
            var container = XmlSerializer<XLIconsContainer>.Deserialize(filePath);
            _data.XLIcons.AddRange(container.Items);

            // Remove duplicates
            _data.XLIcons = _data.XLIcons.Distinct().ToList();
        }
        #endregion

        #region OtherIcons Operations
        public void SaveOtherIcons(string filePath)
        {
            var container = new OtherIconsContainer { Items = _data.OtherIcons };
            XmlSerializer<OtherIconsContainer>.Serialize(container, filePath);
        }

        public void LoadOtherIcons(string filePath)
        {
            var container = XmlSerializer<OtherIconsContainer>.Deserialize(filePath);
            _data.OtherIcons = container.Items;
        }

        public void ImportOtherIcons(string filePath)
        {
            var container = XmlSerializer<OtherIconsContainer>.Deserialize(filePath);
            _data.OtherIcons.AddRange(container.Items);

            // Remove duplicates
            _data.OtherIcons = _data.OtherIcons.Distinct().ToList();
        }
        #endregion

        #region Flags Operations
        public void SaveFlags(string filePath)
        {
            var container = new FlagsContainer { Items = _data.Flags };
            XmlSerializer<FlagsContainer>.Serialize(container, filePath);
        }

        public void LoadFlags(string filePath)
        {
            var container = XmlSerializer<FlagsContainer>.Deserialize(filePath);
            _data.Flags = container.Items;

            // Ensure we always have exactly 3 flags
            while (_data.Flags.Count < 3)
                _data.Flags.Add(false);

            if (_data.Flags.Count > 3)
                _data.Flags = _data.Flags.Take(3).ToList();
        }
        #endregion

        #region Dimensions Operations
        public void SaveDimensions(string filePath)
        {
            var container = new DimensionsContainer { Items = _data.Dimensions };
            XmlSerializer<DimensionsContainer>.Serialize(container, filePath);
        }

        public void LoadDimensions(string filePath)
        {
            var container = XmlSerializer<DimensionsContainer>.Deserialize(filePath);
            _data.Dimensions = container.Items;

            // Ensure we always have exactly 2 dimensions
            while (_data.Dimensions.Count < 2)
                _data.Dimensions.Add(0);

            if (_data.Dimensions.Count > 2)
                _data.Dimensions = _data.Dimensions.Take(2).ToList();
        }

        public void SetDimensions(int width, int height)
        {
            _data.Dimensions[0] = width;
            _data.Dimensions[1] = height;
        }

        public (int width, int height) GetDimensions()
        {
            return (_data.Dimensions[0], _data.Dimensions[1]);
        }
        #endregion
    }

}
