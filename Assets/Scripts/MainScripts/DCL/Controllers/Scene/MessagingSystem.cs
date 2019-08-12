using System.Collections.Generic;
using UnityEngine;

namespace DCL
{
    public class MessagingTypes
    {
        public const string ENTITY_COMPONENT_CREATE = "UpdateEntityComponent";
        public const string ENTITY_CREATE = "CreateEntity";
        public const string ENTITY_REPARENT = "SetEntityParent";
        public const string ENTITY_COMPONENT_DESTROY = "ComponentRemoved";
        public const string SHARED_COMPONENT_ATTACH = "AttachEntityComponent";
        public const string SHARED_COMPONENT_CREATE = "ComponentCreated";
        public const string SHARED_COMPONENT_DISPOSE = "ComponentDisposed";
        public const string SHARED_COMPONENT_UPDATE = "ComponentUpdated";
        public const string ENTITY_DESTROY = "RemoveEntity";
        public const string SCENE_STARTED = "SceneStarted";
        public const string SCENE_LOAD = "LoadScene";
        public const string SCENE_UPDATE = "UpdateScene";
        public const string SCENE_DESTROY = "UnloadScene";
    }

    public class MessagingBusId
    {
        public const string UI = "UI";
        public const string INIT = "INIT";
        public const string SYSTEM = "SYSTEM";
    }


    public enum QueueMode
    {
        Reliable,
        Lossy,
    }

    public class MessagingSystem : System.IDisposable
    {
        public MessagingBus bus;
        public MessageThrottlingController throttler;
        public string id;

        float budgetMin = 0;

        Dictionary<string, LinkedListNode<MessagingBus.QueuedSceneMessage>> unreliableMessages = new Dictionary<string, LinkedListNode<MessagingBus.QueuedSceneMessage>>();
        System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
        public int unreliableMessagesReplaced = 0;

        public bool isThrottled
        {
            get { return throttler != null; }
        }

        public void Dispose()
        {
            bus.Dispose();
        }

        public float Update(float prevTimeBudget)
        {
            float timeBudget = 0;

            if (throttler != null)
            {
                timeBudget = bus.timeBudget = throttler.Update(
                    pendingMsgsCount: bus.pendingMessagesCount,
                    processedMsgsCount: bus.processedMessagesCount,
                    maxBudget: Mathf.Max(budgetMin, bus.budgetMax - prevTimeBudget)
                );
            }
            else
            {
                bus.timeBudget = Mathf.Max(budgetMin, bus.budgetMax - prevTimeBudget);
                timeBudget = bus.lastTimeConsumed;
            }

            return timeBudget;
        }

        public void Enqueue(MessagingBus.QueuedSceneMessage message, QueueMode queueMode = QueueMode.Reliable)
        {
            bool enqueued = true;

            if (queueMode == QueueMode.Reliable)
            {
                bus.pendingMessages.AddLast(message);
            }
            else
            {
                stringBuilder.Clear();

                LinkedListNode<MessagingBus.QueuedSceneMessage> node = null;

                stringBuilder.Append(message.tag);
                stringBuilder.Append(message.sceneId);

                string tag = stringBuilder.ToString();

                if (unreliableMessages.ContainsKey(tag))
                {
                    node = unreliableMessages[tag];

                    if (node.List != null)
                    {
                        node.Value = message;
                        enqueued = false;
                        unreliableMessagesReplaced++;
                    }
                }

                if (enqueued)
                {
                    node = bus.pendingMessages.AddLast(message);
                    unreliableMessages[tag] = node;
                }
            }

            if (enqueued)
            {
                if (message.type == MessagingBus.QueuedSceneMessage.Type.SCENE_MESSAGE)
                {
                    MessagingBus.QueuedSceneMessage_Scene sm = message as MessagingBus.QueuedSceneMessage_Scene;
                    SceneController.i.OnMessageWillQueue?.Invoke(sm.method);
                }
            }
        }

        public MessagingSystem(string id, IMessageHandler handler, float budgetMin = 0.01f, float budgetMax = 0.1f, bool enableThrottler = false)
        {
            this.id = id;
            this.budgetMin = budgetMin;
            this.bus = new MessagingBus(handler, budgetMax);

            if (enableThrottler)
                this.throttler = new MessageThrottlingController();
        }
    }
}

