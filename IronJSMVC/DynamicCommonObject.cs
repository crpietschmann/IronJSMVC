// Copyright (c) 2011 Chris Pietschmann - http://pietschsoft.com
// This work is licensed under a Creative Commons Attribution 3.0 Unites States License
// http://creativecommons.org/licenses/by/3.0/us/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using IronJS;

namespace IronJSMVC
{
    public class DynamicCommonObject : DynamicObject, IEnumerable, IEnumerable<KeyValuePair<string, object>>, IDictionary<string, object>
    {
        public DynamicCommonObject(CommonObject commonObject)
        {
            this.CommonObject = commonObject;
        }

        public CommonObject CommonObject { get; private set; }

        #region "DynamicObject Overrides"

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            var r = this.Keys.Contains(binder.Name) ? this[binder.Name] : null;

            result = getClrBoxedValue(r);

            if (result == null)
            {
                return false;
            }
            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            this.CommonObject.Put(binder.Name, value);
            return true;
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return this.CommonObject.Members.Keys;
        }

        #endregion

        #region "IEnumerable<KeyValuePair<string, object>> Implementation"

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return this.CommonObject.Members.Select(d => new KeyValuePair<string, object>(d.Key, getClrBoxedValue(d.Value))).AsEnumerable().GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

        #region "IDictionary Implementation"

        public void Add(string key, object value)
        {
            this.CommonObject.Put(key, value);
        }

        public bool ContainsKey(string key)
        {
            return this.CommonObject.Members.Keys.Contains(key);
        }

        public ICollection<string> Keys
        {
            get { return this.CommonObject.Members.Keys; }
        }

        public bool Remove(string key)
        {
            return this.CommonObject.Members.Remove(key);
        }

        public bool TryGetValue(string key, out object value)
        {
            object v;
            var retVal = this.CommonObject.Members.TryGetValue(key, out v);

            value = getClrBoxedValue(v);
            return retVal;
        }

        public ICollection<object> Values
        {
            get
            {
                return this.CommonObject.Members.Values.Select(d => getClrBoxedValue(d)).ToArray();
            }
        }

        public object this[string key]
        {
            get
            {
                return getClrBoxedValue(this.CommonObject.Get(key));
            }
            set
            {
                this.CommonObject.Put(key, value);
            }
        }

        public void Add(KeyValuePair<string, object> item)
        {
            this.CommonObject.Members.Add(item.Key, item.Value);
        }

        public void Clear()
        {
            this.CommonObject.Members.Clear();
        }

        public bool Contains(KeyValuePair<string, object> item)
        {
            return this.CommonObject.Members.Contains(item);
        }

        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get { return this.CommonObject.Members.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(KeyValuePair<string, object> item)
        {
            var v = this.CommonObject.Members[item.Key];
            if (v == item.Value)
            {
                return this.CommonObject.Members.Remove(item.Key);
            }
            return false;
        }

        #endregion

        public object this[int index]
        {
            get
            {
                return getClrBoxedValue(this.CommonObject.Get(index));
            }
            set
            {
                this.CommonObject.Put((uint)index, value);
            }
        }

        #region "Static Methods"

        private static object getClrBoxedValue(object obj)
        {
            if (obj is IronJS.BoxedValue)
            {
                return ((IronJS.BoxedValue)obj).ClrBoxed;
            }
            return obj;
        }

        #endregion
    }
}