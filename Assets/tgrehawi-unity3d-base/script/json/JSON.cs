using System.Collections.Generic;
using System;
using System.Collections;

namespace tgrehawi {

  public static partial class JSON {

    public struct Value {

      public object value { get; set; }

      public static Value Null { get { return new Value(null); } }

      public string stringValue {
        get { return (string) value; }
        set { this.value = value; }
      }
      public float floatValue {
        get { return (float) value; }
        set { this.value = value; }
      }
      public int intValue {
        get { return (int) value; }
        set { this.value = value; }
      }
      public bool boolValue {
        get { return (bool) value; }
        set { this.value = value; }
      }

      public List listValue {
        get { return (List) value; }
        set { this.value = value; }
      }
      public Object objectValue {
        get { return (Object) value; }
        set { this.value = value; }
      }

      public bool isNull { get { return value == null; } }
      public bool isNumber {
        get {
          return value.GetType().IsPrimitive && !(
            value is char ||
            value is bool ||
            value is IntPtr ||
            value is UIntPtr
          );
        }
      }

      public Value(object value) {
        this.value = value is Value ? ((Value) value).value : value;
      }

      public static implicit operator string(Value value) { return value.stringValue; }
      public static implicit operator float(Value value) { return value.floatValue; }
      public static implicit operator int(Value value) { return value.intValue; }
      public static implicit operator bool(Value value) { return value.boolValue; }

      public static implicit operator List(Value value) { return value.listValue; }
      public static implicit operator Object(Value value) { return value.objectValue; }

      public T As<T>() {
        return (T) value;
      }

      public override bool Equals(object obj) {
        if (obj is Value) {
          return ((Value) obj).value == this.value;
        }
        else {
          return value == obj;
        }
      }

      public override int GetHashCode() { return this.value.GetHashCode(); }

      public static bool operator ==(Value a, object b) { return a.Equals(b); }
      public static bool operator !=(Value a, object b) { return !a.Equals(b); }

		}

    public class Object : IDictionary<string, Value>, IList<KeyValuePair<string, Value>> {

      List<KeyValuePair<string, Value>> pairs;

      public ICollection<string> Keys { get { return new KeyCollection(this); } }

      public ICollection<Value> Values { get { return new ValueCollection(this); } }

      public int Count { get { return pairs.Count; } }

      public bool IsReadOnly { get { return false; } }

      ICollection<string> IDictionary<string, Value>.Keys { get { return Keys; } }

      public KeyValuePair<string, Value> this[int index] {
        get {
          return pairs[index];
        }
        set {
          pairs[index] = NewPair(value.Key, value.Value);
          Validate();
        }
      }

      public Value this[string key] {
        get {
          if (key == null) throw new NullKeyException();
          foreach (var pair in pairs) {
            if (pair.Key == key) {
              return pair.Value;
            }
          }
          throw new NoSuchKeyException(this, key);
        }
        set {
          if (key == null) throw new NullKeyException();
          for (int i = 0; i < pairs.Count; i ++) {
            if (key == pairs[i].Key) {
              pairs[i] = NewPair(key, value);
              return;
            }
          }
          pairs.Add(NewPair(key, value));
        }
      }

      public Object() {
        pairs = new List<KeyValuePair<string, Value>>();
      }

      KeyValuePair<string, Value> NewPair(string key, object value) {
        if (key == null) throw new NullKeyException();
        if (!(value is Value)) {
          value = new Value(value);
        }
        return new KeyValuePair<string, Value>(key, new Value(value));
      }

      void Validate() {
        for (int i = 0; i < pairs.Count; i ++) {
          if (pairs[i].Key == null) throw new NullKeyException();
          for (int j = i + 1; j < pairs.Count; j ++) {
            if (pairs[i].Key == pairs[j].Key) {
              pairs.RemoveAt(j);
              j --;
            }
          }
        }
      }

      public bool ContainsKey(string key) {
        foreach (var pair in pairs) {
          if (pair.Key == key) {
            return true;
          }
        }
        return false;
      }

      public bool ContainsValue(object value) {
        foreach (var pair in pairs) {
          if (pair.Value.value == value) {
            return true;
          }
        }
        return false;
      }

      public void Add(string key, Value value) {
        Add(NewPair(key, value));
      }

      public bool Remove(string key) {
        for (int i = 0; i < pairs.Count; i ++) {
          if (pairs[i].Key == key) {
            pairs.RemoveAt(i);
            return true;
          }
        }
        return false;
      }

      public bool TryGetValue(string key, out Value value) {
        if (ContainsKey(key)) {
          value = this[key];
          return true;
        }
        else {
          value = Value.Null;
          return false;
        }
      }

      public void Add(KeyValuePair<string, Value> item) {
        this[item.Key] = item.Value;
      }

      public void Clear() {
        pairs.Clear();
      }

      public bool Contains(KeyValuePair<string, Value> item) {
        return ContainsKey(item.Key) && this[item.Key] == item.Value;
      }

      public void CopyTo(KeyValuePair<string, Value>[] array, int arrayIndex) {
        CopyTo(array, arrayIndex, Count, index => pairs[index]);
      }

      public bool Remove(KeyValuePair<string, Value> item) {
        return pairs.Remove(item);
      }

      public IEnumerator<KeyValuePair<string, Value>> GetEnumerator() {
        return pairs.GetEnumerator();
      }

      IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
      }

      static void CopyTo<T>(T[] array, int arrayIndex, int count, Func<int, T> get) {
        if (array == null) throw new ArgumentNullException();
        if (arrayIndex < 0) throw new ArgumentOutOfRangeException();
        if ((array.Length - arrayIndex) < count) throw new ArgumentException();
        for (int i = 0; i < count; i ++) {
          array[arrayIndex + i] = get(i);
        }
      }

      public int IndexOf(KeyValuePair<string, Value> item) {
        return pairs.IndexOf(item);
      }

      public void Insert(int index, KeyValuePair<string, Value> item) {
        pairs.Insert(index, NewPair(item.Key, item.Value));
        Validate();
      }

      public void RemoveAt(int index) {
        pairs.RemoveAt(index);
      }

      private abstract class ElementCollection<T> : ICollection<T> {

        public readonly Object obj;
        public int Count { get { return obj.Count; } }
        public bool IsReadOnly { get { return false; } }

        public ElementCollection(Object obj) {
          this.obj = obj;
        }

        public virtual void Add(T item) { throw new NotImplementedException(); }
        
        public void Clear() {
          obj.pairs.Clear();
        }

        public bool Contains(T item) {
          foreach (var pair in obj.pairs) {
            if (GetFromPair(pair).Equals(item)) {
              return true;
            }
          }
          return false;
        }

        public void CopyTo(T[] array, int arrayIndex) {
          Object.CopyTo(array, arrayIndex, Count, (index) => GetFromPair(obj.pairs[index]));
        }

        public IEnumerator<T> GetEnumerator() {
          foreach (var pair in obj.pairs) {
            yield return GetFromPair(pair);
          }
        }

        public abstract bool Remove(T item);

        IEnumerator IEnumerable.GetEnumerator() {
          return GetEnumerator();
        }

        protected abstract T GetFromPair(KeyValuePair<string, Value> pair);

      }

      private class KeyCollection : ElementCollection<string> {
        
        public KeyCollection(Object obj) : base(obj) {}

        public override void Add(string key) {
          obj.Add(obj.NewPair(key, null));
        }

        public override bool Remove(string key) {
          return obj.Remove(key);
        }

        protected override string GetFromPair(KeyValuePair<string, Value> pair) {
          return pair.Key;
        }

      }

      private class ValueCollection : ElementCollection<Value> {
        
        public ValueCollection(Object obj) : base(obj) {}

        public override bool Remove(Value value) {
          return obj.pairs.RemoveAll(pair => pair.Value == value) > 0;
        }

        protected override Value GetFromPair(KeyValuePair<string, Value> pair) {
          return pair.Value;
        }
      }

    }

    public class List : List<Value> {

      public List() : base() {}
      public List(IEnumerable<Value> elements) : base(elements) {}
      public List(int capacity) : base(capacity) {}

    }

  }

}