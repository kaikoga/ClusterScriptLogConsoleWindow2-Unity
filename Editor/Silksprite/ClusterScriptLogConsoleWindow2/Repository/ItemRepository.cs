using System.Collections.Generic;
using System.Linq;
using ClusterVR.CreatorKit.Item;
using ClusterVR.CreatorKit.Item.Implements;
using Silksprite.ClusterScriptLogConsoleWindow2.Access;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Item = ClusterVR.CreatorKit.Item.Implements.Item;

namespace Silksprite.ClusterScriptLogConsoleWindow2.Repository
{
    public sealed class ItemRepository
    {
        public static readonly ItemRepository Instance = new();
        readonly Dictionary<ulong, int> cachedItemInstanceIdsByItemId = new();
        readonly Dictionary<string, int> cachedItemInstanceIdsByName = new();
        bool collectedItemNamesInScene = false;
        bool collectedItemNamesInTemplate = false;
        bool collectedItemNamesInProject = false;

        public void ClearCache()
        {
            cachedItemInstanceIdsByItemId.Clear();
            cachedItemInstanceIdsByName.Clear();
            collectedItemNamesInScene = false;
            collectedItemNamesInTemplate = false;
            collectedItemNamesInProject = false;
        }

        public Item FindItemSlow(ulong itemId, string itemName)
        {
            Item[] sceneItems = null;
            if (itemId != 0)
            {
                if (TryGetCachedItem(itemId, out var item))
                {
                    return item;
                }  
                sceneItems = AllItemsInScenesSlow().ToArray();
                item = sceneItems.FirstOrDefault(i => i.Id.Value == itemId);
                cachedItemInstanceIdsByItemId[itemId] = item?.GetInstanceID() ?? 0;
                if (item)
                {
                    return item;
                } 
            }

            if (!string.IsNullOrEmpty(itemName))
            {
                if (TryGetCachedItem(itemName, out var item))
                {
                    return item;
                }  
                sceneItems ??= AllItemsInScenesSlow().ToArray();
                item = DoFindItemByNameSlow(itemName, sceneItems);
                if (item)
                {
                    return item;
                }
            }
            
            return null;
        }

        public JavaScriptAsset FindSourceCodeAssetSlow(ulong itemId, string itemName, bool isPlayerScript)
        {
            var item = FindItemSlow(itemId, itemName);
            switch (isPlayerScript)
            {
                case false:
                {
                    var scriptableItem = item?.gameObject.GetComponent<ScriptableItem>();
                    if (scriptableItem == null)
                    {
                        return null;
                    }
                    using var scriptableItemAccess = new ScriptableItemAccess(scriptableItem);
                    return scriptableItemAccess.sourceCodeAsset;
                }
                case true:
                {
                    var playerScript = item?.gameObject.GetComponent<PlayerScript>();
                    if (playerScript == null)
                    {
                        return null;
                    }
                    using var playerScriptAccess = new PlayerScriptAccess(playerScript);
                    return playerScriptAccess.sourceCodeAsset;
                }
            }
        }

        Item DoFindItemByNameSlow(string itemName, Item[] sceneItems)
        {
            if (!collectedItemNamesInScene)
            {
                CollectItemNames(sceneItems);
                collectedItemNamesInScene = true;
            }
            if (TryGetCachedItem(itemName, out var item))
            {
                return item;
            }  
            if (!collectedItemNamesInTemplate)
            {
                CollectItemNames(AllItemsInTemplatesSlow(sceneItems));
                collectedItemNamesInTemplate = true;
            }
            if (TryGetCachedItem(itemName, out item))
            {
                return item;
            }  
            if (!collectedItemNamesInProject)
            {
                CollectItemNames(AllItemsInProjectSlow());
                collectedItemNamesInProject = true;
            }
            if (TryGetCachedItem(itemName, out item))
            {
                return item;
            }  
            cachedItemInstanceIdsByName[itemName] = 0;
            return null;
        }

        bool TryGetCachedItem(ulong itemId, out Item item)
        {
            if (cachedItemInstanceIdsByItemId.TryGetValue(itemId, out var instanceId))
            {
                item = EditorUtility.InstanceIDToObject(instanceId) as Item;
                return true;
            }
            item = null;
            return false;
        }

        bool TryGetCachedItem(string itemName, out Item item)
        {
            if (cachedItemInstanceIdsByName.TryGetValue(itemName, out var instanceId))
            {
                item = EditorUtility.InstanceIDToObject(instanceId) as Item;
                return true;
            }
            item = null;
            return false;
        }

        void CollectItemNames(IEnumerable<Item> items)
        {
            foreach (var item in items.Where(item => !string.IsNullOrEmpty(((IItem)item).ItemName)))
            {
                cachedItemInstanceIdsByName[((IItem)item).ItemName] = item.GetInstanceID();
            }
        }
            
        static IEnumerable<Item> AllItemsInScenesSlow()
        {
            return Enumerable.Range(0, SceneManager.sceneCount)
                .Select(SceneManager.GetSceneAt)
                .SelectMany(scene => scene.GetRootGameObjects())
                .SelectMany(gameObject => gameObject.GetComponentsInChildren<Item>());
        }

        static IEnumerable<Item> AllItemsInTemplatesSlow(IEnumerable<Item> sceneItems)
        {
            return sceneItems
                .Select(item => item.GetComponent<IItemTemplateContainer>())
                .Where(itemTemplateContainer => itemTemplateContainer != null)
                .SelectMany(itemTemplateContainer => itemTemplateContainer.ItemTemplates())
                .Select(itemTemplate => itemTemplate.Item)
                .OfType<Item>()
                .Distinct();
        }

        static IEnumerable<Item> AllItemsInProjectSlow()
        {
            var guids = AssetDatabase.FindAssets("t:Prefab", null);

            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (prefab.TryGetComponent<Item>(out var item))
                {
                    yield return item;
                }
            }
        }
    }
}
