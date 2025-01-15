using System.Collections.Generic;
using UnityEngine;

public class AnalyticsManager : Debuggable
{
    private SessionData _SessionData;

    #region Public Methods
    public void Initialize()
    {
        _SessionData = new SessionData();
        _SessionData.pins = new List<Vector3> ();
        _SessionData.landscapeItems = new List<SessionData.ViewTimeForItem>();
        _SessionData.freelookItems = new List<SessionData.ViewTimeForItem>();
        _SessionData.agentNotificationHistory = new List<string>();
        _SessionData.agentClicks = new List<string>();
        _SessionData.comparedItem = new List<string>();
        _SessionData.travelHistory = new List<string>();
        _SessionData.locationsAddedToPlan = new List<string>();
        _SessionData.addedToFavourites = new List<string>();
    }

    public void AddOrbitPin(Vector3 pin)
    {
        if (_SessionData.pins == null)
        {
            _SessionData.pins = new List<Vector3>();
        }
        _SessionData.pins.Add(pin);

        ShowDebugCall($"AnalyticsManager call-> AddOrbitPin: {{{pin.x}, {pin.y}, {pin.z}}}");
    }

    public void AppendTextToAiChatHistory(string textToAdd)
    {
        if (_SessionData.aiChatHistory == null)
        {
            _SessionData.aiChatHistory = "";
        }

        _SessionData.aiChatHistory += textToAdd;

        ShowDebugCall($"AnalyticsManager call-> AppendTextToAiChatHistory: {{{textToAdd}}}");
    }

    public void AddAiAudioClip(AudioClip audioClip)
    {
        _SessionData.aiAudioRecord = audioClip;

        ShowDebugCall($"AnalyticsManager call-> AddAiAudioClip: {{{audioClip.name}}}");
    }

    public void AddAiChatPlaytime(float addDuration)
    {
        _SessionData.aiUsageDuration += addDuration;

        ShowDebugCall($"AnalyticsManager call-> AddAiChatPlaytime: {{{addDuration}}}");
    }

    public void AddAiDiscardClick()
    {
        ++_SessionData.aiDiscardClicks;

        ShowDebugCall($"AnalyticsManager call-> AddAiDiscardClick.");
    }

    public void AddAiInteractionClick()
    {
        ++_SessionData.aiInteractionClicks;

        ShowDebugCall($"AnalyticsManager call-> AddAiInteractionClick.");
    }

    public void AddAiTextPrompt(string textPrompt)
    {
        _SessionData.aiTextPrompts += textPrompt;

        ShowDebugCall($"AnalyticsManager call-> AddAiTextPrompt: {{{textPrompt}}}");
    }

    public void AddAgentNotificationToHistory(string notificationString)
    {
        if (_SessionData.agentNotificationHistory == null)
        {
            _SessionData.agentNotificationHistory = new List<string>();
        }
        _SessionData.agentNotificationHistory.Add(notificationString);

        ShowDebugCall($"AnalyticsManager call-> AddAgentNotificationToHistory: {{{notificationString}}}");
    }

    public void AddAgentClick(string clickedItem)
    {
        if (_SessionData.agentClicks == null)
        {
            _SessionData.agentClicks = new List<string>();
        }
        _SessionData.agentClicks.Add(clickedItem);

        ShowDebugCall($"AnalyticsManager call-> AddAgentClick: {{{clickedItem}}}");
    }

    public void AddItemComparisonTime(float durationToAdd)
    {
        _SessionData.comparisonTimeMs += durationToAdd;

        ShowDebugCall($"AnalyticsManager call-> AddItemComparisonTime: {{{durationToAdd}}}");
    }

    public void AddComparedHotelToHistory(string hotelName)
    {
        _SessionData.comparedItem.Add(hotelName);

        ShowDebugCall($"AnalyticsManager call-> AddComparedHotelToHistory: {{{hotelName}}}");
    }

    public void AddTravelToHistory(string travelName)
    {
        _SessionData.travelHistory.Add(travelName);

        ShowDebugCall($"AnalyticsManager call-> AddTravelToHistory: {{{travelName}}}");
    }

    public void AddTravelHistoryViewTime(float durationToAdd)
    {
        _SessionData.travelHistoryViewTime += durationToAdd;

        ShowDebugCall($"AnalyticsManager call-> AddTravelHistoryViewTime: {{{durationToAdd}}}");
    }

    public void AddSharedItemCount()
    {
        ++_SessionData.itemSharedCount;

        ShowDebugCall($"AnalyticsManager call-> AddSharedItemCount.");
    }

    public void AddSharedItemsPlaytimeDuration(float durationToAdd)
    {
        _SessionData.sharedItemsPlaytimeDurationMs += durationToAdd;

        ShowDebugCall($"AnalyticsManager call-> AddSharedItemsPlaytimeDuration: {{{durationToAdd}}}");
    }

    public void AddItemToPlanning(string itemNameToAdd)
    {
        _SessionData.locationsAddedToPlan.Add(itemNameToAdd);

        ShowDebugCall($"AnalyticsManager call-> AddItemToPlanning: {{{itemNameToAdd}}}");
    }

    public void AddPlanningPlaytimeDuration(int durationToAdd)
    {
        _SessionData.planningPlaytimeDurationMs += durationToAdd;

        ShowDebugCall($"AnalyticsManager call-> AddPlanningPlaytimeDuration: {{{durationToAdd}}}");
    }

    public void AddItemToFavourites(string itemNameToAdd)
    {
        _SessionData.addedToFavourites.Add(itemNameToAdd);

        ShowDebugCall($"AnalyticsManager call-> AddItemToFavourites: {{{itemNameToAdd}}}");
    }

    public void AddFavouritesPlaytimeDuration(float durationToAdd)
    {
        _SessionData.favouritesPlaytimeDurationMs += durationToAdd;

        ShowDebugCall($"AnalyticsManager call-> AddFavouritesPlaytimeDuration: {{{durationToAdd}}}");
    }

    public void AddAgentResetCount()
    {
        ++_SessionData.agentResetCount;

        ShowDebugCall($"AnalyticsManager call-> AddAgentResetCount.");
    }

    public void SetNotifiedHotelId(string id)
    {
        _SessionData.notifiedHotelId = id;

        ShowDebugCall($"AnalyticsManager call-> SetNotifiedHotelId: {{{id}}}");
    }

    public void SetCloseComponentName(string componentName)
    {
        _SessionData.closeComponent = componentName;

        ShowDebugCall($"AnalyticsManager call-> SetCloseComponentName: {{{componentName}}}");
    }

    public void SetOpenComponenName(string componentName)
    {
        _SessionData.openComponent = componentName;

        ShowDebugCall($"AnalyticsManager call-> SetOpenComponenName: {{{componentName}}}");
    }

    public void SetSessionId(int sessionId)
    {
        _SessionData.sessionId = sessionId;

        ShowDebugCall($"AnalyticsManager call-> SetSessionId: {{{sessionId}}}");
    }

    public void AddSessionActivityDuration(float durationToAdd)
    {
        _SessionData.sessionActivityDurationMs += durationToAdd;

        ShowDebugCall($"AnalyticsManager call-> AddSessionActivityDuration: {{{durationToAdd}}}");
    }

    public void AddResetCount()
    {
        ++_SessionData.userResetCount;

        ShowDebugCall($"AnalyticsManager call-> AddResetCount.");
    }

    #endregion

    #region Internal Methods
    internal void EndSession()
    {
        ShowDebugCall($"AnalyticsManager call-> EndSession.");

        // Export data to datalake
        ExportData(_SessionData);

        // Clear data so it's ready for a new session
        _SessionData.Clear();
    }

    #endregion

    #region Private Methods

    private void ExportData(SessionData sessionData)
    {
        // Check Session Data requisites
        if (sessionData.sessionId < 0)
        {
            // TODO Miquel: Get current session Id if needed
        }
        //...

        // Call backend endpoint to export data
        // TODO Miquel

        ShowDebugCall($"AnalyticsManager call-> ExportData: {{{sessionData}}}");
    }

    #endregion

    public struct ItemViewTime
    {
        public Object item;
        public float time;
    }
}