using System.Collections.Generic;
using UnityEngine;

internal struct SessionData
{
    // Orbit
    internal List<Vector3> pins;

    // AI Chat
    internal string aiChatHistory;
    internal AudioClip aiAudioRecord;
    internal float aiUsageDuration;
    internal int aiDiscardClicks;
    internal int aiInteractionClicks;
    internal string aiTextPrompts;

    // Landscape
    internal List<ViewTimeForItem> landscapeItems;
    internal int landscapeSendToAgentClicks;
    internal int landscapeDiscardClicks;
    internal int landscapeShareClicks;
    internal int landscapeFavouriteClicks;
    internal int landscapeAddToPlanificationClicks;

    // 360 Mode (Freelook mode)
    internal List<ViewTimeForItem> freelookItems;
    internal int freelookSendLocationToAgentClicks;
    internal int freelookDiscardLocationClicks;
    internal int freelookShareLocationClicks;
    internal int freelookFavouriteLocationClicks;
    internal int freelookAddLocationToPlanificationClicks;

    // Agent Inbox
    internal List<string> agentNotificationHistory;
    internal List<string> agentClicks;

    // Hotel Comparison
    internal List<string> comparedItem;
    internal float comparisonTimeMs;

    // Travel history
    internal List<string> travelHistory;
    internal float travelHistoryViewTime;

    // Sharing
    internal int itemSharedCount;
    internal float sharedItemsPlaytimeDurationMs;

    // Planning
    internal List<string> locationsAddedToPlan;
    internal float planningPlaytimeDurationMs;

    // Favourites
    internal List<string> addedToFavourites;
    internal float favouritesPlaytimeDurationMs;

    // Agent Inputs
    internal int agentResetCount;
    internal string notifiedHotelId;
    internal string closeComponent;
    internal string openComponent;

    // Additional
    internal int sessionId;
    internal float sessionActivityDurationMs;
    internal int userResetCount;

    #region Internal Methods
    internal void Clear()
    {
        // Orbit
        pins.Clear();

        // AI Chat
        aiChatHistory = "";
        aiAudioRecord = null;
        aiDiscardClicks = 0;
        aiInteractionClicks = 0;
        aiTextPrompts = "";
        aiUsageDuration = 0f;

        // Landscape
        landscapeItems.Clear();
        freelookSendLocationToAgentClicks = 0;
        freelookDiscardLocationClicks = 0;
        freelookShareLocationClicks = 0;
        freelookFavouriteLocationClicks = 0;
        freelookAddLocationToPlanificationClicks = 0;

        //Freelook
        freelookItems.Clear();
        freelookSendLocationToAgentClicks = 0;
        freelookDiscardLocationClicks = 0;
        freelookShareLocationClicks = 0;
        freelookFavouriteLocationClicks = 0;
        freelookAddLocationToPlanificationClicks = 0;

        // Agent Inbox
        agentNotificationHistory.Clear();
        agentClicks.Clear();

        // Hotel Comparison
        comparedItem.Clear();
        comparisonTimeMs = 0f;

        // Travel History
        travelHistory.Clear();
        travelHistoryViewTime = 0f;

        // Sharing History
        itemSharedCount = 0;
        sharedItemsPlaytimeDurationMs = 0f;

        // Planning
        locationsAddedToPlan.Clear();
        planningPlaytimeDurationMs = 0f;

        // Favourites
        addedToFavourites.Clear();
        favouritesPlaytimeDurationMs = 0f;

        // Agent Inputs
        agentResetCount = 0;
        notifiedHotelId = "";
        closeComponent = "";
        openComponent = ""  ;

        // Additional
        sessionId = -1;
        sessionActivityDurationMs = 0f;
        userResetCount = 0;
    }
    #endregion

    internal struct ViewTimeForItem
    {
        internal string itemId;
        internal float viewTime;
    }
}