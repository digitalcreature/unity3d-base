using System;

namespace tgrehawi {

  public static partial class JSON {

    public class Exception : System.Exception {
      public Exception() {}
      public Exception(string message) : base(message) {}
      public Exception(string message, System.Exception inner) : base(message, inner) {}
    }

    public class ValueTypeException : Exception {

      public readonly Value value;

      public ValueTypeException(Value value, string message)
        : base(string.Format("Error: {0}\nValue: {1}", message, value)) {
        this.value = value;
      }
    }

    public class InvalidValueException : Exception {

      public readonly Value value;

      public InvalidValueException(Value value, string message)
        : base(string.Format("Error: {0}\nValue: {1}", message, value.value)) {
        this.value = value;
      }

    }

    public class NoSuchKeyException : Exception {

      public readonly Object obj;
      public readonly string key;

      public NoSuchKeyException(Object obj, string key)
        : base(string.Format("Object {0} does not contain a field for key {1}", obj, key)) {
        this.obj = obj;
        this.key = key;
      }

    }

    public class NullKeyException : Exception {

      public NullKeyException()
        : base("Cannot use null string for field key") {}
    }

    public class IndexOutOfBoundsException : Exception {

      public readonly int index;

      public IndexOutOfBoundsException(int index)
        : base(string.Format("Index out of bounds: {0}", index)) {
        this.index = index;
      }

    }

  }

}