using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static PublicChannelEntry;

public class PublicChannelEntry : BaseComponentView, IComponentModelConfig<PublicChannelEntryModel>
{
    [SerializeField] internal Button openChatButton;
    [SerializeField] internal TMP_Text nameLabel;
    [SerializeField] internal PublicChannelEntryModel model;
    [SerializeField] internal UnreadNotificationBadge unreadNotifications;
    
    private IChatController chatController;

    public PublicChannelEntryModel Model => model;

    public event Action<PublicChannelEntry> OnOpenChat;

    public static PublicChannelEntry Create()
    {
        return Instantiate(Resources.Load<PublicChannelEntry>("SocialBarV1/PublicChannelElement"));
    }

    public override void Awake()
    {
        base.Awake();
        openChatButton.onClick.AddListener(() => OnOpenChat?.Invoke(this));
    }

    public void Initialize(IChatController chatController)
    {
        this.chatController = chatController;
    }

    public void Configure(PublicChannelEntryModel newModel)
    {
        model = newModel;
        RefreshControl();
    }

    public override void RefreshControl()
    {
        nameLabel.text = $"#{model.name}";
        unreadNotifications.Initialize(chatController, model.channelId);
    }

    [Serializable]
    public class PublicChannelEntryModel : BaseComponentModel
    {
        public string channelId;
        public string name;

        public PublicChannelEntryModel(string channelId, string name)
        {
            this.channelId = channelId;
            this.name = name;
        }
    }
}