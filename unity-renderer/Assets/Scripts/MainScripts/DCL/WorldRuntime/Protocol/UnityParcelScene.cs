using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace DCL.Models
{
    [Serializable]
    public struct CreateGlobalSceneMessage
    {
        public string id;
        public int sceneNumber;
        public string name;
        public string baseUrl;
        public List<ContentServerUtils.MappingPair> contents;
        public string icon;
        public bool isPortableExperience;
    }

    [Serializable]
    public class LoadParcelScenesMessage
    {
        public List<UnityParcelScene> parcelsToLoad = new List<UnityParcelScene>();

        /**
         * This class is received as an element of the list of parcels that need to be visible
         */
        [Serializable]
        public class UnityParcelScene
        {
            public static bool VERBOSE = false;
            
            // We can't get rid of this string id since it's used for mapping portable experience ids to their wearables...
            public string id;
            
            public int sceneNumber;
            public string baseUrl;
            public string baseUrlBundles;

            public List<ContentServerUtils.MappingPair> contents;

            public Vector2Int basePosition;
            public Vector2Int[] parcels;
        }
    }
}