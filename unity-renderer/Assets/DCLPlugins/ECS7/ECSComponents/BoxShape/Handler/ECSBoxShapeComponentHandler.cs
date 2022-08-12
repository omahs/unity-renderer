﻿using DCL.Controllers;
using DCL.ECSRuntime;
using DCL.Models;
using DCL.ECSComponents;
using Google.Protobuf.Collections;
using UnityEngine;

namespace DCL.ECSComponents
{
    public class ECSBoxShapeComponentHandler : IECSComponentHandler<PBBoxShape>
    {
        internal AssetPromise_PrimitiveMesh primitiveMeshPromisePrimitive;
        internal MeshesInfo meshesInfo;
        internal Rendereable rendereable;
        internal PBBoxShape lastModel;

        private GameObject renderingGameObject;

        private readonly DataStore_ECS7 dataStore;
        
        public ECSBoxShapeComponentHandler(DataStore_ECS7 dataStoreEcs7)
        {
            dataStore = dataStoreEcs7;
            renderingGameObject = new GameObject();
        }

        public void OnComponentCreated(IParcelScene scene, IDCLEntity entity)
        {
            renderingGameObject.transform.SetParent(entity.gameObject.transform, false);
        }

        public void OnComponentRemoved(IParcelScene scene, IDCLEntity entity)
        {
            if (primitiveMeshPromisePrimitive != null)
                AssetPromiseKeeper_PrimitiveMesh.i.Forget(primitiveMeshPromisePrimitive);
            DisposeMesh(entity, scene);
            
            GameObject.Destroy(renderingGameObject);
            lastModel = null;
        }

        public void OnComponentModelUpdated(IParcelScene scene, IDCLEntity entity, PBBoxShape model)
        {
            if (lastModel != null && lastModel.Equals(model))
                return;
            
            if (lastModel != null && lastModel.Uvs.Equals(model.Uvs))
            {
                ECSComponentsUtils.UpdateMeshInfo(entity, model.GetVisible(), model.GetWithCollisions(), model.GetIsPointerBlocker(), meshesInfo);
            }
            else
            {
                Mesh generatedMesh = null;
                if (primitiveMeshPromisePrimitive != null)
                    AssetPromiseKeeper_PrimitiveMesh.i.Forget(primitiveMeshPromisePrimitive);

                PrimitiveMeshModel primitiveMeshModelModel = new PrimitiveMeshModel(PrimitiveMeshModel.Type.Box);
                primitiveMeshModelModel.uvs = model.Uvs;
        
                primitiveMeshPromisePrimitive = new AssetPromise_PrimitiveMesh(primitiveMeshModelModel);
                primitiveMeshPromisePrimitive.OnSuccessEvent += shape =>
                {
                    DisposeMesh(entity,scene);
                    generatedMesh = shape.mesh;
                    GenerateRenderer(generatedMesh, scene, entity, model.GetVisible(), model.GetWithCollisions(), model.GetIsPointerBlocker());
                    dataStore.AddShapeReady(entity.entityId,meshesInfo.meshRootGameObject);
                    dataStore.RemovePendingResource(scene.sceneData.id, model);
                };
                primitiveMeshPromisePrimitive.OnFailEvent += ( mesh,  exception) =>
                {
                    dataStore.RemovePendingResource(scene.sceneData.id, model);
                };
            
                dataStore.AddPendingResource(scene.sceneData.id, model);
                AssetPromiseKeeper_PrimitiveMesh.i.Keep(primitiveMeshPromisePrimitive);
            }

            lastModel = model;
        }

        private void GenerateRenderer(Mesh mesh, IParcelScene scene, IDCLEntity entity, bool isVisible, bool withCollisions, bool isPointerBlocker)
        {
            meshesInfo = ECSComponentsUtils.GeneratePrimitive(entity, mesh, renderingGameObject, isVisible, withCollisions, isPointerBlocker);

            // Note: We should add the rendereable to the data store and dispose when it not longer exists
            rendereable = ECSComponentsUtils.AddRendereableToDataStore(scene.sceneData.id, entity.entityId, mesh, renderingGameObject, meshesInfo.renderers);
        }

        internal void DisposeMesh(IDCLEntity entity,IParcelScene scene)
        {
            if (meshesInfo != null)
            {
                dataStore.RemoveShapeReady(entity.entityId);
                ECSComponentsUtils.DisposeMeshInfo(meshesInfo);
            }
            if(rendereable != null)
                ECSComponentsUtils.RemoveRendereableFromDataStore( scene.sceneData.id,rendereable);
            if(lastModel != null)
                dataStore.RemovePendingResource(scene.sceneData.id, lastModel);
            
            meshesInfo = null;
            rendereable = null;
        }
    }
}