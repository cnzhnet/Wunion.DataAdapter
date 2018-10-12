using System;
using System.Collections.Generic;
using System.Text;
using System.Dynamic;
using System.ComponentModel;
using System.Collections;

namespace Wunion.DataAdapter.Kernel.DataCollection
{
    /// <summary>
    /// 表示动态数据实体对象类型.
    /// </summary>
    [Serializable]
    public class DynamicEntity : DynamicObject, IDictionary<string, object>, ICloneable, INotifyPropertyChanged
    {
        private IDictionary<string, object> Data;

        /// <summary>
        /// 创建一个 <see cref="DynamicEntity"/> 的对象实例.
        /// </summary>
        public DynamicEntity(Dictionary<string, object> entityData = null)
        {
            if (entityData == null)
                Data = new Dictionary<string, object>();
            else
                Data = entityData;
        }

        /// <summary>
        /// 用于通知属性值改变的事件.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 获取动态实体的所有键名称.
        /// </summary>
        public ICollection<string> Keys => Data.Keys;

        /// <summary>
        /// 获取动态实体的所有值.
        /// </summary>
        public ICollection<object> Values => Data.Values;

        /// <summary>
        /// 获以动态实体的属性成员数量.
        /// </summary>
        public int Count => Data.Count;

        /// <summary>
        /// 获取或设置是否为只读（未使用）.
        /// </summary>
        public bool IsReadOnly { get; set; }

        /// <summary>
        /// 用于获取动态实体中指定键的值的索引器.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object this[string key]
        {
            get { return Data[key]; }
            set
            {
                Data[key] = value;
                OnPropertyChanged(key);
            }
        }


        /// <summary>
        /// 设置动态属性.
        /// </summary>
        /// <param name="name">属性的名称.</param>
        /// <param name="value">属性值.</param>
        /// <param name="valueType">属性的数据类型.</param>
        public void SetPropertyValue(string name, object value, Type valueType)
        {
            if (value == null || value == DBNull.Value)
            {
                if (valueType.IsValueType)
                    value = System.Activator.CreateInstance(valueType);
                else
                    value = null;
            }
            else
            {
                value = Convert.ChangeType(value, valueType);
            }
            if (Data.ContainsKey(name))
                Data[name] = value;
            else
                Data.Add(name, value);
        }

        /// <summary>
        /// 获取指定的属性值.
        /// </summary>
        /// <typeparam name="T">属性值类型名称.</typeparam>
        /// <param name="name">属性名称.</param>
        /// <param name="defaultValue">当未找到该属性时返回的默认值.</param>
        /// <returns></returns>
        public object GetPropertyValue(string name, object defaultValue)
        {
            object val = null;
            if (!(Data.TryGetValue(name, out val)))
                return defaultValue;
            return val;
        }

        /// <summary>
        /// 获取指定的成员值.
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            return Data.TryGetValue(binder.Name, out result);
        }

        /// <summary>
        /// 设置指定成员的值。
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            if (Data.ContainsKey(binder.Name))
                Data[binder.Name] = value;
            else
                Data.Add(binder.Name, value);
            return true;
        }

        /// <summary>
        /// 调用指定的方法.
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="args"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            bool Okay = false;
            result = null;
            try
            {
                switch (binder.Name)
                {
                    case "SetProperty":
                        if (args == null || args.Length < 3)
                            Okay = false;
                        else
                            SetPropertyValue(args[0].ToString(), args[1], (Type)args[2]);
                        break;
                    case "GetProperty":
                        if (args == null || args.Length < 2)
                            Okay = false;
                        else
                            result = GetPropertyValue(args[0].ToString(), args[1]);
                        break;
                }
            }
            catch
            {
                Okay = false;
            }
            return Okay;
        }

        /// <summary>
        /// 将实体输出为字典.
        /// </summary>
        /// <param name="withOuts">不包含在输出字典中的实体属性名称.</param>
        /// <returns>返回一个包含实体数据的字典.</returns>
        public Dictionary<string, object> ToDictionary(params string[] withOuts)
        {
            Dictionary<string, object> Result = new Dictionary<string, object>();
            List<string> withOutList = new List<string>(withOuts);
            foreach (KeyValuePair<string, object> item in Data)
            {
                if (withOutList.Contains(item.Key))
                    continue;
                Result.Add(item.Key, item.Value);
            }
            return Result;
        }

        /// <summary>
        /// 添加值.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Add(string key, object value)
        {
            Data.Add(key, value);
        }

        /// <summary>
        /// 指定的键在动态实体中存在则返回 true， 否则返回 false.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey(string key)
        {
            return Data.ContainsKey(key);
        }

        /// <summary>
        /// 从动态实体中删除具有指定键名称（即属性名称）的成员.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Remove(string key)
        {
            return Data.Remove(key);
        }

        /// <summary>
        /// 获取与指定键关联的值.
        /// </summary>
        /// <param name="key">用于获取值的键.</param>
        /// <param name="value">用于输出值的缓冲区.</param>
        /// <returns></returns>
        public bool TryGetValue(string key, out object value)
        {
            return Data.TryGetValue(key, out value);
        }

        /// <summary>
        /// 向动态实体中添加一个键值对作为新成员.
        /// </summary>
        /// <param name="item">包含新成员键值对的对象.</param>
        public void Add(KeyValuePair<string, object> item)
        {
            Data.Add(item.Key, item.Value);
        }

        /// <summary>
        /// 清除动态实体中的所有成员.
        /// </summary>
        public void Clear()
        {
            Data.Clear();
        }

        /// <summary>
        /// 当指点定的键值对在动态实体中存在时返回 true，否则返回 false.
        /// </summary>
        /// <param name="item">包含成员键值对信息的对象.</param>
        /// <returns></returns>
        public bool Contains(KeyValuePair<string, object> item)
        {
            return Data.Contains(item);
        }

        /// <summary>
        /// 将动态实体的所有成员信息复制到给定的键值对对象数组中.
        /// </summary>
        /// <param name="array">所有成员复制到该键值对对象数.</param>
        /// <param name="arrayIndex">从数组开始复制的从零开始的索引.</param>
        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            Data.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// 从动态实体中删除指定键值对信息的成员.
        /// </summary>
        /// <param name="item">要删除的成员键值对信息.</param>
        /// <returns></returns>
        public bool Remove(KeyValuePair<string, object> item)
        {
            return Data.Remove(item);
        }

        /// <summary>
        /// 返回一个实体对象的遍历迭代器.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return Data.GetEnumerator();
        }

        /// <summary>
        /// 返回一个实体对象的遍历迭代器.
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return Data.GetEnumerator();
        }

        /// <summary>
        /// 克隆一个动态实体的对象副本.
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            IDictionary<string, object> clone = new DynamicEntity();
            ICloneable cloneable;
            foreach (string k in Data.Keys)
            {
                cloneable = Data[k] as ICloneable;
                if (cloneable == null)
                    clone[k] = Data[k];
                else
                    clone[k] = cloneable.Clone();
            }
            return clone;
        }

        /// <summary>
        /// 用于触发 <see cref="DynamicEntity.PropertyChanged"/> 事件.
        /// </summary>
        /// <param name="name">被更新的属性的名称.</param>
        protected virtual void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}
