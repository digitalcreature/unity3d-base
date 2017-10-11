using System;
using System.Collections.Generic;

namespace tgrehawi {

  public static partial class JSON {

    const string indent = "  ";

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
          Indent(emit, indentLevel);
          emit("]");
        }
      }
    }

    static void ToJSON(string value, Action<string> emit) {
      //  TODO: string generation
    }

    static void Indent(Action<string> emit, int indentLevel) {
      while (indentLevel > 0) {
        emit(indent);
        indentLevel --;
      }
    }

  }
}