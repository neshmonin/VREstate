using System;
using System.Collections.Generic;
using System.Linq;

namespace Vre.Server.BusinessLogic
{
    /// <summary>
    /// Container used to pass specific subset of data between client and server.
    /// </summary>
    public class ClientData : Dictionary<string, object> 
    {
        /// <summary>
        /// Default c'tor.
        /// </summary>
        public ClientData() : base() { }

        /// <summary>
        /// C'tor to process output of JSON deserializer
        /// </summary>
        public ClientData(Dictionary<string, object> source) : base(source)         
        {
            Dictionary<string, ClientData> itemsToReplace = new Dictionary<string, ClientData>();
            Dictionary<string, ClientData[]> arraysToReplace = new Dictionary<string, ClientData[]>();
            Dictionary<string, int[]> intArraysToReplace = new Dictionary<string, int[]>();

            foreach (KeyValuePair<string, object> kvp in this)
            {
                Dictionary<string, object> sub = kvp.Value as Dictionary<string, object>;
                if (sub != null)
                {
                    itemsToReplace.Add(kvp.Key, new ClientData(sub));
                }
                else
                {
                    object[] suba = kvp.Value as object[];  // reflected as object array only!!!
                    if (suba != null)
                    {
                        int idx = suba.Length;
                        if (idx > 0)
                        {
                            Type t = suba[0].GetType();
                            if (t.Equals(typeof(int)))
                            {
                                int[] items = new int[idx];
                                // unboxing
                                for (idx--; idx >= 0; idx--) items[idx] = (int)suba[idx];
                                intArraysToReplace.Add(kvp.Key, items);
                            }
                            else
                            {
                                ClientData[] items = new ClientData[idx];
                                for (idx--; idx >= 0; idx--)
                                {
                                    Dictionary<string, object> subsub = suba[idx] as Dictionary<string, object>;
                                    if (subsub != null) items[idx] = new ClientData(subsub);
                                }
                                arraysToReplace.Add(kvp.Key, items);
                            }
                        }
                        else
                        {
                            arraysToReplace.Add(kvp.Key, new ClientData[0]);
                        }
                    }
                }
            }
            foreach (KeyValuePair<string, ClientData> kvp in itemsToReplace) this[kvp.Key] = kvp.Value;
            foreach (KeyValuePair<string, ClientData[]> kvp in arraysToReplace) this[kvp.Key] = kvp.Value;
            foreach (KeyValuePair<string, int[]> kvp in intArraysToReplace) this[kvp.Key] = kvp.Value;
        }

        public void Merge(ClientData mergingData) { Merge(mergingData, false); }

        public void Merge(ClientData mergingData, bool overwriteExistingKeys)
        {
            foreach (string k in mergingData.Keys)
            {
                if (ContainsKey(k))
                {
                    if (overwriteExistingKeys) this[k] = mergingData[k];
                    continue;
                }
                Add(k, mergingData[k]);
            }
        }

        /// <summary>
        /// Get inner ClientData object by name; returns empty container if no data exists.
        /// </summary>
        public ClientData GetNextLevelDataItem(string subLevelName)
        {
            ClientData result = null;

            object value;
            if (TryGetValue(subLevelName, out value))
            {
                result = value as ClientData;
            }

            if (null == result) return new ClientData();  // avoid returning nulls for the sake of parsing simplicity

            return result;
        }

        /// <summary>
        /// Get inner ClientData object by name; returns empty container if no data exists.
        /// </summary>
        public ClientData[] GetNextLevelDataArray(string subLevelName)
        {
            ClientData[] result = null;

            object value;
            if (TryGetValue(subLevelName, out value))
            {
                result = value as ClientData[];
            }

            if (null == result) return new ClientData[0];  // avoid returning nulls for the sake of parsing simplicity

            return result;
        }

        public string GetProperty(string propertyName, string defaultValue)
        {
            bool read;
            return GetProperty(propertyName, defaultValue, out read);
        }

        public byte[] GetProperty(string propertyName, byte[] defaultValue, out bool read)
        {
            byte[] result = defaultValue;

            object value;
            read = false;
            if (TryGetValue(propertyName, out value))
            {
                result = value as byte[];
                if (null == result)
                {
                    int[] ir = value as int[];
                    if (ir != null)
                    {
                        result = new byte[ir.Length];
                        for (int idx = ir.Length - 1; idx >= 0; idx--) result[idx] = (byte)ir[idx];
                    }
                    else
                    {
                        result = defaultValue;
                    }
                }
                else
                {
                    read = true;
                }
            }

            return result;
        }

        public string GetProperty(string propertyName, string defaultValue, out bool read)
        {
            string result = defaultValue;

            object value;
            read = false;
            if (TryGetValue(propertyName, out value))
            {
                result = value as string;
                if (null == result) result = defaultValue;
                else read = true;
            }

            return result;
        }

        public string[] GetProperty(string propertyName, string[] defaultValue, out bool read)
        {
            string[] result = defaultValue;

            object value;
            read = false;
            if (TryGetValue(propertyName, out value))
            {
                result = value as string[];
                if (null == result) result = defaultValue;
                else read = true;
            }

            return result;
        }

        public bool GetProperty(string propertyName, bool defaultValue)
        {
            bool result = defaultValue;

            object value;
            if (TryGetValue(propertyName, out value))
            {
                if (value is bool)
                {
                    result = (bool)value;
                }
                else if (value is string)
                {
                    if (!bool.TryParse(value as string, out result)) result = defaultValue;
                }
            }

            return result;
        }

        public int GetProperty(string propertyName, int defaultValue)
        {
            int result = defaultValue;

            object value;
            if (TryGetValue(propertyName, out value))
            {
                if (value is int)
                {
                    result = (int)value;
                }
                else if (value is string)
                {
                    if (!int.TryParse(value as string, out result)) result = defaultValue;
                }
            }

            return result;
        }

        public double GetProperty(string propertyName, double defaultValue)
        {
            double result = defaultValue;

            object value;
            if (TryGetValue(propertyName, out value))
            {
                if (value is decimal)
                {
                    result = (double)(decimal)value;
                }
                else if (value is double)
                {
                    result = (double)value;
                }
                else if (value is int)
                {
                    result = (int)value;
                }
                else if (value is long)
                {
                    result = (long)value;
                }
                else if (value is string)
                {
                    if (!double.TryParse(value as string, out result)) result = defaultValue;
                }
            }

            return result;
        }

        public decimal GetProperty(string propertyName, decimal defaultValue)
        {
            decimal result = defaultValue;

            object value;
            if (TryGetValue(propertyName, out value))
            {
                if (value is decimal)
                {
                    result = (decimal)value;
                }
                else if (value is double)
                {
                    result = (decimal)(double)value;
                }
                else if (value is int)
                {
                    result = (int)value;
                }
                else if (value is long)
                {
                    result = (long)value;
                }
                else if (value is string)
                {
                    if (!decimal.TryParse(value as string, out result)) result = defaultValue;
                }
            }

            return result;
        }

        public DateTime GetProperty(string propertyName, DateTime defaultValue)
        {
            bool read;
            return GetProperty(propertyName, defaultValue, out read);
        }

        public DateTime GetProperty(string propertyName, DateTime defaultValue, out bool read)
        {
            DateTime result = defaultValue;

            object value;
            read = false;
            if (TryGetValue(propertyName, out value))
            {
                if (value is DateTime)
                {
                    result = (DateTime)value;
                    read = true;

                    // TODO: Deal with time zone!!!
                    if (result.Kind == DateTimeKind.Utc)
                    {
                        result = DateTime.SpecifyKind(result, DateTimeKind.Local);
                        if (result.IsDaylightSavingTime()) result = result.AddHours(-4);
                        else result = result.AddHours(-5);
                    }
                }
                else if (value is string)
                {
                    if (!DateTime.TryParse(value as string, out result)) result = defaultValue;
                    else read = true;
                }
            }

            return result;
        }

        public DateTime? GetProperty(string propertyName, DateTime? defaultValue)
        {
            bool read;
            return GetProperty(propertyName, defaultValue, out read);
        }

        public DateTime? GetProperty(string propertyName, DateTime? defaultValue, out bool read)
        {
            DateTime? result = defaultValue;

            object value;
            read = false;
            if (TryGetValue(propertyName, out value))
            {
                if (value is DateTime)
                {
                    DateTime intermediate = (DateTime)value;

                    // TODO: Deal with time zone!!!
                    if (intermediate.Kind == DateTimeKind.Utc)
                    {
                        intermediate = DateTime.SpecifyKind(intermediate, DateTimeKind.Local);
                        if (intermediate.IsDaylightSavingTime()) intermediate = intermediate.AddHours(-4);
                        else intermediate = intermediate.AddHours(-5);
                    }

                    result = intermediate;
                    read = true;
                }
                else if (value is string)
                {
                    DateTime intermediate;
                    if (DateTime.TryParse(value as string, out intermediate))
                    {
                        result = intermediate;
                        read = true;
                    }
                    else
                    {
                        result = defaultValue;
                    }
                }
                else if (null == value)
                {
                    result = null;
                    read = true;
                }
            }

            return result;
        }

        public T GetProperty<T>(string propertyName, T defaultValue) where T : struct, IConvertible
        {
            T result = defaultValue;

            object value;
            if (TryGetValue(propertyName, out value))
            {
                if (value is int)
                {
                    result = (T)Enum.ToObject(typeof(T), (int)value);
                }
                else if (value is string)
                {
                    if (!Enum.TryParse<T>(value as string, out result)) result = defaultValue;
                }
            }

            return result;
        }

        public static string ConvertProperty<T>(T propertyValue) where T : struct, IConvertible
        {
            return Enum.GetName(typeof(T), propertyValue);
        }

        /// <summary>
        /// Update string property in business object.
        /// <para>If ClientData has no related value status is considered <b>unchanged</b>.</para>
        /// </summary>
        /// <param name="propertyName">Property name in ClientData store.</param>
        /// <param name="propertyValue">Current BO's property value.</param>
        /// <param name="changed">Value updated to <b>true</b> if property value changed; 
        /// value <b>not changed</b> if property value is same. This allows iterative calls with
        /// minimal surrounded logic.</param>
        /// <returns>Updated or same property value.</returns>
        public string UpdateProperty(string propertyName, string propertyValue, ref bool changed)
        {
            bool read;
            string result = GetProperty(propertyName, propertyValue, out read);
            if (read)
            {
                if (!result.Equals(propertyValue)) changed = true;
            }

            return result;
        }

        /// <summary>
        /// Update string array property in business object.
        /// <para>If ClientData has no related value status is considered <b>unchanged</b>.</para>
        /// </summary>
        /// <param name="propertyName">Property name in ClientData store.</param>
        /// <param name="propertyValue">Current BO's property value.</param>
        /// <param name="changed">Value updated to <b>true</b> if property value changed; 
        /// value <b>not changed</b> if property value is same. This allows iterative calls with
        /// minimal surrounded logic.</param>
        /// <returns>Updated or same property value.</returns>
        public string[] UpdateProperty(string propertyName, string[] propertyValue, ref bool changed)
        {
            bool read;
            string[] result = GetProperty(propertyName, propertyValue, out read);
            if (read)
            {
                int diff = propertyValue.Length;
                if (diff == result.Length)
                {
                    foreach (string v in propertyValue)
                        if (result.Contains(v)) diff--;

                    if (diff != 0) changed = true;
                }
                else
                {
                    changed = true;
                }
            }

            return result;
        }

        /// <summary>
        /// Update floating point property in business object.
        /// <para>If ClientData has no related value or value is unreadable status is considered <b>unchanged</b>.</para>
        /// </summary>
        /// <param name="propertyName">Property name in ClientData store.</param>
        /// <param name="propertyValue">Current BO's property value.</param>
        /// <param name="changed">Value updated to <b>true</b> if property value changed; 
        /// value <b>not changed</b> if property value is same. This allows iterative calls with
        /// minimal surrounded logic.</param>
        /// <returns>Updated or same property value.</returns>
        public double UpdateProperty(string propertyName, double propertyValue, ref bool changed)
        {
            double result = GetProperty(propertyName, propertyValue);
            if (result != propertyValue) changed = true;
            return result;
        }

        /// <summary>
        /// Update integer property in business object.
        /// <para>If ClientData has no related value or value is unreadable status is considered <b>unchanged</b>.</para>
        /// </summary>
        /// <param name="propertyName">Property name in ClientData store.</param>
        /// <param name="propertyValue">Current BO's property value.</param>
        /// <param name="changed">Value updated to <b>true</b> if property value changed; 
        /// value <b>not changed</b> if property value is same. This allows iterative calls with
        /// minimal surrounded logic.</param>
        /// <returns>Updated or same property value.</returns>
        public int UpdateProperty(string propertyName, int propertyValue, ref bool changed)
        {
            int result = GetProperty(propertyName, propertyValue);
            if (result != propertyValue) changed = true;
            return result;
        }

        /// <summary>
        /// Update boolean property in business object.
        /// <para>If ClientData has no related value or value is unreadable status is considered <b>unchanged</b>.</para>
        /// </summary>
        /// <param name="propertyName">Property name in ClientData store.</param>
        /// <param name="propertyValue">Current BO's property value.</param>
        /// <param name="changed">Value updated to <b>true</b> if property value changed; 
        /// value <b>not changed</b> if property value is same. This allows iterative calls with
        /// minimal surrounded logic.</param>
        /// <returns>Updated or same property value.</returns>
        public bool UpdateProperty(string propertyName, bool propertyValue, ref bool changed)
        {
            bool result = GetProperty(propertyName, propertyValue);
            if (result != propertyValue) changed = true;
            return result;
        }

        /// <summary>
        /// Update time property in business object.
        /// <para>If ClientData has no related value or value is unreadable status is considered <b>unchanged</b>.</para>
        /// </summary>
        /// <param name="propertyName">Property name in ClientData store.</param>
        /// <param name="propertyValue">Current BO's property value.</param>
        /// <param name="changed">Value updated to <b>true</b> if property value changed; 
        /// value <b>not changed</b> if property value is same. This allows iterative calls with
        /// minimal surrounded logic.</param>
        /// <returns>Updated or same property value.</returns>
        public DateTime UpdateProperty(string propertyName, DateTime propertyValue, ref bool changed)
        {
            bool read;
            DateTime result = GetProperty(propertyName, propertyValue, out read);

            if (read)
            {
                if (!isTimeEqual(result, propertyValue)) changed = true;
            }

            return result;
        }

        /// <summary>
        /// Update time property in business object.
        /// <para>If ClientData has no related value or value is unreadable status is considered <b>unchanged</b>.</para>
        /// </summary>
        /// <param name="propertyName">Property name in ClientData store.</param>
        /// <param name="propertyValue">Current BO's property value.</param>
        /// <param name="changed">Value updated to <b>true</b> if property value changed; 
        /// value <b>not changed</b> if property value is same. This allows iterative calls with
        /// minimal surrounded logic.</param>
        /// <returns>Updated or same property value.</returns>
        public DateTime? UpdateProperty(string propertyName, DateTime? propertyValue, ref bool changed)
        {
            bool read;
            DateTime? result = GetProperty(propertyName, propertyValue, out read);

            if (read)
            {
                if (!isTimeEqual(result, propertyValue)) changed = true;
            }

            return result;
        }

        /// <summary>
        /// Update enumerable property in business object.
        /// <para>If ClientData has no related value or value is unreadable status is considered <b>unchanged</b>.</para>
        /// </summary>
        /// <typeparam name="T">Enumerable type of property in BO.</typeparam>
        /// <param name="propertyName">Property name in ClientData store.</param>
        /// <param name="propertyValue">Current BO's property value.</param>
        /// <param name="changed">Value updated to <b>true</b> if property value changed; 
        /// value <b>not changed</b> if property value is same. This allows iterative calls with
        /// minimal surrounded logic.</param>
        /// <returns>Updated or same property value.</returns>
        public T UpdateProperty<T>(string propertyName, T propertyValue, ref bool changed) where T : struct, IConvertible
        {
            T result = GetProperty<T>(propertyName, propertyValue);
            if (!propertyValue.Equals(result)) changed = true;
            return result;
        }

        /// <summary>
        /// Compares two time values with millisecond precision (JSON-able precision)
        /// </summary>
        private static bool isTimeEqual(DateTime one, DateTime two)
        {
            return (Math.Abs(two.Subtract(one).TotalMilliseconds) < 1.0);
        }

        /// <summary>
        /// Compares two nullable time values with millisecond precision (JSON-able precision)
        /// </summary>
        private static bool isTimeEqual(DateTime? one, DateTime? two)
        {
            if (!one.HasValue && !two.HasValue) return true;
            if (!one.HasValue || !two.HasValue) return false;
            return isTimeEqual(one.Value, two.Value);
        }
    }

    /// <summary>
    /// Interface implemented by class participating in client-server data exchange.
    /// </summary>
    public interface IClientDataProvider
    {
        /// <summary>
        /// Return dataset to be passed to client.
        /// </summary>
        ClientData GetClientData();
        /// <summary>
        /// Process dataset returned by client.
        /// </summary>
        /// <returns>true if processing resulted in a property value change.</returns>
        bool UpdateFromClient(ClientData data);
    }
}