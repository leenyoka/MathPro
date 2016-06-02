using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.IO;
using System.IO.IsolatedStorage;
using System.Collections.Generic;

namespace MathPro
{
    public class SettingsAssistant
    {
        #region Properties
        StorageAssistant _storageAssistant = StorageAssistant.Instance;

        string _SettingsFile = "settings.txt";

        List<valuePair> _settingsInfo = new List<valuePair>();


        #endregion Properties

        #region Constructor

        public SettingsAssistant()
        {
            //_SettingsFile = name;
            ReadSaved();
        }

        #endregion Constructor

        #region Methods

        //Reads the settings from Isolated Storage
        public void ReadSaved()
        {
            //messages that are "0" dont count;

            if (!_storageAssistant.CreateFile(_SettingsFile))
            {
                //there is nothing on file
                _storageAssistant.WriteToFile("User=username_", System.IO.FileMode.Create, _SettingsFile);
                //need to Add the new one
            }

            _settingsInfo = ConvertToValuePair(_storageAssistant.ReadFile(_SettingsFile));



        }

        //Returns a list of the valuePairs broken down
        //from the passed string
        public List<valuePair> ConvertToValuePair(string text)
        {
            List<valuePair> settingsInfo = new List<valuePair>();


            string[] setters = text.Split('_');

            foreach (string pair in setters)
            {
                if (pair != "\r\n")
                {
                    valuePair aPair = new valuePair(pair);
                    settingsInfo.Add(aPair);
                }
            }
            this._settingsInfo = settingsInfo;

            return settingsInfo;
        }


        //Converts the passed valuePair info to name=value formart
        public string convertToText(List<valuePair> pairs)
        {
            string text = "";

            foreach (valuePair mypair in pairs)
            {
                text += mypair.GetValuePairFormartedString() + "_";
            }

            return text;
        }


        //Changes a variable matching the name
        //of the passed valuepair in settings, assolated storage
        public bool changeSetting(valuePair myPair)
        {
            if (!_storageAssistant.CreateFile(_SettingsFile))
            {
                //there is nothing on file
                _storageAssistant.WriteToFile("user=username", System.IO.FileMode.Create, _SettingsFile);
                //need to Add the new one
                return false;
            }
            else
            {
                _settingsInfo = ConvertToValuePair(_storageAssistant.ReadFile(_SettingsFile));

                int index = getValuePairFromSettingsIndex(myPair.Name);

                if (index == -1)
                {
                    _settingsInfo.Add(myPair);
                    _storageAssistant.WriteToFile(convertToText(_settingsInfo), System.IO.FileMode.Create, _SettingsFile);
                    //notFoud
                    return false;
                }
                else
                {
                    _settingsInfo[index] = myPair;

                    _storageAssistant.WriteToFile(convertToText(_settingsInfo), System.IO.FileMode.Create, _SettingsFile);
                    return true;
                }
            }
        }

        //Returns the index of the valuepair
        //in settingsInfor that matches the passed string
        public int getValuePairFromSettingsIndex(string name)
        {
            for (int m = 0; m < _settingsInfo.Count; m++)
            {
                valuePair mypair = _settingsInfo[m];
                if (mypair.Name.ToLower() == name.ToLower())
                {
                    return m;
                }
            }
            return -1;
        }

        //Returns the valuepair with name matching the passed name
        public valuePair getValuePairFromSettings(string name)
        {
            for (int m = 0; m < _settingsInfo.Count; m++)
            {
                valuePair mypair = _settingsInfo[m];
                if (mypair.Name.ToLower() == name.ToLower())
                {
                    return mypair;
                }
            }
            return new valuePair("none", "none");
        }

        public bool ValuePairExists(string name)
        {
            valuePair pair = getValuePairFromSettings(name);

            if (pair.Name == "none")
                return false;
            return true;
        }

        #endregion Methods
    }

    public class valuePair
    {
        #region Properties
        string _name;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        string _value;

        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }

        #endregion Properties

        #region Constructor

        public valuePair(string name, string value)
        {
            _name = name;
            _value = value;
        }

        public valuePair(string ValuePairFormartedString)
        {
            string[] peices = ValuePairFormartedString.Split('=');

            this.Name = peices[0];
            this.Value = peices[1];
        }

        #endregion Constructor

        #region Methods

        public string GetValuePairFormartedString()
        {
            return this.Name + "=" + this.Value;
        }

        #endregion Methods
    }

    public sealed class StorageAssistant
    {
        private static readonly StorageAssistant _instance = new StorageAssistant();


        private StorageAssistant() { }

        public static StorageAssistant Instance
        {
            get
            {
                return _instance;
            }
        }

        #region Properties
        IsolatedStorageFile _myFile = IsolatedStorageFile.GetUserStoreForApplication();
        string _sFile;//name of the file


        #endregion Properties


        #region Methods

        //creates file in isolated storage
        public bool CreateFile(string fileName)
        {
            _sFile = fileName;

            if (!_myFile.FileExists(_sFile))//check if file exists
            {
                IsolatedStorageFileStream dataFile = _myFile.CreateFile(_sFile);//create file
                dataFile.Close();
                return false;
            }
            return true;
        }

        //Writes the passed text to the passed file
        //in isolated storage
        public bool WriteToFile(string text, FileMode Mode, string fileName)
        {
            _sFile = fileName;
            CreateFile(fileName);

            if (fileName.Trim() != "")
            {
                string msg = text;

                StreamWriter sw = new StreamWriter(new IsolatedStorageFileStream(_sFile, Mode, _myFile));
                sw.WriteLine(msg); //Wrting to the file
                sw.Close();

                return true;
            }
            else
            {
                return false;
            }
        }


        //Reads the file with the passed name
        //in isolated storage
        public string ReadFile(string fileName)
        {

            _sFile = fileName;

            if (_myFile.FileExists(_sFile))
            {

                StreamReader reader = new StreamReader(new IsolatedStorageFileStream(_sFile, FileMode.Open, _myFile));//read file
                string rawData = reader.ReadToEnd();//read file
                reader.Close();

                return rawData.ToString();
            }
            return "";
        }

        #endregion Methods

    }
}
