using UnityEngine;
using UnityEditor;

namespace tgrehawi {

  public class DisableModelMaterialImport : AssetPostprocessor {

    void OnPreprocessModel() {
      ModelImporter modelImporter = assetImporter as ModelImporter;
      modelImporter.importMaterials = false;
    }

  }

}