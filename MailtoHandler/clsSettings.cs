using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.IO;
using System.Security.Cryptography;
using System.Runtime.Serialization.Formatters.Binary;
using System.Drawing.Printing;

namespace MailtoHandler
{
    /// <summary>
    /// Class: Settings
    /// 
    /// Class to perform all operations on XML Configuration files in a self-contained manner.
    /// Settings may only be accessed through public methods on this class.
    /// </summary>
    public class Settings
    {
        private DataSet dsSettings;
        private string configFilePath;
        private bool bolChanged;

        /// <summary>
        /// Determines whether there are any unsaved changes to the settings
        /// </summary>
        /// <returns>true if unsaved changes exist</returns>
        public bool Changed()
        {
            return bolChanged;
        }

        /// <summary>
        /// Discard all changes since last saveXML
        /// </summary>
        public void Reset()
        {
            dsSettings.Clear();
            dsSettings = new DataSet();
            dsSettings.Tables.Add("Settings");
            dsSettings.Tables["Settings"].Columns.Add("Setting");
            dsSettings.Tables["Settings"].Columns.Add("Value");
            dsSettings.ReadXml(configFilePath);
        }

        /// <summary>
        /// Opens/creates a settings file
        /// </summary>
        /// <param name="CreateNewFile">true: create a new file, false: open existing file</param>
        /// <param name="FileName">The path and filename of the file to open or create</param>
        public Settings(bool CreateNewFile = false, string FileName = "config.xml", string FilePath = null)
        {

            if (FilePath != null)
            {
                configFilePath = Path.Combine(FilePath, FileName);
            }
                
            else
            {
                configFilePath = Path.Combine(Environment.CurrentDirectory, FileName);
            }
            if (!CreateNewFile)
            {
                if (!File.Exists(configFilePath))
                {
                    throw new Exception("Cannot find configuration file");

                }
            }
            try
            {
                dsSettings = new DataSet();
                dsSettings.Tables.Add("Settings");
                dsSettings.Tables["Settings"].Columns.Add("Setting");
                dsSettings.Tables["Settings"].Columns.Add("Value");
                if (File.Exists(configFilePath)) dsSettings.ReadXml(configFilePath);
            }
            catch
            {
                throw new Exception("Error reading configuration file");
            }
        }

        /// <summary>
        /// Read all settings from file
        /// </summary>
        /// <returns>Array of SettingItem[]</returns>
        public SettingItem[] GetSettings()
        {
            SettingItem[] sItems = new SettingItem[dsSettings.Tables["Settings"].Rows.Count];
            int itemCount = 0;
            foreach (DataRow dRow in dsSettings.Tables["Settings"].Rows)
            {
                sItems[itemCount] = new SettingItem(dRow["Setting"].ToString(), dRow["Value"].ToString());
                itemCount++;
            }
            return sItems;
        }
        public string Get(string Setting, string Default = null)
        {
            return _get(Setting, false, Default);
        }
        /// <summary>
        /// Read specified setting, decrypt if requested.
        /// </summary>
        /// <param name="Setting">The name of the setting to return</param>
        /// <param name="Encrypted">Whether to decrypt the value</param>
        /// <returns>The setting's value</returns>
        public string Get(string Setting, bool Encrypted, string Default = null)
        {
            return _get(Setting, Encrypted, Default);
        }
        protected virtual string _get(string Setting, bool Encrypted = false, string Default = null) {
            DataRow[] dResult = dsSettings.Tables["Settings"].Select("Setting='" + Setting + "'");
            if (dResult.Count() == 1)
            {
                if (Encrypted) return Decrypt(dResult[0]["Value"].ToString(), true);
                return dResult[0]["Value"].ToString();
            }
            if (Default != null) return Default;
            throw new Exception("Setting does not exist");
        }

        /// <summary>
        /// Set the specified setting, encrypting if requested
        /// </summary>
        /// <param name="Setting">The name of the setting to set</param>
        /// <param name="Value">The value of the setting</param>
        /// <param name="Encrypted">Whether to encrypt the value</param>
        public void Set(string Setting, string Value, bool Encrypted = false)
        {
            DataRow[] dResult = dsSettings.Tables["Settings"].Select("Setting='" + Setting + "'");
            if (dResult.Count() == 1)
            {
                DataRow dRow = dResult[0];
                if (Encrypted)
                {
                    dRow["Value"] = Encrypt(Value, true);
                }
                else
                {
                    dRow["Value"] = Value;
                }
                bolChanged = true;
                return;
            } else if(dResult.Count() == 0) {
                DataRow dRow = dsSettings.Tables["Settings"].NewRow();
                dRow["Setting"] = Setting;
                if (Encrypted)
                {
                    dRow["Value"] = Encrypt(Value, true);
                }
                else
                {
                    dRow["Value"] = Value;
                }
                dsSettings.Tables["Settings"].Rows.Add(dRow);
                bolChanged = true;
                return;
            }
            throw new Exception("Error writing setting");
        }

        /// <summary>
        /// Saves settings to XML file
        /// </summary>
        /// <param name="fileName">(Optional) Path and filename, will use original filename if not specified</param>
        public void saveXML(string fileName = null) {
            bolChanged = false;
            
            if (fileName != null)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(fileName));
                dsSettings.WriteXml(fileName);
                return;
            }
            Directory.CreateDirectory(Path.GetDirectoryName(configFilePath));
            dsSettings.WriteXml(configFilePath);
        }

        /// <summary>
        /// Encrypts data.
        /// </summary>
        /// <param name="toEncrypt">The string to encrypt</param>
        /// <param name="useHashing">Whether to use hashing</param>
        /// <returns>Encrypted string</returns>
        public static string Encrypt(string toEncrypt, bool useHashing)
        {
            byte[] keyArray;
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);

            
            string key = "AK$pa$$w0rdS3Cr£t1744";
            //System.Windows.Forms.MessageBox.Show(key);
            //If hashing use get hashcode regards to your key
            if (useHashing)
            {
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                //Always release the resources and flush data
                // of the Cryptographic service provide. Best Practice

                hashmd5.Clear();
            }
            else
                keyArray = UTF8Encoding.UTF8.GetBytes(key);

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            //set the secret key for the tripleDES algorithm
            tdes.Key = keyArray;
            //mode of operation. there are other 4 modes.
            //We choose ECB(Electronic code Book)
            tdes.Mode = CipherMode.ECB;
            //padding mode(if any extra byte added)

            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateEncryptor();
            //transform the specified region of bytes array to resultArray
            byte[] resultArray =
              cTransform.TransformFinalBlock(toEncryptArray, 0,
              toEncryptArray.Length);
            //Release resources held by TripleDes Encryptor
            tdes.Clear();
            //Return the encrypted data into unreadable string format
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }
        private static string Decrypt(string cipherString, bool useHashing)
        {
            byte[] keyArray;
            //get the byte code of the string

            byte[] toEncryptArray = Convert.FromBase64String(cipherString);

            string key = "AK$pa$$w0rdS3Cr£t1744";

            if (useHashing)
            {
                //if hashing was used get the hash code with regards to your key
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                //release any resource held by the MD5CryptoServiceProvider

                hashmd5.Clear();
            }
            else
            {
                //if hashing was not implemented get the byte code of the key
                keyArray = UTF8Encoding.UTF8.GetBytes(key);
            }

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            //set the secret key for the tripleDES algorithm
            tdes.Key = keyArray;
            //mode of operation. there are other 4 modes. 
            //We choose ECB(Electronic code Book)

            tdes.Mode = CipherMode.ECB;
            //padding mode(if any extra byte added)
            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateDecryptor();
            try
            {
                byte[] resultArray = cTransform.TransformFinalBlock(
                                     toEncryptArray, 0, toEncryptArray.Length);

                  //Release resources held by TripleDes Encryptor                
                tdes.Clear();
               //return the Clear decrypted TEXT
                 return UTF8Encoding.UTF8.GetString(resultArray);
            }
            catch
            {
                tdes.Clear();
                return "";
            }
        }
    }

    /// <summary>
    /// Class to hold a setting's name and value.
    /// </summary>
    public class SettingItem
    {
        public string Setting;
        public string Value;

        public SettingItem(string sSetting, string sValue)
        {
            Setting = sSetting;
            Value = sValue;
        }
    }

    public class SerializePrinterSetting
    {
        public string SettingToString(PrinterSettings settings)
        {
            if (settings == null)
                return null;

            var bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, settings);
                return Convert.ToBase64String(ms.ToArray());
            }
        }

        public PrinterSettings SettingFromString(string base64)
        {
            try
            {
                BinaryFormatter bf = new BinaryFormatter();
                using (var ms = new MemoryStream(Convert.FromBase64String(base64)))
                {
                    return (PrinterSettings)bf.Deserialize(ms);
                }
            }
            catch (Exception)
            {
                return new PrinterSettings();
            }
        }
    }
}
