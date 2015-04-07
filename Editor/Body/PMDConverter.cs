﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEditor;

namespace MMD.Body.Converter
{
    public class PMDConverter
    {
        MMD.Format.PMDFormat format;

        string directory;
        string filename;

        public PMDConverter(string path, float scale)
        {
            if (!path.ToLower().Contains(".pmd"))
                throw new System.ArgumentException("PMDファイル以外のファイルがロードされました");

            format = new MMD.Format.PMDFormat();
            format.Read(path, scale);

            directory = System.IO.Path.GetDirectoryName(path);
            filename = System.IO.Path.GetFileNameWithoutExtension(path);
        }

        GameObject CreateRoot()
        {
            var root = new GameObject(format.Header.modelName);
            var boneRoot = new GameObject("Bones");
            var rigidbodyRoot = new GameObject("Rigidbodies");

            boneRoot.transform.parent = root.transform;
            rigidbodyRoot.transform.parent = root.transform;

            return root;
        }

        void EntryMesh(Mesh mesh)
        {
            AssetDatabase.CreateAsset(mesh, directory + "/" + filename + ".asset");
        }

        void EntryMaterials(Material[] materials)
        {
            AssetDatabase.CreateFolder(directory, "Materials");
            for (int i = 0; i < materials.Length; ++i)
            {
                AssetDatabase.CreateAsset(materials[i], directory + "/Materials/材質" + (i + 1).ToString() + ".mat");
            }
        }

        void ConnectBones(GameObject[] bones, GameObject root)
        {
            bones[0].transform.parent = root.transform;
        }

        void EntryPrefab(GameObject root)
        {
            AssetDatabase.CreateAsset(root, directory + "/" + filename + ".prefab");
        }

        public void Import(Shader shader)
        {
            var root = CreateRoot();

            var renderer = root.AddComponent<SkinnedMeshRenderer>();
            var builder = new MMD.Builder.PMD.ModelBuilder(renderer);
            builder.Read(format, shader);

            // ここより下でアセットの登録を行う
            ConnectBones(builder.Bones, root);
            EntryMesh(builder.Mesh);
            EntryMaterials(builder.Materials);
            EntryPrefab(root);

            // 剛体のほうをいい加減取り組む


            /// TODO
            /// 剛体のAdapter/Builder部分を書く
            /// シェーダを書く
        }
    }
}
