using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Delta_PLC_Tool
{
    public class Configini
    {
        public string PortName { get; set; } = "COM1";

        public string Baud { get; set; } = "9600";

        public string Databit { get; set; } = "7";

        public string Stopbit { get; set; } = "1";

        public string Parity { get; set; } = "Even";

        public string PortName_2 { get; set; } = "COM2";

        public string Baud_2 { get; set; } = "9600";

        public string Databit_2 { get; set; } = "7";

        public string Stopbit_2 { get; set; } = "1";

        public string Parity_2 { get; set; } = "Even";

        public string IP { get; set; } = "192.168.0.100";

        public string Port { get; set; } = "5091";

        public string para1 { get; set; } = "0";

    }

    public class IniDeserialize
    {
        /// <summary>
        /// 若ini文件中无对应值，就提取默认对象中的值
        /// </summary>
        Configini _defaultT;

        /// <summary>
        /// 序列化后的对象
        /// </summary>
        public Configini Value { get; set; }

        IniFile _iniRW;
        public IniDeserialize(string path, Configini defaultT)
        {
            _iniRW = new IniFile(path);
            Value = defaultT;
            _defaultT = defaultT;
            //Deserialize(out T t);
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <returns></returns>
        public bool Deserialize(out Configini t)
        {
            Value = new Configini();
            Type tInfo = Value.GetType();
            string section = tInfo.Name;
            foreach (PropertyInfo p in tInfo.GetProperties())
            {
                string key = p.Name;
                string iniValue = _iniRW.ReadString(section, key);
                if (iniValue == string.Empty)
                {
                    PropertyInfo pd = _defaultT.GetType().GetProperty(key);
                    iniValue = pd.GetValue(_defaultT, null) as string;
                }
                object value = iniValue.Format(p.PropertyType);
                //object value = (object)(p.PropertyType);
                p.SetValue(Value, value, null);
            }
            t = Value;
            return true;
        }

        /// <summary>
        /// 序列化保存
        /// </summary>
        /// <returns></returns>
        public bool Serialize()
        {
            Type tInfo = Value.GetType();
            string section = tInfo.Name;
            foreach (PropertyInfo p in tInfo.GetProperties())
            {
                string key = p.Name;
                object value = p.GetValue(Value, null);
                _iniRW.WriteValue(section, key, value);
            }
            return true;
        }

    }

    public class IniFile
    {
        public string iniPath;

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filepath);

        [DllImport("kernel32")]

        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filepath);


        public IniFile(string inipath)
        {
            iniPath = inipath;
        }

        public void IniWriteValue(string Section, string Key, string Value)
        {
            WritePrivateProfileString(Section, Key, Value, this.iniPath);
        }

        public string IniReadValue(string Section, string Key)
        {
            StringBuilder temp = new StringBuilder(500);
            int i = GetPrivateProfileString(Section, Key, "", temp, 500, this.iniPath);
            return temp.ToString();
        }

        public bool ExistINIFile()
        {
            return File.Exists(iniPath);
        }

        public virtual bool WriteValue(string section, string key, object value)
        {
            string v = value == null ? "" : value.ToString();
            long ret = WritePrivateProfileString(section, key, v, this.iniPath);
            return true;
        }

        public virtual string ReadString(string section, string key)
        {
            StringBuilder sb = new StringBuilder(255);
            string def = string.Empty;
            int ret = GetPrivateProfileString(section, key, def, sb, 255, this.iniPath);
            return ret == 0 ? def : sb.ToString();
        }

        public int ReadInt(string section, string key)
        {
            return (int)ReadDouble(section, key);
        }

        public short ReadShort(string section, string key)
        {
            return (short)ReadDouble(section, key);
        }

        public byte ReadByte(string section, string key)
        {
            return (byte)ReadDouble(section, key);
        }

        public double ReadDouble(string section, string key)
        {
            double res = 0;
            string sb = ReadString(section, key);
            double.TryParse(sb, out res);
            return res;
        }

        public bool ReadBool(string section, string key)
        {
            bool res = false;
            string sb = ReadString(section, key).Trim();
            if (sb == "1")
            {
                return true;
            }
            else if (sb == "0")
            {
                return false;
            }
            bool.TryParse(sb, out res);
            return res;
        }

    }

    public static class Fomat
    {
        /// <summary>
        /// 将字符串格式化成指定的数据类型
        /// </summary>
        /// <param name="str"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object Format(this string str, Type type)
        {
            if (string.IsNullOrEmpty(str))
                return null;
            if (type == null)
                return str;
            if (type.IsArray)
            {
                Type elementType = type.GetElementType();
                string[] strs = str.Split(new char[] { ';' });
                Array array = Array.CreateInstance(elementType, strs.Length);
                for (int i = 0, c = strs.Length; i < c; ++i)
                {
                    array.SetValue(ConvertSimpleType(strs[i], elementType), i);
                }
                return array;
            }
            return ConvertSimpleType(str, type);
        }

        private static object ConvertSimpleType(object value, Type destinationType)
        {
            object returnValue;
            if ((value == null) || destinationType.IsInstanceOfType(value))
            {
                return value;
            }
            string str = value as string;
            if ((str != null) && (str.Length == 0))
            {
                return null;
            }
            TypeConverter converter = TypeDescriptor.GetConverter(destinationType);
            bool flag = converter.CanConvertFrom(value.GetType());
            if (!flag)
            {
                converter = TypeDescriptor.GetConverter(value.GetType());
            }
            if (!flag && !converter.CanConvertTo(destinationType))
            {
                throw new InvalidOperationException("无法转换成类型：" + value.ToString() + "==>" + destinationType);
            }
            try
            {
                returnValue = flag ? converter.ConvertFrom(null, null, value) : converter.ConvertTo(null, null, value, destinationType);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("类型转换出错：" + value.ToString() + "==>" + destinationType, e);
            }
            return returnValue;
        }

    }
}
