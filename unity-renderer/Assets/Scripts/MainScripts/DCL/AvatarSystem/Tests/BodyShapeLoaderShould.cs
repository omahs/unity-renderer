﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using AvatarSystem;
using Cysharp.Threading.Tasks;
using DCL;
using DCL.Helpers;
using NSubstitute;
using NSubstitute.Core.Arguments;
using NSubstitute.Extensions;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

namespace Test.AvatarSystem
{
    public class BodyShapeLoaderShould
    {
        private const string FEMALE_ID = "urn:decentraland:off-chain:base-avatars:BaseFemale";
        private const string MALE_ID = "urn:decentraland:off-chain:base-avatars:BaseMale";
        private const string EYES_ID = "urn:decentraland:off-chain:base-avatars:eyes_00";
        private const string EYEBROWS_ID = "urn:decentraland:off-chain:base-avatars:eyebrows_00";
        private const string MOUTH_ID = "urn:decentraland:off-chain:base-avatars:mouth_00";
        private const string WEARABLE_SHADER_PATH = "DCL/Universal Render Pipeline/Lit";

        private BodyShapeLoader loader;
        private IRetrieverFactory retrieverFactory;
        private GameObject container = null;
        private readonly List<Material> materialsToBeDisposed = new List<Material>();

        private class BodyParts
        {
            public Renderer head;
            public Renderer ubody;
            public Renderer lbody;
            public Renderer feet;
            public Renderer eyes;
            public Renderer eyebrows;
            public Renderer mouth;
        }

        [SetUp]
        public void SetUp()
        {
            container = new GameObject("Container");
            PrepareCatalog();

            retrieverFactory = Substitute.For<IRetrieverFactory>();
            retrieverFactory.Configure().GetWearableRetriever().Returns(Substitute.For<IWearableRetriever>());
            retrieverFactory.Configure().GetFacialFeatureRetriever().Returns(Substitute.For<IFacialFeatureRetriever>());

            loader = new BodyShapeLoader(
                retrieverFactory,
                CatalogController.wearableCatalog[FEMALE_ID],
                CatalogController.wearableCatalog[EYES_ID],
                CatalogController.wearableCatalog[EYEBROWS_ID],
                CatalogController.wearableCatalog[MOUTH_ID]);
        }

        private void PrepareCatalog()
        {
            //This is really, really ugly. There's no other way to solve it until the catalog is in our service locator
            container.AddComponent<CatalogController>();
            AvatarAssetsTestHelpers.CreateTestCatalogLocal();
        }

        [UnityTest]
        public IEnumerator LoadBodyshape() => UniTask.ToCoroutine(async () =>
        {
            //Arrange
            BodyParts bodyParts = GetBodyshapeMock(container.transform);
            SetMaterial(bodyParts.lbody, WEARABLE_SHADER_PATH, "skin"); //lbody will be tinted as skin
            SetMaterial(bodyParts.ubody, WEARABLE_SHADER_PATH, "hair"); //ubody will be tinted as hair

            Rendereable rendereable = PrepareRendereable(bodyParts);
            loader.bodyshapeRetriever.rendereable.Returns(rendereable);
            loader.bodyshapeRetriever.Configure()
                  .Retrieve(Arg.Any<GameObject>(), Arg.Any<ContentProvider>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
                  .Returns(x => new UniTask<Rendereable>(rendereable));

            loader.eyesRetriever.Configure()
                  .Retrieve(Arg.Any<WearableItem>(), Arg.Any<string>())
                  .Returns(x => new UniTask<(Texture main, Texture mask)>((Texture2D.whiteTexture, null)));

            loader.eyebrowsRetriever.Configure()
                  .Retrieve(Arg.Any<WearableItem>(), Arg.Any<string>())
                  .Returns(x => new UniTask<(Texture main, Texture mask)>((Texture2D.whiteTexture, null)));

            loader.mouthRetriever.Configure()
                  .Retrieve(Arg.Any<WearableItem>(), Arg.Any<string>())
                  .Returns(x => new UniTask<(Texture main, Texture mask)>((Texture2D.whiteTexture, null)));

            //Act
            Color hairColor = Color.red;
            Color skinColor = Color.blue;
            Color eyesColor = Color.yellow;
            await loader.Load(container, new AvatarSettings
            {
                bodyshapeId = WearableLiterals.BodyShapes.MALE,
                hairColor = Color.red,
                skinColor = Color.blue,
                eyesColor = Color.yellow
            });

            //Assert
            Assert.AreEqual(IWearableLoader.Status.Succeeded, loader.status);
            Assert.AreEqual(Color.gray, bodyParts.head.material.color);
            Assert.AreEqual(hairColor, bodyParts.ubody.material.color);
            Assert.AreEqual(skinColor, bodyParts.lbody.material.color);
            Assert.AreEqual(Color.gray, bodyParts.feet.material.color);
            Assert.AreEqual(eyesColor, bodyParts.eyes.material.GetColor(AvatarSystemUtils._EyeTint));
            Assert.AreEqual(Texture2D.whiteTexture, bodyParts.eyes.material.GetTexture(AvatarSystemUtils._EyesTexture));
            Assert.AreEqual(hairColor, bodyParts.eyebrows.material.GetColor(AvatarSystemUtils._BaseColor));
            Assert.AreEqual(Texture2D.whiteTexture, bodyParts.eyebrows.material.GetTexture(AvatarSystemUtils._BaseMap));
            Assert.AreEqual(skinColor, bodyParts.mouth.material.GetColor(AvatarSystemUtils._BaseColor));
            Assert.AreEqual(Texture2D.whiteTexture, bodyParts.mouth.material.GetTexture(AvatarSystemUtils._BaseMap));
        });

        [UnityTest]
        public IEnumerator ThrowsIfCanceledTokenIsPassed() => UniTask.ToCoroutine(async () =>
        {
            //Arrange
            var cts = new CancellationTokenSource();
            cts.Cancel();

            // Act & Assert
            await TestUtils.ThrowsAsync<OperationCanceledException>(loader.Load(container, new AvatarSettings(), cts.Token));
        });

        [UnityTest]
        public IEnumerator ThrowsIfFailingBodyshape_FromThrow() => UniTask.ToCoroutine(async () =>
        {
            //Arrange
            loader.bodyshapeRetriever.Configure()
                  .Retrieve(Arg.Any<GameObject>(), Arg.Any<ContentProvider>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
                  .Returns(x => throw new Exception("ThrowingOnPurpose"));

            //Assert
            await TestUtils.ThrowsAsync<Exception>(loader.Load(container, new AvatarSettings()));
            loader.bodyshapeRetriever.Received().Dispose();
            loader.eyesRetriever.Received().Dispose();
            loader.eyebrowsRetriever.Received().Dispose();
            loader.mouthRetriever.Received().Dispose();
        });

        [UnityTest]
        public IEnumerator ThrowsIfFailingBodyshape_FromNull() => UniTask.ToCoroutine(async () =>
        {
            //Arrange
            loader.bodyshapeRetriever.rendereable.Returns(x => null);
            loader.bodyshapeRetriever.Configure()
                  .Retrieve(Arg.Any<GameObject>(), Arg.Any<ContentProvider>(), Arg.Any<string>(), Arg.Any<string>())
                  .Returns(x => new UniTask<Rendereable>(null));

            //Assert
            await TestUtils.ThrowsAsync<Exception>(loader.Load(container, new AvatarSettings()));
            loader.bodyshapeRetriever.Received().Dispose();
            loader.eyesRetriever.Received().Dispose();
            loader.eyebrowsRetriever.Received().Dispose();
            loader.mouthRetriever.Received().Dispose();
        });

        [UnityTest]
        public IEnumerator ThrowsIfFailingFacialFeature_FromThrow_Eyes() => UniTask.ToCoroutine(async () =>
        {
            //Arrange
            loader.eyesRetriever.Configure()
                  .Retrieve(Arg.Any<WearableItem>(), Arg.Any<string>())
                  .Returns(x => throw new Exception("ThrowingOnPurpose"));

            //Assert
            await TestUtils.ThrowsAsync<Exception>(loader.Load(container, new AvatarSettings()));
            loader.bodyshapeRetriever.Received().Dispose();
            loader.eyesRetriever.Received().Dispose();
            loader.eyebrowsRetriever.Received().Dispose();
            loader.mouthRetriever.Received().Dispose();
        });

        [UnityTest]
        public IEnumerator ThrowsIfFailingFacialFeature_FromThrow_Eyebrows() => UniTask.ToCoroutine(async () =>
        {
            //Arrange
            loader.eyebrowsRetriever.Configure()
                  .Retrieve(Arg.Any<WearableItem>(), Arg.Any<string>())
                  .Returns(x => throw new Exception("ThrowingOnPurpose"));

            //Assert
            await TestUtils.ThrowsAsync<Exception>(loader.Load(container, new AvatarSettings()));
            loader.bodyshapeRetriever.Received().Dispose();
            loader.eyesRetriever.Received().Dispose();
            loader.eyebrowsRetriever.Received().Dispose();
            loader.mouthRetriever.Received().Dispose();
        });

        [UnityTest]
        public IEnumerator ThrowsIfFailingFacialFeature_FromThrow_Mouth() => UniTask.ToCoroutine(async () =>
        {
            //Arrange
            loader.mouthRetriever.Configure()
                  .Retrieve(Arg.Any<WearableItem>(), Arg.Any<string>())
                  .Returns(x => throw new Exception("ThrowingOnPurpose"));

            //Assert
            await TestUtils.ThrowsAsync<Exception>(loader.Load(container, new AvatarSettings()));
            loader.bodyshapeRetriever.Received().Dispose();
            loader.eyesRetriever.Received().Dispose();
            loader.eyebrowsRetriever.Received().Dispose();
            loader.mouthRetriever.Received().Dispose();
        });

        [UnityTest]
        public IEnumerator ThrowsIfFailingFacialFeature_FromNull_Eyes() => UniTask.ToCoroutine(async () =>
        {
            //Arrange
            loader.eyesRetriever.Configure()
                  .Retrieve(Arg.Any<WearableItem>(), Arg.Any<string>())
                  .Returns(x => new UniTask<(Texture main, Texture mask)>((null, null)));

            //Assert
            await TestUtils.ThrowsAsync<Exception>(loader.Load(container, new AvatarSettings()));
            loader.bodyshapeRetriever.Received().Dispose();
            loader.eyesRetriever.Received().Dispose();
            loader.eyebrowsRetriever.Received().Dispose();
            loader.mouthRetriever.Received().Dispose();
        });

        [UnityTest]
        public IEnumerator ThrowsIfFailingFacialFeature_FromNull_Eyebrows() => UniTask.ToCoroutine(async () =>
        {
            //Arrange
            loader.eyebrowsRetriever.Configure()
                  .Retrieve(Arg.Any<WearableItem>(), Arg.Any<string>())
                  .Returns(x => new UniTask<(Texture main, Texture mask)>((null, null)));

            //Assert
            await TestUtils.ThrowsAsync<Exception>(loader.Load(container, new AvatarSettings()));
            loader.bodyshapeRetriever.Received().Dispose();
            loader.eyesRetriever.Received().Dispose();
            loader.eyebrowsRetriever.Received().Dispose();
            loader.mouthRetriever.Received().Dispose();
        });

        [UnityTest]
        public IEnumerator ThrowsIfFailingFacialFeature_FromNull_Mouth() => UniTask.ToCoroutine(async () =>
        {
            //Arrange
            loader.mouthRetriever.Configure()
                  .Retrieve(Arg.Any<WearableItem>(), Arg.Any<string>())
                  .Returns(x => new UniTask<(Texture main, Texture mask)>((null, null)));

            //Assert
            await TestUtils.ThrowsAsync<Exception>(loader.Load(container, new AvatarSettings()));
            loader.bodyshapeRetriever.Received().Dispose();
            loader.eyesRetriever.Received().Dispose();
            loader.eyebrowsRetriever.Received().Dispose();
            loader.mouthRetriever.Received().Dispose();
        });

        private Rendereable PrepareRendereable(BodyParts bodyparts)
        {
            return new Rendereable()
            {
                renderers = new List<Renderer>() { bodyparts.head, bodyparts.ubody, bodyparts.lbody, bodyparts.feet, bodyparts.eyes, bodyparts.eyebrows, bodyparts.mouth },
                container = container
            };
        }

        private BodyParts GetBodyshapeMock(Transform parent)
        {
            BodyParts bodyParts = new BodyParts()
            {
                head = GetPrimitiveMockingBodypart(parent, "head"),
                ubody = GetPrimitiveMockingBodypart(parent, "ubody"),
                lbody = GetPrimitiveMockingBodypart(parent, "lbody"),
                feet = GetPrimitiveMockingBodypart(parent, "feet"),
                eyes = GetPrimitiveMockingBodypart(parent, "eyes"),
                eyebrows = GetPrimitiveMockingBodypart(parent, "eyebrows"),
                mouth = GetPrimitiveMockingBodypart(parent, "mouth")
            };

            SetMaterial(bodyParts.head, WEARABLE_SHADER_PATH, "NormalMaterial");
            SetMaterial(bodyParts.ubody, WEARABLE_SHADER_PATH, "NormalMaterial");
            SetMaterial(bodyParts.lbody, WEARABLE_SHADER_PATH, "NormalMaterial");
            SetMaterial(bodyParts.feet , WEARABLE_SHADER_PATH, "NormalMaterial");
            //Facial features materials are set by the loader itself, no need to mock it here

            return bodyParts;
        }

        private Renderer GetPrimitiveMockingBodypart(Transform parent, string holderName)
        {
            GameObject holder = new GameObject(holderName);
            holder.transform.parent = parent;
            GameObject primitive = GameObject.CreatePrimitive(PrimitiveType.Cube);
            primitive.name = holderName;
            if (primitive.TryGetComponent(out Collider collider))
                Object.Destroy(collider);
            primitive.transform.parent = holder.transform;

            Renderer renderer = primitive.GetComponent<Renderer>();
            SkinnedMeshRenderer skr = primitive.AddComponent<SkinnedMeshRenderer>();
            skr.sharedMesh = primitive.GetComponent<MeshFilter>().sharedMesh;

            Object.Destroy(renderer);
            return skr;
        }

        private void SetMaterial(Renderer renderer, string shaderPath, string materialName)
        {
            Material material = new Material(Shader.Find(shaderPath)) { name = materialName };
            materialsToBeDisposed.Add(material);
            renderer.material = material;
        }

        [TearDown]
        public void TearDown()
        {
            loader?.Dispose();
            CatalogController.Clear();
            if (container != null)
                Object.Destroy(container);

            if (CatalogController.i != null)
                Object.Destroy(CatalogController.i);

            for (int i = 0; i < materialsToBeDisposed.Count; i++)
            {
                Material material = materialsToBeDisposed[i];
                if (material != null)
                    Object.Destroy(material);
            }
        }
    }
}