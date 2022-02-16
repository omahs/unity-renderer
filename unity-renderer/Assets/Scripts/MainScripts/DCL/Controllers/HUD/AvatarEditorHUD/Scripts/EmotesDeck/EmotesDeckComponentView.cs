using DCL;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EmotesDeck
{
    public interface IEmotesDeckComponentView
    {
        /// <summary>
        /// It will be triggered when an emote card is selected.
        /// </summary>
        event Action<string> onEmoteSelected;

        /// <summary>
        /// It will be triggered when an emote is equipped.
        /// </summary>
        event Action<string, int> onEmoteEquipped;

        /// <summary>
        /// It represents the container transform of the component.
        /// </summary>
        Transform emotesDeckTransform { get; }

        /// <summary>
        /// Resturn true if the view is currently active.
        /// </summary>
        bool isActive { get; }

        /// <summary>
        /// Get the current selected slot number.
        /// </summary>
        int selectedSlot { get; }

        /// <summary>
        /// Get the current selected card.
        /// </summary>
        EmoteCardComponentView selectedCard { get; }

        /// <summary>
        /// Set the emotes grid component with a list of emote cards.
        /// </summary>
        /// <param name="realms">List of emote cards (model) to be loaded.</param>
        void SetEmotes(List<EmoteCardComponentModel> emotes);

        /// <summary>
        /// Add a list of emotes in the emotes grid component.
        /// </summary>
        /// <param name="emotes">List of emote cards (model) to be added.</param>
        void AddEmotes(List<EmoteCardComponentModel> emotes);

        /// <summary>
        /// Set an emote as favorite or not.
        /// </summary>
        /// <param name="emoteId">Emote Id to update.</param>
        /// <param name="isFavorite">True for set it as favorite.</param>
        void SetEmoteAsFavorite(string emoteId, bool isFavorite);

        /// <summary>
        /// Assign an emote into a specific slot.
        /// </summary>
        /// <param name="emoteId">Emote Id to assign.</param>
        /// <param name="slotNumber">Slot number to assign the emote.</param>
        void EquipEmote(string emoteId, int slotNumber);
    }

    public class EmotesDeckComponentView : BaseComponentView, IEmotesDeckComponentView
    {
        internal const string EMOTE_CARDS_POOL_NAME = "EmotesDeck_EmoteCardsPool";
        internal const int EMOTE_CARDS_POOL_PREWARM = 40;
        internal const int DEFAULT_SELECTED_SLOT = 1;

        [Header("Assets References")]
        [SerializeField] internal EmoteCardComponentView emoteCardPrefab;

        [Header("Prefab References")]
        [SerializeField] internal EmoteSlotSelectorComponentView emoteSlotSelector;
        [SerializeField] internal EmoteSlotViewerComponentView emoteSlotViewer;
        [SerializeField] internal GridContainerComponentView emotesGrid;

        public event Action<string> onEmoteSelected;
        public event Action<string, int> onEmoteEquipped;

        internal Pool emoteCardsPool;

        public bool isActive => gameObject.activeInHierarchy;
        public Transform emotesDeckTransform => transform;
        public int selectedSlot => emoteSlotSelector.selectedSlot;
        public EmoteCardComponentView selectedCard { get; private set; }

        public override void Awake()
        {
            base.Awake();

            emoteSlotSelector.onSlotSelected += OnSlotSelected;

            ConfigureEmotesPool();
        }

        public override void OnEnable()
        {
            base.OnEnable();
        }

        public override void OnDisable()
        {
            base.OnDisable();
        }

        public override void Start()
        {
            base.Start();

            emoteSlotSelector.SelectSlot(DEFAULT_SELECTED_SLOT);
        }

        public override void RefreshControl()
        {
            emoteSlotSelector.RefreshControl();
            emoteSlotViewer.RefreshControl();
            emotesGrid.RefreshControl();
        }

        public override void Dispose()
        {
            base.Dispose();

            emoteSlotSelector.onSlotSelected -= OnSlotSelected;
        }

        public void SetEmotes(List<EmoteCardComponentModel> emotes)
        {
            emotesGrid.ExtractItems();
            emoteCardsPool.ReleaseAll();

            List<BaseComponentView> instantiatedEmotes = new List<BaseComponentView>();
            foreach (EmoteCardComponentModel emotesInfo in emotes)
            {
                EmoteCardComponentView emoteGO = InstantiateAndConfigureEmoteCard(emotesInfo);
                instantiatedEmotes.Add(emoteGO);
            }

            emotesGrid.SetItems(instantiatedEmotes);
        }

        public void AddEmotes(List<EmoteCardComponentModel> emotes)
        {
            List<BaseComponentView> instantiatedEmotes = new List<BaseComponentView>();
            foreach (EmoteCardComponentModel emotesInfo in emotes)
            {
                EmoteCardComponentView emoteGO = InstantiateAndConfigureEmoteCard(emotesInfo);
                instantiatedEmotes.Add(emoteGO);
            }

            foreach (var emote in instantiatedEmotes)
            {
                emotesGrid.AddItem(emote);
            }
        }

        public void SetEmoteAsFavorite(string emoteId, bool isFavorite)
        {
            if (string.IsNullOrEmpty(emoteId))
                return;

            EmoteCardComponentView emoteCardsToUpdate = GetEmoteCardById(emoteId);

            if (emoteCardsToUpdate != null)
                emoteCardsToUpdate.SetEmoteAsFavorite(isFavorite);
        }

        public void EquipEmote(string emoteId, int slotNumber)
        {
            if (string.IsNullOrEmpty(emoteId))
                return;

            EmoteCardComponentView emoteCardsToUpdate = GetEmoteCardById(emoteId);
            if (emoteCardsToUpdate != null && emoteCardsToUpdate.model.assignedSlot == slotNumber)
                return;

            List<EmoteCardComponentView> currentEmoteCards = GetAllEmoteCards();
            foreach (var existingEmoteCard in currentEmoteCards)
            {
                if (existingEmoteCard.model.assignedSlot == slotNumber)
                    existingEmoteCard.AssignSlot(-1);

                if (existingEmoteCard.model.id == emoteId)
                {
                    existingEmoteCard.AssignSlot(slotNumber);
                    emoteSlotSelector.AssignEmoteIntoSlot(slotNumber, emoteId, existingEmoteCard.model.pictureSprite);
                }

                existingEmoteCard.SetEmoteAsAssignedInSelectedSlot(existingEmoteCard.model.assignedSlot == slotNumber);
            }

            onEmoteEquipped?.Invoke(emoteId, slotNumber);
        }

        internal void ConfigureEmotesPool()
        {
            emoteCardsPool = PoolManager.i.GetPool(EMOTE_CARDS_POOL_NAME);
            if (emoteCardsPool == null)
            {
                emoteCardsPool = PoolManager.i.AddPool(
                    EMOTE_CARDS_POOL_NAME,
                    GameObject.Instantiate(emoteCardPrefab).gameObject,
                    maxPrewarmCount: EMOTE_CARDS_POOL_PREWARM,
                    isPersistent: true);

                emoteCardsPool.ForcePrewarm();
            }
        }

        internal EmoteCardComponentView InstantiateAndConfigureEmoteCard(EmoteCardComponentModel emotesInfo)
        {
            EmoteCardComponentView emoteGO = emoteCardsPool.Get().gameObject.GetComponent<EmoteCardComponentView>();
            emoteGO.Configure(emotesInfo);
            emoteGO.onMainClick.RemoveAllListeners();
            emoteGO.onMainClick.AddListener(() => OnEmoteSelected(emoteGO.model.id));
            emoteGO.onEquipClick.RemoveAllListeners();
            emoteGO.onEquipClick.AddListener(() => EquipEmote(emoteGO.model.id, selectedSlot));

            return emoteGO;
        }

        internal void OnSlotSelected(int slotNumber, string emoteId)
        {
            List<EmoteCardComponentView> currentEmoteCards = GetAllEmoteCards();
            foreach (var existingEmoteCard in currentEmoteCards)
            {
                existingEmoteCard.SetEmoteAsAssignedInSelectedSlot(existingEmoteCard.model.assignedSlot == slotNumber);
            }

            emoteSlotViewer.SetSelectedSlot(slotNumber);
        }

        internal void OnEmoteSelected(string emoteId)
        {
            selectedCard = GetEmoteCardById(emoteId);

            if (selectedCard.model.isSelected)
                return;

            List<EmoteCardComponentView> currentEmoteCards = GetAllEmoteCards();
            foreach (var existingEmoteCard in currentEmoteCards)
            {
                existingEmoteCard.SetEmoteAsSelected(existingEmoteCard.model.id == emoteId);
            }

            onEmoteSelected?.Invoke(emoteId);
        }

        internal List<EmoteCardComponentView> GetAllEmoteCards()
        {
            return emotesGrid
                .GetItems()
                .Select(x => x as EmoteCardComponentView)
                .ToList();
        }

        internal EmoteCardComponentView GetEmoteCardById(string emoteId)
        {
            return GetAllEmoteCards().FirstOrDefault(x => x.model.id == emoteId);
        }

        internal static IEmotesDeckComponentView Create()
        {
            EmotesDeckComponentView emotesDeckComponentView = Instantiate(Resources.Load<GameObject>("EmotesDeck/EmotesDeckSection")).GetComponent<EmotesDeckComponentView>();
            emotesDeckComponentView.name = "_EmotesDeckSection";

            return emotesDeckComponentView;
        }
    }
}