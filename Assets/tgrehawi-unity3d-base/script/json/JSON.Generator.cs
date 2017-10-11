using System;
using System.Text;
using System.Collections.Generic;

namespace tgrehawi {

  public static partial class JSON {

    const string indent = "  ";

    public static string ToJSON(Value value, int indentLevel = 0) {
      StringBuilder builder = new StringBuilder();
      ToJSON(value, s => builder.Append(s), indentLevel);
      return builder.ToString();
    }

    static void ToJSON(Value value, Action<string> emit, int indentLevel = 0) {
      if (value.value is Object) {
        Object obj = value.objectValue;
        if (obj.Count == 0) {
          emit("{}");
        }
        else {
          emit("{\n");
          int i = 0;
          foreach (var pair in obj) {
            if (i > 0) {
              emit(",\n");
            }
            Indent(emit, indentLevel + 1);
            ToJSON(pair.Key, emit);
            emit(": ");
            ToJSON(pair.Value, emit, indentLevel + 1);
            i ++;
          }
          emit("\n");
          Indent(emit, indentLevel);
          emit("}");
        }
      }
      else if (value.value is List) {
        List list = value.listValue;
        if (list.Count == 0) {
          emit("[]");
        }
        else {
          emit("[\n");
          int i = 0;
          foreach (Value item in list) {
            if (i > 0) {
              emit(",\n");
            }
            Indent(emit, indentLevel + 1);
            ToJSON(item, emit, indentLevel + 1);
            i ++;
          }
          emit("\n");
          Indent(emit, indentLevel);
          emit("]");
        }
      }
      else if (value.value is string) {
        ToJSON(value.stringValue, emit);
      }
      else if (value.value is bool) {
        if (value.boolValue) {
          emit("true");
        }
        else {
          emit("false");
        }
      }
      else if (value.isNull) {
        emit("null");
      }
      else if (value.value is char) {
        ToJSON(value.As<char>().ToString(), emit);
      }
      else if (value.isNumber) {
        emit(value.value.ToString());
      }
      else {
        throw new InvalidValueException(value, "Cannot generate JSON for value");
      }
    }

    static void ToJSON(string value, Action<string> emit) {
      emit("\"");
      foreach (char c in value) {
        switch (c) {
          case '"':
            emit("\\\"");
            break;
          case '\\':
            emit("\\\\");
            break;
          case '\b':
            emit("\\b");
            break;
          case '\f':
            emit("\\f");
            break;
          case '\n':
            emit("\\n");
            break;
          case '\r':
            emit("\\r");
            break;
          case '\t':
            emit("\\t");
            break;
          default:
            emit(c.ToString());
            break;
        }
      }
      emit("\"");
    }

    static void Indent(Action<string> emit, int indentLevel) {
      while (indentLevel > 0) {
        emit(indent);
        indentLevel --;
      }
    }

  }
}